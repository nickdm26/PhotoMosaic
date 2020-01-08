using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMosaic {
    class ImageCache {
        public ImageCache()
        {
            jsonimageslist = new List<JSONImage>();
        }

        public class JSONImage
        {
            public string ImageURI { get; set; }
            public int Red { get; set; }
            public int Green { get; set; }
            public int Blue { get; set; }
        }

        public class JSONImageList
        {
            public IList<JSONImage> JSONImages { get; set; }
        }

        List<JSONImage> jsonimageslist;
        private readonly object balanceLock = new object();



        public void SaveImageCache()
        {
            JSONImageList jsonList = new JSONImageList();
            
            AddImageToCache(@"C:\Users\User\Documents\PhotoMosaic\Images\n02085620-Chihuahua\", 255, 20, 100);
            
            AddImageToCache(@"C:\Users\User\Documents\PhotoMosaic\Images\test\", 200, 5 ,139);
            
            jsonList.JSONImages = jsonimageslist;

            string json = JsonConvert.SerializeObject(jsonList);

           // Console.WriteLine();
            //Console.WriteLine(json);
           // Console.WriteLine();

            string filename = @"..\...\..\ImageCache.json";
            File.WriteAllText(filename, json);

        }

        public void AddImageToCache(String ImageURI, int Red, int Green, int Blue)
        {
            lock (balanceLock)
            {
                jsonimageslist.Add(new JSONImage()
                {
                    ImageURI = ImageURI,
                    Red = Red,
                    Green = Green,
                    Blue = Blue
                });
            }
            
        }

        public void ReadImageCache()
        {
            try{
                string json = File.ReadAllText(@"..\...\..\ImageCache.json");
                JSONImageList jsonimageList = JsonConvert.DeserializeObject<JSONImageList>(json);
                jsonimageslist = (List<JSONImage>)jsonimageList.JSONImages;
            } catch(Exception e)
            {
                Console.WriteLine(" Exception Caught. Cache does not exist.");
            }
           
        }

        public Color GetColorFromCache(string ImageURI)
        {
            lock (balanceLock)
            {
                Color answer = new Color();
                foreach (JSONImage I in jsonimageslist)
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

        public Boolean DoesImageExistInCache(string ImageURI)
        {
            lock (balanceLock)
            {
                foreach (JSONImage I in jsonimageslist)
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
