using MyLibrary.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace MyLibrary.Modules.Audio
{
    /// <summary>
    /// Gestionnaire Audio centralisé (Singleton).
    /// Gère la lecture de la musique (avec cross-fade) et des bruitages via AudioMixer.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        #region Settings

        [Header("Audio Configuration")]
        public AudioMixer mainMixer;
        public AudioMixerGroup musicGroup;
        public AudioMixerGroup sfxGroup;

        #endregion

        #region Internal State

        // Sources audio virtuelles générées au runtime
        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        #endregion

        #region Initialization

        protected override void Awake()
        {
            base.Awake();

            // Initialisation de la source musique si inexistante
            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.outputAudioMixerGroup = musicGroup;
                _musicSource.loop = true;
            }

            // Initialisation de la source SFX si inexistante
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.outputAudioMixerGroup = sfxGroup;
            }
        }

        private void Start()
        {
            // Application des volumes sauvegardés dès le démarrage
            InitializeVolume();
        }

        #endregion

        #region Volume Management

        /// <summary>
        /// Charge les préférences utilisateur (PlayerPrefs) et applique les volumes au Mixer.
        /// </summary>
        public void InitializeVolume()
        {
            float musicVol = PlayerPrefs.GetFloat("MusicVol", 0.75f);
            float sfxVol = PlayerPrefs.GetFloat("SFXVol", 0.75f);

            SetMixerVolume("MusicVol", musicVol);
            SetMixerVolume("SFXVol", sfxVol);
        }

        private void SetMixerVolume(string paramName, float normalizedVolume)
        {
            // Conversion échelle linéaire (0-1) vers échelle logarithmique décibels (-80dB à 0dB)
            float dbVolume = Mathf.Log10(Mathf.Clamp(normalizedVolume, 0.0001f, 1f)) * 20;

            if (mainMixer != null)
            {
                mainMixer.SetFloat(paramName, dbVolume);
            }
        }

        /// <summary>
        /// Ajuste le volume de la source musique (utilisé pour les fondus internes).
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            _musicSource.volume = Mathf.Clamp01(volume);
        }

        #endregion

        #region Playback Methods

        /// <summary>
        /// Joue un clip audio en tant que bruitage (sans interrompre les autres sons).
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Lance une nouvelle musique avec une transition en fondu enchaîné.
        /// </summary>
        public void PlayMusic(AudioClip newClip, float fadeDuration = 1.0f, float volume = 1.0f)
        {
            if (_musicSource.clip == newClip) return;
            StartCoroutine(FadeMusicRoutine(newClip, fadeDuration, volume));
        }

        private IEnumerator FadeMusicRoutine(AudioClip newClip, float duration, float targetVolume)
        {
            float startVolume = _musicSource.volume;

            // Phase de Fade Out
            if (_musicSource.isPlaying)
            {
                for (float t = 0; t < duration / 2; t += Time.deltaTime)
                {
                    _musicSource.volume = Mathf.Lerp(startVolume, 0, t / (duration / 2));
                    yield return null;
                }
            }

            // Changement du clip
            _musicSource.volume = 0;
            _musicSource.Stop();
            _musicSource.clip = newClip;

            if (newClip != null)
            {
                _musicSource.Play();

                // Phase de Fade In
                for (float t = 0; t < duration / 2; t += Time.deltaTime)
                {
                    _musicSource.volume = Mathf.Lerp(0, targetVolume, t / (duration / 2));
                    yield return null;
                }
                _musicSource.volume = targetVolume;
            }
        }

        #endregion
    }
}