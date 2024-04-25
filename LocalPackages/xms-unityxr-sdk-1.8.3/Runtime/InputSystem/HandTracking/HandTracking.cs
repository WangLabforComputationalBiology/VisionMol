using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.InputSystem.Layouts;
using System.IO;
using Ximmerse.XR.Internal.Mathmatics;
using System.Xml;
using SXR;
namespace Ximmerse.XR.InputSystems
{


    /// <summary>
    /// Gesture type: open hand / fist
    /// </summary>
    public enum GestureType_Fist_OpenHand
    {
        None = 0,

        /// <summary>
        /// Five fingers opened.
        /// </summary>
        Opened = 1,

        /// <summary>
        /// Gesture of making hand as a fist
        /// </summary>
        Fist = 2,

    }

    /// <summary>
    /// Gesture type : grisp opened / closed
    /// </summary>
    public enum GestureType_Grisp
    {
        None = 0,

        /// <summary>
        /// Grisp open 
        /// </summary>
        GrispOpen,

        /// <summary>
        /// Grisp close
        /// </summary>
        GraspClosed,
    }


    /// <summary>
    /// Hand tracking API expose interface for hand tracking functionality with RhinoX in XR platform.
    /// </summary>
    public static class HandTracking
    {
        static I_HandleTrackingModule currentHandTrackModule = null;

        public static Vector3 rgbCameraAnchor = new Vector3(0.02631f, 0.05096f, 0.10121f);

        public static Quaternion rgbCameraQ = Quaternion.Euler(0, 0, 0);

        /// <summary>
        /// The rgb camera anchor
        /// </summary>
        public static Transform rgbCamAnchor;

        static bool sIsGestureDeviceLayoutRegistered = false;

        static bool isDeviceCalibrated;

        static string pathDeviceCalibration = "/backup/slam/device_calibration.xml";

        static string pathRGB2VIO = "/backup/rgb_vio_camera_params.xml";

        /// <summary>
        /// 如果为true，使用 xdevice plugin 中的接口做RGB相机位置的标定数据的读取操作。
        /// </summary>
        static bool useXDevicePluginCalibrationInterface = false;

        /// <summary>
        /// Fixed euler offset fror RGB anchor.
        /// </summary>
        static Vector3 kFixedEulerOffsetForRGBAnchor = new Vector3(-8, -5.5f, 0);

        static Camera m_sMainCam;
        static Camera sMainCam
        {
            get
            {
                if (!m_sMainCam)
                {
                    m_sMainCam = Camera.main;
                }
                return m_sMainCam;
            }
        }

        /// <summary>
        /// Read RGB calibration data.
        /// </summary>
        static void ReadRGBCalibration()
        {
            useXDevicePluginCalibrationInterface = !File.Exists(pathRGB2VIO);
            if (useXDevicePluginCalibrationInterface)
            {
                XCamCalibParam calibParam = default;
                XDevicePlugin.GetCameraCalibration(XCameraID.kCamIdRgb, XCameraID.kCamIdEye, true, ref calibParam);

                Vector3 tTran = new Vector3((float)calibParam.trans[0], (float)calibParam.trans[1], (float)calibParam.trans[2]);
                Matrix3x3 qMatrix = new Matrix3x3((float)calibParam.rot_mat[0], (float)calibParam.rot_mat[1], (float)calibParam.rot_mat[2],
                    (float)calibParam.rot_mat[3], (float)calibParam.rot_mat[4], (float)calibParam.rot_mat[5],
                    (float)calibParam.rot_mat[6], (float)calibParam.rot_mat[7], (float)calibParam.rot_mat[8]);
                Matrix4x4 righthand_rgb_2_eyecenter = Matrix3x3.ToTRS(qMatrix, tTran);//the RGB rig matrix to eye center

                Debug.LogFormat("XDevicePlugin RGB 2 Eye Calibration raw data: trans = {0}, rotation = {1}", tTran.ToString("F5"), qMatrix.ToString("F5"));
                Debug.LogFormat("XDevicePlugin (Right hand space) RGB 2 EyeCenter : {0}, {1}", ((Vector3)righthand_rgb_2_eyecenter.GetColumn(3)).ToString("F5"), righthand_rgb_2_eyecenter.rotation.eulerAngles.ToString("F5"));

                Quaternion qRight = qMatrix.GetRotation();
                //right hand to left hand:
                Vector3 unity_trans = new Vector3(Mathf.Abs(tTran.y), Mathf.Abs(tTran.x), -tTran.z);
                Quaternion unity_rotation = Quaternion.Euler(0, 180, -180) * new Quaternion(-qRight.x, -qRight.y, qRight.z, qRight.w);

                Debug.LogFormat("XDevicePlugin (Unity space) RGB 2 EyeCenter : {0}, {1}", unity_trans.ToString("F5"), unity_rotation.eulerAngles.ToString("F5"));

                //unity_rotation = unity_rotation * Quaternion.Euler(kFixedEulerOffsetForRGBAnchor);

                rgbCameraAnchor = unity_trans;
                rgbCameraQ = unity_rotation;
                isDeviceCalibrated = true;
            }
            else
            {
                try
                {
                    //RGB to VIO R:
                    XmlDocument xmlDoc_rgb = new XmlDocument(); // Create an XML document object
                    xmlDoc_rgb.Load(pathRGB2VIO); // Load the XML document from the specified file
                    XmlNode RotMat = xmlDoc_rgb.GetElementsByTagName("RotMat").Item(0);
                    XmlNode TransVec = xmlDoc_rgb.GetElementsByTagName("TransVec").Item(0);
                    ParseCalibrationMatrixFromText(RotMat["data"].InnerText, TransVec["data"].InnerText, out Matrix4x4 rgb2vioR);

                    //VIO L to VIO R:
                    XmlDocument xmlDoc_deviceCali = new XmlDocument(); // Create an XML document object
                    xmlDoc_deviceCali.Load(pathDeviceCalibration); // Load the XML document from the specified file
                    XmlNode camera_TrackingB = xmlDoc_deviceCali.GetElementsByTagName("Camera").Item(1);
                    XmlNode rig = camera_TrackingB["Rig"];
                    var rigAttri = rig.Attributes;
                    ParseCalibrationMatrixFromText(rigAttri["rowMajorRotationMat"].InnerText, rigAttri["translation"].InnerText, out Matrix4x4 vioL2R);

                    //VIO L to IMU:
                    XmlNode SFConfig = xmlDoc_deviceCali.GetElementsByTagName("SFConfig").Item(0);
                    XmlNode Stateinit = SFConfig["Stateinit"];
                    var StateinitAttr = Stateinit.Attributes;
                    ParseRotationVectorMatrixFromText(StateinitAttr["ombc"].InnerText, StateinitAttr["tbc"].InnerText, out Matrix4x4 vioL2Imu);

                    Debug.LogFormat("Rgb to VIO L: {0}", rgb2vioR);
                    Debug.LogFormat("VIO L to R: {0}", vioL2R);
                    Debug.LogFormat("VIO L to IMU: {0}", vioL2Imu);

                    Matrix4x4 rgb_to_IMU_space = vioL2Imu * vioL2R.inverse * rgb2vioR;
                    Matrix4x4 righthand_rgb_2_imu = IMUSpaceToRightHandSpace(rgb_to_IMU_space);
                    var eye2imu = ParamLoaderFloat16ToMatrix((int)ParamType.Design_Eye2IMU_TransMat_OpenGL_ARRAY16);
                    Matrix4x4 righthand_rgb_2_eyecenter = eye2imu.inverse * righthand_rgb_2_imu; //imu2eye * rgb2imu
                    Matrix4x4 unity_rgb_2_eyecenter = UnityRightHandSpaceToLeftHandSpace(righthand_rgb_2_eyecenter);
                    Debug.LogFormat("(Unity space) RGB 2 EyeCenter : {0}, {1}", ((Vector3)unity_rgb_2_eyecenter.GetColumn(3)).ToString("F5"), unity_rgb_2_eyecenter.rotation.eulerAngles.ToString("F5"));
                    rgbCameraAnchor = (Vector3)unity_rgb_2_eyecenter.GetColumn(3);
                    rgbCameraQ = unity_rgb_2_eyecenter.rotation;
                    isDeviceCalibrated = true;
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        static Matrix4x4 ParamLoaderFloat16ToMatrix(int type)
        {
            float[] float16 = new float[16];
            ParamLoader.ParamLoaderGetFloatArray(type, float16, System.Runtime.InteropServices.Marshal.SizeOf<float>() * 16);
            return new Matrix4x4()
            {
                m00 = float16[0],
                m01 = float16[1],
                m02 = float16[2],
                m03 = float16[3],
                m10 = float16[4],
                m11 = float16[5],
                m12 = float16[6],
                m13 = float16[7],
                m20 = float16[8],
                m21 = float16[9],
                m22 = float16[10],
                m23 = float16[11],
                m30 = float16[12],
                m31 = float16[13],
                m32 = float16[14],
                m33 = float16[15],
            };
        }

        static Matrix4x4 UnityRightHandSpaceToLeftHandSpace(Matrix4x4 rightHandSpace)
        {
            var rightHandT = (Vector3)rightHandSpace.GetColumn(3);
            var rightHandQ = rightHandSpace.rotation.eulerAngles;
            return Matrix4x4.TRS(new Vector3(-rightHandT.x, -rightHandT.y, rightHandT.z), Quaternion.Euler(rightHandQ.x, -rightHandQ.y, -rightHandQ.z), Vector3.one);
        }

        static Matrix4x4 IMUSpaceToRightHandSpace(Matrix4x4 imuSpace)
        {
            // P: (-0.00092, -0.06631, -0.01755), (-0.37781, -169.53320, -89.64063)
            var imu_t = (Vector3)imuSpace.GetColumn(3);
            Vector3 rightHand_t = new Vector3(imu_t.y, imu_t.x, -imu_t.z);
            Quaternion righthand_q = Quaternion.Euler(0, 180, 90) * imuSpace.rotation * Quaternion.Euler(0, 0, 180);
            return Matrix4x4.TRS(rightHand_t, righthand_q, Vector3.one);
        }


        private static void ParseRotationVectorMatrixFromText(string RotationVector, string TextPosition, out Matrix4x4 translate)
        {
            translate = Matrix4x4.identity;
            int row = 0, col = 0;
            try
            {
                var textInArray = RotationVector.Trim().Split(' ');
                Vector3 rotationVector = Vector3.zero;

                for (int i = 0; i < textInArray.Length; i++)
                {
                    string t = textInArray[i];
                    if (float.TryParse(t, out float value))
                    {
                        rotationVector[col] = value;
                        col++;
                    }
                }

                Matrix3x3 qMatrix = RotationVectorToMatrix(rotationVector);

                textInArray = TextPosition.Trim().Split(' ');
                Vector4 column = new Vector4(0, 0, 0, 1);
                col = 0;
                for (int i = 0; i < textInArray.Length; i++)
                {
                    string e = textInArray[i];
                    if (string.IsNullOrEmpty(e.Trim()))
                    {
                        continue;
                    }
                    column[col++] = float.Parse(e.Trim());
                }

                translate = Matrix3x3.ToTRS(qMatrix, (Vector3)column);
            }
            catch (System.Exception exc)
            {
                Debug.LogException(exc);
            }
        }



        private static void ParseCalibrationMatrixFromText(string textQuaternion, string textPosition, out Matrix4x4 translate)
        {
            translate = Matrix4x4.identity;
            int row = 0, col = 0;
            try
            {
                var textInArray = textQuaternion.Trim().Split(' ');
                for (int i = 0; i < textInArray.Length; i++)
                {
                    string t = textInArray[i];
                    if (float.TryParse(t, out float value))
                    {
                        translate[row, col] = value;
                        col++;
                        if (col >= 3)
                        {
                            col = 0;
                            row++;
                        }
                    }
                }

                textInArray = textPosition.Trim().Split(' ');
                Vector4 column = new Vector4(0, 0, 0, 1);
                col = 0;
                for (int i = 0; i < textInArray.Length; i++)
                {
                    string e = textInArray[i];
                    if (string.IsNullOrEmpty(e.Trim()))
                    {
                        continue;
                    }
                    column[col++] = float.Parse(e.Trim());
                }
                translate.SetColumn(3, column);
            }
            catch (System.Exception exc)
            {
                Debug.LogException(exc);
            }
        }

        /// <summary>
        /// Convert rotation vector in radians to a rotation matrix 3x3, using Rodrigues fomular.
        /// </summary>
        /// <param name="RotationVectorInRadians"></param>
        /// <returns>A 3x3 matrix represents a rotation quaternion.</returns>
        static Matrix3x3 RotationVectorToMatrix(Vector3 RotationVectorInRadians)
        {
            Vector3 r = RotationVectorInRadians.normalized;
            float theta = RotationVectorInRadians.magnitude;
            return Mathf.Cos(theta) * Matrix3x3.identity + (1 - Mathf.Cos(theta)) * new Matrix3x3(r, r) + Mathf.Sin(theta) * new Matrix3x3(0, -r.z, r.y, r.z, 0, -r.x, -r.y, r.x, 0);
        }

        /// <summary>
        /// Enable handle tracking
        /// </summary>
        public static void EnableHandTracking()
        {
            if(IsHandTrackingEnable)
            {
                return;
            }
            if (!HandTrackingT3D.UseHandTrackService)
            {
                if (!isDeviceCalibrated)
                {
                    ReadRGBCalibration();
                }
            }
            if (!rgbCamAnchor)
            {
                rgbCamAnchor = new GameObject("RGB Camera Anchor").transform;
                rgbCamAnchor.localPosition = rgbCameraAnchor;
                rgbCamAnchor.localRotation = rgbCameraQ;
                rgbCamAnchor.position = sMainCam.transform.TransformPoint(rgbCameraAnchor);
                rgbCamAnchor.rotation = sMainCam.transform.rotation * (rgbCameraQ);
                Object.DontDestroyOnLoad(rgbCamAnchor.gameObject);
            }
            if (!sIsGestureDeviceLayoutRegistered)
            {
                RegisterXRGestureLayout();

            }
            if (currentHandTrackModule == null)
            {
                currentHandTrackModule = new HandTrackingT3D();
            }
            if (currentHandTrackModule.EnableModule(new InitializeHandTrackingModuleParameter()
                {
                    TrackingAnchor = rgbCamAnchor,
                    smoothHandRotation = XimmerseXRSettings.instance.SmoothHandTrackingData,
                    smoothAngleRange = XimmerseXRSettings.instance.SmoothHandTrackRotationAngleDiffRange,
                    smoothControlCurve = XimmerseXRSettings.instance.SmoothRotationCurve,
                    smoothHandRotationAngularSpeed = XimmerseXRSettings.instance.SmoothingAngularSpeed,
                }))

                    AddHandAnchorInputDevices();

                {
                    if (XimmerseXRHandInput.leftHand == null)
                    {
                        //Adds virtural input devices for gesture input:
                        XimmerseXRHandInput _left = (XimmerseXRHandInput)InputSystem.AddDevice("XimmerseXRHandInput", "LeftHandGesture");
                        InputSystem.SetDeviceUsage(_left, "LeftHandGesture");
                        XimmerseXRHandInput.leftHand = _left;

                        XimmerseXRHandInput _right = (XimmerseXRHandInput)InputSystem.AddDevice("XimmerseXRHandInput", "RightHandGesture");
                        _right.handness = 1;
                        InputSystem.SetDeviceUsage(_right, "RightHandGesture");
                        XimmerseXRHandInput.rightHand = _right;

                        //XimmerseXRGazeInput _gazeInput = (XimmerseXRGazeInput)InputSystem.AddDevice("XimmerseXRGazeInput", "GazeInput");
                        //XimmerseXRGazeInput.gazeInput = _gazeInput;


                        Debug.Log("Ximmerse XR Gesture system : input devices are added");
                    }
                }
            XRManager.OnPostTrackUpdate += UpdateRGBCameraAnchor;
            XRManager.OnPostTrackUpdate += currentHandTrackModule.Tick;

        }

        static void UpdateRGBCameraAnchor()//To avoid main camera destroy issue so we dont attach rgb cam anchor as child of the main camera anymore
        {
            if (rgbCamAnchor && sMainCam)
            {
                rgbCamAnchor.position = sMainCam.transform.TransformPoint(rgbCameraAnchor);
                rgbCamAnchor.rotation = sMainCam.transform.rotation * (rgbCameraQ);
            }
            if (HandTracking.IsHandTrackingEnable && XimmerseXR.RGBCameraTexture && XimmerseXR.RGBCameraTexture.isPlaying == false)
            {
                XimmerseXR.RGBCameraTexture.Play();//SceneManager.LoadScene may stops the web cam texture
            }
        }

        /// <summary>
        /// Gesture input device layout for XR gesture 
        /// </summary>
        private static void RegisterXRGestureLayout()
        {
            InputSystem.RegisterLayout<XimmerseXRHandInput>(matches: new InputDeviceMatcher()
                .WithInterface("HandState"));

            //InputSystem.RegisterLayout<XimmerseXRGazeInput>(matches: new InputDeviceMatcher().WithInterface("GazeInputState"));

            InputSystem.RegisterLayout<HandAnchorInputDevice>(matches: new InputDeviceMatcher()
   .WithInterface("HandAnchorInputState"));

            InputSystem.RegisterLayout<DualHandGestureInputDevice>(matches: new InputDeviceMatcher()
   .WithInterface("DualHandGestureInputState"));

            sIsGestureDeviceLayoutRegistered = true;
            Debug.LogFormat("Gesture input devices layout has been registered.");
        }

        /// <summary>
        /// 
        /// </summary>
        public static I_HandleTrackingModule handTrackModule
        {
            get => currentHandTrackModule;
        }

        /// <summary>
        /// Disable hand tracking.
        /// </summary>
        public static void DisableHandTracking()
        {
            if(!IsHandTrackingEnable)
            {
                return;
            }
            if (currentHandTrackModule != null)
            {
                RemoveHandAnchorInputDevices();
                if (XimmerseXRHandInput.leftHand != null)
                {
                    InputSystem.RemoveDevice(XimmerseXRHandInput.leftHand);
                    InputSystem.RemoveDevice(XimmerseXRHandInput.rightHand);
                    //InputSystem.RemoveDevice(XimmerseXRGazeInput.gazeInput);
                }

                XRManager.OnPostTrackUpdate -= UpdateRGBCameraAnchor;
                XRManager.OnPostTrackUpdate -= currentHandTrackModule.Tick;
                currentHandTrackModule.DisableModule();

            }
        }

        /// <summary>
        /// Is hand tracking module currently enabled and running ?
        /// </summary>
        public static bool IsHandTrackingEnable
        {
            get => currentHandTrackModule != null && currentHandTrackModule.IsModuleEnabled;
        }

        public static HandTrackingInfo GetHandTrackingInfo(HandnessType handnessType)
        {
            if (currentHandTrackModule == null)
                return default;
            if (handnessType == HandnessType.Left)
            {
                return currentHandTrackModule.LeftHandTrackingInfo;
            }
            else
            {
                return currentHandTrackModule.RightHandTrackingInfo;
            }
        }

        /// <summary>
        /// If IsHandTrackingEnable is true, this is the left hand track info.
        /// </summary>
        public static HandTrackingInfo LeftHandTrackInfo
        {
            get
            {
                return currentHandTrackModule != null ? currentHandTrackModule.LeftHandTrackingInfo : default(HandTrackingInfo);
            }
        }

        /// <summary>
        /// If IsHandTrackingEnable is true, this is the right hand track info.
        /// </summary>
        public static HandTrackingInfo RightHandTrackInfo
        {
            get
            {
                return currentHandTrackModule != null ? currentHandTrackModule.RightHandTrackingInfo : default(HandTrackingInfo);
            }
        }


        /// <summary>
        /// If IsHandTrackingEnable is true, this is the prev left hand track info.
        /// </summary>
        public static HandTrackingInfo PrevLeftHandTrackInfo
        {
            get
            {
                return currentHandTrackModule != null ? currentHandTrackModule.PrevLeftHandTrackingInfo : default(HandTrackingInfo);
            }
        }

        /// <summary>
        /// If IsHandTrackingEnable is true, this is the right hand track info.
        /// </summary>
        public static HandTrackingInfo PrevRightHandTrackInfo
        {
            get
            {
                return currentHandTrackModule != null ? currentHandTrackModule.PrevRightHandTrackingInfo : default(HandTrackingInfo);
            }
        }

        /// <summary>
        /// Check if the hand tracking info matches the snapshot .
        /// </summary>
        /// <param name="handTrackingInfo"></param>
        /// <param name="snapshotData"></param>
        /// <returns></returns>
        public static bool TryRecognize(HandTrackingInfo handTrackingInfo, HandGestureSnapshotData snapshotData)
        {
            if (handTrackingInfo.IsValid == false)
                return false;

            float confience = snapshotData.ConfidenceValve;

            float bendnessThumb_template = snapshotData.snapshot.ThumbFinger.bendness;
            float bendnessThumb_user = handTrackingInfo.ThumbFinger.bendness;
            bool match = Mathf.Abs(bendnessThumb_template - bendnessThumb_user) <= (1 - confience);
            if (!match)
            {
                return false;
            }

            float bendnessIndex_template = snapshotData.snapshot.IndexFinger.bendness;
            float bendnessIndex_user = handTrackingInfo.IndexFinger.bendness;
            match = Mathf.Abs(bendnessIndex_template - bendnessIndex_user) <= (1 - confience);
            if (!match)
            {
                return false;
            }

            float bendnessMiddle_template = snapshotData.snapshot.MiddleFinger.bendness;
            float bendnessMiddle_user = handTrackingInfo.MiddleFinger.bendness;
            match = Mathf.Abs(bendnessMiddle_template - bendnessMiddle_user) <= (1 - confience);
            if (!match)
            {
                return false;
            }

            float bendnessRing_template = snapshotData.snapshot.RingFinger.bendness;
            float bendnessRing_user = handTrackingInfo.RingFinger.bendness;
            match = Mathf.Abs(bendnessRing_template - bendnessRing_user) <= (1 - confience);
            if (!match)
            {
                return false;
            }

            float bendnessLittle_template = snapshotData.snapshot.LittleFinger.bendness;
            float bendnessLittle_user = handTrackingInfo.LittleFinger.bendness;
            match = Mathf.Abs(bendnessLittle_template - bendnessLittle_user) <= (1 - confience);
            if (!match)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Registered hand anchor input device in unity input system
        /// </summary>
        private static void AddHandAnchorInputDevices()
        {
            //Adds left hand anchor input device:
            HandAnchorInputDevice left = (HandAnchorInputDevice)InputSystem.AddDevice("HandAnchorInputDevice", "LeftHandAnchor");
            left.inputConfig = XimmerseXRSettings.instance.leftHandAnchorInputDeviceConfig;
            InputSystem.SetDeviceUsage(left, "LeftHandAnchor");
            HandAnchorInputDevice.left = left;

            //Adds right hand anchor input device:
            HandAnchorInputDevice right = (HandAnchorInputDevice)InputSystem.AddDevice("HandAnchorInputDevice", "RightHandAnchor");
            right.inputConfig = XimmerseXRSettings.instance.rightHandAnchorInputDeviceConfig;
            right.handness = 1;
            InputSystem.SetDeviceUsage(right, "RightHandAnchor");
            HandAnchorInputDevice.right = right;

            //Adds dual hand input device:
            DualHandGestureInputDevice dualHandInput = (DualHandGestureInputDevice)InputSystem.AddDevice("DualHandGestureInputDevice", "DualHandGesture");
            dualHandInput.config = XimmerseXRSettings.instance.dualHandInputDeviceConfig;
            DualHandGestureInputDevice.dualHandInput = dualHandInput;

            Debug.LogFormat("Hand anchor input device is added. Layout: {0}", left.layout);
        }


        /// <summary>
        /// Remove hand anchor input devices in unity input system
        /// </summary>
        private static void RemoveHandAnchorInputDevices()
        {
            //var handAnchorInputDevice = InputSystem.devices.FirstOrDefault(x => x is HandAnchorInputDevice);
            //if (handAnchorInputDevice != null)
            //{
            //    InputSystem.RemoveDevice(handAnchorInputDevice);
            //    Debug.Log("Hand anchor input devices removed.");
            //}


            //var dualHandInput = InputSystem.devices.FirstOrDefault(x => x is DualHandGestureInputDevice);
            //if (dualHandInput != null)
            //{
            //    InputSystem.RemoveDevice(dualHandInput);
            //    Debug.Log("Dual hand input devices removed.");
            //}

            if (HandAnchorInputDevice.left != null)
            {
                InputSystem.RemoveDevice(HandAnchorInputDevice.left);
            }

            if (HandAnchorInputDevice.right != null)
            {
                InputSystem.RemoveDevice(HandAnchorInputDevice.right);
            }

            if (DualHandGestureInputDevice.dualHandInput != null)
            {
                InputSystem.RemoveDevice(DualHandGestureInputDevice.dualHandInput);
            }

            sIsGestureDeviceLayoutRegistered = false;
        }

#if UNITY_EDITOR

        //[UnityEditor.MenuItem("Ximmerse/Create Gesture Device")]
        private static void TestAddGestureDevice()
        {
            if (!sIsGestureDeviceLayoutRegistered)
            {
                InputSystem.RegisterLayout<XimmerseXRHandInput>(matches: new InputDeviceMatcher()
    .WithInterface("HandState"));

                InputSystem.RegisterLayout<XimmerseXRGazeInput>(matches: new InputDeviceMatcher()
  .WithInterface("GazeInputState"));
                sIsGestureDeviceLayoutRegistered = true;
            }
            XimmerseXRHandInput left = (XimmerseXRHandInput)InputSystem.AddDevice("XimmerseXRHandInput", "LeftHand");
            InputSystem.SetDeviceUsage(left, "LeftHand");
            XimmerseXRHandInput.leftHand = left;

            XimmerseXRHandInput right = (XimmerseXRHandInput)InputSystem.AddDevice("XimmerseXRHandInput", "RightHand");
            right.handness = 1;
            InputSystem.SetDeviceUsage(right, "RightHand");
            XimmerseXRHandInput.rightHand = right;


            XimmerseXRGazeInput gazeInput = (XimmerseXRGazeInput)InputSystem.AddDevice("XimmerseXRGazeInput", "GazeInput");

            Debug.LogFormat("Gesture device is added. Layout: {0}", left.layout);
            Debug.LogFormat("GazeInput device is added. Layout: {0}", gazeInput.layout);
        }

        //[UnityEditor.MenuItem("Ximmerse/Remove Gesture Device")]
        private static void RemoveDevice()
        {
            var gestureDevice = InputSystem.devices.FirstOrDefault(x => x is XimmerseXRHandInput);
            if (gestureDevice != null)
            {
                InputSystem.RemoveDevice(gestureDevice);
                Debug.Log("Gesture device is removed.");
            }

            var gazeDevice = InputSystem.devices.FirstOrDefault(x => x is XimmerseXRGazeInput);
            if (gestureDevice != null)
            {
                InputSystem.RemoveDevice(gazeDevice);
                Debug.Log("Gaze device is removed.");
            }
        }




        //[UnityEditor.MenuItem("Ximmerse/Create Hand Anchor Input Device")]
        private static void TestAddHandAnchorInputDevice()
        {
            AddHandAnchorInputDevices();
        }

        //[UnityEditor.MenuItem("Ximmerse/Remove Hand Anchor Input Device")]
        private static void TestRemoveHandAnchorInputDevice()
        {
            RemoveHandAnchorInputDevices();
        }
#endif

    }
}