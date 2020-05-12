using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class CameraVR_Disable : MonoBehaviour
{

    void Start()
    {
        XRSettings.enabled = false;
    }
}

