/* Copyright (c) 2022 Crunchfish AB. All rights reserved. All information herein
 * is or may be trade secrets of Crunchfish AB.
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Ximmerse.XR.InputSystems;

namespace TouchlessA3D
{
    using static NativeCalls;
    /// <summary>
    /// SkeletalId for all the bones and boneLengths of a Hand
    /// </summary>
    public enum SkeletalId : int
    {
        Wrist,
        Thumb0,
        Thumb1,
        Thumb2,
        Thumb3,
        ThumbTip,
        Index0,
        Index1,
        Index2,
        Index3,
        IndexTip,
        Middle0,
        Middle1,
        Middle2,
        Middle3,
        MiddleTip,
        Ring0,
        Ring1,
        Ring2,
        Ring3,
        RingTip,
        Pinky0,
        Pinky1,
        Pinky2,
        Pinky3,
        PinkyTip,
    }

    /// <summary>
    /// A Hand is a core data structure and the main result of the SDK. 
    /// Accessible through TouchlessSession.instance.leftHand and
    /// TouchlessSession.instance.rightHand. 
    /// The TouchlessSession keeps the hand updated with the latest HandEvents.
    /// The hand is oriented relative to the camera with the back of the hand
    /// being up and wrist to knuckles being forward.
    /// </summary>
    public class Hand
    {
        public static event System.Action eventOnHandCreated;
        public static void CreateLeftAndRightHand()
        {
            if (sLeftHand == null)
            {
                sLeftHand = new Hand(HandednessType.LEFT_HAND);
            }
            if (sRightHand == null)
            {
                sRightHand = new Hand(HandednessType.RIGHT_HAND);
            }
            eventOnHandCreated?.Invoke();
        }
        public static Hand sLeftHand, sRightHand;

        public static Hand GetHand(HandednessType handedness)
        {
            return handedness == HandednessType.LEFT_HAND ? sLeftHand : sRightHand;
        }

        /// <summary>
        /// The latest received GestureType
        /// </summary>
        public GestureType type { get; protected set; }
        /// <summary>
        /// The handedness, set on initialization
        /// </summary>
        public HandednessType handedness { get; protected set; }
        /// <summary>
        /// The latest UpdateHand was valid
        /// </summary>
        public bool active { get; protected set; }
        /// <summary>
        /// The HandEvent.timestamp of the latest valid UpdateHand
        /// </summary>
        public long timestamp { get; protected set; }
        /// <summary>
        /// The root for all transforms part of the hand. Transforming the root and setting the parent of the root is allowed.
        /// </summary>
        public Transform root { get; protected set; }
        /// <summary>
        /// The position and orientation represented as Transforms. The same transforms are valid throughout the lifetime of the Hand
        /// </summary>
        /// <typeparam name="SkeletalId">Key</typeparam>
        /// <typeparam name="Transform">Value</typeparam>
        public readonly Dictionary<SkeletalId, Transform> bones = new Dictionary<SkeletalId, Transform>();
        /// <summary>
        /// The length of each bone. Zero for Wrist and all fingerTips
        /// </summary>
        /// <typeparam name="SkeletalId">Key</typeparam>
        /// <typeparam name="float">Value</typeparam>
        public readonly Dictionary<SkeletalId, float> boneLengths = new Dictionary<SkeletalId, float>();
        private IntPtr skeletonPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ta3d_skeleton_3d_s)));
        /// <summary>
        /// Constructor for the Hand. Instantiates all related Transforms
        /// </summary>
        public Hand(HandednessType handedness)
        {
            this.handedness = handedness;
            var name = HandednessType.LEFT_HAND == handedness ? "LeftHand" : "RightHand";
            root = new GameObject(name).transform;
            root.parent = UnityEngine.Object.FindObjectOfType<Unity.XR.CoreUtils.XROrigin>().transform;
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.identity;
            foreach (var id in SkeletalInfo.SkeletonTypes)
            {
                var transform = new GameObject(id.ToString()).transform;
                transform.parent = id == SkeletalId.Wrist ? root : bones[id.Previous()];
                bones.Add(id, transform);
                boneLengths.Add(id, 0f);
            }
        }

        /// <summary>
        /// Destructor for the hand. All instantiated transforms and child transforms are destroyed
        /// </summary>
        ~Hand()
        {
            GameObject.Destroy(root.gameObject);
            Marshal.FreeHGlobal(skeletonPtr);
        }

        /// <summary>
        /// Update the type, active status, timestamp, bones and boneLengths  of the hand
        /// </summary>
        /// <param name="timestamp"> The time the frame is considered to be created</param>
        /// <param name="type">The new GestureType</param>
        /// <param name="skeletonData">The data for positioning and orientation</param>
        public void UpdateHand(long timestamp, GestureType type, ta3d_skeleton_3d_s skeletonData, HandTrackingInfo handTrackingInfo)
        {
            if (skeletonData.status != ResultType.RESULT_OK || GestureType.NO_HAND == type)
            {
                active = false;
                return;
            }
            if (this.timestamp == timestamp) return;
            active = true;
            this.timestamp = timestamp;
            this.type = type;

            //The order we set the transforms has to be from wrist to fingertip
            var wristCoords = skeletonData.points[(int)NativePointID.WRIST].coordinates;
            var wristPosition = new Vector3(wristCoords[0], -wristCoords[1], wristCoords[2]);
            var scale = 1f;
            if (null != root)
            {
                wristPosition = root.position + root.rotation * wristPosition;
                scale = root.lossyScale.z;
            }
            wristPosition = handTrackingInfo.WristPosition;
            bones[SkeletalId.Wrist].position = wristPosition;

            foreach (var id in SkeletalInfo.SkeletonTypes)
            {
                //Tip inherits rotation and does not have a valid .Next() to set position on
                if (id.IsTip()) continue;
                //Calculate and set Rotation of id
                var index = id.GetNativeBoneIndex();
                var mat33 = skeletonData.rotations[index].elements;
                //The right direction vector would be Vector3(mat33[0],-mat33[3+0],mat33[6+0]).
                //However we only need the up and forward vector to construct the quaternion.
                var up = HandTracking.rgbCamAnchor.TransformVector(new Vector3(-mat33[1], mat33[3 + 1], -mat33[6 + 1]));
                var forward = HandTracking.rgbCamAnchor.TransformVector(new Vector3(mat33[2], -mat33[3 + 2], mat33[6 + 2]));
                var rotation = Quaternion.LookRotation(forward, up);
                bones[id].rotation = rotation;

                //Wrist has no length and no valid .Next() to set position on.
                if (SkeletalId.Wrist == id) continue;
                float length = scale * skeletonData.bone_lengths[index];
                boneLengths[id] = length;

                //Set position of id.Next(). Thumb0, Index0, Middle0, Ring0 and Pinky0 positions 
                //are just inherited from Wrist, ie localPosition = Vecor3.zero.
                bones[id.Next()].position = bones[id].position + rotation * Vector3.forward * length;
            }
        }
    }

    /// <summary>
    /// Extension Methods and info for SkeletalId
    /// Allows calling these functions on any instance of SkeletalId
    /// </summary>
    public static class SkeletalInfo
    {
        /// <summary>
        /// An indexable array with all the SkeletalId values
        /// </summary>
        public static SkeletalId[] SkeletonTypes = (SkeletalId[])Enum.GetValues(typeof(SkeletalId));

        /// <summary>
        /// Get the SkeletalId of the next bone of a finger. 
        /// Invalid for src.IsTip()
        /// </summary>
        /// <param name="src">SkeletalId to query</param>
        /// <returns>Subsequent SkeletalId</returns>
        public static SkeletalId Next(this SkeletalId src)
        {
            System.Diagnostics.Debug.Assert(!src.IsTip());
            return SkeletonTypes[(int)src + 1];
        }

        /// <summary>
        /// Get the SkeletalId of the previous bone of a finger.
        /// Wrist if src.ConnectedToWrist
        /// </summary>
        /// <param name="src">SkeletalId to query</param>
        /// <returns>Previous SkeletalId</returns>
        public static SkeletalId Previous(this SkeletalId src)
        {
            return src.ConnectedToWrist() ? SkeletalId.Wrist : SkeletonTypes[(int)src - 1];
        }

        /// <summary>
        /// Is the id ConnectedToWrist?
        /// </summary>
        /// <param name="id">SkeletalId to query</param>
        public static bool ConnectedToWrist(this SkeletalId id)
        {
            switch (id)
            {
                case SkeletalId.Wrist:
                case SkeletalId.Thumb0:
                case SkeletalId.Index0:
                case SkeletalId.Middle0:
                case SkeletalId.Ring0:
                case SkeletalId.Pinky0:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Is the id a fingertip?
        /// </summary>
        /// <param name="id">SkeletalId to query</param>
        /// <returns></returns>
        public static bool IsTip(this SkeletalId id)
        {
            switch (id)
            {
                case SkeletalId.ThumbTip:
                case SkeletalId.IndexTip:
                case SkeletalId.MiddleTip:
                case SkeletalId.RingTip:
                case SkeletalId.PinkyTip:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// A conversion from SkeletalId to the index of a corresponding NativeBoneID.
        /// </summary>
        /// <param name="id">SkeletalId to query</param>
        /// <returns>Index of a corresponding NativeBoneID</returns>
        internal static int GetNativeBoneIndex(this SkeletalId id)
        {
            if (SkeletalId.Wrist == id) return (int)NativeBoneID.WRIST_MIDDLE1;//to use the rotation
            int i = (int)id;
            return i - 1 - i / 5;//NativeBoneID does not have Wrist or fingerTips but index is otherwise equal to SkeletalID
        }
    }
}