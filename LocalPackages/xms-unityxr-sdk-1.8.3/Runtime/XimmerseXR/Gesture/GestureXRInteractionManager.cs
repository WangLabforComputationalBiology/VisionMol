using Unity.XR.CoreUtils;
using UnityEngine;

namespace Ximmerse.XR.InputSystems.GazeAndGestureInteraction
{
    public class GestureXRInteractionManager : MonoBehaviour
    {
        #region Property
        private GameObject _eyeray;
        private CursorManager _cursorManager;
        private XROrigin _xrorigin;
        public CursorManager CursorManagergo
        {
            get => _cursorManager;
        }
        #endregion
        private void Awake()
        {
            //_eyeray = FindObjectOfType<EyeReticle>().gameObject;
            //if (GazeAndHandInteractionSystem.instance.EyeRayGO == GazeAndHandInteractionSystem.EyeRayGameobject.Default)
            //{
            //    if (_eyeray != null)
            //    {
            //        Destroy(_eyeray);
            //    }
            //    if (GazeAndHandInteractionSystem.instance._eyeRay == null)
            //    {
            //        _eyeray = GameObject.Instantiate(Resources.Load("Gesture/Prefabs/Eye Ray")) as GameObject;
            //        _eyeray.name = "Eye Ray";
            //        GazeAndHandInteractionSystem.instance._eyeRay = _eyeray;
            //        _xrorigin = FindObjectOfType<XROrigin>();
            //        _eyeray.transform.parent = _xrorigin.CameraFloorOffsetObject.transform;
            //        _eyeray.transform.localPosition = new Vector3(0, 0, 0);
            //    }
            //    else if (GazeAndHandInteractionSystem.instance._eyeRay != null)
            //    {
            //        _eyeray = GazeAndHandInteractionSystem.instance._eyeRay;
            //    }
            //}
            //else
            //{
            //    if (_eyeray != null && GazeAndHandInteractionSystem.instance._eyeRay == null)
            //    {
            //        GazeAndHandInteractionSystem.instance._eyeRay = _eyeray;
            //    }
            //    else if (_eyeray == null && GazeAndHandInteractionSystem.instance._eyeRay == null)
            //    {
            //        _eyeray = GameObject.Instantiate(Resources.Load("Gesture/Prefabs/Eye Ray")) as GameObject;
            //        _eyeray.name = "Eye Ray";
            //        GazeAndHandInteractionSystem.instance._eyeRay = _eyeray;
            //        _xrorigin = FindObjectOfType<XROrigin>();
            //        _eyeray.transform.parent = _xrorigin.CameraFloorOffsetObject.transform;
            //        _eyeray.transform.localPosition = new Vector3(0, 0, 0);
            //    }
            //    else
            //    {
            //        _eyeray = GazeAndHandInteractionSystem.instance._eyeRay;
            //    }
            //}
            //_cursorManager = new GameObject("CursorManager").AddComponent<CursorManager>();
        }
    }
}

