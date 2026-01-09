using UnityEngine;

namespace MyLib.Core.Utils
{
    // À placer sur ton Player (au même niveau que FirstPersonController)
    public class PlayModeSafety : MonoBehaviour
    {
        private void OnGUI()
        {
            // On ne fait rien si on n'est pas dans l'éditeur (pour le build final)
            if (!Application.isEditor) return;

            Event e = Event.current;

            // On écoute uniquement les touches appuyées
            if (e != null && e.isKey)
            {
                // Si la touche Control (Windows) ou Command (Mac) est enfoncée
                if (e.control || e.command)
                {
                    // Liste des touches à censurer pour l'éditeur
                    switch (e.keyCode)
                    {
                        case KeyCode.Z: // Bloque le Undo (et la suppression de ton Animator !)
                        case KeyCode.S: // Bloque le Save (et le warning "Cannot save during play")
                        case KeyCode.Y: // Bloque le Redo
                        case KeyCode.P: // Bloque la mise en pause accidentelle

                            // Cette commande magique dit à Unity : 
                            // "C'est bon, j'ai utilisé cette touche, ne fais rien d'autre avec."
                            e.Use();
                            break;
                    }
                }
            }
        }
    }
}