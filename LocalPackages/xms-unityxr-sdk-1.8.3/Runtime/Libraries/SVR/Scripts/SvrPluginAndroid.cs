using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class SvrPluginAndroid : SvrPlugin
{

    public enum RenderEvent
	{
		Initialize,
		BeginVr,
		EndVr,
		BeginEye,
		EndEye,
		SubmitFrame,
        Foveation,
		Shutdown,
		RecenterTracking,
		SetTrackingMode,
		SetPerformanceLevels,
        OccludeEye,
        OcclusionMesh,
        AnchorData,
        CloudData,
        PauseXr,
        ResumeXr,
    };

    public static SvrPluginAndroid Create()
	{
        Debug.Log("SvrPluginAndroid Create");
        if (Application.isEditor)
		{
			Debug.LogError("SvrPlugin not supported in unity editor!");
			throw new InvalidOperationException();
		}

        return new SvrPluginAndroid ();
	}

    private SvrPluginAndroid()
    {
        beginEyeCommandBuffer = new CommandBuffer();
        //beginEyeCommandBuffer.ClearRenderTarget(true, true, Color.red);
        beginEyeCommandBuffer.IssuePluginEvent(GetRenderEventFunc(), (int)RenderEvent.OccludeEye);
    }

    internal void IssueEvent(RenderEvent e)
	{
		// Queue a specific callback to be called on the render thread
		GL.IssuePluginEvent(GetRenderEventFunc(), (int)e);
	}

    public override bool IsInitialized() { return SvrIsInitialized(); }

    public override bool IsRunning() { return SvrIsRunning(); }

    public override IEnumerator Initialize()
	{
        //yield return new WaitUntil(() => SvrIsInitialized() == false);  // Wait for shutdown

        yield return base.Initialize();

#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

		SvrInitializeEventData(activity.GetRawObject());
#endif
        SXR.ParamLoader.ParamLoaderInit((int)deviceModel);
        IssueEvent(RenderEvent.Initialize);
		yield return new WaitUntil (() => SvrIsInitialized () == true);

        yield return null;  // delay one frame - fix for re-init w multi-threaded rendering

        deviceInfo = GetDeviceInfo();
    }

	public override void BeginVr(int cpuPerfLevel, int gpuPerfLevel, int optionFlags)
	{
        Debug.Log("=>>BeginVr");
        //yield return new WaitUntil(() => SvrIsRunning() == false);  // Wait for EndVr

        //yield return base.BeginVr(cpuPerfLevel, gpuPerfLevel, optionFlags);

        // float[6]: x, y, z, w, u, v
        //float[] lowerLeft = { -1f, -1f, 0f, 1f, 0f, 0f };
        //float[] upperLeft = { -1f,  1f, 0f, 1f, 0f, 1f };
        //float[] upperRight = { 1f,  1f, 0f, 1f, 1f, 1f };
        //float[] lowerRight = { 1f, -1f, 0f, 1f, 1f, 0f };
        //SvrSetupLayerCoords(-1, lowerLeft, lowerRight, upperLeft, upperRight);    // Layers/All

        SvrSetPerformanceLevelsEventData(cpuPerfLevel, gpuPerfLevel);

        SvrSetOptionFlags(optionFlags);

        //if ( ((optionFlags & (int)SvrManager.SvrSettings.eOptionFlags.FoveationSubsampled) != 0) && (SystemInfo.supportsMultisampledTextures == 0))
        //{
        //    Debug.LogWarning("FoveationSampled requested but not supported");
        //}
		
		ColorSpace space = QualitySettings.activeColorSpace;
		if(space == ColorSpace.Gamma)
		{
			// 0 == kColorSpaceLinear from svrApi.h
			SvrSetColorSpace(0);   //Unity will be supplying gamma space eye buffers into warp so we want
								   //to setup a linear color space display surface so no further gamma 
								   //correction is performed
		}
		else
		{
			// 1 == kColorSpaceSRGB from svrApi.h
			SvrSetColorSpace(1);	//Unity will be supplying linear space eye buffers into warp so we want
									//to setup an sRGB color space display surface to properly convert
									//incoming linear values into sRGB
		}
        
		//yield return new WaitUntil(() => SvrCanBeginVR() == true);
        //IssueEvent (RenderEvent.BeginVr);
       // yield return null;  // delay one frame - fix for multi-threaded rendering
    }

    public override void EndVr()
	{
        base.EndVr();
		IssueEvent (RenderEvent.EndVr);
	}

    public override void PauseXr()
    {
        base.PauseXr();
        IssueEvent(RenderEvent.PauseXr);
    }

    public override void ResumeXr()
    {
        base.ResumeXr();
        IssueEvent(RenderEvent.ResumeXr);
    }

    public override void BeginEye(int renderIndex, int sideMask, float[] frameDelta)
	{
        SvrSetFrameOffset(renderIndex, frameDelta);    // Enabled for foveation head orientation delta
        SvrSetEyeEventData(renderIndex, sideMask, 0);
        IssueEvent (RenderEvent.BeginEye);
	}

    public override void OccludeEye(int renderIndex, Matrix4x4 projection, Matrix4x4 view)
    {
        //float[] projMtx = { proj.m00, proj.m01, proj.m02, proj.m03,
        //                    proj.m10, proj.m11, proj.m12, proj.m13,
        //                    proj.m20, proj.m21, proj.m22, proj.m23,
        //                    proj.m30, proj.m31, proj.m32, proj.m33};

        //float[] viewMtx = { view.m00, view.m01, view.m02, view.m03,
        //                    view.m10, view.m11, view.m12, view.m13,
        //                    view.m20, view.m21, view.m22, view.m23,
        //                    view.m30, view.m31, view.m32, view.m33};

        Matrix4x4 proj = GL.GetGPUProjectionMatrix(projection, true);

        float[] projMtx = { proj.m00, proj.m10, proj.m20, proj.m30,
                            proj.m01, proj.m11, proj.m21, proj.m31,
                            proj.m02, proj.m12, proj.m22, proj.m32,
                            proj.m03, proj.m13, proj.m23, proj.m33};

        float[] viewMtx = { view.m00, view.m10, view.m20, view.m30,
                            view.m01, view.m11, view.m21, view.m31,
                            view.m02, view.m12, view.m22, view.m32,
                            view.m03, view.m13, view.m23, view.m33};

        SvrSetEyeRenderData(renderIndex, projMtx, viewMtx);
        //IssueEvent(RenderEvent.OccludeEye);
    }

    public override void EndEye(int renderIndex, int sideMask, int layerMask)
	{
        SvrSetEyeEventData(renderIndex, sideMask, layerMask);
        IssueEvent(RenderEvent.EndEye);
	}

    public override void SetTrackingMode(int modeMask)
    {
        Debug.Log("SetTrackingMode:"+ modeMask);
        SvrSetTrackingModeEventData(modeMask);
		IssueEvent (RenderEvent.SetTrackingMode);
    }

	public override void SetFoveationParameters(int renderIndex, int textureId, int previousId, float focalPointX, float focalPointY, float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum)
	{
		SvrSetFoveationParameters(renderIndex, textureId, previousId, focalPointX, focalPointY, foveationGainX, foveationGainY, foveationArea, foveationMinimum);
	}

    public override void ApplyFoveation()
    {
        IssueEvent(RenderEvent.Foveation);
    }

    public override int GetTrackingMode()
    {
        return SvrGetTrackingMode();
    }

    public override void SetPerformanceLevels(int newCpuPerfLevel, int newGpuPerfLevel)
    {
        SvrSetPerformanceLevelsEventData((int)newCpuPerfLevel, (int)newGpuPerfLevel);
		IssueEvent (RenderEvent.SetPerformanceLevels);
    }

    public override void SetFrameOption(FrameOption frameOption)
    {
        SvrSetFrameOption((uint)frameOption);
    }

    public override void UnsetFrameOption(FrameOption frameOption)
    {
        SvrUnsetFrameOption((uint)frameOption);
    }

    public override void SetVSyncCount(int vSyncCount)
    {
        Debug.Log("SetVSyncCount:"+ vSyncCount);
        SvrSetVSyncCount(vSyncCount);
    }

    public override bool RecenterTracking()
	{
        //IssueEvent (RenderEvent.RecenterTracking);
        return SvrRecenterTrackingPose();
	}

    public override int GetPredictedPose(ref Quaternion orientation, ref Vector3 position, int frameIndex)
    {
        UInt64 timestamp = 0;   // Unused

        orientation.z = -orientation.z;
        position.x = -position.x;
        position.y = -position.y;

        int rv = SvrGetPredictedPose(ref timestamp, ref orientation.x, ref orientation.y, ref orientation.z, ref orientation.w,
                            ref position.x, ref position.y, ref position.z, frameIndex, SystemInfo.graphicsMultiThreaded);

        orientation.z = -orientation.z;
        position.x = -position.x;
        position.y = -position.y;

        return rv;
    }
    public override int GetHeadPose(ref HeadPose headPose, int frameIndex)
    {
        headPose.orientation.z = -headPose.orientation.z;
        headPose.position.x = -headPose.position.x;
        headPose.position.y = -headPose.position.y;

        int rv = SvrGetPredictedPose(ref headPose.timestamp, ref headPose.orientation.x, ref headPose.orientation.y, ref headPose.orientation.z, ref headPose.orientation.w,
                            ref headPose.position.x, ref headPose.position.y, ref headPose.position.z, frameIndex, SystemInfo.graphicsMultiThreaded);

        headPose.orientation.z = -headPose.orientation.z;
        headPose.position.x = -headPose.position.x;
        headPose.position.y = -headPose.position.y;

        return rv;
    }

    public override int GetEyePose(ref EyePose eyePose, int frameIndex = -1)
    {
        // Transform Unity to SVR by negating z-axis
        eyePose.leftDirection.z = -eyePose.leftDirection.z;
        eyePose.rightDirection.z = -eyePose.rightDirection.z;
        eyePose.combinedDirection.z = -eyePose.combinedDirection.z;

        eyePose.leftPosition.z = -eyePose.leftPosition.z;
        eyePose.rightPosition.z = -eyePose.rightPosition.z;
        eyePose.combinedPosition.z = -eyePose.combinedPosition.z;

        int rv = SvrGetEyePose(
            ref eyePose.timestamp,
            ref eyePose.leftStatus, ref eyePose.rightStatus, ref eyePose.combinedStatus,
            ref eyePose.leftBlink, ref eyePose.rightBlink,
            ref eyePose.leftOpenness, ref eyePose.rightOpenness,
            ref eyePose.leftDilation, ref eyePose.rightDilation,
            ref eyePose.leftDirection.x, ref eyePose.leftDirection.y, ref eyePose.leftDirection.z,
            ref eyePose.leftPosition.x, ref eyePose.leftPosition.y, ref eyePose.leftPosition.z,
            ref eyePose.leftGuide.x, ref eyePose.leftGuide.y, ref eyePose.leftGuide.z,
            ref eyePose.rightDirection.x, ref eyePose.rightDirection.y, ref eyePose.rightDirection.z,
            ref eyePose.rightPosition.x, ref eyePose.rightPosition.y, ref eyePose.rightPosition.z,
            ref eyePose.rightGuide.x, ref eyePose.rightGuide.y, ref eyePose.rightGuide.z,
            ref eyePose.combinedDirection.x, ref eyePose.combinedDirection.y, ref eyePose.combinedDirection.z,
            ref eyePose.combinedPosition.x, ref eyePose.combinedPosition.y, ref eyePose.combinedPosition.z,
            frameIndex);

        // Transform SVR to Unity by negating z-axis
        eyePose.leftDirection.z = -eyePose.leftDirection.z;
        eyePose.rightDirection.z = -eyePose.rightDirection.z;
        eyePose.combinedDirection.z = -eyePose.combinedDirection.z;

        eyePose.leftPosition.z = -eyePose.leftPosition.z;
        eyePose.rightPosition.z = -eyePose.rightPosition.z;
        eyePose.combinedPosition.z = -eyePose.combinedPosition.z;

        return rv;
    }

    public override int GetEyeFocalPoint(ref Vector2 focalPoint)
    {
        ulong timeStamp = 0;
        ulong foveatedState = 0;
        Vector3 foveatedDirection = Vector3.back;

        int rv = SvrGetEyeFocalPoint(
            ref timeStamp,
            ref foveatedState,
            ref foveatedDirection.x, ref foveatedDirection.y, ref foveatedDirection.z);

        // Transform SVR to Unity by negating z-axis
        foveatedDirection.z = -foveatedDirection.z;

        if (foveatedDirection.sqrMagnitude > 0f)
        {
            foveatedDirection.Normalize();
            //Debug.LogFormat("Eye Direction: ({0:F2}, {1:F2}, {2:F2})", foveatedDirection.x, foveatedDirection.y, foveatedDirection.z);

            float denominator = Vector3.Dot(foveatedDirection, Vector3.forward);
            if (denominator > float.Epsilon)
            {
                // eye direction intersection with frustum near plane (left)
                var eyePoint = foveatedDirection * deviceInfo.targetFrustumLeft.near / denominator;

                // size of the frustum near plane (left)
                var nearSize = new Vector2(0.5f * (deviceInfo.targetFrustumLeft.right - deviceInfo.targetFrustumLeft.left),
                    0.5f * (deviceInfo.targetFrustumLeft.top - deviceInfo.targetFrustumLeft.bottom));

                focalPoint.Set(eyePoint.x / nearSize.x, eyePoint.y / nearSize.y);   // Normalized [-1,1]
                //Debug.LogFormat("[{0}] Eye Focus: {1}", timeStamp, focalPoint.ToString());
            }
        }

        return rv;
    }

    public override bool Is3drOcclusion()
    {
        return SvrIs3drOcclusion();
    }

    public override void GetOcclusionMesh()
    {
        IssueEvent(RenderEvent.OcclusionMesh);
    }

    public override DeviceInfo GetDeviceInfo()
	{
        DeviceInfo info = new DeviceInfo();
        info.trackingCalibration = Matrix4x4.identity;

        SvrGetDeviceInfo (ref info.displayWidthPixels,
                          ref info.displayHeightPixels,
                          ref info.displayRefreshRateHz,
                          ref info.targetEyeWidthPixels,
                          ref info.targetEyeHeightPixels,
                          ref info.targetFovXRad,
                          ref info.targetFovYRad,
                          ref info.targetFrustumLeft.left, ref info.targetFrustumLeft.right, ref info.targetFrustumLeft.bottom, ref info.targetFrustumLeft.top, ref info.targetFrustumLeft.near, ref info.targetFrustumLeft.far,
                          ref info.targetFrustumLeft.position.x, ref info.targetFrustumLeft.position.y, ref info.targetFrustumLeft.position.z,
                          ref info.targetFrustumLeft.rotation.x, ref info.targetFrustumLeft.rotation.y, ref info.targetFrustumLeft.rotation.z, ref info.targetFrustumLeft.rotation.w,
                          ref info.targetFrustumRight.left, ref info.targetFrustumRight.right, ref info.targetFrustumRight.bottom, ref info.targetFrustumRight.top, ref info.targetFrustumRight.near, ref info.targetFrustumRight.far,
                          ref info.targetFrustumRight.position.x, ref info.targetFrustumRight.position.y, ref info.targetFrustumRight.position.z,
                          ref info.targetFrustumRight.rotation.x, ref info.targetFrustumRight.rotation.y, ref info.targetFrustumRight.rotation.z, ref info.targetFrustumRight.rotation.w, 
                          ref info.targetFrustumConvergence, ref info.targetFrustumPitch,
                          ref info.lowFoveation.Gain.x, ref info.lowFoveation.Gain.y, ref info.lowFoveation.Area, ref info.lowFoveation.Minimum,
                          ref info.medFoveation.Gain.x, ref info.medFoveation.Gain.y, ref info.medFoveation.Area, ref info.medFoveation.Minimum,
                          ref info.highFoveation.Gain.x, ref info.highFoveation.Gain.y, ref info.highFoveation.Area, ref info.highFoveation.Minimum,
                          ref info.trackingCalibration.m00, ref info.trackingCalibration.m01, ref info.trackingCalibration.m02, ref info.trackingCalibration.m03,
                          ref info.trackingCalibration.m10, ref info.trackingCalibration.m11, ref info.trackingCalibration.m12, ref info.trackingCalibration.m13,
                          ref info.trackingCalibration.m20, ref info.trackingCalibration.m21, ref info.trackingCalibration.m22, ref info.trackingCalibration.m23,
                          ref info.trackingIntrinsics.PrincipalPoint.x, ref info.trackingIntrinsics.PrincipalPoint.y,
                          ref info.trackingIntrinsics.FocalLength.x, ref info.trackingIntrinsics.FocalLength.y,
                          ref info.trackingIntrinsics.Distortion0, ref info.trackingIntrinsics.Distortion1, ref info.trackingIntrinsics.Distortion2, ref info.trackingIntrinsics.Distortion3, ref info.trackingIntrinsics.Distortion4, ref info.trackingIntrinsics.Distortion5, ref info.trackingIntrinsics.Distortion6, ref info.trackingIntrinsics.Distortion7,
                          ref info.trackingCapabilities
                          );

        // Transform coords from SVR to Unity by negating z-axis (negate position.x, rotation.x, and rotation.y)
        info.targetFrustumLeft.position.z = -info.targetFrustumLeft.position.z;
        info.targetFrustumLeft.rotation.x = -info.targetFrustumLeft.rotation.x;
        info.targetFrustumLeft.rotation.y = -info.targetFrustumLeft.rotation.y;

        info.targetFrustumRight.position.z = -info.targetFrustumRight.position.z;
        info.targetFrustumRight.rotation.x = -info.targetFrustumRight.rotation.x;
        info.targetFrustumRight.rotation.y = -info.targetFrustumRight.rotation.y;
       
        return info;
	}


	public override void Shutdown()
	{
        IssueEvent (RenderEvent.Shutdown);

        base.Shutdown();
	}

  //  public override bool PollEvent(ref SvrManager.SvrEvent frameEvent)
  //  {
  //      int dataCount = Marshal.SizeOf(frameEvent.eventData) / sizeof(uint);
  //      uint[] dataBuffer = new uint[dataCount];
		//int eventType = 0;
  //      bool isEvent = SvrPollEvent(ref eventType, ref frameEvent.deviceId, ref frameEvent.eventTimeStamp, dataCount, dataBuffer);
		//frameEvent.eventType = (SvrManager.svrEventType)(eventType);
  //      switch (frameEvent.eventType)
  //      {
  //          case SvrManager.svrEventType.kEventThermal:
  //              //Debug.LogFormat("PollEvent: data {0} {1}", dataBuffer[0], dataBuffer[1]);
  //              frameEvent.eventData.thermal.zone = (SvrManager.svrThermalZone)dataBuffer[0];
  //              frameEvent.eventData.thermal.level = (SvrManager.svrThermalLevel)dataBuffer[1];
  //              break;
  //          case SvrManager.svrEventType.kEventProximity:
  //              //Debug.LogFormat("PollEvent: data {0} {1}", dataBuffer[0], dataBuffer[1]);
  //              frameEvent.eventData.proximity.distance = (float)dataBuffer[0];
  //              break;
  //          case SvrManager.svrEventType.kEventControllerConnected:
  //          case SvrManager.svrEventType.kEventControllerConnecting:
  //          case SvrManager.svrEventType.kEventControllerDisconnected:
  //              frameEvent.eventData.data = dataBuffer[0];
  //              break;
  //      }
  //      return isEvent;
  //  }

	//---------------------------------------------------------------------------------------------
	//Controller Apis
	//---------------------------------------------------------------------------------------------

	/// <summary>
	/// Controllers the start tracking.
	/// </summary>
	/// <returns>The start tracking.</returns>
	/// <param name="desc">Desc.</param>
	//---------------------------------------------------------------------------------------------
    public override int ControllerStartTracking(string desc)
    {
        return SvrControllerStartTracking(desc);
    }
    
	/// <summary>
	/// Controllers the stop tracking.
	/// </summary>
	/// <param name="handle">Handle.</param>
	//---------------------------------------------------------------------------------------------
	public override void ControllerStopTracking(int handle)
    {
        SvrControllerStopTracking(handle);
    }

	/// <summary>
	/// Dumps the state.
	/// </summary>
	/// <param name="state">State.</param>
	//---------------------------------------------------------------------------------------------

   

    public override int CloudPointData(CloudPoint[] cloudData)
    {
        //IssueEvent(RenderEvent.CloudData);
        SvrCloudGetData();

        int pointCount = 0;
        while (pointCount < cloudData.Length &&
            SvrCloudGetDataElement(pointCount, ref cloudData[pointCount].id,  ref cloudData[pointCount].x, ref cloudData[pointCount].y, ref cloudData[pointCount].z))
        {
            pointCount++;
        }

        return pointCount;
    }

    public override string AnchorToString(AnchorUuid id)
    {
        return SvrAnchorToString(ref id);
    }

    public override bool AnchorCreate(Vector3 position, Quaternion rotation, ref AnchorUuid id)
    {
        // Transform Unity to SXR
        return SvrAnchorCreate(-position.x, -position.y, position.z, rotation.x, rotation.y, -rotation.z, rotation.w, ref id);
        //return SvrAnchorCreate(position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w, ref id);
    }

    public override bool AnchorDestroy(AnchorUuid id)
    {
        return SvrAnchorDestroy(id);
    }

    public override bool AnchorSave(AnchorUuid id)
    {
        return SvrAnchorSave(id, Application.persistentDataPath);
    }

    public override int AnchorGetData(AnchorInfo[] anchorData)
    {
        //IssueEvent(RenderEvent.AnchorData);
        SvrAnchorGetData();

        int anchorCount = 0;
        while (anchorCount < anchorData.Length &&
            SvrAnchorGetDataElement(anchorCount, ref anchorData[anchorCount].id, ref anchorData[anchorCount].revision,
                ref anchorData[anchorCount].pose.position.x, ref anchorData[anchorCount].pose.position.y, ref anchorData[anchorCount].pose.position.z,
                ref anchorData[anchorCount].pose.orientation.x, ref anchorData[anchorCount].pose.orientation.y, ref anchorData[anchorCount].pose.orientation.z, ref anchorData[anchorCount].pose.orientation.w,
                ref anchorData[anchorCount].pose.poseQuality))
        {
            // Transform Unity to SXR
            anchorData[anchorCount].pose.orientation.z = -anchorData[anchorCount].pose.orientation.z;
            anchorData[anchorCount].pose.position.x = -anchorData[anchorCount].pose.position.x;
            anchorData[anchorCount].pose.position.y = -anchorData[anchorCount].pose.position.y;

            anchorCount++;
        }
        return anchorCount;
    }

    public override bool AnchorStartRelocating()
    {
        return SvrAnchorRelocatingStart(Application.persistentDataPath);
    }

    public override bool AnchorStopRelocating()
    {
        return SvrAnchorRelocatingStop();
    }

    #region Native-Interfaces
    protected static string Tag = "SvrPluginAndroid";
    [DllImport("svrplugin")]
    internal static extern IntPtr GetRenderEventFunc();

    [DllImport("unity_native_api")]
    internal static extern void Unity_setReticleTextureId(int reticleTextureID, int texWidth, int texHeight);

    [DllImport("unity_native_api")]
    internal static extern bool Unity_getReticleRendering();

    [DllImport("unity_native_api")]
    internal static extern void Unity_setReticleRendering(bool renderFlag);


    [DllImport("unity_native_api")]
    internal static extern bool Unity_getOverlayRendering();

    /// <summary>
    /// 开关 overlay rendering 特性
    /// </summary>
    /// <param name="enableOverlayRendering"></param>
    [DllImport("unity_native_api")]
    internal static extern void Unity_setOverlayRendering(bool enableOverlayRendering);

    /// <summary>
    /// 设置 overlay rendering 的 texture id.
    /// </summary>
    /// <param name="overlayTextureIDLeft"></param>
    /// <param name="overlayTextureIDRight"></param>
    [DllImport("unity_native_api")]
    internal static extern void Unity_setOverlayRenderingTextureId(int overlayTextureIDLeft, int overlayTextureIDRight);
    /// <summary>
    /// sets world space overlay texture params
    /// </summary>
    /// <param name="renderingFlag"></param>
    /// <param name="layerIndex"></param>
    /// <param name="textureId"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="anchorMatrix"></param>
    [DllImport("unity_native_api")]
    internal static extern void Unity_setWorldOverlayTexture(bool renderingFlag, int layerIndex, int textureId, float width, float height, float[] anchorMatrix,
        float[] ll, float[] lt, float[] rt, float[] rb);

    [DllImport("unity_native_api")]
    internal static extern void Unity_setFrame(int frame);

    [DllImport("svrplugin")]
    private static extern bool SvrIsInitialized();

    [DllImport("svrplugin")]
    public static extern bool SvrIsRunning();

    [DllImport("svrplugin")]
    private static extern bool SvrCanBeginVR();

    [DllImport("svrplugin")]
    private static extern void SvrInitializeEventData(IntPtr activity);

    [DllImport("svrplugin")]
    internal static extern void SvrSubmitFrameEventData(int frameIndex, float fieldOfView, int frameType);

    [DllImport("svrplugin")]
    internal static extern void SvrSetupLayerCoords(int layerIndex, float[] lowerLeft, float[] lowerRight, float[] upperLeft, float[] upperRight);

    [DllImport("svrplugin")]
    internal static extern void SvrSetupLayerData(int layerIndex, int sideMask, int textureId, int textureType, int layerFlags);


    [DllImport("svrplugin")]
    private static extern void SvrSetTrackingModeEventData(int mode);

    [DllImport("svrplugin")]
    private static extern void SvrSetPerformanceLevelsEventData(int newCpuPerfLevel, int newGpuPerfLevel);

    [DllImport("svrplugin")]
    private static extern void SvrSetEyeEventData(int renderIndex, int sideMask, int layerMask);

    [DllImport("svrplugin")]
    private static extern void SvrSetEyeRenderData(int renderIndex, float[] eyeProjMat, float[] eyeViewMat);

    [DllImport("svrplugin")]
    private static extern void SvrSetColorSpace(int colorSpace);

    [DllImport("svrplugin")]
    private static extern void SvrSetOptionFlags(int optionFlags);

    [DllImport("svrplugin")]
    private static extern void SvrSetFrameOption(uint frameOption);

    [DllImport("svrplugin")]
    private static extern void SvrUnsetFrameOption(uint frameOption);

    [DllImport("svrplugin")]
    private static extern void SvrSetVSyncCount(int vSyncCount);

    [DllImport("svrplugin")]
    private static extern int SvrGetPredictedPose(ref UInt64 timeStampNs,
                                                   ref float rx,
                                                   ref float ry,
                                                   ref float rz,
                                                   ref float rw,
                                                   ref float px,
                                                   ref float py,
                                                   ref float pz,
                                                   int frameIndex,
                                                   bool isMultiThreadedRender);
    [DllImport("svrplugin")]
    private static extern int SvrGetEyeFocalPoint(ref UInt64 timeStampNs,
                                            ref UInt64 foveatedStatus,
                                            ref float foveatedDirectionX,
                                            ref float foveatedDirectionY,
                                            ref float foveatedDirectionZ);
    [DllImport("svrplugin")]
    private static extern int SvrGetEyePose(ref UInt64 timeStampNs,
                                            ref int leftStatus,
                                            ref int rightStatus,
                                            ref int combinedStatus,
                                            ref bool leftBlink,
                                            ref bool rightBlink,
                                            ref float leftOpenness,
                                            ref float rightOpenness,
                                            ref float leftDilation,
                                            ref float rightDilation,
                                            ref float leftDirectionX,
                                            ref float leftDirectionY,
                                            ref float leftDirectionZ,
                                            ref float leftPositionX,
                                            ref float leftPositionY,
                                            ref float leftPositionZ,
                                            ref float leftGuideX,
                                            ref float leftGuideY,
                                            ref float leftGuideZ,
                                            ref float rightDirectionX,
                                            ref float rightDirectionY,
                                            ref float rightDirectionZ,
                                            ref float rightPositionX,
                                            ref float rightPositionY,
                                            ref float rightPositionZ,
                                            ref float rightGuideX,
                                            ref float rightGuideY,
                                            ref float rightGuideZ,
                                            ref float combinedDirectionX,
                                            ref float combinedDirectionY,
                                            ref float combinedDirectionZ,
                                            ref float combinedPositionX,
                                            ref float combinedPositionY,
                                            ref float combinedPositionZ,
                                            int frameIndex);

    [DllImport("svrplugin")]
    private static extern bool SvrRecenterTrackingPose();

    [DllImport("svrplugin")]
    private static extern int SvrGetTrackingMode();

    [DllImport("svrplugin")]
    private static extern bool SvrIs3drOcclusion();

    [DllImport("svrplugin")]
    private static extern void SvrGetDeviceInfo(ref int displayWidthPixels,
                                                ref int displayHeightPixels,
                                                ref float displayRefreshRateHz,
                                                ref int targetEyeWidthPixels,
                                                ref int targetEyeHeightPixels,
                                                ref float targetFovXRad,
                                                   ref float targetFovYRad,
                                                ref float leftFrustumLeft, ref float leftFrustumRight, ref float leftFrustumBottom, ref float leftFrustumTop, ref float leftFrustumNear, ref float leftEyeFrustumFar,
                                                ref float leftFrustumPositionX, ref float leftFrustumPositionY, ref float leftFrustumPositionZ,
                                                ref float leftFrustumRotationX, ref float leftFrustumRotationY, ref float leftFrustumRotationZ, ref float leftFrustumRotationW,
                                                ref float rightFrustumLeft, ref float rightFrustumRight, ref float rightFrustumBottom, ref float rightFrustumTop, ref float rightFrustumNear, ref float rightFrustumFar,
                                                ref float rightFrustumPositionX, ref float rightFrustumPositionY, ref float rightFrustumPositionZ,
                                                ref float rightFrustumRotationX, ref float rightFrustumRotationY, ref float rightFrustumRotationZ, ref float rightFrustumRotationW,
                                                ref float targetfrustumConvergence, ref float targetFrustumPitch,
                                                ref float lowFoveationGainX, ref float lowFoveationGainY, ref float lowFoveationArea, ref float lowFoveationMinimum,
                                                ref float medFoveationGainX, ref float medFoveationGainY, ref float medFoveationArea, ref float medFoveationMinimum,
                                                ref float highFoveationGainX, ref float highFoveationGainY, ref float highFoveationArea, ref float highFoveationMinimum,
                                                ref float trackingCalibrationM00, ref float trackingCalibrationM01, ref float trackingCalibrationM02, ref float trackingCalibrationM03,
                                                ref float trackingCalibrationM10, ref float trackingCalibrationM11, ref float trackingCalibrationM12, ref float trackingCalibrationM13,
                                                ref float trackingCalibrationM20, ref float trackingCalibrationM21, ref float trackingCalibrationM22, ref float trackingCalibrationM23,
                                                ref float trackingPrincipalPointX, ref float trackingPrincipalPointY,
                                                ref float trackingFocalLengthX, ref float trackingFocalLengthY,
                                                ref float trackingDistortion0, ref float trackingDistortion1, ref float trackingDistortion2, ref float trackingDistortion3, ref float trackingDistortion4, ref float trackingDistortion5, ref float trackingDistortion6, ref float trackingDistortion7,
                                                ref ulong trackingCapabilities);

    [DllImport("svrplugin")]
    private static extern void SvrSetFrameOffset(int renderIndex, float[] delta);

    [DllImport("svrplugin")]
    private static extern void SvrSetFoveationParameters(int renderIndex, int textureId, int previousId, float focalPointX, float focalPointY, float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum);

    [DllImport("svrplugin")]
    public static extern bool SvrPollEvent(ref int eventType, ref uint deviceId, ref float eventTimeStamp, int eventDataCount, uint[] eventData);

    //---------------------------------------------------------------------------------------------
    // Controller Api
    //---------------------------------------------------------------------------------------------
    [DllImport("svrplugin")]
    private static extern int SvrControllerStartTracking(string desc);

    [DllImport("svrplugin")]
    private static extern void SvrControllerStopTracking(int handle);

    [DllImport("svrplugin")]
    private static extern void SvrControllerSendMessage(int handle, int what, int arg1, int arg2);

    [DllImport("svrplugin")]
    private static extern int SvrControllerQuery(int handle, int what, IntPtr mem, int size);
    //---------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------
    // Anchors Api
    //---------------------------------------------------------------------------------------------
    [DllImport("svrplugin")]
    private static extern string SvrAnchorToString(ref AnchorUuid id);

    [DllImport("svrplugin")]
    private static extern bool SvrAnchorCreate(float x, float y, float z, float qx, float qy, float qz, float qw, ref AnchorUuid id);

    [DllImport("svrplugin")]
    private static extern bool SvrAnchorDestroy(AnchorUuid id);

    [DllImport("svrplugin")]
    private static extern bool SvrAnchorSave(AnchorUuid id, string fmapFolder);

    [DllImport("svrplugin")]
    private static extern int SvrAnchorGetData();

    [DllImport("svrplugin")]
    private static extern bool SvrAnchorGetDataElement(int index, ref AnchorUuid id, ref UInt32 revision,
        ref float x, ref float y, ref float z, ref float qx, ref float qy, ref float qz, ref float qw, ref float quality);

    [DllImport("svrplugin")]
    private static extern bool SvrAnchorRelocatingStart(string fmapFolder);

    [DllImport("svrplugin")]
    private static extern bool SvrAnchorRelocatingStop();
    //---------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------
    // PointCloud Api
    //---------------------------------------------------------------------------------------------
    [DllImport("svrplugin")]
    private static extern int SvrCloudGetData();

    [DllImport("svrplugin")]
    private static extern bool SvrCloudGetDataElement(int index, ref UInt32 id, ref float x, ref float y, ref float z);
    //---------------------------------------------------------------------------------------------
    #endregion
}
