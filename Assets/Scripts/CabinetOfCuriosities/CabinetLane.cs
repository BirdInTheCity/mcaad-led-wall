using System;
using UnityEngine;

namespace CabinetOfCuriosities
{
    public class CabinetLane : MonoBehaviour
    {
        private DownloadManager downloadManager;
        private GameObject curiosityPrefab;
        private float borderSize = 5.0f;
        private Curiosity[] placedCuriosities;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }


        public void Init(GameObject prefab, float border, int maxCuriosities, float laneWidth, float offsetX)
        {
            curiosityPrefab = prefab;
            borderSize = border;

            rectTransform.sizeDelta = new Vector2(laneWidth - border, rectTransform.sizeDelta.y);
            rectTransform.anchoredPosition = new Vector2(offsetX, rectTransform.anchoredPosition.y);

            // init download manager        
            downloadManager = FindFirstObjectByType<DownloadManager>().GetComponent<DownloadManager>();

            // init curiosities
            placedCuriosities = new Curiosity[maxCuriosities];
            for (var i = 0; i < maxCuriosities; i++)
            {
                placedCuriosities[i] = Instantiate(curiosityPrefab, transform).GetComponent<Curiosity>();
                placedCuriosities[i].UpdateTexture(downloadManager.GetNextPortrait());
            }
            RefreshSolution();
        }

        public void SwapNewCuriosity()
        {
            var newImg = downloadManager.GetNextPortrait();
            var aspectRatio = newImg.width / (float) newImg.height;
            var closestAspect = placedCuriosities[0];
            
            
            // Find the closest aspect
            foreach (var t in placedCuriosities)
            {
                if (Math.Abs(t.AspectRatio - aspectRatio) < Math.Abs(closestAspect.AspectRatio - aspectRatio)) 
                    continue;
                closestAspect = t;
                break;
            }

            closestAspect.UpdateTexture(newImg);
            RefreshSolution();
        }

        private void RefreshSolution()
        {
            var solution = Diorama.SearchSolution(rectTransform.rect.width, rectTransform.rect.height - borderSize,
                placedCuriosities, 200);
            
            if (solution == null) return;
            
            foreach (var node in solution)
            {
                var xOffset = node.X == 0 ? 0 : borderSize;
                node.Curiosity.RefreshPlacement(node.Width - xOffset, node.Height - borderSize, node.X + xOffset, node.Y + borderSize);
            }
        }
    }
}