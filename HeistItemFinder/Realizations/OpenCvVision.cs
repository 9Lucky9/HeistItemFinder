using HeistItemFinder.Exceptions;
using HeistItemFinder.Interfaces;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HeistItemFinder.Realizations
{
    public class OpenCvVision : IOpenCvVision
    {
        public OpenCvVision()
        {
        }

        //TODO: Improve processing algorithm.
        /// <summary>
        /// Process image to prepare it for text recognition.
        /// </summary>
        /// <param name="source">Original image.</param>
        /// <returns>Ready to text recognition image's.</returns>
        public List<Bitmap> ProcessImage(Image source)
        {
            try
            {
                using var imageMat = new Bitmap(source).ToMat();
                using var templateMat = new Mat(
                AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\heist-lock.bmp");

                var allMatches = FindAllMatches(imageMat, templateMat);
                //Cv2.ImShow("Test", _image);
                //Cv2.WaitKey();

                //Group matches by Y coordinate.
                //By selecting points where Y coordinate is within small deviation (around 2 pixels).
                var grouppedPoints = new List<Tuple<ImageTemplatePoint, ImageTemplatePoint>>();
                for (int i = 0; i < allMatches.Count; i++)
                {
                    for(int j = i + 1; j < allMatches.Count; j++)
                    {
                        var deviationY = Math.Abs(
                            allMatches[i].ImagePoint.Y - allMatches[j].ImagePoint.Y);
                        if (deviationY <= 2)
                        {
                            var tuple = new Tuple<ImageTemplatePoint, ImageTemplatePoint>(
                                allMatches[i],
                                allMatches[j]);

                            grouppedPoints.Add(tuple);
                            i += 1;
                            j += 1;
                        }
                    }
                }

                if (!grouppedPoints.Any())
                    throw new NoTemplateMatchesException(
                        "No matches on the image were found, try again.");

                var resultImages = new List<Bitmap>();
                foreach (var group in grouppedPoints)
                {
                    var firstMatch = group.Item1;
                    var secondMatch = group.Item2;
                    var min = new OpenCvSharp.Point();
                    var max = new OpenCvSharp.Point();

                    if (firstMatch.ImagePoint.X < secondMatch.ImagePoint.X)
                    {
                        min = firstMatch.ImagePoint;
                        max = secondMatch.ImagePoint;
                    }
                    else
                    {
                        min = secondMatch.ImagePoint;
                        max = firstMatch.ImagePoint;
                    }

                    //Increase height of starting point by template height
                    //because gems have smaller box.
                    min.Y -= templateMat.Height;

                    //Size of rectangle
                    var rectWidth = max.X - min.X + firstMatch.TemplatePoint.X;
                    //Multiply by 2 because replica's have much higher box.
                    var rectHeight = firstMatch.TemplatePoint.Y * 2;

                    var rect = new Rect(
                        min,
                        new OpenCvSharp.Size(rectWidth, rectHeight));

                    using var result = imageMat[rect];
                    using var resultGray = result.CvtColor(ColorConversionCodes.BGR2GRAY);
                    using var binarized = resultGray.Threshold(86, 255, ThresholdTypes.Binary);
                    var bitmap = binarized.ToBitmap();
                    resultImages.Add(bitmap);
                    Cv2.ImShow("Test", binarized);
                    Cv2.WaitKey();
                }
                return resultImages;
            }
            catch (Exception)
            {
                throw new ImageNotRecognizedException(
                    "Image processing have failed. Probably due to bad image source.");
            }
        }

        /// <summary>
        /// Find all templates in the image
        /// by step downscaling to the template size.
        /// </summary>
        /// <returns>List of location on the image, and scale of resizing</returns>
        private List<ImageTemplatePoint> FindAllMatches(Mat source, Mat template)
        {
            var scale = 100f;
            using var temporary = new Mat();
            using var imageGray = source.CvtColor(ColorConversionCodes.BGR2GRAY);
            using var templateGray = template.CvtColor(ColorConversionCodes.BGR2GRAY);
            var templateMatchPoints = new List<ImageTemplatePoint>();
            while (true)
            {
                Cv2.MatchTemplate(imageGray, templateGray, temporary, TemplateMatchModes.CCoeffNormed);
                double threshold = 0.8;
                double minval, maxval = 0.0;
                OpenCvSharp.Point minloc, maxloc;
                temporary.MinMaxLoc(out minval, out maxval, out minloc, out maxloc);
                if (maxval >= threshold)
                {
                    //Match point on the downscaled image with original image.
                    var originalWidthPoint = maxloc.X * (100 / scale);
                    var originalHeightPoint = maxloc.Y * (100 / scale);
                    var originalImagePoint = new OpenCvSharp.Point(
                        originalWidthPoint, 
                        originalHeightPoint);

                    //Match point on the template with upscaled template.
                    var templateUpscaled = new OpenCvSharp.Point(
                        template.Width * (100 / scale),
                        template.Height * (100 / scale));

                    templateMatchPoints.Add(
                        new ImageTemplatePoint(
                            originalImagePoint, 
                            templateUpscaled));

                    //DrawRectangleOnPoint(source, template, originalImagePoint, scale);

                    //Draw black rectangle on found match to not find it again.
                    var blackRect = new Rect(
                        new OpenCvSharp.Point(maxloc.X, maxloc.Y),
                        new OpenCvSharp.Size(
                            template.Width,
                            template.Height));
                    imageGray.Rectangle(blackRect, Scalar.Black, -1);
                    //Cv2.ImShow("result", imageGray);
                    //Cv2.WaitKey();
                }
                //If threshold is not passed downscale image by 1/10.
                else
                {
                    if (scale <= 10)
                    {
                        break;
                    }
                    scale -= 10f;
                    var widthResized = source.Width * (scale / 100);
                    var heightResized = source.Height * (scale / 100);
                    var resize = new OpenCvSharp.Size(widthResized, heightResized);
                    Cv2.Resize(imageGray, imageGray, resize);
                }
            }

            return templateMatchPoints;
        }

        private class ImageTemplatePoint
        {
            /// <summary>
            /// Image point.
            /// </summary>
            public OpenCvSharp.Point ImagePoint { get; set; }

            /// <summary>
            /// Upscaled template point to image.
            /// </summary>
            public OpenCvSharp.Point TemplatePoint { get; set; }

            public ImageTemplatePoint(OpenCvSharp.Point image, OpenCvSharp.Point template)
            {
                ImagePoint = image;
                TemplatePoint = template;
            }
        }

        /// <summary>
        /// FOR TEST PURPOSES ONLY.
        /// Draw rectangles on finded point on the original image.
        /// </summary>
        private void DrawRectangleOnPoint(
            Mat source,
            Mat template,
            OpenCvSharp.Point originalPoint,
            float scale)
        {
            var r = new Rect(
                originalPoint,
                new OpenCvSharp.Size(
                    template.Width * 100 / scale,
                    template.Height * 100 / scale));

            source.Rectangle(r, Scalar.Red);
            source.PutText(
                $"X:{originalPoint.X}, Y: {originalPoint.Y}", 
                originalPoint, 
                HersheyFonts.HersheyPlain, 2.0, Scalar.Green);
            Cv2.ImShow("Test", source);
            Cv2.WaitKey();
        }
    }
}
