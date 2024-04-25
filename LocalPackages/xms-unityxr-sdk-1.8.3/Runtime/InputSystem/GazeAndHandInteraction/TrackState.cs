using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ximmerse.XR.InputSystems.GazeAndGestureInteraction
{
    /// <summary>
    /// Interface : hand tracking state.
    /// </summary>
    public interface I_HandTrackingState
    {
       
        bool IsTracking
        {
            get;
        }


        GazeAndHandInteractionSystem.GestureType gestureType
        {
            get;
        }

        /// <summary>
        /// Delta position movement
        /// </summary>
        Vector3 DeltaPositionWorld
        {
            get;
        }

        /// <summary>
        /// Delta angular rotation
        /// </summary>
        Vector3 DeltaAngluarWorld
        {
            get;
        }
    }

    /// <summary>
    /// 更新 tracking state 的状态控制器
    /// </summary>
    public partial class GazeAndHandInteractionSystem
    {
        /// <summary>
        /// Impl of hand track state.
        /// </summary>
        internal partial class TrackState : I_HandTrackingState
        {
            public bool IsTracking
            {
                get => trackingState == HandTrackingStatus.Tracking;
            }

            /// <summary>
            /// 跟踪判断变更条件.
            /// </summary>
            public struct HandTrackStateChangeCondition
            {
                public int frameCount;

                public float timer;
            }

            /// <summary>
            /// Condition : valid to invalid
            /// </summary>
            public HandTrackStateChangeCondition valid2invalid = new HandTrackStateChangeCondition()
            {
                timer = 0.1f,
                frameCount = 3,
            };

            /// <summary>
            /// Condition : invalid to valid
            /// </summary>
            public HandTrackStateChangeCondition invalid2valid = new HandTrackStateChangeCondition()
            {
                timer = 0.1f,
                frameCount = 3,
            };



            public enum HandTrackingStatus : byte
            {
                /// <summary>
                /// 无有效跟踪
                /// </summary>
                Invalid = 0,

                /// <summary>
                /// 有效跟踪状态
                /// </summary>
                Tracking = 1,
            }

            /// <summary>
            /// 跟踪状态
            /// </summary>
            public HandTrackingStatus trackingState
            {
                get; private set;
            }

            public event Action<HandTrackingStatus> OnTrackingStatusChanged;

            internal TrackState()
            {
                trackingState = HandTrackingStatus.Invalid;
            }

            /// <summary>
            /// 合理/非合理帧计数器
            /// </summary>
            int validTrackingCounter = 0, invalidTrackingCounter = 0;

            /// <summary>
            /// 合理/非合理帧计时器
            /// </summary>
            float validTrackingDeltaTimer = 0, invalidTrackingDeltaTimer = 0;

            float previousStateChangeTime;

            const float kMinAllowStateChangeTime = 0.1f;
            public bool IsEnabled
            {
                get; private set;
            }

            public void OnEnable()
            {
                IsEnabled = true;
            }

            public void Tick()
            {
                TickGestureState();
            }
             

            void InternalStateChange(HandTrackingStatus newState)
            {
                if (newState != this.trackingState)
                {
                    this.trackingState = newState;
                    previousStateChangeTime = Time.realtimeSinceStartup;
                    OnTrackingStatusChanged?.Invoke(newState);
                }
            }
            public void OnDisable()
            {
                IsEnabled = false;
            }
        }
    }

}