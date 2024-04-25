using UnityEditor;

namespace Ximmerse.XR.Tag
{
    [CustomEditor(typeof(TagProfileLoading))]
    public class TagProfileLoadingEditor : Editor
    {
        private SerializedObject tagProfileLoading;

        private SerializedProperty usageType;
        //private SerializedProperty isUseTracking;
        //private SerializedProperty parametersPath;
        private SerializedProperty dependentFilesPath;
        private SerializedProperty debug;
        private SerializedProperty Beacon;
        private SerializedProperty LiBeacon;
        private SerializedProperty TopoTag100_227;
        private SerializedProperty TopoTag228_407;
        private SerializedProperty SingleCard;
        private SerializedProperty DefaultTrackingMarker;
        private SerializedProperty specialTrackingMarker;
        private SerializedProperty TopoTagSize_100_227;
        private SerializedProperty TopoTagSize_228_407;
        private SerializedProperty LiBeaconType;

        void OnEnable()
        {
            tagProfileLoading = new SerializedObject(target);

            usageType = tagProfileLoading.FindProperty("usageType");
            //isUseTracking = tagProfileLoading.FindProperty("isUseTracking");
            //parametersPath = tagProfileLoading.FindProperty("parametersPath");
            dependentFilesPath = tagProfileLoading.FindProperty("dependentFilesPath");
            debug = tagProfileLoading.FindProperty("m_debug");
            Beacon = tagProfileLoading.FindProperty("Beacon");
            LiBeacon = tagProfileLoading.FindProperty("LiBeacon");
            TopoTag100_227 = tagProfileLoading.FindProperty("TopoTag100_227");
            TopoTag228_407 = tagProfileLoading.FindProperty("TopoTag228_407");
            SingleCard = tagProfileLoading.FindProperty("SingleCard");
            DefaultTrackingMarker = tagProfileLoading.FindProperty("DefaultTrackingMarker");
            specialTrackingMarker = tagProfileLoading.FindProperty("specialTrackingMarker");
            TopoTagSize_100_227 = tagProfileLoading.FindProperty("topoTagSize100_227");
            TopoTagSize_228_407 = tagProfileLoading.FindProperty("topoTagSize228_407");
            LiBeaconType = tagProfileLoading.FindProperty("liBeaconType");
        }

        public override void OnInspectorGUI()
        {
            tagProfileLoading.Update();
            SerializedProperty property = tagProfileLoading.GetIterator();
            while (property.NextVisible(true))
            {
                using (new EditorGUI.DisabledScope("m_Script" == property.propertyPath))
                {
                    EditorGUILayout.PropertyField(property, true);
                    break;
                }
            }

            EditorGUILayout.PropertyField(usageType);



            if (usageType.enumValueIndex==1)
            {

                EditorGUILayout.PropertyField(DefaultTrackingMarker);
                EditorGUILayout.PropertyField(specialTrackingMarker);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(Beacon);
                EditorGUILayout.PropertyField(LiBeacon);
                if (LiBeacon.boolValue)
                {
                    EditorGUILayout.PropertyField(LiBeaconType);
                }
                EditorGUILayout.PropertyField(TopoTag100_227);
                if (TopoTag100_227.boolValue)
                {
                    EditorGUILayout.PropertyField(TopoTagSize_100_227);
                }
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(TopoTag228_407);
                if (TopoTag228_407.boolValue)
                {
                    EditorGUILayout.PropertyField(TopoTagSize_228_407);
                }
                EditorGUILayout.PropertyField(SingleCard);



                //if (LiBeacon.boolValue && AllTrackingMarker.boolValue)
                //{
                //    UnityEditor.EditorUtility.DisplayDialog("Error", "Tag type conflict", "Confirm");

                //    LiBeacon.boolValue = false;
                //    AllTrackingMarker.boolValue = false;

                //}

                if (SingleCard.boolValue && DefaultTrackingMarker.boolValue)
                {
                    UnityEditor.EditorUtility.DisplayDialog("Error", "Tag id conflict", "Confirm");

                    SingleCard.boolValue = false;
                    DefaultTrackingMarker.boolValue = false;
                }

                if (LiBeacon.boolValue && specialTrackingMarker.boolValue)
                {
                    UnityEditor.EditorUtility.DisplayDialog("Error", "Tag id conflict", "Confirm");

                    LiBeacon.boolValue = false;
                    specialTrackingMarker.boolValue = false;
                }

            }
            else
            {
                EditorGUILayout.PropertyField(DefaultTrackingMarker);
                EditorGUILayout.PropertyField(specialTrackingMarker);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(dependentFilesPath);
                //EditorGUILayout.PropertyField(parametersPath);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(debug);
            }



            tagProfileLoading.ApplyModifiedProperties();
        }
    }
}


