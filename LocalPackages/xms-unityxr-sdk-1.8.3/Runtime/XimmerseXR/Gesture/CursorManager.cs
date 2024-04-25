using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ximmerse.XR.InputSystems.GazeAndGestureInteraction;

namespace Ximmerse.XR.InputSystems.GazeAndGestureInteraction
{
    public class CursorManager : MonoBehaviour
    {
        #region Property
        private static CursorManager instance;
        public static CursorManager Instance
        {
            get
            {
                return instance;
            }
        }
        private GameObject cursorGo;
        private GameObject _reticle;
        private Sprite _normal;
        private Sprite _tracking;
        private Sprite _none;

        public GameObject CursorGo
        {
            get => cursorGo;
        }
        public Transform MainCamera
        {
            get => xROrigin.Camera.transform;
        }
        public GameObject Reticle
        {
            get => _reticle;
        }

        public Sprite Normal
        {
            get => _normal;
            set => _normal = value;
        }
        public Sprite Tracking
        {
            get => _tracking;
            set => _tracking = value;
        }
        public Sprite None
        {
            get => _none;
            set => _none = value;
        }

        private XROrigin xROrigin;
        private GameObject reticle;

        #endregion

        #region Unity
        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //xROrigin = FindObjectOfType<XROrigin>();
            //if (GazeAndHandInteractionSystem.instance.normal == null)
            //{
            //    _normal = Resources.Load<Sprite>("Gesture/Texture/Normal");
            //}
            //else
            //{
            //    _normal = GazeAndHandInteractionSystem.instance.normal;
            //}
            //if (_tracking == null)
            //{
            //    _tracking = Resources.Load<Sprite>("Gesture/Texture/Tracking");
            //}
            //else
            //{
            //    _tracking = GazeAndHandInteractionSystem.instance.hovering;
            //}
            //if (_none == null)
            //{
            //    _none = Resources.Load<Sprite>("Gesture/Texture/None");
            //}
            //else
            //{
            //    _none = GazeAndHandInteractionSystem.instance.select;
            //}
            //cursorGo = Instantiate(Resources.Load("Gesture/Prefabs/Cursor"), xROrigin.Camera.transform) as GameObject;
            //_reticle = Instantiate(Resources.Load("Gesture/Prefabs/CURSOR0 (2)")) as GameObject;
            ////_normal = Resources.Load<Sprite>("Gesture/Texture/Normal");
            ////_tracking = Resources.Load<Sprite>("Gesture/Texture/Tracking");
            ////_none = Resources.Load<Sprite>("Gesture/Texture/None");

            //reticle = new GameObject("reticle");
            //reticle.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            //_reticle.transform.parent = reticle.transform;
            //reticle.AddComponent<CursorScale>();
            //_reticle.transform.eulerAngles = new Vector3(90, 0, 0);
            //_reticle.transform.localScale = new Vector3(1, 1, 1);
            //_reticle.transform.localPosition = new Vector3(0, 0.5f, 0);
            //GazeAndHandInteractionSystem.instance._eyeRay.GetComponent<XRInteractorLineVisual>().reticle = reticle;
        }
        private void Update()
        {
            cursorGo.SetActive(!reticle.activeSelf);
        }

        #endregion

        #region Method
        public void NormalCursor()
        {
            cursorGo.GetComponent<SpriteRenderer>().sprite = _normal;
        }
        public void HideCursor()
        {
            cursorGo.GetComponent<SpriteRenderer>().sprite = _none;
        }

        public void TrackingCursor()
        {
            cursorGo.GetComponent<SpriteRenderer>().sprite = _tracking;
        }

        public void NormalReticle()
        {
            _reticle.GetComponent<SpriteRenderer>().sprite = _normal;
        }

        public void TrackingReticle()
        {
            _reticle.GetComponent<SpriteRenderer>().sprite = _tracking;
        }

        public void HideReticle()
        {
            _reticle.GetComponent<SpriteRenderer>().sprite = _none;
        }
        #endregion
    }
}

