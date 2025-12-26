namespace MyLibrary.Modules.Interaction
{
    // Interface définissant le contrat pour tout objet interactif.
    public interface IInteractable
    {
        // Méthode exécutée lors de l'interaction (Touche 'E' ou Bouton Action).
        void Interact();

        // Texte contextuel à afficher dans l'UI (ex: "Ouvrir Porte").
        string InteractionPrompt { get; }
    }
}