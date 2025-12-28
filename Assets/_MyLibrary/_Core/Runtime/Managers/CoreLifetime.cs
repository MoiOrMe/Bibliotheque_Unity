using UnityEngine;

namespace MyLibrary.Core.Managers
{
    /// <summary>
    /// Placez ce script sur l'objet racine "Core_Managers".
    /// Il assure que toute la hiérarchie (Game, Audio, UI, Scene) survit entre les scènes.
    /// </summary>
    public class CoreLifetime : MonoBehaviour
    {
        private static CoreLifetime _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                // Si un Core_Managers existe déjà (ex: retour au menu), on détruit le nouveau doublon
                Destroy(gameObject);
                return;
            }

            _instance = this;

            // C'est ici qu'on sauve tout le groupe d'un coup
            DontDestroyOnLoad(gameObject);
        }
    }
}