using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Ximmerse.XR
{
    [CustomEditor(typeof(XMR_Manager))]
    public class XMRManagerEditor : Editor
    {
        private SerializedObject xrManager;

        private SerializedProperty handTracking;
        private SerializedProperty HandTrackingConfigType;
        private SerializedProperty handTrackDataSource;

        private SerializedProperty MRCFirstPersonRealityCamera;
        private SerializedProperty MRCThirdPersonCamera;
        private SerializedProperty MRCThirdPersonObject;


        private SerializedProperty bodyTracking;

        private SerializedProperty displayOptimization;
        private SerializedProperty displayOptimizationConfigType;
        private SerializedProperty useSubsample;
        private SerializedProperty foveatedLevel;
        private SerializedProperty ASWMode;
        private SerializedProperty PTWMode;
        private SerializedProperty WarpVertexMode;

        private SerializedProperty tagRecognition;
        private SerializedProperty upCameraTracking;

        void OnEnable()
        {
            xrManager = new SerializedObject(target);

            handTracking = xrManager.FindProperty("handTracking");
            //HandTrackingConfigType = xrManager.FindProperty("HandTrackingConfigType");

            handTrackDataSource = xrManager.FindProperty("handTrackDataSource");

            MRCFirstPersonRealityCamera = xrManager.FindProperty("MRCFirstPersonRealityCamera");
            MRCThirdPersonCamera = xrManager.FindProperty("MRCThirdPersonCamera");
            MRCThirdPersonObject = xrManager.FindProperty("MRCThirdPersonObject");

            bodyTracking = xrManager.FindProperty("bodyTracking");

            //displayOptimization = xrManager.FindProperty("displayOptimization");
            //displayOptimizationConfigType = xrManager.FindProperty("displayOptimizationConfigType");
            useSubsample = xrManager.FindProperty("useSubsample");
            foveatedLevel = xrManager.FindProperty("foveatedLevel");
            ASWMode = xrManager.FindProperty("ASWMode");
            PTWMode = xrManager.FindProperty("PTWMode");
            WarpVertexMode = xrManager.FindProperty("WarpVertexMode");

            tagRecognition = xrManager.FindProperty("tagRecognition");
            upCameraTracking = xrManager.FindProperty("upCameraTracking");

        }

        public override void OnInspectorGUI()
        {
            xrManager.Update();
            SerializedProperty property = xrManager.GetIterator();
            while (property.NextVisible(true))
            {
                using (new EditorGUI.DisabledScope("m_Script" == property.propertyPath))
                {
                    EditorGUILayout.PropertyField(property, true);
                    break;
                }
            }

            #region Renderer
            //EditorGUILayout.PropertyField(displayOptimization);
            //if (displayOptimization.boolValue)
            //{
            //EditorGUILayout.PropertyField(displayOptimizationConfigType);

            //if (displayOptimizationConfigType.enumValueIndex == 1)
            //{

            EditorGUILayout.PropertyField(foveatedLevel);
            if (foveatedLevel.enumValueIndex!=0)
            {
                EditorGUILayout.PropertyField(useSubsample);
            }

            EditorGUILayout.PropertyField(ASWMode);
            EditorGUILayout.PropertyField(PTWMode);
            //EditorGUILayout.PropertyField(WarpVertexMode);
            //}
            //}


            EditorGUILayout.Space();
            #endregion

            #region Hand Tracking

            EditorGUILayout.PropertyField(handTracking);

            if (handTracking.boolValue)
            {
                //EditorGUILayout.PropertyField(HandTrackingConfigType);

                //if (HandTrackingConfigType.enumValueIndex == 1)
                //{
                    EditorGUILayout.PropertyField(handTrackDataSource);
                //}
            }



            EditorGUILayout.Space();
            #endregion


            #region MRC
            EditorGUILayout.PropertyField(MRCFirstPersonRealityCamera);
            //EditorGUILayout.PropertyField(MRCThirdPersonObject);
            #endregion

            //EditorGUILayout.PropertyField(bodyTracking);



            #region Tag Recognition
            EditorGUILayout.PropertyField(tagRecognition);

            if (tagRecognition.boolValue)
            {
                EditorGUILayout.PropertyField(upCameraTracking);
            }

            #endregion

            xrManager.ApplyModifiedProperties();
        }
    }
}

