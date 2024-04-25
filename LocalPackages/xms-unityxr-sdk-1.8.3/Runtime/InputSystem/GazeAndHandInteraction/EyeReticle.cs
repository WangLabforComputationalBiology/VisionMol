using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.EventSystems;

namespace Ximmerse.XR.InputSystems.GazeAndGestureInteraction
{
    /// <summary>
    /// Eye reticle - 注视点交互射线控制器。
    /// 需要场景中存在 XRUIInputModule 组件。
    /// </summary>
    [DisallowMultipleComponent]
    public class EyeReticle : XRRayInteractor
    {
        public class InteractingObjectDescription
        {
            public GameObject target;

            public bool isUI;
        }
        /// <summary>
        /// If true, eye reticle cusor texture is not displayed when interacting nothing.
        /// </summary>
        public bool DisplayEyeReticleCursorWhenIdle = true;

        ///// <summary>
        ///// The default reticle cursor texture.
        ///// </summary>
        //public Texture2D defaultReticleTexture;

        ///// <summary>
        ///// The reticle cursor texture when eye reticle hover interactable objects.
        ///// </summary>
        //public Texture2D hoverReticleTexture;

        ///// <summary>
        ///// The reticle cursor texture  when eye reticle select interactable object.
        ///// </summary>
        //public Texture2D selectStateReticleTexture;

        XRUIInputModule m_CurrentXRInputModule;

        public enum EyeReticleTransformMode : byte
        {
            LockToHead = 0,

            LockToTarget,
        }

        /// <summary>
        /// The current transform mode of the eye reticle object.
        /// </summary>
        public EyeReticleTransformMode TransformMode
        {
            get; private set;
        }

        /// <summary>
        /// 当前 raycast hovering 目标对象
        /// </summary>
        public InteractingObjectDescription CurrentHoveringTarget
        {
            get; private set;
        }

        /// <summary>
        /// 当前交互中的目标对象
        /// </summary>
        public InteractingObjectDescription CurrentInteractingTarget
        {
            get; private set;
        }

        private GameObject target;
        //public GameObject plam;
        Vector3 targetaxis;

        private bool _select = false;


        float defaultRaycastingDistance = 0;

        protected override void Awake()
        {
            base.Awake();
            CurrentHoveringTarget = new InteractingObjectDescription();
            CurrentInteractingTarget = new InteractingObjectDescription();
            defaultRaycastingDistance = this.maxRaycastDistance;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!m_CurrentXRInputModule)
            {
                m_CurrentXRInputModule = FindObjectOfType<XRUIInputModule>();
            }
            //UI:
            if (m_CurrentXRInputModule)
            {
                m_CurrentXRInputModule.pointerEnter += onXRUIInputPointerEnter;
                m_CurrentXRInputModule.pointerExit += onXRUIInputPointerExit;

                m_CurrentXRInputModule.pointerDown += onXRUIInputPointerDown;
                m_CurrentXRInputModule.pointerUp += onXRUIInputPointerUp;
            }
            //3D object:
            XimmerseXR.DisplayReticle = this.DisplayEyeReticleCursorWhenIdle;
            this.hoverEntered.AddListener(OnFirstHoverEntered);
            this.hoverExited.AddListener(OnLastHoverExited);
            this.selectEntered.AddListener(OnFirstSelectEntered);
            this.selectExited.AddListener(OnLastSelectExited);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            //UI:
            if (m_CurrentXRInputModule)
            {
                m_CurrentXRInputModule.pointerEnter -= onXRUIInputPointerEnter;
                m_CurrentXRInputModule.pointerExit -= onXRUIInputPointerExit;

                m_CurrentXRInputModule.pointerDown -= onXRUIInputPointerDown;
                m_CurrentXRInputModule.pointerUp -= onXRUIInputPointerUp;
            }

            //3D object:
            this.hoverEntered.RemoveListener(OnFirstHoverEntered);
            this.hoverExited.RemoveListener(OnLastHoverExited);
            this.selectEntered.RemoveListener(OnFirstSelectEntered);
            this.selectExited.RemoveListener(OnLastSelectExited);
        }

        #region Pointer Enter/Exit (UI), Hover Enter/Exit(3D Object)

        private void onXRUIInputPointerEnter(GameObject targetGameObject, PointerEventData pointerEvent)
        {
            Component interactionComponent = null;
            if (pointerEvent is TrackedDeviceEventData)
            {
                TrackedDeviceEventData tracEvntDta = pointerEvent as TrackedDeviceEventData;
                interactionComponent = tracEvntDta.interactor as Component;
            }
            if (ReferenceEquals(this, interactionComponent))
            {
                OnEyeRayEnterInternal(targetGameObject, true); //This event called when with UI object
            }
            // Debug.LogFormat("Raycaster: {0} enter: {1}", interactionComponent ? interactionComponent.name : "", targetGameObject.name);
        }

        private void onXRUIInputPointerExit(GameObject targetGameObject, PointerEventData pointerEvent)
        {
            Component interactionComponent = null;
            if (pointerEvent is TrackedDeviceEventData)
            {
                TrackedDeviceEventData tracEvntDta = pointerEvent as TrackedDeviceEventData;
                interactionComponent = tracEvntDta.interactor as Component;
            }
            if (ReferenceEquals(this, interactionComponent))
            {
                OnEyeRayExitInternal(targetGameObject, true); //This event called when with UI object
            }
            // Debug.LogFormat("Raycaster: {0} exit: {1}", interactionComponent ? interactionComponent.name : "", targetGameObject);
        }

        


        /// <summary>
        /// Hover enter : 显示reticle texture，示意此物体可以交互.
        /// 此方法在进入3D object的时候回调。等同于 UI 对象的 Pointer Enter
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnFirstHoverEntered(HoverEnterEventArgs args)
        {
            
            OnEyeRayEnterInternal(args.interactableObject.transform.gameObject, false); //This event called when with 3D object

            // Debug.LogFormat(args.interactorObject.transform, "On First Hover Enter: {0}", args.interactorObject.transform.name);
        }
        /// <summary>
        /// Hover exit :  hide / switch reticle texture，示意已经离开交互区间
        /// 此方法在离开 3D object的时候回调。等同于 UI 对象的 Pointer Exit
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnLastHoverExited(HoverExitEventArgs args)
        {
            OnEyeRayExitInternal(args.interactableObject.transform.gameObject, false);
            
            // Debug.LogFormat(args.interactorObject.transform, "On Last Hover Enter: {0}", args.interactorObject.transform.name);
        }


        private void OnEyeRayEnterInternal(GameObject target, bool isUI)
        {
            this.CurrentHoveringTarget.target = target;
            this.CurrentHoveringTarget.isUI = isUI;
        }

        private void OnEyeRayExitInternal(GameObject target, bool isUI)
        {
            this.CurrentHoveringTarget.target = null;
            this.CurrentHoveringTarget.isUI = isUI;
        }

        #endregion


        #region Pointer Down/Up

        /// <summary>
        /// On GUI pointer down
        /// </summary>
        /// <param name="targetGameObject"></param>
        /// <param name="pointerEvent"></param>
        private void onXRUIInputPointerDown(GameObject targetGameObject, PointerEventData pointerEvent)
        {
            _select = true;
            Component interactionComponent = null;
            if (pointerEvent is TrackedDeviceEventData)
            {
                TrackedDeviceEventData tracEvntDta = pointerEvent as TrackedDeviceEventData;
                interactionComponent = tracEvntDta.interactor as Component;
            }
            if (ReferenceEquals(this, interactionComponent))
            {
                OnEyeReticlePointerDown(targetGameObject, true); //This event called when with UI object
            }
        }

        /// <summary>
        /// On GUI pointer up
        /// </summary>
        /// <param name="targetGameObject"></param>
        /// <param name="pointerEvent"></param>
        private void onXRUIInputPointerUp(GameObject targetGameObject, PointerEventData pointerEvent)
        {
            _select = false;
            Component interactionComponent = null;
            if (pointerEvent is TrackedDeviceEventData)
            {
                TrackedDeviceEventData tracEvntDta = pointerEvent as TrackedDeviceEventData;
                interactionComponent = tracEvntDta.interactor as Component;
            }
            if (ReferenceEquals(this, interactionComponent))
            {
                OnEyeReticlePointerUp(targetGameObject, true); //This event called when with UI object
            }
        }
        
        protected void OnFirstSelectEntered(SelectEnterEventArgs args)
        {
            Debug.Log("OnFirstSelectEntered");
            OnEyeReticlePointerDown(args.interactorObject.transform.gameObject, false);
            Debug.LogFormat(args.interactorObject.transform, "On First Select Enter: {0}", args.interactorObject.transform.name);
        }

        protected void OnLastSelectExited(SelectExitEventArgs args)
        {
            OnEyeReticlePointerUp(args.interactorObject.transform.gameObject, false);
            Debug.LogFormat(args.interactorObject.transform, "On Last Select Exit: {0}", args.interactorObject.transform.name);
        }

        bool isselect = true;

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            _select = true;
            base.OnSelectEntering(args);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            _select = false;
            base.OnSelectExited(args);
        }
        private bool _hover;
        protected override void OnHoverEntering(HoverEnterEventArgs args)
        {
            base.OnHoverEntering(args);
        }
        
        protected override void OnHoverExiting(HoverExitEventArgs args)
        {
            base.OnHoverExiting(args);
        }

        private void OnEyeReticlePointerDown(GameObject target, bool isUI)
        {
            _select = true;
            if (target && isUI)
            {
                this.CurrentInteractingTarget.target = target;
                this.CurrentInteractingTarget.isUI = isUI;
                if (isUI)
                {
                    //SetTransformMode(EyeReticleTransformMode.LockToTarget);
                }
                Debug.LogFormat("On Eye Reticle Pointer down at UI :{0}", target);
            }
        }

        private void OnEyeReticlePointerUp(GameObject target, bool isUI)
        {
            _select = false;
            this.CurrentInteractingTarget.target = null;
        }

        #endregion

        //internal void SetTransformMode(EyeReticleTransformMode mode)
        //{
        //    this.TransformMode = mode;
        //    switch (mode)
        //    {
        //        case EyeReticleTransformMode.LockToHead:
        //            //anchor to head transform:
        //            transform.SetParent(Camera.main.transform, true);
        //            transform.localPosition = Vector3.zero;
        //            transform.localRotation = Quaternion.identity;
        //            Debug.LogFormat("EyeReticle : lock to head !");
        //            break;

        //        case EyeReticleTransformMode.LockToTarget:
        //            var targetTransform = CurrentInteractingTarget.target.transform;
        //            if (targetTransform)
        //            {
        //                transform.position = targetTransform.position + -targetTransform.forward;
        //                transform.forward = targetTransform.forward;
        //                transform.SetParent(targetTransform, true);
        //                Debug.LogFormat("EyeReticle : lock to target UI !");
        //            }
        //            else
        //            {
        //                //unsupport
        //            }

        //            break;
        //    }
        //}

        private void LateUpdate()
        {
            var currentHandGestureTrackingState = GazeAndHandInteractionSystem.instance.TrackingState;

            //无手势数据的时候，清空交互对象引用:
            if (!currentHandGestureTrackingState.IsTracking || CurrentInteractingTarget.target)
            {
                CurrentInteractingTarget.target = null;
            }
            if (HandTracking.LeftHandTrackInfo.IsValid)
            {
                if (_select)
                {
                    SelectState();
                }
                else if (this.CurrentHoveringTarget.target)
                {
                    HoverState();
                }
                else
                {
                    TrackingState();
                }
            }
            else
            {
                if (this.CurrentHoveringTarget.target)
                {
                    CursorManager.Instance.HideCursor();
                    CursorManager.Instance.NormalReticle();
                }
                else
                {
                    NormalState();
                }
            }


            XimmerseXR.DisplayReticle = true;
        }

        #region cursorstate
        private void TrackingState()
        {
            CursorManager.Instance.TrackingCursor();
        }
        private void NormalState()
        {
            CursorManager.Instance.NormalCursor();
            //CursorManager.Instance.HideReticle();
        }
        private void SelectState()
        {
            CursorManager.Instance.HideCursor();
            CursorManager.Instance.HideReticle();
        }
        private void HoverState()
        {
            CursorManager.Instance.HideCursor();
            CursorManager.Instance.TrackingReticle();
        }
        #endregion
    }
}