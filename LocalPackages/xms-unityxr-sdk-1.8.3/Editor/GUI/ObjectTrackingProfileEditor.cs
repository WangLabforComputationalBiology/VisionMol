using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Collections;

namespace Ximmerse.XR
{
    /// <summary>
    /// Tracked object json (editor only)
    /// </summary>
    [System.Serializable]
    internal class TrackedObjectJsonEditor
    {
        /// <summary>
        /// Card group json (editor only)
        /// </summary>
        [System.Serializable]
        public class CardGroupJsonEditor
        {
            /// <summary>
            /// The calibration file name.
            /// </summary>
            public string CalibFile;

            /// <summary>
            /// The type of the mode.
            /// </summary>
            public string ModeType;

            /// <summary>
            /// The group identifier.
            /// </summary>
            public int GroupID;

            /// <summary>
            /// The sub-markers.
            /// </summary>
            public int[] Markers;

            /// <summary>
            /// The size of the sub-markers.
            /// </summary>
            public float[] MarkersSize;
        }

        /// <summary>
        /// Single card json (editor only)
        /// </summary>
        [System.Serializable]
        public class SingleCardJsonEditor
        {
            /// <summary>
            /// The calibration file name.
            /// </summary>
            public string CalibFile;

            /// <summary>
            /// The single markers
            /// </summary>
            public int[] Markers;

            /// <summary>
            /// The size of the single markers.
            /// </summary>
            public float[] MarkersSize;

            public string CardIDs = string.Empty;
        }

        /// <summary>
        /// The card group.
        /// </summary>
        [SerializeField]
        public CardGroupJsonEditor CARD_GROUP = null;

        /// <summary>
        /// The card single.
        /// </summary>
        [SerializeField]
        public SingleCardJsonEditor CARD_SINGLE = null;


        public bool IsCardGroup
        {
            get
            {
                return CARD_GROUP != null && !string.IsNullOrEmpty(CARD_GROUP.CalibFile) && CARD_GROUP.GroupID >= 0;
            }
        }

        [System.NonSerialized]
        public TrackedObjectJsonEditor conflictTo = null;

        [System.NonSerialized]
        public string conflictToJson = null;

        public bool ValidateVs(TrackedObjectJsonEditor otherJson)
        {
            List<int> markers01 = new List<int>();
            if (this.IsCardGroup)
            {
                markers01.AddRange(this.CARD_GROUP.Markers);
            }
            else
            {
                markers01.AddRange(this.CARD_SINGLE.Markers);
            }

            List<int> markers02 = new List<int>();
            if (otherJson.IsCardGroup)
            {
                markers02.AddRange(otherJson.CARD_GROUP.Markers);
            }
            else
            {
                markers02.AddRange(otherJson.CARD_SINGLE.Markers);
            }

            if (markers01.Intersect(markers02).Count() > 0)
            {
                return false;
            }
            else return true;
        }
    }

    [CustomEditor(typeof(ObjectTrackingProfile))]
    [CanEditMultipleObjects]
    internal sealed class ObjectTrackingProfileInspector : UnityEditor.Editor
    {
        string errorMsg = string.Empty;

        string markerMsg = string.Empty;

        ObjectTrackingProfile mTarget
        {
            get
            {
                return this.target as ObjectTrackingProfile;
            }
        }

        TrackedObjectJsonEditor[] jsons = null;

        SerializedProperty TrackingItemArray = null;

        void OnEnable()
        {
            RefreshJsonItems();

            TrackingItemArray = serializedObject.FindProperty("items");
        }

        private void RefreshJsonItems()
        {
            jsons = mTarget.trackingItems.Select(x => JsonUtility.FromJson<TrackedObjectJsonEditor>(
            File.ReadAllText(AssetDatabase.GetAssetPath(x.JSONConfig))
                )).ToArray();

            foreach (var json in jsons)
            {
                //fill marker info:
                if (json.CARD_SINGLE != null && !string.IsNullOrEmpty(json.CARD_SINGLE.CalibFile) && json.CARD_SINGLE.Markers != null
                && json.CARD_SINGLE.Markers.Length > 0)
                {
                    StringBuilder buffer = new StringBuilder();
                    int c = 0;
                    foreach (var id in json.CARD_SINGLE.Markers)
                    {
                        c++;
                        if (c >= 15)
                        {
                            c = 0;
                            buffer.AppendFormat("{0} \r\n", id);
                        }
                        else
                        {
                            buffer.AppendFormat("{0} ", id);

                        }
                    }
                    json.CARD_SINGLE.CardIDs = buffer.ToString();
                }



                //check duplication:
                for (int i = 0, jsonsLength = jsons.Length; i < jsonsLength; i++)
                {
                    var json2 = jsons[i];
                    if (json != json2)
                    {
                        if (!json.ValidateVs(json2))
                        {
                            json.conflictTo = json2;
                            json.conflictToJson = this.mTarget.trackingItems[i].JSONConfig.name;
                            continue;
                        }
                        json.conflictTo = null;
                    }
                }
            }

            //check config:

        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Script"), true);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Description"), true);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("--- Tracking Config ---");
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_TrackBeacons"), true);
            //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_TrackController"),new GUIContent("Track Right Controller"), true);
            //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_TrackLeftController"), true);
            EditorGUILayout.EndVertical();


            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_CustomTrackingCalibrationFiles"), new GUIContent("Additive Tracking"));

            if (mTarget.CustomTrackingCalibrationFiles)
            {
                EditorGUI.indentLevel++;
                DropAreaGUI();

                if (jsons != null && jsons.Length > 0)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.Space();

                    for (int i = 0, maxjsonsLength = this.jsons.Length; i < maxjsonsLength; i++)
                    {
                        var jItem = this.jsons[i];
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                        {
                            TrackingItemArray.DeleteArrayElementAtIndex(i);
                            serializedObject.ApplyModifiedProperties();
                            RefreshJsonItems();
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                            return;
                        }
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField(mTarget.trackingItems[i].JSONConfig, typeof(UnityEngine.Object), true, GUILayout.MaxWidth(200));
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.EndHorizontal();

                        if (jItem.CARD_GROUP != null && !string.IsNullOrEmpty(jItem.CARD_GROUP.CalibFile) && jItem.CARD_GROUP.GroupID >= 0)
                        {
                            //it's card group
                            EditorGUILayout.LabelField(string.Format("Tracking ID: {0}", jItem.CARD_GROUP.GroupID));
                        }
                        else if (jItem.CARD_SINGLE != null && !string.IsNullOrEmpty(jItem.CARD_SINGLE.CardIDs))
                        {
                            //it's card single
                            EditorGUILayout.TextArea(string.Format("Tracking ID: {0}", jItem.CARD_SINGLE.CardIDs), GUILayout.Height(60));
                        }

                        if (jItem.conflictTo != null)
                        {
                            EditorGUILayout.LabelField(string.Format("conflict to: {0}", jItem.conflictToJson));
                        }
                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.EndVertical();

                }

                EditorGUI.indentLevel--;
            }


            serializedObject.ApplyModifiedProperties();

        }

        void DropAreaGUI()
        {
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 40.0f, GUILayout.ExpandWidth(true));

            EditorGUILayout.LabelField("[Drag JSON tracking profile to this area]");
            GUI.Box(drop_area, "");
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        var dragObjs = DragAndDrop.objectReferences;
                        foreach (var obj in dragObjs)
                        {
                            if (IsValidTrackingJson(obj, out TrackedObjectJsonEditor trackedObjectData, out byte[] data))
                            {
                                int index = TrackingItemArray.arraySize;
                                TrackingItemArray.InsertArrayElementAtIndex(index);
                                serializedObject.ApplyModifiedProperties();
                                var TrackingItemProperty = TrackingItemArray.GetArrayElementAtIndex(index);
                                TrackingItemProperty.FindPropertyRelative("jsonName").stringValue = obj.name + ".json";
                                TrackingItemProperty.FindPropertyRelative("JSONConfig").objectReferenceValue = obj;
                                TrackingItemProperty.FindPropertyRelative("jsonContent").stringValue = File.ReadAllText(AssetDatabase.GetAssetPath(obj));
                                var markerIDsProp = TrackingItemProperty.FindPropertyRelative("m_MarkerIDs");
                                var markerSizesProp = TrackingItemProperty.FindPropertyRelative("m_MarkerSizes");
                                var dataProp = TrackingItemProperty.FindPropertyRelative("m_Data");
                                //Single cards:
                                if (!trackedObjectData.IsCardGroup)
                                {
                                    markerIDsProp.arraySize = trackedObjectData.CARD_SINGLE.Markers.Length;
                                    markerSizesProp.arraySize = trackedObjectData.CARD_SINGLE.Markers.Length;
                                    TrackingItemProperty.FindPropertyRelative("m_TrackedAsGroup").boolValue = false;
                                    for (int i = 0; i < trackedObjectData.CARD_SINGLE.Markers.Length; i++)
                                    {
                                        markerIDsProp.GetArrayElementAtIndex(i).intValue = trackedObjectData.CARD_SINGLE.Markers[i];
                                        markerSizesProp.GetArrayElementAtIndex(i).floatValue = trackedObjectData.CARD_SINGLE.MarkersSize[i];
                                    }
                                }
                                //Group :
                                else
                                {
                                    markerIDsProp.arraySize = 1;
                                    markerSizesProp.arraySize = 1;
                                    TrackingItemProperty.FindPropertyRelative("m_TrackedAsGroup").boolValue = true;
                                    markerIDsProp.GetArrayElementAtIndex(0).intValue = trackedObjectData.CARD_GROUP.GroupID;
                                    markerSizesProp.GetArrayElementAtIndex(0).floatValue = trackedObjectData.CARD_GROUP.MarkersSize[0];
                                }

                                //Copy data:
                                dataProp.arraySize = data.Length;
                                for (int i = 0; i < data.Length; i++)
                                {
                                    dataProp.GetArrayElementAtIndex(i).intValue = data[i];
                                }
                                serializedObject.ApplyModifiedProperties();

                                RefreshJsonItems();
                            }

                        }
                    }
                    break;
            }
        }

        bool IsValidTrackingJson(UnityEngine.Object candidate, out TrackedObjectJsonEditor trackedObjectData, out byte[] data)
        {
            trackedObjectData = null;
            data = null;
            foreach (var item in this.mTarget.trackingItems)
            {
                if (item != null && item.JSONConfig == candidate)
                {
                    Debug.LogFormat("[RhinoX] Json tracking profile : {0} has already been added !", candidate.name);
                    return false;
                }
            }
            bool isJson = false;
            try
            {
                string jsonAssetPath = AssetDatabase.GetAssetPath(candidate);
                string txt = File.ReadAllText(AssetDatabase.GetAssetPath(candidate));
                TrackedObjectJsonEditor tJson = JsonUtility.FromJson<TrackedObjectJsonEditor>(txt);
                trackedObjectData = tJson;

                //Get data:

                string dataAssetPath = Path.Combine(Path.GetDirectoryName(jsonAssetPath), Path.GetFileNameWithoutExtension(jsonAssetPath) + ".dat");
                if (File.Exists(dataAssetPath))
                {
                    var bytes = File.ReadAllBytes(dataAssetPath);
                    data = bytes;
                }
                isJson = true;
            }
            catch
            {
                isJson = false;
            }
            return isJson;
        }

    }
}