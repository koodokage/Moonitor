using UnityEngine;
using UnityEditor;
namespace BSS
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SO_Data), true), CanEditMultipleObjects]
    public class SO_DataEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SO_Data data = (SO_Data)target;

            if (GUILayout.Button("Delete Save"))
            {
                data.DeleteSavedData();
            }

            if (GUILayout.Button("Reset Module"))
            {
                data.ResetData();
            }
        }

    }
#endif
    /// <summary>
    /// <b>Contain some virtual method for save/load ops. </b>
    /// </summary>
    public class SO_Data : ScriptableObject
    {
        public string fileName;
       internal BinnarySaveSystem bss;
        /// <summary>
        /// <b>-Use Sample-</b> 
        /// <example>
        /// <code>
        ///     [System.Serializable] StructData : IData{ public int _score;}
        ///     BinnarySaveSystem bss = new BinnarySaveSystem(fileName);
        ///     StructData sctData = new StructData();
        ///     sctData._score = score;
        ///     bss.Save(sctData);
        ///</code>
        ///</example>
        /// </summary>
        public virtual void SaveLocalData() { bss = new BinnarySaveSystem(fileName); }

        /// <summary>
        /// <b>-Use Sample-</b> 
        /// <example>
        /// <code>
        ///     BinnarySaveSystem bss = new BinnarySaveSystem(fileName);
        ///     var conntiueToRead = bss.DataExist();
        ///     if (!conntiueToRead) return;
        ///     StructData sctData = new StructData();
        ///     var commingData = (StructData)bss.Load(sctData);
        ///     score = commingData._score;
        ///</code>
        ///</example>
        /// </summary>
        public virtual void LoadLocalData() { bss = new BinnarySaveSystem(fileName); }

        public void DeleteSavedData()
        {
            BinnarySaveSystem.DeleteDatas(fileName);
        }

        public virtual void ResetData()
        {
        }


    }
}

