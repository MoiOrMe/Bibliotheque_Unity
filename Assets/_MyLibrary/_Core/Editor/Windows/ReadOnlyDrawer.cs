using UnityEditor;
using UnityEngine;
using MyLibrary.Core.Attributes;

namespace MyLibrary.Editor
{
    /// <summary>
    /// Dessinateur de propriété (PropertyDrawer) pour l'attribut [ReadOnly].
    /// Rend le champ inspecteur gris et non modifiable tout en affichant sa valeur.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        #region GUI Logic

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Désactivation de l'interface graphique pour ce champ
            GUI.enabled = false;

            // Dessin du champ standard
            EditorGUI.PropertyField(position, property, label);

            // Réactivation de l'interface pour les champs suivants
            GUI.enabled = true;
        }

        #endregion
    }
}