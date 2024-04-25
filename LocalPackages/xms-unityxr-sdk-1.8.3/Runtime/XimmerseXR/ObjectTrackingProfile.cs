using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using Object = UnityEngine.Object;
using System.Threading;
using System.Threading.Tasks;

namespace Ximmerse.XR
{
    /// <summary>
    /// Internal object : represents a json file that defines the calibration data path and marker reference.
    /// </summary>
    [System.Serializable]
    internal class TrackedObjectJson
    {

        [System.Serializable]
        public class CardGroupJson
        {
            public string CalibFile;

            public string ModeType;

            public int GroupID;

            public int[] Markers;

            public float[] MarkersSize;
        }

        [System.Serializable]
        public class SingleCardJson
        {
            public string CalibFile;

            public int[] Markers;

            public float[] MarkersSize;
        }

        [SerializeField]
        public CardGroupJson CARD_GROUP;

        [SerializeField]
        public SingleCardJson CARD_SINGLE;


        public bool IsCardGroup
        {
            get
            {
                return CARD_GROUP != null && !string.IsNullOrEmpty(CARD_GROUP.CalibFile) && CARD_GROUP.GroupID >= 0;
            }
        }


        #region Ext Methods 

        /// <summary>
        /// Fills the trackable json's content to a dictionary map where key = markerID, value = marker's config info.
        /// </summary>
        /// <param name="configurationMap">Configuration map.</param>
        public void FillDictionary(Dictionary<int, MarkerConfigInfo> configurationMap)
        {
            var jsonObj = this;
            var map = configurationMap;
            if (jsonObj.CARD_SINGLE.Markers != null && jsonObj.CARD_SINGLE.MarkersSize != null && !string.IsNullOrEmpty(jsonObj.CARD_SINGLE.CalibFile) &&
           jsonObj.CARD_SINGLE.Markers.Length == jsonObj.CARD_SINGLE.MarkersSize.Length && jsonObj.CARD_SINGLE.Markers.Length > 0)
            {
                for (int j = 0, jsonObjCARD_SINGLEMarkersLength = jsonObj.CARD_SINGLE.Markers.Length; j < jsonObjCARD_SINGLEMarkersLength; j++)
                {
                    int markerID = jsonObj.CARD_SINGLE.Markers[j];
                    float markerSize = jsonObj.CARD_SINGLE.MarkersSize[j];
                    var markerConfigInfo = new MarkerConfigInfo()
                    {
                        MarkerID = markerID,
                        MarkerConfigSize = markerSize,
                        markerType = ConfigMarkerType.SingleMarker,
                        GroupType = string.Empty,
                    };
                    if (!map.ContainsKey(markerID))
                    {
                        map.Add(markerID, markerConfigInfo);
                    }
                    else
                    {
                        map[markerID] = markerConfigInfo;
                    }
                }
            }
            //For card_group:
            else if (jsonObj.CARD_GROUP.Markers != null && jsonObj.CARD_GROUP.MarkersSize != null && !string.IsNullOrEmpty(jsonObj.CARD_GROUP.CalibFile) &&
            jsonObj.CARD_GROUP.Markers.Length == jsonObj.CARD_GROUP.MarkersSize.Length && jsonObj.CARD_GROUP.Markers.Length > 0)
            {
                //add marke group:
                {
                    int groupID = jsonObj.CARD_GROUP.GroupID;
                    float markerSize = jsonObj.CARD_GROUP.MarkersSize[0];
                    var markerConfigInfo = new MarkerConfigInfo()
                    {
                        MarkerID = groupID,
                        MarkerConfigSize = markerSize,
                        markerType = ConfigMarkerType.GroupNode,
                        GroupType = jsonObj.CARD_GROUP.ModeType.ToUpper(),
                    };

                    if (!map.ContainsKey(groupID))
                    {
                        map.Add(groupID, markerConfigInfo);
                    }
                    else
                    {
                        map[groupID] = markerConfigInfo;
                    }
                }

                //add sub-marker:
                {
                    for (int j = 0, jsonObjCARD_GROUPMarkersLength = jsonObj.CARD_GROUP.Markers.Length; j < jsonObjCARD_GROUPMarkersLength; j++)
                    {
                        int markerID = jsonObj.CARD_GROUP.Markers[j];
                        float markerSize = jsonObj.CARD_GROUP.MarkersSize[j];
                        var markerConfigInfo = new MarkerConfigInfo()
                        {
                            MarkerID = markerID,
                            MarkerConfigSize = markerSize,
                            markerType = ConfigMarkerType.MarkerGroup_Submarker,
                            GroupType = string.Empty,
                        };

                        if (!map.ContainsKey(markerID))
                        {
                            map.Add(markerID, markerConfigInfo);
                        }
                        else
                        {
                            map[markerID] = markerConfigInfo;
                        }
                    }
                }

            }
        }
        #endregion
    }

    /// <summary>
    /// Tracking item.
    /// </summary>
    [System.Serializable]
    public class TrackingItem
    {
        /// <summary>
        /// Editor only 
        /// </summary>
        public UnityEngine.Object JSONConfig = null;

        /// <summary>
        /// The name of the json file.
        /// </summary>
        public string jsonName;

        /// <summary>
        /// The content of the json.
        /// </summary>
        public string jsonContent;

        [SerializeField]
        internal int[] m_MarkerIDs;

        /// <summary>
        /// The marker IDs inside the Json file
        /// </summary>
        public int[] MarkerIDs
        {
            get => m_MarkerIDs;
        }

        /// <summary>
        /// The datas
        /// </summary>
        [SerializeField]
        [HideInInspector]
        internal byte[] m_Data;

        /// <summary>
        /// Marker size
        /// </summary>
        [SerializeField]
        internal float[] m_MarkerSizes;

        /// <summary>
        /// The marker size.
        /// </summary>
        public float[] MarkerSizes
        {
            get => m_MarkerSizes;
        }

        [SerializeField]
        internal bool m_TrackedAsGroup;

        /// <summary>
        /// If true, the markers are taken in a group when being tracked, as reference group id.
        /// If false, the markers are tracked seperately as single piece.
        /// </summary>
        public bool TrackedAsGroup
        {
            get => m_TrackedAsGroup;
        }

        [System.NonSerialized]
        internal TrackedObjectJson trackableJson;
    }

    /// <summary>
    /// Marker config info : a single marker object's config info from JSON.
    /// </summary>
    internal struct MarkerConfigInfo
    {
        /// <summary>
        /// The marker identifier.
        /// </summary> 
        public int MarkerID;
        /// <summary>
        /// The size of the marker config.
        /// </summary>
        public float MarkerConfigSize;

        /// <summary>
        /// 是 Marker 组还是 单个Marker ?
        /// </summary>
        public ConfigMarkerType markerType;

        /// <summary>
        /// The type of the group : controller | cube | map
        /// </summary>
        public string GroupType;
    }

    /// <summary>
    /// Config marker type.
    /// </summary>
    internal enum ConfigMarkerType
    {
        /// <summary>
        /// The marker is config as a single card
        /// </summary>
        SingleMarker = 0,

        /// <summary>
        /// The marker is config as a sub-card of the group
        /// </summary>
        MarkerGroup_Submarker = 1,

        /// <summary>
        /// This is a top-group.
        /// </summary>
        GroupNode = 2,
    }


    //[CreateAssetMenu(menuName = "Ximmerse/Tracking Profile", fileName = "TrackingProfile")]
    public sealed class ObjectTrackingProfile : ScriptableObject
    {
        [System.NonSerialized]
        bool m_IsLoaded;

        /// <summary>
        /// Has the tracking profile loaded by tracking system ?
        /// </summary>
        /// <value><c>true</c> if is loaded; otherwise, <c>false</c>.</value>
        public bool IsLoaded
        {
            get
            {
                return m_IsLoaded;
            }
            internal set
            {
                m_IsLoaded = value;
            }
        }

        [Multiline(3)]
        public string Description;

        /// <summary>
        /// Config the tracking items.
        /// </summary>
        [SerializeField]
        TrackingItem[] items = new TrackingItem[] {
        };

        /// <summary>
        /// Gets the tracking items.
        /// </summary>
        /// <value>The tracking items.</value>
        public TrackingItem[] trackingItems
        {
            get
            {
                return items;
            }
        }

        [SerializeField]
        bool m_TrackBeacons = true;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Ximmerse.RhinoX.ObjectTrackingProfile"/> track beacon.
        /// </summary>
        /// <value><c>true</c> if track beacon; otherwise, <c>false</c>.</value>
        public bool TrackBeacons
        {
            get
            {
                return m_TrackBeacons;
            }
        }

        /// <summary>
        /// Customize tracking calibration files.
        /// </summary>
        [SerializeField, Tooltip("If true, enables additional object tracking. Develoeprs may add the tracking calibration files by drag and drop to the below area.")]
        bool m_CustomTrackingCalibrationFiles;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Ximmerse.RhinoX.ObjectTrackingProfile"/> customize tracking
        /// calibration files.
        /// </summary>
        /// <value><c>true</c> if custom tracking calibration files; otherwise, <c>false</c>.</value>
        public bool CustomTrackingCalibrationFiles
        {
            get
            {
                return m_CustomTrackingCalibrationFiles;
            }
        }

        private void OnEnable()
        {
        }

        private void Awake()
        {
            //Copies json and dat to internal path :
            if (Application.platform == RuntimePlatform.Android)
            {
                if (s_BuiltTrackingItemsToCopy == null)
                {
                    s_BuiltTrackingItemsToCopy = new List<TrackingItem>();
                }
                s_BuiltTrackingItemsToCopy.AddRange(this.trackingItems);

                if (!copyThreadStarted)
                    Task.Run(CopyData);
            }
        }

        static List<TrackingItem> s_BuiltTrackingItemsToCopy = null;

        private static bool copyThreadStarted = false;

        /// <summary>
        /// Copy built player's tracking files to internal storage.
        /// </summary>
        private static void CopyData()
        {
            Thread.Sleep(10);
            while (string.IsNullOrEmpty(SDKVariants.kTrackingDataDir_Internal))
            {
                Thread.Sleep(10);
            }
            foreach (var item in s_BuiltTrackingItemsToCopy)
            {
                string jsonPath = Path.Combine(SDKVariants.kTrackingDataDir_Internal, item.jsonName);
                string dataPath = Path.Combine(SDKVariants.kTrackingDataDir_Internal, Path.GetFileNameWithoutExtension(item.jsonName) + ".dat");
                if (!File.Exists(jsonPath))
                {
                    File.WriteAllText(jsonPath, item.jsonContent);
                    Debug.LogFormat("Tracking profile : {0} has been copied to internal path", item.jsonName);
                }
                if (!File.Exists(dataPath))
                {
                    File.WriteAllBytes(dataPath, item.m_Data);
                    Debug.LogFormat("Tracking data : {0} has been copied to internal path", item.jsonName);
                }
            }
            s_BuiltTrackingItemsToCopy = null;
        }



    }
}