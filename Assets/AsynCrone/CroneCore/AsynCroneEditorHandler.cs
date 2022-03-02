using AsCrone.Transmision;
using UnityEditor;
using UnityEngine;

namespace AsCrone.EditorModule
{
    public class AsynCroneEditorHandler
    {
        static string performerName = "[CRONE]";
        static CronePerformer cronePerformer;
        [MenuItem("AsynCrone / Create Performer", false, 10)]
        static void CreateCronePerformer(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject(performerName);
            go.AddComponent<CronePerformer>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            cronePerformer = go.GetComponent<CronePerformer>();
        }

        [MenuItem("AsynCrone / Createt DataLake", false, 10)]
        static void CreateDataLake()
        {
            // Create a custom game object
            DataLake assetStatic = ScriptableObject.CreateInstance<DataLake>();
            AssetDatabase.CreateAsset(assetStatic, "Assets/AsynCrone/Data/DataLake.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = assetStatic;
            if(cronePerformer == null)
            {
                var crone = GameObject.Find(performerName);
                cronePerformer = crone.GetComponent<CronePerformer>();
            }
            cronePerformer.SetDataLake = assetStatic;
        }


    }

    

}


