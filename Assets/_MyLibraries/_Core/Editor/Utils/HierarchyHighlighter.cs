using UnityEngine;
using UnityEditor;

// Outil Editor (non-Runtime) qui personnalise l'affichage de la fenêtre Hiérarchie.
// Détecte les GameObjects dont le nom commence par "---" (ex: "--- SETUP ---")
// et dessine un fond coloré pour les transformer en séparateurs visuels.

namespace MyLib.Core.Editor.Utils
{
    [InitializeOnLoad] // Lance le constructeur statique dès le chargement d'Unity.
    public static class HierarchyHighlighter
    {
        // Constructeur statique appelé au lancement de l'éditeur.
        static HierarchyHighlighter()
        {
            // S'abonne à l'événement de dessin de chaque item de la hiérarchie.
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        // Méthode appelée pour chaque ligne de la fenêtre Hiérarchie.
        // selectionRect : La zone rectangulaire de la ligne actuelle.
        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            // Récupère l'objet associé à l'ID.
            var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (obj != null && obj.name.StartsWith("---"))
            {
                // Définit la couleur de fond (Gris foncé/Bleuté).
                EditorGUI.DrawRect(selectionRect, new Color(0.2f, 0.25f, 0.3f, 1f));

                // Redessine le texte en gras et centré, en blanc.
                EditorGUI.LabelField(selectionRect, obj.name.Replace("-", "").ToUpper(), new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = Color.white },
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                });
            }
        }
    }
}