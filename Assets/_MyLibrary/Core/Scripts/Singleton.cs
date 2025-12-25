using UnityEngine;

namespace MyLibrary.Core
{
    /// <summary>
    /// SINGLETON GENERIQUE :
    /// Ce script sert de "moule" pour créer des Managers (Audio, Game, UI).
    /// Il garantit deux choses : 
    /// 1. Il n'y a qu'un seul exemplaire de ce script dans tout le jeu.
    /// 2. Il est accessible de partout via ".Instance".
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        // Variable privée qui stocke la référence unique vers l'objet.
        // Elle est "static" pour être partagée par tous les scripts.
        private static T _instance;

        // --- ACCÈS PUBLIC ---
        // C'est la "porte d'entrée" que les autres scripts utiliseront.
        public static T Instance
        {
            get
            {
                // Étape 1 : Est-ce qu'on connait déjà l'instance ?
                if (_instance == null)
                {
                    // Étape 2 : Si non, on cherche dans la scène si l'objet existe déjà.
                    _instance = FindFirstObjectByType<T>();

                    // Étape 3 : Si on ne le trouve toujours pas (oubli de le mettre dans la scène),
                    // on le crée automatiquement par le code. "Lazy Loading".
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;         // On nomme l'objet comme le script (ex: "AudioManager")
                        _instance = obj.AddComponent<T>(); // On lui colle le script dessus
                    }
                }
                return _instance;
            }
        }

        // --- INITIALISATION ---
        // Cette fonction se lance automatiquement au démarrage du jeu ou de l'objet.
        protected virtual void Awake()
        {
            // Cas A : Je suis le premier arrivé.
            if (_instance == null)
            {
                // Je deviens l'instance officielle.
                _instance = this as T;

                // Si je suis un objet racine,
                // je demande à Unity de ne pas me détruire quand on change de scène.
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            // Cas B : Une instance existe DÉJÀ.
            else if (_instance != this)
            {
                // Je m'autodétruis pour éviter d'avoir deux Managers qui entrent en conflit.
                Destroy(gameObject);
            }
        }
    }
}