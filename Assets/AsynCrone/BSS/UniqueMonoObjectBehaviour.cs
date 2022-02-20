using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSS
{
    /// <summary>
    /// Create volatiled fast instance with save opportunity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UniqueMonoObjectBehaviour<T> : MonoObject where T : UniqueMonoObjectBehaviour<T>
    {
        private static volatile T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                }
                return instance;
            }
        }

    }
}
