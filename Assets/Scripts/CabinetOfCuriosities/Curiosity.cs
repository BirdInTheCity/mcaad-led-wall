using UnityEngine;
using UnityEngine.UI;


namespace CabinetOfCuriosities
{
    public class Curiosity : MonoBehaviour
    {
        public Texture2D Texture;
        public float AspectRatio => (Texture != null) ? (float) Texture.width / Texture.height : 0.0f;

      
        private RectTransform rectTransform;
        private Image image;
        private AspectRatioFitter fitter;
        
        
        void OnEnable()
        {
            image = GetComponentInChildren<Image>();
            fitter = GetComponentInChildren<AspectRatioFitter>();
            rectTransform = GetComponent<RectTransform>();
        }
        
        public void RefreshPlacement(float width = 1f, float height = 1f, float xOffset = 0f, float yOffset = 0)
        {
            this.rectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
            this.rectTransform.sizeDelta = new Vector2(width, height);
        }
        
        public void UpdateTexture(Texture2D texture)
        {
            if (texture == null && image == null) return;
            image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Texture = texture;
            
            if (fitter == null) return;
            fitter.aspectRatio = (float) texture.width / texture.height;
        }
    }
    
    
}