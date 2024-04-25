using SXR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.XR.Utils
{
    public class UltiHeadset : MonoBehaviour
    {
        #region Instance
        private static UltiHeadset instance;
        public static UltiHeadset Instance
        {
            get => instance;
        }
        private void Awake()
        {
            instance = this;
        }
        #endregion

        #region Property
        int _reticleTextureId;
        int _reticleTexW;
        int _reticleTexH;
        private Texture screenTexture;
        /// <summary>
        /// Screen texture
        /// </summary>
        public Texture ScreenTexture
        {
            get => screenTexture;
            set => screenTexture = value;
        }
        #endregion

        #region Method
        /// <summary>
        /// Get current headset type
        /// </summary>
        /// <returns></returns>
        public DeviceID GetCurrentDeviceType()
        {
            DeviceID deviceID;
            int currentDeviceId =ParamLoader.ParamLoaderGetInt((int)ParamType.Param_DeviceID);

            switch (currentDeviceId)
            {
                case (int)DeviceID.Device_RhinoX2:
                    deviceID = DeviceID.Device_RhinoX2;
                    break;
                    case (int)DeviceID.Device_RhinoXPro:
                    deviceID = DeviceID.Device_RhinoXPro;
                    break;
                default:
                    deviceID = DeviceID.Device_RhinoXH;
                    break;
            }
            return deviceID;
        }

        /// <summary>
        /// Whether to enable screen texture
        /// </summary>
        /// <param name="enable"></param>switch
        public void SetScreenTextureEnable(bool enable)
        {
            if (screenTexture==null&& enable)
            {
                Debug.LogError("Current texture is Null");
            }
            else if (screenTexture != null&&enable)
            {
                XimmerseXR.DisplayReticle = enable;
                SetScreenTexture();
            }
            else
            {
                XimmerseXR.DisplayReticle = enable;
            }
        }
        /// <summary>
        /// Set screen texture
        /// </summary>
        /// <param name="texture"></param>screen texture
        public void SetScreenTexture(Texture texture)
        {
            screenTexture = texture;
            _reticleTextureId = screenTexture.GetNativeTexturePtr().ToInt32();
            _reticleTexW = screenTexture.width;
            _reticleTexH = screenTexture.height;
            XimmerseXR.SetReticleTexture(_reticleTextureId, _reticleTexW, _reticleTexH);
        }

        private void SetScreenTexture()
        {
            _reticleTextureId = screenTexture.GetNativeTexturePtr().ToInt32();
            _reticleTexW = screenTexture.width;
            _reticleTexH = screenTexture.height;
            XimmerseXR.SetReticleTexture(_reticleTextureId, _reticleTexW, _reticleTexH);
        }

        #endregion

        #region Unity

        private void OnDestroy()
        {
            if (XimmerseXR.DisplayReticle)
            {
                XimmerseXR.DisplayReticle = false;
            }
        }
        #endregion
    }
}
