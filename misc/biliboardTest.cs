using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class biliboardTest : MonoBehaviour
{
    public Camera Cam { get; set; }
    void Start()
    {
        if (Cam == null)
        {
            Cam = Camera.main;
        }
    }
    private void LateUpdate()
    {
        // make the canvas points at player
        if(gameObject.activeSelf)
            transform.LookAt(transform.position + Cam.transform.forward);
    }
}
