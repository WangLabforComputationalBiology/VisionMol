using System;
using System.Runtime.InteropServices;
using System.Threading;
using TouchlessA3D;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//This Monobehaviour manages the connection to the gesture engine and syncs event to the Unity Update loop
//it is better to sync the events before other scripts by setting this first in the Unity Project Settings -> Script Execution Order 
public class UnityTouchlessSession : ITouchlessSession
{
    public float startDelay = 1.0f;
    //subscribe events to the eventhandler with ITouchlessSession.instance.ta3dEventHandler+=functionName;
    //if the function is a method, remember to remove the event when the object containing it is destroyed, eg OnDestroyed(){ITouchlessSession.instance.ta3dEventHandler-=functionName}
    public UnityCameraIntegration camIntegration;
    private WebCamTexture camTex;
    private Color32[] imgData = new Color32[UnityCameraIntegration.width * UnityCameraIntegration.height];
    private GCHandle handleToData;
    private string uniqueID;
    private string storageLocation;
    private GestureEvent locked_gestureEvent;
    private readonly object lockObj = new object();
    private Engine touchlessEngine;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != instance)
        {
            DestroyImmediate(this);
        }
    }

    private bool isStarted = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        handleToData = GCHandle.Alloc(imgData, GCHandleType.Pinned);
        uniqueID = SystemInfo.deviceUniqueIdentifier;
        storageLocation = Application.persistentDataPath;
        //var calibration = new NativeCalibration();
        //touchlessEngine = new Engine(uniqueID, storageLocation, calibration);
        isStarted = true;
    }

    private void Update()
    {
        if(!isStarted)
        {
            return;
        }
        GestureEvent syncedEvent = null;
        lock (lockObj)
        {
            if (locked_gestureEvent != null)
            {
                syncedEvent = new GestureEvent(locked_gestureEvent);
                locked_gestureEvent = null;
            }
        }
        if (null != syncedEvent && null != ta3dEventHandler)
        {
            ta3dEventHandler(this, syncedEvent);
        }
        ProccessOneFrame();
    }

    private void TouchlessEvent(object sender, GestureEvent args)
    {
        lock (lockObj)
        {
            locked_gestureEvent = args;
        }
    }

    private void ProccessOneFrame()
    {
        if (null == camTex) camTex = camIntegration.cameraTexture;

        if (null == camTex || !camTex.didUpdateThisFrame)
        {
            return;
        }
        camTex.GetPixels32(imgData);
        using (var frame = new TouchlessA3D.Frame(handleToData.AddrOfPinnedObject(), camTex.width * 4,
          camTex.width, camTex.height, System.DateTimeOffset.Now.ToUnixTimeMilliseconds(),
          FrameRotation.ROTATION_NONE, true))
        {
            touchlessEngine.handleFrame(frame);
        }
    }

    private void OnApplicationQuit()
    {
        if (null != handleToData)
        {
            handleToData.Free();
        }
    }
}