using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityTimer;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace CabinetOfCuriosities
{
    public class CabinetOfCuriosities : MonoBehaviour
    {
        
        public GameObject curiosityPrefab; 
        [Header("Timer (Seconds)")]
        public float imageSwapTimer = 3.0f;
        
        
        [Header("Center Lane")]
        public int centerMaxPhotos = 7;
        public float centerLaneHeight = 100f;
        
        [Header("Mid Lane")]
        public int midMaxPhotos = 5;
        public float midLaneWidth = 22f;
        public float midLaneHeight = 85f;

        [Header("Outer Lane")]
        public int outerMaxPhotos = 3;
        public float outerLaneWidth = 13f;
        public float outerLaneHeight = 68f;

        [Header("Border Size")]
        public float borderSize = 5.0f;
        
        private float defaultWidth;
        private float defaultHeight;
        private CuriosityColumn[] lanes;
        private List<Curiosity> curiosities;
        private int prevLane;
        
        
        private void OnEnable()
        { 
            this.GetComponent<RectTransform>().sizeDelta = Vector2.one;
            EventDispatcher.Instance.Subscribe("CACHED_IMAGES_LOADED", InitCuriosities);
            lanes = Object.FindObjectsByType<CuriosityColumn>(FindObjectsSortMode.None)
                .OrderBy(lane => lane.gameObject.name)
                .ToArray();
            
            DOTween.Init();

        }

        private void InitCuriosities(object data)
        {
            if ((string) data != "CabinetOfCuriosities") return;
            
            var offsetX = borderSize;
            var screenWidth = Screen.width - borderSize;
            var laneSizeOuter = new Vector2(outerLaneWidth * screenWidth / 100, Screen.height * outerLaneHeight / 100f);
            var laneSizeMid = new Vector2(midLaneWidth * screenWidth / 100, Screen.height * midLaneHeight / 100f);
            var laneSizeCenter = new Vector2(screenWidth - (laneSizeOuter.x + laneSizeMid.x) * 2, Screen.height * centerLaneHeight / 100f);

            curiosities = new List<Curiosity>();

            for (var i=0; i<lanes.Length; i++)
            {
                switch (i)
                {
                    case 0:
                    case 4:
                        lanes[i].Init(i, curiosityPrefab, borderSize, outerMaxPhotos, laneSizeOuter, offsetX);
                        offsetX += laneSizeOuter.x;
                        break;
                    case 1:
                    case 3:
                        lanes[i].Init(i, curiosityPrefab, borderSize, midMaxPhotos, laneSizeMid, offsetX);
                        offsetX += laneSizeMid.x;
                        break;
                    case 2:
                        lanes[i].Init(i, curiosityPrefab, borderSize, centerMaxPhotos, laneSizeCenter, offsetX);
                        offsetX += laneSizeCenter.x;
                        break;
                }
                lanes[i].GetCuriosities().ToList().ForEach(curiosity => curiosities.Add(curiosity));
            }
            AssignLifetimeDates();
            Timer.Register(imageSwapTimer, SwapNewCuriosity, isLooped: true);
        }
        
        private void SwapNewCuriosity()
        {
            if (lanes == null || lanes.Length == 0) return;

            int newLaneIndex;
            do
            {
                newLaneIndex = UnityEngine.Random.Range(0, lanes.Length);
            } while (newLaneIndex == prevLane);

            prevLane = newLaneIndex;
            lanes[newLaneIndex].SwapNewCuriosity();        
        }

        private void AssignLifetimeDates()
        {
            var shuffled = CuriosityPlacement.Shuffle(curiosities.ToArray());
            for (var i = 0; i < shuffled.Length; i++)
            {
                shuffled[i].LastUpdated = DateTime.Now.AddDays(-i);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SwapNewCuriosity();
            }
        }
    }
}