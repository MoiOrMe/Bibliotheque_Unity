using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

namespace MyLibrary.Editor
{
    /// <summary>
    /// Outil de fenêtre d'éditeur permettant de charger rapidement des scènes spécifiques.
    /// Liste les scènes incluses dans les Build Settings.
    /// </summary>
    public class SceneQuickLoader : EditorWindow
    {
        #region Window Setup

        [MenuItem("Tools/MyLibrary/Scene Loader")]
        public static void ShowWindow()
        {
            GetWindow<SceneQuickLoader>("Quick Load");
        }

        #endregion

        #region GUI Implementation

        private void OnGUI()
        {
            GUILayout.Label("Core Scenes", EditorStyles.boldLabel);

            // Bouton spécifique pour le Boot (Chemin à adapter selon votre structure exacte)
            if (GUILayout.Button("Load BOOT"))
            {
                OpenScene("Assets/_MyLibraries/Scenes/_Boot.unity");
            }

            GUILayout.Space(10);
            GUILayout.Label("Build Settings Scenes", EditorStyles.boldLabel);

            // Génération dynamique des boutons pour chaque scène du Build Settings
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    if (GUILayout.Button($"Load {sceneName}"))
                    {
                        OpenScene(scene.path);
                    }
                }
            }
        }

        private void OpenScene(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path);
            }
        }

        #endregion
    }
}