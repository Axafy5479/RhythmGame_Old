using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnableOnObjEnable : MonoBehaviour
{
    private void OnEnable()
    {
        this.GetComponent<Button>().enabled = true;
    }
}
