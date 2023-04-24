using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugging : MonoBehaviour
{
    public void ButtonClicked()
    {
        Debug.Log(Time.timeScale);
    }
}
