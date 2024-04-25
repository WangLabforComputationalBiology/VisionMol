using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Text;


namespace Ximmerse.XR
{
    /// <summary>
    /// utility codes on manipulation of RGB camera at RhinoX Pro.
    /// </summary>
    public static class RGBCameraUtils
    {
        /// <summary>
        /// At RhinoX pro, the XML file path contains the rgb camera parameter.
        /// </summary>
        const string kRGBCalibrationXMLPath_RhinoX_Pro = "/backup/rgb_vio_camera_params.xml";

        /// <summary>
        /// 如果为true，使用 xdevice plugin 中的接口做标定数据的读取操作。
        /// </summary>
        static bool useXDevicePluginCalibrationInterface = false;

        /// <summary>
        /// 从RhinoX pro 机器中读取RGB相机的标定外参.
        /// </summary>
        /// <param name="CalibrationMatrix">A 3 x 3 float array to calibrate RGB camera.</param>
        /// <param name="DistortionCoefficients"> A 8 array to identify the distortion coefficients of the RGB camera.</param>
        public static void ParseRGBCameraParams(out float[,] CalibrationMatrix, out float[] DistortionCoefficients)
        {
            useXDevicePluginCalibrationInterface = !System.IO.File.Exists(kRGBCalibrationXMLPath_RhinoX_Pro);
            if (useXDevicePluginCalibrationInterface)
            {
                XCamCalibParam calibParam = default;
                XDevicePlugin.GetCameraCalibration(XCameraID.kCamIdRgb, XCameraID.kCamIdEye, true, ref calibParam);
                CalibrationMatrix = new float[3, 3];
                for(int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        CalibrationMatrix[row, col] = (float)calibParam.intrinsic[row * 3 + col];
                    }
                }

                DistortionCoefficients = new float[8];
                for(int i = 0; i < DistortionCoefficients.Length; i++)
                {
                    DistortionCoefficients[i] = (float)calibParam.distort[i];
                }

#if DEVELOPMENT_BUILD
                StringBuilder buffer = new StringBuilder();
                buffer.AppendFormat("XDevicePlugin - Cam Size {0}x{1} CalibrationMatrix : ", calibParam.size[0], calibParam.size[1]);
                for (int row = 0; row < 3; row++)
                    for (int col = 0; col < 3; col++)
                    {
                        buffer.AppendFormat("{0} ", CalibrationMatrix[col, row]);
                    }


                buffer.AppendFormat("\r\nDistortionCoefficients : ");
                foreach (var coefficient in DistortionCoefficients)
                {
                    buffer.AppendFormat("{0} ", coefficient);
                }
                Debug.Log(buffer.ToString());
#endif
            }
            else
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
                    xmlDoc.Load(kRGBCalibrationXMLPath_RhinoX_Pro); // Load the XML document from the specified file
                    XmlNodeList RGBCamMats = xmlDoc.GetElementsByTagName("RGBCamMat");
                    ParseCalibrationMatrix(RGBCamMats, out CalibrationMatrix);
                    XmlNodeList RGBDistCoeff = xmlDoc.GetElementsByTagName("RGBDistCoeff");
                    ParseDistortionCoefficients(RGBDistCoeff, out DistortionCoefficients);

#if DEVELOPMENT_BUILD
                    StringBuilder buffer = new StringBuilder();
                    buffer.AppendFormat("CalibrationMatrix : ");
                    for (int row = 0; row < 3; row++)
                        for (int col = 0; col < 3; col++)
                        {
                            buffer.AppendFormat("{0} ", CalibrationMatrix[col, row]);
                        }


                    buffer.AppendFormat("\r\nDistortionCoefficients : ");
                    foreach (var coefficient in DistortionCoefficients)
                    {
                        buffer.AppendFormat("{0} ", coefficient);
                    }
                    Debug.Log(buffer.ToString());
#endif
                }
                catch (Exception exc)
                {
                    CalibrationMatrix = null;
                    DistortionCoefficients = null;
                    Debug.LogErrorFormat("ParseRGBCameraParams error : {0}", exc.Message);
                    Debug.LogException(exc);
                }
            }
        }



        /// <summary>
        /// Reads XML element and output a calibrationMatrix (3x3) array
        /// </summary>
        /// <param name="RGBCamMats"></param>
        /// <param name="calibrationMatrix"></param>
        private static void ParseCalibrationMatrix(XmlNodeList RGBCamMats, out float[,] calibrationMatrix)
        {
            calibrationMatrix = new float[3, 3];
            int row = 0, col = 0;
            try
            {
                XmlElement dataElement = RGBCamMats.Item(0)["data"];
                var textInArray = dataElement.InnerText.Trim().Split(' ');
                for (int i = 0; i < textInArray.Length; i++)
                {
                    string t = textInArray[i];
                    if (float.TryParse(t, out float value))
                    {
                        calibrationMatrix[row, col] = value;
                        col++;
                        if (col >= 3)
                        {
                            col = 0;
                            row++;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
            }
        }


        /// <summary>
        /// Reads XML element and output a distortion cofficient (8) array
        /// </summary>
        /// <param name="RGBCamMats"></param>
        /// <param name="DistortionCofficients"></param>
        private static void ParseDistortionCoefficients(XmlNodeList RGBDistCoeff, out float[] DistortionCofficients)
        {
            DistortionCofficients = new float[8];
            int col = 0;
            try
            {
                XmlElement dataElement = RGBDistCoeff.Item(0)["data"];
                var textInArray = dataElement.InnerText.Trim().Split(' ');
                for (int i = 0; i < textInArray.Length; i++)
                {
                    string t = textInArray[i];
                    if (float.TryParse(t, out float value))
                    {
                        DistortionCofficients[col] = value;
                        col++;
                        if (col >= 8)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.LogException(exc);
            }
        }
    }
}