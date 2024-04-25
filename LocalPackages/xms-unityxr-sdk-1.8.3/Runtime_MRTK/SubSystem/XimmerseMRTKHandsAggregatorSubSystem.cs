using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine.XR;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;
using Microsoft.MixedReality.Toolkit.Input;
namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// Hands aggregator subSystem, your project must has MRTK package to let this subsystem works.
    /// </summary>
    [Preserve]
    [MRTKSubsystem(
        Name = "com.ximmerse.xr.hands",
        DisplayName = "Subsystem for Ximmerse XR Hands API",
        Author = "Ximmerse",
        ProviderType = typeof(XimmerseXRHandProvider),
        SubsystemTypeOverride = typeof(XimmerseMRTKHandsAggregatorSubSystem),
        ConfigType = typeof(MRTKHandsAggregatorConfig))]
    public class XimmerseMRTKHandsAggregatorSubSystem : HandsAggregatorSubsystem
    {


        private static List<MRTKSubsystemDescriptor<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>> _mrtkSubSysDescriptors = new List<MRTKSubsystemDescriptor<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Register()
        {
            Debug.Log("On Register SubsystemRegistration : XimmerseMRTKHandsAggregatorSubSystem");
            if (Application.platform == RuntimePlatform.Android)
            {
                // Fetch subsystem metadata from the attribute.
                var cinfo = XRSubsystemHelpers.ConstructCinfo<XimmerseMRTKHandsAggregatorSubSystem, MRTKSubsystemCinfo>();

                if (!XRSubsystemHelpers.CheckTypes<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>(cinfo.Name, cinfo.SubsystemTypeOverride, cinfo.ProviderType))
                {
                    throw new System.ArgumentException("Could not create XimmerseMRTKHandsAggregatorSubSystem.");
                }
                var descriptor = new MRTKSubsystemDescriptor<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>(cinfo);
                SubsystemDescriptorStore.RegisterDescriptor(descriptor);
                Debug.Log("Register SubsystemRegistration : XimmerseMRTKHandsAggregatorSubSystem successfully !");

                XimmerseXRLoader.eventOnInit += OnXRLoaderInit;
                XimmerseXRLoader.eventOnStart += OnXRLoaderStart;
                XimmerseXRLoader.eventOnStop += OnXRLoaderStop;
                XimmerseXRLoader.eventOnDestroy += OnXRLoaderDestroy;
            }
        }

        static void OnXRLoaderInit(XimmerseXRLoader loader)
        {
            loader.CreatesSubsystem<MRTKSubsystemDescriptor<HandsAggregatorSubsystem, HandsAggregatorSubsystem.Provider>, XimmerseMRTKHandsAggregatorSubSystem>(_mrtkSubSysDescriptors, "com.ximmerse.xr.hands");
        }
        static void OnXRLoaderStart(XimmerseXRLoader loader)
        {
            loader.StartsSubSystem<XimmerseMRTKHandsAggregatorSubSystem>();
        }
        static void OnXRLoaderStop(XimmerseXRLoader loader)
        {
            loader.StopsSubsystem<XimmerseMRTKHandsAggregatorSubSystem>();
        }

        static void OnXRLoaderDestroy(XimmerseXRLoader loader)
        {
            loader.DestroysSubSystem<XimmerseMRTKHandsAggregatorSubSystem>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            Debug.Log("On XimmerseMRTKHandsAggregatorSubSystem start () !");
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Log("On XimmerseMRTKHandsAggregatorSubSystem stop () !");
        }

        /// <summary>
        /// XR hand info provider 
        /// </summary>
        [Preserve]
        public class XimmerseXRHandProvider : Provider, IHandsSubsystem
        {
            List<HandJointPose> jointPosesL = new List<HandJointPose>();

            List<HandJointPose> jointPosesR = new List<HandJointPose>();


            public override void Start()
            {
                base.Start();
                jointPosesL.Clear();
                //See TrackedHandJoint
                for (int i = 0; i < 26; i++)
                {
                    jointPosesL.Add(default);
                }
                jointPosesR.Clear();
                //See TrackedHandJoint
                for (int i = 0; i < 26; i++)
                {
                    jointPosesR.Add(default);
                }
            }

            public override void Stop()
            {
                base.Stop();
            }

            /// <summary>
            /// Get entire hand joint list.
            /// https://learn.microsoft.com/en-us/dotnet/api/microsoft.mixedreality.toolkit.subsystems.handsaggregatorsubsystem.trygetentirehand?view=mrtkcore-3.0
            /// https://learn.microsoft.com/en-us/dotnet/api/microsoft.mixedreality.toolkit.trackedhandjoint?view=mrtkcore-3.0
            /// </summary>
            /// <param name="hand"></param>
            /// <param name="jointPoses"></param>
            /// <returns></returns>
            public override bool TryGetEntireHand(XRNode hand, out IReadOnlyList<HandJointPose> jointPoses)
            {
                switch (hand)
                {
                    case XRNode.LeftHand:
                        if (HandTracking.LeftHandTrackInfo.IsValid)
                        {
                            InternalTryGetEntireHand(jointPosesL, HandTracking.LeftHandTrackInfo);
                            jointPoses = jointPosesL;
                            return true;
                        }
                        else
                        {
                            jointPoses = jointPosesL;
                            return false;
                        }

                    case XRNode.RightHand:
                        if (HandTracking.RightHandTrackInfo.IsValid)
                        {
                            InternalTryGetEntireHand(jointPosesR, HandTracking.RightHandTrackInfo);
                            jointPoses = jointPosesR;
                            return true;
                        }
                        else
                        {
                            jointPoses = jointPosesR;
                            return false;
                        }
                    default:
                        jointPoses = default;
                        return false;
                }
            }

            public override bool TryGetJoint(TrackedHandJoint joint, XRNode hand, out HandJointPose jointPose)
            {
                switch (hand)
                {
                    case XRNode.LeftHand:
                        if (HandTracking.LeftHandTrackInfo.IsValid)
                        {
                            InternalTryGetJoint(joint, HandTracking.LeftHandTrackInfo, out jointPose);
                            return true;
                        }
                        else
                        {
                            jointPose = default;
                            return false;
                        }

                    case XRNode.RightHand:
                        if (HandTracking.RightHandTrackInfo.IsValid)
                        {
                            InternalTryGetJoint(joint, HandTracking.RightHandTrackInfo, out jointPose);
                            return true;
                        }
                        else
                        {
                            jointPose = default;
                            return false;
                        }
                    default:
                        jointPose = default;
                        return false;
                }
            }

            public override bool TryGetNearInteractionPoint(XRNode hand, out HandJointPose jointPose)
            {
                return TryGetJoint(TrackedHandJoint.IndexTip, hand, out jointPose);
            }

            public override bool TryGetPalmFacingAway(XRNode hand, out bool palmFacingAway)
            {
                switch (hand)
                {
                    case XRNode.LeftHand:
                        if (HandTracking.LeftHandTrackInfo.IsValid)
                        {
                            palmFacingAway = HandTracking.LeftHandTrackInfo.IsPalmFacingTowardsUser == false;
                            return true;
                        }
                        else
                        {
                            palmFacingAway = false;
                            return false;
                        }
                    case XRNode.RightHand:
                        if (HandTracking.RightHandTrackInfo.IsValid)
                        {
                            palmFacingAway = HandTracking.RightHandTrackInfo.IsPalmFacingTowardsUser == false;
                            return true;
                        }
                        else
                        {
                            palmFacingAway = false;
                            return false;
                        }
                    default:
                        palmFacingAway = false;
                        return false;
                }
            }

            public override bool TryGetPinchingPoint(XRNode hand, out HandJointPose jointPose)
            {
                switch (hand)
                {
                    case XRNode.LeftHand:
                        if (HandTracking.LeftHandTrackInfo.IsValid)
                        {
                            jointPose = new HandJointPose(HandAnchorInputDevice.left.InputState_World_Space.anchorPosition, Quaternion.identity, 0.016f);
                            return true;
                        }
                        else
                        {
                            jointPose = default;
                            return false;
                        }
                    case XRNode.RightHand:
                        if (HandTracking.RightHandTrackInfo.IsValid)
                        {
                            jointPose = new HandJointPose(HandAnchorInputDevice.right.InputState_World_Space.anchorPosition, Quaternion.identity, 0.016f);
                            return true;
                        }
                        else
                        {
                            jointPose = default;
                            return false;
                        }
                    default:
                        jointPose = default;
                        return false;
                }
            }

            public override bool TryGetPinchProgress(XRNode hand, out bool isReadyToPinch, out bool isPinching, out float pinchAmount)
            {
                switch (hand)
                {
                    case XRNode.LeftHand:
                        if (HandTracking.LeftHandTrackInfo.IsValid)
                        {
                            isReadyToPinch = true;
                            isPinching = HandAnchorInputDevice.left.InputState_World_Space.isGrip;
                            pinchAmount = HandAnchorInputDevice.left.InputState_World_Space.gripValue;
                            return true;
                        }
                        else
                        {
                            isReadyToPinch = false;
                            isPinching = false;
                            pinchAmount = 0.0f;
                            return false;
                        }
                    case XRNode.RightHand:
                        if (HandTracking.RightHandTrackInfo.IsValid)
                        {
                            isReadyToPinch = true;
                            isPinching = HandAnchorInputDevice.right.InputState_World_Space.isGrip;
                            pinchAmount = HandAnchorInputDevice.right.InputState_World_Space.gripValue;
                            return true;
                        }
                        else
                        {
                            isReadyToPinch = false;
                            isPinching = false;
                            pinchAmount = 0.0f;
                            return false;
                        }
                    default:
                        isReadyToPinch = false;
                        isPinching = false;
                        pinchAmount = 0.0f;
                        return false;
                }
            }

            #region Internal functions

            /// <summary>
            /// Internal get entire hand joint poses.
            /// https://learn.microsoft.com/en-us/dotnet/api/microsoft.mixedreality.toolkit.subsystems.handsaggregatorsubsystem.trygetentirehand?view=mrtkcore-3.0
            /// https://learn.microsoft.com/en-us/dotnet/api/microsoft.mixedreality.toolkit.trackedhandjoint?view=mrtkcore-3.0
            /// </summary>
            /// <param name="joints"></param>
            /// <param name="handTrackingInfo"></param>
            private void InternalTryGetEntireHand(List<HandJointPose> joints, HandTrackingInfo handTrackingInfo)
            {
                if (handTrackingInfo.IsValid == false)
                {
                    return;
                }
                RawFingerTrackingInfo thumbFinger = handTrackingInfo.ThumbFinger;
                RawFingerTrackingInfo indexFinger = handTrackingInfo.IndexFinger;
                RawFingerTrackingInfo middleFinger = handTrackingInfo.MiddleFinger;
                RawFingerTrackingInfo ringFinger = handTrackingInfo.RingFinger;
                RawFingerTrackingInfo littleFinger = handTrackingInfo.LittleFinger;

                const float kDefaultRadius = 0.016f;
                //0 : Palm (Note: In MRTK system, hand normal is flipped to the Xim hand system)
                joints[0] = new HandJointPose(handTrackingInfo.PalmPosition, handTrackingInfo.PalmRotation * Quaternion.Euler(0, 0, 180), handTrackingInfo.PalmScale.x);
                //1 : Wrist
                joints[1] = new HandJointPose(handTrackingInfo.WristPosition, handTrackingInfo.PalmRotation, kDefaultRadius);
                //2 : Thumb Metacarpal
                joints[2] = new HandJointPose(Vector3.Lerp(handTrackingInfo.WristPosition, thumbFinger.Positions[0], 0.75f), Quaternion.LookRotation(thumbFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                //3 : Thumb Proximal
                joints[3] = new HandJointPose(thumbFinger.Positions[0], Quaternion.LookRotation(thumbFinger.Positions[1] - thumbFinger.Positions[0]), kDefaultRadius);
                //4 : ThumbDistal (The thumb's first (furthest) joint.)
                joints[4] = new HandJointPose(thumbFinger.Positions[1], Quaternion.LookRotation(thumbFinger.Positions[2] - thumbFinger.Positions[1]), kDefaultRadius);
                //5 : ThumbTip  The tip of the thumb.
                joints[5] = new HandJointPose(thumbFinger.Positions[2], Quaternion.LookRotation(thumbFinger.Positions[2] - thumbFinger.Positions[1]), kDefaultRadius);
                //6 : Index Metacarpal
                joints[6] = new HandJointPose(Vector3.Lerp(handTrackingInfo.WristPosition, indexFinger.Positions[0], 0.75f), Quaternion.LookRotation(indexFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                //7 : IndexProximal
                joints[7] = new HandJointPose(indexFinger.Positions[0], Quaternion.LookRotation(indexFinger.Positions[1] - indexFinger.Positions[0]), kDefaultRadius);
                //8 : Index Intermediate
                joints[8] = new HandJointPose(indexFinger.Positions[1], Quaternion.LookRotation(indexFinger.Positions[2] - indexFinger.Positions[1]), kDefaultRadius);
                //9 : Index Distal
                joints[9] = new HandJointPose(indexFinger.Positions[2], Quaternion.LookRotation(indexFinger.Positions[3] - indexFinger.Positions[2]), kDefaultRadius);
                //10 : Index Tip
                joints[10] = new HandJointPose(indexFinger.Positions[3], Quaternion.LookRotation(indexFinger.Positions[3] - indexFinger.Positions[2]), kDefaultRadius);
                //11 : MiddleMetacarpal
                joints[11] = new HandJointPose(Vector3.Lerp(handTrackingInfo.WristPosition, middleFinger.Positions[0], 0.75f), Quaternion.LookRotation(middleFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                //12 :  Middle Proximal
                joints[12] = new HandJointPose(middleFinger.Positions[0], Quaternion.LookRotation(middleFinger.Positions[1] - middleFinger.Positions[0]), kDefaultRadius);
                //13 : Middle Intermediate
                joints[13] = new HandJointPose(middleFinger.Positions[1], Quaternion.LookRotation(middleFinger.Positions[2] - middleFinger.Positions[1]), kDefaultRadius);
                //14 : Middle Distal
                joints[14] = new HandJointPose(middleFinger.Positions[2], Quaternion.LookRotation(middleFinger.Positions[3] - middleFinger.Positions[2]), kDefaultRadius);
                //15 :  Middle Tip
                joints[15] = new HandJointPose(middleFinger.Positions[3], Quaternion.LookRotation(middleFinger.Positions[3] - middleFinger.Positions[2]), kDefaultRadius);
                //16 : Ring Metacarpal
                joints[16] = new HandJointPose(Vector3.Lerp(handTrackingInfo.WristPosition, ringFinger.Positions[0], 0.75f), Quaternion.LookRotation(ringFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                //17 : Ring Proximal
                joints[17] = new HandJointPose(ringFinger.Positions[0], Quaternion.LookRotation(ringFinger.Positions[1] - ringFinger.Positions[0]), kDefaultRadius);
                //18 : Ring Intermediate 
                joints[18] = new HandJointPose(ringFinger.Positions[1], Quaternion.LookRotation(ringFinger.Positions[2] - ringFinger.Positions[1]), kDefaultRadius);
                //19 : Ring Distal
                joints[19] = new HandJointPose(ringFinger.Positions[2], Quaternion.LookRotation(ringFinger.Positions[3] - ringFinger.Positions[2]), kDefaultRadius);
                //20 : Ring Tip
                joints[20] = new HandJointPose(ringFinger.Positions[3], Quaternion.LookRotation(ringFinger.Positions[3] - ringFinger.Positions[2]), kDefaultRadius);
                //21 : Little Metacarpal
                joints[21] = new HandJointPose(Vector3.Lerp(handTrackingInfo.WristPosition, littleFinger.Positions[0], 0.75f), Quaternion.LookRotation(littleFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                //22 : Little Proximal
                joints[22] = new HandJointPose(littleFinger.Positions[0], Quaternion.LookRotation(littleFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                //23 : Little Intermediate
                joints[23] = new HandJointPose(littleFinger.Positions[1], Quaternion.LookRotation(littleFinger.Positions[1] - littleFinger.Positions[0]), kDefaultRadius);
                //24 : Little Distal
                joints[24] = new HandJointPose(littleFinger.Positions[2], Quaternion.LookRotation(littleFinger.Positions[2] - littleFinger.Positions[1]), kDefaultRadius);
                //25 : Little Tip
                joints[25] = new HandJointPose(littleFinger.Positions[3], Quaternion.LookRotation(littleFinger.Positions[3] - littleFinger.Positions[2]), kDefaultRadius);
            }

            private void InternalTryGetJoint(TrackedHandJoint joint, HandTrackingInfo handTrackingInfo, out HandJointPose jointPose)
            {
                const float kDefaultRadius = 0.016f;
                RawFingerTrackingInfo thumbFinger = handTrackingInfo.ThumbFinger;
                RawFingerTrackingInfo indexFinger = handTrackingInfo.IndexFinger;
                RawFingerTrackingInfo middleFinger = handTrackingInfo.MiddleFinger;
                RawFingerTrackingInfo ringFinger = handTrackingInfo.RingFinger;
                RawFingerTrackingInfo littleFinger = handTrackingInfo.LittleFinger;
                switch (joint)
                {
                    case TrackedHandJoint.Palm:
                        jointPose = new HandJointPose(handTrackingInfo.PalmPosition, handTrackingInfo.PalmRotation * Quaternion.Euler(0, 0, 180), handTrackingInfo.PalmScale.x);
                        break;

                    case TrackedHandJoint.Wrist:
                        jointPose = new HandJointPose(handTrackingInfo.WristPosition, handTrackingInfo.PalmRotation, kDefaultRadius);
                        break;

                    case TrackedHandJoint.ThumbMetacarpal:
                        jointPose = new HandJointPose(Vector3.Lerp(handTrackingInfo.WristPosition, thumbFinger.Positions[0], 0.75f), Quaternion.LookRotation(thumbFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                        break;

                    case TrackedHandJoint.ThumbProximal:
                        jointPose = new HandJointPose(thumbFinger.Positions[0], Quaternion.LookRotation(thumbFinger.Positions[1] - thumbFinger.Positions[0]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.ThumbDistal:
                        jointPose = new HandJointPose(thumbFinger.Positions[1], Quaternion.LookRotation(thumbFinger.Positions[2] - thumbFinger.Positions[1]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.ThumbTip:
                        jointPose = new HandJointPose(thumbFinger.Positions[2], Quaternion.LookRotation(thumbFinger.Positions[2] - thumbFinger.Positions[1]), kDefaultRadius);
                        break;


                    case TrackedHandJoint.IndexMetacarpal:
                        jointPose = new HandJointPose(Vector3.Lerp(handTrackingInfo.WristPosition, indexFinger.Positions[0], 0.75f), Quaternion.LookRotation(indexFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                        break;
                    case TrackedHandJoint.IndexProximal:
                        jointPose = new HandJointPose(indexFinger.Positions[0], Quaternion.LookRotation(indexFinger.Positions[1] - indexFinger.Positions[0]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.IndexIntermediate:
                        jointPose = new HandJointPose(indexFinger.Positions[1], Quaternion.LookRotation(indexFinger.Positions[2] - indexFinger.Positions[1]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.IndexDistal:
                        jointPose = new HandJointPose(indexFinger.Positions[2], Quaternion.LookRotation(indexFinger.Positions[3] - indexFinger.Positions[2]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.IndexTip:
                        jointPose = new HandJointPose(indexFinger.Positions[3], Quaternion.LookRotation(indexFinger.Positions[3] - indexFinger.Positions[2]), kDefaultRadius);
                        break;


                    case TrackedHandJoint.MiddleMetacarpal:
                        jointPose = new HandJointPose(Vector3.Lerp(handTrackingInfo.WristPosition, middleFinger.Positions[0], 0.75f), Quaternion.LookRotation(middleFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                        break;
                    case TrackedHandJoint.MiddleProximal:
                        jointPose = new HandJointPose(middleFinger.Positions[0], Quaternion.LookRotation(middleFinger.Positions[1] - middleFinger.Positions[0]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.MiddleIntermediate:
                        jointPose = new HandJointPose(middleFinger.Positions[1], Quaternion.LookRotation(middleFinger.Positions[2] - middleFinger.Positions[1]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.MiddleDistal:
                        jointPose = new HandJointPose(middleFinger.Positions[2], Quaternion.LookRotation(middleFinger.Positions[3] - middleFinger.Positions[2]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.MiddleTip:
                        jointPose = new HandJointPose(middleFinger.Positions[3], Quaternion.LookRotation(middleFinger.Positions[3] - middleFinger.Positions[2]), kDefaultRadius);
                        break;


                    case TrackedHandJoint.RingMetacarpal:
                        jointPose = new HandJointPose(Vector3.Lerp(handTrackingInfo.WristPosition, ringFinger.Positions[0], 0.75f), Quaternion.LookRotation(ringFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                        break;
                    case TrackedHandJoint.RingProximal:
                        jointPose = new HandJointPose(ringFinger.Positions[0], Quaternion.LookRotation(ringFinger.Positions[1] - ringFinger.Positions[0]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.RingIntermediate:
                        jointPose = new HandJointPose(ringFinger.Positions[1], Quaternion.LookRotation(ringFinger.Positions[2] - ringFinger.Positions[1]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.RingDistal:
                        jointPose = new HandJointPose(ringFinger.Positions[2], Quaternion.LookRotation(ringFinger.Positions[3] - ringFinger.Positions[2]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.RingTip:
                        jointPose = new HandJointPose(ringFinger.Positions[3], Quaternion.LookRotation(ringFinger.Positions[3] - ringFinger.Positions[2]), kDefaultRadius);
                        break;



                    case TrackedHandJoint.LittleMetacarpal:
                        jointPose = new HandJointPose(Vector3.Lerp(handTrackingInfo.WristPosition, littleFinger.Positions[0], 0.75f), Quaternion.LookRotation(littleFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                        break;
                    case TrackedHandJoint.LittleProximal:
                        jointPose = new HandJointPose(littleFinger.Positions[0], Quaternion.LookRotation(littleFinger.Positions[0] - handTrackingInfo.WristPosition), kDefaultRadius);
                        break;

                    case TrackedHandJoint.LittleIntermediate:
                        jointPose = new HandJointPose(littleFinger.Positions[1], Quaternion.LookRotation(littleFinger.Positions[1] - littleFinger.Positions[0]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.LittleDistal:
                        jointPose = new HandJointPose(littleFinger.Positions[2], Quaternion.LookRotation(littleFinger.Positions[2] - littleFinger.Positions[1]), kDefaultRadius);
                        break;

                    case TrackedHandJoint.LittleTip:
                        jointPose = new HandJointPose(littleFinger.Positions[3], Quaternion.LookRotation(littleFinger.Positions[3] - littleFinger.Positions[2]), kDefaultRadius);
                        break;


                    default:
                        jointPose = new HandJointPose(handTrackingInfo.WristPosition, handTrackingInfo.PalmRotation, kDefaultRadius); //default returns Palm pose
                        break;
                }
            }

            #endregion
        }




    }
}
