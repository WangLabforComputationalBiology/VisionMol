using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Ximmerse.XR;

public class VPUTest : MonoBehaviour
{
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    private void Awake()
    {
        //Debug.Log("vpu client init");
        //XDevicePlugin.xdevc_init();
        //XDevicePlugin.ResetTrackingMarkerSettings();
        //int[] ids = new int[10];
        // XDevicePlugin.LoadTrackingMarkerSettingsFile("/sdcard/BEACON-500.json", out ids, 100);

        //NativePluginApi.Unity_setMaxPredictTime(30.0f);
        //NativePluginApi.Unity_setHalfExposureTime(11.0f);

    }

    private void Start()
    {
        //XDevicePlugin.ConnectController(0, "E7:5B:F4:C8:B5:FA", true);
    }


    private void Update()
    { 
        TestAPI2();
    }

    float px, py, pz, qx, qy, qz, qw;
    Int64 timestamp;
    float confidence;
    float marker_distance;
    int index;
    int state;

    long predictedTimeNs;
    int beacon_id;
    long beacon_timestamp;
    float beacon_pos0;
    float beacon_pos1;
    float beacon_pos2;
    float beacon_rot0;
    float beacon_rot1;
    float beacon_rot2;
    float beacon_rot3;
    float beacon_tracking_confidence;
    float beacon_min_distance;
    float beacon_correct_weight;
    private void TestAPI() {

        sw.Reset();
        sw.Start();
        int ret = NativePluginApi.Unity_TagPredict(0);

        if (ret >= 0){
            for (int i = 0; i < 100; i++) {
                bool ret2 = NativePluginApi.Unity_getTagTracking2(i,
                      ref index, ref timestamp, ref state,
                      ref px, ref py, ref pz,
                      ref qx, ref qy, ref qz, ref qw,
                      ref confidence, ref marker_distance);
              

                if (ret2 && state > 0) {
                    Debug.Log("TestAPI:GetTrackingByIDs:" + i+","+ px+","+py+","+pz+","+qx+","+qy+","+qz+","+qw);
                }
            }
        }
        
        sw.Stop();

        Debug.Log("c:GetTrackingByIDs cost time:" + sw.ElapsedMilliseconds);



        NativePluginApi.Unity_getFusionResult(predictedTimeNs,
                     ref beacon_id,
                     ref beacon_timestamp,
                     ref beacon_pos0,
                     ref beacon_pos1,
                     ref beacon_pos2,
                     ref beacon_rot0,
                     ref beacon_rot1,
                     ref beacon_rot2,
                     ref beacon_rot3,
                     ref beacon_tracking_confidence,
                     ref beacon_min_distance,
                     ref beacon_correct_weight);

        Debug.Log("c:getFusionResult:" + beacon_id);

    }

    private void TestAPI2()
    {
        bool ret2 = NativePluginApi.Unity_getControllerTracking(1,0,
                      ref index, ref timestamp, ref state,
                      ref px, ref py, ref pz,
                      ref qx, ref qy, ref qz, ref qw);


        if (ret2)
        {
           Debug.Log("TestAPI:ControllerTracking" + "," + px + "," + py + "," + pz + "," + qx + "," + qy + "," + qz + "," + qw);
        }

    }

}
