using UnityEngine;
using UnityEngine.Events;
using MyLib.Core.Audio.Data; // Namespace futur pour les données Audio

// Canal d'événement spécifique pour le système Audio.
// Permet de demander la lecture d'un "AudioCue" (groupe de sons) avec une configuration spécifique
// et une position dans l'espace 3D, découplant totalement le déclencheur du gestionnaire audio.

namespace MyLib.Core.Events.EventChannels
{
    [CreateAssetMenu(menuName = "MyLib/Events/Audio Cue Channel", fileName = "AudioCueChannel")]
    public class AudioCueChannelSO : ScriptableObject
    {
        // Action complexe transportant les données du son, sa config, et sa position.
        // AudioCueSO et AudioConfigurationSO seront définis dans la Phase 4.
        public UnityAction<AudioCueSO, AudioConfigurationSO, Vector3> OnAudioCueRequested;

        // Demande la lecture d'un son.
        // audioCue : Le conteneur de clips à jouer.
        // settings : La configuration (volume, pitch) optionnelle.
        // position : L'emplacement 3D du son (Vector3.zero pour les sons 2D/UI).
        public void RaiseEvent(AudioCueSO audioCue, AudioConfigurationSO settings, Vector3 position)
        {
            if (OnAudioCueRequested != null)
            {
                OnAudioCueRequested.Invoke(audioCue, settings, position);
            }
            else
            {
                // Avertissement utile car manquer des sons est difficile à débugger visuellement.
                Debug.LogWarning($"AudioCue demandé sur {name} mais l'AudioManager ne semble pas écouter.");
            }
        }
    }
}