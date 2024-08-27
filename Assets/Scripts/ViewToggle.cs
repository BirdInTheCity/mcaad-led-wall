using System;
using System.Collections;
using UnityEngine;
using UnityTimer;


namespace DefaultNamespace
{
    public class ViewToggle : MonoBehaviour
    {
        [Header("Timer (Seconds)")]
        public float ViewToggleTimer = 60.0f;


        [Header("Alpha Group References")]
        public CanvasGroup CabinetOfCuriosities;
        public CanvasGroup PortraitFlow;

        private bool cabinetActive;

        private void OnEnable()
        {
            CabinetOfCuriosities.alpha = 1.0f;
            PortraitFlow.alpha = 0.0f;
            
            Timer.Register(ViewToggleTimer, Toggle, isLooped: true);
        }


        public void Toggle()
        {
            cabinetActive = !cabinetActive;
            
            CanvasGroup cg1 = cabinetActive ? CabinetOfCuriosities : PortraitFlow;
            CanvasGroup cg2 = cabinetActive ? PortraitFlow : CabinetOfCuriosities;
            
            // Animate the alpha of the first CanvasGroup to 0
            StartCoroutine(AnimateAlpha(cg1, 0f, 1f));

            // Animate the alpha of the second CanvasGroup to 1
            StartCoroutine(AnimateAlpha(cg2, 1f, 1f));
        }
        
      
        private IEnumerator AnimateAlpha(CanvasGroup cg, float targetAlpha, float duration)
        {
            float elapsedTime = 0f;
            float startAlpha = cg.alpha;

            while (elapsedTime < duration)
            {
                cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cg.alpha = targetAlpha;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Toggle();
            }
        }
    }
}