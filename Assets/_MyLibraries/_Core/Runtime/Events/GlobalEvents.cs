using System;

// Classe statique regroupant des événements C# purs (Actions) accessibles globalement.
// Sert d'alternative légère aux EventChannels pour des événements techniques 
// ne nécessitant pas de configuration dans l'inspecteur (ex: App Quit, Low Memory).

namespace MyLib.Core.Events
{
    public static class GlobalEvents
    {
        // Événement déclenché lorsque l'application est mise en pause ou perd le focus.
        // Utile pour forcer la sauvegarde automatique ou mettre le jeu en pause silencieuse.
        public static Action<bool> OnApplicationFocusChanged;

        // Événement déclenché lorsque l'application va quitter.
        // Utile pour le nettoyage des ressources et la sauvegarde d'urgence.
        public static Action OnApplicationQuit;

        // Méthode helper pour nettoyer tous les abonnés (utile lors des rechargements de domaines en éditeur).
        public static void ClearAllListeners()
        {
            OnApplicationFocusChanged = null;
            OnApplicationQuit = null;
        }
    }
}