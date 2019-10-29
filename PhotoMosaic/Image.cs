using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMosaic {
    class Image {
        private string ImageURI;
        public Bitmap bitmap;
        private Graphics Canvas;
        private Color AvgColor;
        private int AvgBrightness;
        public int Width;
        public int Height;
        public readonly static int Cells = 16;

        public Image(Graphics Canvas)
        {
            Canvas = this.Canvas;
            ImageURI = @"C:\Users\nick.muldrew\Downloads\4_Squares.png";
            bitmap = new Bitmap(ImageURI);
            Canvas.DrawImage(bitmap, 800, 800);
        }

        public Color CalculateAverageColor()
        {
            Color CalculateAverageColor = new Color();

            return CalculateAverageColor;
        }

        public int CalculateAverageBrightness()
        {
            int CalculatedBrightness = 0;

            return CalculatedBrightness;
        }
    }
}
