using UnityEngine;
using System.Collections.Generic;
using MyLib.Core.Utilities.Extensions; // Utilise nos extensions pour le Random

// Composant gérant les points d'apparition (Spawn Points) dans une scène.
// Permet de récupérer une position de départ pour le joueur ou les entités
// en choisissant parmi les transform enfants de cet objet.

namespace MyLib.Core.Data.Definitions
{
    public class SpawnSystem : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Couleur des gizmos dans l'éditeur pour visualiser les points.")]
        [SerializeField] private Color _gizmoColor = Color.green;

        private List<Transform> _spawnPoints = new List<Transform>();

        // Initialisation : récupère tous les enfants comme points de spawn potentiels.
        private void Awake()
        {
            foreach (Transform child in transform)
            {
                _spawnPoints.Add(child);
            }

            if (_spawnPoints.Count == 0)
            {
                // Ajoute l'objet lui-même si aucun enfant n'est défini pour éviter les erreurs.
                _spawnPoints.Add(transform);
            }
        }

        // Retourne un point de spawn aléatoire parmi ceux disponibles.
        public Transform GetRandomSpawnPoint()
        {
            return _spawnPoints.GetRandom();
        }

        // Retourne le point de spawn correspondant à un index spécifique (ex: J1, J2).
        public Transform GetSpawnPointByIndex(int index)
        {
            if (index < 0 || index >= _spawnPoints.Count)
            {
                return transform; // Retour par défaut.
            }
            return _spawnPoints[index];
        }

        // Dessine des gizmos dans l'éditeur pour visualiser les positions de spawn.
        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            foreach (Transform child in transform)
            {
                Gizmos.DrawWireSphere(child.position, 0.5f);
                Gizmos.DrawLine(child.position, child.position + child.forward * 1f); // Indique la direction.
            }
        }
    }
}