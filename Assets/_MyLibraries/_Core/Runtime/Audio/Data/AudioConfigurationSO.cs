using UnityEngine;
using UnityEngine.Audio;

// ScriptableObject contenant les paramètres de configuration pour un AudioSource.
// Permet de définir des variations de volume, de pitch et de spatialisation
// qui seront appliquées dynamiquement au moment de la lecture du son.

namespace MyLib.Core.Audio.Data
{
    [CreateAssetMenu(menuName = "MyLib/Audio/Audio Configuration", fileName = "AudioConfiguration")]
    public class AudioConfigurationSO : ScriptableObject
    {
        [Header("Properties")]
        [Tooltip("Groupe de mixage de sortie (ex: SFX, Music).")]
        public AudioMixerGroup OutputAudioMixerGroup = null;

        [Tooltip("Priorité du son (0 = Trés haute priorité, 256 = Basse).")]
        [Range(0, 256)] public int Priority = 128;

        [Header("Sound Properties")]
        [Tooltip("Volume de base du son.")]
        [Range(0f, 1f)] public float Volume = 1f;

        [Tooltip("Hauteur (vitesse) de lecture du son.")]
        [Range(0.1f, 3f)] public float Pitch = 1f;

        [Header("Spatialisation")]
        [Tooltip("Mélange entre 2D (0) et 3D (1).")]
        [Range(0f, 1f)] public float SpatialBlend = 1f;

        [Tooltip("Distance au-delà de laquelle le son n'est plus audible.")]
        public float MaxDistance = 50f;

        [Tooltip("Type d'atténuation du volume avec la distance.")]
        public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;

        // Applique cette configuration à un AudioSource donné.
        // Modifie les propriétés de la source audio pour correspondre à ces réglages.
        public void ApplyTo(AudioSource audioSource)
        {
            audioSource.outputAudioMixerGroup = OutputAudioMixerGroup;
            audioSource.priority = Priority;
            audioSource.volume = Volume;
            audioSource.pitch = Pitch;
            audioSource.spatialBlend = SpatialBlend;
            audioSource.maxDistance = MaxDistance;
            audioSource.rolloffMode = RolloffMode;
        }
    }
}