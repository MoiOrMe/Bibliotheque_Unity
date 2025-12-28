using System;
using UnityEngine;

namespace MyLibrary.Core.Utilities.Helpers
{
    /// <summary>
    /// Classe utilitaire légère pour gérer des comptes à rebours.
    /// Doit être mise à jour manuellement via la méthode Tick(deltaTime).
    /// </summary>
    public class Timer
    {
        #region Public Properties

        public float Duration { get; private set; }
        public float RemainingTime { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Retourne la progression de 0 (début) à 1 (fin).
        /// </summary>
        public float Progress => 1f - (RemainingTime / Duration);

        #endregion

        #region Events

        public event Action OnTimerStart;
        public event Action<float> OnTimerUpdate; // Renvoie le temps restant
        public event Action OnTimerEnd;

        #endregion

        #region Constructors

        public Timer(float duration)
        {
            Duration = duration;
            RemainingTime = duration;
        }

        #endregion

        #region Control Methods

        public void Start()
        {
            RemainingTime = Duration;
            IsRunning = true;
            IsPaused = false;
            OnTimerStart?.Invoke();
        }

        public void Stop()
        {
            IsRunning = false;
            IsPaused = false;
            RemainingTime = 0;
        }

        public void Pause()
        {
            IsPaused = true;
            IsRunning = false;
        }

        public void Resume()
        {
            if (IsPaused)
            {
                IsPaused = false;
                IsRunning = true;
            }
        }

        public void Reset(float newDuration = -1)
        {
            if (newDuration > 0) Duration = newDuration;
            RemainingTime = Duration;
            IsRunning = false;
            IsPaused = false;
        }

        #endregion

        #region Update Loop

        /// <summary>
        /// À appeler dans le Update() de votre MonoBehaviour.
        /// </summary>
        /// <param name="deltaTime">Time.deltaTime ou Time.unscaledDeltaTime</param>
        public void Tick(float deltaTime)
        {
            if (!IsRunning || IsPaused) return;

            RemainingTime -= deltaTime;

            OnTimerUpdate?.Invoke(RemainingTime);

            if (RemainingTime <= 0)
            {
                RemainingTime = 0;
                IsRunning = false;
                OnTimerEnd?.Invoke();
            }
        }

        #endregion
    }
}