using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace CabinetOfCuriosities
{
    public class CabinetOfCuriosities : MonoBehaviour
    {
        
        public GameObject curiosityPrefab; 
        
        [Header("Center Lane")]
        public int centerMaxPhotos = 7;
        
        [Header("Mid Lane")]
        public int midMaxPhotos = 5;
        public float midLaneWidth = 150f;

        [Header("Outer Lane")]
        public int outerMaxPhotos = 3;
        public float outerLaneWidth = 100f;

        [Header("Border Size")]
        public float borderSize = 5.0f;
        
        private DownloadManager downloadManager;
        private float defaultWidth;
        private float defaultHeight;
        private CabinetLane[] lanes;
        
        
        private void OnEnable()
        { 
            EventDispatcher.Instance.Subscribe("CACHED_IMAGES_LOADED", InitCuriosities);
            lanes = Object.FindObjectsByType<CabinetLane>(FindObjectsSortMode.None)
                .OrderBy(lane => lane.gameObject.name)
                .ToArray();
        }

        private void InitCuriosities(object data)
        {
            var offsetX = borderSize;
            
            for (var i=0; i<lanes.Length; i++)
            {
                switch (i)
                {
                    case 0:
                    case 4:
                        lanes[i].Init(curiosityPrefab, borderSize, outerMaxPhotos, outerLaneWidth, offsetX);
                        offsetX += outerLaneWidth;
                        break;
                    case 1:
                    case 3:
                        lanes[i].Init(curiosityPrefab, borderSize, midMaxPhotos, midLaneWidth, offsetX);
                        offsetX += midLaneWidth;
                        break;
                    case 2:
                        var centerLandWidth = Screen.width - midLaneWidth * 2 - outerLaneWidth * 2 - borderSize;
                        lanes[i].Init(curiosityPrefab, borderSize, centerMaxPhotos, centerLandWidth, offsetX);
                        offsetX += centerLandWidth;
                        break;
                }
            }
        }
    }
}