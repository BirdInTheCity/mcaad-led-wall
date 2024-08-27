using System;
using System.Collections;
using ChocDino.UIFX;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace CabinetOfCuriosities
{
    public class Curiosity : MonoBehaviour
    {
        public Texture2D Texture;
        public float AspectRatio => (Texture != null) ? (float) Texture.width / Texture.height : 0.0f;
        public bool markedForDeletion = false;

      
        private RectTransform rectTransform;
        private Image image;
        private AspectRatioFitter fitter;
        private bool initialized = false;
        private VertexSkew skew;
        
        
        void OnEnable()
        {
            image = GetComponentInChildren<Image>();
            fitter = GetComponentInChildren<AspectRatioFitter>();
            rectTransform = GetComponentInChildren<RectTransform>();
            // skew = GetComponentInChildren<VertexSkew>();
            
            rectTransform.sizeDelta = Vector2.zero;
        }
        
        public void RefreshPlacement(float width = 1f, float height = 1f, float xOffset = 0f, float yOffset = 0)
        {
            StartCoroutine(AnimatePlacement(new Vector2(xOffset, yOffset), new Vector2(width, height), 0.8f));
        }

        /*public void AnimateIn()
        {

            StartCoroutine(AnimateInCoroutine(0.8f));
        }
        
        public void AnimateOut()
        {
            // Create a coroutine to animate the Curiosity
            StartCoroutine(AnimateOutCoroutine(0.4f));
        }*/
        
        private IEnumerator AnimatePlacement(Vector2 targetPosition, Vector2 targetSize, float duration, float delay = 0f)
        {
            var startPosition = rectTransform.anchoredPosition;
            var startSize = rectTransform.sizeDelta;
            var elapsedTime = 0f;
            var startSkewStrength = 0f;
            var targetSkewStrength = 0f;

            if (!initialized)
            {
                var xOffset = 40.0f;
                var yOffset = 10.0f;
                var sizeOffset = .4f;
                var skewOffset = 11f;
                delay = .8f;
                
                startPosition = new Vector2(targetPosition.x, targetPosition.y);
                startSize = new Vector2(0f, targetSize.y);
                startSkewStrength = 1f;
                
                rectTransform.anchoredPosition = startPosition;
                rectTransform.sizeDelta = startSize;
                // skew.Strength = startSkewStrength;
            }
            
            elapsedTime = - Random.value * delay;

            while (elapsedTime < duration)
            {
                // Eased time
                var t = (float) MaterialEasing(  elapsedTime / duration);
                
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
                rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, t);
                // skew.Strength = Mathf.Lerp(startSkewStrength, targetSkewStrength, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.anchoredPosition = targetPosition;
            rectTransform.sizeDelta = targetSize;
            // skew.Strength = targetSkewStrength;
            
            initialized = true;

            yield return null;
        }
        
        private float EaseInOutQuad(float t)
        {
            if (t < 0.5f)
                return 2 * t * t;
            return -1 + (4 - 2 * t) * t;
        }
        
        
        private static double MaterialEasing(double x)
        {
            return 1 - Math.Pow(1 - x, 3);
        }
        
        
        public void UpdateTexture(Texture2D texture)
        {
            if (texture == null && image == null) return;
            image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Texture = texture;
            
            if (fitter == null) return;
            fitter.aspectRatio = (float) texture.width / texture.height;
        }

        /*
        private IEnumerator AnimateInCoroutine(float duration)
        {
            // Record the initial scale and opacity
            float initialScale = transform.localScale.x;
            float initialOpacity = GetComponent<Renderer>().material.color.a;

            // Calculate the target scale and opacity
            float targetScale = 0.4f;
            float targetOpacity = 0f;

            // Animate the scale and opacity over the duration
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                t = t * t * (3f - 2f * t); // Smoothstep function

                transform.localScale = Vector3.one * Mathf.Lerp(initialScale, targetScale, t);
                GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, Mathf.Lerp(initialOpacity, targetOpacity, t));

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the scale and opacity are exactly at the target values
            transform.localScale = Vector3.one * targetScale;
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, targetOpacity);

            
            yield return null;
        }

        private IEnumerator AnimateOutCoroutine(float duration)
        {
            // Record the initial scale and opacity
            float initialScale = transform.localScale.x;
            float initialOpacity = GetComponent<Renderer>().material.color.a;

            // Calculate the target scale and opacity
            float targetScale = 0.4f;
            float targetOpacity = 0f;

            // Animate the scale and opacity over the duration
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                t = t * t * (3f - 2f * t); // Smoothstep function

                transform.localScale = Vector3.one * Mathf.Lerp(initialScale, targetScale, t);
                GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, Mathf.Lerp(initialOpacity, targetOpacity, t));

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the scale and opacity are exactly at the target values
            transform.localScale = Vector3.one * targetScale;
            GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, targetOpacity);

            // Remove and destroy the Curiosity
            Destroy(gameObject);
        }
        */
        
        
        
        //Just hit another collider 2D
        private void OnTriggerEnter(Collider other)
        {
           Debug.Log("OnTriggerEnter");
        }
    }
    
    
}