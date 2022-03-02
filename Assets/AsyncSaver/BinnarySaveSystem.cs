using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class BinnarySaveSystem
{
    public string path;
    static BinaryFormatter formatter;
    static bool inWorkPhase;
    public BinnarySaveSystem()
    {
        if (formatter == null)
            formatter = new BinaryFormatter();

        path = Application.persistentDataPath + $"/BSS.";
    }


    public void Save<T>(T data, string fileName)
    {
        string pathLocal = $"{path}{fileName}";
        using (FileStream streamer = new FileStream(pathLocal, FileMode.Create))
        {
            formatter.Serialize(streamer, data);
        }
    }

    public void OverridedSave<T>(T data, string fileName)
    {
        if (inWorkPhase)
            return;

        inWorkPhase = true;
        string pathLocal = $"{path}{fileName}";
        using (FileStream streamer = new FileStream(pathLocal, FileMode.Create))
        {
            formatter.Serialize(streamer, data);
            inWorkPhase = false;
        }
    }

    public T Load<T>(T data, string fileName)
    {
        string pathLocal = $"{path}{fileName}";

        if (formatter == null)
            formatter = new BinaryFormatter();

        FileStream streamer = null;
        try
        {
            streamer = new FileStream(pathLocal, FileMode.Open);
            if (streamer.Length > 0)
            {
                data = (T)formatter.Deserialize(streamer);

            }

        }
        catch (System.Exception ex)
        {
            Debug.Log($"Binnary Deserialize Error : {ex.Message}");

        }
        finally
        {
            if (streamer != null)
                streamer.Close();
        }
        return data;


    }

    public static T LoadSyncroned<T>(T data, string fileName)
    {
        var path = Application.persistentDataPath + $"/BSS.";
        string pathLocal = $"{path}{fileName}";

        if (formatter == null)
            formatter = new BinaryFormatter();

        FileStream streamer = null;
        try
        {
            streamer = new FileStream(pathLocal, FileMode.Open);
            if (streamer.Length > 0)
            {
                data = (T)formatter.Deserialize(streamer);

            }

        }
        catch (System.Exception ex)
        {
            Debug.Log($"Binnary Deserialize Error : {ex.Message}");

        }
        finally
        {
            if (streamer != null)
                streamer.Close();
        }
        return data;
    }

    public static bool DataExisted(string fileName)
    {
        string sPath = Application.persistentDataPath + $"/BSS.{fileName}";
        if (File.Exists(sPath)) return true; else return false;
    }

    public void DeleteData(string _fileName)
    {
        string pats = $"{path}{_fileName}";
        if (File.Exists(pats))
        {
            File.Delete(pats);
            Debug.LogWarning($"DELETED {_fileName} !");

        }

    }
    public void SaveAll<T>(string[] fileNames,T[] datas)
    {

        for (int i = 0; i < fileNames.Length; i++)
        {
            string pathLocal = $"{path}{fileNames[i]}";
            FileStream streamer = new FileStream(pathLocal, FileMode.Create);
            formatter.Serialize(streamer, datas[i]);

            streamer.Close();
        }

    }

    public  void DeleteAll(string[] fileNames)
    {
        for (int i = 0; i < fileNames.Length; i++)
        {
            string localPath = $"{path}{fileNames[i]}";


            if (File.Exists(localPath))
            {
                File.Delete(localPath);
                Debug.LogWarning($"DELETED {i} !");

            }
        }

    }


}
