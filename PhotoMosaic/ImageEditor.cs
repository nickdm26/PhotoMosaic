using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMosaic {
    class ImageEditor {
        PictureBox pictureBox;
        Bitmap importedImage;

        public ImageEditor(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            importedImage = new Bitmap(800, 800);
        }

        public void ImportImage(string filepath)
        {
            importedImage = new Bitmap(filepath);
            Draw();
        }

        private Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            Rectangle rect = new Rectangle(0, 0, width, height);
            Bitmap returnImage = new Bitmap(width, height);

            returnImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(returnImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return returnImage;
        }

        public void Crop()
        {

        }

        public void SaveImage()
        {

        }

        public void Draw()
        {
            //pictureBox.Image = ResizeImage(importedImage, 800, 800);
            pictureBox.Image = ResizeImageKeepAspectRatio(importedImage, 800, 800);
        }

        private System.Drawing.Bitmap ResizeImageKeepAspectRatio(System.Drawing.Bitmap image, int width, int height)
        {
            Bitmap result = null;

            try
            {
                if(image.Width != width || image.Height != height)
                {
                    //Resize
                    float sourceRatio = (float)image.Width / image.Height;

                    using (var target = new Bitmap(width, height))
                    {
                        using (var g = Graphics.FromImage(target))
                        {
                            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                            float scaling;
                            float scalingY = (float)image.Height / height;
                            float scalingX = (float)image.Width / width;
                            if(scalingX < scalingY)
                            {
                                scaling = scalingX;
                            }
                            else
                            {
                                scaling = scalingY;
                            }

                            int newWidth = (int)(image.Width / scaling);
                            int newHeight = (int)(image.Height / scaling);

                            // Correct float to int rounding
                            if (newWidth < width) newWidth = width;
                            if (newHeight < height) newHeight = height;

                            // See if image needs to be cropped
                            int shiftX = 0;
                            int shiftY = 0;

                            if (newWidth > width)
                            {
                                shiftX = (newWidth - width) / 2;
                            }

                            if (newHeight > height)
                            {
                                shiftY = (newHeight - height) / 2;
                            }

                            //Draw Image
                            g.DrawImage(image, -shiftX, -shiftY, newWidth, newHeight);

                        }
                        result = (System.Drawing.Bitmap)target.Clone();
                    }
                }
                else
                {
                    //Image size matched the given size
                    result = (System.Drawing.Bitmap)image.Clone();
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
    }
}
