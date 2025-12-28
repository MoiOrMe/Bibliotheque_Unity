using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MyLibrary.Core; // Nécessaire pour l'EventBus
using MyLibrary.Modules.Stats;

namespace MyLibrary.Modules.Stats.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("UI References")]
        public Image healthFillImage;

        [Header("Display Settings")]
        public float updateSpeed = 10f;

        private CharacterHealth _targetHealth;
        private float _targetFillAmount = 1f;

        #region Unity Lifecycle

        private void OnEnable()
        {
            FindAndBindPlayer();

            // Abonnement aux événements globaux
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventBus.Subscribe(GameEventType.PlayerDied, OnPlayerDied);
        }

        private void OnDisable()
        {
            if (_targetHealth != null)
            {
                _targetHealth.health.OnValueChanged -= UpdateHealthBar;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EventBus.Unsubscribe(GameEventType.PlayerDied, OnPlayerDied);
        }

        private void Update()
        {
            if (healthFillImage != null)
            {
                healthFillImage.fillAmount = Mathf.Lerp(healthFillImage.fillAmount, _targetFillAmount, Time.deltaTime * updateSpeed);
            }
        }

        #endregion

        #region Event Handlers

        private void OnPlayerDied()
        {
            // FORCE VISUELLE : On met tout à 0 immédiatement pour éviter le délai du Lerp
            _targetFillAmount = 0f;
            if (healthFillImage != null) healthFillImage.fillAmount = 0f;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FindAndBindPlayer();
        }

        #endregion

        #region Binding Logic

        private void FindAndBindPlayer()
        {
            if (_targetHealth != null) _targetHealth.health.OnValueChanged -= UpdateHealthBar;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) _targetHealth = player.GetComponent<CharacterHealth>();
            else _targetHealth = FindFirstObjectByType<CharacterHealth>();

            if (_targetHealth != null)
            {
                _targetHealth.health.OnValueChanged += UpdateHealthBar;
                UpdateHealthBar(_targetHealth.health.currentValue, _targetHealth.health.maxValue);
            }
            else
            {
                _targetFillAmount = 0f; // Pas de joueur = barre vide
            }
        }

        private void UpdateHealthBar(float current, float max)
        {
            if (max > 0) _targetFillAmount = current / max;
            else _targetFillAmount = 0;
        }

        #endregion
    }
}