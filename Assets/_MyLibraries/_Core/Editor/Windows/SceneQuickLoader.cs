using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement; // Nécessaire pour ouvrir les scènes en mode Editor

// Fenêtre Editor personnalisée listant toutes les scènes ajoutées aux Build Settings.
// Permet aux développeurs de passer d'une scène à l'autre en un clic, 
// sans avoir à naviguer dans l'arborescence du projet.

namespace MyLib.Core.Editor.Windows
{
    public class SceneQuickLoader : EditorWindow
    {
        [MenuItem("Tools/Scene Quick Loader")]
        public static void ShowWindow()
        {
            GetWindow<SceneQuickLoader>("Scene Loader");
        }

        private void OnGUI()
        {
            GUILayout.Label("Chargement Rapide", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Parcourt toutes les scènes enregistrées dans les Build Settings.
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    // Extrait le nom propre de la scène depuis son chemin (Assets/Scenes/Boot.unity -> Boot).
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);

                    if (GUILayout.Button($"Ouvrir {sceneName}"))
                    {
                        OpenScene(scene.path);
                    }
                }
            }
        }

        // Ouvre la scène demandée en demandant d'abord de sauvegarder la scène actuelle.
        private void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
    }
}