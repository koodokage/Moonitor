using System;
using System.Threading;
using System.Threading.Tasks;

public static class AsyncJob
{
    //static BinnarySaveSystem BSS = new BinnarySaveSystem();
    static BinnarySaveSystem BSS = new BinnarySaveSystem();

    /// <summary>
    /// Saving Asyncrone Operations
    /// </summary>
    public static void AsyncSave<T>(T data, string fileName)
    {
        SaveAsync(data, fileName);
    }

    /// <summary>
    /// Loading Asyncrone Operations
    /// </summary>
    public static T AsyncLoad<T>(T data, string fileName)
    {
        var task = LoadAsync(data, fileName);
        task.Wait();
        return task.Result;

    }

    /// <summary>
    /// Deleting Name Based File Asyncrone Operations
    /// </summary>
    /// <param name="fileName"></param>
    public static void AsyncDelete( string fileName)
    {
        DeleteAsync(fileName);
    }

    /// <summary>
    /// Save All Same Type Data With One Thread 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileNames"></param>
    /// <param name="datas"></param>
    public static  void SaveAllAsync<T>(string[] fileNames , T[] datas)
    {
        Thread allSaver = new Thread(()=> BSS.SaveAll(fileNames, datas))
        {
            IsBackground = true
        };
        allSaver.Start();
    }

    /// <summary>
    /// Deleting All Data Asyncrone Operations
    /// </summary>
    public static void AsyncDeleteAll(string[] fileNames)
    {
        AllDeleteAsync(fileNames);
    }

    /// <summary>
    /// Do Some Func Asyncrone Operations
    /// </summary>
    /// <param name="actionT"></param>
    public static void DoAsync(Action actionT)
    {
        WriteAsync(actionT);
    }



    private static async void SaveAsync<T>(T data, string fileName)
    {
        await Task.Run(() =>
        {
            BSS.Save(data, fileName);
        });
    }



    private static Task<T> LoadAsync<T>(T data, string fileName)
    {
        var task1 = Task.Run(() => BSS.Load(data, fileName));
        return task1;

    }

    private static async void DeleteAsync(string fileName)
    {
        await Task.Run(() =>
        {
            BSS.DeleteData(fileName);
        });
    }

    private static async void AllDeleteAsync(string[] fileNames)
    {
        await Task.Run(() =>
        {
            BSS.DeleteAll(fileNames);
        });
    }

    private static async void WriteAsync(Action func)
    {
        await Task.Run(func);
    }
}
