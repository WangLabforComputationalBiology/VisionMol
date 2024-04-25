using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using Ximmerse.XR.InputSystems;
using Ximmerse.XR.Tag;

namespace Ximmerse.XR
{
    public class XMR_Manager : MonoBehaviour
    {
        private const string TAG = "XMR_Manager";

        public const int UpCameraFPS = 15;

        public enum FoveatedMode { None = 0, Low, Medium, High, TopHigh };

        public enum ASWModeENUM { None = 0, ASW, APPSW };

        public enum WarpVertexENUM { None = 0, Low, High };

        public enum Config { Default,Custom};

        #region Global Configuration
        [Header("Hand tracking related settings")]
        [Tooltip("Whether to enable hand tracking")]
        [SerializeField]
        private bool handTracking;

        //[Tooltip("Whether to need to customize the hand tracking configuration")]
        //[SerializeField]
        //private Config HandTrackingConfigType = Config.Default;

        [Tooltip("Choose how to turn on hand tracking")]
        [SerializeField]
        private HandTrackDataSource handTrackDataSource;


        [Header("MRC related settings")]
        [Tooltip("Whether to enable MRC")]

        [SerializeField]
        bool MRCFirstPersonRealityCamera = false;

        [SerializeField]
        bool MRCThirdPersonCamera = false;

        [SerializeField]
        Transform MRCThirdPersonObject = null;

        [Header("Body tracking related settings")]
        [HideInInspector]
        private bool bodyTracking;


        [Header("Renderer related settings")]
        //[SerializeField]
        //private bool displayOptimization = true;

        //[Tooltip("Whether to need to customize the display configuration")]
        //[SerializeField]
        //private Config displayOptimizationConfigType = Config.Default;


        [SerializeField]
        public FoveatedMode foveatedLevel = FoveatedMode.None;

        [SerializeField]
        public bool useSubsample = false;

        [SerializeField]
        public ASWModeENUM ASWMode = ASWModeENUM.None;

        [SerializeField]
        public bool PTWMode = true;

        [SerializeField]
        public bool DynamicResolutionMode = false;

        [SerializeField]
        public WarpVertexENUM WarpVertexMode = WarpVertexENUM.None;

        [Header("Positioning and tracking related settings")]
        [SerializeField]
        [Tooltip("Whether to enable Tag recognition")]
        private bool tagRecognition = true;
        [SerializeField]
        [Tooltip("Is the upper view camera positioning function enabled")]
        private bool upCameraTracking = false;


        AdditionBlit depthBlit;

        public bool TagRecognition
        {
            get => tagRecognition;
        }

        #endregion

        #region Instance
        private static XMR_Manager instance = null;
        [HideInInspector]
        private Camera[] eyeCamera;
        [HideInInspector]
        private DepthTextureMode lastDepthTextureMode;
        public static XMR_Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<XMR_Manager>();
                    if (instance == null)
                    {
                        Debug.LogError("XMR_Manager instance is not initialized!");
                    }
                }
                return instance;
            }
        }
        #endregion


        #region API
        public void SetFFR(FoveatedMode mode)
        {
            Debug.Log("SetFFR mode:" + mode);

            if (mode==FoveatedMode.None)
            {
#if !UNITY_EDITOR
            NativePluginApi.Unity_setFFR((int)mode, false);
#endif
            }
            else
            {
#if !UNITY_EDITOR
            NativePluginApi.Unity_setFFR((int)mode, useSubsample);
#endif
            }

            foveatedLevel = mode;
        }

        public void SetASW(ASWModeENUM mode)
        {
            Debug.Log("SetASW mode:" + mode);
#if !UNITY_EDITOR
            NativePluginApi.Unity_setAppSW((int)mode);
#endif
            ASWMode = mode;
        }

        public void SetPTW(bool enabled)
        {
            if (enabled)
            {
                depthBlit = FindObjectOfType<AdditionBlit>();
                if (depthBlit == null)
                {
                    depthBlit = new GameObject("AdditionBlit").AddComponent<AdditionBlit>();
                }
            }
            Debug.Log("SetPTW status:" + enabled);

            for (int i = 0; i < 3; i++)
            {
                if (eyeCamera[i] != null && eyeCamera[i].enabled)
                {
                    if (enabled)
                    {
                        lastDepthTextureMode = eyeCamera[i].depthTextureMode;
                        eyeCamera[i].depthTextureMode |= DepthTextureMode.Depth;


                    }
                    else
                    {
                        eyeCamera[i].depthTextureMode = lastDepthTextureMode;
                    }
                }
            }
#if !UNITY_EDITOR
            NativePluginApi.Unity_setPTW(enabled);
#endif
            PTWMode = enabled;
        }

        public void SetDynamicResolution(bool enable) {
#if !UNITY_EDITOR
            NativePluginApi.Unity_setDynamicResolution(enable);
#endif
        }

        public void SetMRCThirdPerson(bool enabled)
        {
            if (enabled)
            {
                if (GraphicsSettings.renderPipelineAsset != null)
                {
                    RenderPipelineManager.beginFrameRendering += MRCAPI.BeginRendering;
                    RenderPipelineManager.endFrameRendering += MRCAPI.EndRendering;
                }
                else
                {
                    Camera.onPreRender += MRCAPI.OnPreRenderCallBack;
                    Camera.onPostRender += MRCAPI.OnPostRenderCallBack;
                }

                MRCAPI.CreateMRCCam(this.transform);

                if (MRCThirdPersonObject != null)
                {
                    MRCAPI.CalibrationMRCCam(MRCThirdPersonObject.transform.position.x,
                                        MRCThirdPersonObject.transform.position.y,
                                        MRCThirdPersonObject.transform.position.z,
                                        MRCThirdPersonObject.transform.rotation.x,
                                        MRCThirdPersonObject.transform.rotation.y,
                                        MRCThirdPersonObject.transform.rotation.z,
                                        MRCThirdPersonObject.transform.rotation.w);
                }
            }

        }

        public void SetWarpVertex(int mode)
        {
            Debug.Log("SetWarpVertex mode:" + mode);
#if !UNITY_EDITOR
            NativePluginApi.Unity_setWarpVertex(mode);
#endif
        }

        private void SetUpCamera(bool enable)
        {
#if !UNITY_EDITOR
            XDevicePlugin.xdevc_vpu_set_up_camera_config(XDevicePlugin.xdevc_get_vpu(), enable, XMR_Manager.UpCameraFPS);
#endif
        }

        public void SetMRCFirstPersonCamera(bool enable) {
#if !UNITY_EDITOR
            Debug.Log("Set MRC first person camera:" + enable);
            NativePluginApi.Unity_setMRCRealityCam(enable);
#endif
        }

        private void Init()
        {

            GetCamera();

            SetPTW(PTWMode);

            SetWarpVertex((int)(WarpVertexMode));

            SetFFR(foveatedLevel);

            SetASW(ASWMode);

            //SetDynamicResolution(DynamicResolutionMode);

            SetMRCFirstPersonCamera(MRCFirstPersonRealityCamera);

            SetMRCThirdPerson(MRCThirdPersonCamera);


            if (bodyTracking)
            {
                MotionCapture.Init();
            }

            SetHandTracking(handTracking);

            EnablePositioningAndTracking(tagRecognition);

        }

        public void EnablePositioningAndTracking(bool enable)
        {
            if (enable)
            {
                SetUpCamera(upCameraTracking);
                if (TagProfileLoading.Instance!=null)
                {
                    TagProfileLoading.Instance.EnablePositioningAndTracking();
                }
            }
            else
            {
                SetUpCamera(false);
                if (TagProfileLoading.Instance != null)
                    TagProfileLoading.Instance.CleanAllMarker();
            }
            Debug.Log("EnablePositioningAndTracking " + enable);
        }


        public void SetHandTracking(bool enable)
        {
            Debug.Log("SetHandTracking " + enable);
            if (enable)
            {
                HandTrackingT3D.UseHandTrackService = handTrackDataSource == HandTrackDataSource.SystemService;

                HandTracking.EnableHandTracking();
            }
            else
            {
                HandTracking.DisableHandTracking();
            }
        }
#endregion

#region LifeCycle
        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            Init();
#if !UNITY_EDITOR
            NativePluginApi.Unity_Recenter(0.0f);
#endif
        }
        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (upCameraTracking)
                {
                    SetUpCamera(upCameraTracking);
                }
            }
            else
            {
                SetUpCamera(false);
            }
        }


        private void OnDestroy()
        {
            if (bodyTracking)
            {
                MotionCapture.Exit();
            }

            SetHandTracking(false);

            EnablePositioningAndTracking(false);

            SetPTW(false);

            SetWarpVertex(0);

            SetFFR(FoveatedMode.None);

            SetASW(ASWModeENUM.None);

            SetMRCThirdPerson(false);
        }
#endregion
        
#region other
        void GetCamera()
        {

            eyeCamera = new Camera[3];
            Camera[] cam = gameObject.GetComponentsInChildren<Camera>();
            for (int i = 0; i < cam.Length; i++)
            {
                if (cam[i].stereoTargetEye == StereoTargetEyeMask.Both && cam[i] == Camera.main)
                {
                    eyeCamera[0] = cam[i];
                }
                else if (cam[i].stereoTargetEye == StereoTargetEyeMask.Left)
                {
                    eyeCamera[1] = cam[i];
                }
                else if (cam[i].stereoTargetEye == StereoTargetEyeMask.Right)
                {
                    eyeCamera[2] = cam[i];
                }
            }

        }
#endregion

    }
}
    
