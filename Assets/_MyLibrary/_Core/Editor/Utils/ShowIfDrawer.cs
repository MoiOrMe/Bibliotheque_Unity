using UnityEditor;
using UnityEngine;
using MyLibrary.Core.Attributes;

namespace MyLibrary.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        #region Metrics

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                // Hauteur standard si visible
                return EditorGUI.GetPropertyHeight(property, label);
            }
            // Hauteur nulle si caché (réduit l'espace vide)
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        #endregion

        #region Drawing Logic

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        #endregion

        #region Helper Methods

        private bool ShouldShow(SerializedProperty property)
        {
            ShowIfAttribute attribute = (ShowIfAttribute)base.attribute;

            // On cherche la propriété booléenne conditionnelle
            // On utilise property.serializedObject.FindProperty pour la sécurité relative au chemin
            // Note: FindProperty cherche depuis la racine, il faut gérer les chemins relatifs si nécessaire
            // Pour simplifier ici, on cherche au niveau parent direct

            string conditionPath = attribute.ConditionFieldName;

            // Gestion des propriétés imbriquées (ex: dans une classe sérialisée)
            string propertyPath = property.propertyPath;
            int lastDotIndex = propertyPath.LastIndexOf('.');
            if (lastDotIndex >= 0)
            {
                string parentPath = propertyPath.Substring(0, lastDotIndex);
                conditionPath = parentPath + "." + attribute.ConditionFieldName;
            }

            SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionPath);

            if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
            {
                return conditionProperty.boolValue;
            }

            // Par sécurité, si la condition n'est pas trouvée, on affiche le champ
            // (Optionnel : On pourrait logger un warning)
            return true;
        }

        #endregion
    }
}