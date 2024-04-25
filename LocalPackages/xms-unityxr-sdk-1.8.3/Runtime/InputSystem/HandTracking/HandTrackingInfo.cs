using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System;
using Unity.Jobs;
using TouchlessA3D;
namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// State the left / right hand
    /// </summary>
    public enum HandnessType : byte
    {
        Left = 0, Right = 1,
    }

    /// <summary>
    /// Swing data cache
    /// </summary>
    public struct SwingCache
    {
        public float swingAngle, timeStamp;
    }

    public class SwingCacheList
    {
        public float lastSwingInvokeTime;
        public List<SwingCache> caches = new List<SwingCache>();
        public float swingHandSum = 0;
        public bool EnoughCache(int count)
        {
            return caches.Count >= count;
        }
        public void AddCache(float swingAngle, float time)
        {
            caches.Add(new SwingCache()
            {
                swingAngle = swingAngle,
                timeStamp = time,
            });
            swingHandSum += swingAngle;
        }

        /// <summary>
        /// 当caches中最后一条记录的timestamp 和 now 的时差大于 timeout 的时候， 清空队列。
        /// </summary>
        /// <param name="now"></param>
        /// <param name="timeout"></param>
        public void ClearCacheTimeout(float now, float timeout)
        {
            if (caches.Count > 0)
            {
                if (Mathf.Abs(caches[caches.Count - 1].timeStamp - now) >= timeout)
                {
                    caches.Clear();
                    swingHandSum = 0;
                }
            }
        }

        public void Clear()
        {
            caches.Clear();
            swingHandSum = 0;
        }
    }

    /// <summary>
    /// Hand tracking data cache 
    /// </summary>
    public struct HandTrackingDataCache
    {
        /// <summary>
        /// wrist point in local head space
        /// </summary>
        public Vector3 wristPointL;

        /// <summary>
        /// index root point in local head space
        /// </summary>
        public Vector3 indexRootL;

        /// <summary>
        /// 2d position of the wrist point
        /// </summary>
        public Vector2 wristPoint2D;

        /// <summary>
        /// 2d position of the index finger root node
        /// </summary>
        public Vector2 indexRoot2D;

        /// <summary>
        /// time stamp
        /// </summary>
        public float timestamp;

        public HandTrackingDataCache(Vector3 wristPointL, Vector3 indexRootL, Vector2 wristPoint2D, Vector2 indexRoot2D, float timestamp)
        {
            this.wristPointL = wristPointL;
            this.indexRootL = indexRootL;
            this.wristPoint2D = wristPoint2D;
            this.indexRoot2D = indexRoot2D;
            this.timestamp = timestamp;
        }
    }

    /// <summary>
    /// hand tracking info.
    /// </summary>
    [Serializable]
    public struct HandTrackingInfo : IDisposable
    {
        /// <summary>
        /// The hand tracking frame time stamp.
        /// </summary>
        public long Timestamp;

        /// <summary>
        /// is currently valid tracking state ?
        /// </summary>
        public bool IsValid;

        /// <summary>
        /// Currently tracking left or right hand. 
        /// For T3D, one hand per frame.
        /// </summary>
        public HandnessType Handness;

        /// <summary>
        /// 手腕的位置, world space
        /// </summary>
        public Vector3 WristPosition;

        /// <summary>
        /// Wrist Point in head space.
        /// </summary>
        public Vector3 WristLocalPosition;

        /// <summary>
        /// 手腕的原始输出位置数据
        /// </summary>
        public Vector3 WristRawPosition;

        /// <summary>
        /// Palm center world position
        /// </summary>
        public Vector3 PalmPosition;

        /// <summary>
        /// The palm delta position in world space.
        /// </summary>
        public Vector3 PalmDeltaPosition;

        /// <summary>
        /// The palm delta rotation euler in world space
        /// </summary>
        public Vector3 PalmDeltaEuler;

        /// <summary>
        /// Palm velocity in world space.
        /// </summary>
        public Vector3 PalmVelocity;

        /// <summary>
        /// Palm velocity in user head space.
        /// </summary>
        public Vector3 PalmProjectionVelocity;

        /// <summary>
        /// Palm position in user head space.
        /// </summary>
        public Vector3 PalmProjectionPosition;

        /// <summary>
        /// Palm surface world normal ray
        /// </summary>
        public Vector3 PalmNormal;

        /// <summary>
        /// Palm scale.
        /// </summary>
        public Vector3 PalmScale;

        /// <summary>
        /// Palm world rotation
        /// </summary>
        public Quaternion PalmRotation;

        /// <summary>
        /// The palm's local position relative to main camera's parent transform.
        /// </summary>
        public Vector3 PalmLocalPosition;

        /// <summary>
        /// The palm's local rotation relative to main camera's parent transform.
        /// </summary>
        public Quaternion PalmLocalRotation;

        /// <summary>
        /// The palm's local normal relative to main camera's parent transform.
        /// </summary>
        public Vector3 PalmLocalNormal;

        /// <summary>
        /// indicates if the palm is facing towards user's face 
        /// </summary>
        public bool IsPalmFacingTowardsUser;

        /// <summary>
        /// Palm 2d position in user view space .
        /// Left bottom = {0,0};
        /// Right top = {1,1};
        /// </summary>
        public Vector2 PalmViewSpacePosition;

        /// <summary>
        /// Thumb tracking info
        /// </summary>
        public RawFingerTrackingInfo ThumbFinger;

        /// <summary>
        /// Index tracking info
        /// </summary>
        public RawFingerTrackingInfo IndexFinger;

        /// <summary>
        /// Middle tracking info
        /// </summary>
        public RawFingerTrackingInfo MiddleFinger;

        /// <summary>
        /// Ring tracking info
        /// </summary>
        public RawFingerTrackingInfo RingFinger;

        /// <summary>
        /// Little finger info
        /// </summary>
        public RawFingerTrackingInfo LittleFinger;

        /// <summary>
        /// The current gesture type of open hand/fist.
        /// </summary>
        public GestureType_Fist_OpenHand gestureFistOpenHand;

        /// <summary>
        /// The current gesture type of grisp
        /// </summary>
        public GestureType_Grisp gestureGrisp;

        /// <summary>
        /// The gesture type from native plugin.
        /// -1 for invalid status.
        /// </summary>
        public int NativeGestureType;

        /// <summary>
        /// The hand ray direction.
        /// </summary>
        public Quaternion handRayDirection;

        /// <summary>
        /// 手掌在过去0.5s内的挥动速度
        /// </summary>
        public float SwingVelocity;

        /// <summary>
        /// 挥手标志， 1 = 向左挥手， 2 = 向右挥手, 0 = none
        /// </summary>
        public int SwingFlag;

        /// <summary>
        /// 手腕在 2D texture 上的位置数据,范围[0..1], 左下角是[0,0],右上角是 [1,1]
        /// </summary>
        public Vector2 WristPosition2D;

        /// <summary>
        /// 食指根节点的 2D texture 位置数据
        /// </summary>
        public Vector2 IndexRootPosition2D;

        /// <summary>
        /// 最近一次丢跟踪的帧序号
        /// </summary>
        public int prevLostTrackingFrame;

        internal void UpdateProperties()
        {
            ThumbFinger.UpdateInternalProperties();
            IndexFinger.UpdateInternalProperties();
            MiddleFinger.UpdateInternalProperties();
            RingFinger.UpdateInternalProperties();
            LittleFinger.UpdateInternalProperties();
        }

        public void Dispose()
        {
            ThumbFinger.Dispose();
            IndexFinger.Dispose();
            MiddleFinger.Dispose();
            RingFinger.Dispose();
            LittleFinger.Dispose();
        }

        internal void CopyFrom(HandTrackingInfo other)
        {
            Timestamp = other.Timestamp;
            IsValid = other.IsValid;
            Handness = other.Handness;
            PalmPosition = other.PalmPosition;
            PalmDeltaPosition = other.PalmDeltaPosition;
            PalmDeltaEuler = other.PalmDeltaEuler;
            PalmNormal = other.PalmNormal;
            PalmScale = other.PalmScale;
            PalmRotation = other.PalmRotation;
            PalmLocalPosition = other.PalmLocalPosition;
            PalmLocalRotation = other.PalmLocalRotation;
            PalmLocalNormal = other.PalmLocalNormal;
            IsPalmFacingTowardsUser = other.IsPalmFacingTowardsUser;
            ThumbFinger.CopyFrom(other.ThumbFinger);
            IndexFinger.CopyFrom(other.IndexFinger);
            MiddleFinger.CopyFrom(other.MiddleFinger);
            RingFinger.CopyFrom(other.RingFinger);
            LittleFinger.CopyFrom(other.LittleFinger);
            gestureFistOpenHand = other.gestureFistOpenHand;
            gestureGrisp = other.gestureGrisp;
            NativeGestureType = other.NativeGestureType;
            PalmViewSpacePosition = other.PalmViewSpacePosition;
            PalmProjectionVelocity = other.PalmProjectionVelocity;
            PalmProjectionPosition = other.PalmProjectionPosition;
            WristPosition = other.WristPosition;
            WristRawPosition = other.WristRawPosition;
            SwingVelocity = other.SwingVelocity;
            SwingFlag = other.SwingFlag;
            WristPosition2D = other.WristPosition2D;
            IndexRootPosition2D = other.IndexRootPosition2D;
        }

        public Pose GetNodePose(SkeletalId joint)
        {
            var handTrackingInfo = this;
            const float kDefaultRadius = 0.016f;
            RawFingerTrackingInfo thumbFinger = handTrackingInfo.ThumbFinger;
            RawFingerTrackingInfo indexFinger = handTrackingInfo.IndexFinger;
            RawFingerTrackingInfo middleFinger = handTrackingInfo.MiddleFinger;
            RawFingerTrackingInfo ringFinger = handTrackingInfo.RingFinger;
            RawFingerTrackingInfo littleFinger = handTrackingInfo.LittleFinger;
            Pose jointPose = default;
            switch (joint)
            {
                case SkeletalId.Wrist:
                    jointPose = new Pose(handTrackingInfo.WristPosition, handTrackingInfo.PalmRotation);
                    break;

                case SkeletalId.Thumb0:
                    jointPose = new Pose(Vector3.Lerp(handTrackingInfo.WristPosition, thumbFinger.Positions[0], 0.75f), Quaternion.LookRotation(thumbFinger.Positions[0] - handTrackingInfo.WristPosition));
                    break;

                case SkeletalId.Thumb1:
                    jointPose = new Pose(thumbFinger.Positions[0], Quaternion.LookRotation(thumbFinger.Positions[1] - thumbFinger.Positions[0]));
                    break;

                case SkeletalId.Thumb2:
                    jointPose = new Pose(thumbFinger.Positions[1], Quaternion.LookRotation(thumbFinger.Positions[2] - thumbFinger.Positions[1]));
                    break;

                case SkeletalId.Thumb3:
                case SkeletalId.ThumbTip:
                    jointPose = new Pose(thumbFinger.Positions[2], Quaternion.LookRotation(thumbFinger.Positions[2] - thumbFinger.Positions[1]));
                    break;


                case SkeletalId.Index0:
                    jointPose = new Pose(Vector3.Lerp(handTrackingInfo.WristPosition, indexFinger.Positions[0], 0.75f), Quaternion.LookRotation(indexFinger.Positions[0] - handTrackingInfo.WristPosition));
                    break;
                case SkeletalId.Index1:
                    jointPose = new Pose(indexFinger.Positions[0], Quaternion.LookRotation(indexFinger.Positions[1] - indexFinger.Positions[0]));
                    break;

                case SkeletalId.Index2:
                    jointPose = new Pose(indexFinger.Positions[1], Quaternion.LookRotation(indexFinger.Positions[2] - indexFinger.Positions[1]));
                    break;

                case SkeletalId.Index3:
                    jointPose = new Pose(indexFinger.Positions[2], Quaternion.LookRotation(indexFinger.Positions[3] - indexFinger.Positions[2]));
                    break;

                case SkeletalId.IndexTip:
                    jointPose = new Pose(indexFinger.Positions[3], Quaternion.LookRotation(indexFinger.Positions[3] - indexFinger.Positions[2]));
                    break;


                case SkeletalId.Middle0:
                    jointPose = new Pose(Vector3.Lerp(handTrackingInfo.WristPosition, middleFinger.Positions[0], 0.75f), Quaternion.LookRotation(middleFinger.Positions[0] - handTrackingInfo.WristPosition));
                    break;
                case SkeletalId.Middle1:
                    jointPose = new Pose(middleFinger.Positions[0], Quaternion.LookRotation(middleFinger.Positions[1] - middleFinger.Positions[0]));
                    break;

                case SkeletalId.Middle2:
                    jointPose = new Pose(middleFinger.Positions[1], Quaternion.LookRotation(middleFinger.Positions[2] - middleFinger.Positions[1]));
                    break;

                case SkeletalId.Middle3:
                    jointPose = new Pose(middleFinger.Positions[2], Quaternion.LookRotation(middleFinger.Positions[3] - middleFinger.Positions[2]));
                    break;

                case SkeletalId.MiddleTip:
                    jointPose = new Pose(middleFinger.Positions[3], Quaternion.LookRotation(middleFinger.Positions[3] - middleFinger.Positions[2]));
                    break;


                case SkeletalId.Ring0:
                    jointPose = new Pose(Vector3.Lerp(handTrackingInfo.WristPosition, ringFinger.Positions[0], 0.75f), Quaternion.LookRotation(ringFinger.Positions[0] - handTrackingInfo.WristPosition));
                    break;
                case SkeletalId.Ring1:
                    jointPose = new Pose(ringFinger.Positions[0], Quaternion.LookRotation(ringFinger.Positions[1] - ringFinger.Positions[0]));
                    break;

                case SkeletalId.Ring2:
                    jointPose = new Pose(ringFinger.Positions[1], Quaternion.LookRotation(ringFinger.Positions[2] - ringFinger.Positions[1]));
                    break;

                case SkeletalId.Ring3:
                    jointPose = new Pose(ringFinger.Positions[2], Quaternion.LookRotation(ringFinger.Positions[3] - ringFinger.Positions[2]));
                    break;

                case SkeletalId.RingTip:
                    jointPose = new Pose(ringFinger.Positions[3], Quaternion.LookRotation(ringFinger.Positions[3] - ringFinger.Positions[2]));
                    break;

                case SkeletalId.Pinky0:
                    jointPose = new Pose(Vector3.Lerp(handTrackingInfo.WristPosition, littleFinger.Positions[0], 0.75f), Quaternion.LookRotation(littleFinger.Positions[0] - handTrackingInfo.WristPosition));
                    break;
                case SkeletalId.Pinky1:
                    jointPose = new Pose(littleFinger.Positions[0], Quaternion.LookRotation(littleFinger.Positions[0] - handTrackingInfo.WristPosition));
                    break;

                case SkeletalId.Pinky2:
                    jointPose = new Pose(littleFinger.Positions[1], Quaternion.LookRotation(littleFinger.Positions[1] - littleFinger.Positions[0]));
                    break;

                case SkeletalId.Pinky3:
                    jointPose = new Pose(littleFinger.Positions[2], Quaternion.LookRotation(littleFinger.Positions[2] - littleFinger.Positions[1]));
                    break;

                case SkeletalId.PinkyTip:
                    jointPose = new Pose(littleFinger.Positions[3], Quaternion.LookRotation(littleFinger.Positions[3] - littleFinger.Positions[2]));
                    break;

                default:
                    jointPose = new Pose(handTrackingInfo.WristPosition, handTrackingInfo.PalmRotation); //default returns Palm pose
                    break;
            }
            return jointPose;
        }
    }

    /// <summary>
    /// The raw finger data.
    /// </summary>
    [Serializable]
    public struct RawFingerTrackingInfo : IDisposable
    {
        /// <summary>
        /// Position nodes of each finger joint, in world space
        /// </summary>
        public NativeArray<Vector3> Positions;

        /// <summary>
        /// Raw tracked position of the each finger joint.
        /// </summary>
        public NativeArray<Vector3> RawPositions;

        /// <summary>
        /// Track position of each finger joint, in head space.
        /// </summary>
        public NativeArray<Vector3> LocalPositions;

        /// <summary>
        /// Finger straightness factor.
        /// 
        /// 1 means straight, 0 means bended.
        /// 
        /// </summary>
        public float straightness;

        /// <summary>
        /// finger bendess factor.
        /// </summary>
        public float bendness;

        internal float bendnessRangeMin, bendnessRangeMax;

        internal void UpdateInternalProperties()
        {
            if (Positions.Length == 3)
            {
                Vector3 dir21 = (Positions[2] - Positions[1]).normalized;
                Vector3 dir10 = (Positions[1] - Positions[0]).normalized;
                this.straightness = Mathf.Abs(Vector3.Dot(dir21, dir10));
            }
            else if (Positions.Length == 4)
            {
                Vector3 dir32 = (Positions[3] - Positions[2]).normalized;
                Vector3 dir21 = (Positions[2] - Positions[1]).normalized;
                Vector3 dir10 = (Positions[1] - Positions[0]).normalized;
                this.straightness = Mathf.Abs(Vector3.Dot(dir32, dir21)) * Mathf.Abs(Vector3.Dot(dir21, dir10));
            }
            this.bendness = Mathf.Clamp01((1 - Mathf.InverseLerp(bendnessRangeMin, bendnessRangeMax, straightness) - bendnessRangeMin) / (bendnessRangeMax - bendnessRangeMin));
        }

        internal RawFingerTrackingInfo(int positionCapacity)
        {
            bendnessRangeMin = bendnessRangeMax = 0;
            straightness = 0;
            bendness = 0;
            Positions = new NativeArray<Vector3>(positionCapacity, Allocator.Persistent);
            RawPositions = new NativeArray<Vector3>(positionCapacity, Allocator.Persistent);
            LocalPositions = new NativeArray<Vector3>(positionCapacity, Allocator.Persistent);
        }

        public void Dispose()
        {
            if (Positions.IsCreated)
            {
                Positions.Dispose();
            }
            if (RawPositions.IsCreated)
            {
                RawPositions.Dispose();
            }
            if (LocalPositions.IsCreated)
            {
                LocalPositions.Dispose();
            }
        }

        public void CopyFrom(RawFingerTrackingInfo other)
        {
            Positions.CopyFrom(other.Positions);
            RawPositions.CopyFrom(other.RawPositions);
            LocalPositions.CopyFrom(other.LocalPositions);
            straightness = other.straightness;
            bendness = other.bendness;
            bendnessRangeMin = other.bendnessRangeMin;
            bendnessRangeMax = other.bendnessRangeMax;
        }
    }
}