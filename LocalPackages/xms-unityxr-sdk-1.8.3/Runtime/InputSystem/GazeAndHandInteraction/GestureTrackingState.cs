using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ximmerse.XR.InputSystems.GazeAndGestureInteraction
{

    /// <summary>
    /// 更新 tracking state 的状态控制器
    /// </summary>
    public partial class GazeAndHandInteractionSystem
    {
        /// <summary>
        /// Gesture type recoginzed by system
        /// </summary>
        public enum GestureType : byte
        {
            /// <summary>
            /// 不明手势
            /// </summary>
            Unkown = 0,

            /// <summary>
            /// 手指张开并朝向用户前方
            /// </summary>
            OpenHandAndPointForward,

            /// <summary>
            /// 手指张开并朝向用户自身方向
            /// </summary>
            OpenHandAndPointToUser,

            /// <summary>
            /// 食指拇指捏合
            /// </summary>
            Pinch,

            /// <summary>
            /// 握拳并手心朝向用户前方
            /// </summary>
            CloseHandAndPointForward,

            /// <summary>
            /// 握拳并手心朝向用户自身
            /// </summary>
            CloseHandAndPointToUser,
        }


        /// <summary>
        /// Impl of gesture tracking state.
        /// </summary>
        internal partial class TrackState
        {

            Camera m_mainCamera;

            public Camera mainCamera
            {
                get
                {
                    if (!m_mainCamera)
                        m_mainCamera = Camera.main;

                    return m_mainCamera;
                }
            }

            /// <summary>
            /// Current gesture type
            /// </summary>
            public GestureType gestureType
            {
                get; private set;
            }


            /// <summary>
            /// Delta position movement in world space
            /// </summary>
            public Vector3 DeltaPositionWorld
            {
                get => this.gestureRecords[0].PalmWorldPose.position - gestureRecords[1].PalmWorldPose.position;
            }

            /// <summary>
            /// Delta angular rotation in world space
            /// </summary>
            public Vector3 DeltaAngluarWorld
            {
                get => this.gestureRecords[0].PalmWorldPose.rotation.eulerAngles - gestureRecords[1].PalmWorldPose.rotation.eulerAngles;
            }

            public event Action<GestureType> OnGestureChanged;

            float previousGestureChangeTime;



            /// <summary>
            /// Structure to cache native gesture data.
            /// </summary>
            struct NativeGestureRecord
            {
                public bool IsValidFrame;

                public float RealFrameTime;

                /// <summary>
                /// Palm world pose
                /// </summary>
                public Pose PalmWorldPose;

                /// <summary>
                /// Palm local pose.
                /// </summary>
                public Pose PalmLocalPose;

                /// <summary>
                /// Palm movement velocity.
                /// </summary>
                public Pose PalmVelocity;

                /// <summary>
                /// distance to previous frame
                /// </summary>
                public float PalmDeltaDistance;

                /// <summary>
                /// Angle diff to previous frame
                /// </summary>
                public Vector3 DeltaAngle;

                public int NativeGesturePluginCode;
            }

            const int kCacheGestureDataCount = 5;

            /// <summary>
            /// 缓存的手势输入数据.
            /// </summary>
            readonly NativeGestureRecord[] gestureRecords = new NativeGestureRecord[kCacheGestureDataCount];

            void TickGestureState()
            {
                UpdateGestureRecordQueue();

                //连续三帧判断状态:
                if (CheckValidFrameCount(true, 2))
                {
                    trackingState = HandTrackingStatus.Tracking;
                }
                else if (CheckValidFrameCount(false, 2))
                {
                    trackingState = HandTrackingStatus.Invalid;
                }


                //don't change gesture type too frequently
                if (trackingState == HandTrackingStatus.Tracking && (Time.realtimeSinceStartup - previousGestureChangeTime) >= kMinAllowStateChangeTime)
                {
                    UpdateGestureTypeChange();
                }
            }

            /// <summary>
            /// 更新 gesture输入队列
            /// </summary>
            void UpdateGestureRecordQueue()
            {

                HandTrackingInfo currentHandTrackInfo = HandTracking.LeftHandTrackInfo;
                if (!currentHandTrackInfo.IsValid)
                {
                    InsertRecord(new NativeGestureRecord());//insert dummy record
                }
                //添加一条有效数据
                else
                {
                    NativeGestureRecord record = new NativeGestureRecord()
                    {
                        IsValidFrame = true,
                        RealFrameTime = Time.realtimeSinceStartup,
                        NativeGesturePluginCode = currentHandTrackInfo.NativeGestureType,
                        PalmWorldPose = new Pose(currentHandTrackInfo.PalmPosition, Quaternion.LookRotation(currentHandTrackInfo.PalmNormal)),
                        PalmLocalPose = new Pose(currentHandTrackInfo.PalmLocalPosition, Quaternion.LookRotation(currentHandTrackInfo.PalmLocalNormal)),
                    };
                    //计算delta:
                    if (GetPreviousValidFrame(out NativeGestureRecord prevValidFrame))
                    {
                        float deltaTime = Mathf.Min(Mathf.Epsilon, record.RealFrameTime - prevValidFrame.RealFrameTime);
                        Vector3 vector2prev = record.PalmWorldPose.position - prevValidFrame.PalmWorldPose.position;
                        Vector3 deltaAngle = (Quaternion.Inverse(prevValidFrame.PalmWorldPose.rotation) * record.PalmWorldPose.rotation).eulerAngles;
                        record.PalmVelocity = new Pose(
                             (vector2prev) /
                             deltaTime,
                              Quaternion.Euler(deltaAngle / deltaTime));
                        record.PalmDeltaDistance = vector2prev.magnitude;
                        record.DeltaAngle = deltaAngle;
                    }

                    InsertRecord(record);
                }
            }

            /// <summary>
            /// 更新手势识别的当前手势输入类型
            /// </summary>
            void UpdateGestureTypeChange()
            {
                int checkFrame = 2;
                //如果连续五帧是Open Hand姿态:
                if (CheckNativeGestureCodeMatchContinueFrames((int)TouchlessA3D.GestureType.OPEN_HAND, checkFrame)
                    || CheckNativeGestureCodeMatchContinueFrames((int)TouchlessA3D.GestureType.HAND, checkFrame))
                {
                    //手心张开，向前
                    if (CheckForawrdMatchContinueFrames(this.mainCamera.transform.forward, checkFrame))
                    {
                        InternalUpdateGestureState(GestureType.OpenHandAndPointForward);
                    }
                    //手心张开,向后
                    else if (CheckForawrdMatchContinueFrames(-this.mainCamera.transform.forward, checkFrame))
                    {
                        InternalUpdateGestureState(GestureType.OpenHandAndPointToUser);
                    }
                    else
                    {
                        InternalUpdateGestureState(GestureType.Unkown);
                    }
                }

                if (CheckNativeGestureCodeMatchContinueFrames((int)TouchlessA3D.GestureType.CLOSED_PINCH, checkFrame))
                {
                    InternalUpdateGestureState(GestureType.Pinch);
                }
                if (CheckNativeGestureCodeMatchContinueFrames((int)TouchlessA3D.GestureType.CLOSED_HAND, checkFrame))
                {
                    //握拳，向前
                    if (CheckForawrdMatchContinueFrames(this.mainCamera.transform.forward, checkFrame))
                    {
                        InternalUpdateGestureState(GestureType.CloseHandAndPointForward);
                    }
                    //握拳,向用户
                    else if (CheckForawrdMatchContinueFrames(-this.mainCamera.transform.forward, checkFrame))
                    {
                        InternalUpdateGestureState(GestureType.CloseHandAndPointToUser);
                    }
                    else
                    {
                        InternalUpdateGestureState(GestureType.Unkown);
                    }
                }
            }

            void InternalUpdateGestureState(GestureType newGesture)
            {
                if (gestureType != newGesture)
                {
                    gestureType = newGesture;
                    previousGestureChangeTime = Time.realtimeSinceStartup;
                    OnGestureChanged?.Invoke(newGesture);
                }
            }

            void InsertRecord(NativeGestureRecord nativeRecord)
            {
                for (int i = gestureRecords.Length - 1; i > 0; i--)
                {
                    gestureRecords[i] = gestureRecords[i - 1];
                }
                gestureRecords[0] = nativeRecord;
            }

            /// <summary>
            /// Get previous valid record.
            /// </summary>
            /// <param name="prevNativeRecord"></param>
            /// <returns></returns>
            bool GetPreviousValidFrame(out NativeGestureRecord prevNativeRecord)
            {
                for (int i = 0; i < gestureRecords.Length; i++)//starts from 1
                {
                    if (gestureRecords[i].IsValidFrame)
                    {
                        prevNativeRecord = gestureRecords[i];
                        return true;
                    }
                }

                prevNativeRecord = default(NativeGestureRecord);
                return false;
            }

            /// <summary>
            /// 检查连续若干帧数据是有效数据
            /// </summary>
            /// <param name="frameCount"></param>
            /// <returns></returns>
            bool CheckValidFrameCount(bool isValid, int frameCount)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    if (gestureRecords[i].IsValidFrame != isValid)
                    {
                        return false;
                    }
                }
                return true;
            }


            /// <summary>
            /// Check a certain frames matching forward direction 
            /// </summary>
            /// <param name="forward"></param>
            /// <param name="frameCount"></param>
            /// <param name="expectDot"></param>
            /// <returns></returns>
            bool CheckForawrdMatchContinueFrames(Vector3 forward, int frameCount, float expectDot = 0.45f)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    if (Vector3.Dot(forward, this.gestureRecords[i].PalmWorldPose.forward) < expectDot)
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Check if a certain frames matching target gesture code.
            /// </summary>
            /// <param name="targetCode"></param>
            /// <param name="frameCount"></param>
            /// <returns></returns>
            bool CheckNativeGestureCodeMatchContinueFrames(int targetCode, int frameCount)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    if (targetCode != this.gestureRecords[i].NativeGesturePluginCode)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}