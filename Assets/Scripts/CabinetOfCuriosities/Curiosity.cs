using UnityEngine;
using UnityEngine.UI;


namespace CabinetOfCuriosities
{
    public class Curiosity : MonoBehaviour
    {
      
        private RectTransform rectTransform;
        private Image image;
        private AspectRatioFitter fitter;
       
        
        void OnEnable()
        {
            image = GetComponentInChildren<Image>();
            fitter = GetComponentInChildren<AspectRatioFitter>();
        }

        
        private void AnimateIn()
        {
            // Animate the image in
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }
        
        private void AnimateOut()
        {
            // Animate the image out
            rectTransform.anchoredPosition = new Vector2(0, 1000);
        }

        public void Init(Texture2D texture, float width = 1f, 
            float height = 1f, float xOffset = 0f, float yOffset = 0)
        {
            rectTransform = GetComponent<RectTransform>();
            this.rectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
            this.rectTransform.sizeDelta = new Vector2(width, height);

            if (texture == null && image == null) return;
            image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            if (fitter == null) return;
            fitter.aspectRatio = (float) texture.width / texture.height;  // width / height > 1 ? .1f : 3.0f;
        }
        
    
            
        void Remove()
        {
            Destroy(this.gameObject);
        }
    }
    
    
}