using System;
using System.Runtime.InteropServices;
using AOT;
namespace TouchlessA3D
{
    public enum PreconfiguredCalibrations
    {
        TA3D_SAMSUNG_S10_26MM_4_BY_3 = 0,
        END = 1
    }

    public interface ICalibration
    {
        IntPtr getNativeCalibration();
    }

    // a sample construction of a the camera calibration 
    public class Calibration : ICalibration
    {
        //default calibration values for a samsung galaxy s10
        public float width = 1440f;
        public float height = 1080f;
        public float[,] calibrationMatrix = new float[3, 3] { { 1.0901655419118702E3f, 0f, 720f }, { 0f, 1.1031809655612601E3f, 540f }, { 0f, 0f, 1f }
    };
        public float[] distortionCoefficients = new float[8] {
      1.3524293927664879E-1f, -4.4922888561930802E-1f, 2.3574067791219083E-3f, -8.2965119619095784E-4f, 5.1354760994403104E-1f, 0f, 0f, 0f
    };
        public NativeCalls.ta3d_calibration_s m_nativeCalibration;
        private IntPtr ta3d_calibration_s = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeCalls.ta3d_calibration_s)));
        public Calibration()
        {

            m_nativeCalibration.calibration_size.width = width;
            m_nativeCalibration.calibration_size.height = height;

            m_nativeCalibration.camera_matrix = new float[9];
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    m_nativeCalibration.camera_matrix[row * 3 + col] = calibrationMatrix[row, col];
                }
            }
            m_nativeCalibration.distortion_coefficients = distortionCoefficients;

        }
        public Calibration(float width, float height, float[,] calibrationMatrix, float[] distortionCoefficients)
        {
            this.width = width;
            this.height = height;
            this.calibrationMatrix = calibrationMatrix;
            this.distortionCoefficients = distortionCoefficients;

            m_nativeCalibration.calibration_size.width = width;
            m_nativeCalibration.calibration_size.height = height;

            m_nativeCalibration.camera_matrix = new float[9];
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    m_nativeCalibration.camera_matrix[row * 3 + col] = calibrationMatrix[row, col];
                }
            }
            m_nativeCalibration.distortion_coefficients = distortionCoefficients;
        }

        public IntPtr getNativeCalibration()
        {
            Marshal.StructureToPtr(m_nativeCalibration, ta3d_calibration_s, false);
            return ta3d_calibration_s;
        }

        ~Calibration()
        {
            Marshal.FreeHGlobal(ta3d_calibration_s);
        }
    }
}