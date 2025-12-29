using UnityEngine;
using MyLib.Core.Utilities.Extensions; // Utilise nos extensions de liste pour le random
using System.Collections.Generic;

// ScriptableObject représentant un "Signal Audio" (Cue).
// Contient une liste de clips audio et une méthode pour en récupérer un.
// Permet de grouper des variations d'un même son (ex: Pas_Herbe_01, Pas_Herbe_02)
// pour éviter la répétitivité auditive.

namespace MyLib.Core.Audio.Data
{
    [CreateAssetMenu(menuName = "MyLib/Audio/Audio Cue", fileName = "NewAudioCue")]
    public class AudioCueSO : ScriptableObject
    {
        [Header("Clips")]
        [Tooltip("Liste des clips audio pouvant être joués pour ce signal.")]
        [SerializeField] private List<AudioClip> _audioClips = new List<AudioClip>();

        [Header("Sequence Logic")]
        [Tooltip("Si vrai, les sons seront joués dans l'ordre de la liste au lieu d'être aléatoires.")]
        [SerializeField] private bool _sequenceMode = false;

        private int _nextClipIndex = 0;

        // Retourne un AudioClip à jouer selon la configuration.
        // Gère la sélection aléatoire ou séquentielle.
        public AudioClip GetAudioClip()
        {
            if (_audioClips.Count == 0) return null; // Sécurité si la liste est vide.

            if (_sequenceMode)
            {
                // Récupère le clip actuel et incrémente l'index pour la prochaine fois (boucle).
                AudioClip clip = _audioClips[_nextClipIndex];
                _nextClipIndex = (_nextClipIndex + 1) % _audioClips.Count;
                return clip;
            }
            else
            {
                // Utilise l'extension de liste créée en Phase 2 pour un choix aléatoire.
                return _audioClips.GetRandom();
            }
        }
    }
}