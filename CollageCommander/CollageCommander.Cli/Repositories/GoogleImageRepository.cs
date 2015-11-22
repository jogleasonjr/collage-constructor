using CollageCommander.Cli.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CollageCommander.Cli.Repositories
{
    public class GoogleImageRepository : IImageRepository
    {
        //private const string BaseUrl = "";
        private const string SearchUrl = "https://ajax.googleapis.com/ajax/services/search/images?v=1.0&imgsz=medium&q=";

        public async Task<IEnumerable<FileInfo>> Get(string path, int count)
        {
            var files = new List<FileInfo>();
            int startIndex = 0;

            //google image api restriction for 64 images max
            count = Math.Min(60, count);

            while (files.Count < count)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(CreateImageSearchQuery(path, startIndex));
                    var json = await response.Content.ReadAsStringAsync();

                    //dynamic results = JsonConvert.DeserializeObject(json);

                    var jObject = JObject.Parse(json);
                    var searchResults = jObject["responseData"]["results"];

                    files.AddRange(GetImagesFromResults(searchResults));
                    startIndex += searchResults.Count();

                    //currentPath = jObject["responseData"]["cursor"]["moreResultsUrl"].Value<string>();
                }
            }

            return files;
        }

        private static IEnumerable<FileInfo> GetImagesFromResults(IEnumerable<JToken> searchResults)
        {
            foreach (var result in searchResults)
            {
                var imageUrl = result["unescapedUrl"].Value<string>();

                //var trimAddlQueryUrl = imageUrl.Substring()

                FileInfo tempImageFile = null;

                try
                {
                    tempImageFile = DownloadImageToTemp(imageUrl).Result;
                }
                catch (Exception ex)
                {
                    // bad url
                    continue;
                }


                yield return tempImageFile;
            }
        }

        private static async Task<FileInfo> DownloadImageToTemp(string imageUri)
        {
            string extension = "bmp"; // Path.GetExtension(imageUri);

            using (var client = new HttpClient())
            {
                var data = await client.GetByteArrayAsync(imageUri);
                var tempImagePath = $"{Path.GetTempFileName()}{extension}";

                File.WriteAllBytes(tempImagePath, data);

                return new FileInfo(tempImagePath);
            }
        }


        private static string CreateImageSearchQuery(string searchTerm, int startIndex)
        {
            return $"{SearchUrl}{searchTerm}&start={startIndex}";
        }
    }
}
