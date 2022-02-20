using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace BSS
{

#if UNITY_EDITOR
[CustomEditor(typeof(MonoDataObject), true), CanEditMultipleObjects]
public class MonoDataObjectEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MonoDataObject data = (MonoDataObject)target;

        if (GUILayout.Button("Delete Save Data"))
        {
            data.DeleteSavedData_Base();
        }
    }

}
#endif

public class MonoDataObject : MonoBehaviour
{
    public ConcreteDataLogger.StructData structedData;
    public virtual void Awake()
    {
        ConcreteDataLogger.LoadingProcess(transform.name,ref structedData);
    }

    public virtual void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            ConcreteDataLogger.SaveData(transform.name, structedData);
        }
    }

    internal void DeleteSavedData_Base()
    {
        ConcreteDataLogger.DeleteDataProcess(transform.name);
    }

    internal void DeleteSavedData_Base(string[] fileNames)
    {
        ConcreteDataLogger.DeleteDataProcess(fileNames);
    }
}

}
