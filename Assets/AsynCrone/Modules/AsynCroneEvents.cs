using UnityEngine;
namespace AsCrone.Module
{
    public class AsynCroneEvents : MonoBehaviour
    {
        public delegate void LevelFinished();
        public event LevelFinished OnLevelFinished;

        public void OnLevelFinish()
        {
            OnLevelFinished.Invoke();
        }
    }
}

