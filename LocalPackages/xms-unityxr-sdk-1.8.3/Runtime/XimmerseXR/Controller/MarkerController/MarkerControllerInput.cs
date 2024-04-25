using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using Ximmerse.Wrapper.XDeviceService.Client;
using Ximmerse.XR.Tag;
using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.XR.InputSystems
{


    public struct MarkerController : IInputStateTypeInfo
    {
        public FourCC format => new FourCC();

        /// <summary>
        /// left
        /// </summary>
        /// 
        [InputControl(name = "LeftAPP", layout = "Button",offset =0)]
        [InputControl(name = "LeftTrigger", layout = "Button", offset = 1)]
        [InputControl(name = "LeftHome", layout = "Button", offset = 2)]
        [InputControl(name = "LeftTouth", layout = "Button", offset = 3)]
        [InputControl(name = "RightAPP", layout = "Button", offset = 4)]
        [InputControl(name = "RightTrigger", layout = "Button", offset = 5)]
        [InputControl(name = "RightHome", layout = "Button", offset = 6)]
        [InputControl(name = "RightTouth", layout = "Button", offset = 7)]
        [InputControl(name = "LeftTriggerValue", layout = "Axis", offset = 8)]
        [InputControl(name = "RightTriggerValue", layout = "Axis", offset = 12)]
        public bool LeftAPP;
        public bool LeftTrigger;
        public bool LeftHome;
        public bool LeftTouth;
        public bool RightAPP;
        public bool RightTrigger;
        public bool RightHome;
        public bool RightTouth;
        public float LeftTriggerValue;
        public float RightTriggerValue;
        [InputControl(name = "LeftMarkerControllerPosition", layout = "Vector3")]
        public Vector3 LeftMarkerControllerPosition;
        [InputControl(name = "LeftMarkerControllerRotation", layout = "Quaternion")]
        public Quaternion LeftMarkerControllerRotation;
        [InputControl(name = "LeftMarkerControllerTrackingState", layout = "Integer")]
        public int LeftMarkerControllerTrackingState;
        [InputControl(name = "LeftMarkerControllerImuRotation", layout = "Quaternion")]
        public Quaternion LeftMarkerControllerImuRotation;
        [InputControl(name = "LeftMarkerControllerConnectState", layout = "Integer")]
        public int LeftMarkerControllerConnectState;

        [InputControl(name = "RightMarkerControllerPosition", layout = "Vector3")]
        public Vector3 RightMarkerControllerPosition;
        [InputControl(name = "RightMarkerControllerRotation", layout = "Quaternion")]
        public Quaternion RightMarkerControllerRotation;
        [InputControl(name = "RightMarkerControllerTrackingState", layout = "Integer")]
        public int RightMarkerControllerTrackingState;
        [InputControl(name = "RightMarkerControllerImuRotation", layout = "Quaternion")]
        public Quaternion RightMarkerControllerImuRotation;
        [InputControl(name = "RightMarkerControllerConnectState", layout = "Integer")]
        public int RightMarkerControllerConnectState;

    }
    [InputControlLayout(stateType = typeof(MarkerController))]
    public class MarkerControllerInput : InputDevice, IInputUpdateCallbackReceiver
    {
        /// <summary>
        /// left
        /// </summary>
        public ButtonControl LeftAPP { get; private set; }
        public ButtonControl LeftTrigger { get; private set; }
        public ButtonControl LeftHome { get; private set; }
        public ButtonControl LeftTouth { get; private set; }
        public ButtonControl RightAPP { get; private set; }
        public ButtonControl RightTrigger { get; private set; }
        public ButtonControl RightHome { get; private set; }
        public ButtonControl RightTouth { get; private set; }

        public AxisControl LeftTriggerValue { get; private set; }
        public AxisControl RightTriggerValue { get; private set; }

        //public ButtonControl thirdButton { get; private set; }

        private int leftMarkerController;
        private int rightMarkerController;


        public Vector3Control leftPosition { get; private set; }
        public QuaternionControl leftRotation { get; private set; }
        public IntegerControl leftTrackingState { get; private set; }
        public QuaternionControl leftImuRotation { get; private set; }
        public IntegerControl leftConnectState { get; private set; }

        public Vector3Control rightPosition { get; private set; }
        public QuaternionControl rightRotation { get; private set; }
        public IntegerControl rightTrackingState { get; private set; }
        public QuaternionControl rightImuRotation { get; private set; }
        public IntegerControl rightConnectState { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            LeftAPP = (ButtonControl)TryGetChildControl("LeftAPP");
            LeftTrigger = (ButtonControl)TryGetChildControl("LeftTrigger");
            LeftHome = (ButtonControl)TryGetChildControl("LeftHome");
            LeftTouth = (ButtonControl)TryGetChildControl("LeftTouth");
            RightAPP = (ButtonControl)TryGetChildControl("RightAPP");
            RightTrigger = (ButtonControl)TryGetChildControl("RightTrigger");
            RightHome = (ButtonControl)TryGetChildControl("RightHome");
            RightTouth = (ButtonControl)TryGetChildControl("RightTouth");
            LeftTriggerValue =(AxisControl)TryGetChildControl("LeftTriggerValue");
            RightTriggerValue = (AxisControl)TryGetChildControl("RightTriggerValue");


            leftPosition = GetChildControl<Vector3Control>("LeftMarkerControllerPosition");
            leftRotation = GetChildControl<QuaternionControl>("LeftMarkerControllerRotation");
            leftTrackingState = GetChildControl<IntegerControl>("LeftMarkerControllerTrackingState");
            leftImuRotation = GetChildControl<QuaternionControl>("LeftMarkerControllerImuRotation");
            leftConnectState = GetChildControl<IntegerControl>("LeftMarkerControllerConnectState");

            rightPosition = GetChildControl<Vector3Control>("RightMarkerControllerPosition");
            rightRotation = GetChildControl<QuaternionControl>("RightMarkerControllerRotation");
            rightTrackingState = GetChildControl<IntegerControl>("RightMarkerControllerTrackingState");
            rightImuRotation = GetChildControl<QuaternionControl>("RightMarkerControllerImuRotation");
            rightConnectState = GetChildControl<IntegerControl>("RightMarkerControllerConnectState");
        }

        Quaternion trackingOffset = Quaternion.Euler(0f, 180f, 0f);
        MarkerController markerController = new MarkerController();
        public void OnUpdate()
        {
            GetAllInfo();

            #region Button
            markerController.LeftAPP = isLeftAppButtonDown;
            markerController.LeftTrigger = isLeftTriggerButtonDown || (leftTriggerValue != 0);
            markerController.LeftHome = isLeftHomeButtonDown;
            markerController.LeftTouth = isLeftTouchButtonDown;
            markerController.RightAPP = isRightAppButtonDown;
            markerController.RightTrigger = isRightTriggerButtonDown || (rightTriggerValue != 0);
            markerController.RightHome = isRightHomeButtonDown;
            markerController.RightTouth = isRightTouchButtonDown;
            markerController.LeftTriggerValue = leftTriggerValue;
            markerController.RightTriggerValue = rightTriggerValue;

            InputSystem.QueueDeltaStateEvent(LeftAPP, markerController.LeftAPP);
            InputSystem.QueueDeltaStateEvent(LeftTrigger, markerController.LeftTrigger);
            InputSystem.QueueDeltaStateEvent(LeftHome, markerController.LeftHome);
            InputSystem.QueueDeltaStateEvent(LeftTouth, markerController.LeftTouth);
            InputSystem.QueueDeltaStateEvent(RightAPP, markerController.RightAPP);
            InputSystem.QueueDeltaStateEvent(RightTrigger, markerController.RightTrigger);
            InputSystem.QueueDeltaStateEvent(RightHome, markerController.RightHome);
            InputSystem.QueueDeltaStateEvent(RightTouth, markerController.RightTouth);
            InputSystem.QueueDeltaStateEvent(LeftTriggerValue, markerController.LeftTriggerValue);
            InputSystem.QueueDeltaStateEvent(RightTriggerValue, markerController.RightTriggerValue);
            #endregion


            #region Tracking
            markerController.LeftMarkerControllerPosition = leftPos;
            markerController.LeftMarkerControllerRotation = leftRot * trackingOffset;
            markerController.LeftMarkerControllerTrackingState = leftState;
            markerController.RightMarkerControllerPosition = rightPos;
            markerController.RightMarkerControllerRotation = rightRot * trackingOffset;
            markerController.RightMarkerControllerTrackingState = rightState;

            InputSystem.QueueDeltaStateEvent(leftPosition, markerController.LeftMarkerControllerPosition);
            InputSystem.QueueDeltaStateEvent(leftRotation, markerController.LeftMarkerControllerRotation);
            InputSystem.QueueDeltaStateEvent(leftTrackingState, markerController.LeftMarkerControllerTrackingState);
            InputSystem.QueueDeltaStateEvent(rightPosition, markerController.RightMarkerControllerPosition);
            InputSystem.QueueDeltaStateEvent(rightRotation, markerController.RightMarkerControllerRotation);
            InputSystem.QueueDeltaStateEvent(rightTrackingState, markerController.RightMarkerControllerTrackingState);
            #endregion

            #region Imu

            markerController.LeftMarkerControllerImuRotation = leftImuRot * currentLeftLerp;

            markerController.LeftMarkerControllerConnectState = leftConnect;

            markerController.RightMarkerControllerImuRotation = rightImuRot * currentRightLerp;

            markerController.RightMarkerControllerConnectState = rightConnect;

            InputSystem.QueueDeltaStateEvent(leftImuRotation, markerController.LeftMarkerControllerImuRotation);
            InputSystem.QueueDeltaStateEvent(leftConnectState, markerController.LeftMarkerControllerConnectState);
            InputSystem.QueueDeltaStateEvent(rightImuRotation, markerController.RightMarkerControllerImuRotation);
            InputSystem.QueueDeltaStateEvent(rightConnectState, markerController.RightMarkerControllerConnectState);

            #endregion



            //InputSystem.QueueStateEvent(this, markerController);
        }

        private bool isLeftTriggerButtonDown;

        private bool isLeftTouchButtonDown;

        private bool isLeftAppButtonDown;

        private bool isLeftHomeButtonDown;

        private int leftTriggerValue;



        private bool isRightTriggerButtonDown;

        private bool isRightTouchButtonDown;

        private bool isRightAppButtonDown;

        private bool isRightHomeButtonDown;

        private int rightTriggerValue;


        XAttrControllerState leftControllerState;
        Vector3 leftEuler = new Vector3();


        XAttrControllerState rightControllerState;
        Vector3 rightEuler = new Vector3();

        private bool trackingstate;
        int index = 0;
        long timestamp = 0;
        int state = 0;
        float posX = 0;
        float posY = 0;
        float posZ = 0;
        float rotX = 0;
        float rotY = 0;
        float rotZ = 0;
        float rotW = 0;
        float confidence = 0;
        float marker_distance = 0;

        Vector3 leftPos = new Vector3();
        Quaternion leftRot= Quaternion.identity;
        int leftState = 0;
        Quaternion leftImuRot = Quaternion.identity;
        int leftConnect = 0;
        public Quaternion LeftImuRot
        {
            get => leftImuRot;
        }

        Vector3 rightPos = new Vector3();
        Quaternion rightRot = Quaternion.identity;
        int rightState = 0;
        Quaternion rightImuRot = Quaternion.identity;
        int rightConnect = 0;
        public Quaternion RightImuRot
        {
            get => rightImuRot;
        }
        //public Quaternion leftControllerReset = Quaternion.identity;
        //public Quaternion rightControllerReset = Quaternion.identity;
        public Quaternion currentLeftLerp = Quaternion.identity;
        public Quaternion currentRightLerp = Quaternion.identity;
        private const int TrackingStateValue = 3;

        bool isLeftFirstGetImu;
        bool isRightFirstGetImu;

        //public XROrigin xr;
        int leftButtonBitmask;
        int rightButtonBitmask;
        public void GetAllInfo()
        {
            if (leftMarkerController>= 0 && (XDeviceClientWrapper.ClientControllerDic[leftMarkerController].GetConnectState() == XConnectionStates.kXConnSt_Connected))
            {
                #region Button

                leftButtonBitmask = XDevicePlugin.xdevc_ctrl_get_button_state_bitmask(XDevicePlugin.xdevc_get_controller(leftMarkerController));

                if ((leftButtonBitmask & (1 << 5)) == (1 << 5))
                {
                    isLeftTriggerButtonDown = true;
                }
                else
                {
                    isLeftTriggerButtonDown = false;
                }

                if ((leftButtonBitmask & (1 << 2)) == (1 << 2))
                {
                    isLeftTouchButtonDown = true;
                }
                else
                {
                    isLeftTouchButtonDown = false;
                }

                if ((leftButtonBitmask & (1 << 4)) == (1 << 4))
                {
                    isLeftAppButtonDown = true;
                }
                else
                {
                    isLeftAppButtonDown = false;
                }

                if ((leftButtonBitmask & (1 << 3)) == (1 << 3))
                {
                    isLeftHomeButtonDown = true;
                }
                else
                {
                    isLeftHomeButtonDown = false;
                }

                leftTriggerValue = XDevicePlugin.xdevc_ctrl_get_trigger(XDevicePlugin.xdevc_get_controller(leftMarkerController));
                #endregion

                //XDeviceClientWrapper.ClientControllerDic[leftMarkerController].GetControllerState();
                leftControllerState = XDeviceClientWrapper.ClientControllerDic[leftMarkerController].GetControllerState();
                //leftImuRot.x = leftControllerState.rotation[0];
                //leftImuRot.y = leftControllerState.rotation[1];
                //leftImuRot.z = leftControllerState.rotation[2];
                //leftImuRot.w = leftControllerState.rotation[3];

                leftEuler.x = -leftControllerState.euler[0];
                leftEuler.y = -leftControllerState.euler[1];
                leftEuler.z = leftControllerState.euler[2];

                leftImuRot = Quaternion.Euler(leftEuler);

                leftConnect = TrackingStateValue;
            }
            else
            {
                leftConnect = 0;
                isLeftTriggerButtonDown = false;
                isLeftTouchButtonDown = false;
                isLeftAppButtonDown = false;
                isLeftHomeButtonDown = false;
                leftTriggerValue = 0;
            }

            //right Button

            if ((rightMarkerController>=0) && (XDeviceClientWrapper.ClientControllerDic[rightMarkerController].GetConnectState() == XConnectionStates.kXConnSt_Connected))
            {

                #region Button

                rightButtonBitmask = XDevicePlugin.xdevc_ctrl_get_button_state_bitmask(XDevicePlugin.xdevc_get_controller(rightMarkerController));

                if ((rightButtonBitmask & (1 << 5)) == (1 << 5))
                {
                    isRightTriggerButtonDown = true;
                }
                else
                {
                    isRightTriggerButtonDown = false;
                }

                if ((rightButtonBitmask & (1 << 2)) == (1 << 2))
                {
                    isRightTouchButtonDown = true;
                }
                else
                {
                    isRightTouchButtonDown = false;
                }

                if ((rightButtonBitmask & (1 << 4)) == (1 << 4))
                {
                    isRightAppButtonDown = true;
                }
                else
                {
                    isRightAppButtonDown = false;
                }

                if ((rightButtonBitmask & (1 << 3)) == (1 << 3))
                {
                    isRightHomeButtonDown = true;
                }
                else
                {
                    isRightHomeButtonDown = false;
                }

                rightTriggerValue = XDevicePlugin.xdevc_ctrl_get_trigger(XDevicePlugin.xdevc_get_controller(rightMarkerController));
                #endregion

                rightControllerState = XDeviceClientWrapper.ClientControllerDic[rightMarkerController].GetControllerState();
                //rightImuRot.x = rightControllerState.rotation[0];
                //rightImuRot.y = rightControllerState.rotation[1];
                //rightImuRot.z = rightControllerState.rotation[2];
                //rightImuRot.w = rightControllerState.rotation[3];

                rightEuler.x = -rightControllerState.euler[0];
                rightEuler.y = -rightControllerState.euler[1];
                rightEuler.z = rightControllerState.euler[2];

                rightImuRot = Quaternion.Euler(rightEuler);
                rightConnect = TrackingStateValue;
            }
            else
            {
                rightConnect = 0;
                isRightTriggerButtonDown = false;
                isRightTouchButtonDown = false;
                isRightAppButtonDown = false;
                isRightHomeButtonDown = false;
                rightTriggerValue = 0;
            }

            //#endif
            TagTracking();
        }

        private void TagTracking()
        {
            if (TagProfileLoading.Instance != null && XMR_Manager.Instance != null && XMR_Manager.Instance.TagRecognition
                &&(TagProfileLoading.Instance.TrackingTagList.Contains(81)|| TagProfileLoading.Instance.TrackingTagList.Contains(82)))
            {

                for (int i = 0; i < TagProfileLoading.Instance.TrackingTagList.Count; i++)
                {
                    bool ret2 = NativePluginApi.Unity_GetTagPredict(i,
                        ref index, ref timestamp, ref state,
                        ref posX, ref posY, ref posZ,
                        ref rotX, ref rotY, ref rotZ, ref rotW,
                        ref confidence, ref marker_distance);

                    if ((index == 81) && (state != 0))
                    {
                        rightState = TrackingStateValue;

                        rightPos.x = posX;
                        rightPos.y = posY;
                        rightPos.z = posZ;

                        rightRot.x = rotX;
                        rightRot.y = rotY;
                        rightRot.z = rotZ;
                        rightRot.w = rotW;

                        //if (xr!=null)
                        //{
                        //    rightPos = xr.CameraFloorOffsetObject.transform.TransformPoint(new Vector3(posX, posY, posZ));
                        //    rightRot = xr.CameraFloorOffsetObject.transform.rotation * new Quaternion(rotX, rotY, rotZ, rotW);
                        //}
                        //else
                        //{
                        //    rightPos = Camera.main.transform.parent.transform.TransformPoint(new Vector3(posX, posY, posZ));
                        //    rightRot = Camera.main.transform.parent.transform.rotation * new Quaternion(rotX, rotY, rotZ, rotW);
                        //}
                    }
                    else
                    {
                        rightState = 0;
                    }

                    if ((index == 82) && (state != 0))
                    {
                        leftState = TrackingStateValue;

                        leftPos.x = posX;
                        leftPos.y = posY;
                        leftPos.z = posZ;

                        leftRot.x = rotX;
                        leftRot.y = rotY;
                        leftRot.z = rotZ;
                        leftRot.w = rotW;

                        //if (xr != null)
                        //{
                        //    leftPos = xr.CameraFloorOffsetObject.transform.TransformPoint(new Vector3(posX, posY, posZ));
                        //    leftRot = xr.CameraFloorOffsetObject.transform.rotation * new Quaternion(rotX, rotY, rotZ, rotW);
                        //}
                        //else
                        //{
                        //    leftPos = Camera.main.transform.TransformPoint(new Vector3(posX, posY, posZ));
                        //    leftRot = Camera.main.transform.rotation * new Quaternion(rotX, rotY, rotZ, rotW);
                        //}
                    }
                    else
                    {
                        leftState = 0;
                    }
                }
            }
            else
            {
                rightState = 0;
                leftState = 0;
            }
        }

        public void RefreshMarkerControllerIndex()
        {
            leftMarkerController = rightMarkerController = -1;

            for (int i = 0; i < 6; i++)
            {
#if !UNITY_EDITOR
                if ((XControllerTypes)XDevicePlugin.xdevc_ctrl_get_device_type(XDevicePlugin.xdevc_get_controller(i)) == XControllerTypes.kControllerType_TagLeft)
                {
                    leftMarkerController = i;
                    Debug.Log("QmQ  leftMarkerController Index:" + i);
                }

                if ((XControllerTypes)XDevicePlugin.xdevc_ctrl_get_device_type(XDevicePlugin.xdevc_get_controller(i)) == XControllerTypes.kControllerType_TagRight)
                {
                    rightMarkerController = i;
                    Debug.Log("QmQ  rightMarkerController Index:" + i);
                }
#endif
            }
        }

    }

}

