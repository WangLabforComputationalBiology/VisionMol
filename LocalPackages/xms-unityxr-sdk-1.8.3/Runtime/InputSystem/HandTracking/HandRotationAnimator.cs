/* Copyright (c) 2023 Crunchfish AB. All rights reserved. All information herein
 * is or may be trade secrets of Crunchfish AB.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TouchlessA3D;
namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// Animates a hand mesh based on the rotations of a tracked hand. 
    /// Note that the joints of the animated hand is not fully aligned with the joints of the mesh.
    /// </summary>
    public class HandRotationAnimator : MonoBehaviour
    {
        /// <summary>
        /// The 3d model of a hand to animate.
        /// </summary>
        public Transform handModel;
        /// <summary>
        /// What hand should we select.
        /// </summary>
        public HandednessType handedness;
        /// <summary>
        /// The selected hand.
        /// </summary>
        private Hand hand;
        /// <summary>
        /// The bones of the hand mesh to animate. 
        /// Bones should be named to match the corresponding SkeletalId.
        /// </summary>
        /// <typeparam name="SkeletalId">The id to be tracked and matched to a bone.</typeparam>
        /// <typeparam name="Transform">The bone of the mesh to be animated.</typeparam>
        private Dictionary<SkeletalId, Transform> animatedBones = new Dictionary<SkeletalId, Transform>();
        /// <summary>
        /// The length of the mesh.
        /// </summary>
        private float meshBaseLength;
        private void Start()
        {
            Hand.CreateLeftAndRightHand();
            hand = Hand.GetHand(this.handedness);

            string[] idNames = new string[SkeletalInfo.SkeletonTypes.Length];
            for (int i = 0; i < idNames.Length; i++)
            {
                idNames[i] = SkeletalInfo.SkeletonTypes[i].ToString();
            }
            AddChildBones(handModel, idNames);
            meshBaseLength = Vector3.Distance(animatedBones[SkeletalId.Middle1].position,
              animatedBones[SkeletalId.Wrist].position);
        }

        /// <summary>
        /// Recursively add the children of parent listed in idNames to animatedBones.
        /// </summary>
        /// <param name="parent">The parent of potential bones.</param>
        /// <param name="idNames">List of names to add to animated.</param>
        private void AddChildBones(Transform parent, string[] idNames)
        {
            foreach (Transform child in parent.GetComponentInChildren<Transform>(true))
            {
                AddChildBones(child, idNames);
                var index = Array.IndexOf(idNames, child.name);
                if (index == -1)
                {
                    continue;
                }
                var id = SkeletalInfo.SkeletonTypes[index];
                animatedBones.Add(id, child);
            }
        }

        /// <summary>
        /// Set active hand, handSize and the rotations of the joints.
        /// We use LateUpdate to have the visualization match the latest possible updated hand.
        /// </summary>
        private void LateUpdate()
        {
            HandTrackingInfo handTrackingInfo = HandTracking.GetHandTrackingInfo(this.handedness == HandednessType.LEFT_HAND ? HandnessType.Left : HandnessType.Right);
            handModel.gameObject.SetActive(handTrackingInfo.IsValid);
            if (!handTrackingInfo.IsValid)
            {
                return;
            }

            hand.UpdateHand(handTrackingInfo.Timestamp,
                (GestureType)handTrackingInfo.NativeGestureType,
                HandTrackingT3D.instance.GetRawSkeleton(this.handedness == HandednessType.LEFT_HAND ? HandnessType.Left : HandnessType.Right),
                handTrackingInfo);

            animatedBones[SkeletalId.Wrist].rotation = hand.bones[SkeletalId.Wrist].rotation;
            animatedBones[SkeletalId.Wrist].position = hand.bones[SkeletalId.Wrist].position;

            var handSize = Vector3.Distance(hand.bones[SkeletalId.Middle1].position,
              hand.bones[SkeletalId.Wrist].position) /
              (meshBaseLength * animatedBones[SkeletalId.Wrist].parent.lossyScale.x);
            //animatedBones[SkeletalId.Wrist].localScale = Vector3.one * handSize;

            SetFinger(SkeletalId.Thumb1);
            SetFinger(SkeletalId.Index1);
            SetFinger(SkeletalId.Middle1);
            SetFinger(SkeletalId.Ring1);
            SetFinger(SkeletalId.Pinky1);
        }

        /// <summary>
        /// Set rotation of all animatedBones of the finger.
        /// </summary>
        /// <param name="id">The finger to set bone rotations of.</param>
        private void SetFinger(SkeletalId id)
        {
            while (!id.IsTip())
            {
                var joint = animatedBones[id];
                if (id == SkeletalId.Thumb1)
                {
                    var thumbDir = hand.bones[SkeletalId.Thumb2].position +
                      hand.bones[SkeletalId.Wrist].forward * 0.01f - joint.position;
                    var thumbForward = Vector3.ProjectOnPlane(hand.bones[id].up, thumbDir);
                    joint.rotation = Quaternion.LookRotation(thumbDir, thumbForward);
                }
                else
                {
                    joint.transform.rotation = hand.bones[id].rotation;
                }
                id = id.Next();
            }
        }
    }
}
