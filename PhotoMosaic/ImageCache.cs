using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public void SaveImageCache()
        {
            JSONImageList jsonList = new JSONImageList();
            
            AddImageToCache(@"C:\Users\User\Documents\PhotoMosaic\Images\n02085620-Chihuahua\", 255, 20, 100);
            
            AddImageToCache(@"C:\Users\User\Documents\PhotoMosaic\Images\test\", 200, 5 ,139);
            
            jsonList.JSONImages = jsonimageslist;

            string json = JsonConvert.SerializeObject(jsonList);

            Console.WriteLine();
            Console.WriteLine(json);
            Console.WriteLine();

            string filename = @"..\...\..\ImageCache.json";
            File.WriteAllText(filename, json);

        }

        public void AddImageToCache(String ImageURI, int Red, int Green, int Blue)
        {
            jsonimageslist.Add(new JSONImage() {
                ImageURI = ImageURI,
                Red = Red,
                Green = Green,
                Blue = Blue
            });
        }
    }
}
