using System;
using System.Collections;
using ChocDino.UIFX;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace CabinetOfCuriosities
{
    public class Curiosity : MonoBehaviour
    {
        [FormerlySerializedAs("Texture")] public Texture2D texture;
        public Texture2D newTexture;
            public float slideTime = 1.8f;
        public float inOutTime = 0.8f;
        public float overlapTime = 0.2f;
        public float AspectRatio => (newTexture ? (float) newTexture.width / newTexture.height : 
            texture ? (float) texture.width/texture.height : 0f);

        public DateTime? LastUpdated = DateTime.MinValue;
        public int ColumnId { get; set; }

        
        private RectTransform parentRect;
        private RectTransform rectTransform;
        private Image image;
        private AspectRatioFitter fitter;
        
        // Define a color constant
        private Color darkGrey = new Color(0.2f, 0.2f, 0.2f, 0.0f);
        [FormerlySerializedAs("initialized")] public bool visible = false;
        
        
        void OnEnable()
        {
            image = GetComponentInChildren<Image>();
            fitter = GetComponentInChildren<AspectRatioFitter>();
            parentRect = GetComponent<RectTransform>();
            rectTransform = transform.GetChild(0).GetComponentInChildren<RectTransform>();

            rectTransform.localScale = Vector3.zero;
        }
        
        public void RefreshPlacement(float width = 1f, float height = 1f, float xOffset = 0f, float yOffset = 0)
        {
            var newSize = new Vector2(width, height);
            var newPosition = new Vector2(xOffset, yOffset);
            
            if (!visible)
                AnimateIn();
            else if (newTexture != null)
                AnimateOut();                   
            else
                AnimateUpdate();
            return;
            
            
            void AnimateUpdate()
            {
                var mySequence = DOTween.Sequence();
                mySequence.Insert(inOutTime - overlapTime, parentRect.DOAnchorPos(newPosition, slideTime).SetEase(Ease.InOutExpo));
                mySequence.Insert(inOutTime - overlapTime, parentRect.DOSizeDelta(newSize, slideTime).SetEase(Ease.InOutExpo));

                mySequence.Play();
                return;
            }

            void AnimateOut()
            {
                visible = false;

                var mySequence = DOTween.Sequence();
                mySequence.Insert(0, rectTransform.DORotate(new Vector3(0f, 90f, 0f), inOutTime).SetEase(Ease.InExpo));
                mySequence.Insert(0, rectTransform.DOScale(new Vector3(.6f, .6f, 1f), inOutTime).SetEase(Ease.InExpo));
                mySequence.Insert(0, image.DOColor(darkGrey, inOutTime).SetEase(Ease.InExpo));
                mySequence.InsertCallback(inOutTime, () =>RefreshPlacement(width, height, xOffset, yOffset));
                mySequence.Play();
                return;
            }

            void AnimateIn()
            {
                UpdateTexture();

                // Set start values
                parentRect.anchoredPosition = newPosition;
                parentRect.sizeDelta = newSize;
                rectTransform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                rectTransform.localScale = new Vector3(.6f, .6f, 1f);
                image.color = darkGrey;
                
                var mySequence = DOTween.Sequence();
                mySequence.Insert(slideTime - 2 * overlapTime, rectTransform.DORotate(Vector3.zero, inOutTime).SetEase(Ease.OutExpo));
                mySequence.Insert(slideTime - 2 * overlapTime, rectTransform.DOScale(Vector3.one, inOutTime).SetEase(Ease.OutExpo));
                mySequence.Insert(slideTime - 2 * overlapTime, image.DOColor(Color.white, inOutTime).SetEase(Ease.OutExpo));
                mySequence.Play();
                
                visible = true; 
                return;
            }
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

        public void ReplaceCurrentTexture(Texture2D incomingTexture)
        {
            newTexture = incomingTexture;
        }
        
        
        private void UpdateTexture()
        {
            if (newTexture == null) return;
            texture = newTexture;
            newTexture = null;
            
            image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            fitter.aspectRatio = (float) texture.width / texture.height;
        }
        
        
        
        //Just hit another collider 2D
        private void OnTriggerEnter(Collider other)
        {
           Debug.Log("OnTriggerEnter");
        }
    }
    
    
}