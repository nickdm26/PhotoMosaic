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
        Image[,] mosaicImages;
        Boolean[,] mosaicImagesDrawn;
        Color[,] AVGColors;
        int Cells = 64;
        ImageCache imagecache;
        Bitmap CreatedBitmap;
        int Size = 800;


        public ImageController(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
        }

        public void GenerateMosaic(string InputImageFileName, string SourceImagesDirectory)
        {
            
            var stopwatch = new System.Diagnostics.Stopwatch();
            var stopwatchDraw = new System.Diagnostics.Stopwatch();
            
            var stopwatchFindClosestImage = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            imagecache = new ImageCache();
            imagecache.ReadImageCache();

            //List<Image> SourceImages = ProcessSourceImages(SourceImagesDirectory);
            List<Image> SourceImages = ProcessSourceImagesInParrallel(SourceImagesDirectory);
            imagecache.SaveImageCache();

            //foreach(Image i in SourceImages)
            //{
            //    imagecache.AddImageToCache(i.ImageURI, i.AvgColor.R, i.AvgColor.G, i.AvgColor.B);
            //}


            inputImage = new SourceImage(pictureBox, Cells, InputImageFileName);
            inputImage.bitmap = ImageEditor.ResizeImageKeepAspectRatio(inputImage.bitmap, Size, Size);

            inputImage.CalculateAVGCellColors();
            //inputImage.SetAverageColor();   //Delete me later

            AVGColors = inputImage.AVGColors;
            mosaicImages = inputImage.mosaicImages;
            mosaicImagesDrawn = new Boolean[Cells, Cells];
            ClearMemory();
            stopwatchFindClosestImage.Start();

            for (int h = 0; h < Cells; h++)
            {
                for (int w = 0; w < Cells; w++)
                {
                    mosaicImages[h, w] = FindClosestImageAvgColor(SourceImages, AVGColors[h, w]);
                    mosaicImagesDrawn[h, w] = false;
                }
            }

            //for (int h = 0; h < Cells; h++)
            //{
            //    Parallel.For(0, Cells, w =>
            //    {
            //        mosaicImages[h, w] = FindClosestImageAvgColor(SourceImages, AVGColors[h, w]);
            //        mosaicImagesDrawn[h, w] = false;
            //    });
            //}

            //FindClosestImageAvgColor(SourceImages, inputImage.AvgColor);
            //inputImage.mosaicImages = mosaicImages;     //need to change how this part interacts with SourceImage class.
            stopwatchFindClosestImage.Stop();
            

            CreatedBitmap = CreateOutPutBitmap();
            stopwatchDraw.Start();
            Draw(CreatedBitmap);
            stopwatchDraw.Stop();

            stopwatch.Stop();
            Console.WriteLine("Finding Closest Images Execution Time: " + stopwatchFindClosestImage.ElapsedMilliseconds + " ms");
            Console.WriteLine("Drawing Image: " + stopwatchDraw.ElapsedMilliseconds + " ms");
            Console.WriteLine("Total Execution Time: " + stopwatch.ElapsedMilliseconds/1000 + " seconds");
                       
            ClearMemory();
        }

        private void ClearMemory()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }

        //Return 1st in the sortedQuery as its how far away from the Color its comparing against.
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
            
            Image answer = sortedQuery.First(); //gets the first element from the Sorted LINQ Query
            return answer;
        }

        public void Save(string SaveLocation)
        {
            CreatedBitmap.Save(SaveLocation);
        }

        private List<Image> ProcessSourceImages(string SourceImagesDirectory)
        {
            var stopwatchProcessImages = new System.Diagnostics.Stopwatch();
            stopwatchProcessImages.Start();
            Console.WriteLine("Source Directory: " + SourceImagesDirectory);

            List<string> filenames = new List<string>();
            int counter = 0;
            foreach (string s in Directory.EnumerateFiles(@SourceImagesDirectory, "*.*", SearchOption.AllDirectories))
            {
                filenames.Add(s);
                counter++;
            }

            Console.WriteLine("Input Images used: " + counter);

            List<Image> sourceImages = new List<Image>();
            foreach (string s in filenames)
            {
                //Console.WriteLine("In Directory: " + s);
                Image nwImage = new Image(s, 64);
                nwImage.SetAverageColor();
                sourceImages.Add(nwImage);
            }
            stopwatchProcessImages.Stop();
            Console.WriteLine("Processing Input Images Execution Time: " + stopwatchProcessImages.ElapsedMilliseconds + " ms");
            return sourceImages;
        }

        private List<Image> ProcessSourceImagesInParrallel(string SourceImagesDirectory)
        {
            var stopwatchProcessImages = new System.Diagnostics.Stopwatch();
            stopwatchProcessImages.Start();
            Console.WriteLine("Source Directory: " + SourceImagesDirectory);

            List<string> filenames = new List<string>();
            int counter = 0;
            foreach (string s in Directory.EnumerateFiles(@SourceImagesDirectory, "*.*", SearchOption.AllDirectories)) //Foreach loop used to count how many images are going to be processed.
            {
                filenames.Add(s);
                counter++;
            }
            Console.WriteLine("Input Images used: " + counter); //Writes to console how many images are going to be processed.


            List<Image> sourceImages = new List<Image>();
            Parallel.ForEach(filenames, s =>
            {
                //Console.WriteLine("In Directory: " + s);
                Image nwImage = new Image(s, 64);

                if (imagecache.DoesImageExistInCache(nwImage.ImageURI))
                {
                    nwImage.AvgColor = imagecache.GetColorFromCache(nwImage.ImageURI);
                }
                else
                {
                    nwImage.SetAverageColor();
                    imagecache.AddImageToCache(nwImage.ImageURI, nwImage.AvgColor.R, nwImage.AvgColor.G, nwImage.AvgColor.B);
                }
                nwImage.ClearImageBitmap();
                sourceImages.Add(nwImage);
            });

            
            stopwatchProcessImages.Stop();
            Console.WriteLine("Processing Input Images in Parallel Execution Time: " + stopwatchProcessImages.ElapsedMilliseconds/1000 + " seconds");
            return sourceImages;
        }



        public Bitmap CreateOutPutBitmap()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            Bitmap result = new Bitmap(Size, Size);
            Graphics canvas = Graphics.FromImage(result);
            Image currentImage = null;
            Bitmap resizedBitmap = null;

            int Width_Height = Size / Cells;
            for (int h = 0; h < Cells; h++)    //loop though mosaicImages array
            {
                for (int w = 0; w < Cells; w++)
                {
                    if(mosaicImagesDrawn[h, w] == false)
                    {
                        resizedBitmap = ImageEditor.ResizeImageKeepAspectRatio(mosaicImages[h, w].ImageURI, Size / Cells, Size / Cells);
                        currentImage = mosaicImages[h, w];
                        mosaicImages[h, w].bitmap = resizedBitmap;
                        canvas.DrawImage(mosaicImages[h, w].bitmap, new Point(h * Width_Height, w * Width_Height));
                        mosaicImagesDrawn[h, w] = true;

                        DrawSameImages(result, h, w, currentImage, resizedBitmap, Width_Height, canvas);
                    }
                    
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Output Execution Time: " + stopwatch.ElapsedMilliseconds + " ms");
            return result;
        }

        private void DrawSameImages(Bitmap result, int hposition, int wposition, Image currentImage, Bitmap resizedBitmap, int Width_Height, Graphics canvas){
            for (int h = hposition; h < Cells; h++)    
            {
                for (int w = wposition; w < Cells; w++)
                {
                    if (mosaicImagesDrawn[h, w] == false)
                    {
                        if (currentImage == mosaicImages[h, w])
                        {
                            //resizedBitmap = ImageEditor.ResizeImageKeepAspectRatio(mosaicImages[h, w].ImageURI, 800 / Cells, 800 / Cells);
                            //currentImage = mosaicImages[h, w];
                            mosaicImages[h, w].bitmap = resizedBitmap;
                            canvas.DrawImage(mosaicImages[h, w].bitmap, new Point(h * Width_Height, w * Width_Height));
                            mosaicImagesDrawn[h, w] = true;
                        }                        
                    }
                }
            }
        }



        public void ClearScreen()
        {
            var b = new Bitmap(1, 1);
            b.SetPixel(0, 0, Color.White);
            pictureBox.Image = new Bitmap(b, Size, Size);
        }

        public void Draw(Bitmap bitmap)
        {
            pictureBox.Image = bitmap;
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

        public void SaveStats()
        {

        }
    }
}
