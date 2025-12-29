// Interface définissant le contrat pour tout service de persistance de données.
// Permet de découpler le gestionnaire de sauvegarde (SaveManager) de la méthode réelle de stockage
// (Fichier JSON local, PlayerPrefs, Sauvegarde Cloud, etc.).

namespace MyLib.Core.Data.Persistence
{
    public interface IDataService
    {
        // Sauvegarde des données génériques sous un nom de fichier spécifique.
        // overwrite : Indique si l'on doit écraser le fichier existant.
        void SaveData<T>(string relativePath, T data, bool overwrite = true);

        // Charge des données génériques depuis un fichier.
        // Retourne les données typées ou une valeur par défaut si le fichier n'existe pas.
        T LoadData<T>(string relativePath);

        // Supprime un fichier de sauvegarde spécifique.
        // Utile pour réinitialiser la progression ou supprimer des slots.
        bool DeleteData(string relativePath);
    }
}