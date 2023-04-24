using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OpenDataPath : MonoBehaviour
{
    /// <summary>
    /// PersistentDataPathをOSごとに判定して開く
    /// </summary>
    [MenuItem("DataPath/Open PersistentPath")]
    public static void OpenPersistentPath()
    {

        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            System.Diagnostics.Process.Start(Application.persistentDataPath);
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            EditorUtility.RevealInFinder("C:/Users/SunnyMilk/AppData/LocalLow/FraMaricom/ReflecTouhou/data.json");
        }

    }
}
