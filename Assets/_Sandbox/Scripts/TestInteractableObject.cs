using UnityEngine;
using MyLibrary.Modules.Interaction;

public class TestInteractableObject : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "Changer Couleur";

    public void Interact()
    {
        // Ce code se lance quand le joueur appuie sur E en regardant l'objet
        Debug.Log("INTERACTION REUSSIE !");

        // On change la couleur au hasard pour prouver que ça marche
        GetComponent<Renderer>().material.color = Random.ColorHSV();
    }
}