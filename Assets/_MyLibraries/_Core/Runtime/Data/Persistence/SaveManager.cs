using UnityEngine;
using System.IO;

// Implémentation du service de données utilisant JsonUtility (natif Unity).
// Sauvegarde les données en format texte lisible (JSON) dans le dossier persistant de l'appareil.
// Cette version ne nécessite aucun package externe.

namespace MyLib.Core.Data.Persistence
{
    public class SaveManager : IDataService
    {
        // Sauvegarde un objet générique sur le disque.
        public void SaveData<T>(string relativePath, T data, bool overwrite = true)
        {
            // Construit le chemin complet compatible avec l'OS (Windows/Mac/Android/iOS).
            string path = Path.Combine(Application.persistentDataPath, relativePath);

            if (File.Exists(path) && !overwrite)
            {
                Debug.LogWarning($"SaveManager: Impossible d'écraser {path}, overwrite est false.");
                return;
            }

            try
            {
                // Utilise le sérialiseur interne d'Unity (rapide et simple).
                // 'true' active le pretty-print pour que le fichier soit lisible par un humain.
                string json = JsonUtility.ToJson(data, true);

                File.WriteAllText(path, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur Sauvegarde {path} : {e.Message}");
            }
        }

        // Charge un objet depuis le disque.
        public T LoadData<T>(string relativePath)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);

            if (!File.Exists(path))
            {
                // Retourne null (ou 0/false) si le fichier n'existe pas encore.
                return default(T);
            }

            try
            {
                string json = File.ReadAllText(path);

                // Désérialise le texte JSON pour recréer l'objet C#.
                return JsonUtility.FromJson<T>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur Chargement {path} : {e.Message}");
                return default(T);
            }
        }

        // Supprime un fichier de sauvegarde.
        public bool DeleteData(string relativePath)
        {
            string path = Path.Combine(Application.persistentDataPath, relativePath);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
    }
}