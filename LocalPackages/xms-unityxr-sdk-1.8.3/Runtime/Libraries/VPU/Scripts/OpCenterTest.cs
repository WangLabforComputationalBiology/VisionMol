using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Ximmerse.XR;

public class OpCenterTest : MonoBehaviour
{

    
    private void Awake()
    {
        XDevicePlugin.OpcInit("ABC");

        Debug.Log("OpServiceVersionCode:"+ XDevicePlugin.GetOpServiceVersionCode());

       


    }

    private void Start()
    {
        XDevicePlugin.OnMissionBegin();
    }
    private void OnApplicationQuit()
    {
        XDevicePlugin.OpcExit();
    }

    private void Update()
    {
        Debug.Log("Server ip:" + XDevicePlugin.GetBoundGameServerAddress());
    }

}
