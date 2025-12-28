using UnityEngine;
using System.IO;
using System;

namespace MyLibrary.Core.Data
{
    /// <summary>
    /// Interface définissant le contrat pour les systèmes de stockage (JSON, Binaire, Cloud).
    /// </summary>
    public interface IDataService
    {
        bool SaveData<T>(string relativePath, T data, bool encrypted);
        T LoadData<T>(string relativePath, bool encrypted);
    }

    /// <summary>
    /// Implémentation standard sauvegardant les données en JSON local via JsonUtility.
    /// </summary>
    public class JsonDataService : IDataService
    {
        public bool SaveData<T>(string relativePath, T data, bool encrypted)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);

            try
            {
                string jsonContent = JsonUtility.ToJson(data, true);

                if (encrypted)
                {
                    // Placeholder pour l'encrytpion (XOR ou AES à implémenter ici)
                    // jsonContent = Encrypt(jsonContent); 
                }

                File.WriteAllText(path, jsonContent);
                Debug.Log($"[DataService] Sauvegarde réussie : {path}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataService] Erreur sauvegarde : {e.Message}");
                return false;
            }
        }

        public T LoadData<T>(string relativePath, bool encrypted)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"[DataService] Fichier introuvable : {path}");
                return default;
            }

            try
            {
                string dataToLoad = File.ReadAllText(path);

                if (encrypted)
                {
                    // dataToLoad = Decrypt(dataToLoad);
                }

                return JsonUtility.FromJson<T>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataService] Erreur chargement : {e.Message}");
                return default;
            }
        }
    }
}