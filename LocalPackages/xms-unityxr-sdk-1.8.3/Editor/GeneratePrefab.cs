using UnityEngine;
using UnityEditor;
using Ximmerse.XR.InputSystems.GazeAndGestureInteraction;
using Unity.XR.CoreUtils;
using Ximmerse.XR;
using Ximmerse.XR.Tag;
using Ximmerse.XR.InputSystems;

public class GeneratePrefab
{
    [MenuItem("GameObject/Ximmerse XR/XR Origin (ActionBase)", false,0)]
    [MenuItem("Component/Ximmerse XR/XR Origin (ActionBase)", false, 0)]
    public static void GenerateXROriginActionBase()
    {
        GameObject go = GameObject.Instantiate(Resources.Load("XR Origin (ActionBase)")) as GameObject;
        go.name = "XR Origin (ActionBase)";
    }

    [MenuItem("GameObject/Ximmerse XR/XR Origin (Device Base)", false, 1)]
    [MenuItem("Component/Ximmerse XR/XR Origin (Device Base)", false, 1)]
    public static void GenerateXROriginDeviceBase()
    {
        GameObject go = GameObject.Instantiate(Resources.Load("XR Origin (Device Base)")) as GameObject;
        go.name = "XR Origin (Device Base)";
    }

    [MenuItem("GameObject/Ximmerse XR/Tag Profile Loading", false, 3)]
    [MenuItem("Component/Ximmerse XR/Tag Profile Loading", false, 3)]
    public static void GenerateTagLoading()
    {
        GameObject go = GameObject.Instantiate(Resources.Load("Tag/Prefabs/Tag Profile Loading")) as GameObject;
        go.name = "Tag Profile Loading";
    }

    [MenuItem("GameObject/Ximmerse XR/Creates Ground Plane By Json", false, 4)]
    [MenuItem("Component/Ximmerse XR/Creates Ground Plane By Json", false, 4)]
    public static void GenerateCreatesGroundPlane()
    {
        GameObject go = GameObject.Instantiate(Resources.Load("Tag/Prefabs/Creates Ground Plane By Json")) as GameObject;
        go.name = "Creates Ground Plane By Json";
    }

    [MenuItem("GameObject/Ximmerse XR/Tracking Target", false, 5)]
    [MenuItem("Component/Ximmerse XR/Tracking Target", false, 5)]
    public static void GenerateTagTracking()
    {
        TagTracking go = new GameObject("Tracking Target").AddComponent<TagTracking>();
    }

    [MenuItem("GameObject/Ximmerse XR/Ground Plane", false, 6)]
    [MenuItem("Component/Ximmerse XR/Ground Plane", false, 6)]
    public static void GenerateTagGround()
    {
        TagGroundPlane go = new GameObject("Ground Plane").AddComponent<TagGroundPlane>();
    }

    //[MenuItem("GameObject/Ximmerse XR/Gesture/EyeRay", false, 20)]
    //[MenuItem("Component/Ximmerse XR/Gesture/EyeRay", false, 20)]
    //public static void GenerateEyeRay()
    //{
    //    GameObject eyeRay = GameObject.Instantiate(Resources.Load("Gesture/Prefabs/Eye Ray")) as GameObject;
    //    eyeRay.name = "Eye Ray";
    //    eyeRay.transform.parent = Object.FindObjectOfType<XROrigin>().CameraFloorOffsetObject.transform;
    //}


    //[MenuItem("GameObject/Ximmerse XR/Gesture/Gaze And Hand Interaction System", false, 21)]
    //[MenuItem("Component/Ximmerse XR/Gesture/Gaze And Hand Interaction System", false, 21)]
    //public static void GenerateGazeAndHandInteractionSystem()
    //{
    //    GazeAndHandInteractionSystem go = new GameObject("Gaze And Hand Interaction System").AddComponent<GazeAndHandInteractionSystem>();
    //}

    [MenuItem("GameObject/Ximmerse XR/Gesture/Virtual Hand Model", false, 22)]
    [MenuItem("Component/Ximmerse XR/Gesture/Virtual Hand Model", false, 22)]
    public static void GenerateVirtualHandModel()
    {
        GameObject leftHand = GameObject.Instantiate(Resources.Load("Gesture/Prefabs/Virtual Hand Model - Left")) as GameObject;
        leftHand.name = "Virtual Hand Model - Left";
        GameObject rightHand = GameObject.Instantiate(Resources.Load("Gesture/Prefabs/Virtual Hand Model - Right")) as GameObject;
        rightHand.name = "Virtual Hand Model - Right";
    }

    [MenuItem("GameObject/Ximmerse XR/Gesture/Hand Anchor Interactor", false, 23)]
    [MenuItem("Component/Ximmerse XR/Gesture/Hand Anchor Interactor", false, 23)]
    public static void GenerateHandAnchorInteractor()
    {
        XROrigin xROrigin = GameObject.FindObjectOfType<XROrigin>();
        if (xROrigin!=null&& xROrigin.CameraFloorOffsetObject!=null)
        {
            GameObject leftAnchorInteractor = GameObject.Instantiate(Resources.Load("Gesture/Prefabs/Left Anchor Interactor"), xROrigin.CameraFloorOffsetObject.transform) as GameObject;
            leftAnchorInteractor.name = "Left Anchor Interactor";
            GameObject rightAnchorInteractor = GameObject.Instantiate(Resources.Load("Gesture/Prefabs/Right Anchor Interactor"), xROrigin.CameraFloorOffsetObject.transform) as GameObject;
            rightAnchorInteractor.name = "Right Anchor Interactor";
        }
        else
        {
            GameObject leftAnchorInteractor = GameObject.Instantiate(Resources.Load("Gesture/Prefabs/Left Anchor Interactor")) as GameObject;
            leftAnchorInteractor.name = "Left Anchor Interactor";
            GameObject rightAnchorInteractor = GameObject.Instantiate(Resources.Load("Gesture/Prefabs/Right Anchor Interactor")) as GameObject;
            rightAnchorInteractor.name = "Right Anchor Interactor";
        }

    }



    [MenuItem("GameObject/Ximmerse XR/Legacy Controller/Marker Controller InputSystem", false, 30)]
    [MenuItem("Component/Ximmerse XR/Legacy Controller/Marker Controller InputSystem", false, 30)]
    public static void GenerateMarkerControllerInputSystem()
    {
        MarkerControllerInputSystem go = new GameObject("Marker Controller InputSystem").AddComponent<MarkerControllerInputSystem>();
    }

    [MenuItem("GameObject/Ximmerse XR/Legacy Controller/Marker Controller Manager", false, 31)]
    [MenuItem("Component/Ximmerse XR/Legacy Controller/Marker Controller Manager", false, 31)]
    public static void GenerateMarkerControllerManager()
    {
        MarkerControllerManager go = new GameObject("Marker Controller Manager").AddComponent<MarkerControllerManager>();
    }

    //[MenuItem("GameObject/Ximmerse XR/Legacy Controller/Marker Controller 3Dof", false, 32)]
    //[MenuItem("Component/Ximmerse XR/Legacy Controller/Marker Controller 3Dof", false, 32)]
    //public static void GenerateMarkerController3dof()
    //{
    //    GameObject go = GameObject.Instantiate(Resources.Load("Controller/Prefabs/Marker Controller-3dof")) as GameObject;
    //    go.name = "Marker Controller 3Dof";
    //}

    [MenuItem("GameObject/Ximmerse XR/Legacy Controller/Marker Controller", false, 33)]
    [MenuItem("Component/Ximmerse XR/Legacy Controller/Marker Controller", false, 33)]
    public static void GenerateMarkerControllerTracking()
    {
        GameObject go = GameObject.Instantiate(Resources.Load("Controller/Prefabs/Marker Controller")) as GameObject;
        go.name = "Marker Controller";
    }
}