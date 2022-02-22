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
                CroneAPI.CroneEnventHandler.OnLevelFinished += Close_API;
                break;
        }

        transform.name = $"{subject}-API";
    }

    public void Close_API()
    {
        switch (subject)
        {
            case StopWatchSubject.Session:
                dataBlock.SessionPlayTimme = stopwatch.Elapsed.Seconds;
                break;
            case StopWatchSubject.MainLevel:
                dataBlock.MainLevelTime = stopwatch.Elapsed.Seconds;
                break;
            case StopWatchSubject.SubLevel:
                dataBlock.Sub_levelTime = stopwatch.Elapsed.Seconds;
                break;
        }
        stopwatch.Stop();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Close_API();
    }

    private void OnDestroy()
    {
        if(subject != StopWatchSubject.Session)
            CroneAPI.CroneEnventHandler.OnLevelFinished -= Close_API;

        CroneAPI.CroneEnventHandler.OnLevelFinished -= CroneAPI._CronePerformer.LevelPacked_InEventCallBack;
    }
}