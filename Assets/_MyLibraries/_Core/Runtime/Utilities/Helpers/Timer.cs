using System;

// Classe utilitaire légère (non-MonoBehaviour) pour gérer des comptes à rebours.
// Évite d'avoir à créer manuellement des variables 'float timer' et 'float duration' dans chaque script.
// Doit être mis à jour manuellement via la méthode Tick() dans un Update.

namespace MyLib.Core.Utilities.Helpers
{
    public class Timer
    {
        public float RemainingTime { get; private set; }
        public bool IsRunning { get; private set; }

        // Événement déclenché lorsque le timer atteint 0.
        public event Action OnTimerStop;

        // Constructeur initialisant le timer avec une valeur par défaut.
        public Timer(float duration = 0f)
        {
            RemainingTime = duration;
            IsRunning = false;
        }

        // Démarre ou redémarre le timer avec une nouvelle durée.
        public void Start(float duration)
        {
            RemainingTime = duration;
            IsRunning = true;
        }

        // Méthode à appeler dans l'Update() du script propriétaire.
        // Décrémente le temps et déclenche l'événement à la fin.
        public void Tick(float deltaTime)
        {
            if (!IsRunning || RemainingTime <= 0f) return;

            RemainingTime -= deltaTime;

            if (RemainingTime <= 0f)
            {
                Stop();
                OnTimerStop?.Invoke(); // Notifie les abonnés que le temps est écoulé.
            }
        }

        // Arrête le timer et remet le temps à 0.
        public void Stop()
        {
            IsRunning = false;
            RemainingTime = 0f;
        }
    }
}