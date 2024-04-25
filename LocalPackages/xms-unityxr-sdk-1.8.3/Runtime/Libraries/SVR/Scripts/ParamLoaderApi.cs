using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
namespace SXR
{
    public enum DeviceID
    {
        Device_Default = 0x100,
        Device_RhinoXPro = 0x200,
        Device_RhinoX2 = 0x300,
        Device_RhinoXH = 0x400
    };

    enum LoadFileType
    {
        LoadSvrConfig = 0,
        LoadAligmentCalibLeft = 1,
        LoadAligmentCalibRight = 2
    };

    enum ParamBase
    {
        ParamBase_Device = 0x0,
        ParamBase_Render = 0x10000,
        ParamBase_Design = 0x20000,
        ParamBase_Tracking = 0x30000
    };

    enum ParamDataType
    {
    ParamDataType_INT= 0x0000, 
    ParamDataType_BOOL = 0x1000,
    ParamDataType_FLOAT = 0x2000,
    ParamDataType_FLOAT_ARRAY = 0x3000,
    ParamDataType_Max = 0xf000
    };

    public enum ParamType
    {
        ParamIsLoader = ParamDataType.ParamDataType_BOOL + 0,
        Param_DeviceID = ParamBase.ParamBase_Device + ParamDataType.ParamDataType_INT + 1,
        ///< render param
        Render_EyeBufferWidth_INT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_INT + 0,
        Render_EyeBufferHeight_INT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_INT + 1,
        Render_WarpMeshType_INT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_INT + 2,
        Render_WarpMeshRows_INT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_INT + 3,
        Render_WarpMeshCols_INT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_INT + 4,
        Render_EnableTimeWarp_BOOL = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_BOOL + 5,
        Render_DisableLensCorrection_BOOL = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_BOOL + 6,
        Render_TimeWarpDelayBetweenEyes_BOOL = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_BOOL + 7,
        Render_IPD_FLOAT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_FLOAT + 8,
        Render_EyePitch_FLOAT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_FLOAT + 9,
        Render_Frustum_Near_FLOAT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_FLOAT + 10,
        Render_Frustum_Far_FLOAT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_FLOAT + 11,
        Render_Frustum_Left_FLOAT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_FLOAT + 12,
        Render_Frustum_Right_FLOAT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_FLOAT + 13,
        Render_Frustum_Top_FLOAT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_FLOAT + 14,
        Render_Frustum_Bottom_FLOAT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_FLOAT + 15,
        Render_EyeBufferFovX_FLOAT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_FLOAT + 16,
        Render_EyeBufferFovY_FLOAT = ParamBase.ParamBase_Render + ParamDataType.ParamDataType_FLOAT + 17,

  ///< design param                                                                                               
          Design_VPU_UseFixedRotateDownward_BOOL = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_BOOL + 0,                             // flag to use fixed rotate downward angle for VPU
          Design_VPU_RotateLeft_FlOAT = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT + 1,                                       // angle of VPU rotate left; unit: degrees
          Design_VPU_RotateDownward_FlOAT  = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT + 2,                                  // angle of VPU rotate downward; unit: degrees
          Design_Eye2IMU_TransMat_OpenGL_ARRAY16 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 3,                      // 4x4 transformation matrix of IMU pose relative to eye center, both use right-handed coordinate system
          Design_Eye2IMU_TransMat_Unity_ARRAY16 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 4,                       // 4x4 transformation matrix of IMU pose relative to eye center, both use left-handed coordinate system
          Design_Eye2IMU_Pos_Unity_ARRAY3 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 5,                             // 3x1 position of IMU relative to eye center, both use left-handed coordinate system; unit: m
          Design_Eye2IMU_Euler_Unity_ARRAY3 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 6,                           // 3x1 rotation angles (x/y/z) of IMU relative to eye center, both use right-handed coordinate system; unit: degrees
          Design_Eye2VPU_TransMat_OpenGL_ARRAY16 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 7,                       // 4x4 transformation matrix of VPU pose relative to eye center, both use right-handed coordinate system
          Design_Eye2VPU_TransMat_Unity_ARRAY16 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 8,                        // 4x4 transformation matrix of VPU pose relative to eye center, both use left-handed coordinate system
          Design_Eye2VPU_Pos_Unity_ARRAY3 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 9,                             // 3x1 position of VPU relative to eye center, both use left-handed coordinate system; unit: m
          Design_Eye2VPU_Euler_Unity_ARRAY3 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 10,                           // 3x1 rotation angles (x/y/z) of VPU relative to eye center, both use right-handed coordinate system; unit: degrees
          Design_IMU2VPU_TransMat_OpenGL_ARRAY16 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 11,                      // 4x4 transformation matrix of VPU pose relative to IMU, both use right-handed coordinate system
          Design_IMU2VPU_TransMat_Unity_ARRAY16 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 12,                       // 4x4 transformation matrix of VPU pose relative to IMU, both use left-handed coordinate system
          Design_IMU2VPU_Pos_Unity_ARRAY3  = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 13,                           // 3x1 position of VPU relative to IMU, both use left-handed coordinate system; unit: m
          Design_IMU2VPU_Euler_Unity_ARRAY3  = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 14,                         // 3x1 rotation angles (x/y/z) of VPU relative to IMU, both use right-handed coordinate system; unit: degrees
          Design_MarkerController2Model_TransMat_OpenGL_ARRAY16 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 15,      // 4x4 transformation matrix of model pose relative to marker controller track-pose, both use right-handed coordinate system
          Design_MarkerController2Model_TransMat_Unity_ARRAY16 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 16,       // 4x4 transformation matrix of model pose relative to marker controller track-pose, both use left-handed coordinate system
          Design_MarkerController2Model_Pos_Unity_ARRAY3 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 17,             // 3x1 position of model relative to marker controller track-pose, both use left-handed coordinate system; unit: m
          Design_MarkerController2Model_Euler_Unity_ARRAY3 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 18,           // 3x1 rotation angles (x/y/z) of model relative to marker controller track-pose, both use right-handed coordinate system; unit: degrees
          Design_RingController2Model_TransMat_OpenGL_ARRAY16 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 19,        // 4x4 transformation matrix of model pose relative to ring controller track-pose, both use right-handed coordinate system
          Design_RingController2Model_TransMat_Unity_ARRAY16 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 20,         // 4x4 transformation matrix of model pose relative to ring controller track-pose, both use left-handed coordinate system
          Design_RingController2Model_Pos_Unity_ARRAY3 = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 21,               // 3x1 position of model relative to ring controller track-pose, both use left-handed coordinate system; unit: m
          Design_RingController2Model_Euler_Unity_ARRAY3  = ParamBase.ParamBase_Design + ParamDataType.ParamDataType_FLOAT_ARRAY + 22,            // 3x1 rotation angles (x/y/z) of model relative to ring controller track-pose, both use right-handed coordinate system; unit: degrees

        ///< tracking param
        Track_VIO_MaxPredictTime_FLOAT = ParamBase.ParamBase_Tracking + ParamDataType.ParamDataType_FLOAT + 1,
        Track_VIO_FallbackTime_FLOAT = ParamBase.ParamBase_Tracking + ParamDataType.ParamDataType_FLOAT + 2,
        Track_Controller_MaxPredictTime_FLOAT = ParamBase.ParamBase_Tracking + ParamDataType.ParamDataType_FLOAT + 3,
        Track_Controller_MinConfidence_FLOAT = ParamBase.ParamBase_Tracking + ParamDataType.ParamDataType_FLOAT + 4,
        Track_Beacon_MaxPredictTime_FLOAT = ParamBase.ParamBase_Tracking + ParamDataType.ParamDataType_FLOAT + 5,

    };
    public class ParamLoader
    {
       
        [DllImport("sxrapi")]
        public static extern bool ParamLoaderInit(int deviceID);
        [DllImport("sxrapi")]
        public static extern void ParamLoaderExit();
        [DllImport("sxrapi")]
        public static extern bool ParamLoaderAddFloatArray(int type, float[] value, int len);
        [DllImport("sxrapi")]
        public static extern bool ParamLoaderGetFloatArray(int type,  float[] value, int len);
        [DllImport("sxrapi")]
        public static extern bool ParamLoaderSetFloatArray(int type,  float[] value, int len);
        [DllImport("sxrapi")]
        public static extern void ParamLoaderPrint(int type, string tag);

        [DllImport("sxrapi")]
        public static extern bool ParamLoaderAddFloat(int type, float value);
        [DllImport("sxrapi")]
        public static extern bool ParamLoaderSetFloat(int type, float value);
        [DllImport("sxrapi")]
        public static extern float ParamLoaderGetFloat(int type);

        [DllImport("sxrapi")]
        public static extern bool ParamLoaderAddBool(int type, bool value);
        [DllImport("sxrapi")]
        public static extern bool ParamLoaderSetBool(int type, bool value);
        [DllImport("sxrapi")]
        public static extern bool ParamLoaderGetBool(int type);

        [DllImport("sxrapi")]
        public static extern bool ParamLoaderAddInt(int type, int value);
        [DllImport("sxrapi")]
        public static extern bool ParamLoaderSetInt(int type, int value);
        [DllImport("sxrapi")]
        public static extern int ParamLoaderGetInt(int type);


    }

}