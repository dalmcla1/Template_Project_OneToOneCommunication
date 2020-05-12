using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class CameraVR_Enable : MonoBehaviour
{
    void Start()
    {
        XRSettings.enabled = true;
    }
}
