using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public abstract class SvrPlugin
{
	private static SvrPlugin instance = null;

    public enum DeviceModel
    {
        Default = 0x100,
        RhinoXPro = 0x200,
        HMD20 = 0x300,
        RhinoXH = 0x400
    };

    public static DeviceModel deviceModel = DeviceModel.Default;

    public static SvrPlugin Instance
	{
		get
		{
			if (instance == null)
			{
				if(!Application.isEditor && Application.platform == RuntimePlatform.Android)
				{
                    //deviceModel = SvrManager.Instance.settings.deviceModel;

                    switch (deviceModel) {

                        case DeviceModel.Default:
                        case DeviceModel.RhinoXPro:
                        case DeviceModel.HMD20:
                        {
                                instance = SvrPluginAndroid.Create();
                            }
                            break;
                        case DeviceModel.RhinoXH:
                            {
                              //  instance = SvrPluginHiXR.Create();
                            }
                            break;
                        default:break;
                        
                    }
                   
				}
				else
				{
					//instance = SvrPluginWin.Create();
				}
			}
			return instance;
		}
	}

    public SvrManager svrCamera = null;

    public DeviceInfo deviceInfo;
    public CommandBuffer beginEyeCommandBuffer = null;

    public enum EyeMask
    {
        kLeft = 0x00000001,
        kRight = 0x00000002,
        kBoth = 0x00000003,
    };

    public enum TextureType
    {
        kTypeTexture = 0,               //!< Standard texture
        kTypeTextureArray,              //!< Standard texture array (Left eye is first layer, right eye is second layer)
        kTypeImage,                     //!< EGL Image texture
        kTypeEquiRectTexture,           //!< Equirectangular texture
        kTypeEquiRectImage,             //!< Equirectangular Image texture
        kTypeCubemapTexture,            //!< Cubemap texture (Not supporting cubemap image)
        kTypeVulkan,                    //!< Vulkan texture
        kTypeCamera,                    //!< Video camera frame texture
    };

    public enum LayerFlags
    {
        kLayerFlagNone = 0x00000000,
        kLayerFlagHeadLocked = 0x00000001,
        kLayerFlagOpaque = 0x00000002,
        kLayerFlagSubsampled = 0x00000004,
    };
    public enum PerfLevel
	{
        kPerfSystem = 0,
        kPerfMaximum = 1,
		kPerfNormal = 2,
		kPerfMinimum = 3
	}

    public enum TrackingMode
    {
        kTrackingOrientation = (1 << 0),
        kTrackingPosition = (1 << 1),
        kTrackingEye = (1 << 2),
    }

    public enum EyePoseStatus
    {
        kGazePointValid = (1 << 0),
        kGazeVectorValid = (1 << 1),
        kEyeOpennessValid = (1 << 2),
        kEyePupilDilationValid = (1 << 3),
        kEyePositionGuideValid = (1 << 4),
        kEyeBlinkValid = (1 << 5),
    };

    public enum ServiceCapabilities
    {
        kCapabilityCombinedGaze = 0x00000001,
        kCapabilityConvergenceDistance = 0x00000002,
        kCapabilityFoveatedGaze = 0x00000004,
        kCapabilityPerEyeGazeOrigin = 0x00000008,
        kCapabilityPerEyeGazeDirection = 0x00000010,
        kCapabilityPerEyeGazePoint = 0x00000020,
        kCapabilityPerEyeGazeOpenness = 0x00000040,
        kCapabilityPerEyePupilDilation = 0x00000080,
        kCapabilityPerEyePositionGuide = 0x00000100,
        kCapabilityPerEyeBlink = 0x00000200,
    };

    public enum FrameOption
    {
        kDisableDistortionCorrection = (1 << 0),    //!< Disables the lens distortion correction (useful for debugging)
        kDisableReprojection = (1 << 1),            //!< Disables re-projection
        kEnableMotionToPhoton = (1 << 2),           //!< Enables motion to photon testing 
        kDisableChromaticCorrection = (1 << 3)      //!< Disables the lens chromatic aberration correction (performance optimization)
    };

    public struct HeadPose
    {
        public UInt64 timestamp;
        public Vector3 position;
        public Quaternion orientation;
    }

    public struct EyePose
    {
        public UInt64 timestamp;            //!< Eye pose timestamp

        public int leftStatus;              //!< Bit field (svrEyePoseStatus) indicating left eye pose status
        public int rightStatus;             //!< Bit field (svrEyePoseStatus) indicating right eye pose status
        public int combinedStatus;          //!< Bit field (svrEyePoseStatus) indicating combined eye pose status

        public Vector3 leftPosition;        //!< Left Eye Gaze Point
        public Vector3 rightPosition;       //!< Right Eye Gaze Point
        public Vector3 combinedPosition;    //!< Combined Eye Gaze Point (HMD center-eye point)

        public Vector3 leftDirection;       //!< Left Eye Gaze Point
        public Vector3 rightDirection;      //!< Right Eye Gaze Point
        public Vector3 combinedDirection;   //!< Comnbined Eye Gaze Vector (HMD center-eye point)

        public bool leftBlink;              //!< Left eye value indicating lid up or down.
        public bool rightBlink;             //!< Right eye value indicating lid up or down.

        public float leftOpenness;          //!< Left eye value between 0.0 and 1.0 where 1.0 means fully open and 0.0 closed.
        public float rightOpenness;         //!< Right eye value between 0.0 and 1.0 where 1.0 means fully open and 0.0 closed.

        public float leftDilation;          //!< Left eye value in millimeters indicating the pupil dilation
        public float rightDilation;         //!< Right eye value in millimeters indicating the pupil dilation

        public Vector3 leftGuide;           //!< Position of the inner corner of the left eye in meters from the HMD center-eye coordinate system's origin.
        public Vector3 rightGuide;          //!< Position of the inner corner of the right eye in meters from the HMD center-eye coordinate system's origin.
    }

    public struct ViewFrustum
    {
        public float left;           //!< Left Plane of Frustum
        public float right;          //!< Right Plane of Frustum
        public float top;            //!< Top Plane of Frustum
        public float bottom;         //!< Bottom Plane of Frustum

        public float near;           //!< Near Plane of Frustum
        public float far;            //!< Far Plane of Frustum (Arbitrary)

        public Vector3 position;    //!< Position Offset of Frustum
        public Quaternion rotation; //!< Rotation Quaternion of Frustum
    }

    public struct Foveation
    {
        public Vector2 Gain;         //!< Foveation Gain Rate [1, ...]
        public float Area;           //!< Foveation Area Size [0, ...]
        public float Minimum;        //!< Foveation Minimum Resolution [1, 1/2, 1/4, ..., 1/16, 0]
    }

    public struct CameraIntrinsics
    {
        public Vector2 PrincipalPoint;
        public Vector2 FocalLength;
        public float Distortion0;
        public float Distortion1;
        public float Distortion2;
        public float Distortion3;
        public float Distortion4;
        public float Distortion5;
        public float Distortion6;
        public float Distortion7;
    }

    public struct CloudPoint
    {
        public float x;
        public float y;
        public float z;
        public UInt32 id;
    }

    public struct AnchorUuid
    {
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] uuid;

        public override string ToString() { return SvrPlugin.Instance.AnchorToString(this); }
    }

    public struct AnchorPose
    {
        public Quaternion orientation;
        public Vector3 position;
        public float poseQuality;
    }

    public struct AnchorInfo
    {
        public AnchorUuid id;
        public UInt32 revision;
        public AnchorPose pose;
    }

    public struct DeviceInfo
	{
		public int 		displayWidthPixels;
		public int    	displayHeightPixels;
		public float  	displayRefreshRateHz;
		public int    	targetEyeWidthPixels;
		public int    	targetEyeHeightPixels;
		public float  	targetFovXRad;
		public float  	targetFovYRad;
        public ViewFrustum targetFrustumLeft;
        public ViewFrustum targetFrustumRight;
        public float    targetFrustumConvergence;
        public float    targetFrustumPitch;
        public Foveation lowFoveation;
        public Foveation medFoveation;
        public Foveation highFoveation;
        public Matrix4x4 trackingCalibration;
        public CameraIntrinsics trackingIntrinsics;
        public UInt64 trackingCapabilities; //ServiceCapabilities
    }

   // public virtual bool PollEvent(ref SvrManager.SvrEvent frameEvent) { return false; }

    public virtual bool IsInitialized() { return false; }
    public virtual bool IsRunning() { return false; }
    public virtual IEnumerator Initialize ()
    {
        svrCamera = SvrManager.Instance;
        if (svrCamera == null)
        {
            Debug.LogError("SvrManager object not found!");
            yield break;
        }

        yield break;
    }
	public virtual void BeginVr(int cpuPerfLevel, int gpuPerfLevel, int optionFlags)
    {
      //  yield break;
    }
    public virtual void EndVr()
    {
       
    }
    public virtual void PauseXr() { }
    public virtual void ResumeXr() { }
	public virtual void BeginEye(int renderIndex, int sideMask, float[] frameDelta) { }
    public virtual void OccludeEye(int renderIndex, Matrix4x4 proj, Matrix4x4 view) { }
    public virtual void EndEye(int renderIndex, int sideMask, int layerMask) { }
    public virtual void SetTrackingMode(int mode) { }
	public virtual void SetFoveationParameters(int renderIndex, int textureId, int previousId, float focalPointX, float focalPointY, float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum) {}
    public virtual void ApplyFoveation() { }
    public virtual int  GetTrackingMode() { return 0; }
    public virtual void SetPerformanceLevels(int newCpuPerfLevel, int newGpuPerfLevel) { }
    public virtual void SetFrameOption(FrameOption frameOption) { }
    public virtual void UnsetFrameOption(FrameOption frameOption) { }
    public virtual void SetVSyncCount(int vSyncCount) { }
    public virtual bool RecenterTracking() { return true; }
    public virtual void SubmitFrame(int frameIndex, float fieldOfView, int frameType) { }
    public virtual int GetPredictedPose(ref Quaternion orientation, ref Vector3 position, int frameIndex = -1)
    {
        orientation = Quaternion.identity;
        position = Vector3.zero;
        return 0;
    }
    public virtual int GetHeadPose(ref HeadPose headPose, int frameIndex = -1)
    {
        headPose.timestamp = 0;
        headPose.orientation = Quaternion.identity;
        headPose.position = Vector3.zero;
        return 0;
    }
    public virtual int GetEyePose(ref EyePose eyePose, int frameIndex = -1)
    {
        eyePose.leftStatus = 0;
        eyePose.rightStatus = 0;
        eyePose.combinedStatus = 0;
        return 0;
    }
    public virtual int GetEyeFocalPoint(ref Vector2 focalPoint)
    {
        focalPoint = Vector2.zero;
        return 0;
    }
    public virtual bool Is3drOcclusion()
    {
        return false;
    }
    public virtual void GetOcclusionMesh()
    {
    }
    public abstract DeviceInfo GetDeviceInfo ();
	public virtual void Shutdown()
    {
        SvrPlugin.instance = null;
    }

	//---------------------------------------------------------------------------------------------
	public virtual int ControllerStartTracking(string desc) {
		return -1;
	}

	//---------------------------------------------------------------------------------------------
	public virtual void ControllerStopTracking(int handle) {
	}



    public virtual int CloudPointData(CloudPoint[] points) { return 0; }

    public virtual string AnchorToString(AnchorUuid id) { return string.Empty; }
    public virtual bool AnchorCreate(Vector3 position, Quaternion rotation, ref AnchorUuid id) { return true; }
    public virtual bool AnchorDestroy(AnchorUuid id) { return true; }
    public virtual bool AnchorSave(AnchorUuid id) { return true; }
    public virtual int AnchorGetData(AnchorInfo[] anchorData) { return 0; }
    public virtual bool AnchorStartRelocating() { return true; }
    public virtual bool AnchorStopRelocating() { return true; }

}

