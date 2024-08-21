using UnityEngine;

namespace CabinetOfCuriosities
{
    public class CabinetLane : MonoBehaviour
    {
        private DownloadManager downloadManager;
        private GameObject curiosityPrefab;
        private float borderSize = 5.0f;
        private Texture2D[] placedCuriosities;
        private RectTransform rectTransform;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        
        public void Init(GameObject prefab, float border,  int maxCuriosities, float laneWidth, float offsetX)
        {
            this.curiosityPrefab = prefab;
            this.borderSize = border;

            rectTransform.sizeDelta = new Vector2(laneWidth - border, rectTransform.sizeDelta.y);
            rectTransform.anchoredPosition = new Vector2(offsetX, rectTransform.anchoredPosition.y);
            
            // init download manager        
            downloadManager = FindFirstObjectByType<DownloadManager>().GetComponent<DownloadManager>();
            
            // init curiosity images
            placedCuriosities = new Texture2D[maxCuriosities];
            for (var i = 0; i < maxCuriosities; i++)
            {
                placedCuriosities[i] = downloadManager.GetNextPortrait();
            }
            RefreshSolution();
            
        }

        public void SwapNewCuriosity()
        {

        }

        private void RefreshSolution()
        {
            var solution = Diorama.SearchSolution(rectTransform.rect.width, rectTransform.rect.height - borderSize, placedCuriosities, 200);
            if (solution == null) return;
            foreach (var t in solution)
            {
                if (gameObject.name == "Lane1") t.Trace();
                var xOffset = t.X == 0 ? 0 : borderSize;
                PlaceNextCuriosity(t.Texture, t.X + xOffset, t.Y + borderSize, t.Width - xOffset, t.Height - borderSize);
            }
        }
        
        private void PlaceNextCuriosity(Texture2D texture, float xOffset = 0f, float yOffset = 0f, float width = 1f, float height = 1f)
        {
            var curiosity = Instantiate(curiosityPrefab, transform).GetComponent<Curiosity>();
            curiosity.Init(texture, width, height, xOffset, yOffset);
        }
    }
}