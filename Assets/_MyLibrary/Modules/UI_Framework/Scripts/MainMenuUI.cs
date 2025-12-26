using UnityEngine;
using MyLibrary.Core;

namespace MyLibrary.Modules.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        // Noms exacts des scènes (doivent correspondre aux fichiers dans Build Settings)
        [Header("Noms des Scènes")]
        public string sceneFPS = "Demo_FPS";
        public string sceneTPSExplo = "Demo_TPS_Explo";
        public string sceneTPSCombat = "Demo_TPS_Combat";
        public string sceneSideView = "Demo_SideView";

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
    }
}