using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Ximmerse.XR.Tag
{
    [CustomEditor(typeof(GroundPlaneLayoutConfiguration))]
    public class GroundPlaneLayoutConfigurationEditor : Editor
    {
        GroundPlaneLayoutConfiguration tScript
        {
            get => this.target as GroundPlaneLayoutConfiguration;
        }

        private void OnEnable()
        {

        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Export to JSON"))
            {
                string json = (JsonUtility.ToJson(tScript.layout, true));
                string path = EditorUtility.SaveFilePanel("Save Json", "Assets", "groundplane-layout", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    File.WriteAllText(path, json);
                    Debug.Log(json);
                    Debug.LogFormat("Writes to {0}", path);
                }
            }
            if (GUILayout.Button("Import from JSON"))
            {
                string file = EditorUtility.OpenFilePanel("Open GroundPlane layout json file", "Assets", "json");
                if (!string.IsNullOrEmpty(file))
                {
                    var layout = JsonUtility.FromJson<GroundPlaneLayout>(File.ReadAllText(file));
                    tScript.layout = layout;
                    EditorUtility.SetDirty(tScript);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
