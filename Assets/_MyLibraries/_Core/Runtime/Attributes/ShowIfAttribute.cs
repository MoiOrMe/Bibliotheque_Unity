using UnityEngine;
using System;

// Définit un attribut personnalisé [ShowIf] permettant d'afficher ou de masquer
// conditionnellement un champ dans l'Inspecteur en fonction de la valeur booléenne
// d'un autre champ.

namespace MyLib.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionalSourceField { get; private set; }
        public bool ExpectedValue { get; private set; }

        // Initialise l'attribut avec le nom du champ booléen à vérifier.
        // Par défaut, le champ cible sera affiché si le champ conditionnel est vrai.
        public ShowIfAttribute(string conditionalSourceField)
        {
            this.ConditionalSourceField = conditionalSourceField;
            this.ExpectedValue = true;
        }

        // Initialise l'attribut avec le nom du champ à vérifier et la valeur attendue.
        // Permet d'inverser la logique (ex: afficher si le booléen est faux).
        public ShowIfAttribute(string conditionalSourceField, bool expectedValue)
        {
            this.ConditionalSourceField = conditionalSourceField;
            this.ExpectedValue = expectedValue;
        }
    }
}