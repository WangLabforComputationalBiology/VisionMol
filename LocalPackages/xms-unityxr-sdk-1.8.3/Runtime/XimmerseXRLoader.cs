namespace Ximmerse.XR
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.XR;
    using UnityEngine.XR.Management;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Reflection;
    using UnityEngine.XR.ARSubsystems;
    using System.IO;
    using Object = UnityEngine.Object;
    using UnityEngine.SceneManagement;
    using UnityEditor;
    using Ximmerse.XR.Tag;
    using System.Collections;
    using Ximmerse.XR.InputSystems;

    /// <summary>
    /// XR Loader for Cardboard XR Plugin.
    /// Loads Display and Input Subsystems.
    /// </summary>
    [DefaultExecutionOrder(-20000)]
    public class XimmerseXRLoader : XRLoaderHelper
    {
        static NativePluginApi.DeviceType sDeviceType = NativePluginApi.DeviceType.Device_AUTO;

        private static List<XRDisplaySubsystemDescriptor> _displaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();

        private static List<XRInputSubsystemDescriptor> _inputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();

        //private static List<MRTKSubsystemDescriptor<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>> _mrtkSubSysDescriptors = new List<MRTKSubsystemDescriptor<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>>();

        public XimmerseXRSettings settings;

        public static System.Action<XimmerseXRLoader> eventOnInit, eventOnStart, eventOnStop, eventOnDestroy;

        internal static bool _isInitialized { get; private set; }

        internal static bool _isStarted { get; private set; }

        private const string kXimHMD = "Xim_HMD";
        private const string kLeftController = "LeftHand";
        private const string kRightController = "RightHand";

        private const string unityxrsdkVersion = "v1.8.3";

        private void Awake()
        {
            if (settings==null)
            {
                settings = new XimmerseXRSettings();
            }
#if !UNITY_EDITOR
            NativePluginApi.Unity_requiresRGBEyeBuffer(GraphicsSettings.currentRenderPipeline != null); //require sRGB supports in URP pipeline.
#endif
            SetSDKVars();//set sdk vars at main thread
            if (!SDKVariants.IsSupported)
            {
                return;
            }
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            Debug.Log("Unity XR SDK Version : " + unityxrsdkVersion);

            //int _reticleTextureId = settings.displayReticle && settings.reticleTexture ? settings.reticleTexture.GetNativeTexturePtr().ToInt32() : -1;
            //int _reticleTexW = settings.displayReticle && settings.reticleTexture ? settings.reticleTexture.width : -1;
            //int _reticleTexH = settings.displayReticle && settings.reticleTexture ? settings.reticleTexture.height : -1;


            ////Async op at daemon thread. 
            //Task.Run(async () =>
            //{
            //    Debug.Log("XR initialization thread starts...");
            //    try
            //    {
            //        while (!SvrPluginAndroid.SvrIsRunning())
            //        {
            //            Thread.Sleep(10);
            //        }
            //        //Set reticle texture id:
            //        Debug.Log("Daemon threads detect SVR system is started.");
            //        if (settings.reticleTexture && settings.displayReticle)
            //        {
            //            XimmerseXR.DisplayReticle = true;
            //            XimmerseXR.SetReticleTexture(_reticleTextureId, _reticleTexW, _reticleTexH);
            //        }
            //        else
            //        {
            //            XimmerseXR.DisplayReticle = false;
            //        }
            //        //Loads default tracking profile:
            //        try
            //        {
            //            if (!ReferenceEquals(null, (System.Object)settings.defaultTrackingProfile))
            //            {
            //                //Load tracking profile : 
            //                XimmerseXR.LoadTrackingProfile(this.settings.defaultTrackingProfile.trackingItems);
            //            }
            //        }
            //        catch (System.Exception exc)
            //        {
            //            Debug.LogError("Default tracking profile not loaded.");
            //            Debug.LogException(exc);
            //        }
            //        Debug.Log("Reticle set successfully.");
            //        await Task.Delay(10);//delay a bit to start fusion:
            //        if (SDKVariants.groundPlaneLayout != null && SDKVariants.groundPlaneLayout.IsValid())
            //        {
            //            XimmerseXR.LoadGroundPlaneLayout(SDKVariants.groundPlaneLayout);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Debug.LogErrorFormat("XR initialization exception: {0}, {1}", e.Message, e.StackTrace, e);
            //        Debug.LogException(e);
            //    }
            //});
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            //Create XR manager:
            var xrMgr = new GameObject("XR Manager", typeof(XRManager));
            Object.DontDestroyOnLoad(xrMgr.gameObject);

            xrMgr.GetComponent<XRManager>().StartCoroutine(initializeDeivces());
            //if (this.settings.HandTracking)
            //{
            //    Ximmerse.XR.InputSystems.HandTracking.EnableHandTracking();
            //}

            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

        }

        /// <summary>
        /// Init XR devices : HMD , left and right controller.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator initializeDeivces()
        {
            bool hmdDeviceEnabled = false;
            bool leftHandEnabled = false;
            bool rightHandEnabled = false;
            UnityEngine.InputSystem.InputSystem.onDeviceChange -= OnDevicesChanged;
            UnityEngine.InputSystem.InputSystem.onDeviceChange += OnDevicesChanged;
            while (true)
            {
                if (hmdDeviceEnabled && leftHandEnabled && rightHandEnabled)
                {
                    yield break;
                }
                for (int i = 0; i < UnityEngine.InputSystem.InputSystem.devices.Count; i++)
                {
                    UnityEngine.InputSystem.InputDevice device = UnityEngine.InputSystem.InputSystem.devices[i];
                    if (!hmdDeviceEnabled && device.name == kXimHMD && !device.enabled)
                    {
                        UnityEngine.InputSystem.InputSystem.EnableDevice(device);
                        hmdDeviceEnabled = true;
                        Debug.Log("Ximmerse XR Loader : Enable Xim_HMD.");
                    }
                    if (!leftHandEnabled && device.name == kLeftController && !device.enabled)
                    {
                        UnityEngine.InputSystem.InputSystem.EnableDevice(device);
                        leftHandEnabled = true;
                        Debug.Log("Ximmerse XR Loader : Enable Left Controller.");
                    }
                    if (!rightHandEnabled && device.name == kRightController && !device.enabled)
                    {
                        UnityEngine.InputSystem.InputSystem.EnableDevice(device);
                        rightHandEnabled = true;
                        Debug.Log("Ximmerse XR Loader : Enable Right Controller.");
                    }
                    if(device.enabled == false && (device is HandAnchorInputDevice || device is XimmerseXRHandInput || device is XimmerseXRGazeInput || device is HandGunInputDevice || device is DualHandGestureInputDevice || device is LongGunInputDevice))
                    {
                        UnityEngine.InputSystem.InputSystem.EnableDevice(device);
                    }
                }

                yield return null;
            }


#if DEVELOPMENT_BUILD
            for (int i = 0; i < UnityEngine.InputSystem.InputSystem.devices.Count; i++)
            {
                UnityEngine.InputSystem.InputDevice device = UnityEngine.InputSystem.InputSystem.devices[i];
                System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                if (device.name == kLeftController || device.name == kRightController || device.name == kXimHMD)
                {
                    var controls = device.allControls;
                    for (int j = 0; j < controls.Count; j++)
                    {
                        buffer.Clear();
                        UnityEngine.InputSystem.InputControl ctrl = controls[j];
                        foreach (var usage in ctrl.usages)
                        {
                            buffer.Append(usage.ToString() + ' ');
                        }
                        Debug.LogFormat("Xim device layout, name = {0}, path = {1}, value type = {2}, usage = {3}, device type = {4}", ctrl.name, ctrl.path, ctrl.valueType, buffer.ToString(), device.GetType());
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Enables devices after reconnection.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void OnDevicesChanged(UnityEngine.InputSystem.InputDevice arg1, UnityEngine.InputSystem.InputDeviceChange arg2)
        {
            if (arg2 == UnityEngine.InputSystem.InputDeviceChange.Reconnected && arg1.name == kLeftController && !arg1.enabled)
            {
                UnityEngine.InputSystem.InputSystem.EnableDevice(arg1);
            }
            if (arg2 == UnityEngine.InputSystem.InputDeviceChange.Reconnected && arg1.name == kRightController && !arg1.enabled)
            {
                UnityEngine.InputSystem.InputSystem.EnableDevice(arg1);
            }
        }

        public override bool Initialize()
        {
            SDKInitialize();
            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(_displaySubsystemDescriptors, "XmsXRDisplay");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(_inputSubsystemDescriptors, "XmsXRInput");

            //SubsystemManager.GetSubsystemDescriptors<MRTKSubsystemDescriptor<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>>(_mrtkSubSysDescriptors);

            //Debug.LogFormat("XimmerseMRTKHandsAggregatorSubSystem descripitor.Count == {0}", _mrtkSubSysDescriptors.Count);

            //_mrtkSubSysDescriptors.Clear();

            //CreateSubsystem<MRTKSubsystemDescriptor<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>, XimmerseMRTKHandsAggregatorSubSystem>(_mrtkSubSysDescriptors, "com.ximmerse.xr.hands");

            //Debug.LogFormat("XRLoader.Initialize, settings: {0}, SubSystems has been created.", settings.name);
#if UNITY_INPUT_SYSTEM
            Ximmerse.XR.InputSystems.InputLayout.RegisterInputLayouts();
#endif

            //UnityEngine.InputSystem.InputSystem.onDeviceChange += (device, changeEvent) =>
            //{
            //    Debug.LogFormat("Control inputSystem device event : {0}, device : {1}, {2}, {3}, {4}", changeEvent, device.name, device.displayName, device.deviceId, device);
            //};
            _isInitialized = true;

            eventOnInit?.Invoke(this);

            return true;
        }

        public override bool Start()
        {
            Debug.Log("[XRLoader] ==>Start");
            if (Application.platform == RuntimePlatform.Android)
            {
                if (sDeviceType == NativePluginApi.DeviceType.Device_AUTO)
                {
                    SvrPlugin.deviceModel = SvrPlugin.DeviceModel.Default;
                    int trackingMode = (int)SvrPlugin.TrackingMode.kTrackingOrientation | (int)SvrPlugin.TrackingMode.kTrackingPosition;
                    NativePluginApi.Unity_setTrackingMode(trackingMode);
                    NativePluginApi.Unity_setVsync(1);
                    //SvrPlugin.Instance.SetTrackingMode(trackingMode);
                    //SvrPlugin.Instance.SetVSyncCount(1);
                    SvrPlugin.Instance.BeginVr(3, 3, 0x0);
                    NativePluginApi.Unity_setBeginRecenter(0.0f,true);
                }
            }

            StartSubsystem<XRDisplaySubsystem>();
            StartSubsystem<XRInputSubsystem>();
           // StartSubsystem<XimmerseMRTKHandsAggregatorSubSystem>();
            // StartSubsystem<RhinoXGroundPlaneSubSystem>();

            _isStarted = true;
            eventOnStart?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Creates a subsystem
        /// </summary>
        /// <typeparam name="TDescriptor"></typeparam>
        /// <typeparam name="TSubsystem"></typeparam>
        /// <param name="descriptors"></param>
        /// <param name="id"></param>
        public void CreatesSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id)
    where TDescriptor : ISubsystemDescriptor
    where TSubsystem : ISubsystem
        {
            CreateSubsystem<TDescriptor, TSubsystem>(descriptors, id);
        }

        /// <summary>
        /// Starts a sub system
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void StartsSubSystem<T> () where T : class, ISubsystem
        {
            StartSubsystem<T>();
        }

        /// <summary>
        /// Stops a subsystem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void StopsSubsystem<T>() where T : class, ISubsystem
        {
            StopSubsystem<T>();
        }

        /// <summary>
        /// Destroys a subsystem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void DestroysSubSystem<T>() where T : class, ISubsystem
        {
            DestroySubsystem<T>();
        }


        public override bool Stop()
        {
            Debug.Log("[XRLoader] ==>Stop");
            StopSubsystem<XRDisplaySubsystem>();
            StopSubsystem<XRInputSubsystem>();
           // StopSubsystem<XimmerseMRTKHandsAggregatorSubSystem>();
            //  StopSubsystem<RhinoXGroundPlaneSubSystem>();

            _isStarted = false;
            eventOnStop?.Invoke(this);
            return true;
        }

        public override bool Deinitialize()
        {
            Debug.Log("[XRLoader] ==>Deinitialize");
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRInputSubsystem>();
          //  DestroySubsystem<XimmerseMRTKHandsAggregatorSubSystem>();
            // DestroySubsystem<RhinoXGroundPlaneSubSystem>();

            _isInitialized = false;
            eventOnDestroy?.Invoke(this);
            return true;
        }

        private void SDKInitialize()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                NativePluginApi.Unity_setDeviceType((int)sDeviceType);

                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                var context = activity.Call<AndroidJavaObject>("getApplicationContext");
               //NativePluginApi.Unity_setRenderResolution(2160, 2160);
                NativePluginApi.Unity_initializeAndroid(activity.GetRawObject());

                switch (SystemInfo.graphicsDeviceType)
                {
                    case GraphicsDeviceType.OpenGLES2:
                        NativePluginApi.Unity_setGraphicsApi(NativePluginApi.GraphicsApi.kOpenGlEs2);
                        break;
                    case GraphicsDeviceType.OpenGLES3:
                        NativePluginApi.Unity_setGraphicsApi(NativePluginApi.GraphicsApi.kOpenGlEs3);
                        break;
                    default:
                        Debug.LogErrorFormat(
                          "The Ximmerse XR Plugin cannot be initialized given that the selected " +
                          "Graphics API ({0}) is not supported. Please use OpenGL ES 2.0, " +
                          "OpenGL ES 3.0 or Metal.", SystemInfo.graphicsDeviceType);
                        break;
                }

                NativePluginApi.Unity_setScreenParams((int)Screen.width, (int)Screen.height, (int)Screen.safeArea.x, (int)Screen.safeArea.y, (int)Screen.safeArea.width, (int)Screen.safeArea.height);
                //NativePluginApi.Unity_setRenderingMode(settings.SinglePassRendering ? 1 : 0);//multi pass = 0 ; single pass = 1
                return;
            }
        }


        private void SetSDKVars()
        {
            SDKVariants.kTrackingDataDir_Internal = Application.persistentDataPath;

            SDKVariants.IsSupported = Application.platform == RuntimePlatform.Android; //todo : verify on android phone

            //SDKVariants.groundPlaneLayout = settings.defaultGroundPlaneLayoutConfig ? settings.defaultGroundPlaneLayoutConfig.layout : default(GroundPlaneLayout);

            //SDKVariants.TrackingAnchor = Matrix4x4.TRS(SDKVariants.kVPU_Shift, Quaternion.Euler(SDKVariants.kVPU_TiltEuler), Vector3.one);

            //SDKVariants.DrawTrackedMarkerGizmos = settings.DrawTrackedMarkerGizmos;

            //SDKVariants.DrawDetailTrackedInfo = settings.DrawDetailTrackedInfo;
        }
    }
}
