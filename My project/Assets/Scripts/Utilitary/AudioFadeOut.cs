using System.Collections;
using UnityEngine;

namespace Utilitary
{
    public static class AudioFadeOut {

        public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime, float targetVolume = 0f)
        {
            float startVolume = audioSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / fadeTime;
        
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, progress);
                yield return null;
            }
    
            audioSource.volume = targetVolume;
        }

        public static IEnumerator FadeIn(AudioSource audioSource, float fadeTime, float targetVolume = 1f)
        {
            float startVolume = audioSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / fadeTime;
        
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, progress);
                yield return null;
            }
    
            audioSource.volume = targetVolume;
        }
    }
}