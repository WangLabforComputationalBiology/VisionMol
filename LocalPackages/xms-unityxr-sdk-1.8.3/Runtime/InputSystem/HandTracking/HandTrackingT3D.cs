using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using System.Runtime.InteropServices;
using TouchlessA3D;
using System;
using System.Xml;
using System.Text;
using Unity.Collections;
using UnityEngine.Profiling;

namespace Ximmerse.XR.InputSystems
{
    using static NativeCalls;

    /// <summary>
    /// Hand tracking module implementor on touchless 3D SDK.
    /// </summary>
    public partial class HandTrackingT3D : I_HandleTrackingModule
    {
        internal static HandTrackingT3D instance;

        private readonly static int width = 1440;

        private readonly static int height = 1080; //264

        /// <summary>
        /// A 3 x 3 float array to calibrate RGB camera.
        /// </summary>
        static float[,] CalibrationMatrix = null;

        /// <summary>
        /// A 8 array to identify the distortion coefficients of the RGB camera.
        /// </summary>
        static float[] DistortionCoefficients = null;


        /// <summary>
        /// Image data array.
        /// </summary>
        private Color32[] imgData;

        private bool m_IsModuleEnabled;



        /// <summary>
        /// RGB camera texture to be passed to native plugin.
        /// </summary>
        private WebCamTexture cameraTexture;

        private GCHandle imageHandle;

        public bool IsModuleEnabled
        {
            get => m_IsModuleEnabled;
        }

        private Engine touchlessEngine;

        Transform handTrackingAnchor;

        Transform mainCamera;

        Camera mainCam;

        Matrix4x4 hand_raw_2_world;

        /// <summary>
        /// Max cache time
        /// </summary>
        const float kMaxCacheTime = 0.5f;

        /// <summary>
        /// Hand tracking cache pose, first element = earlier , last element = latest
        /// </summary>
        List<HandTrackingDataCache> leftHandCache = new List<HandTrackingDataCache>(), rightHandCache = new List<HandTrackingDataCache>();

        /// <summary>
        /// Left and right hand swing cache.
        /// </summary>
        SwingCacheList leftHandSwingCache = new SwingCacheList(), rightHandSwingCache = new SwingCacheList();

        /// <summary>
        /// Raw hand track info by native plugin.
        /// </summary>
        HandTrackingInfo leftHandTrackInfo, rightHandTrackInfo;

        /// <summary>
        /// 上一个合法帧的hand track info.
        /// </summary>
        HandTrackingInfo prevFrameLeftHandTrackInfo, prevFrameRightHandTrackInfo;

        /// <summary>
        /// Gets the left hand track info
        /// </summary>
        public HandTrackingInfo LeftHandTrackingInfo
        {
            get => leftHandTrackInfo;
        }

        /// <summary>
        /// Gets the right hand track info
        /// </summary>
        public HandTrackingInfo RightHandTrackingInfo
        {
            get => rightHandTrackInfo;
        }


        /// <summary>
        /// Gets the previous frame left hand track info
        /// </summary>
        public HandTrackingInfo PrevLeftHandTrackingInfo
        {
            get => prevFrameLeftHandTrackInfo;
        }


        /// <summary>
        /// Gets the previous frame right hand track info
        /// </summary>
        public HandTrackingInfo PrevRightHandTrackingInfo
        {
            get => prevFrameRightHandTrackInfo;
        }

        private readonly object lockObj = new object();
        GestureEvent locked_gestureEvent;

        bool smoothHandRotation;

        Vector2 smoothHandRotationAngleDiffRange;

        float smoothHandRotationAngularSpeed = 120;

        AnimationCurve smoothHandRotationCurve;

        /// <summary>
        /// If true, use the hand track service instead of local API.
        /// </summary>
        public static bool UseHandTrackService = true;

        [Tooltip("if true, hand tracking lost event will be delay one-frame fired. Used in low frame rate app.")]
        public static bool DelayOneFrameLostTracking = false;


        [Tooltip("if true, hand tracking show raw data skeleton.")]
        public static bool m_IsShowSkeleton = false;

        /// <summary>
        /// Service data receiver struct
        /// </summary>
        HandClientData leftHandServiceData, rightHandServiceData;

        /// <summary>
        /// 22 bones data at service runtime.
        /// </summary>
        Vector3[] leftHandRawPoints, rightHandRawPoints;

        /// <summary>
        /// Left skel and right skel is the raw hand tracking SDK API data structure.
        /// </summary>
        internal ta3d_skeleton_3d_s leftSkel = default, rightSkel = default;

        internal ta3d_skeleton_3d_s GetRawSkeleton(HandnessType handness)
        {
            return handness == HandnessType.Left ? leftSkel : rightSkel;
        }

        /// <summary>
        /// Left skel and right skel is the raw hand tracking SDK API 2d data structure.
        /// </summary>
        internal ta3d_skeleton_2d_s leftSkel2d = default, rightSkel2d = default;

        internal ta3d_skeleton_2d_s GetRawSkeleton2d(HandnessType handness)
        {
            return handness == HandnessType.Left ? leftSkel2d : rightSkel2d;
        }

        /// <summary>
        /// 手掌朝向的固定偏移角度。
        /// </summary>
        public static Quaternion PalmQFixedOffsetL = Quaternion.Euler(-7.952f, 28.375f, -7.72f);
        public static Quaternion PalmQFixedOffsetR = Quaternion.Euler(-11f, -30.4f, 9.5f);

        /// <summary>
        /// 手掌位置的固定偏移向量。
        /// </summary>
        public static Vector3 PalmTFixedOffsetL = new Vector3(-0.1339f, 0.1065f, -0.2183f);
        public static Vector3 PalmTFixedOffsetR = new Vector3(0.0528f, 0.0901f, -0.293f);
        public void DisableModule()
        {
#if DEVELOPMENT_BUILD
            try
            {
#endif
                if (!m_IsModuleEnabled)
                {
                    return;
                }
                m_IsModuleEnabled = false;
                leftHandTrackInfo.Dispose();
                rightHandTrackInfo.Dispose();
                prevFrameLeftHandTrackInfo.Dispose();
                prevFrameRightHandTrackInfo.Dispose();

                if (!UseHandTrackService)
                {
                    XimmerseXR.RequestStopRGBCamera();
                    touchlessEngine = null;
                    imageHandle.Free();
                }
                else
                {
                    HandTrackClient.Exit();
                    Debug.Log("Hand track client exits.");
                }

#if DEVELOPMENT_BUILD
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#endif
        }


        public bool EnableModule(InitializeHandTrackingModuleParameter initParameter)
        {
            instance = this;
            if (UseHandTrackService) //Enable hand tracking module using service
            {
                if (m_IsModuleEnabled)
                {
                    Debug.Log("HandTrackingT3D module already activate.");
                    return false;
                }



                this.handTrackingAnchor = initParameter.TrackingAnchor;
                this.smoothHandRotation = initParameter.smoothHandRotation;
                this.smoothHandRotationAngleDiffRange = initParameter.smoothAngleRange;
                this.smoothHandRotationCurve = initParameter.smoothControlCurve;
                this.smoothHandRotationAngularSpeed = initParameter.smoothHandRotationAngularSpeed;

                //Init service data receiver struct:
                if (leftHandServiceData.points == null)
                {
                    leftHandServiceData = new HandClientData(new float[66]);
                    leftHandRawPoints = new Vector3[22];
                }
                if (rightHandServiceData.points == null)
                {
                    rightHandServiceData = new HandClientData(new float[66]);
                    rightHandRawPoints = new Vector3[22];
                }



                leftHandTrackInfo = new HandTrackingInfo()
                {
                    ThumbFinger = new RawFingerTrackingInfo(3)
                    {
                        bendnessRangeMin = 0.7f,
                        bendnessRangeMax = 0.9f,
                    },
                    IndexFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    MiddleFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    RingFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    LittleFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                };

                rightHandTrackInfo = new HandTrackingInfo()
                {
                    ThumbFinger = new RawFingerTrackingInfo(3)
                    {
                        bendnessRangeMin = 0.7f,
                        bendnessRangeMax = 0.9f,
                    },
                    IndexFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    MiddleFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    RingFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    LittleFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                };

                prevFrameLeftHandTrackInfo = new HandTrackingInfo()
                {
                    ThumbFinger = new RawFingerTrackingInfo(3)
                    {
                        bendnessRangeMin = 0.7f,
                        bendnessRangeMax = 0.9f,
                    },
                    IndexFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    MiddleFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    RingFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    LittleFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                };

                prevFrameRightHandTrackInfo = new HandTrackingInfo()
                {
                    ThumbFinger = new RawFingerTrackingInfo(3)
                    {
                        bendnessRangeMin = 0.7f,
                        bendnessRangeMax = 0.9f,
                    },
                    IndexFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    MiddleFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    RingFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                    LittleFinger = new RawFingerTrackingInfo(4)
                    {
                        bendnessRangeMin = 0.1f,
                        bendnessRangeMax = 0.9f,
                    },
                };

                int ret = HandTrackClient.Init();
                Debug.LogFormat("Init hand track service result : {0}", ret);

                m_IsModuleEnabled = ret == 0;
                return ret == 0;
            }
            else //using local library
            {

                if (DistortionCoefficients == null || CalibrationMatrix == null)
                {
                    RGBCameraUtils.ParseRGBCameraParams(out CalibrationMatrix, out DistortionCoefficients);
                }

#if DEVELOPMENT_BUILD
                try
                {
#endif
                    if (m_IsModuleEnabled)
                    {
                        Debug.Log("HandTrackingT3D module already activate.");
                        return false;
                    }
                    handTrackingAnchor = initParameter.TrackingAnchor;
                    if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                    {
                        Permission.RequestUserPermission(Permission.Camera);
                    }

                    smoothHandRotation = initParameter.smoothHandRotation;
                    this.smoothHandRotationAngleDiffRange = initParameter.smoothAngleRange;
                    this.smoothHandRotationCurve = initParameter.smoothControlCurve;
                    this.smoothHandRotationAngularSpeed = initParameter.smoothHandRotationAngularSpeed;

                    XimmerseXR.RequestOpenRGBCamera(width, height);
                    cameraTexture = XimmerseXR.RGBCameraTexture;

                    if (imgData == null)
                    {
                        imgData = new Color32[width * height];
                    }
                    imageHandle = GCHandle.Alloc(imgData, GCHandleType.Pinned);
                    string uniqueID = SystemInfo.deviceUniqueIdentifier;
                    string storageLocation = Application.persistentDataPath;

                    var calibration = new Calibration(1440, 1080, CalibrationMatrix, DistortionCoefficients);
                    touchlessEngine = new Engine(uniqueID, storageLocation, calibration);



                    leftHandTrackInfo = new HandTrackingInfo()
                    {
                        ThumbFinger = new RawFingerTrackingInfo(3)
                        {
                            bendnessRangeMin = 0.7f,
                            bendnessRangeMax = 0.9f,
                        },
                        IndexFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        MiddleFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        RingFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        LittleFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                    };

                    rightHandTrackInfo = new HandTrackingInfo()
                    {
                        ThumbFinger = new RawFingerTrackingInfo(3)
                        {
                            bendnessRangeMin = 0.7f,
                            bendnessRangeMax = 0.9f,
                        },
                        IndexFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        MiddleFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        RingFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        LittleFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                    };

                    prevFrameLeftHandTrackInfo = new HandTrackingInfo()
                    {
                        ThumbFinger = new RawFingerTrackingInfo(3)
                        {
                            bendnessRangeMin = 0.7f,
                            bendnessRangeMax = 0.9f,
                        },
                        IndexFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        MiddleFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        RingFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        LittleFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                    };

                    prevFrameRightHandTrackInfo = new HandTrackingInfo()
                    {
                        ThumbFinger = new RawFingerTrackingInfo(3)
                        {
                            bendnessRangeMin = 0.7f,
                            bendnessRangeMax = 0.9f,
                        },
                        IndexFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        MiddleFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        RingFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                        LittleFinger = new RawFingerTrackingInfo(4)
                        {
                            bendnessRangeMin = 0.1f,
                            bendnessRangeMax = 0.9f,
                        },
                    };

                    //if (leftHand == null)
                    //{
                    //    leftHand = new Hand(HandednessType.LEFT_HAND);
                    //}
                    //if (rightHand == null)
                    //{
                    //    rightHand = new Hand(HandednessType.RIGHT_HAND);
                    //}
                    m_IsModuleEnabled = true;

                    Debug.Log("T3D hand tracking is now active.");

                    return true;

#if DEVELOPMENT_BUILD
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return false;
#endif
            }
        }

        /// <summary>
        /// Call per frame , in main thread.
        /// </summary>
        public void Tick()
        {
#if DEVELOPMENT_BUILD
            try
            {
#endif
                Profiler.BeginSample("HandTrackingT3D.Tick().Part.1");
                //Calculate  local position to main camera:
                if (!mainCamera)
                {
                    mainCamera = Camera.main.transform;
                }
                if (!mainCam)
                {
                    mainCam = Camera.main;
                }

                if (!handTrackingAnchor)
                {
                    Debug.LogErrorFormat("Hand tracking anchor == NULL");
                    return;
                }
                //Matrix convert raw tracked data to world space, when using hand track service, raw data in view space, when using local app library, raw data in tracking anchor space:
                hand_raw_2_world = UseHandTrackService ? mainCam.transform.localToWorldMatrix : handTrackingAnchor.localToWorldMatrix;

                //Matrix convert raw tracked data to head space:
                Matrix4x4 raw_2_head = Matrix4x4.TRS(mainCamera.InverseTransformPoint(handTrackingAnchor.position), Quaternion.Inverse(mainCamera.rotation) * handTrackingAnchor.rotation, Vector3.one);

                if (UseHandTrackService)
                {
                    leftHandServiceData.Reset();
                    rightHandServiceData.Reset();
                    bool leftGet = HandTrackClient.GetEvent(HandType.LEFT_HAND, ref this.leftHandServiceData) && leftHandServiceData.timestamp != 0;
                    bool rightGet = HandTrackClient.GetEvent(HandType.RIGHT_HAND, ref this.rightHandServiceData) && rightHandServiceData.timestamp != 0;

                    leftSkel = default;
                    rightSkel = default;

                    ta3d_skeleton_2d_s leftSkel2d = default;
                    ta3d_skeleton_2d_s rightSkel2d = default;

                    GestureType leftGesture = GestureType.NO_HAND;
                    GestureType rightGesture = GestureType.NO_HAND;

                    bool flipped = UseHandTrackService ? false : true;
                    if (leftGet)
                    {
                        leftHandServiceData.GetSkeleton(out leftSkel, out leftSkel2d, mainCam.worldToCameraMatrix);
                        leftGesture = (GestureType)leftHandServiceData.Gestureflag;
                        leftHandServiceData.CopyTo(leftHandRawPoints);
                        for (int i = 0; i < 21; i++)
                        {
                            Vector3 rawPoint = leftHandRawPoints[i];
                            leftSkel.SetPosition(i, rawPoint);
                            //Vector3 bone = hand_raw_2_world.MultiplyPoint3x4(leftHandRawPoints[i]);
                            Vector3 bone = hand_raw_2_world.MultiplyPoint3x4(leftSkel.GetPosition((NativePointID)i, flipped));
                            if (m_IsShowSkeleton && leftGesture!= GestureType.NO_HAND)
                            {
                                Ximmerse.XR.Utils.RxDraw.DrawSphere(bone, 0.009f, Color.white, 0);
                                // Ximmerse.XR.Utils.RxDraw.Text3D(bone, mainCamera.rotation, 0.006f, i.ToString(), Color.red, 0, XimmerseXRSettings.instance.debugFont);
                            }

                        }
                    }
                    if (rightGet)
                    {
                        rightHandServiceData.GetSkeleton(out rightSkel, out rightSkel2d, mainCam.worldToCameraMatrix);
                        rightGesture = (GestureType)rightHandServiceData.Gestureflag;
                        rightHandServiceData.CopyTo(rightHandRawPoints);
                        for (int i = 0; i < 21; i++)
                        {
                            Vector3 rawPoint = rightHandRawPoints[i];
                            rightSkel.SetPosition(i, rawPoint);
                            //Vector3 bone = hand_raw_2_world.MultiplyPoint3x4(leftHandRawPoints[i]);
                            Vector3 bone = hand_raw_2_world.MultiplyPoint3x4(rightSkel.GetPosition((NativePointID)i, flipped));
                            if (m_IsShowSkeleton && leftGesture != GestureType.NO_HAND)
                            {
                                Ximmerse.XR.Utils.RxDraw.DrawSphere(bone, 0.009f, Color.white, 0);
                                // Ximmerse.XR.Utils.RxDraw.Text3D(bone, mainCamera.rotation, 0.006f, i.ToString(), Color.red, 0, XimmerseXRSettings.instance.debugFont);
                            }
                        }
                    }
                    UpdateHand(ref this.leftHandTrackInfo, ref this.prevFrameLeftHandTrackInfo, leftSkel, leftSkel2d, leftGesture, 0, hand_raw_2_world, raw_2_head, this.leftHandCache, this.leftHandSwingCache);
                    UpdateHand(ref this.rightHandTrackInfo, ref this.prevFrameRightHandTrackInfo, rightSkel, rightSkel2d, rightGesture, 1, hand_raw_2_world, raw_2_head, this.rightHandCache, this.rightHandSwingCache);

                    //Debug.LogFormat(
                    //    "Hand track service: left timestamp : {0}, right timestamp : {1}, leftHandTrackInfo valid: {2}, rightHandTrackInfo valid: {3}",
                    //    leftHandServiceData.timestamp, rightHandServiceData.timestamp,
                    //    leftHandTrackInfo.IsValid,
                    //    rightHandTrackInfo.IsValid
                    //    );
                }
                else
                {

                    //update the tracking anchor local to world matrix
                    if (imgData == null)
                    {
                        imgData = new Color32[width * height];
                    }
                    cameraTexture.GetPixels32(imgData);

                    using (var frame = new Frame(imageHandle.AddrOfPinnedObject(), cameraTexture.width * 4, cameraTexture.width, cameraTexture.height, System.DateTimeOffset.Now.ToUnixTimeMilliseconds(), FrameRotation.ROTATION_NONE, true))
                    {
                        touchlessEngine.handleFrame2(frame);
                    }

                    touchlessEngine.UpdateHands(
                        out leftSkel, out leftSkel2d, out GestureType leftGesture,
                        out rightSkel, out rightSkel2d, out GestureType rightGesture,
                        out long evntTime, predict: true, predictEventLifeTime: 150
                    );
                    UpdateHand(ref this.leftHandTrackInfo, ref this.prevFrameLeftHandTrackInfo, leftSkel, leftSkel2d, leftGesture, 0, hand_raw_2_world, raw_2_head, this.leftHandCache, this.leftHandSwingCache);
                    UpdateHand(ref this.rightHandTrackInfo, ref this.prevFrameRightHandTrackInfo, rightSkel, rightSkel2d, rightGesture, 1, hand_raw_2_world, raw_2_head, this.rightHandCache, this.rightHandSwingCache);
                }



                Profiler.EndSample();
#if DEVELOPMENT_BUILD
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#endif
        }

        /// <summary>
        /// Update hand tracking info
        /// </summary>
        /// <param name="handTrackInfo"></param>
        /// <param name="prevFrame">上一帧的数据</param>
        /// <param name="gestureType"></param>
        /// <param name="skeleton"></param>
        /// <param name="skeleton_2D"></param>
        /// <param name="index"> 0 = left, 1 = right</param>
        /// <param name="hand_raw_2_world">Matrix converts raw track data to world space.</param>
        /// <param name="hand_raw_to_head">Matrix converts raw track data to head space.</param>
        /// <param name="cacheList">Hand tracking cache list.</param>
        /// <param name="swingCacheList">Swing hand cache list.</param>
        private void UpdateHand(ref HandTrackingInfo handTrackInfo, ref HandTrackingInfo prevFrame, ta3d_skeleton_3d_s skeleton, ta3d_skeleton_2d_s skeleton_2D, GestureType gestureType, int index, Matrix4x4 hand_raw_2_world, Matrix4x4 hand_raw_to_head, List<HandTrackingDataCache> cacheList, SwingCacheList swingCacheList)
        {

            //Calculate  local position to main camera:
            if (!mainCamera)
            {
                mainCamera = Camera.main.transform;
            }
            if (!mainCam)
            {
                mainCam = Camera.main;
            }

            handTrackInfo.Handness = index == 0 ? HandnessType.Left : HandnessType.Right;
            handTrackInfo.Timestamp = DateTime.Now.Ticks;
            //Invalid hand tracking info:
            if (skeleton.status != ResultType.RESULT_OK  || !mainCamera || gestureType == GestureType.NO_HAND) //skip when no valid frame or no main camera
            {
                handTrackInfo.IsValid = false;
                handTrackInfo.PalmDeltaEuler = Vector3.zero;
                handTrackInfo.PalmDeltaPosition = Vector3.zero;
                if (DelayOneFrameLostTracking && (Time.frameCount - handTrackInfo.prevLostTrackingFrame) > 1)//如果持续性的丢跟踪， 才将 isValid 置为 false
                {
                    handTrackInfo.IsValid = true;
                }
                handTrackInfo.prevLostTrackingFrame = Time.frameCount; //记录下最近一次丢跟踪的帧
                prevFrame.CopyFrom(handTrackInfo);
                return;
            }
            handTrackInfo.IsValid = true;
            handTrackInfo.SwingFlag = 0;
            bool flipped = UseHandTrackService ? false : true;
            handTrackInfo.WristRawPosition = skeleton.GetPosition(NativePointID.WRIST, flipped);
            handTrackInfo.WristPosition2D = skeleton_2D.GetPosition(NativePointID.WRIST, flipped);
            handTrackInfo.IndexRootPosition2D = skeleton_2D.GetPosition(NativePointID.INDEX1, flipped);

            handTrackInfo.WristLocalPosition = hand_raw_to_head.MultiplyPoint3x4(handTrackInfo.WristRawPosition);
            handTrackInfo.WristPosition = hand_raw_2_world.MultiplyPoint3x4(handTrackInfo.WristRawPosition);

            handTrackInfo.ThumbFinger.Positions[0] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.THUMB2, flipped));
            handTrackInfo.ThumbFinger.Positions[1] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.THUMB3, flipped));
            handTrackInfo.ThumbFinger.Positions[2] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.THUMB_TIP, flipped));

            handTrackInfo.IndexFinger.Positions[0] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.INDEX1, flipped));
            handTrackInfo.IndexFinger.Positions[1] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.INDEX2, flipped));
            handTrackInfo.IndexFinger.Positions[2] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.INDEX3, flipped));
            handTrackInfo.IndexFinger.Positions[3] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.INDEX_TIP, flipped));


            handTrackInfo.MiddleFinger.Positions[0] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.MIDDLE1, flipped));
            handTrackInfo.MiddleFinger.Positions[1] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.MIDDLE2, flipped));
            handTrackInfo.MiddleFinger.Positions[2] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.MIDDLE3, flipped));
            handTrackInfo.MiddleFinger.Positions[3] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.MIDDLE_TIP, flipped));


            handTrackInfo.RingFinger.Positions[0] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.RING1, flipped));
            handTrackInfo.RingFinger.Positions[1] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.RING2, flipped));
            handTrackInfo.RingFinger.Positions[2] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.RING3, flipped));
            handTrackInfo.RingFinger.Positions[3] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.RING_TIP, flipped));


            handTrackInfo.LittleFinger.Positions[0] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.PINKY1, flipped));
            handTrackInfo.LittleFinger.Positions[1] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.PINKY2, flipped));
            handTrackInfo.LittleFinger.Positions[2] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.PINKY3, flipped));
            handTrackInfo.LittleFinger.Positions[3] = hand_raw_2_world.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.PINKY_TIP, flipped));

            //Update local positions: 
            {
                handTrackInfo.ThumbFinger.LocalPositions[0] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.THUMB2, flipped));
                handTrackInfo.ThumbFinger.LocalPositions[1] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.THUMB3, flipped));
                handTrackInfo.ThumbFinger.LocalPositions[2] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.THUMB_TIP, flipped));

                handTrackInfo.IndexFinger.LocalPositions[0] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.INDEX1, flipped));
                handTrackInfo.IndexFinger.LocalPositions[1] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.INDEX2, flipped));
                handTrackInfo.IndexFinger.LocalPositions[2] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.INDEX3, flipped));
                handTrackInfo.IndexFinger.LocalPositions[3] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.INDEX_TIP, flipped));


                handTrackInfo.MiddleFinger.LocalPositions[0] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.MIDDLE1, flipped));
                handTrackInfo.MiddleFinger.LocalPositions[1] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.MIDDLE2, flipped));
                handTrackInfo.MiddleFinger.LocalPositions[2] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.MIDDLE3, flipped));
                handTrackInfo.MiddleFinger.LocalPositions[3] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.MIDDLE_TIP, flipped));


                handTrackInfo.RingFinger.LocalPositions[0] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.RING1, flipped));
                handTrackInfo.RingFinger.LocalPositions[1] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.RING2, flipped));
                handTrackInfo.RingFinger.LocalPositions[2] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.RING3, flipped));
                handTrackInfo.RingFinger.LocalPositions[3] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.RING_TIP, flipped));


                handTrackInfo.LittleFinger.LocalPositions[0] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.PINKY1, flipped));
                handTrackInfo.LittleFinger.LocalPositions[1] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.PINKY2, flipped));
                handTrackInfo.LittleFinger.LocalPositions[2] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.PINKY3, flipped));
                handTrackInfo.LittleFinger.LocalPositions[3] = hand_raw_to_head.MultiplyPoint3x4(skeleton.GetPosition(NativePointID.PINKY_TIP, flipped));
            }

            //Update raw positions:
            {

                handTrackInfo.ThumbFinger.RawPositions[0] = skeleton.GetPosition(NativePointID.THUMB2, flipped);
                handTrackInfo.ThumbFinger.RawPositions[1] = skeleton.GetPosition(NativePointID.THUMB3, flipped);
                handTrackInfo.ThumbFinger.RawPositions[2] = skeleton.GetPosition(NativePointID.THUMB_TIP, flipped);

                handTrackInfo.IndexFinger.RawPositions[0] = skeleton.GetPosition(NativePointID.INDEX1, flipped);
                handTrackInfo.IndexFinger.RawPositions[1] = skeleton.GetPosition(NativePointID.INDEX2, flipped);
                handTrackInfo.IndexFinger.RawPositions[2] = skeleton.GetPosition(NativePointID.INDEX3, flipped);
                handTrackInfo.IndexFinger.RawPositions[3] = skeleton.GetPosition(NativePointID.INDEX_TIP, flipped);


                handTrackInfo.MiddleFinger.RawPositions[0] = skeleton.GetPosition(NativePointID.MIDDLE1, flipped);
                handTrackInfo.MiddleFinger.RawPositions[1] = skeleton.GetPosition(NativePointID.MIDDLE2, flipped);
                handTrackInfo.MiddleFinger.RawPositions[2] = skeleton.GetPosition(NativePointID.MIDDLE3, flipped);
                handTrackInfo.MiddleFinger.RawPositions[3] = skeleton.GetPosition(NativePointID.MIDDLE_TIP, flipped);


                handTrackInfo.RingFinger.RawPositions[0] = skeleton.GetPosition(NativePointID.RING1, flipped);
                handTrackInfo.RingFinger.RawPositions[1] = skeleton.GetPosition(NativePointID.RING2, flipped);
                handTrackInfo.RingFinger.RawPositions[2] = skeleton.GetPosition(NativePointID.RING3, flipped);
                handTrackInfo.RingFinger.RawPositions[3] = skeleton.GetPosition(NativePointID.RING_TIP, flipped);


                handTrackInfo.LittleFinger.RawPositions[0] = skeleton.GetPosition(NativePointID.PINKY1, flipped);
                handTrackInfo.LittleFinger.RawPositions[1] = skeleton.GetPosition(NativePointID.PINKY2, flipped);
                handTrackInfo.LittleFinger.RawPositions[2] = skeleton.GetPosition(NativePointID.PINKY3, flipped);
                handTrackInfo.LittleFinger.RawPositions[3] = skeleton.GetPosition(NativePointID.PINKY_TIP, flipped);
            }


            handTrackInfo.UpdateProperties();

            //Old code : define palm point by wrist/ring/middle 

            Vector3 wristPos = handTrackInfo.WristPosition;
            Vector3 wristToRing = handTrackInfo.RingFinger.Positions[0] - wristPos;
            Vector3 wristToMiddle = handTrackInfo.MiddleFinger.Positions[0] - wristPos;
            Vector3 ringToMiddle = handTrackInfo.MiddleFinger.Positions[0] - handTrackInfo.RingFinger.Positions[0];

            //    handTrackInfo.PalmPosition = wristPos + wristToRing * 0.45f + ringToMiddle * 0.43f;
            //    handTrackInfo.PalmScale = Vector3.one * wristToMiddle.magnitude * 0.86f;



            //New code : define palm point by wrist/thumb/index

            Vector3 wristToIndex = handTrackInfo.IndexFinger.Positions[0] - wristPos;
            Vector3 wristToThumb = handTrackInfo.ThumbFinger.Positions[0] - wristPos;
            Vector3 thumbToIndex = handTrackInfo.IndexFinger.Positions[0] - handTrackInfo.ThumbFinger.Positions[0];

            handTrackInfo.PalmPosition = wristPos + wristToIndex * 0.45f + thumbToIndex * 0.43f;
            handTrackInfo.PalmScale = Vector3.one * wristToIndex.magnitude * 0.86f;






            if (prevFrame.IsValid)
            {
                handTrackInfo.PalmDeltaPosition = handTrackInfo.PalmPosition - prevFrame.PalmPosition;
                handTrackInfo.PalmVelocity = handTrackInfo.PalmDeltaPosition / (float)new TimeSpan(handTrackInfo.Timestamp - prevFrame.Timestamp).TotalSeconds;
                handTrackInfo.PalmProjectionVelocity = mainCamera.InverseTransformVector(handTrackInfo.PalmVelocity); //Map world velocity to head 
            }

            //Old code : define crs by wristToRing | wristToMiddle
            Vector3 crs = Vector3.Cross(wristToRing, wristToMiddle);

            //New code : define crs by wristToThumb | wristToIndex
            //Vector3 crs = Vector3.Cross(wristToIndex, wristToThumb);


            //Make palm normal always facing UP upon the palm surface:
            if (index == 0)//left
            {
                crs = -crs;
            }
            // handTrackInfo.PalmRotation = Quaternion.LookRotation(wristToRing, crs);
            var _palmQ = Quaternion.LookRotation(wristToIndex, crs);
            Matrix4x4 palmTRS = Matrix4x4.TRS(handTrackInfo.PalmPosition, _palmQ, handTrackInfo.PalmScale);
            switch (index)
            {
                //left:
                case 0:
                    palmTRS = palmTRS * Matrix4x4.TRS(PalmTFixedOffsetL, PalmQFixedOffsetL, Vector3.one);
                    break;

                //right:
                case 1:
                default:
                    palmTRS = palmTRS * Matrix4x4.TRS(PalmTFixedOffsetR, PalmQFixedOffsetR, Vector3.one);
                    break;
            }
            handTrackInfo.PalmPosition = palmTRS.GetColumn(3);
            handTrackInfo.PalmRotation = palmTRS.rotation;
            handTrackInfo.PalmNormal = crs.normalized;

            handTrackInfo.PalmProjectionPosition = mainCamera.InverseTransformPoint(handTrackInfo.PalmPosition); //Map palm position to head 

            //Debug.LogFormat("{0} palm rotation in head space: {1} / {2}", handTrackInfo.Handness, (Quaternion.Inverse(Camera.main.transform.rotation) * handTrackInfo.PalmRotation).eulerAngles, ((Quaternion.Inverse(Camera.main.transform.rotation) * handTrackInfo.PalmRotation) * Vector3.forward).ToString("F5"));


            Vector3 viewSpacePoint = mainCamera.GetComponent<Camera>().WorldToViewportPoint(handTrackInfo.PalmPosition);
            handTrackInfo.PalmViewSpacePosition = (Vector3)viewSpacePoint;
            //Debug.LogFormat("{0} view pos2d : {1}", handTrackInfo.Handness, viewSpacePoint.ToString("F3"));

            if (mainCamera && mainCamera.parent)
            {
                handTrackInfo.PalmLocalPosition = mainCamera.parent.InverseTransformPoint(handTrackInfo.PalmPosition);
                handTrackInfo.PalmLocalRotation = Quaternion.Inverse(mainCamera.parent.rotation) * handTrackInfo.PalmRotation;
                handTrackInfo.PalmLocalNormal = mainCamera.parent.InverseTransformVector(handTrackInfo.PalmNormal);
                handTrackInfo.IsPalmFacingTowardsUser = Vector3.Dot((mainCamera.transform.localPosition - handTrackInfo.PalmLocalPosition).normalized, handTrackInfo.PalmLocalNormal) >= 0.35f;
            }

            if (prevFrame.IsValid)
            {
                handTrackInfo.PalmDeltaEuler = (Quaternion.Inverse(prevFrame.PalmRotation) * handTrackInfo.PalmRotation).eulerAngles;
            }

            //Debug.LogFormat("Get valid hand track frame at time: {0}, is valid: {3} palm point: {1}, thumb point: {2}",
            //    handTrackInfo.Timestamp, handTrackInfo.PalmPosition, handTrackInfo.ThumbFinger.Positions[0], handTrackInfo.IsValid);

            handTrackInfo.NativeGestureType = (int)gestureType;

            //Update gesture enum:
            if (handTrackInfo.IsValid == false)
            {
                handTrackInfo.gestureFistOpenHand = GestureType_Fist_OpenHand.None;
                handTrackInfo.gestureGrisp = GestureType_Grisp.None;
                handTrackInfo.NativeGestureType = -1;
            }
            else
            {
                //gesture type of open hand / grisp : 
                if (handTrackInfo.NativeGestureType == (int)TouchlessA3D.GestureType.HAND || handTrackInfo.NativeGestureType == (int)TouchlessA3D.GestureType.OPEN_HAND)
                {
                    handTrackInfo.gestureFistOpenHand = GestureType_Fist_OpenHand.Opened;
                    handTrackInfo.gestureGrisp = GestureType_Grisp.GrispOpen;
                }

                //Fist - close hand and grasp clsoed
                if (handTrackInfo.NativeGestureType == (int)TouchlessA3D.GestureType.CLOSED_HAND)
                {
                    handTrackInfo.gestureFistOpenHand = GestureType_Fist_OpenHand.Fist;
                    handTrackInfo.gestureGrisp = GestureType_Grisp.GraspClosed;
                }

                //Grisp pinch:
                if (handTrackInfo.NativeGestureType == (int)TouchlessA3D.GestureType.CLOSED_PINCH)
                {
                    handTrackInfo.gestureGrisp = GestureType_Grisp.GraspClosed;
                }
            }

            this.InsertCache(cacheList, handTrackInfo);

            //Update swing flag:
            {
                float now = Time.realtimeSinceStartup;
                swingCacheList.ClearCacheTimeout(now, 0.5f);
                //Search poses queue in 0.3s before :
                if (this.SearchCachePoses(0.333f, cacheList, out NativeArray<HandTrackingDataCache> outputCachePose, out float totalCacheTime))
                {
                    float swingAngle = 0;
                    for (int i = 0, iMax = outputCachePose.Length - 1; i < iMax; i++)
                    {
                        var cachePose0 = outputCachePose[i];
                        var cachePose1 = outputCachePose[i + 1];
                        //Vector3 dir0 = (cachePose0.indexRootL - cachePose0.wristPointL).normalized;
                        //Vector3 dir1 = (cachePose1.indexRootL - cachePose1.wristPointL).normalized;
                        var dir0 = (cachePose0.indexRoot2D - cachePose0.wristPoint2D).normalized;
                        var dir1 = (cachePose1.indexRoot2D - cachePose1.wristPoint2D).normalized;

                        swingAngle += Vector2.SignedAngle(dir0, dir1);
                        //PolyEngine.PEDraw.DrawSphere(mainCamera.transform.TransformPoint(cachePose0.indexRootL), 0.01f, Color.Lerp(Color.white / iMax, Color.white, (float)i / iMax), true, 0.5f);
                    }

                    const int kSwingAngleValve = 50; //挥手角度阈值
                    if (Mathf.Abs(swingAngle) >= kSwingAngleValve)
                    {
                        swingCacheList.AddCache(swingAngle, Time.realtimeSinceStartup);
                    }

                    if (swingCacheList.EnoughCache(3))
                    {
                        //Debug.LogFormat("user swing hand: {0}", Mathf.Sign(swingCacheList.swingHandSum) == -1 ? "Left" : "Right");
                        if (Mathf.Abs(swingCacheList.lastSwingInvokeTime - now) >= 0.35f)
                        {
                            swingCacheList.lastSwingInvokeTime = now;
                            handTrackInfo.SwingFlag = Mathf.Sign(swingCacheList.swingHandSum) == -1 ? 1 : 2;
                            //Debug.LogFormat("user swing hand: {0}", handTrackInfo.SwingFlag == 1 ? "Left" : "Right");
                            //PolyEngine.PEDraw.Text3D(mainCamera.transform.position + mainCamera.transform.forward * 0.5f, Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up), 0.05f, handTrackInfo.SwingFlag == 1 ? "Swing Left" : "Swing Right", Color.grey, 1.5f, XimmerseXRSettings.instance.debugFont);
                        }
                        swingCacheList.Clear();
                    }

                    //Debug draw swing angle above palm position:
                    //var indexRoot = mainCamera.transform.TransformPoint(handTrackInfo.IndexFinger.LocalPositions[0]);
                    //PolyEngine.PEDraw.Text3D(indexRoot + Vector3.up * 0.015f, Quaternion.LookRotation(indexRoot - mainCamera.transform.position, Vector3.up), 0.015f, swingAngle.ToString("F2"), Color.grey, 0, XimmerseXRSettings.instance.debugFont); 
                    //Debug.LogFormat("Get cache count : {0}/{1} for 0.3 sec, swing angle = {2}", outputCachePose.Length, totalCacheTime, swingAngle);
                    outputCachePose.Dispose();
                }
            }

            ////Apply smoothing:
            //if (true)
            //{
            //    SmoothHandTrackInfo(ref handTrackInfo, prevFrame);
            //}

            prevFrame.CopyFrom(handTrackInfo);
        }

        /// <summary>
        /// Callback on native event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTouchlessEvent(object sender, GestureEvent args)
        {
            //Debug.LogFormat("OnTouchlessEvent : {0}", args.skeletonValid);
            lock (lockObj)
            {
                locked_gestureEvent = args;
            }
        }


        /// <summary>
        /// Insert cache 
        /// </summary>
        /// <param name="caches"></param>
        /// <param name="handTrackingInfo"></param>
        private void InsertCache(List<HandTrackingDataCache> caches, HandTrackingInfo handTrackingInfo)
        {
            if (caches.Count == 0)
            {
                caches.Add(new HandTrackingDataCache(handTrackingInfo.WristLocalPosition, handTrackingInfo.IndexFinger.LocalPositions[0], handTrackingInfo.WristPosition2D, handTrackingInfo.IndexRootPosition2D, Time.realtimeSinceStartup));
            }
            else
            {
                //移除超出 kMaxCacheTime 的数据
                removeTimeout:
                if (caches.Count > 0 && Mathf.Abs(caches[0].timestamp - Time.realtimeSinceStartup) >= kMaxCacheTime)
                {
                    caches.RemoveAt(0);
                    goto removeTimeout;
                }

                //新的数据添加到末尾:
                caches.Add(new HandTrackingDataCache(handTrackingInfo.WristLocalPosition, handTrackingInfo.IndexFinger.LocalPositions[0], handTrackingInfo.WristPosition2D, handTrackingInfo.IndexRootPosition2D, Time.realtimeSinceStartup));
            }
        }

        /// <summary>
        /// 搜寻距离时长为 timeSpan 的历史姿态
        /// </summary>
        /// <param name="timespan"></param>
        /// <param name="cachePose"></param>
        /// <returns></returns>
        private bool SearchCachePose(float timespan, List<HandTrackingDataCache> caches, out HandTrackingDataCache cachePose)
        {
            for (int i = 0, iMax = caches.Count; i < iMax; i++)
            {
                if ((caches[i].timestamp - Time.realtimeSinceStartup) <= timespan)
                {
                    cachePose = caches[i];
                    return true;
                }
            }
            cachePose = default;
            return false;
        }

        /// <summary>
        /// 搜寻距离时长为 withInTimespan 内的历史姿态队列
        /// </summary>
        /// <param name="inTime"></param>
        /// <param name="caches"></param>
        /// <param name="outputCachePose"></param>
        /// <param name="totalSumTime"></param>
        /// <param name="allocator"></param>
        /// <returns></returns>
        private bool SearchCachePoses(float inTime, List<HandTrackingDataCache> caches, out NativeArray<HandTrackingDataCache> outputCachePose, out float totalSumTime, Allocator allocator = Allocator.Temp)
        {
            if (caches.Count <= 2)
            {
                totalSumTime = 0;
                outputCachePose = default;
                return false;
            }
            totalSumTime = 0;
            int fromIndex = -1;
            float now = Time.realtimeSinceStartup;
            for (int i = 0, iMax = caches.Count; i < iMax; i++)
            {
                if ((caches[i].timestamp - now) <= inTime)//找到时间累积为 inTime 的那一帧
                {
                    fromIndex = i;
                    break;
                }
            }
            if (fromIndex == -1)
            {
                outputCachePose = default;
                return false;
            }

            int _arrIdx = 0;
            outputCachePose = new NativeArray<HandTrackingDataCache>(caches.Count - fromIndex, allocator);
            for (int i = fromIndex; i < caches.Count; i++)
            {
                outputCachePose[_arrIdx++] = caches[i];
            }
            totalSumTime = Mathf.Abs(now - outputCachePose[0].timestamp);
            return true;
        }
    }
}