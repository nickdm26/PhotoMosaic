using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMosaic {
    class SourceImage : Image{
        Color[,] AVGColors = new Color[Cells, Cells];         //Color array with average colours.
        
        public SourceImage(Graphics Canvas) : base(Canvas)
        {

        }

        public void CalculateAVGCellColors()
        {
            for(int w = 0; w < Cells; w++)
            {
                for(int h = 0; h < Cells; h++)
                {

                }
            }
        }

        private void CalculateSection(int x, int y)
        {
            //Tuple<int, int, int> tuple = new Tuple<int, int, int>(0, 0, 0);
            int r = 0;
            int g = 0;
            int b = 0;

            for(int w = x; w < x + Cells; w++)
            {
                for(int h = y; h < y + Cells; h++)
                {
                    r += 0;
                }
            }
        }
    }
}
