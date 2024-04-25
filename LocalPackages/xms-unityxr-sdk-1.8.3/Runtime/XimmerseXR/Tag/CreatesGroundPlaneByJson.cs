using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;

namespace Ximmerse.XR.Tag
{
    [System.Serializable]
    public struct GroundPlanePlacementData
    {
        public GroundPlanePlacementItem[] items;
    }

    [System.Serializable]
    public class GroundPlanePlacementItem
    {
        public int beacon_id;

        public int group_id;

        public float size;

        public Vector3 position;

        public Vector3 rotation;

        public int coord_system_flag; // 1 = left hand (unity), 0 = right hand (openXR)

        public float confidence_thresh = 0.9f;

        public float min_distance_thresh = 0.1f;

        public float max_distance_thresh = 1.8f;

        public float drift_recenter_angle_threshold = 1.0f;

        public float drift_recenter_distance_threshold = 1.0f;
    }

    /// <summary>
    /// 读取指定路径的json配置文件，自动创建 Ground Plane。
    /// 配置文件举例:
    /*
     {
         "items": [
             {
                "beacon_id": 67,
                "group_id": 0,
                 "position": {
                     "x": 0.0,
                     "y": 0.0,
                     "z": 0.0
                 },
                 "rotation": {
                     "x": 0.0,
                     "y": 0.0,
                     "z": 0.0
                 },
                 "coord_system_flag": 1,
                 "confidence_thresh": 0.9,
                 "max_distance_thresh": 1.8,
                 "min_distance_thresh": 0.1,
                 "drift_recenter_angle_threshold": 1,
                 "drift_recenter_distance_threshold": 1
             },
             {
                 "beacon_id": 66,
                 "group_id": 1,
                 "position": {
                     "x": 1.0,
                     "y": 1.0,
                     "z": 1.0
                 },
                 "rotation": {
                     "x": 0.0,
                     "y": 90.0,
                     "z": 0.0
                 },
                 "coord_system_flag": 1,
                 "confidence_thresh": 0.9,
                 "max_distance_thresh": 1.8,
                 "min_distance_thresh": 0.1,
                 "drift_recenter_angle_threshold": 1,
                 "drift_recenter_distance_threshold": 1
             },
             {
                "beacon_id": 65,
                "group_id": 1,
                "position": {
                    "x": 0.0,
                    "y": 0.0,
                    "z": 1.0
                },
                "rotation": {
                    "x": 0.0,
                    "y": 0.0,
                    "z": 0.0
                },
                "coord_system_flag": 1,
                "confidence_thresh": 0.9,
                "max_distance_thresh": 1.8,
                "min_distance_thresh": 0.1,
                "drift_recenter_angle_threshold": 1,
                "drift_recenter_distance_threshold": 1
            }
         ]
     }
    */
    /// </summary>
    public class CreatesGroundPlaneByJson : MonoBehaviour
    {
        private static CreatesGroundPlaneByJson instance;
        private CreatesGroundPlaneByJson() { }
        public static CreatesGroundPlaneByJson Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<CreatesGroundPlaneByJson>();
                }
                return instance;
            }
        }
        private void Awake()
        {
            instance = this;
        }

        public enum JsonFilePathType
        {
            /// <summary>
            /// Reads file at path, at runtime
            /// </summary>
            FileAtPath,

            /// <summary>
            /// Reads json file as a text assets.
            /// </summary>
            Reference,
        }

        public JsonFilePathType FileType = JsonFilePathType.FileAtPath;
            

        public string JsonFilePath = "/sdcard/GroundPlaneConfig.txt";

        public TextAsset jsonFile;

        public bool autoCreates = true;

        public bool debugView = false;

        public float size = 0.17f;

        [ContextMenu("Create ground plane from json config")]
        public void CreateGroundPlanesFromConfig()
        {
            if (TagProfileLoading.Instance.CurrentUsageType==TagProfileLoading.UsageType.Custom)
            {
                if (TagProfileLoading.Instance.TagGroundbyJson.Count != 0)
                {
                    for (int i = 0; i < TagProfileLoading.Instance.TagGroundbyJson.Count; i++)
                    {
                        Destroy(TagProfileLoading.Instance.TagGroundbyJson[i].gameObject);
                        TagGroundPlane.tagGroundPlaneList.Remove(TagProfileLoading.Instance.TagGroundbyJson[i]);
                    }

                    TagProfileLoading.Instance.TagGroundbyJson.Clear();
                }
                try
                {
                    if ((FileType == JsonFilePathType.FileAtPath && File.Exists(JsonFilePath))
                        ||
                        (FileType == JsonFilePathType.Reference && jsonFile))
                    {
                        var txt = FileType == JsonFilePathType.FileAtPath ? File.ReadAllText(JsonFilePath) : jsonFile.text;
                        if (!string.IsNullOrEmpty(txt))
                        {
                            GroundPlanePlacementData placementData = JsonUtility.FromJson<GroundPlanePlacementData>(txt);

                            for (int i = 0; i < placementData.items.Length; i++)
                            {
                                GroundPlanePlacementItem groundPlaneItem = placementData.items[i];

                                GameObject go = GameObject.Instantiate(Resources.Load("Tag/Prefabs/GroundPlane")) as GameObject;
                                go.name = "GroundPlane - " + groundPlaneItem.beacon_id;
                                var gp = go.GetComponent<TagGroundPlane>();
                                if (debugView)
                                {
                                    gp.DebugView = true;
                                    gp.Size = size;
                                }
                                gp.TrackId = groundPlaneItem.beacon_id;
                                //gp.text.text = gp.track_id.ToString();
                                gp.BeaconDriftRecenterAngleThreshold = groundPlaneItem.drift_recenter_angle_threshold;
                                gp.BeaconDriftRecenterDistanceThreshold = groundPlaneItem.drift_recenter_distance_threshold;
                                gp.BeaconConfidenceThresh = groundPlaneItem.confidence_thresh;
                                gp.BeaconMaxDistanceThresh = groundPlaneItem.max_distance_thresh;
                                gp.BeaconMinDistanceThresh = groundPlaneItem.min_distance_thresh;
                                gp.BeaconCoordSystemFlag = groundPlaneItem.coord_system_flag;
                                gp.GroupId = groundPlaneItem.group_id;
                                go.transform.position = groundPlaneItem.position;
                                go.transform.eulerAngles = groundPlaneItem.rotation;

                                TagProfileLoading.Instance.TagGroundbyJson.Add(gp);
                            }
                            //var txt = Resources.Load<TextAsset>("groundplane-layout").ToString();

                        }
                    }
                    else
                    {
                        Debug.LogError("The path is empty, please check that the file path is entered correctly");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }


        [ContextMenu("Test convert to json string")]
        public void TestToJson()
        {
            GroundPlanePlacementData data = new GroundPlanePlacementData();
            data.items = new GroundPlanePlacementItem[]
            {
                new GroundPlanePlacementItem()
                {
                     beacon_id = 1, position = Vector3.zero, rotation = Vector3.zero,
                },

                new GroundPlanePlacementItem()
                {
                    beacon_id = 2, position = Vector3.one, rotation = new  Vector3(0,90,0),
                    confidence_thresh = 0.85f, coord_system_flag = 0, drift_recenter_angle_threshold = 7.5f,
                    drift_recenter_distance_threshold = 0.75f,
                    group_id = 1,
                    max_distance_thresh= 1.1f,
                    min_distance_thresh = 0.5f,
                }
            };

            Debug.Log(JsonUtility.ToJson(data, true));
        }
    }
}