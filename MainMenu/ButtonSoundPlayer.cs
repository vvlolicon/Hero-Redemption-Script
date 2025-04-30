using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ButtonSoundPlayer : MonoBehaviour
{
    AudioSource _audio;

    void Start()
    {
        InitializeButtonSound();
        _audio = GetComponent<AudioSource>();
    }
    public void InitializeButtonSound()
    {
        Button[] buttons = GameObject.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => _audio.Play());
        }
    }

}
