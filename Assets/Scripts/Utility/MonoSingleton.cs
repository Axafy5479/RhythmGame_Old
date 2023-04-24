using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T:MonoBehaviour
{
    #region Singleton
    private static T instance;

    public static T I
    {
        get
        {
            T[] instances = null;
            if (instance == null)
            {
                instances = FindObjectsOfType<T>();
                if (instances.Length == 0)
                {
                    Debug.LogError(typeof(T).ToString()+"のインスタンスが存在しません");
                    return null;
                }
                else if (instances.Length > 1)
                {
                    Debug.LogError(typeof(T).ToString()+"のインスタンスが複数存在します");
                    return null;
                }
                else
                {
                    instance = instances[0];
                    (instance as MonoSingleton<T>).OnInstanceFirstCalled();
                }
            }
            return instance;
        }
    }
    #endregion

    protected virtual void OnInstanceFirstCalled() { }

}
