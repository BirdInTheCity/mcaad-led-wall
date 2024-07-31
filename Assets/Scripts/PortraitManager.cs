using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PortraitManager : MonoBehaviour
{
    public float baseSpeed = 25f; // Base speed for movement
    private RectTransform rectTransform;
    private float moveSpeed;
    private float startY;
    private PortraitFlowColumn portraitFlowColumn;
    private bool destroyed = false;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        startY = rectTransform.anchoredPosition.y;
        
        

    }
    
    public void SetPortrait(PortraitFlowColumn column, Texture2D texture, float size = 1.0f,float yOffset = 0f, float xOffset = 0f)
    {
        if (texture == null) return;
        this.portraitFlowColumn = column;
        GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        this.rectTransform.sizeDelta = new Vector2(texture.width * size, texture.height * size);
        this.rectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
        
        // Calculate move speed based on the scale of the image
        // Smaller images (smaller scale) move slower
        moveSpeed = baseSpeed * size * 4f;
    }

    public float GetY()
    {
        return this.rectTransform.anchoredPosition.y;
    }

    void Update()
    {
        // Move the image upwards
        rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

        // Reset to bottom of the screen if the image is above the camera view
        if (!rectTransform.IsVisibleFrom() && rectTransform.GetGUIElementOffset().y < 0 && !destroyed)
        {
            RemovePortrait();
        }
    
    }
    
    void RemovePortrait()
    {
        this.destroyed = true;
        this.portraitFlowColumn.PlaceNextPortrait();
        Destroy(this.gameObject);
    }
    
}