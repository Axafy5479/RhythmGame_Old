using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ClickOneTime : MonoBehaviour
{

    [SerializeField] private bool enter;

    private void Awake()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.7f;
        AudioClip clip = Resources.Load<AudioClip>($"Se/{(enter ? "enter" : "cancel")}");

        Button button = this.GetComponent<Button>();
        button.onClick.AddListener(() => {
            audioSource.PlayOneShot(clip);
            Debug.Log("ƒNƒŠƒbƒN"); 
            button.enabled = false;
        }); ;

    }
}
