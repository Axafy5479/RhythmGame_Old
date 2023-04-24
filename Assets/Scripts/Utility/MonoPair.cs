using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoPair<T> : MonoBehaviour where T:MonoPair<T>
{
    [SerializeField] private bool isPlayer;
    public bool IsPlayer => isPlayer;

    private static Dictionary<bool,T> iMap;

    public static T I(bool isPlayer)
    {
        if (iMap == null)
        {
            T[] i = FindObjectsOfType<T>();
            if (i.Length == 2)
            {
                iMap = new Dictionary<bool, T>()
                { { i[0].isPlayer, i[0] }, { i[1].isPlayer, i[1] } };
            }
            else
            {
                Debug.LogError(typeof(T).ToString() + "のインスタンスが" + i.Length + "個存在します。2個に修正してください");
                return null;
            }
        }
        return iMap[isPlayer];

    }

    protected virtual void OnDestroy()
    {
       
        Debug.Log("Destroy");
        iMap = null;
    }


}
