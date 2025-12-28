using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyLibrary.Core
{
    public class BootLoader : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Le nom exact de la scène du menu principal.")]
        public string nextSceneName = "Menu_Hub";

        private void Start()
        {
            // On charge la scène suivante immédiatement
            SceneManager.LoadScene(nextSceneName);
        }
    }
}