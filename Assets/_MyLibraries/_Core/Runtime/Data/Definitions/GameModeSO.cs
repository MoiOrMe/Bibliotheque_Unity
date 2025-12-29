using UnityEngine;

// ScriptableObject définissant les règles et configurations d'un mode de jeu spécifique.
// Permet de configurer quel Prefab de joueur faire apparaître, les conditions de victoire,
// ou les règles spécifiques (ex: Temps limite, Score à atteindre).

namespace MyLib.Core.Data.Definitions
{
    [CreateAssetMenu(menuName = "MyLib/Data/Game Mode", fileName = "GameMode_New")]
    public class GameModeSO : ScriptableObject
    {
        [Header("Player Configuration")]
        [Tooltip("Le Prefab du joueur à instancier pour ce mode de jeu.")]
        public GameObject PlayerPrefab;

        [Tooltip("Le Prefab de l'interface utilisateur principale (HUD) pour ce mode.")]
        public GameObject HUDPrefab;

        [Header("Rules")]
        [Tooltip("Temps limite en secondes (0 = infini).")]
        public float TimeLimit = 0f;

        [Tooltip("Score nécessaire pour gagner (0 = pas de condition de score).")]
        public int ScoreToWin = 0;

        [Tooltip("Si vrai, le joueur respawn automatiquement après la mort.")]
        public bool AutoRespawn = true;

        [Tooltip("Délai avant le respawn en secondes.")]
        public float RespawnDelay = 3f;
    }
}