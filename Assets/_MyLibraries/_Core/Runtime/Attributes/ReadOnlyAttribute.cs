using UnityEngine;

// Définit un attribut personnalisé [ReadOnly] qui permet d'afficher un champ 
// dans l'Inspecteur Unity tout en empêchant sa modification manuelle.
// Cet attribut est utilisé pour le débogage visuel de valeurs gérées par le code.

namespace MyLib.Core.Attributes
{
    public class ReadOnlyAttribute : PropertyAttribute
    {
        // Constructeur par défaut de l'attribut.
        // Aucune logique spécifique n'est requise à l'instanciation pour ce marqueur.
        public ReadOnlyAttribute() { }
    }
}