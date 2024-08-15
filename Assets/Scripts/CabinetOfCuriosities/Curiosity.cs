using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CabinetOfCuriosities
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class Curiosity : MonoBehaviour
    {
      
        private RectTransform rectTransform;
        private CabinetOfCuriosities parent;
        
       
        
        void OnEnable()
        {
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

        public void Init(CabinetOfCuriosities cabinet, Texture2D texture, 
            float width = 1f, float height = 1f, float xOffset = 0f, float yOffset = 0f)
        {
            this.parent = cabinet;
            rectTransform = GetComponent<RectTransform>();
            this.rectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
            this.rectTransform.sizeDelta = new Vector2(width, height);

            if (texture == null) return;
            GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            
        }
        
        void Update()
        {
            
        }

            
        void Remove()
        {
            Destroy(this.gameObject);
        }
    }
    
    
}