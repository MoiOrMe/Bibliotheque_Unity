using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
Description du script :
Ce script génère un fichier 'hierarchy.txt' listant l'arborescence complète du dossier '_MyLibraries'.
Il détecte le type de chaque fichier (extension ou nom spécifique) pour lui attribuer une description contextuelle (ex: [CRITIQUE], [NOYAU]).
Le formatage assure un alignement vertical des commentaires pour reproduire la mise en page demandée.
Les fichiers .meta sont exclus du processus.
*/

public class HierarchyAnnotatedGenerator
{
    // Constante définissant la colonne à laquelle les commentaires doivent commencer pour l'alignement.
    private const int CommentColumnIndex = 60;

    // Point d'entrée de la commande dans le menu Tools.
    // Initialise les chemins, lance l'analyse récursive et écrit le résultat final.
    [MenuItem("Tools/Generate Annotated Hierarchy")]
    public static void GenerateHierarchy()
    {
        string assetsPath = Application.dataPath;
        string targetDir = Path.Combine(assetsPath, "_MyLibraries");
        string outputPath = Path.Combine(assetsPath, "hierarchy.txt");

        if (!Directory.Exists(targetDir))
        {
            Debug.LogError($"Le dossier cible est introuvable : {targetDir}");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Assets/_MyLibraries/");

        ProcessDirectory(targetDir, "", sb);

        File.WriteAllText(outputPath, sb.ToString());

        // Force Unity à rafraîchir la vue projet pour afficher le nouveau fichier texte.
        AssetDatabase.Refresh();

        Debug.Log($"Hiérarchie annotée générée : {outputPath}");
    }

    // Parcourt récursivement les dossiers et fichiers.
    // Construit la chaîne de caractères de l'arborescence et y appose les commentaires alignés.
    private static void ProcessDirectory(string path, string indent, StringBuilder sb)
    {
        string[] files = Directory.GetFiles(path)
                                  .Where(f => !f.EndsWith(".meta"))
                                  .ToArray();

        string[] directories = Directory.GetDirectories(path);

        var items = files.Concat(directories).ToList();
        int count = items.Count;

        for (int i = 0; i < count; i++)
        {
            string itemPath = items[i];
            bool isLast = (i == count - 1);
            string name = Path.GetFileName(itemPath);
            bool isDirectory = Directory.Exists(itemPath);
            string displayName = isDirectory ? name + "/" : name;

            // Construction de la partie gauche (arbre) de la ligne actuelle.
            string treeNode = $"{indent}{(isLast ? "└── " : "├── ")}{displayName}";

            // Récupération de la description basée sur le nom ou l'extension du fichier.
            string description = isDirectory ? GetDirectoryDescription(name) : GetFileDescription(name);

            // Calcul du nombre d'espaces nécessaires pour atteindre la colonne de commentaire définie.
            // Si la ligne d'arbre est plus longue que la colonne cible, on ajoute juste un espace de séparation.
            int paddingLength = Mathf.Max(1, CommentColumnIndex - treeNode.Length);
            string padding = new string(' ', paddingLength);

            // Assemblage final de la ligne si une description existe.
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine($"{treeNode}{padding}# {description}");
            }
            else
            {
                sb.AppendLine(treeNode);
            }

            if (isDirectory)
            {
                string newIndent = indent + (isLast ? "    " : "│   ");
                ProcessDirectory(itemPath, newIndent, sb);

                // Ajout d'une ligne verticale de séparation uniquement si ce n'est pas le dernier élément du dossier parent.
                if (!isLast)
                {
                    sb.AppendLine($"{indent}│");
                }
            }
        }
    }

    // Retourne une description textuelle basée sur le nom exact du fichier ou son extension.
    // Utilise un switch pour catégoriser les assets techniques courants d'Unity.
    private static string GetFileDescription(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();

        // Vérification des noms de fichiers spécifiques d'abord.
        if (fileName.Equals("package.json", System.StringComparison.OrdinalIgnoreCase))
            return "[CRITIQUE] Définit les dépendances";

        // Vérification basée sur l'extension.
        switch (extension)
        {
            case ".asmdef":
                return "[NOYAU] Définition d'Assembly (Compilation)";
            case ".cs":
                return "Script C#";
            case ".mat":
                return "Matériau";
            case ".shader":
            case ".shadergraph":
                return "Shader / Graphique";
            case ".asset":
                return "Configuration / Profil ScriptableObject";
            case ".mixer":
                return "Audio Mixer";
            case ".prefab":
                return "Prefabricated Object";
            case ".png":
            case ".jpg":
            case ".tga":
            case ".psd":
                return "Texture / Sprite";
            case ".fbx":
            case ".obj":
            case ".blend":
                return "Modèle 3D";
            case ".wav":
            case ".mp3":
            case ".ogg":
                return "Fichier Audio";
            case ".anim":
            case ".controller":
                return "Animation / Animator Controller";
            default:
                return ""; // Aucune description par défaut.
        }
    }

    // Retourne une description simple basée sur des noms de dossiers conventionnels.
    // Permet d'annoter les dossiers structurels importants.
    private static string GetDirectoryDescription(string directoryName)
    {
        switch (directoryName)
        {
            case "_Core":
                return "[NOYAU] Dépendance obligatoire";
            case "Art":
                return "Assets techniques partagés";
            case "Editor":
                return "Outils Editor (Ne compile pas dans le build)";
            case "Audio":
                return "Assets Audio";
            case "Scripts":
                return "Logique code";
            case "Prefabs":
                return "Objets pré-configurés";
            default:
                return "";
        }
    }
}