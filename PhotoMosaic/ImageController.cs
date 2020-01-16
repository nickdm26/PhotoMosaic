using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMosaic
{
    class ImageController
    {
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

        /*
         * GenerateMosaic is the main method of this class used to Generate a PhotoMosaic
         * Finds the AverageColor of the Images
         * Resizes the InputImage to a usable size
         * Splits the Input Image into Cells Calculating the averageColor for each Cell
         * Finds the Closest Image for Every Cell in the input image.
         * Creates the Output Image.
         * Draws the Image to the Screen.
         */
        public void GenerateMosaic(string InputImageFileName, string SourceImagesDirectory)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();         //StopWatch to Calculate Overall time
            var stopwatchDraw = new System.Diagnostics.Stopwatch();

            var stopwatchFindClosestImage = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            imagecache = new ImageCache();          //Intialises the Image Cache
            imagecache.ReadImageCache();            //ImageCache Reads from its file.

            List<Image> SourceImages = ProcessSourceImagesInParrallel(SourceImagesDirectory);   //Creates a List of Images by Calling ProcessSourceImagesInParrallel
            imagecache.SaveImageCache();                                                        //Saves the Cache    
            inputImage = new SourceImage(pictureBox, Cells, InputImageFileName);                //Creates a SourceImage variable to be used
            inputImage.bitmap = ImageEditor.ResizeImageKeepAspectRatio(inputImage.bitmap, Size, Size);      //Resizes the Input Image to be the required Dimensions

            inputImage.CalculateAVGCellColors();    //Calls CalculateAvgCellColors to Break the Input Image down into Cells and Calculate the Average Color for each cell

            //Fix this VVVVV
            AVGColors = inputImage.AVGColors;       //Creates a 2d array of Color which is filled from the Input Images AverageColors.
            mosaicImages = inputImage.mosaicImages;
            mosaicImagesDrawn = new Boolean[Cells, Cells];
            //Fix this ^^^^^
            ClearMemory();

            stopwatchFindClosestImage.Start();  //Start the stopwatch to time Calculate finding the Cloest images
            for (int h = 0; h < Cells; h++)
            {
                for (int w = 0; w < Cells; w++)
                {
                    mosaicImages[h, w] = FindClosestImageAvgColor(SourceImages, AVGColors[h, w]);   //Loop over all the MosaicImage Cells Finding and Setting the Image with the ClosestAvgColor.
                    mosaicImagesDrawn[h, w] = false;                                                //Set every MosaicImagesDrawn Cell to False.    
                }
            }
            stopwatchFindClosestImage.Stop();   //Stop Timing Finding the Cloest Image.
            
            CreatedBitmap = CreateOutPutBitmap();   //Calls CreateOutPutBitmap and saves it as a global varable.
            stopwatchDraw.Start();
            Draw(CreatedBitmap);                    //Calls Draw to Draw the Bitmap
            stopwatchDraw.Stop();

            stopwatch.Stop();
            Console.WriteLine("Finding Closest Images Execution Time: " + stopwatchFindClosestImage.ElapsedMilliseconds + " ms");       //Prints to Console some Times that methods took to run.
            Console.WriteLine("Drawing Image: " + stopwatchDraw.ElapsedMilliseconds + " ms");
            Console.WriteLine("Total Execution Time: " + stopwatch.ElapsedMilliseconds / 1000 + " seconds");
            ClearMemory();
        }

        /*
         * ClearMemory is used to Forcefully call the garbage Collector.
         */
        private void ClearMemory()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }

        /*
         * FindClosestImageAvgColor is used to Compare All the AverageColors of the Images and return the closest Matching to avgcolor
         */
        private Image FindClosestImageAvgColor(List<Image> SourceImages, Color avgcolor)
        {            
            var sortedQuery =       //Use Linq as Parallel to loop over the SourceImages 
                from i in SourceImages.AsParallel()
                orderby i.Colordiffrence = i.CompareColors(avgcolor) ascending  //Call the CompareColors method and then order them by ColorDiffrence.
                select i;

            Image answer = sortedQuery.First(); //gets the first element from the Sorted LINQ Query
            return answer;
        }

        /*
         * Save is passed a FilePath as a string
         * It then saves the Bitmap at that location
         */
        public void Save(string SaveLocation)
        {
            CreatedBitmap.Save(SaveLocation);
        }

        /*
         * ProcessSourceImagesInParrallel is used to Process the images in the Directory Chossen.
         * It will return a list of Images.
         * 
         * Search though the Directory provided and adds the images to a list of strings.
         * Loop over the List of Strings Using Parallel ForEach (Performs a Foreach using multiple threads, Alost Faster)
         * Creates an image, Checks if it Exists in the ImageCache if so it Grabs the AverageColor.
         * If it does not exist then it has to calculate the AverageColor, Adds the image info to the ImageCache.
         */
        private List<Image> ProcessSourceImagesInParrallel(string SourceImagesDirectory)
        {
            var stopwatchProcessImages = new System.Diagnostics.Stopwatch();    //Stopwatch used to Time how long the Processing Takes
            stopwatchProcessImages.Start();
            Console.WriteLine("Source Directory: " + SourceImagesDirectory);

            List<string> filenames = new List<string>();
            foreach (string s in Directory.EnumerateFiles(@SourceImagesDirectory, "*.*", SearchOption.AllDirectories)) //Foreach loop iterate thought the directory and add the images to a list of strings.
            {
                filenames.Add(s);
            }
            Console.WriteLine("Input Images used: " + filenames.Count); //Writes to console how many images are going to be processed.
            
            List<Image> sourceImages = new List<Image>();
            Parallel.ForEach(filenames, name =>            //Parallel For Each Loop used to loop over each of the filenames
            {
                Image nwImage = new Image(name, Cells);    //Creates a new Image, passing the filename and Cells.

                if (imagecache.DoesImageExistInCache(nwImage.ImageURI))     //Calls the ImageCache to Check if its in the ImageCache
                {
                    nwImage.AvgColor = imagecache.GetColorFromCache(nwImage.ImageURI);  //Sets the AvgColor of the Image from the Color saved in the cache
                }
                else
                {
                    nwImage.SetAverageColor();          //If not in the ImageCache it has to Calculate the AvgerageColor
                    imagecache.AddImageToCache(nwImage.ImageURI, nwImage.AvgColor.R, nwImage.AvgColor.G, nwImage.AvgColor.B);   //Adds the Image information to the cache because its not there.
                }
                nwImage.ClearImageBitmap();     //Dispose of the Bitmap to perserve memory
                sourceImages.Add(nwImage);      //Adds the Image to the List of Images
            });

            stopwatchProcessImages.Stop();      //Stop the Timer
            Console.WriteLine("Processing Input Images in Parallel Execution Time: " + stopwatchProcessImages.ElapsedMilliseconds / 1000 + " seconds");
            return sourceImages;
        }


        /*
         * CreateOuputBitmap is used to Create the Output Bitmap which will then be shown on the screen.
         */
        public Bitmap CreateOutPutBitmap()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();     //Stopwatch used to time how long it takes to create the Output Bitmap.
            stopwatch.Start();

            Bitmap result = new Bitmap(Size, Size);                 //Create a bitmap with the required Dimensions
            Graphics canvas = Graphics.FromImage(result);           //Create a Graphics object from the Bitmap to draw on.
            Image currentImage = null;
            Bitmap resizedBitmap = null;

            int Cell_Width_Height = Size / Cells;                        //Create variable with Dimensions of Cell Area
            for (int r = 0; r < Cells; r++)    //loop though mosaicImages array
            {
                for (int c = 0; c < Cells; c++)
                {
                    if (mosaicImagesDrawn[r, c] == false)       //If Cell area has not been drawn.
                    {
                        resizedBitmap = ImageEditor.ResizeImageKeepAspectRatio(mosaicImages[r, c].ImageURI, Size / Cells, Size / Cells);    //Resize the image from mosaicImages
                        currentImage = mosaicImages[r, c];          //Save the Current Image from mosaicImages thats being used
                        mosaicImages[r, c].bitmap = resizedBitmap;  //give mosaicImages the resized Image
                        canvas.DrawImage(mosaicImages[r, c].bitmap, new Point(r * Cell_Width_Height, c * Cell_Width_Height));   //Draw the newly resized Bitmap to the canvas
                        mosaicImagesDrawn[r, c] = true;             //Set the MosaicImagesDraw to True

                        DrawSameImages(r, c, currentImage, resizedBitmap, Cell_Width_Height, canvas);   //Calls DrawSameImages so we dont need to resize the same image over and over.
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Output Execution Time: " + stopwatch.ElapsedMilliseconds + " ms");
            return result;              //Return the Created Bitmap Yay!!!
        }

        /*
         * DrawSameImages is used to loop over the bitmap and Draw the Same Bitmaps.
         * This increases the speed of Creating the OutputBitmap because it does not need to Resize the Bitmap over and over.
         */
        private void DrawSameImages(int Row_position, int Col_position, Image currentImage, Bitmap resizedBitmap, int Width_Height, Graphics canvas)
        {
            for (int r = Row_position; r < Cells; r++)      //Loop over the Rows starting at the RowPosition Provided
            {
                for (int c = Col_position; c < Cells; c++)  //Loop over the Columns starting at the ColPosition Provided.
                {
                    if (mosaicImagesDrawn[r, c] == false)   //If the Cell has not been drawn
                    {
                        if (currentImage == mosaicImages[r, c])     //Check if the CurrentImage is the Same as the one at the location.
                        {
                            mosaicImages[r, c].bitmap = resizedBitmap;      //If it is Set the Bitmap to the resizedBitmap
                            canvas.DrawImage(mosaicImages[r, c].bitmap, new Point(r * Width_Height, c * Width_Height));     //Draw the Resized Bitmap at the loacation
                            mosaicImagesDrawn[r, c] = true;         //Set MosaicImagesDrawn to True so we know its been drawn and can skip it.
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

        /*
         * Draw is used to Draw the Bitmap to the PictureBox
         */
        public void Draw(Bitmap bitmap)
        {
            pictureBox.Image = bitmap;
        }
           
        public void SaveStats()
        {

        }
    }
}
