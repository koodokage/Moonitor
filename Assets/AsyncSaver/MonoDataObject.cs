using UnityEditor;
using UnityEngine;

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
            data.DeleteSavedData();
        }
    }

}
#endif
public class MonoDataObject : MonoBehaviour
{
    internal ConcreteData.StructDataMining data;
    internal string savedName;
    public virtual void OnAwake()
    {
        savedName = transform.name;
        if (!BinnarySaveSystem.DataExisted(savedName))
        {
            return;
        }
        data = BinnarySaveSystem.LoadSyncroned(data, savedName);
    }

    public virtual void ApplicationFocus()
    {
        savedName = transform.name;
        AsyncJob.AsyncSave(data,savedName);
    }


    internal void DeleteSavedData() 
    {
        savedName = transform.name;
       AsyncJob.AsyncDelete($"{savedName}");
    }
}



