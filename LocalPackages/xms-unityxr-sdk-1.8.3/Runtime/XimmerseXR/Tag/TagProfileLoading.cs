using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Ximmerse.XR.Tag
{
    /// <summary>
    /// It is used to load the calibration file and pass in the Tag coordinates.
    /// </summary>
    public class TagProfileLoading : TagLoadingManager
    {
        /// <summary>
        /// Enable the loading of calibration parameters with a Tag type of Beacon and ID 65-67.
        /// </summary>
        [Header("Beacon")]
        [Tooltip("ID 65,66,67")]
        [SerializeField] private bool Beacon = true;
        /// <summary>
        /// Enable loading of the Tag type as LiBeacon.
        /// </summary>
        [Header("LiBeacon")]
        [Tooltip("ID 36,35,28,32")]
        [SerializeField] private bool LiBeacon = false;
        /// <summary>
        /// Enable the calibration parameter with Topo Tag type.
        /// </summary>
        [Header("TopoTag")]
        [Tooltip("ID 100-227")]
        [SerializeField] private bool TopoTag100_227 = true;

        [Tooltip("ID 228-407")]
        [SerializeField] private bool TopoTag228_407 = false;
        /// <summary>
        /// Enable the loading of calibration parameters with Tag type SingleCard and ID 18, 19, 22, 23, 25, 39.
        /// </summary>
        [Header("Card")]
        [Tooltip("ID 18,19,22,23,25,39")]
        [SerializeField] private bool SingleCard = false;


        #region Property
        /// <summary>
        /// Select the type of LiBeacon.
        /// </summary>
        [SerializeField] [EnumFlags] private LiBeaconType liBeaconType;
        /// <summary>
        /// Choose the size of the Topo Tag 100-227, 50cm or 38cm.
        /// </summary>
        [SerializeField] private TopoTagSize topoTagSize100_227;
        /// <summary>
        /// Choose the size of the Topo Tag 228-407, 50cm or 38cm.
        /// </summary>
        [SerializeField] private TopoTagSize topoTagSize228_407;
        /// <summary>
        /// Select the type of Gun, 92 or 95.
        /// </summary>
        [SerializeField] [EnumFlags] private GunType guntype;

        /// <summary>
        /// The path to the calibration file in the RhinoX.
        /// </summary>

        public const string CalibraFilePath = "/sdcard/vpusdk/marker_calib";
        public const string CalibraFilePathV8 = "/sdcard/vpusdk/marker_calib/v8";

        public const string BeaconPathOldPath = CalibraFilePath + "/BEACON/BEACON-500.json";

        public const string LiBeacon36OldPath = CalibraFilePath + "/BEACON/LiBeacon-500-1.json";
        public const string LiBeacon35OldPath = CalibraFilePath + "/BEACON/LiBeacon-500-2.json";
        public const string LiBeacon28OldPath = CalibraFilePath + "/BEACON/LiBeacon-500-3.json";
        public const string LiBeacon32OldPath = CalibraFilePath + "/BEACON/LiBeacon-500-4.json";

        public const string LiBeacon36NewPath = CalibraFilePathV8 + "/LocalizationMarker36.json";
        public const string LiBeacon35NewPath = CalibraFilePathV8 + "/LocalizationMarker35.json";
        public const string LiBeacon28NewPath = CalibraFilePathV8 + "/LocalizationMarker28.json";
        public const string LiBeacon32NewPath = CalibraFilePathV8 + "/LocalizationMarker32.json";

        public const string TopoTag100_227_450mmOldPath = CalibraFilePath + "/BEACON/Topotag_model_100_to_227.json";
        public const string TopoTag100_227_350mmOldPath = CalibraFilePath + "/BEACON/Topotag_model_100_to_227_350mm.json";

        public const string TopoTag100_227_450mmNewPath = CalibraFilePathV8 + "/Topotag_model_100_to_227.json";
        public const string TopoTag100_227_350mmNewPath = CalibraFilePathV8 + "/Topotag_model_100_to_227_350mm.json";


        public const string TopoTag228_407_350mmOldPath = CalibraFilePath + "/BEACON/Topotag_4x4_circle_model_228_to_407_350mm.json";
        public const string TopoTag228_407_450mmOldPath = CalibraFilePath + "/BEACON/Topotag_4x4_circle_model_228_to_407_450mm.json";

        public const string TopoTag228_407_350mmNewPath = CalibraFilePathV8 + "/Topotag_4x4_circle_model_228_to_407_350mm.json";
        public const string TopoTag228_407_450mmNewPath = CalibraFilePathV8 + "/Topotag_4x4_circle_model_228_to_407_450mm.json";

        public const string SingleCardPath = CalibraFilePath + "/CARD/single_markers_500_03_62mm.json";
        private int markerSum;
        Coroutine setGroundPlane;
        #region enum Type
        public class EnumFlags : PropertyAttribute
        {
        }


        /// <summary>
        /// The size of the Topo Tag.
        /// </summary>
        public enum TopoTagSize
        {
            TopoTag_450mm,
            TopoTag_350mm,
        }

        /// <summary>
        /// The type of gun.
        /// </summary>
        [System.Flags]
        public enum GunType
        {
            gunsight92 = 1,
            gunsight95 = 2,
        }
        /// <summary>
        /// The type and ID of LiBeacon
        /// </summary>
        [System.Flags]
        public enum LiBeaconType
        {
            LiBeacon_1ID36 = 1,
            LiBeacon_2ID35 = 2,
            LiBeacon_3ID28 = 4,
            LiBeacon_4ID32 = 8,
        }

        public enum SingleCardSize
        {
            Single_40mm,
            Single_62mm,
        }
        #endregion

        #region Instance
        private static TagProfileLoading instance;
        public static TagProfileLoading Instance
        {
            get
            {
                if(!instance)
                {
                    instance = FindObjectOfType<TagProfileLoading>();
                }
                return instance;
            }
        }
        #endregion
        public List<int> TrackingTagList
        {
            get => _trackingtaglist;
        }
        public int TagFusion
        {
            get => _tagfusion;
        }
        public Thread ThreadLoad
        {
            get => threadLoad;
        }
        public List<TagGroundPlane> TagGroundbyJson
        {
            get => _tagGroundbyJson;
            set => _tagGroundbyJson = value;
        }
        public List<TagGroundPlane> AllTagGroundPlane
        {
            get => TagGroundPlane.tagGroundPlaneList;
        }
        public Vector3 OffsetPos
        {
            get => offsetPos;
            set => offsetPos = value;
        }
        public Vector3 OffsetRot
        {
            get => offsetRot;
            set => offsetRot = value;
        }
        public UsageType CurrentUsageType
        {
            get => usageType;
        }
        public string DependentFilesPath
        {
            get => dependentFilesPath;
            set => dependentFilesPath = value;
        }
        #endregion

        #region Unity
        private void Awake()
        {
            if (instance==null)
            {
                instance = this;
            }
            //ThreadTagLoading();
        }

        //private void Start()
        //{
        //    if (setGroundPlane!=null)
        //    {
        //        StopCoroutine(setGroundPlane);
        //    }
        //    setGroundPlane = StartCoroutine(StartFusion());
        //}

//        private void OnDestroy()
//        {
//#if !UNITY_EDITOR
//            XDevicePlugin.ResetTrackingMarkerSettings();
//#endif
//        }
        #endregion

        #region Method
        /// <summary>
        /// Refresh and re-acquire the coordinate information of the large spatial positioning board.
        /// </summary>
        public void RefreshBeaconTran()
        {
            RefreshBeacon();
        }
        /// <summary>
        /// Clearing the algorithm data invalidates the large spatial positioning function.
        /// </summary>
        public void CleanBeaconData()
        {
            CleanBeacon();
        }
        /// <summary>
        /// Set targeting parameters
        /// </summary>
        public void SettingData()
        {
            SettingTagData();
        }
        /// <summary>
        /// Load calibration parameters
        /// </summary>
        private void SetCalibraFile()
        {
#if !UNITY_EDITOR
            XDevicePlugin.ResetTrackingMarkerSettings();

            if (usageType==UsageType.Custom)
            {
                if (Beacon)
                {
                    if (File.Exists(BeaconPathOldPath))
                    {
                        int[] ids = new int[3];
                        XDevicePlugin.LoadTrackingMarkerSettingsFile(BeaconPathOldPath, out ids, 3);
                    }
                    else
                    {
                        Debug.LogError(BeaconPathOldPath + "  file missing");
                    }
                }

                if (LiBeacon)
                {
                    if ((liBeaconType & LiBeaconType.LiBeacon_1ID36) != 0)
                    {
                        if (File.Exists(LiBeacon36OldPath))
                        {
                            int[] ids = new int[1];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(LiBeacon36OldPath, out ids, 1);
                        }
                        else if (File.Exists(LiBeacon36NewPath))
                        {
                            int[] ids = new int[1];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(LiBeacon36NewPath, out ids, 1);
                        }
                        else
                        {
                            Debug.LogError(LiBeacon36OldPath + " and " + LiBeacon36NewPath + " file missing");
                        }
                    }

                    if ((liBeaconType & LiBeaconType.LiBeacon_2ID35) != 0)
                    {
                        if (File.Exists(LiBeacon35OldPath))
                        {
                            int[] ids = new int[1];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(LiBeacon35OldPath, out ids, 1);
                        }
                        else if (File.Exists(LiBeacon35NewPath))
                        {
                            int[] ids = new int[1];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(LiBeacon35NewPath, out ids, 1);
                        }
                        else
                        {
                            Debug.LogError(LiBeacon35OldPath + " and " + LiBeacon35NewPath + " file missing");
                        }
                    }

                    if ((liBeaconType & LiBeaconType.LiBeacon_3ID28) != 0)
                    {
                        if (File.Exists(LiBeacon28OldPath))
                        {
                            int[] ids = new int[1];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(LiBeacon28OldPath, out ids, 1);
                        }
                        else if (File.Exists(LiBeacon28NewPath))
                        {
                            int[] ids = new int[1];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(LiBeacon28NewPath, out ids, 1);
                        }
                        else
                        {
                            Debug.LogError(LiBeacon28OldPath + " and " + LiBeacon28NewPath + " file missing");
                        }
                    }

                    if ((liBeaconType & LiBeaconType.LiBeacon_4ID32) != 0)
                    {
                        if (File.Exists(LiBeacon32OldPath))
                        {
                            int[] ids = new int[1];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(LiBeacon32OldPath, out ids, 1);
                        }
                        else if (File.Exists(LiBeacon32NewPath))
                        {
                            int[] ids = new int[1];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(LiBeacon32NewPath, out ids, 1);
                        }
                        else
                        {
                            Debug.LogError(LiBeacon32OldPath + " and " + LiBeacon32NewPath + " file missing");
                        }
                    }
                }


                if (TopoTag100_227)
                {
                    if (topoTagSize100_227 == TopoTagSize.TopoTag_450mm)
                    {
                        if (File.Exists(TopoTag100_227_450mmOldPath))
                        {
                            int[] ids = new int[128];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(TopoTag100_227_450mmOldPath, out ids, 128);
                        }
                        else if (File.Exists(TopoTag100_227_450mmNewPath))
                        {
                            int[] ids = new int[128];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(TopoTag100_227_450mmNewPath, out ids, 128);
                        }
                        else
                        {
                            Debug.LogError(TopoTag100_227_450mmOldPath + " and " + TopoTag100_227_450mmNewPath + " file missing");
                        }
                    }

                    if (topoTagSize100_227 == TopoTagSize.TopoTag_350mm)
                    {
                        if (File.Exists(TopoTag100_227_350mmOldPath))
                        {
                            int[] ids = new int[128];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(TopoTag100_227_350mmOldPath, out ids, 128);
                        }
                        else if (File.Exists(TopoTag100_227_350mmNewPath))
                        {
                            int[] ids = new int[128];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(TopoTag100_227_350mmNewPath, out ids, 128);
                        }
                        else
                        {
                            Debug.LogError(TopoTag100_227_350mmOldPath + " and " + TopoTag100_227_350mmNewPath + " file missing");
                        }
                    }
                }

                if (TopoTag228_407)
                {
                    if (topoTagSize228_407 == TopoTagSize.TopoTag_350mm)
                    {
                        if (File.Exists(TopoTag228_407_350mmOldPath))
                        {
                            int[] ids = new int[180];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(TopoTag228_407_350mmOldPath, out ids, 180);
                        }
                        else if (File.Exists(TopoTag228_407_350mmNewPath))
                        {
                            int[] ids = new int[180];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(TopoTag228_407_350mmNewPath, out ids, 180);
                        }
                        else
                        {
                            Debug.LogError(TopoTag228_407_350mmOldPath + " and " + TopoTag228_407_350mmNewPath + " file missing");
                        }
                    }

                    if (topoTagSize228_407 == TopoTagSize.TopoTag_450mm)
                    {
                        if (File.Exists(TopoTag228_407_450mmOldPath))
                        {
                            int[] ids = new int[180];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(TopoTag228_407_450mmOldPath, out ids, 180);
                        }
                        else if (File.Exists(TopoTag228_407_450mmNewPath))
                        {
                            int[] ids = new int[180];
                            XDevicePlugin.LoadTrackingMarkerSettingsFile(TopoTag228_407_450mmNewPath, out ids, 180);
                        }
                        else
                        {
                            Debug.LogError(TopoTag228_407_450mmOldPath + " and " + TopoTag228_407_450mmNewPath + " file missing");
                        }
                    }
                }

                if (SingleCard)
                {
                    if (File.Exists(SingleCardPath))
                    {
                        int[] ids = new int[6];
                        XDevicePlugin.LoadTrackingMarkerSettingsFile(SingleCardPath, out ids, 6);
                    }
                    else
                    {
                        Debug.LogError(SingleCardPath + " file missing");
                    }
                }

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
                        int[] ids = new int[33];
                        XDevicePlugin.LoadTrackingMarkerSettingsFile(DynamicTargetMarkers2Path, out ids, ids.Length);
                    }
                    else
                    {
                        Debug.LogError(DynamicTargetMarkers2Path + " is null.Please update the version.");
                    }
                }
            }
            else
            {
                LoadCalibrationPath();
            }
#endif
        }

        public void CleanAllMarker()
        {
            threadLoad = null;
#if !UNITY_EDITOR
            XDevicePlugin.ResetTrackingMarkerSettings();
#endif
        }

        public void ThreadTagLoading()
        {
            Thread thread;
            thread = new Thread(SetCalibraFile);
            threadLoad = thread;
            threadLoad.Start();
        }

        public void EnablePositioningAndTracking()
        {
            ThreadTagLoading();
            if (setGroundPlane != null)
            {
                StopCoroutine(setGroundPlane);
            }
            setGroundPlane = StartCoroutine(StartFusion());
        }

        #endregion
    }
}

