using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Utilitary;

namespace Manager
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager instance;
        
        public AudioSource audioSource;

        [Range(0,1)] public float masterVolume = 1f;

        [SerializeField] private SoundBankData soundBankData;

        public SoundBankData SoundData => soundBankData;

        private AudioSource mainMusic;
        
        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<SoundManager>();
                    
                    if (instance == null)
                    {
                        GameObject soundManager = new GameObject("SoundManager");
                        instance = soundManager.AddComponent<SoundManager>();
                    }
                }
                return instance;
            }
        }
        
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
            mainMusic = InitialisationAudioObjectDestroyAtEnd(SoundData.MainSound, true, true, 1f, "Main Music").GetComponent<AudioSource>();
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

        public IEnumerator SayWinner(AudioClip winner)
        {
            PlayMusicOneShot(winner);
            yield return new WaitForSecondsRealtime(winner.length);
            PlayMusicOneShot(SoundData.Win);
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

        private void FadeOutMusic(AudioSource music)
        {
            StartCoroutine(AudioFadeOut.FadeOut(music,1));
        }
        private void FadeInMusic(AudioSource music)
        {
            StartCoroutine(AudioFadeOut.FadeIn(music,1));
        }

        public void FadeOutMainMusic() => FadeOutMusic(mainMusic);
        public void FadeInMainMusic() => FadeInMusic(mainMusic);
    }
}
