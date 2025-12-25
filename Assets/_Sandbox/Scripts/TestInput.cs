using UnityEngine;
using MyLibrary.Core;

public class TestInput : MonoBehaviour
{
    void Update()
    {
        // 1. Test de lecture du Mouvement (WASD / Stick Gauche)
        // On affiche le message seulement si on bouge
        if (InputManager.Instance.MoveInput != Vector2.zero)
        {
            Debug.Log($"Mouvement détecté : {InputManager.Instance.MoveInput}");
        }

        // 2. Test de lecture du Bouton Saut (Espace / Bouton Sud)
        if (InputManager.Instance.IsJumpPressed)
        {
            Debug.Log("Le bouton SAUT est appuyé !");
        }

        // 3. Test de lecture de l'Interaction (E / Bouton Ouest)
        if (InputManager.Instance.IsInteractPressed)
        {
            Debug.Log("Le bouton INTERACTION est appuyé !");
        }
    }
}