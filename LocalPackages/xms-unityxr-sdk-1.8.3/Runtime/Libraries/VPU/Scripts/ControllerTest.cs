using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Ximmerse.XR;

public class ControllerTest : MonoBehaviour
{
    public XDevicePlugin.XControllerTypes devieType = 0;
    private void Awake()
    {
    

    }

    private void Start()
    {
        //XDevicePlugin.ConnectController(0, "E7:5B:F4:C8:B5:FA", true);
    }

    float px, py, pz, qx, qy, qz, qw;
    Int64 timestamp;
    int index;
    int state;

    private void Update()
    {
        
        bool ret2 = NativePluginApi.Unity_getControllerTrackingByType((int)devieType, 0,
                 ref index, ref timestamp, ref state,
                 ref px, ref py, ref pz,
                 ref qx, ref qy, ref qz, ref qw);

        if (ret2)
        {
           // Debug.Log("TestAPI:ControllerTracking:" + (int)devieType + "," + px + "," + py + "," + pz + "," + qx + "," + qy + "," + qz + "," + qw);
            transform.position = new Vector3(px, py, pz);
            transform.rotation = new Quaternion(qx, qy, qz, qw);
        }

    }

    

   

    

}
