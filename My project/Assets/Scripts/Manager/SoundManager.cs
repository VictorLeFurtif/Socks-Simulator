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
        private AudioSource finalRoundmusic;
        
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
            mainMusic = InitialisationAudioObjectDestroyAtEnd(SoundData.MainSound, true, 
                true, 0.7f, "Main Music").GetComponent<AudioSource>();
            finalRoundmusic = InitialisationAudioObjectDestroyAtEnd(SoundData.FinalRound, 
                true, true, 1f, "Final Music").GetComponent<AudioSource>();
            finalRoundmusic.Pause();
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
            StartCoroutine(AudioFadeOut.FadeOut(music,1,0));
        }
        private void FadeInMusic(AudioSource music)
        {
            StartCoroutine(AudioFadeOut.FadeIn(music,1,0.7f));
        }

        public void FadeInLastRound()
        {
            if (finalRoundmusic.isPlaying)
            {
                finalRoundmusic.Stop();
            }
    
            finalRoundmusic.time = 0;
            finalRoundmusic.Play();
            finalRoundmusic.volume = 0f; 
    
            StartCoroutine(AudioFadeOut.FadeIn(finalRoundmusic, 1));
        }

        private void FadeInResetMain()
        {
            if (mainMusic.isPlaying)
            {
                mainMusic.Stop();
            }
    
            mainMusic.time = 0;
            mainMusic.Play();
            mainMusic.volume = 0f; 
    
            FadeInMainMusic();
        }
        
        public void FadeOutLastRound()
        {
            StartCoroutine(AudioFadeOut.FadeOut(finalRoundmusic, 1));
            FadeInResetMain();
        }

        public void FadeOutMainMusic() => FadeOutMusic(mainMusic);
        public void FadeInMainMusic() => FadeInMusic(mainMusic);
    }
}
