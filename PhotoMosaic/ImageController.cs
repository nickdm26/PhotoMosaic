using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMosaic {
    class ImageController {
        string sourceFolderFilePath;
        string inputImage;

        public ImageController()
        {

        }

        public Bitmap CreateOutPutBitmap()
        {
            Bitmap result = new Bitmap(800, 800);

            return result;
        }

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
