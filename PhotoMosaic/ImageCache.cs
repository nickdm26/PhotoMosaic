using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMosaic {

    /*
     * ImageCache is used to Read and Write a list of JSONImages to a json file.
     * Acts like a cache so you dont need to calculate the AverageColor of an image everytime.
     * You can simply check if its in the jsonimageslist variable.
     */
    class ImageCache {

        /*
         * ImageCache Constructor used to insialise jsonimageslist.
         */
        public ImageCache()
        {
            jsonimageslist = new List<JSONImage>();
        }

        /*
         * JSONImage Class is used to Hold Variables which will be read or written from a json file.
         */
        public class JSONImage
        {
            public string ImageURI { get; set; }
            public int Red { get; set; }
            public int Green { get; set; }
            public int Blue { get; set; }
        }

        /*
         * JSONImageList Class is used to hold a List of JSONImages
         */
        public class JSONImageList
        {
            public IList<JSONImage> JSONImages { get; set; }
        }

        List<JSONImage> jsonimageslist;     //Global Varaible holding all the JSONImages
        private readonly object balanceLock = new object();     //object used as a lock for Threading.
        
        /*
         * SaveImageCache method is used to Save the List into Json format and then save it in a json file.
         */
        public void SaveImageCache()
        {
            JSONImageList jsonList = new JSONImageList();                        
            jsonList.JSONImages = jsonimageslist;
            string json = JsonConvert.SerializeObject(jsonList);    //Serialize the object
            string filename = @"..\...\..\ImageCache.json";         //Filename
            File.WriteAllText(filename, json);                      //Write the json Text to the File and save it.
        }

        /*
         * AddImageToCache is used by passing the ImageURI and the Red, Green and Blue of the Color.
         * It then adds this to the jsonimageslist variable.
         */
        public void AddImageToCache(String ImageURI, int Red, int Green, int Blue)
        {
            lock (balanceLock)          //Lock is used so that this cant be used at the same time as reading from jsonimageslist.
            {
                jsonimageslist.Add(new JSONImage()  //Creates JSONImage onject and adds it to the global variable jsonimageslist.
                {
                    ImageURI = ImageURI,
                    Red = Red,
                    Green = Green,
                    Blue = Blue
                });
            }            
        }

        /*
         * ReadImageCache is used to Read the ImageCache.json file.
         * Fills the global variable jsonimageslist with the contents from the json file.
         */
        public void ReadImageCache()
        {
            try{
                string json = File.ReadAllText(@"..\...\..\ImageCache.json");       //try read file.
                JSONImageList jsonimageList = JsonConvert.DeserializeObject<JSONImageList>(json);   //DeserializeObject
                jsonimageslist = (List<JSONImage>)jsonimageList.JSONImages;         //Fill jsonimageslist
            } catch(Exception e)
            {
                Console.WriteLine(" Exception Caught. Cache does not exist.");  //File Could not be read, does not exist.
            }           
        }

        /*
         * GetColorFromCache is used to Return the Color from the List of JSONImages.
         * Does ImageExistInCache is called First from Outside.
         */
        public Color GetColorFromCache(string ImageURI)
        {
            lock (balanceLock)                      //Lock Used becauce This method is called from diffrent Threads and we dont want the List Added to while we are using it.
            {
                Color answer = new Color();
                foreach (JSONImage I in jsonimageslist)     //Loop over the JSONImages List to find the matching Image and return the Color.
                {
                    if (ImageURI.Equals(I.ImageURI))
                    {
                        answer = Color.FromArgb(I.Red, I.Green, I.Blue);
                        break;
                    }
                }
                return answer;
            }            
        }

        /*
         * DoesImageExistInCache is used to loop through the List of Images read from the file.
         * It will return true if it finds the image with the matching filename
         * Else return False
         */
        public Boolean DoesImageExistInCache(string ImageURI)
        {
            lock (balanceLock)      //Lock Used becauce This method is called from diffrent Threads and we dont want the List Added to while we are using it.
            {
                foreach (JSONImage I in jsonimageslist)     //Loop over JSONImages to check if ImageURI matches a Images Filename.
                {
                    if (ImageURI.Equals(I.ImageURI))
                    {
                        return true;
                    }
                }
                return false;
            }            
        }
    }
}
