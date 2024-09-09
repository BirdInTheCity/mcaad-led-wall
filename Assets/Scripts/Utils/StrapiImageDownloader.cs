using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Utils
{
    public class StrapiImageDownloader: MonoBehaviour
    {
        private string _strapiUrl;
        private string _apiKey;
        private string downloadPath;

        private void Start()
        {
            Init(
                "http://167.71.164.46:1337/api/portraits?filters%5Bactive%5D%5B$eq%5D=true&sort=id:desc&populate=*",
                "");
        }

       
        public async void Init(string strapiUrl, string apiKey)
        {
            _strapiUrl = strapiUrl;
            _apiKey = apiKey;
            downloadPath = Application.streamingAssetsPath + "/Portraits/";
           
            var response = JsonConvert.DeserializeObject<StrapiResponse>(await GetResponseAsync());
            DownloadImagesAsync(response);
            // Debug.Log(response.data[0].attributes.image.data.attributes.url);
        }
        
        private async Task DownloadImagesAsync(StrapiResponse response)
        {
            foreach (var item in response.data)
            {
                var url = item.attributes.image.data.attributes.url;
                if (url == null) continue;
                await DownloadImageAsync(url);
            }
        }
        
        private async Task<string> GetResponseAsync()
        {
            // Set the API key in the headers
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            // Send a GET request to the API endpoint
            var response = await httpClient.GetAsync(_strapiUrl);

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                // Get the response content as a string
                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            else
            {
                throw new Exception("Failed to retrieve response");
            }
        }


        private async Task DownloadImageAsync(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var filename = System.IO.Path.GetFileName(uri.LocalPath);
            var filePath = Path.Combine(downloadPath, filename);
            
            // Check local cache
            if (File.Exists(filePath))
            {
                // Debug.Log($"Image already exists in local cache: {filePath}");
                return;
            }
            
            // Set the API key in the headers
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            // Send a GET request to the API to retrieve the image
            var response = await httpClient.GetAsync(imageUrl);

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(filePath, imageBytes);
                Console.WriteLine($"Image downloaded successfully to {downloadPath}");
            }
            else
            {
                Console.WriteLine($"Error downloading image: {response.StatusCode}");
            }
        }
    }
    
    [Serializable]
    public class StrapiResponse
    {
        [JsonProperty(PropertyName = "data")]
        public Data[] data { get; set; }

        public int GetLength()
        {
            return data.Length;
        }
        // Trace data
        
    }

    [Serializable]

    public class Data
    {
        [JsonProperty(PropertyName = "attributes")]
        public Attributes attributes { get; set; }
    }
    [Serializable]

    public class Attributes
    {
        public string name { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
        
        [JsonProperty(PropertyName = "image")]
        public Image image { get; set; }
        
        [JsonProperty(PropertyName = "backgroundImage")]
        public BackgroundImage backgroundImage { get; set; }
    }
    [Serializable]

    public class Image
    {
        public Data data { get; set; }
        
        [JsonProperty(PropertyName = "attributes")]
        public Attributes attributes { get; set; }
    }
    [Serializable]

    public class BackgroundImage
    {
        [JsonProperty(PropertyName = "attributes")]
        public Attributes attributes { get; set; }
    }

    public class ImageData
    {
        
    }
}