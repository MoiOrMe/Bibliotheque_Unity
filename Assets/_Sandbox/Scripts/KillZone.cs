using UnityEngine;
using MyLibrary.Core;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // On vérifie si c'est le joueur
        if (other.CompareTag("Player"))
        {
            Debug.Log("Le joueur est entré dans la zone mortelle !");

            // On envoie juste le message.
            EventBus.Publish(GameEventType.PlayerDied);
        }
    }
}