using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance { get; private set; }
    
        public List<AudioSource> musicsEffects = new List<AudioSource>();
    
        public AudioSource audioSource;

        [Range(0,1)] public float masterVolume = 1f;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
        
        private void Start()
        {
            audioSource.volume = 1f;
        }
        
        
        private void Update()
        {
            foreach (var music in musicsEffects)
            {
                if (music != null) 
                    music.volume = masterVolume;
            }
        }
        
        public void PlayMusicOneShot(AudioClip _audioClip)
        {
            if (_audioClip == null)
            {
                Debug.LogError("The audioClip you tried to play is null");
                return;
            }
            audioSource.PlayOneShot(_audioClip);
        }
        
        
        public void UpdateMasterVolume(float volume)
        {
            masterVolume = volume;
            audioSource.volume = masterVolume;
        }

        public GameObject InitialisationAudioObjectDestroyAtEnd(AudioClip audioClipTarget, bool looping, 
            bool playingAwake, float volumeSound, string _name)
        {
            GameObject emptyObject = new GameObject(_name);
            emptyObject.transform.SetParent(gameObject.transform);

            AudioSource audioSourceGeneral = emptyObject.AddComponent<AudioSource>();
            audioSourceGeneral.clip = audioClipTarget;
            audioSourceGeneral.loop = looping;
            audioSourceGeneral.playOnAwake = playingAwake;
            audioSourceGeneral.volume = volumeSound * masterVolume;
            audioSourceGeneral.Play();
            
            if (!looping)
            {
                Destroy(emptyObject, audioClipTarget.length);
            }
            
            return emptyObject;
        }
    }
}
