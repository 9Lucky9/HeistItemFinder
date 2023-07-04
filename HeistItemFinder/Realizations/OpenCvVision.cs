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
    internal class OpenCvVision : IOpenCvVision
    {
        private Mat _image;
        private readonly Mat _template;

        private readonly Mat _temporary;

        private int _originalTemplateHeight;
        private int _originaltemplateWidth;

        public OpenCvVision()
        {
            _temporary = new Mat();
            _template = new Mat(
                AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\heist-lock.bmp");
        }

        //TODO: Improve processing algorithm.
        /// <summary>
        /// Process image to prepare it for text recognition.
        /// </summary>
        /// <param name="image">Original image.</param>
        /// <returns>Ready to text recognition image's.</returns>
        public List<Bitmap> ProcessImage(Image image)
        {
            try
            {
                _image = new Bitmap(image).ToMat();

                var allMatches = FindAllMatches();
                //Cv2.ImShow("Test", _image);
                //Cv2.WaitKey();

                //Group matches by identical Y coordinate.
                //Then select groups where 2 points are present.
                var groups = allMatches
                    .GroupBy(x => x.Y)
                    .Where(x => x.Count() == 2);
                if(!groups.Any())
                    throw new NoTemplateMatchesException(
                        "No matches on the image were found, try again.");

                var min = new OpenCvSharp.Point();
                var max = new OpenCvSharp.Point();

                foreach(var group in groups)
                {
                    //TODO: fill with points
                }

                var points = groups.First();
                var firstPoint = points.First();
                var lastPoint = points.Last();
                if (firstPoint.X < lastPoint.X)
                {
                    min = firstPoint;
                    max = lastPoint;
                }
                else
                {
                    min = lastPoint;
                    max = firstPoint;
                }
                

                //Crop image
                var rectWidth = max.X - min.X + _originaltemplateWidth;
                //var tabHeight = min.Y + _originalTemplateHeight;
                var rectHeight = _originalTemplateHeight;

                //Increase height of starting point by template height.
                min.Y -= _template.Height;

                var rect = new Rect(
                    min,
                    new OpenCvSharp.Size(rectWidth, rectHeight));

                var result = _image[rect];

                //TODO: Convert
                foreach(var point in points)
                {

                }
                var resultGray = result.CvtColor(ColorConversionCodes.BGR2GRAY);
                var binarized = resultGray.Threshold(86, 255, ThresholdTypes.Binary);
                Cv2.ImShow("Test", binarized);
                Cv2.WaitKey();
                var bitmap = binarized.ToBitmap();

                return bitmap;
            }
            catch(Exception ex)
            {
                throw new ImageNotRecognizedException(
                    "Image processing have failed. Probably due to bad image source.");
            }

        }

        /// <summary>
        /// Find all templates in the image
        /// by step downscaling to the template size.
        /// </summary>
        private List<OpenCvSharp.Point> FindAllMatches()
        {
            var scale = 100f;
            var imageGray = _image.CvtColor(ColorConversionCodes.BGR2GRAY);
            var templateGray = _template.CvtColor(ColorConversionCodes.BGR2GRAY);
            var templateMatchPoints = new List<OpenCvSharp.Point>();
            var matchCount = 0;
            while (true)
            {
                Cv2.MatchTemplate(imageGray, templateGray, _temporary, TemplateMatchModes.CCoeffNormed);
                double threshold = 0.8;
                double minval, maxval = 0.0;
                OpenCvSharp.Point minloc, maxloc;
                _temporary.MinMaxLoc(out minval, out maxval, out minloc, out maxloc);
                if (maxval >= threshold)
                {
                    matchCount++;
                    //Match points on the scaled image with original image.
                    var originalWidthPoint = maxloc.X * (100 / scale);
                    var originalHeightPoint = maxloc.Y * (100 / scale);
                    var originalPoint = new OpenCvSharp.Point(
                        originalWidthPoint, originalHeightPoint);
                    templateMatchPoints.Add(originalPoint);

                    _originalTemplateHeight = Convert.ToInt32(_template.Height * 100 / scale);
                    _originaltemplateWidth = Convert.ToInt32(_template.Width * 100 / scale);
                    //DrawRectanglesOnImage(originalPoint, originalWidthPoint, scale);
                    //Cv2.ImShow("Test", _image);
                    //Cv2.WaitKey();

                    //Draw black rectangle on found match to not find it again.
                    var blackRect = new Rect(
                        new OpenCvSharp.Point(maxloc.X, maxloc.Y),
                        new OpenCvSharp.Size(
                            _template.Width,
                            _template.Height));
                    imageGray.Rectangle(blackRect, Scalar.Black, -1);
                }
                //If threshold is not passed downscale image by 1/10.
                else
                {
                    if (scale <= 10)
                    {
                        break;
                    }
                    scale -= 10f;
                    var widthResized = _image.Width * (scale / 100);
                    var heightResized = _image.Height * (scale / 100);
                    var reSize = new OpenCvSharp.Size(widthResized, heightResized);
                    imageGray = imageGray.Resize(reSize);
                }
            }
            //Cv2.ImShow("result", Image);
            //Cv2.WaitKey();
            return templateMatchPoints;
        }

        /// <summary>
        /// Draw rectangles on the original image.
        /// For test purposes only.
        /// </summary>
        private void DrawRectanglesOnImage(
            OpenCvSharp.Point originalPoint,
            float originalWidthPoint,
            float scale)
        {
            var r = new Rect(
                originalPoint,
                new OpenCvSharp.Size(
                    _template.Width * 100 / scale,
                    _template.Height * 100 / scale));

            _image.Rectangle(r, Scalar.Red);
            _image.PutText(
                $"{originalWidthPoint}", 
                originalPoint, 
                HersheyFonts.HersheyPlain, 2.0, Scalar.Green);
            Cv2.ImShow("Test", _image);
            Cv2.WaitKey();
        }
    }
}
