using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMosaic {
    class SourceImage : Image{
        public Color[,] AVGColors;       //Color array with average colours.
        public Image[,] mosaicImages;
        Pen pen = new Pen(Color.Black);
        
        public SourceImage(PictureBox pictureBox, int Cells, string filename) : base(pictureBox, Cells, filename)
        {
            this.pictureBox = pictureBox;
            ImageURI = filename;
            bitmap = new Bitmap(ImageURI);

            Width = bitmap.Width;
            Height = bitmap.Height;
            this.Cells = Cells;
            CellPixels = Width / Cells;

            AVGColors = new Color[Cells, Cells];
            mosaicImages = new Image[Cells, Cells];
        }

        private void SetWidthHeightCellPixels()
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            CellPixels = Width / Cells;
        }

        public void CalculateAVGCellColors()
        {
            var StopwatchCalculateAvgCellColors = new System.Diagnostics.Stopwatch();
            StopwatchCalculateAvgCellColors.Start();
            SetWidthHeightCellPixels();

            for (int w = 0; w < Cells; w++)
            {
                for (int h = 0; h < Cells; h++)
                {
                    AVGColors[w, h] = CalculateSection(w * CellPixels, h * CellPixels, (w * CellPixels) + CellPixels, (h * CellPixels) + CellPixels);

                    //Console.WriteLine(AVGColors[w, h]);
                    //AVGColors[w, h] = CalculateSectionUsingLockBits(w * CellPixels, h * CellPixels, (w * CellPixels) + CellPixels, (h * CellPixels) + CellPixels);

                }
            }

            //for (int w = 0; w < Cells; w++)
            //{
            //    Parallel.For(0, Cells, h =>
            //    {
            //        AVGColors[w, h] = CalculateSection(w * CellPixels, h * CellPixels, (w * CellPixels) + CellPixels, (h * CellPixels) + CellPixels);
            //        //Console.WriteLine(AVGColors[w, h]);
            //    });
            //}


            StopwatchCalculateAvgCellColors.Stop();
            Console.WriteLine("Calculating Avg Cell Colors: " + StopwatchCalculateAvgCellColors.ElapsedMilliseconds + " ms");
        }

        public void DrawAvgColors()
        {
            int wid = 0;
            int hei = 0;
            Brush brush = new SolidBrush(Color.Black);
            Bitmap tempBitmap = new Bitmap(800, 800);
            Graphics Canvas = Graphics.FromImage(tempBitmap);

            for (int w = 0; w < Cells; w++)
            {
                wid += 8;
                hei = 0;
                for (int h = 0; h < Cells; h++)
                {                    
                    brush = new SolidBrush(AVGColors[w, h]);
                    Canvas.FillRectangle(brush, wid, hei, 8, 8);
                    //Console.WriteLine(wid + " " + hei);
                    hei += 8;
                }                
            }
            pictureBox.Image = tempBitmap;
            Console.WriteLine("drawn");
        }        

        public void CreateBitmap()
        {
            Bitmap flag = new Bitmap(800, 800);
            Graphics flagGraphics = Graphics.FromImage(flag);
            int red = 0;
            int white = 11;
            while(white <= 100)
            {
                flagGraphics.FillRectangle(Brushes.Red, 0, red, 200, 10);
                flagGraphics.FillRectangle(Brushes.Wheat, 0, white, 200, 10);
                red += 20;
                white += 20;
            }
            pictureBox.Image = flag;
        }

        
    }
}
