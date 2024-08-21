using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace CabinetOfCuriosities
{
    public class CabinetOfCuriosities : MonoBehaviour
    {
        
        public GameObject curiosityPrefab; 
        public int maxCuriosities = 14;
        /*public int cols = 8;
        public int rows = 5;
        public float gapWidth = 10.0f;
        public float gapHeight = 10.0f;*/
        
        
        private DownloadManager downloadManager;
        private float defaultWidth;
        private float defaultHeight;
        private Texture2D[] pics;

        
        
        private void OnEnable()
        { 
            EventDispatcher.Instance.Subscribe("CACHED_IMAGES_LOADED", InitCuriosities);
            
            /*
            defaultWidth = (Screen.width - (gapWidth * (cols + 1))) / cols;
            defaultHeight = (Screen.height - (gapHeight * (rows + 1))) / rows;
            */

        }
        
        private void InitCuriosities(object data)
        {
            downloadManager = FindFirstObjectByType<DownloadManager>().GetComponent<DownloadManager>();

            pics = new Texture2D[maxCuriosities];

            for (var i = 0; i < maxCuriosities; i++)
            {
                var t= downloadManager.GetNextPortrait();
                pics[i] = t;
            }
            
            var solution = Diorama.SearchSolution(Screen.width, Screen.height, pics, 10000);
            if (solution == null) return;
            foreach (var t in solution)
            {
                // t.Trace();
                PlaceNextCuriosity(t.Texture, t.X, t.Y, t.Width, t.Height);
            }
        }

        private void PlaceNextCuriosity(Texture2D texture, float xOffset = 0f, float yOffset = 0f, float width = 1f, float height = 1f)
        {
            var curiosity = Instantiate(curiosityPrefab, transform).GetComponent<Curiosity>();
            curiosity.Init(this, texture, width, height, xOffset, yOffset);
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