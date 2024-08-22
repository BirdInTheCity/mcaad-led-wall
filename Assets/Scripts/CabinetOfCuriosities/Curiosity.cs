using System.Collections;
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
        private bool initialized = false;
        
        
        void OnEnable()
        {
            image = GetComponentInChildren<Image>();
            fitter = GetComponentInChildren<AspectRatioFitter>();
            rectTransform = GetComponent<RectTransform>();
        }
        
        public void RefreshPlacement(float width = 1f, float height = 1f, float xOffset = 0f, float yOffset = 0)
        {
            StartCoroutine(AnimatePlacement(new Vector2(xOffset, yOffset), new Vector2(width, height), 1.0f));
        }
        
        private IEnumerator AnimatePlacement(Vector2 targetPosition, Vector2 targetSize, float duration)
        {
            Vector2 startPosition = rectTransform.anchoredPosition;
            Vector2 startSize = rectTransform.sizeDelta;
            float elapsedTime = 0f;

            if (!initialized)
            {
                duration = 0f;
                initialized = true;
            }

            while (elapsedTime < duration)
            {
                float t = EaseInOutQuad(elapsedTime / duration);
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
                rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.anchoredPosition = targetPosition;
            rectTransform.sizeDelta = targetSize;
        }
        
        private float EaseInOutQuad(float t)
        {
            if (t < 0.5f)
                return 2 * t * t;
            return -1 + (4 - 2 * t) * t;
        }
        
        private float CubicBezier(float t, float p0, float p1, float p2, float p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            return (uuu * p0) + (3 * uu * t * p1) + (3 * u * tt * p2) + (ttt * p3);
        }
        
        private float MaterialEase(float t)
        {
            return CubicBezier(t, 0.4f, 0.0f, 0.2f, 1.0f);
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