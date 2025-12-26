using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.Modules.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        #region Configuration

        [Header("Noms des Scènes")]
        // Les noms doivent correspondre exactement à ceux dans File > Build Settings
        public string sceneFPS = "Demo_FPS";
        public string sceneTPSExplo = "Demo_TPS_Explo";
        public string sceneTPSCombat = "Demo_TPS_Combat";
        public string sceneSideView = "Demo_SideView";

        #endregion

        #region Button Events

        // Méthodes reliées aux boutons via l'inspecteur Unity
        public void OnClick_FPS()
        {
            SceneLoader.Instance.LoadScene(sceneFPS);
        }

        public void OnClick_TPS_Explo()
        {
            SceneLoader.Instance.LoadScene(sceneTPSExplo);
        }

        public void OnClick_TPS_Combat()
        {
            SceneLoader.Instance.LoadScene(sceneTPSCombat);
        }

        public void OnClick_SideView()
        {
            SceneLoader.Instance.LoadScene(sceneSideView);
        }

        public void OnClick_Quit()
        {
            SceneLoader.Instance.QuitGame();
        }

        #endregion
    }
}