using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilitary
{
    public static class UiHelper
    {
        private static Dictionary<Slider, Coroutine> activeCoroutines = new Dictionary<Slider, Coroutine>();
        
        public static void UpdateSlider(MonoBehaviour caller, Slider slider, float targetValue, float duration = 0.3f)
        {
            if (activeCoroutines.ContainsKey(slider) && activeCoroutines[slider] != null)
            {
                caller.StopCoroutine(activeCoroutines[slider]);
            }

            Coroutine newCoroutine = caller.StartCoroutine(SmoothTransitionSlider(slider, targetValue, duration));
            activeCoroutines[slider] = newCoroutine;
        }

        public static Coroutine UpdateSliderCoroutine(MonoBehaviour caller, Slider slider, float targetValue, float duration = 0.3f)
        {
            if (activeCoroutines.ContainsKey(slider) && activeCoroutines[slider] != null)
            {
                caller.StopCoroutine(activeCoroutines[slider]);
            }

            Coroutine newCoroutine = caller.StartCoroutine(SmoothTransitionSlider(slider, targetValue, duration));
            activeCoroutines[slider] = newCoroutine;
    
            return newCoroutine; 
        }
        
        private static IEnumerator SmoothTransitionSlider(Slider slider, float targetValue, float duration)
        {
            float elapsed = 0f;
            float startValue = slider.value;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                slider.value = Mathf.Lerp(startValue, targetValue, t);
                yield return null;
            }

            slider.value = targetValue;
            if (activeCoroutines.ContainsKey(slider))
            {
                activeCoroutines.Remove(slider);
            }
        }
    }
}