using UnityEngine;

namespace MyLibrary.Core.Attributes
{
    /// <summary>
    /// Attribut personnalisé rendant une variable visible dans l'inspecteur mais non modifiable.
    /// Nécessite le script ReadOnlyDrawer.cs dans le dossier Editor.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
        // Aucune logique requise ici, sert uniquement de marqueur.
    }
}