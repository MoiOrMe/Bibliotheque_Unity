using UnityEngine;
using System;

namespace MyLibrary.Core.Attributes
{
    /// <summary>
    /// Attribut conditionnel affichant un champ dans l'inspecteur uniquement si une condition booléenne est remplie.
    /// Usage: [ShowIf("isSpecialMode")] public float specialValue;
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionFieldName { get; private set; }

        /// <summary>
        /// Constructeur de l'attribut.
        /// </summary>
        /// <param name="conditionFieldName">Le nom exact de la variable booléenne qui conditionne l'affichage.</param>
        public ShowIfAttribute(string conditionFieldName)
        {
            ConditionFieldName = conditionFieldName;
        }
    }
}