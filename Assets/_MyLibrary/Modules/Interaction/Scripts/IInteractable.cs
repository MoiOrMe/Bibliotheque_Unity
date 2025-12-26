namespace MyLibrary.Modules.Interaction
{
    /// <summary>
    /// Ce n'est pas une classe MonoBehaviour, c'est une INTERFACE.
    /// Tout objet qui veut être interactif
    /// DOIT posséder cette méthode Interact().
    /// </summary>
    public interface IInteractable
    {
        // La méthode que l'objet devra exécuter quand on appuie sur E
        void Interact();

        // Un texte pour l'UI (ex: "Ouvrir", "Ramasser", "Parler")
        string InteractionPrompt { get; }
    }
}