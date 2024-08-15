using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CabinetOfCuriosities
{
    public class CabinetOfCuriosities : MonoBehaviour
    {
        
        public GameObject curiosityPrefab; 
        public float maxCuriosities = 14.0f;
        public int cols = 8;
        public int rows = 5;
        public float gapWidth = 10.0f;
        public float gapHeight = 10.0f;
        
        
        private DownloadManager downloadManager;
        private float defaultWidth;
        private float defaultHeight;

        
        
        private void OnEnable()
        { 
            EventDispatcher.Instance.Subscribe("CACHED_IMAGES_LOADED", InitCuriosities);
            
            defaultWidth = (Screen.width - (((gapWidth + 2) * cols))) / cols;
            defaultHeight = (Screen.height - ((gapHeight + 2) * rows)) / rows;
        }
        
        private void InitCuriosities(object data)
        {
            downloadManager = FindFirstObjectByType<DownloadManager>().GetComponent<DownloadManager>();


            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    PlaceNextCuriosity((defaultWidth + gapWidth) * i + gapWidth, (defaultHeight + gapHeight) * j + gapHeight, defaultWidth, defaultHeight);
                }
            }
            
            // Get the list of curiosities
            // List<Curiosity> curiosities = CuriosityManager.Instance.GetCuriosities();
        }

        private void PlaceNextCuriosity(float xOffset = 0f, float yOffset = 0f, float width = 1f, float height = 1f)
        {
            // Texture2D texture = downloadManager.GetNextPortrait();
            // if (texture == null) return;

            /*float previousY = this.GetLastPortraitY();
            float extraSpacing = UnityEngine.Random.Range(-this.spacingDeviation, this.spacingDeviation) * this.spacing;
            float xOffsetFinal = UnityEngine.Random.Range(-this.xOffset, this.xOffset);
            float scale = this.portraitScale + UnityEngine.Random.Range(-this.portraitScaleDeviation, this.portraitScaleDeviation);
            */
        
        
            var curiosity = Instantiate(curiosityPrefab, transform).GetComponent<Curiosity>();
            curiosity.Init(this, null, width, height, xOffset, yOffset);
        }
        
        
        void Update()
        {
            /*// Move the image upwards
            rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

            // Reset to bottom of the screen if the image is above the camera view
            if (!rectTransform.IsVisibleFrom() && rectTransform.GetGUIElementOffset().y < 0 && !destroyed)
            {
                Remove();
            }*/
    
        }
        
    }
}