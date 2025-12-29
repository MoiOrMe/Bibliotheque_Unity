using UnityEngine;
using MyLib.Core.BaseClasses; // Pour Singleton
using MyLib.Core.UI.Components;

// Singleton gérant la file d'attente des notifications.
// Instancie le prefab de "Toast" dans un conteneur dédié (Vertical Layout Group)
// pour les empiler proprement à l'écran.

namespace MyLib.Core.Managers
{
    public class NotificationManager : Singleton<NotificationManager>
    {
        [Header("References")]
        [Tooltip("Le Prefab du Toast à instancier.")]
        [SerializeField] private NotificationToast _toastPrefab;

        [Tooltip("Le conteneur UI où les toasts seront empilés (doit avoir un VerticalLayoutGroup).")]
        [SerializeField] private Transform _toastContainer;

        // Affiche une notification standard.
        public void ShowNotification(string message)
        {
            ShowNotification(message, Color.black); // Noir par défaut
        }

        // Affiche une notification avec une couleur spécifique (ex: Rouge pour erreur).
        public void ShowNotification(string message, Color backgroundColor)
        {
            if (_toastPrefab == null || _toastContainer == null)
            {
                Debug.LogWarning("NotificationManager: Prefab ou Container manquant.");
                return;
            }

            // Instancie le toast enfant du conteneur.
            NotificationToast newToast = Instantiate(_toastPrefab, _toastContainer);

            // Configure le toast.
            newToast.Initialize(message, backgroundColor);
        }
    }
}