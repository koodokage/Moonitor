using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
namespace BSS
{

    public class BinnarySaveSystem
    {
        string _fileName;
        string path;
        public BinnarySaveSystem(string fileName)
        {
            _fileName = fileName;
            path = Application.persistentDataPath + $"/BSS_{Application.productName}" + $"{_fileName}.{Application.productName}";
        }


        public void Save(IData data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream streamer = new FileStream(path, FileMode.Create);
            formatter.Serialize(streamer, data);
            streamer.Close();
        }

        public bool DataExist() { if (File.Exists(path)) return true; else return false; }

        public IData Load(IData data)
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream streamer = new FileStream(path, FileMode.Open);
                data = (IData)formatter.Deserialize(streamer);
                Debug.Log(path);
                streamer.Close();
                return data;

            }
            return null;

        }

        public static void DeleteDatas(string _fileName)
        {
            string path = Application.persistentDataPath + $"/BSS_{Application.productName}" + $"{_fileName}.{Application.productName}";
            Debug.Log(path);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.LogWarning(path);

            }

            RefreshEditorProjectWindow();
        }

        public static void DeleteDatas(string[] _fileName)
        {
            for (int i = 0; i < _fileName.Length; i++)
            {
                string path = Application.persistentDataPath + $"/BSS_{Application.productName}" + $"{_fileName[i]}.{Application.productName}";
                Debug.Log(path);

                if (File.Exists(path))
                {
                    File.Delete(path);
                    Debug.LogWarning(path);

                }
            }

            RefreshEditorProjectWindow();
        }


        static void RefreshEditorProjectWindow()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

    }
}
