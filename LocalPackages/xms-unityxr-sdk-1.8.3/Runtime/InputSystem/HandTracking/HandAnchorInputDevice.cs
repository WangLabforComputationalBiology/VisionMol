using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
namespace Ximmerse.XR.InputSystems
{
    /// <summary>
    /// Hand anchor input device.
    /// </summary>
    [InputControlLayout(commonUsages = new[] { "LeftHandAnchor", "RightHandAnchor" }, isGenericTypeOfDevice = false, displayName = "HandAnchorInputDevice", stateType = typeof(HandAnchorInputState))]
    public class HandAnchorInputDevice : InputDevice, IInputUpdateCallbackReceiver
    {
        public static HandAnchorInputDevice left, right;

        public static HandAnchorInputDevice GetInput(HandnessType handness)
        {
            if (handness == HandnessType.Left)
            {
                return left;
            }
            else
            {
                return right;
            }
        }

        /// <summary>
        /// Input device config of this hand anchor input device.
        /// </summary>
        public AnchorHandInputDeviceConfig inputConfig
        {
            get; internal set;
        }

        /// <summary>
        /// 0 = left, 1 = right
        /// </summary>
        public int handness
        {
            get; internal set;
        }


        [InputControl(name = "HandAnchorInputState/isTracked")]
        public ButtonControl isTracked
        {
            get; internal set;
        }

        /// <summary>
        /// Tracking state, the value mapped to UnityEngine.XR.InputTrackingState:
        /// None = 0, Position = 1, Rotation = 2,Velocity = 4,AngularVelocity = 8,
        /// Acceleration = 16,AngularAcceleration = 32, All = 63
        /// </summary>
        [InputControl(name = "HandAnchorInputState/trackingState")]
        public IntegerControl trackingState
        {
            get; internal set;
        }

        [InputControl(name = "HandAnchorInputState/anchorPosition")]
        public Vector3Control anchorPosition
        {
            get; internal set;
        }

        /// <summary>
        /// 锚定点射线起点(即头部位置)
        /// </summary>
        [InputControl(name = "HandAnchorInputState/anchorRayOrigin")]
        public Vector3Control anchorRayOrigin
        {
            get; internal set;
        }

        /// <summary>
        /// 锚定点射线方向(头部射线)
        /// </summary>
        [InputControl(name = "HandAnchorInputState/anchorRayRotation")]
        public QuaternionControl anchorRayRotation
        {
            get; internal set;
        }

        /// <summary>
        /// 手部射线起点
        /// </summary>
        [InputControl(name = "HandAnchorInputState/handRayOrigin")]
        public Vector3Control handRayOrigin
        {
            get; internal set;
        }

        /// <summary>
        /// 手部射线方向
        /// </summary>
        [InputControl(name = "HandAnchorInputState/handRayRotation")]
        public QuaternionControl handRayRotation
        {
            get; internal set;
        }

        [InputControl(name = "HandAnchorInputState/fingerGraspPoint")]
        public Vector3Control fingerGraspPoint
        {
            get; internal set;
        }

        [InputControl(name = "HandAnchorInputState/isGrip")]
        public ButtonControl isGrip
        {
            get; internal set;
        }

        [InputControl(name = "HandAnchorInputState/gripValue")]
        public AxisControl gripValue
        {
            get; internal set;
        }

        Camera m_mainCam;

        Camera MainCam
        {
            get
            {
                if (!m_mainCam)
                {
                    m_mainCam = Camera.main;
                }
                return m_mainCam;
            }
        }

        Transform m_MainCameraT;

        /// <summary>
        /// Main camera transform.
        /// </summary>
        Transform mainCameraT
        {
            get
            {
                if (!m_MainCameraT)
                {
                    m_MainCameraT = Camera.main.transform;
                }
                return m_MainCameraT;
            }
        }

        /// <summary>
        /// Input state, data in XR space.
        /// </summary>
        public HandAnchorInputState InputState_XR_Space { get => m_inputState_XR_Space; }

        /// <summary>
        /// Input state, data in world space.
        /// </summary>
        public HandAnchorInputState InputState_World_Space { get => m_inputState_World_Space; }

        /// <summary>
        /// anchor point at runtime.
        /// </summary>
        [System.NonSerialized]
        public Vector3 anchorPointRuntime;

        /// <summary>
        /// finger grasp point at runtime.
        /// This point is head space.
        /// </summary>
        public Vector3 fingerGripPointRuntime, prevFingerGripPointRuntime;

        /// <summary>
        /// Hand anchor input state, data in XR rig space and world space.
        /// </summary>
        private HandAnchorInputState m_inputState_XR_Space = new HandAnchorInputState(), m_inputState_World_Space = new HandAnchorInputState();


        protected override void FinishSetup()
        {
            base.FinishSetup();
            isTracked = GetChildControl<ButtonControl>("isTracked");
            trackingState = GetChildControl<IntegerControl>("trackingState");
            anchorPosition = GetChildControl<Vector3Control>("anchorPosition");
            anchorRayOrigin = GetChildControl<Vector3Control>("anchorRayOrigin");
            anchorRayRotation = GetChildControl<QuaternionControl>("anchorRayRotation");
            handRayOrigin = GetChildControl<Vector3Control>("handRayOrigin");
            handRayRotation = GetChildControl<QuaternionControl>("handRayRotation");
            fingerGraspPoint = GetChildControl<Vector3Control>("fingerGraspPoint");
            isGrip = GetChildControl<ButtonControl>("isGrip");
            gripValue = GetChildControl<AxisControl>("gripValue");
        }


        public void OnUpdate()
        {
            if (HandTracking.IsHandTrackingEnable)
            {
                UpdateHandState(ref m_inputState_XR_Space, ref m_inputState_World_Space, handness == 0 ? HandTracking.LeftHandTrackInfo : HandTracking.RightHandTrackInfo);
            }
            else
            {
                m_inputState_XR_Space.isTracked = 0;
                m_inputState_XR_Space.trackingState = 0u;

                m_inputState_World_Space.isTracked = m_inputState_XR_Space.isTracked;
                m_inputState_World_Space.trackingState = m_inputState_XR_Space.trackingState;
                m_inputState_XR_Space.isGrip = false;
                m_inputState_XR_Space.gripValue = 0;

                m_inputState_XR_Space.isGrip = false;
                m_inputState_XR_Space.gripValue = 0;
            }

            InputSystem.QueueStateEvent(this, m_inputState_XR_Space);
        }


        /// <summary>
        /// Update both XR and world space hand state
        /// </summary>
        /// <param name="_state_XR_Space"></param>
        /// <param name="_state_World_Space"></param>
        /// <param name="handTrackInfo"></param>
        private void UpdateHandState(ref HandAnchorInputState _state_XR_Space, ref HandAnchorInputState _state_World_Space, HandTrackingInfo handTrackInfo)
        {
            _state_XR_Space.isTracked = handTrackInfo.IsValid ? 1 : 0;
            _state_XR_Space.trackingState = handTrackInfo.IsValid ? 3u : 0u;

            _state_World_Space.isTracked = _state_XR_Space.isTracked;
            _state_World_Space.trackingState = _state_XR_Space.trackingState;

            if (_state_XR_Space.isTracked != 0)
            {
                var indexTip = handTrackInfo.IndexFinger.LocalPositions[handTrackInfo.IndexFinger.LocalPositions.Length - 1];
                var thumbTip = handTrackInfo.ThumbFinger.LocalPositions[handTrackInfo.ThumbFinger.LocalPositions.Length - 1];

                float distanceThumbIndexTip = Vector3.Distance(thumbTip, indexTip);
                bool isPinch = false;
                float pinchLevel = 0;

                //world2XR matrix converts world space to XR space (parent of XR Camera transform)
                var world2XR = mainCameraT.parent.worldToLocalMatrix;
                var head2World = mainCameraT.localToWorldMatrix;

                //计算 finger grip point:
                if (distanceThumbIndexTip <= inputConfig.graspDistanceSpan)
                {
                    isPinch = true;
                    pinchLevel = Mathf.Clamp01(1 - distanceThumbIndexTip / inputConfig.graspDistanceSpan);

                }
                Vector3 tipsMiddle = 0.75f * thumbTip + 0.25f * indexTip + (indexTip - thumbTip).normalized * 0.02f;
                prevFingerGripPointRuntime = fingerGripPointRuntime;
                inputConfig.graspPointDampMovementConfig.UpdatePoint(ref fingerGripPointRuntime, tipsMiddle);
                _state_World_Space.fingerGraspPoint = head2World.MultiplyPoint3x4(fingerGripPointRuntime);
                _state_XR_Space.fingerGraspPoint = world2XR.MultiplyPoint3x4(_state_World_Space.fingerGraspPoint);
                // PolyEngine.PEDraw.DrawSphere(mainCameraT.TransformPoint(fingerGripPointRuntime), 0.01f, Color.white, false);


                var wrist = mainCameraT.TransformPoint(handTrackInfo.WristLocalPosition);
                var point1 = mainCameraT.TransformPoint(handTrackInfo.IndexFinger.LocalPositions[0]);
                //PolyEngine.PEDraw.DrawLine(wrist, point1, Color.white, 0.0025f);

                Vector3 point2 = mainCameraT.TransformPoint(handTrackInfo.ThumbFinger.LocalPositions[0]);

                var fwd = ((point1 - wrist).normalized + (point2 - wrist).normalized).normalized;
                Quaternion pivotQ = Quaternion.LookRotation(fwd) * Quaternion.Euler(inputConfig.localEulerOffset);
                Matrix4x4 trs_wrist_in_world_space = Matrix4x4.TRS(wrist, pivotQ, Vector3.one);
                // PolyEngine.PEDraw.DrawTranslateGizmos(trs.GetColumn(3), trs.rotation, 0.1f, false, 0);
                Vector3 anchor = trs_wrist_in_world_space.MultiplyPoint3x4(inputConfig.localAnchorOffset);
                //   PEDraw.DrawWireSphere(anchor, 0.025f, Color.white, true);//白色：投影源点


                switch (inputConfig.anchorPointMode)
                {
                    //targetAnchorPoint is a point on fixed distance ahead of user's head
                    case AnchorHandInputDeviceConfig.AnchorPointMode.ProjectOnFixedPlane:
                        //将 anchor 投影到 projection plane 上:
                        Plane projectionPlane = new Plane(mainCameraT.forward * -1, mainCameraT.position + mainCameraT.forward * inputConfig.projectionPlaneDistance);
                        Ray ray = new Ray(anchor, mainCameraT.forward);
                        projectionPlane.Raycast(ray, out float d);
                        Vector3 prjAnchor = ray.GetPoint(d);
                        // PEDraw.DrawWireSphere(prjAnchor, 0.025f, Color.green, true);//绿色:未过滤Z轴之前的投影终点 
                        var iPrjAnchor = mainCameraT.InverseTransformPoint(prjAnchor);
                        iPrjAnchor.z = inputConfig.projectionPlaneDistance;//移除 z 轴的抖动因子，保留XY

                        //Smooth targetAnchorPoint (in world space)
                        Vector3 targetAnchorPoint = mainCameraT.TransformPoint(iPrjAnchor);
                        inputConfig.anchorPointerDampMovementConfig.UpdatePoint(ref anchorPointRuntime, targetAnchorPoint);
                        break;

                    //targetAnchorPoint is a 3d space point , rigid to middle - forward of wrist-thumb and wrist-index
                    case AnchorHandInputDeviceConfig.AnchorPointMode.ThreeDimensionSpace:
                        targetAnchorPoint = anchor;
                        inputConfig.anchorPointerDampMovementConfig.UpdatePoint(ref anchorPointRuntime, targetAnchorPoint);
                        break;

                    case AnchorHandInputDeviceConfig.AnchorPointMode.AlignToGraspPoint:
                        anchorPointRuntime = mainCameraT.TransformPoint(fingerGripPointRuntime);
                        break;
                }

                //PolyEngine.PEDraw.DrawWireSphere(anchorPointRuntime, 0.025f, Color.red, true);//红色：已经平滑过的目标投影点



                _state_XR_Space.anchorPosition = world2XR.MultiplyPoint3x4(anchorPointRuntime);
                //Vector3 ipdDir = handness == 0 ? -mainCameraT.right : mainCameraT.right;
                Vector3 ipdDir = mainCameraT.right; //右眼做为 raycast 的主眼
                _state_XR_Space.anchorRayOrigin = world2XR.MultiplyPoint3x4(mainCameraT.position + ipdDir * 0.032f); //ray origin = eye center
                _state_XR_Space.anchorRayRotation = Quaternion.LookRotation(_state_XR_Space.anchorPosition - _state_XR_Space.anchorRayOrigin);
                _state_XR_Space.isGrip = isPinch;
                _state_XR_Space.gripValue = pinchLevel;

                //设置Hand ray origin/direction at XR space:
                _state_XR_Space.handRayOrigin = _state_XR_Space.anchorPosition;
                Vector3 handRayOriginOffset = Matrix4x4.TRS(mainCameraT.localPosition, mainCameraT.localRotation, Vector3.one).MultiplyPoint3x4(this.inputConfig.localHandRayOriginOffset);
                _state_XR_Space.handRayRotation = Quaternion.LookRotation(_state_XR_Space.handRayOrigin - handRayOriginOffset);

                var xr2world = world2XR.inverse;
                _state_World_Space.anchorPosition = xr2world.MultiplyPoint(_state_XR_Space.anchorPosition);
                _state_World_Space.anchorRayOrigin = mainCameraT.position + ipdDir * 0.032f; //ray origin = eye center
                _state_World_Space.anchorRayRotation = Quaternion.LookRotation(_state_World_Space.anchorPosition - _state_World_Space.anchorRayOrigin);
                _state_World_Space.isGrip = isPinch;
                _state_World_Space.gripValue = pinchLevel;

                //设置Hand ray origin/direction at world space:
                _state_World_Space.handRayOrigin = _state_World_Space.anchorPosition;
                _state_World_Space.handRayRotation = xr2world.rotation * _state_XR_Space.handRayRotation;

                //Update hand ray/hand rotation: 
                _state_XR_Space.handRayOrigin = _state_XR_Space.anchorPosition;

                //disable hand ray if palm is facing back:
                if (inputConfig.disableHandRayWhenPalmFaceUser && handTrackInfo.IsPalmFacingTowardsUser)
                {
                    _state_XR_Space.trackingState = 0;
                    _state_World_Space.trackingState = 0;
                }

                //PolyEngine.PEDraw.DrawRay(wrist, fwd, Color.blue, false);
            }
            else
            {
                _state_XR_Space.isGrip = false;
                _state_XR_Space.gripValue = 0;

                _state_World_Space.isGrip = false;
                _state_World_Space.gripValue = 0;
            }
        }
    }
}