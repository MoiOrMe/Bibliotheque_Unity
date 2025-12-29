using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using MyLib.Core.BaseClasses;
using MyLib.Core.Events.EventChannels; // Pour écouter les demandes de son
using MyLib.Core.Audio.Data; // Pour lire les AudioCueSO

// Singleton persistant responsable de tout l'audio du jeu.
// Écoute les événements du AudioCueChannelSO pour jouer des sons de manière centralisée.
// Gère un pool dynamique d'AudioSources pour éviter les instanciations répétitives.

namespace MyLib.Core.Managers
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [Header("Configuration")]
        [Tooltip("Référence au Mixer principal pour le contrôle du volume global.")]
        [SerializeField] private AudioMixer _mainMixer;

        [Header("Listening To")]
        [Tooltip("Le canal d'événement que l'AudioManager écoute pour jouer les SFX.")]
        [SerializeField] private AudioCueChannelSO _sfxEventChannel;

        [Tooltip("Le canal d'événement pour la musique.")]
        [SerializeField] private AudioCueChannelSO _musicEventChannel;

        // Initialisation et abonnement aux canaux.
        private void OnEnable()
        {
            if (_sfxEventChannel != null)
                _sfxEventChannel.OnAudioCueRequested += PlayAudioCue;

            if (_musicEventChannel != null)
                _musicEventChannel.OnAudioCueRequested += PlayMusicCue;
        }

        // Désabonnement propre.
        private void OnDisable()
        {
            if (_sfxEventChannel != null)
                _sfxEventChannel.OnAudioCueRequested -= PlayAudioCue;

            if (_musicEventChannel != null)
                _musicEventChannel.OnAudioCueRequested -= PlayMusicCue;
        }

        // Méthode principale appelée par l'événement.
        // Instancie un émetteur sonore et le configure selon les données reçues.
        private void PlayAudioCue(AudioCueSO audioCue, AudioConfigurationSO settings, Vector3 position)
        {
            AudioClip clipToPlay = audioCue.GetAudioClip();

            if (clipToPlay == null) return;

            // Crée un nouvel objet temporaire pour jouer le son.
            // Dans une version optimisée, on utiliserait le PoolManager ici.
            GameObject audioObj = new GameObject("SFX_" + clipToPlay.name);
            audioObj.transform.position = position;

            // Ajoute et configure la source audio.
            AudioSource source = audioObj.AddComponent<AudioSource>();
            source.clip = clipToPlay;

            // Applique les réglages de configuration (Volume, Pitch, Spatialisation).
            settings.ApplyTo(source);

            source.Play();

            // Lance une coroutine pour détruire l'objet une fois le son terminé.
            StartCoroutine(DestroyAudioSourceWhenFinished(source));
        }

        // Logique simplifiée pour la musique (généralement on fait du cross-fade ici).
        private void PlayMusicCue(AudioCueSO audioCue, AudioConfigurationSO settings, Vector3 position)
        {
            // TODO: Implémenter le cross-fade et la gestion de la piste musique persistante.
            // Pour l'instant, on utilise la même logique que les SFX.
            PlayAudioCue(audioCue, settings, position);
        }

        // Coroutine surveillant la fin de lecture du clip pour nettoyer la scène.
        private IEnumerator DestroyAudioSourceWhenFinished(AudioSource source)
        {
            // Attend la durée du clip audio (en temps réel pour ne pas être coupé par la pause).
            yield return new WaitForSecondsRealtime(source.clip.length);

            if (source != null && source.gameObject != null)
            {
                Destroy(source.gameObject);
            }
        }
    }
}