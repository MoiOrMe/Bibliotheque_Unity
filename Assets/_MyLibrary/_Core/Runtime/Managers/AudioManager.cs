using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace MyLibrary.Core.Managers
{
    /// <summary>
    /// Gère la lecture des effets sonores (SFX) et de la musique.
    /// Utilise un pooling d'AudioSources pour optimiser les performances lors de lectures multiples.
    /// </summary>
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        #region Settings

        [Header("Audio Configuration")]
        [Tooltip("Le Mixer principal (doit exposer les paramètres 'MusicVol' et 'SFXVol').")]
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
            base.Awake(); // Important pour le PersistentSingleton

            InitializeSources();
        }

        private void Start()
        {
            InitializeVolume();
        }

        private void InitializeSources()
        {
            // Initialisation de la source musique si inexistante
            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.outputAudioMixerGroup = musicGroup;
                _musicSource.loop = true;
                _musicSource.playOnAwake = false;
            }

            // Initialisation de la source SFX si inexistante
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.outputAudioMixerGroup = sfxGroup;
                _sfxSource.playOnAwake = false;
            }
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

        /// <summary>
        /// Définit le volume dans le mixer en convertissant l'échelle linéaire (0-1) en décibels.
        /// </summary>
        public void SetMixerVolume(string paramName, float normalizedVolume)
        {
            // Conversion échelle linéaire (0-1) vers échelle logarithmique décibels (-80dB à 0dB)
            // Clamp à 0.0001 pour éviter log(0) = -infini
            float dbVolume = Mathf.Log10(Mathf.Clamp(normalizedVolume, 0.0001f, 1f)) * 20;

            if (mainMixer != null)
            {
                mainMixer.SetFloat(paramName, dbVolume);
            }
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

            StopAllCoroutines(); // Arrête tout fade en cours
            StartCoroutine(FadeMusicRoutine(newClip, fadeDuration, volume));
        }

        private IEnumerator FadeMusicRoutine(AudioClip newClip, float duration, float targetVolume)
        {
            float startVolume = _musicSource.volume;
            float halfDuration = duration / 2f;

            // Phase 1 : Fade Out
            if (_musicSource.isPlaying)
            {
                for (float t = 0; t < halfDuration; t += Time.deltaTime)
                {
                    _musicSource.volume = Mathf.Lerp(startVolume, 0, t / halfDuration);
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

                // Phase 2 : Fade In
                for (float t = 0; t < halfDuration; t += Time.deltaTime)
                {
                    _musicSource.volume = Mathf.Lerp(0, targetVolume, t / halfDuration);
                    yield return null;
                }
                _musicSource.volume = targetVolume;
            }
        }

        #endregion
    }
}