using UnityEditor;
using UnityEngine;

namespace MyLibrary.Editor
{
    /// <summary>
    /// Modifie l'apparence de la fenêtre Hiérarchie de Unity.
    /// Applique des couleurs de fond aux GameObjects nommés avec "---" pour servir de séparateurs.
    /// </summary>
    [InitializeOnLoad]
    public class HierarchyHighlighter
    {
        #region Constructor

        static HierarchyHighlighter()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        #endregion

        #region Drawing Logic

        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (obj != null && obj.name.StartsWith("---"))
            {
                // Fond gris foncé pour le séparateur
                EditorGUI.DrawRect(selectionRect, new Color(0.2f, 0.2f, 0.2f));

                // Style du texte : Centré, Gras, Blanc
                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white }
                };

                // Affichage du nom en majuscules sans les tirets
                EditorGUI.LabelField(selectionRect, obj.name.Replace("-", "").ToUpper(), style);
            }
        }

        #endregion
    }
}