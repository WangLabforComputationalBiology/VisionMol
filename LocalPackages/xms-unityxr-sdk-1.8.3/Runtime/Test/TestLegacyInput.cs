using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Ximmerse.XR.Utils;
using System.Text;


namespace Ximmerse.XR.tests
{
    /// <summary>
    /// Test XR controller input interface using legacy UnityEngine.Input class.
    /// </summary>
    public class TestLegacyInput : MonoBehaviour
    {

        static readonly string[] xrButtons_Left = new string[]
        {
            "XRI_Left_PrimaryButton", "XRI_Left_SecondaryButton", "XRI_Left_Primary2DAxisClick",
            "XRI_Left_Primary2DAxisTouch","XRI_Left_GripButton", "XRI_Left_TriggerButton", "XRI_Left_MenuButton",
            "XRI_Left_Thumbrest",
        };

        static readonly string[] xrButtons_Right = new string[]
        {
            "XRI_Right_PrimaryButton", "XRI_Right_SecondaryButton", "XRI_Right_Primary2DAxisClick",
            "XRI_Right_Primary2DAxisTouch","XRI_Right_GripButton", "XRI_Right_TriggerButton", "XRI_Right_MenuButton",
            "XRI_Right_Thumbrest",
        };


        static readonly string[] xrAxes_Left = new string[]
        {
            "XRI_Left_Primary2DAxis_Vertical", "XRI_Left_Primary2DAxis_Horizontal", "XRI_Left_Trigger","XRI_Left_IndexTouch","XRI_Left_Grip"
        };


        static readonly string[] xrAxes_Right = new string[]
        {
            "XRI_Right_Primary2DAxis_Vertical", "XRI_Right_Primary2DAxis_Horizontal", "XRI_Right_Trigger","XRI_Right_IndexTouch","XRI_Right_Grip"
        };

        StringBuilder leftHandMsg = new StringBuilder();

        StringBuilder rightHandMsg = new StringBuilder();

        void LateUpdate()
        {
            leftHandMsg.Clear();
            rightHandMsg.Clear();

            #region Left Hand
            foreach (var btn in xrButtons_Left)
            {
                const string msg = "UnityXRInput - Input.CheckButton: {0} , on : {1}, down : {2}, up : {3}";
                bool print = false;
                bool on = false, down = false, up = false;
                if (Input.GetButton(btn))
                {
                    leftHandMsg.AppendFormat(" {0} ", btn);
                    print = true;
                    on = true;
                }
                if (Input.GetButtonDown(btn))
                {
                    print = true;
                    down = true;
                }
                if (Input.GetButtonUp(btn))
                {
                    print = true;
                    up = true;
                }
                if (print)
                {
                    Debug.LogFormat(msg, btn, on, down, up);
                }
            }

            foreach (var axis in xrAxes_Left)
            {
                const string msg = "UnityXRInput - Input.CheckAxes: {0} , value: {1}";
                bool print = false;
                float val = 0;
                if (Input.GetAxis(axis) != 0)
                {
                    val = Input.GetAxis(axis);
                    print = true;
                    leftHandMsg.AppendFormat("\r\n {0}={1} ", axis, val.ToString("F1"));
                }
                if (print)
                {
                    Debug.LogFormat(msg, axis, val);
                }
            }
            #endregion

            #region Right Hand
            foreach (var btn in xrButtons_Right)
            {
                const string msg = "UnityXRInput - Input.CheckButton: {0} , on : {1}, down : {2}, up : {3}";
                bool print = false;
                bool on = false, down = false, up = false;
                if (Input.GetButton(btn))
                {
                    rightHandMsg.AppendFormat(" {0} ", btn);
                    print = true;
                    on = true;
                }
                if (Input.GetButtonDown(btn))
                {
                    print = true;
                    down = true;
                }
                if (Input.GetButtonUp(btn))
                {
                    print = true;
                    up = true;
                }
                if (print)
                {
                    Debug.LogFormat(msg, btn, on, down, up);
                }
            }

            foreach (var axis in xrAxes_Right)
            {
                const string msg = "UnityXRInput - Input.CheckAxes: {0} , value: {1}";
                bool print = false;
                float val = 0;
                if (Input.GetAxis(axis) != 0)
                {
                    val = Input.GetAxis(axis);
                    print = true;
                    rightHandMsg.AppendFormat("\r\n {0}={1} ", axis, val.ToString("F1"));
                }
                if (print)
                {
                    Debug.LogFormat(msg, axis, val);
                }
            }
            #endregion

            if (leftHandMsg.Length > 0)
            {
                PoseDataSource.GetDataFromSource(TrackedPoseDriver.TrackedPose.LeftPose, out Pose pose);
                Matrix4x4 world = Camera.main.transform.parent.localToWorldMatrix * Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one);
                RxDraw.Text3D(world.GetColumn(3), Quaternion.LookRotation(Camera.main.transform.forward), 0.01f, leftHandMsg.ToString(), Color.green);
            }

            if (rightHandMsg.Length > 0)
            {
                PoseDataSource.GetDataFromSource(TrackedPoseDriver.TrackedPose.RightPose, out Pose pose);
                Matrix4x4 world = Camera.main.transform.parent.localToWorldMatrix * Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one);
                RxDraw.Text3D(world.GetColumn(3), Quaternion.LookRotation(Camera.main.transform.forward), 0.01f, rightHandMsg.ToString(), Color.green);
            }
        }
    }
}