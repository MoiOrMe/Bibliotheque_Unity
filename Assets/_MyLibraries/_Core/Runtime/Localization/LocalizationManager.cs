using UnityEngine;
using System.Collections.Generic;
using MyLib.Core.BaseClasses;
using UnityEngine.Events;

// Singleton gérant la langue active du jeu.
// Charge les données depuis un LanguageDataSO et fournit une méthode pour récupérer
// la traduction d'une clé. Notifie les composants UI lors d'un changement de langue.

namespace MyLib.Core.Localization
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        [Header("Configuration")]
        [Tooltip("Langue par défaut chargée au démarrage.")]
        [SerializeField] private LanguageDataSO _currentLanguage;

        // Dictionnaire interne pour l'accès rapide aux traductions.
        private Dictionary<string, string> _translationDict;

        // Événement déclenché quand la langue change (pour mettre à jour les textes en temps réel).
        public UnityAction OnLanguageChanged;

        // Initialisation du dictionnaire au démarrage.
        protected override void Awake()
        {
            base.Awake();
            if (_currentLanguage != null)
            {
                LoadLanguage(_currentLanguage);
            }
        }

        // Change la langue active et rafraîchit le dictionnaire.
        public void LoadLanguage(LanguageDataSO newLanguage)
        {
            _currentLanguage = newLanguage;
            _translationDict = newLanguage.GetDictionary();

            // Notifie tous les composants LocalizedText que la langue a changé.
            OnLanguageChanged?.Invoke();

            Debug.Log($"Langue chargée : {_currentLanguage.LanguageName}");
        }

        // Récupère la traduction associée à une clé.
        // Retourne la clé elle-même avec un préfixe "MISSING" si la traduction n'existe pas.
        public string GetTranslation(string key)
        {
            if (_translationDict != null && _translationDict.TryGetValue(key, out string value))
            {
                return value;
            }

            return $"MISSING_{key}"; // Retour de débogage pour repérer les oublis.
        }
    }
}