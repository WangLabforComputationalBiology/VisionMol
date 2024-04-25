using UnityEditor;
using Ximmerse.XR.InputSystems.GazeAndGestureInteraction;

[CustomEditor(typeof(GazeAndHandInteractionSystem))]
public class GazeEditor : Editor
{

    private SerializedObject gazeimage;
    private SerializedProperty EyeRayGO;
    private SerializedProperty cursor;
    private SerializedProperty normal;
    private SerializedProperty tracking;
    private SerializedProperty select;
    private SerializedProperty EyeRay;

    void OnEnable()
    {
        gazeimage = new SerializedObject(target);

        EyeRayGO = gazeimage.FindProperty("EyeRayGO");
        cursor = gazeimage.FindProperty("_cursorStateImage");
        normal = gazeimage.FindProperty("normal");
        tracking = gazeimage.FindProperty("tracking");
        select = gazeimage.FindProperty("select");
        EyeRay= gazeimage.FindProperty("_eyeRay");
    }

    public override void OnInspectorGUI()
    {
        gazeimage.Update();

        EditorGUILayout.PropertyField(cursor);

        if (cursor.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(EyeRayGO);
            //EditorGUILayout.PropertyField(cursor);
        }
        else
        {
            EditorGUILayout.PropertyField(normal);
            EditorGUILayout.PropertyField(tracking);
            EditorGUILayout.PropertyField(select);
            EditorGUILayout.PropertyField(EyeRayGO);
        }
        if (EyeRayGO.enumValueIndex!=0)
        {
            EditorGUILayout.PropertyField(EyeRay);
        }
        gazeimage.ApplyModifiedProperties();
    }
}