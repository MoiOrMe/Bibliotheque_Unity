using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MyLib.Core.Editor.Utils
{
    public class AssemblySetupTool : EditorWindow
    {
        [MenuItem("Tools/Setup Assembly Definitions")]
        public static void ShowWindow()
        {
            GetWindow<AssemblySetupTool>("Assembly Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Générateur d'Assembly Definitions (.asmdef)", EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label("Crée les fichiers de définition pour forcer");
            GUILayout.Label("l'architecture modulaire et les dépendances.");

            GUILayout.Space(20);

            if (GUILayout.Button("Générer les .asmdef"))
            {
                CreateAssemblies();
            }
        }

        private void CreateAssemblies()
        {
            // 1. Définition du Core (La base, aucune référence)
            CreateAsmDef("Assets/_MyLibraries/_Core", "MyLib.Core", new string[] { "Unity.InputSystem", "Unity.TextMeshPro" });
            // Note: J'ajoute InputSystem et TMP par défaut car le Core en aura besoin.

            // 2. Définition des Modules Piliers (Ref: Core)
            string[] coreRef = new string[] { "MyLib.Core", "Unity.InputSystem", "Unity.TextMeshPro" };

            CreateAsmDef("Assets/_MyLibraries/_Modules/FirstPerson", "MyLib.FirstPerson", coreRef);
            CreateAsmDef("Assets/_MyLibraries/_Modules/ThirdPerson", "MyLib.ThirdPerson", coreRef);
            CreateAsmDef("Assets/_MyLibraries/_Modules/SideView", "MyLib.SideView", coreRef);
            CreateAsmDef("Assets/_MyLibraries/_Modules/Isometric", "MyLib.Isometric", coreRef);
            CreateAsmDef("Assets/_MyLibraries/_Modules/VR", "MyLib.VR", new string[] { "MyLib.Core", "Unity.XR.Management" }); // Exemple VR
            CreateAsmDef("Assets/_MyLibraries/_Modules/AR", "MyLib.AR", new string[] { "MyLib.Core", "Unity.XR.ARFoundation" }); // Exemple AR
            CreateAsmDef("Assets/_MyLibraries/_Modules/Novel", "MyLib.Novel", coreRef);

            // 3. Définition des Sous-Modules (Ref: Core + Parent Module)
            // FIRST PERSON
            string[] fpsRefs = new string[] { "MyLib.Core", "MyLib.FirstPerson" };
            CreateAsmDef("Assets/_MyLibraries/_Modules/FirstPerson/Shooter_Competitive", "MyLib.FPS.Competitive", fpsRefs);
            CreateAsmDef("Assets/_MyLibraries/_Modules/FirstPerson/Shooter_Extraction", "MyLib.FPS.Extraction", fpsRefs);
            CreateAsmDef("Assets/_MyLibraries/_Modules/FirstPerson/RPG_Immersive", "MyLib.FPS.RPG", fpsRefs);
            CreateAsmDef("Assets/_MyLibraries/_Modules/FirstPerson/Horror_Exploration", "MyLib.FPS.Horror", fpsRefs);

            // THIRD PERSON
            string[] tpsRefs = new string[] { "MyLib.Core", "MyLib.ThirdPerson" };
            CreateAsmDef("Assets/_MyLibraries/_Modules/ThirdPerson/Shooter_Tactical", "MyLib.TPS.Tactical", tpsRefs);
            CreateAsmDef("Assets/_MyLibraries/_Modules/ThirdPerson/Action_Slasher", "MyLib.TPS.Slasher", tpsRefs);
            CreateAsmDef("Assets/_MyLibraries/_Modules/ThirdPerson/Adventure_Platformer", "MyLib.TPS.Platformer", tpsRefs);

            // SIDE VIEW
            string[] sideRefs = new string[] { "MyLib.Core", "MyLib.SideView" };
            CreateAsmDef("Assets/_MyLibraries/_Modules/SideView/Platformer_Precision", "MyLib.Side.Precision", sideRefs);
            CreateAsmDef("Assets/_MyLibraries/_Modules/SideView/Metroidvania", "MyLib.Side.Metroidvania", sideRefs);
            CreateAsmDef("Assets/_MyLibraries/_Modules/SideView/Fighting_Brawler", "MyLib.Side.Fighting", sideRefs);

            // ISOMETRIC
            string[] isoRefs = new string[] { "MyLib.Core", "MyLib.Isometric" };
            CreateAsmDef("Assets/_MyLibraries/_Modules/Isometric/Strategy_RTS", "MyLib.Iso.RTS", isoRefs);
            CreateAsmDef("Assets/_MyLibraries/_Modules/Isometric/Action_HackAndSlash", "MyLib.Iso.HackAndSlash", isoRefs);
            CreateAsmDef("Assets/_MyLibraries/_Modules/Isometric/Tactical_TurnBased", "MyLib.Iso.Tactical", isoRefs);

            AssetDatabase.Refresh();
            Debug.Log("<color=green>Tous les fichiers .asmdef ont été générés et configurés !</color>");
        }

        private void CreateAsmDef(string folderPath, string asmName, string[] references)
        {
            // Vérifie d'abord si le dossier existe (au cas où le dossier script n'a pas été lancé)
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, asmName + ".asmdef");

            // Si le fichier existe déjà, on ne l'écrase pas pour ne pas perdre de config manuelle
            if (File.Exists(filePath)) return;

            // Structure JSON d'un .asmdef Unity standard
            AssemblyDefinitionJson asmJson = new AssemblyDefinitionJson
            {
                name = asmName,
                rootNamespace = asmName, // Nouveauté Unity 6 : aide à l'auto-complete des namespaces
                references = references,
                includePlatforms = new string[0],
                excludePlatforms = new string[0],
                allowUnsafeCode = false,
                overrideReferences = false,
                precompiledReferences = new string[0],
                autoReferenced = true,
                defineConstraints = new string[0],
                versionDefines = new VersionDefine[0],
                noEngineReferences = false
            };

            string jsonContent = JsonUtility.ToJson(asmJson, true);
            File.WriteAllText(filePath, jsonContent);
        }

        // Classes utilitaires pour sérialiser le JSON correctement
        [System.Serializable]
        private class AssemblyDefinitionJson
        {
            public string name;
            public string rootNamespace;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public string[] precompiledReferences;
            public bool autoReferenced;
            public string[] defineConstraints;
            public VersionDefine[] versionDefines;
            public bool noEngineReferences;
        }

        [System.Serializable]
        private class VersionDefine
        {
            public string name;
            public string expression;
            public string define;
        }
    }
}