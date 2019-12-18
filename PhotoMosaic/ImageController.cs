using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMosaic {
    class ImageController {
        string sourceFolderFilePath;
        SourceImage inputImage;
        PictureBox pictureBox;

        public ImageController(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
        }

        public void GenerateMosaic(string InputImageFileName, string SourceImagesDirectory)
        {
            List<Image> SourceImages = ProcessSourceImages(SourceImagesDirectory);
            inputImage = new SourceImage(pictureBox, 64, InputImageFileName);
            inputImage.bitmap = ImageEditor.ResizeImageKeepAspectRatio(inputImage.bitmap, 800, 800);
            inputImage.CalculateAVGCellColors();
            inputImage.SetAverageColor();   //Delete me later

            Color[,] AVGColors = inputImage.AVGColors;
            FindClosestImageAvgColor(SourceImages, inputImage.AvgColor);


        }

        private Image FindClosestImageAvgColor(List<Image> SourceImages, Color avgcolor)
        {
            foreach(Image i in SourceImages)        //Loop through each Image in the list to set the ColorDiffrence field
            {                                       
                i.Colordiffrence = i.CompareColors(avgcolor);   //Should change this line it seems unnecessary
            }

            var sortedQuery =       //LINQ Query used to sort the Images by ColorDiffrence
                from i in SourceImages
                orderby i.Colordiffrence ascending
                select i;

            Console.WriteLine("Color num to match: " + avgcolor);

            foreach(var i in sortedQuery)
            {
                Console.WriteLine("Sorted Colors: " + i.Colordiffrence);
            }
            //Return 1st in the sortedQuery as its how far away from the Color its comparing against.

            Image answer = sortedQuery.First(); //gets the first element from the Sorted LINQ Query
            return answer;
        }

        public void SaveMosiac(Bitmap mosaicBitmap, string SaveLocation)
        {

        }

        private List<Image> ProcessSourceImages(string SourceImagesDirectory)
        {
            Console.WriteLine("Source Directory: " + SourceImagesDirectory);

            List<Image> sourceImages = new List<Image>();
            foreach (string s in Directory.EnumerateFiles(@SourceImagesDirectory, "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine("In Directory: " + s);
                Image nwImage = new Image(s, 64);
                nwImage.SetAverageColor();
                sourceImages.Add(nwImage);
            }

            return sourceImages;
        }

        public Bitmap CreateOutPutBitmap()
        {
            Bitmap result = new Bitmap(800, 800);

            return result;
        }

        /*
         * ImportSourceImages is used to create an array holding all of the images imported from the folder.
         */
        public void ImportSourceImages(string folderPath)
        {
            folderPath = sourceFolderFilePath;
            ArrayList imagesArray = new ArrayList();
            
            //string[] array1 = Directory.GetFiles(@"C:\Users\nick.muldrew\Documents\GitHub\PhotoMosaic\Images");

            foreach(string s in Directory.EnumerateFiles(@"..\..\..\Images\", "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine("In Directory: " + s);
            }
        }
    }
}
