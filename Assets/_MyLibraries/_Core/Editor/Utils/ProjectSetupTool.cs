using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MyLibraries.Core.Editor.Utils
{
    public class ProjectSetupTool : EditorWindow
    {
        [MenuItem("Tools/Setup Project Hierarchy")]
        public static void ShowWindow()
        {
            GetWindow<ProjectSetupTool>("Project Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Générateur de Structure 'Librairie Pro'", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label("Génère l'arborescence complète basée sur");
            GUILayout.Label("votre fichier HierarchyFile.txt");

            GUILayout.Space(20);

            if (GUILayout.Button("Générer la Hiérarchie"))
            {
                CreateHierarchy();
            }
        }

        private void CreateHierarchy()
        {
            string root = "Assets/_MyLibraries";
            List<string> folders = new List<string>();

            // --- 1. CORE ---
            // Art & Audio
            folders.Add($"{root}/_Core/Art/Materials");
            folders.Add($"{root}/_Core/Art/PhysicMaterials");
            folders.Add($"{root}/_Core/Art/Shaders");
            folders.Add($"{root}/_Core/Art/Rendering");
            folders.Add($"{root}/_Core/Audio/Mixers");
            folders.Add($"{root}/_Core/Audio/Music");
            folders.Add($"{root}/_Core/Audio/SFX/UI");

            // Editor & Input
            folders.Add($"{root}/_Core/Editor/Utils");
            folders.Add($"{root}/_Core/Editor/Windows");
            folders.Add($"{root}/_Core/Input/Settings");

            // Runtime (Code)
            folders.Add($"{root}/_Core/Runtime/Attributes");
            folders.Add($"{root}/_Core/Runtime/Audio/Data");
            folders.Add($"{root}/_Core/Runtime/BaseClasses");
            folders.Add($"{root}/_Core/Runtime/Data/Definitions");
            folders.Add($"{root}/_Core/Runtime/Data/Persistence");
            folders.Add($"{root}/_Core/Runtime/Data/ScriptableObjects");
            folders.Add($"{root}/_Core/Runtime/Events/EventChannels");
            folders.Add($"{root}/_Core/Runtime/Localization");
            folders.Add($"{root}/_Core/Runtime/Managers");
            folders.Add($"{root}/_Core/Runtime/Patterns/ObjectPool");
            folders.Add($"{root}/_Core/Runtime/Patterns/FSM");
            folders.Add($"{root}/_Core/Runtime/UI/Components");
            folders.Add($"{root}/_Core/Runtime/Utilities/Extensions");
            folders.Add($"{root}/_Core/Runtime/Utilities/Helpers");
            folders.Add($"{root}/_Core/Runtime/Utilities/Logger");
            folders.Add($"{root}/_Core/Runtime/Utilities/Maths");

            // Tests, Resources, Scenes, Prefabs
            folders.Add($"{root}/_Core/Tests/Editor");
            folders.Add($"{root}/_Core/Tests/Runtime");
            folders.Add($"{root}/_Core/Resources");
            folders.Add($"{root}/_Core/Scenes");
            folders.Add($"{root}/_Core/Prefabs/SystemPrefabs");
            folders.Add($"{root}/_Core/Prefabs/UI/Menus");
            folders.Add($"{root}/_Core/Prefabs/UI/Systems");

            // --- 2. MODULES ---
            // Common Global (Demandé dans le prompt)
            folders.Add($"{root}/_Modules/_Common");

            // First Person
            GenerateModuleFolders(folders, root, "FirstPerson", new string[] { "Shooter_Competitive", "Shooter_Extraction", "RPG_Immersive", "Horror_Exploration" });

            // Third Person
            GenerateModuleFolders(folders, root, "ThirdPerson", new string[] { "Shooter_Tactical", "Action_Slasher", "Adventure_Platformer" });

            // Side View
            GenerateModuleFolders(folders, root, "SideView", new string[] { "Platformer_Precision", "Metroidvania", "Fighting_Brawler" });

            // Isometric
            GenerateModuleFolders(folders, root, "Isometric", new string[] { "Strategy_RTS", "Action_HackAndSlash", "Tactical_TurnBased" });

            // VR / AR / Novel (Structure légèrement différente)
            folders.Add($"{root}/_Modules/VR/Stationary_Experience/Scripts");
            folders.Add($"{root}/_Modules/VR/Stationary_Experience/Prefabs");
            folders.Add($"{root}/_Modules/VR/Action_Locomotion/Scripts");
            folders.Add($"{root}/_Modules/VR/Action_Locomotion/Prefabs");

            folders.Add($"{root}/_Modules/AR/Tabletop_Game/Scripts");
            folders.Add($"{root}/_Modules/AR/Tabletop_Game/Prefabs");
            folders.Add($"{root}/_Modules/AR/GeoLocation/Scripts");
            folders.Add($"{root}/_Modules/AR/GeoLocation/Prefabs");

            folders.Add($"{root}/_Modules/Novel/Editor");
            folders.Add($"{root}/_Modules/Novel/VisualNovel/Scripts");
            folders.Add($"{root}/_Modules/Novel/VisualNovel/Prefabs");
            folders.Add($"{root}/_Modules/Novel/TextAdventure/Scripts");
            folders.Add($"{root}/_Modules/Novel/TextAdventure/Prefabs");

            // --- 3. SANDBOX ---
            folders.Add("Assets/_Sandbox");

            // Création physique
            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"<color=green>Hiérarchie 'Librairie Pro' générée avec succès ! ({folders.Count} dossiers)</color>");
        }

        // Helper pour générer les sous-dossiers standards des modules
        private void GenerateModuleFolders(List<string> list, string root, string category, string[] subModules)
        {
            // Dossier Common du module (ex: _Modules/FirstPerson/_Common)
            list.Add($"{root}/_Modules/{category}/_Common");

            foreach (var mod in subModules)
            {
                // Pour chaque sous-module, on crée Scripts, Prefabs, et parfois Editor/Samples selon le besoin générique
                string path = $"{root}/_Modules/{category}/{mod}";
                list.Add($"{path}/Scripts");
                list.Add($"{path}/Prefabs");

                // Ajout spécifique vu dans le fichier texte (Ex: Samples pour Competitive, Editor pour RPG)
                if (mod == "Shooter_Competitive")
                {
                    list.Add($"{path}/Samples/SampleAssets");
                    list.Add($"{path}/Documentation");
                }
                if (mod == "RPG_Immersive") list.Add($"{path}/Editor");
            }
        }
    }
}