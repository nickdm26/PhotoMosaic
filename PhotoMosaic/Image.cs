using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMosaic {
    class Image {
        protected string ImageURI;
        public Bitmap bitmap;
        protected PictureBox pictureBox;
        protected Color AvgColor;
        protected int AvgBrightness;
        public int Width;
        public int Height;
        public int Cells = 64;
        public int CellPixels = 0;

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

        public Color CalculateAverageColor()
        {          
            return CalculateSection(0, 0, Width, Height); ;
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
            int pixelCounter = 0;
            Color sectionAvgColor;
            Color tempColor;
            Brush tempbrush = new SolidBrush(Color.Black);
            //Console.WriteLine(ImageURI);

            for (int w = Start_x; w < End_x; w++)
            {
                for (int h = Start_y; h < End_y; h++)
                {
                    tempColor = bitmap.GetPixel(w, h);
                    //Console.WriteLine(tempColor);
                    r += tempColor.R;
                    g += tempColor.G;
                    b += tempColor.B;
                    pixelCounter++;
                    //Console.WriteLine(r + " " + g + " " + b);
                    //Console.WriteLine(w + " " + h);
                    // Canvas.FillRectangle(tempbrush, w, h, 1, 1);
                }
            }
            r = r / pixelCounter;
            g = g / pixelCounter;
            b = b / pixelCounter;

            sectionAvgColor = Color.FromArgb(r, g, b);
            return sectionAvgColor;
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
