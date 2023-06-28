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
        private Mat _template;

        private readonly Mat _temporary;

        private int _originalTemplateHeight;
        private int _originaltemplateWidth;

        public OpenCvVision()
        {
            _temporary = new Mat();
        }

        /// <summary>
        /// Return image that is ready for text recognition.
        /// </summary>
        /// <param name="image">Original image.</param>
        /// <param name="template">Template.</param>
        public Bitmap ProcessImage(Image image, Image template)
        {
            _image = new Bitmap(image).ToMat();
            _template = new Bitmap(template).ToMat();

            var allMatches = FindAllMatches();
            //Cv2.ImShow("Test", _image);
            //Cv2.WaitKey();
            var groups = allMatches.GroupBy(x => x.Y);
            
            OpenCvSharp.Point min = new OpenCvSharp.Point();
            OpenCvSharp.Point max = new OpenCvSharp.Point();
            foreach (var group in groups)
            {
                if (group.Count() == 2)
                {
                    var firstPoint = group.First();
                    var lastPoint = group.Last();
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
                }
            }

            //Crop image
            var tabHeight = min.Y + _originalTemplateHeight;
            var tabWidth = max.X - min.X + _originaltemplateWidth;

            min.Y -= _originalTemplateHeight;
            var rect = new Rect(
                min,
                new OpenCvSharp.Size(tabWidth, tabHeight));
            var result = _image[rect];

            var resultGray = result.CvtColor(ColorConversionCodes.BGR2GRAY);
            var binarized = resultGray.Threshold(85, 255, ThresholdTypes.Binary);
            //Cv2.ImShow("Test", binarized);
            //Cv2.WaitKey();
            var bitmap = binarized.ToBitmap();

            return bitmap;
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
                    var originalWidthPoint = maxloc.X * (100 / scale);
                    var originalHeightPoint = maxloc.Y * (100 / scale);
                    var originalPoint = new OpenCvSharp.Point(
                        originalWidthPoint, originalHeightPoint);
                    templateMatchPoints.Add(originalPoint);

                    _originalTemplateHeight = Convert.ToInt32(_template.Height * 100 / scale);
                    _originaltemplateWidth = Convert.ToInt32(_template.Width * 100 / scale);
                    //DrawRectanglesOnImage(originalPoint, originalWidthPoint, scale);

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
        /// Draw rectangle on the original image.
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
            _image.PutText($"{originalWidthPoint}", originalPoint, HersheyFonts.HersheyPlain, 2.0, Scalar.Green);
        }
    }
}
