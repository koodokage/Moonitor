using UnityEngine;
namespace BSS
{
    /// <summary>
    /// Saveable MonoBehaviour Object
    /// </summary>
    public class MonoObject : MonoBehaviour
    {
        public SO_Data dataOptions;
        public virtual void Awake()
        {
            dataOptions.LoadLocalData();
        }

        public virtual void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                dataOptions.SaveLocalData();
            }
        }
    }
}


