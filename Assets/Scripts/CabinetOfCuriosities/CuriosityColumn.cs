using System;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace CabinetOfCuriosities
{
    public class CuriosityColumn : MonoBehaviour
    {
        public int ColumnId;
        
        private DownloadManager downloadManager;
        private GameObject curiosityPrefab;
        private float borderSize = 5.0f;
        private Curiosity[] placedCuriosities;
        private RectTransform rectTransform;
        private float oldSolution;
        

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }


        public void Init(int id, GameObject prefab, float border, int maxCuriosities, Vector2 laneSize, float offsetX)
        {
            ColumnId = id;
            curiosityPrefab = prefab;
            borderSize = border;

            rectTransform.sizeDelta = new Vector2(laneSize.x - border, laneSize.y);
            rectTransform.anchoredPosition = new Vector2(offsetX, Screen.height * 0.5f);

            // init download manager        
            downloadManager = FindObjectsByType<DownloadManager>(FindObjectsSortMode.None)
                .First(item => item.instanceName == "CabinetOfCuriosities");

            // init curiosities
            placedCuriosities = new Curiosity[maxCuriosities];
            for (var i = 0; i < maxCuriosities; i++)
            {
                placedCuriosities[i] = Instantiate(curiosityPrefab, transform).GetComponent<Curiosity>();
                placedCuriosities[i].ColumnId = ColumnId;
                placedCuriosities[i].ReplaceCurrentTexture(downloadManager.GetNextPortrait());
            }
            RefreshSolution();
        }

        public Curiosity[] GetCuriosities()
        {
            return placedCuriosities;
        }

        public void SwapNewCuriosity()
        {
            var index = new Random().Next(0, placedCuriosities.Length);
            var curiosity = placedCuriosities.OrderBy(c => c.LastUpdated).First();
            curiosity.ReplaceCurrentTexture(downloadManager.GetNextPortrait());
            curiosity.LastUpdated = DateTime.Now;
            RefreshSolution();
        }

        private async void RefreshSolution()
        {
            var solution = await CuriosityPlacement.SearchSolutionAsync(rectTransform.rect.width, rectTransform.rect.height - borderSize,
                placedCuriosities, 500);

            if (solution == null)
            {
                Debug.Log("NO SOLUTION FOUND");
                return;
            }
            
            
            var nameIndex = 0; 
            foreach (var node in solution)
            {
                if (node.Curiosity == null) return;
                node.Curiosity.gameObject.name = "Curiosity " + nameIndex++;
            }
            

            foreach (var node in solution)
            {
                var c = node.Curiosity;
                var xOffset = node.X == 0 ? 0 : borderSize;
                node.Curiosity.RefreshPlacement(node.Width - xOffset, node.Height - borderSize, node.X + xOffset, node.Y + borderSize);
            }
        }
    }
}