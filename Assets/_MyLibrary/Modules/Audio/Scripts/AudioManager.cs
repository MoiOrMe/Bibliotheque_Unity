using MyLibrary.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace MyLibrary.Modules.Audio
{
    /// <summary>
    /// Gestionnaire Audio centralisé.
    /// Gère la musique de fond (avec fondu) et les bruitages (SFX).
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Configuration Mixer")]
        public AudioMixer mainMixer; // Référence au fichier MainMixer
        public AudioMixerGroup musicGroup; // Le groupe Music
        public AudioMixerGroup sfxGroup; // Le groupe SFX

        // On crée deux sources audio virtuelles : une pour la musique, une pour les bruitages.
        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        protected override void Awake()
        {
            base.Awake();

            // Création dynamique des sources si elles n'existent pas
            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.outputAudioMixerGroup = musicGroup;
                _musicSource.loop = true;
            }

            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.outputAudioMixerGroup = sfxGroup;
            }
        }

        private void Start()
        {
            InitializeVolume();
        }

        public void InitializeVolume()
        {
            // On récupère les valeurs sauvegardées ou 0.75 par défaut
            float musicVol = PlayerPrefs.GetFloat("MusicVol", 0.75f);
            float sfxVol = PlayerPrefs.GetFloat("SFXVol", 0.75f);

            // On applique la formule logarithmique tout de suite
            SetMixerVolume("MusicVol", musicVol);
            SetMixerVolume("SFXVol", sfxVol);
        }

        private void SetMixerVolume(string paramName, float normalizedVolume)
        {
            // La même formule que dans l'UI
            float dbVolume = Mathf.Log10(Mathf.Clamp(normalizedVolume, 0.0001f, 1f)) * 20;

            if (mainMixer != null)
            {
                mainMixer.SetFloat(paramName, dbVolume);
            }
        }

        /// <summary>
        /// Joue un bruitage (SFX).
        /// </summary>
        /// <param name="clip">Le fichier son à jouer</param>
        /// <param name="volume">Volume (0 à 1), par défaut à 1</param>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;

            // PlayOneShot permet de jouer plusieurs sons en même temps
            // sur la même source sans qu'ils se coupent la parole.
            // Idéal pour des coups de feu rapides ou des pièces ramassées.
            _sfxSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Change la musique de fond avec un fondu et un volume spécifique.
        /// </summary>
        /// <param name="newClip">La nouvelle musique</param>
        /// <param name="fadeDuration">Temps de transition</param>
        /// <param name="volume">Volume cible (0 à 1)</param>
        public void PlayMusic(AudioClip newClip, float fadeDuration = 1.0f, float volume = 1.0f)
        {
            if (_musicSource.clip == newClip) return;
            StartCoroutine(FadeMusicRoutine(newClip, fadeDuration, volume));
        }

        // --- OUTILS INTERNES (Coroutines) ---
        private IEnumerator FadeMusicRoutine(AudioClip newClip, float duration, float targetVolume)
        {
            float startVolume = _musicSource.volume;

            // 1. Fade Out
            if (_musicSource.isPlaying)
            {
                for (float t = 0; t < duration / 2; t += Time.deltaTime)
                {
                    _musicSource.volume = Mathf.Lerp(startVolume, 0, t / (duration / 2));
                    yield return null;
                }
            }

            // 2. Changement
            _musicSource.volume = 0;
            _musicSource.Stop();
            _musicSource.clip = newClip;

            if (newClip != null)
            {
                _musicSource.Play();

                // 3. Fade In
                for (float t = 0; t < duration / 2; t += Time.deltaTime)
                {
                    _musicSource.volume = Mathf.Lerp(0, targetVolume, t / (duration / 2));
                    yield return null;
                }
                _musicSource.volume = targetVolume;
            }
        }

        // --- CONTRÔLE DE VOLUME ---
        public void SetMusicVolume(float volume)
        {
            _musicSource.volume = Mathf.Clamp01(volume);
        }
    }
}