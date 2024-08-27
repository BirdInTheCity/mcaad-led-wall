using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PortraitFlowColumnConfig
{
    public float spacing;
    public float spacingDeviation;
    public float portraitScale;
    public float portraitScaleDeviation;
    public float xOffset;

}

public class PortraitFlowColumn : MonoBehaviour
{
    // Assume these are set in the Unity Editor or somewhere in your code
    public GameObject portraitPrefab; 
    
    public float xOffset = 10.0f;

    public float portraitScale = 1.0f;
    public float portraitScaleDeviation = 0.1f;
    public float spacing = 80f; // Spacing between tiles
    public float spacingDeviation = 0;

    private DownloadManager downloadManager;
    
    private int imgCount = 0;

    private void OnEnable()
    { 
        EventDispatcher.Instance.Subscribe("CACHED_IMAGES_LOADED", InitPortraits);
    }
    
    private void InitPortraits(object data)
    {
        if ((string) data != "PortraitFlow") return;

        
        downloadManager = FindObjectsByType<DownloadManager>(FindObjectsSortMode.None)
            .First(item => item.instanceName == "PortraitFlow");
        
        for(int i = 0; i < 4; i++)
        {
            PlaceNextPortrait();
        }
    }

    public void PlaceNextPortrait()
    {
        Texture2D texture = downloadManager.GetNextPortrait();
        if (texture == null) return;

        float previousY = this.GetLastPortraitY();
        float extraSpacing = UnityEngine.Random.Range(-this.spacingDeviation, this.spacingDeviation) * this.spacing;
        float xOffsetFinal = UnityEngine.Random.Range(-this.xOffset, this.xOffset);
        float scale = this.portraitScale + UnityEngine.Random.Range(-this.portraitScaleDeviation, this.portraitScaleDeviation);
        
        
        PortraitManager portrait = Instantiate(portraitPrefab, transform).GetComponent<PortraitManager>();
        portrait.SetPortrait(this, texture, scale, previousY - this.spacing - extraSpacing, xOffsetFinal);
        imgCount++;
        
    }
    
    private float GetLastPortraitY()
    {
        if (transform.childCount == 0) 
            return Screen.height;
        
        PortraitManager lastPortrait = transform.GetChild(transform.childCount - 1).GetComponent<PortraitManager>();
        return lastPortrait.GetY();
    }

    Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
        
    
}