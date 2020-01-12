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
        public Double Colordiffrence;
        public int Width;
        public int Height;
        public int Cells;
        public int CellPixels;

        public Image(PictureBox pictureBox, int Cells, string filename)
        {
            this.pictureBox = pictureBox;
            ImageURI = filename;
            bitmap = new Bitmap(ImageURI);
            Width = bitmap.Width;
            Height = bitmap.Height;
            this.Cells = Cells;
            CellPixels = Width / Cells;
        }

        public Image(string filename, int Cells)
        {
            ImageURI = filename;
            bitmap = new Bitmap(ImageURI);
            Width = bitmap.Width;
            Height = bitmap.Height;
            this.Cells = Cells;
            CellPixels = Width / Cells;
        }

        public void ImportImage()
        {
            bitmap = new Bitmap(ImageURI);
        }

        public void ClearImageBitmap()
        {
            bitmap.Dispose();
        }

        public Color CalculateAverageColor()
        {          
            return CalculateSection(0, 0, Width, Height); ;
        }

        public void SetAverageColor()
        {
            AvgColor = CalculateAverageColor();
            ClearImageBitmap();
        }

        public int CalculateAverageBrightness()
        {
            int CalculatedBrightness = 0;

            return CalculatedBrightness;
        }

        protected Color CalculateSection(int Start_x, int Start_y, int End_x, int End_y)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            int pixelAmount = (End_x - Start_x) * (End_y - Start_y);
            Color sectionAvgColor;
            Color tempColor;
            int counter = 0;

            for (int w = Start_x; w < End_x; w++)
            {
                for (int h = Start_y; h < End_y; h++)
                {
                    tempColor = bitmap.GetPixel(w, h);
                    r += tempColor.R;
                    g += tempColor.G;
                    b += tempColor.B;
                    counter++;
                }
            }
            //r = r / pixelAmount;
            //g = g / pixelAmount;
            //b = b / pixelAmount;

            //sectionAvgColor = Color.FromArgb(r, g, b);
            Color answer = Color.FromArgb(r / pixelAmount, g / pixelAmount, b / pixelAmount);

            return answer;
        }

        protected Color CalculateSectionUsingLockBits(int Start_x, int Start_y, int End_x, int End_y)
        {
            int red = 0;
            int green = 0;
            int blue = 0;
            int pixelAmount = (End_x - Start_x) * (End_y - Start_y);
            int counter = 0;

            unsafe
            {             
                //Lock The bitmap into system memory
                //PixelFormat can be "Format24bppRGB", "Format32bppArgb", etc
                BitmapData bitmapData = 
                    bitmap.LockBits(new Rectangle(0,0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);

                //Define variables for bytes per pixel, as well as Image Width & Height
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;

                //Define a pointer to the first pixel in the locked image
                //Scan0 gets or sets the address of the first pixel data in the bitmap.
                //This can also be thought of as the first scan line in the bitmap.
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                //Step thru each pixel in the image using pointers
                //Parallel.For executres a 'for' loop in which iterations 
                //may run in parallel

                //Parallel.For(Start_x, End_x, w =>
                //{
                //    //use the 'Stride' (scanline width) property to step line by line thru the image
                //    byte* currentLine = PtrFirstPixel + (w * bitmapData.Stride);
                //    for(int h = Start_y; h < (widthInBytes - (End_y * bytesPerPixel)); h = h + bytesPerPixel)
                //    {
                //        blue = currentLine[h];
                //        green = currentLine[h + 1];
                //        red = currentLine[h + 2];
                //    }
                //});
                
                for (int w = Start_x; w < End_x; w++)
                {
                    //use the 'Stride' (scanline width) property to step line by line thru the image
                    byte* currentLine = PtrFirstPixel + (w * bitmapData.Stride);
                    int end = (End_y * bytesPerPixel);
                    for (int h = Start_y; h < end; h = h + bytesPerPixel)
                    {
                        blue += Convert.ToInt32(currentLine[h]);
                        green += Convert.ToInt32(currentLine[h + 1]);
                        red += Convert.ToInt32(currentLine[h + 2]);
                        counter++;
                    }
                }

                bitmap.UnlockBits(bitmapData);

            }

            red = red / pixelAmount;
            green = green / pixelAmount;
            blue = blue / pixelAmount;

            if(red > 255)
            {
                Console.WriteLine("red: " + red);
                red = 255;
            }
            if(green > 255)
            {
                Console.WriteLine("green: " + green);
                green = 255;
            }
            if(blue > 255)
            {
                Console.WriteLine("blue: " + blue);
                blue = 255;
            }
            Color answer = Color.FromArgb(red, green, blue);
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


        public void Draw()
        {
            pictureBox.Image = bitmap;
        }
    }
}
