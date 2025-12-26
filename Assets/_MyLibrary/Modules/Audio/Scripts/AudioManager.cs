using UnityEngine;
using System.Collections;
using MyLibrary.Core;

namespace MyLibrary.Modules.Audio
{
    /// <summary>
    /// Gestionnaire Audio centralisé.
    /// Gère la musique de fond (avec fondu) et les bruitages (SFX).
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        // On crée deux sources audio virtuelles : une pour la musique, une pour les bruitages.
        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        protected override void Awake()
        {
            base.Awake();

            // Création automatique des composants AudioSource sur l'objet du Manager.

            // 1. Configuration de la source MUSIQUE
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true; // La musique doit tourner en boucle

            // 2. Configuration de la source SFX
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop = false; // Un bruitage ne boucle pas
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