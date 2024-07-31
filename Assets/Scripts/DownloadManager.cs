using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownloadManager : MonoBehaviour
{
    public string imagesFolderName = "Portraits";
    private string strapiImagesEndpoint = "https://picsum.photos/200/300";

    private string imagesCachePath;
    
    private Texture2D[] unusedTextures = new Texture2D[0];
    private Texture2D[] usedTextures = new Texture2D[0];

    private bool allTexturesLoaded = false;

    void Start()
    {
        imagesCachePath = Application.dataPath + "/" + imagesFolderName + "/";
        LoadAllImages();
    }
/*
    IEnumerator DownloadImagesCoroutine()
    {
        // Ensure the cache directory exists
        if (!System.IO.Directory.Exists(imagesCachePath))
        {
            System.IO.Directory.CreateDirectory(imagesCachePath);
        }

        UnityWebRequest request = UnityWebRequest.Get(strapiImagesEndpoint);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            // Process the received data to extract image URLs (assuming JSON response)
            // This needs to be adjusted based on the actual JSON structure
            List<string> imageUrls = ParseImageUrls(request.downloadHandler.text);

            // Example: Foreach URL in imageUrls, download the image
            foreach (string imageUrl in imageUrls)
            {
                // Generate a filename for the cached image
                string filename = System.IO.Path.GetFileName(imageUrl);
                string localPath = System.IO.Path.Combine(imagesCachePath, filename);

                // Check if the image is already cached
                if (!System.IO.File.Exists(localPath))
                {
                    // Download and cache the image
                    UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);
                    yield return imageRequest.SendWebRequest();
                    if (imageRequest.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
                        byte[] imageBytes = texture.EncodeToPNG();
                        System.IO.File.WriteAllBytes(localPath, imageBytes);
                    }
                    else
                    {
                        Debug.LogError(imageRequest.error);
                    }
                }

                // Load the image from cache and display it
                Texture2D loadedTexture = new Texture2D(2, 2);
                byte[] fileData = System.IO.File.ReadAllBytes(localPath);
                loadedTexture.LoadImage(fileData);
                // Assuming you have a method to create and display tiles
                // CreateTile(loadedTexture);
            }
        }
    }
    */
    
    void LoadAllImages()
    {
        
        string[] imageFiles = Directory.GetFiles(imagesCachePath, "*.*", SearchOption.TopDirectoryOnly)
            .Where(file => file.EndsWith(".jpg") || file.EndsWith(".png")).ToArray();
        
        foreach (string imagePath in imageFiles)
        {
            Texture2D texture = LoadTextureFromFile(imagePath);
            if (texture != null)
            {
                this.unusedTextures = this.unusedTextures.Append(texture).ToArray();
                // Debug.Log(imagePath + " loaded");
            }
            else
            {
                Debug.LogError("Failed to load image from path: " + imagePath);
            }
        }
        this.allTexturesLoaded = true;
        EventDispatcher.Instance.Dispatch("CACHED_IMAGES_LOADED", null);

    }
    
    public Texture2D GetNextPortrait()
    {
        // Debug.Log("Unused textures: " + unusedTextures.Length);
        if (unusedTextures == null)
            return null;


        if (unusedTextures.Length == 0)
        {
            // Shuffle the used textures into the unused textures
            unusedTextures = usedTextures.OrderBy(x => UnityEngine.Random.value).ToArray();
        }

        Texture2D texture = unusedTextures[0];
        unusedTextures = unusedTextures.Skip(1).ToArray();  
        usedTextures = usedTextures.Append(texture).ToArray();
        return texture;
    }
    
    
    Texture2D LoadTextureFromFile(string filePath)
    {
        Texture2D texture = null;
        if (System.IO.File.Exists(filePath))
        {
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2); // The size will be replaced by LoadImage.
            texture.LoadImage(fileData); // This will auto-resize the texture dimensions.
        }
        return texture;
    }
    
}