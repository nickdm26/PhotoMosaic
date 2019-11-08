using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMosaic {
    class ImageEditor {
        PictureBox pictureBox;
        Bitmap importedImage;
        string filepath;

        public ImageEditor(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            importedImage = new Bitmap(800, 800);
        }

        public void ImportImage(string filepath)
        {
            this.filepath = filepath;
            Console.WriteLine(filepath);
            importedImage = new Bitmap(filepath);
        }

        public void ImportImage_CropAndResize_Save(string filepath)
        {
            ImportImage(filepath);
        }

        public void Crop()
        {

        }

        public void SaveImage()
        {
            string path = @"..\..\..\Images\Test";

            string imageName = Path.GetFileName(filepath);

            Console.WriteLine(imageName);                        

            try
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine("The Path already exists.");
                    importedImage.Save(path + @"\" + imageName);
                }
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("The Directory was created successfully at {0}.", Directory.GetCreationTime(path));
                importedImage.Save(path + @"\" + imageName);
            }
            catch(Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            //importedImage.Save(filepath);
        }



        public void Draw()
        {
            //pictureBox.Image = ResizeImage(importedImage, 800, 800);
            pictureBox.Image = ResizeImageKeepAspectRatio(importedImage, 800, 800);
        }

        private Bitmap ResizeImageKeepAspectRatio(Bitmap image, int width, int height)
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
                        result = (Bitmap)target.Clone();
                    }
                }
                else
                {
                    //Image size matched the given size
                    result = (Bitmap)image.Clone();
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
