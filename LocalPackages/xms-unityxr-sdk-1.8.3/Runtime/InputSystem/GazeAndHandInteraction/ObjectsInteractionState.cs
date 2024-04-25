using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Ximmerse.XR.InputSystems.GazeAndGestureInteraction
{
    /// <summary>
    /// Gaze and hand interaction system manages how XR user interacts world objects withe eye reticle and hand gesture.
    /// </summary>
    public partial class GazeAndHandInteractionSystem
    {
        /// <summary>
        /// UI objects interaction state.
        /// </summary>
        internal class ObjectsInteractionState : I_InteractionState
        {
            public bool IsEnabled
            {
                get; private set;
            }

            Transform mainCam;

            GameObject palm;

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
                if (palm==null)
                {
                    VirtualHandRenderer hand = FindObjectOfType<VirtualHandRenderer>();
                    palm = hand.palm;
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
                var isHoveringUI = GazeAndHandInteractionSystem.instance.eyeReticle.CurrentInteractingTarget.isUI;
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
                //Debug.Log("miao0" + GazeAndHandInteractionSystem.instance.eyeReticle.CurrentInteractingTarget.target.name);
                //var isPinchGesture = HandTracking.HandTrackingInfo.NativeGestureType == (byte)(TouchlessA3D.GestureType.CLOSED_PINCH);
                //设置 lock target : slider UI:
                if (isPinchGesture)
                {
                    
                }


            }

            private void MoveSliderUI(GameObject target)
            {
                
                //Debug.Log("miao:" + target.name);
                target.transform.parent = palm.transform;
                //Debug.Log("miao:target " + target.transform.position);
            }

            public void OnReticleExit()
            {
            }
        }
    }
}