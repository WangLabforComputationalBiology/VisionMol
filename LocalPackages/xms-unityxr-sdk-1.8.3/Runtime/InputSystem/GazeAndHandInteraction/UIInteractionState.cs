using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Ximmerse.XR.InputSystems.GazeAndGestureInteraction
{
    /// <summary>
    /// Interface : interaction with objects
    /// </summary>
    public interface I_InteractionState
    {
        bool IsEnabled
        {
            get;
        }

        void OnEnable();

        void OnDisable();

        void OnReticleEnter();

        void Tick();

        void OnReticleExit();
    }
    class LockedGameObjectInfo
    {
        public Object lockedReference;

        public enum LockType
        {
            None = 0,
            /// <summary>
            /// Lock interaction target for slider UI
            /// </summary>
            SliderUI,
            /// <summary>
            /// Lock interaction target for 3d GameObject
            /// </summary>
            GameObject,
        }

        public LockType lockType = LockType.None;

        public float lockTime = 0;
    }
    /// <summary>
    /// Gaze and hand interaction system manages how XR user interacts world objects withe eye reticle and hand gesture.
    /// </summary>
    public partial class GazeAndHandInteractionSystem
    {
        /// <summary>
        /// UI objects interaction state.
        /// </summary>
        internal class UIObjectsInteractionState : I_InteractionState
        {
            public bool IsEnabled
            {
                get; private set;
            }

            Transform mainCam;



            LockedGameObjectInfo lockInfo = new LockedGameObjectInfo();

            public void OnEnable()
            {

            }

            public void OnDisable()
            {

            }
            public void OnReticleEnter()
            {
            }


            public void Tick()
            {
                if (!mainCam)
                {
                    mainCam = Camera.main.transform;
                }
                if (HandTracking.LeftHandTrackInfo.IsValid == false)
                {
                    //Clear lock info:
                    if (lockInfo.lockType != LockedGameObjectInfo.LockType.None && (Time.realtimeSinceStartup - lockInfo.lockTime) >= 0.333f)
                    {
                        lockInfo.lockType = LockedGameObjectInfo.LockType.None;
                        lockInfo.lockedReference = null;
                        Debug.Log("Slider UI : Clear 1 : " + HandTracking.LeftHandTrackInfo.IsValid);
                    }
                    return;
                }
                var isInteractingUI = GazeAndHandInteractionSystem.instance.eyeReticle.CurrentInteractingTarget.isUI;
                bool isclosepinch = HandTracking.LeftHandTrackInfo.NativeGestureType == (int)TouchlessA3D.GestureType.CLOSED_PINCH;
                bool isclosehand = HandTracking.LeftHandTrackInfo.gestureFistOpenHand == GestureType_Fist_OpenHand.Fist;
                bool isPinchGesture = false;
                if (isclosepinch || isclosehand)
                {
                    isPinchGesture = true;
                }
                else
                {
                    isPinchGesture = false;
                }
                //var isPinchGesture = HandTracking.HandTrackingInfo.NativeGestureType == (byte)(TouchlessA3D.GestureType.CLOSED_PINCH);
                //设置 lock target : slider UI:
                if (isInteractingUI && GazeAndHandInteractionSystem.instance.eyeReticle.CurrentInteractingTarget.target)
                {
                    Debug.Log("miao0:" + GazeAndHandInteractionSystem.instance.eyeReticle.CurrentInteractingTarget.target.name+ GazeAndHandInteractionSystem.instance.eyeReticle.CurrentHoveringTarget.target.GetComponentInParent<Slider>());
                    Slider sliderUI = GazeAndHandInteractionSystem.instance.eyeReticle.CurrentInteractingTarget.target.GetComponentInParent<Slider>();

                    if (sliderUI && isPinchGesture)
                    {
                        lockInfo.lockType = LockedGameObjectInfo.LockType.SliderUI;
                        lockInfo.lockedReference = sliderUI;
                        lockInfo.lockTime = Time.realtimeSinceStartup;//记录lock time
                        Debug.Log("Slider UI : Set");
                    }
                }

                //在 lock slider UI的情况下， 拖动进度条:
                if (this.lockInfo.lockType == LockedGameObjectInfo.LockType.SliderUI)
                {
                    //放手或者丢失引用的时候， 解锁:
                    if ((!lockInfo.lockedReference || !isPinchGesture) && (Time.realtimeSinceStartup - lockInfo.lockTime) >= 0.1f)
                    {
                        lockInfo.lockType = LockedGameObjectInfo.LockType.None;
                        lockInfo.lockedReference = default(Object);
                        Debug.Log("Slider UI : Clear 2");
                    }
                    else
                    {
                        //捏合姿态下，移动slider:
                        if (isPinchGesture)
                        {
                            MoveSliderUI(lockInfo.lockedReference as Slider);
                            lockInfo.lockTime = Time.realtimeSinceStartup;//更新 lock time
                            Debug.Log("Slider UI : Move !");
                        }
                    }
                }
            }

            private void MoveSliderUI(Slider sliderUI)
            {
                //Debug.LogFormat("On UI state hover: {0}, {1}", sliderUI, isPinchGesture);
                {
                    
                    var delta = HandTracking.LeftHandTrackInfo.PalmDeltaPosition;
                    var deltaVectorToHead = mainCam.InverseTransformVector(delta);
                    float velocityX = Mathf.Abs(delta.x / Time.deltaTime);
                    float velocityY = Mathf.Abs(delta.y / Time.deltaTime);
                    if (sliderUI.direction== Slider.Direction.LeftToRight|| sliderUI.direction == Slider.Direction.RightToLeft)
                    {
                        if (velocityX >= 0.5f)
                        {
                            bool isRight = deltaVectorToHead.x > 0;
                            float progressDelta = Mathf.Abs(delta.x) * (isRight ? 1 : -1) * 8;//加速8倍
                            sliderUI.normalizedValue = Mathf.Clamp01(sliderUI.normalizedValue + progressDelta);
                            Debug.LogFormat("Change slider UI with delta: {0}, velX : {1}", progressDelta, velocityX);
                        }
                    }
                    if (sliderUI.direction == Slider.Direction.BottomToTop || sliderUI.direction == Slider.Direction.TopToBottom)
                    {
                        if (velocityY >= 0.5f)
                        {
                            bool istop = deltaVectorToHead.y > 0;
                            float progressDelta = Mathf.Abs(delta.y) * (istop ? 1 : -1) * 8;//加速8倍
                            sliderUI.normalizedValue = Mathf.Clamp01(sliderUI.normalizedValue + progressDelta);
                            Debug.LogFormat("Change slider UI with delta: {0}, velX : {1}", progressDelta, velocityY);
                        }
                    }
                }
            }

            public void OnReticleExit()
            {
            }
        }
    }
}