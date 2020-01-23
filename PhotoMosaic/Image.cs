using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMosaic {
    class Image {
        public string ImageURI;
        public Bitmap bitmap;
        protected PictureBox pictureBox;
        public Color AvgColor;
        protected int AvgBrightness;
        public Double Colordiffrence;       //Global Variables
        public int Width;
        public int Height;
        public int Cells;
        public int CellPixels;
        Bitmap clone;

        /*
         * Image Constructor
         * Creates Image from FileName, Cells and PictureBox.
         */
        public Image(PictureBox pictureBox, int Cells, string filename)
        {
            this.pictureBox = pictureBox;
            ImageURI = filename;
            bitmap = new Bitmap(ImageURI);
            clone = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
            Width = bitmap.Width;
            Height = bitmap.Height;
            this.Cells = Cells;
            CellPixels = Width / Cells;
        }

        /*
         * Image Constructor
         * Creates Image from FileName and Cells
         */
        public Image(string filename, int Cells)
        {
            ImageURI = filename;
            bitmap = new Bitmap(ImageURI);
            clone = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
            Width = bitmap.Width;
            Height = bitmap.Height;
            this.Cells = Cells;
            CellPixels = Width / Cells;
        }

        public void ImportImage()
        {
            bitmap = new Bitmap(ImageURI);
        }

        /*
         * ClearImageBitmap is used to Dispose the Bitmap Clearing up memory.
         */
        public void ClearImageBitmap()
        {
            bitmap.Dispose();
            if(clone != null)
            {
                clone.Dispose();
            }
            
        }

        /*
         * CalculateAverageColor is used to Calculate the Average Color of the Image.
         * Returns the Color.
         */
        private Color CalculateAverageColor()
        {
         //   Test();
            return CalculateSectionUsingLockBits(0, 0, Width, Height);   //Calls CalculateSection Passing it the Dimensions of the image.
        }

        /*
         * SetAverageColor is used to Set the AverageColor of the Image.
         */
        public void SetAverageColor()
        {
            AvgColor = CalculateAverageColor();
            ClearImageBitmap();                     //Clears the Bitmap to save Memory.
        }

        /*
         * CalculateAverageBrightness is not used at the moment.
         */
        public int CalculateAverageBrightness()
        {
            int CalculatedBrightness = 0;

            return CalculatedBrightness;
        }

        public void Test()
        {
            clone = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
        }
        
        /*
         * CalculateSection is used to break a image into a section and Return the averageColor.
         * Can be used on the Entire image or just part of it.
         */
        public Color CalculateSection(int Start_x, int Start_y, int End_x, int End_y)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            int pixelAmount = (End_x - Start_x) * (End_y - Start_y);    //Amount of Pixels
            Color tempColor;

            for (int w = Start_x; w < End_x; w++)       //Loops over the Pixels in the Image
            {
                for (int h = Start_y; h < End_y; h++)
                {
                    tempColor = bitmap.GetPixel(w, h);      //Gets the Pixel
                    r += tempColor.R;                       //Adds the Red from the pixel to the variable
                    g += tempColor.G;                       //Adds the Green from the pixel to the variable
                    b += tempColor.B;                       //Adds the Blue from the pixel to the variable
                }
            }
            Color answer = Color.FromArgb(r / pixelAmount, g / pixelAmount, b / pixelAmount);   //Creates a Color from the Median Red, Green and Blue
            return answer;
        }


        /*
         * CalculateSectionUsingLockBits is about 30times faster than CalculateSection.
         * Can be used on the Entire image or just part of it.
         */
        public Color CalculateSectionUsingLockBits(int Start_x, int Start_y, int End_x, int End_y)
        {

            float red = 0;
            float green = 0;
            float blue = 0;
            int pixelAmount = (End_x - Start_x) * (End_y - Start_y);
            int counter = 0;            
            
            unsafe
            {             
                //Lock The bitmap into system memory
                //PixelFormat can be "Format24bppRGB", "Format32bppArgb", etc
                BitmapData bitmapData = 
                    clone.LockBits(new Rectangle(0,0, clone.Width, clone.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite, clone.PixelFormat);

                //Console.WriteLine(clone.PixelFormat)


                //Define variables for bytes per pixel, as well as Image Width & Height
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(clone.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                int end = (End_x * bytesPerPixel);
                int start = (Start_x * bytesPerPixel);

                //Define a pointer to the first pixel in the locked image
                //Scan0 gets or sets the address of the first pixel data in the bitmap.
                //This can also be thought of as the first scan line in the bitmap.
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                //Step thru each pixel in the image using pointers
                //Parallel.For executres a 'for' loop in which iterations 
                
                for (int w = Start_y; w < End_y; w++)
                {
                    //use the 'Stride' (scanline width) property to step line by line thru the image
                    byte* currentLine = PtrFirstPixel + (w * bitmapData.Stride);
                    
                    for (int h = start; h < end; h = h + bytesPerPixel)
                    {
                        blue += currentLine[h];
                        green += currentLine[h + 1];
                        red += currentLine[h + 2];
                        counter++;
                    }
                }
                
                clone.UnlockBits(bitmapData);
            }

            red = red / pixelAmount;
            green = green / pixelAmount;
            blue = blue / pixelAmount;
            
            Color answer = Color.FromArgb((int) red, (int) green, (int) blue);
            return answer;
        }
        
        /**
         * Calculates the Color Diffrence using the Euclidean Distance.
         * Returns as a double.
         */
        public double CompareColors(Color otherColor)
        {
            double R = Math.Pow((otherColor.R - AvgColor.R), 2);
            double G = Math.Pow((otherColor.G - AvgColor.G), 2);
            double B = Math.Pow((otherColor.B - AvgColor.B), 2);

            return Math.Sqrt(R + G + B);            
        }
    }
}
