using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ximmerse.XR.Internal;
using static Ximmerse.XR.PluginVioFusion;

namespace Ximmerse.XR.Tag
{
    public partial class TagLoadingManager : MonoBehaviour
    {
        #region Property
        protected Thread threadLoad;
        //private List<GameObject> _groundplanelist = new List<GameObject>();
        protected List<TagGroundPlane> _tagGroundbyJson = new List<TagGroundPlane>();
        protected List<int> _trackingtaglist = new List<int>();

        [Header("Offset")]
        protected Vector3 offsetPos = new Vector3();
        protected Vector3 offsetRot = new Vector3();
        

        int beacon_id = -1;
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

        protected int _tagfusion;
        private long predTimestampNano = 0;
        int index = 0;
        long timestamp = 0;
        int state = 0;
        float posX = 0;
        float posY = 0;
        float posZ = 0;
        float rotX = 0;
        float rotY = 0;
        float rotZ = 0;
        float rotW = 0;
        float confidence = 0;
        float marker_distance = 0;
        bool trakingstate;

        private XROrigin xr;
        //private XDevicePlugin.XAttrTrackingInfo trackingInfo;

        private Vector3 xroffsetpos;
        private Quaternion xroffsetrot;

        private Vector3 cameraOffsetRot = new Vector3(0, 0, 0);

        Transform xrOffsetTransform;

        public Transform XrOffsetTransform
        {
            get => xrOffsetTransform;
        }

        private Vector3 beaconFusionPos = new Vector3();
        private Quaternion beaconFusionRot = new Quaternion();

        public Vector3 BeaconFusionPos
        {
            get => beaconFusionPos;
        }

        public Quaternion BeaconFusionRot
        {
            get => beaconFusionRot;
        }

        #endregion

        #region Unity

        private void Update()
        {
            if (XMR_Manager.Instance!=null&&XMR_Manager.Instance.TagRecognition)
            {
                GetTrackingStateAndData();
                CorrectCameraOffset();
            }
        }
#endregion

        #region Method
        /// <summary>
        /// Get the coordinates of the Tag ground plane in the scene and enable large space positioning
        /// </summary>
        private void GroundPlaneSetting()
        {
#if !UNITY_EDITOR
            PluginVioFusion.plugin_vio_fusion_reset(0);
            isUsingUpCamera = false;
            //TagGroundPlane[] enemies = UnityEngine.Object.FindObjectsOfType<TagGroundPlane>();
            if (TagGroundPlane.tagGroundPlaneList != null)
            {
                if (TagGroundPlane.tagGroundPlaneList.Count!=0)
                {
                    xrOffsetTransform = TagGroundPlane.tagGroundPlaneList[0].transform;

                    foreach (var item in TagGroundPlane.tagGroundPlaneList)
                    {
                        if (!isUsingUpCamera)
                        {
                            angle = Vector3.Angle(item.transform.up, Vector3.up);
                            if (angle >= 120)
                            {
                                isUsingUpCamera = true;
                                Debug.Log("QmQ : There is a positioning board facing downwards in the current scene, so turn on the up camera");
                            }
                        }

                        PluginVioFusion.XAttrBeaconInWorldInfo beacon_in_world_info = item.beacon_info;
                        Debug.Log("id£º" + beacon_in_world_info.beacon_id);
                        PluginVioFusion.plugin_vio_fusion_set_param(ref beacon_in_world_info);
                    }
                    PluginVioFusion.plugin_vio_fusion_run(0);
                }
            }
#endif
        }

        /// <summary>
        /// Start enabling the Large Space component
        /// </summary>
        protected IEnumerator StartFusion()
        {
            GetXRComponent();

            while (threadLoad ==null || threadLoad.ThreadState==ThreadState.Running)
            {
                yield return null;
            }

            if (usageType==UsageType.Custom)
            {
                if (CreatesGroundPlaneByJson.Instance != null&& CreatesGroundPlaneByJson.Instance.autoCreates)
                {
                    CreatesGroundPlaneByJson.Instance.CreateGroundPlanesFromConfig();
                }
            }
            else
            {
                CreateGroundPlane();
            }
            SettingTagData();
        }

        /// <summary>
        /// <summary>
        /// Refresh offsets and get coordinate information for large spatial positioning boards.
        /// </summary>
        protected void RefreshBeacon()
        {
            UpdateTagGroundPlane();
        }

        /// <summary>
        /// Clearing the algorithm data invalidates the large spatial positioning function.
        /// </summary>
        protected void CleanBeacon()
        {
#if !UNITY_EDITOR
            PluginVioFusion.plugin_vio_fusion_reset(0);
#endif
        }

        /// <summary>
        /// Setting Tag Data
        /// </summary>
        protected void SettingTagData()
        {
            GroundPlaneSetting();
            if (isUsingUpCamera)
            {
                SetUpCamera(isUsingUpCamera);
            }
        }

        private void UpdateTagGroundPlane()
        {
            if (usageType == UsageType.Custom)
            {
                if (CreatesGroundPlaneByJson.Instance != null && CreatesGroundPlaneByJson.Instance.autoCreates)
                {
                    CreatesGroundPlaneByJson.Instance.CreateGroundPlanesFromConfig();
                }
            }
            else
            {
                CreateGroundPlane();
            }
            SettingTagData();
        }

        private void GetXRComponent()
        {
            if (xr==null)
            {
                xr = FindObjectOfType<XROrigin>();
            }
        }

        /// <summary>
        /// When the positioning function takes effect, correct the camera offset
        /// </summary>
        private void CorrectCameraOffset()
        {
            if (_tagfusion!=-1&& xr!=null&& xrOffsetTransform!=null)
            {
                if (xr.CameraFloorOffsetObject.transform.position!=(offsetPos+xrOffsetTransform.position) || xr.CameraFloorOffsetObject.transform.rotation!= (xrOffsetTransform.rotation* Quaternion.Euler(offsetRot)))
                {
                    OffsetXr();
                    Debug.Log("fusion && offset");
                }
            }
        }

        /// <summary>
        /// Set the offset
        /// </summary>
        protected void OffsetXr()
        {
            if (xr!=null)
            {
                xr.CameraFloorOffsetObject.transform.position = offsetPos + xrOffsetTransform.position;
                xr.CameraFloorOffsetObject.transform.rotation  = Quaternion.Euler(offsetRot) * xrOffsetTransform.rotation;
                Debug.Log("QmQ XR Offset Pos: "+ xr.CameraFloorOffsetObject.transform.position + " Rot: " + xr.CameraFloorOffsetObject.transform.eulerAngles);
            }
        }

        protected void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (xr!=null)
                {
                    xr.CameraFloorOffsetObject.transform.position = new Vector3(0, xr.CameraYOffset, 0);
                    xr.CameraFloorOffsetObject.transform.eulerAngles = Vector3.zero;
                }
                if (isUsingUpCamera)
                {
                    SetUpCamera(isUsingUpCamera);
                }
            }
        }

        /// <summary>
        /// Get the tracking status and the ID of the fusion
        /// </summary>
        protected void GetTrackingStateAndData()
        {


#if !UNITY_EDITOR
            int count = NativePluginApi.Unity_TagPredict(0);
            _trackingtaglist.Clear();
            for (int i = 0; i < count; i++)
            {
                bool ret2 = NativePluginApi.Unity_GetTagPredict(i,
                         ref index, ref timestamp, ref state,
                         ref posX, ref posY, ref posZ,
                         ref rotX, ref rotY, ref rotZ, ref rotW,
                         ref confidence, ref marker_distance);

                if (ret2)
                {
                    _trackingtaglist.Add(index);
                }
            }
#endif
            _tagfusion = GetTagFusionState();
        }

        /// <summary>
        /// Gets a valid Tag ID
        /// </summary>
        /// <returns></returns>
        protected int GetTagFusionState()
        {
#if !UNITY_EDITOR
            NativePluginApi.Unity_getFusionResult(predTimestampNano, ref beacon_id,
                            ref beacon_timestamp,
                            ref beacon_pos0,
                            ref beacon_pos1,
                            ref  beacon_pos2,
                            ref  beacon_rot0,
                            ref  beacon_rot1,
                            ref  beacon_rot2,
                            ref  beacon_rot3,
                            ref  beacon_tracking_confidence,
                            ref  beacon_min_distance,
                            ref  beacon_correct_weight);
#endif
            if (beacon_id!=-1)
            {
                beaconFusionPos.x = beacon_pos0;
                beaconFusionPos.y = beacon_pos1;
                beaconFusionPos.z = beacon_pos2;
                beaconFusionRot.x = beacon_rot0;
                beaconFusionRot.y = beacon_rot1;
                beaconFusionRot.z = beacon_rot2;
                beaconFusionRot.w = beacon_rot3;
            }

            return beacon_id;
        }
#endregion
    }

    /// <summary>
    /// Adapt to the new version
    /// </summary>
    public partial class TagLoadingManager
    {
        #region Property
        /// <summary>
        /// Select the usage mode of the Large Space component in the project
        /// </summary>
        [Header("Usage Type")]
        [SerializeField] protected UsageType usageType = UsageType.Custom;

        /// <summary>
        /// Enable loading of Tag type to tracking.
        /// </summary>
        [Header("Default Tracking Marker")]
        [Tooltip("ID 2,6,11,12,13,15,16,18,19,20,21,22,23,24,25,26,27,29,30,31,38,39,41,42,43,44,50,53,55,58,61,62,64")]
        [SerializeField] protected bool DefaultTrackingMarker = false;

        /// <summary>
        /// Enable loading of special Tag type to tracking.
        /// </summary>
        [Header("Special Tracking Marker")]
        [Tooltip("ID 28,32,35,36")]
        [SerializeField] protected bool specialTrackingMarker = false;
        

        [Header("The path to the dependent file")]
        [SerializeField] protected string dependentFilesPath;

        [Header("Whether to start debug")]
        [SerializeField] protected bool m_debug;

        [Serializable]
        public struct CardData
        {
            public CardSingleData CARD_SINGLE;
        }

        [Serializable]
        public struct CardSingleData
        {
            public string CalibFile;
            public int[] Markers;
            public float[] MarkersSize;
        }

        public const string LiBeaconPath = "/LocalizationLiBeacon.json";
        public const string BeaconPath = "/LocalizationBeacon.json";
        public const string TopoPath = "/LocalizationTopo.json";
        public const string ConfigPath = "/GroundPlaneConfig.txt";
        public const string DynamicTargetMarkers1Path = "/sdcard/vpusdk/marker_calib/v8/DynamicTargetMarkers1.json";
        public const string DynamicTargetMarkers2Path = "/sdcard/vpusdk/marker_calib/v8/DynamicTargetMarkers2.json";
        protected Dictionary<int, float> markers = new Dictionary<int, float>();
        protected List<int> markerId = new List<int>();
        protected List<float> markerSize = new List<float>();
        private float angle;
        protected bool isUsingUpCamera = false;
        public bool IsUsingUpCamera
        {
            get => isUsingUpCamera;
        }
        public enum UsageType
        {
            Default,
            Custom
        }
        #endregion

        #region Method
        protected void LoadCalibrationPath()
        {
            if (string.IsNullOrEmpty(dependentFilesPath) || (!Directory.Exists(dependentFilesPath)))
            {
                #region TagTracking

                if (DefaultTrackingMarker)
                {
                    if (File.Exists(DynamicTargetMarkers1Path))
                    {
                        int[] ids = new int[33];

                        XDevicePlugin.LoadTrackingMarkerSettingsFile(DynamicTargetMarkers1Path, out ids, ids.Length);
                    }
                    else
                    {
                        Debug.LogError(DynamicTargetMarkers1Path + " is null.Please update the version.");
                    }
                }

                if (specialTrackingMarker)
                {
                    if (File.Exists(DynamicTargetMarkers2Path))
                    {
                        int[] ids = new int[4];
                        XDevicePlugin.LoadTrackingMarkerSettingsFile(DynamicTargetMarkers2Path, out ids, ids.Length);
                    }
                    else
                    {
                        Debug.LogError(DynamicTargetMarkers2Path + " is null.Please update the version.");
                    }
                }
                #endregion

                Debug.LogError("The file path is sure, check the project configuration");
            }
            else
            {
                #region Tag Ground Plane
                markers.Clear();
                string beaconPath = dependentFilesPath + TagLoadingManager.BeaconPath;
                string liBeaconPath = dependentFilesPath + TagLoadingManager.LiBeaconPath;
                string topoPath = dependentFilesPath + TagLoadingManager.TopoPath;

                if (File.Exists(beaconPath))
                {
                    CardData beaconCard = JsonUtility.FromJson<CardData>(File.ReadAllText(beaconPath));

                    for (int i = 0; i < beaconCard.CARD_SINGLE.Markers.Length; i++)
                    {
                        markers.Add(beaconCard.CARD_SINGLE.Markers[i], beaconCard.CARD_SINGLE.MarkersSize[i]);
                    }

                    int[] ids = new int[beaconCard.CARD_SINGLE.Markers.Length];
                    XDevicePlugin.LoadTrackingMarkerSettingsFile(beaconPath, out ids, ids.Length);
                }
                if (File.Exists(liBeaconPath))
                {
                    CardData liBeaconCard = JsonUtility.FromJson<CardData>(File.ReadAllText(liBeaconPath));
                    for (int i = 0; i < liBeaconCard.CARD_SINGLE.Markers.Length; i++)
                    {
                        markers.Add(liBeaconCard.CARD_SINGLE.Markers[i], liBeaconCard.CARD_SINGLE.MarkersSize[i]);
                    }
                    int[] ids = new int[liBeaconCard.CARD_SINGLE.Markers.Length];
                    XDevicePlugin.LoadTrackingMarkerSettingsFile(liBeaconPath, out ids, ids.Length);
                }
                if (File.Exists(topoPath))
                {
                    CardData topoCard = JsonUtility.FromJson<CardData>(File.ReadAllText(topoPath));
                    for (int i = 0; i < topoCard.CARD_SINGLE.Markers.Length; i++)
                    {
                        markers.Add(topoCard.CARD_SINGLE.Markers[i], topoCard.CARD_SINGLE.MarkersSize[i]);
                    }
                    int[] ids = new int[topoCard.CARD_SINGLE.Markers.Length];

                    XDevicePlugin.LoadTrackingMarkerSettingsFile(topoPath, out ids, ids.Length);
                }
                #endregion


                #region TagTracking

                if (DefaultTrackingMarker)
                {
                    if (File.Exists(DynamicTargetMarkers1Path))
                    {
                        int[] ids = new int[33];

                        XDevicePlugin.LoadTrackingMarkerSettingsFile(DynamicTargetMarkers1Path, out ids, ids.Length);
                    }
                    else
                    {
                        Debug.LogError(DynamicTargetMarkers1Path + " is null.Please update the version.");
                    }
                }

                if (specialTrackingMarker)
                {
                    if (File.Exists(DynamicTargetMarkers2Path))
                    {
                        int[] ids = new int[4];
                        XDevicePlugin.LoadTrackingMarkerSettingsFile(DynamicTargetMarkers2Path, out ids, ids.Length);
                    }
                    else
                    {
                        Debug.LogError(DynamicTargetMarkers2Path + " is null.Please update the version.");
                    }
                }
                #endregion
            }
        }

        protected void CreateGroundPlane()
        {
            #region Clear previous data
            if (_tagGroundbyJson.Count != 0)
            {
                for (int i = 0; i < _tagGroundbyJson.Count; i++)
                {
                    //Destroy(_tagGroundbyJson[i]);
                    Destroy(_tagGroundbyJson[i].gameObject);
                    TagGroundPlane.tagGroundPlaneList.Remove(_tagGroundbyJson[i]);
                }

            }
            _tagGroundbyJson.Clear();
            #endregion

            #region Create TagGroundPlane to the configuration file

            if (File.Exists(dependentFilesPath + TagLoadingManager.ConfigPath))
            {
                GroundPlanePlacementData placementData = JsonUtility.FromJson<GroundPlanePlacementData>(File.ReadAllText(dependentFilesPath + TagLoadingManager.ConfigPath));
                for (int i = 0; i < placementData.items.Length; i++)
                {
                    GroundPlanePlacementItem groundPlaneItem = placementData.items[i];

                    GameObject go = GameObject.Instantiate(Resources.Load("Tag/Prefabs/GroundPlane")) as GameObject;
                    go.name = "GroundPlane - " + groundPlaneItem.beacon_id;
                    var gp = go.GetComponent<TagGroundPlane>();
                    if (m_debug)
                    {
                        gp.DebugView = true;
                        if (markers.ContainsKey(groundPlaneItem.beacon_id))
                        {
                            gp.Size = markers[groundPlaneItem.beacon_id];
                        }
                    }
                    gp.TrackId = groundPlaneItem.beacon_id;
                    gp.BeaconDriftRecenterAngleThreshold = groundPlaneItem.drift_recenter_angle_threshold;
                    gp.BeaconDriftRecenterDistanceThreshold = groundPlaneItem.drift_recenter_distance_threshold;
                    gp.BeaconConfidenceThresh = groundPlaneItem.confidence_thresh;
                    gp.BeaconMaxDistanceThresh = groundPlaneItem.max_distance_thresh;
                    gp.BeaconMinDistanceThresh = groundPlaneItem.min_distance_thresh;
                    gp.BeaconCoordSystemFlag = groundPlaneItem.coord_system_flag;
                    gp.GroupId = groundPlaneItem.group_id;
                    go.transform.position = groundPlaneItem.position;
                    go.transform.eulerAngles = groundPlaneItem.rotation;

                    _tagGroundbyJson.Add(gp);
                }
            }
            else
            {
                Debug.LogError("Not found GroundPlaneConfig.txt");
            }


            #endregion
        }

        private void SetUpCamera(bool enable)
        {
#if !UNITY_EDITOR
            XDevicePlugin.xdevc_vpu_set_up_camera_config(XDevicePlugin.xdevc_get_vpu(), enable, XMR_Manager.UpCameraFPS);
#endif
            Debug.Log("SetUpCamera " + enable);
        }

        #endregion
    }
}
