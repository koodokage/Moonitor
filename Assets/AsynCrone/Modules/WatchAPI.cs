using System.Diagnostics;
using AsCrone;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum StopWatchSubject
{
    Session,
    MainLevel,
    SubLevel
}
public class WatchAPI : MonoBehaviour
{
    internal DataLake dataBlock;
    internal StopWatchSubject subject;
    Stopwatch stopwatch;

    private void OnEnable()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

    public void Start()
    {
        switch (subject)
        {
            case StopWatchSubject.Session:
                break;
            case StopWatchSubject.MainLevel:
            case StopWatchSubject.SubLevel:
                CroneAPI.CroneEnventHandler.OnLevelFinished += Close;
                break;
        }

        transform.name = $"{subject}-API";
    }

    public void Close()
    {
        switch (subject)
        {
            case StopWatchSubject.Session:
                dataBlock.SessionPlayTimme = stopwatch.ElapsedMilliseconds;
                break;
            case StopWatchSubject.MainLevel:
                dataBlock.MainLevelTime = stopwatch.ElapsedMilliseconds;
                break;
            case StopWatchSubject.SubLevel:
                dataBlock.Sub_levelTime = stopwatch.ElapsedMilliseconds;
                break;
        }
        Debug.Log($"LEVEL PLAYTIME : {stopwatch.ElapsedMilliseconds}");
        stopwatch.Stop();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if(subject != StopWatchSubject.Session)
            CroneAPI.CroneEnventHandler.OnLevelFinished -= Close;

        CroneAPI.CroneEnventHandler.OnLevelFinished -= AsynCroner.CronePerforms.LevelPacked_InEventCallBack;
    }
}