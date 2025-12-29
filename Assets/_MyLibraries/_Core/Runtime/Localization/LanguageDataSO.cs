using UnityEngine;
using System.Collections.Generic;

// ScriptableObject contenant les données de traduction pour une langue spécifique (ex: FR, EN).
// Stocke une liste de paires Clé/Valeur pour associer un identifiant texte à sa traduction.

namespace MyLib.Core.Localization
{
    [CreateAssetMenu(menuName = "MyLib/Localization/Language Data", fileName = "Language_FR")]
    public class LanguageDataSO : ScriptableObject
    {
        [Tooltip("Nom de la langue (ex: Français).")]
        public string LanguageName;

        [Tooltip("Code ISO de la langue (ex: fr-FR).")]
        public string LanguageCode;

        // Classe interne sérialisable pour simuler un Dictionnaire dans l'inspecteur Unity.
        [System.Serializable]
        public class LocalizationEntry
        {
            public string Key;      // Ex: "MENU_START"
            [TextArea]
            public string Value;    // Ex: "Commencer"
        }

        [Tooltip("Liste des traductions.")]
        public List<LocalizationEntry> Translations = new List<LocalizationEntry>();

        // Convertit la liste en Dictionnaire au runtime pour une recherche rapide (O(1)).
        public Dictionary<string, string> GetDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var entry in Translations)
            {
                if (!dict.ContainsKey(entry.Key))
                {
                    dict.Add(entry.Key, entry.Value);
                }
            }
            return dict;
        }
    }
}