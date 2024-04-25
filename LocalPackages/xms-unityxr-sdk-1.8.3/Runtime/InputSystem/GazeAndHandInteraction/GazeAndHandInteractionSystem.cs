using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
namespace Ximmerse.XR.InputSystems.GazeAndGestureInteraction
{
    /// <summary>
    /// Gaze and hand interaction system manages how XR user interacts world objects withe eye reticle and hand gesture.
    /// </summary>
    public partial class GazeAndHandInteractionSystem : MonoBehaviour
    {
        /// <summary>
        /// The eye ray
        /// </summary>
        [SerializeField]
        XRRayInteractor m_eyeRay;

        /// <summary>
        /// The eye ray interactor.
        /// </summary>
        public XRRayInteractor eyeRay
        {
            get => m_eyeRay;
            set => m_eyeRay = value;
        }

        /// <summary>
        /// The sprite renderer to render cursor icon.
        /// </summary>
        public SpriteRenderer cursor;

        /// <summary>
        /// Normal icon when the gaze ray is not hovering 
        /// </summary>
        public Sprite normal;

        /// <summary>
        /// Hovering icon when the gaze ray is hovering
        /// </summary>
        public Sprite hovering;

        /// <summary>
        /// Icon when the gaze ray is interacting.
        /// </summary>
        public Sprite select;

        public static GazeAndHandInteractionSystem instance
        {
            get; private set;
        }

        public EyeReticle eyeReticle
        {
            get;
        }

        private void Awake()
        {
            instance = this;
        }

        static Camera sMainCamera;

        TrackState m_TrackingState = new TrackState();

        UIObjectsInteractionState m_UIInteractionState = new UIObjectsInteractionState();

        /// <summary>
        /// Hand tracking state.
        /// </summary>
        public I_HandTrackingState TrackingState
        {
            get => m_TrackingState;
        }

        public enum CursorStateImage
        {
            Default,
            Custom,
        }
        public enum EyeRayGameobject
        {
            Default,
            Custom,
        }
        public CursorStateImage _cursorStateImage = CursorStateImage.Default;

        public EyeRayGameobject EyeRayGO = EyeRayGameobject.Default;


        private GestureXRInteractionManager _gestureXRInteractionManager;

        /// <summary>
        /// m_IsHoveringUIObject : 是否在某个UI对象上悬停;
        /// m_IsHoveringWorldObject : 是否在某个 World Object 对象上悬停
        /// </summary>
        private bool m_IsHoveringUIObject, m_IsHoveringWorldObject;

        /// <summary>
        /// 正在悬停和正在与之交互的UI对象。
        /// </summary>
        private Component m_HoveringUIObject, m_InteractingUIObject;


        // Update is called once per frame
        void Update()
        {
            UpdateInteractionInfo(); //更新交互状态
            UpdateCursorIcons();//更新图标
            OnInteractingWithUI();//处理UI交互逻辑
        }

        /// <summary>
        /// Get eye reticle local pose to main camera
        /// </summary>
        /// <returns></returns>
        public static bool GetEyeReticleLocalPose(out Pose pose)
        {
            if (!sMainCamera)
            {
                sMainCamera = Camera.main;
                if (!sMainCamera)
                {
                    pose = default(Pose);
                    return false;
                }
            }
            pose = new Pose(Vector3.zero, Quaternion.identity);
            if (sMainCamera.transform.parent)
            {
                pose.position = sMainCamera.transform.parent.InverseTransformPoint(sMainCamera.transform.position);
                pose.rotation = Quaternion.Inverse(sMainCamera.transform.parent.rotation) * (sMainCamera.transform.rotation);
            }
            else
            {
                pose.position = sMainCamera.transform.position;
                pose.rotation = sMainCamera.transform.rotation;
            }
            return true;
        }


        /// <summary>
        /// 更新交互信息.
        /// </summary>
        private void UpdateInteractionInfo()
        {
            if (!m_eyeRay)
            {
                return;
            }

            var _eyeRay = m_eyeRay;
            bool _isHoveringUI = _eyeRay.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult);
            bool _isHoveringInteractableUI = false;
            Component _hoveringUIComponent = null;
            if (_isHoveringUI && raycastResult.gameObject)
            {
                _hoveringUIComponent = raycastResult.gameObject.GetComponentInParent<Selectable>();
                _isHoveringInteractableUI = _isHoveringUI && _hoveringUIComponent != null;
            }

            this.m_IsHoveringUIObject = _isHoveringInteractableUI;
            this.m_HoveringUIObject = _hoveringUIComponent;
            this.m_IsHoveringWorldObject = _eyeRay.interactablesHovered != null && _eyeRay.interactablesHovered.Count > 0;

            //更新 m_InteractingUIObject 引用:
            var gazeInput = XimmerseXRGazeInput.gazeInput;
            //Is selecting:
            if (gazeInput.gazeSelect.ReadValue() > 0)
            {
                if(m_HoveringUIObject)
                {
                    m_InteractingUIObject = m_HoveringUIObject;
                }
            }
            //no selecting:
            else
            {
                m_InteractingUIObject = null;//当用户没有做select操作的时候，清空此字段.
            }
        }

        /// <summary>
        /// 更新cursor icon
        /// </summary>
        private void UpdateCursorIcons()
        {
            if (!cursor)
            {
                return;
            }
            if (this.m_IsHoveringUIObject || m_IsHoveringWorldObject || m_InteractingUIObject)
            {
                var gazeInput = XimmerseXRGazeInput.gazeInput;
                bool isSelecting = gazeInput.gazeSelect.ReadValue() != 0;
                bool isGazeInteracting = isSelecting || gazeInput.isInteracting.ReadValue() != 0;

                //Confirm 是最高优先级:
                if (isSelecting)
                {
                    cursor.sprite = this.select;
                }
                //Gaze interacting 是次高优先级:
                else if (isGazeInteracting)
                {
                    cursor.sprite = this.select;
                }
                //最低优先级: hovering
                else
                {
                    cursor.sprite = this.hovering;
                }
            }
            else
            {
                cursor.sprite = this.normal;
            }
        }

        /// <summary>
        /// 处理 ui 组件交互逻辑。 目前只支持 slider
        /// </summary>
        private void OnInteractingWithUI()
        {
            if (!m_eyeRay)
            {
                return;
            }
            if (m_InteractingUIObject && m_InteractingUIObject is Slider)
            {
                var gazeInput = XimmerseXRGazeInput.gazeInput;
                Vector3 gestureDragVelocity = gazeInput.gestureDragVelocity.ReadValue();
                if (gestureDragVelocity.sqrMagnitude >= 0.01f)
                {
                    var slider = m_InteractingUIObject as Slider;
                    float velocity = 0;
                    const float rate = 0.2f;
                    switch (slider.direction)
                    {
                        case Slider.Direction.LeftToRight:
                            velocity = gestureDragVelocity.x;
                            break;

                        case Slider.Direction.RightToLeft:
                            velocity = -gestureDragVelocity.x;
                            break;

                        case Slider.Direction.BottomToTop:
                            velocity = gestureDragVelocity.y;
                            break;

                        case Slider.Direction.TopToBottom:
                            velocity = -gestureDragVelocity.y;
                            break;
                    }
                    slider.normalizedValue = Mathf.Clamp01(slider.normalizedValue + velocity * rate);
                    //Debug.LogFormat("Dragging slider UI at velocity: {0}", velocity.ToString("F3"));
                }
                //Debug.LogFormat("Dragging slider UI: {0}/{1}", gestureDragVelocity.sqrMagnitude, gestureDragVelocity.ToString("F3"));
            }
        }
    }
}
