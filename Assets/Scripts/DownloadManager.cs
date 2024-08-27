using System.IO;
using System.Linq;
using UnityEngine;

public class DownloadManager : MonoBehaviour
{
    public string instanceName = "";
    public string imagesFolderName = "Portraits";
    private string strapiImagesEndpoint = "https://picsum.photos/200/300";

    private string imagesCachePath;
    
    private Texture2D[] unusedTextures = new Texture2D[0];
    private Texture2D[] usedTextures = new Texture2D[0];

    private bool allTexturesLoaded = false;

    private void Start()
    {
        imagesCachePath = Application.dataPath + "/" + imagesFolderName + "/";
        LoadAllImages();
    }

    void LoadAllImages()
    {
        
        var imageFiles = Directory.GetFiles(imagesCachePath, "*.*", SearchOption.TopDirectoryOnly)
            .Where(file => file.EndsWith(".jpg") || file.EndsWith(".png")).ToArray();
        
        foreach (var imagePath in imageFiles)
        {
            var texture = LoadTextureFromFile(imagePath);
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
        EventDispatcher.Instance.Dispatch("CACHED_IMAGES_LOADED", this.instanceName);

    }
    
    public Texture2D GetNextPortrait()
    {
        // Debug.Log("Unused textures: " + unusedTextures.Length);
        if (unusedTextures == null)
            return null;


        if (unusedTextures.Length == 0)
        {
            // Shuffle the used textures into the unused textures
            unusedTextures = usedTextures.OrderBy(_ => Random.value).ToArray();
        }

        var texture = unusedTextures[0];
        unusedTextures = unusedTextures.Skip(1).ToArray();  
        usedTextures = usedTextures.Append(texture).ToArray();
        return texture;
    }
    
    
    Texture2D LoadTextureFromFile(string filePath)
    {
        Texture2D texture = null;
        if (!System.IO.File.Exists(filePath)) return texture;
        var fileData = System.IO.File.ReadAllBytes(filePath);
        texture = new Texture2D(2, 2); // The size will be replaced by LoadImage.
        texture.LoadImage(fileData); // This will auto-resize the texture dimensions.
        return texture;
    }
    
}