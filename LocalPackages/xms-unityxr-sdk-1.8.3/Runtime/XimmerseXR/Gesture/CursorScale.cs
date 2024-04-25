using UnityEngine;

namespace Ximmerse.XR.InputSystems.GazeAndGestureInteraction
{
    public class CursorScale : MonoBehaviour
    {
        #region Property
        private Transform Target;
        private GameObject CamCursor;
        private float ScaleFactor;

        private Vector3 OriginScale;
        #endregion

        #region Unity
        private void Start()
        {
            CamCursor = CursorManager.Instance.CursorGo;
            OriginScale = CamCursor.transform.localScale;
            Target = CursorManager.Instance.MainCamera;
            ScaleFactor = Vector3.Distance(CamCursor.transform.position, Target.position);
        }

        void Update()
        {
            float distance = Vector3.Distance(transform.position, Target.position);
            Vector3 scale = distance / ScaleFactor * OriginScale;
            transform.localScale = scale;
        }
        #endregion
    }
}

