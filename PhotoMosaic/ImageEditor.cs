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

    /*
     * ImageEditor is a Static class used to perform Operations on Bitmaps
     */
    static class ImageEditor {

        /*
         * Imports the image then Crops it using the ResizeImageKeepAspectRatio method.
         * Calls the SaveImage Method to save the new Image.
         */
        public static void ImportImage_CropAndResize_Save(string filepath)
        {
            //ImportImage(filepath);
            Bitmap importedImage = new Bitmap(filepath);
            Bitmap editedImage = ResizeImageKeepAspectRatio(importedImage, 800, 800);
            SaveImage(editedImage, filepath);
        }

        public static void Crop()
        {

        }

        /*
         * SaveImage is used to save the Bitmap as a .png to the Images\Test folder.
         * If the Folder exists it will save it in there if not it will create the new folder.
         */
        public static void SaveImage(Bitmap bitmap, string givenfilepath)
        {
            string path = @"..\..\..\Images\Test";
            string imageName = Path.GetFileNameWithoutExtension(givenfilepath);      //Get the name of the file without the filename extension.

            string saveFileName = path + @"\" + imageName + ".png";

            Console.WriteLine(Path.GetFileNameWithoutExtension(givenfilepath));            

            try
            {
                if (Directory.Exists(path))                                 //Check if folder already exists if so save it there.
                {
                    Console.WriteLine("The Path already exists.");
                    bitmap.Save(saveFileName);
                }
                else
                {                                                          //If folder does not exist save create a new directory to save the image to.                             
                    DirectoryInfo di = Directory.CreateDirectory(path);
                    Console.WriteLine("The Directory was created successfully at {0}.", Directory.GetCreationTime(path));
                    bitmap.Save(saveFileName);
                }                
            }
            catch(Exception e)                                             //Writes an error to console if error occured saving image.
            {
                Console.WriteLine("ERROR: The folder to save to could not be created/found: {0}", e.ToString());
            }
        }
        
        /*
         * ResizeImageKeepAspectRatio is used to Resize the Bitmap while keeping the aspect ratio
         * width & Height are the desired dimensions that you want the bitmap to be returned as.
         */
        public static Bitmap ResizeImageKeepAspectRatio(Bitmap image, int width, int height)
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

        /*
         * ResizeImageKeepAspectRatio is used to Resize the Bitmap while keeping the aspect ratio
         * width & Height are the desired dimensions that you want the bitmap to be returned as.
         * Can be passed a filepath and it will Create a Bitmap from that instead.
         */
        public static Bitmap ResizeImageKeepAspectRatio(string path, int width, int height)
        {
            Bitmap result = null;
            Bitmap image = new Bitmap(path);

            try
            {
                if (image.Width != width || image.Height != height)
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
                            if (scalingX < scalingY)
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

            image.Dispose();
            return result;
        }
    }
}
