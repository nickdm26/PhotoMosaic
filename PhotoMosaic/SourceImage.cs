using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMosaic {
    class SourceImage : Image{
        public Color[,] AVGColors;       //Color array with average colours
        public Image[,] mosaicImages;
        
        /*
         * SourceImage Constructor used to Insistalise Variables.
         */
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


        /*
         * CalculateAVGCellColors is used to Split the Image up into Cells and then Calculate the Average Color for Each Cell.
         */
        public void CalculateAVGCellColors()
        {
            //var StopwatchCalculateAvgCellColors = new System.Diagnostics.Stopwatch();   //StopWatch Used to Time this method
            //StopwatchCalculateAvgCellColors.Start();
            SetWidthHeightCellPixels();     //Set the Width, Height and CellPixels 
            Test();

            for (int w = 0; w < Cells; w++)     //Loop over the Cells
            {
                for (int h = 0; h < Cells; h++)
                {
                    AVGColors[w, h] = CalculateSectionUsingLockBits(w * CellPixels, h * CellPixels, (w * CellPixels) + CellPixels, (h * CellPixels) + CellPixels);
                }
            }

            //StopwatchCalculateAvgCellColors.Stop();
            //Console.WriteLine("Calculating Avg Cell Colors: " + StopwatchCalculateAvgCellColors.ElapsedMilliseconds + " ms");   //Write to Console how long the method took.
        }

        /*
         * SetWidthHeightCellPixels is used to Set the Width, Height and CellPixels Variables.
         */
        private void SetWidthHeightCellPixels()
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            CellPixels = Width / Cells;
        }
    }
}
