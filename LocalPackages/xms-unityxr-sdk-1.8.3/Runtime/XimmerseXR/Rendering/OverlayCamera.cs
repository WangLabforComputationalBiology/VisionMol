using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SXR;
using UnityEngine.Rendering;

namespace Ximmerse.XR.Rendering
{
    /// <summary>
    /// 用于渲染随头UI/3D 物体的组件。
    /// </summary>
    public class OverlayCamera : MonoBehaviour
    {
        /// <summary>
        /// Culling mask of the overlay camera
        /// </summary>
        [Tooltip("Culling mask of the overlay camera")]
        public LayerMask cullingMask = 1 << 5;

        /// <summary>
        /// Depth of the overlay camera
        /// </summary>
        [Tooltip("Depth of the overlay camera")]
        public int depth = 1;

        [Range(0.1f, 1)]
        public float renderScale = 1;

        const int kBufferCount = 1;

        RenderTexture[] renderTexturesL = new RenderTexture[kBufferCount];

        RenderTexture[] renderTexturesR = new RenderTexture[kBufferCount];

        private int rtIndex = 0;

        Transform eyeTrans;

        Camera overlayCamL, overlayCamR;

        /// <summary>
        /// Background color of overlay camera
        /// </summary>
        Color m_overlayBackgroundColor = new Color(0, 0, 0, 0);

        public Color overlayBackgroundColor
        {
            get => m_overlayBackgroundColor;
            set
            {
                m_overlayBackgroundColor = value;
                if (overlayCamL)
                    overlayCamL.backgroundColor = value;
                if (overlayCamR)
                    overlayCamR.backgroundColor = value;
            }
        }

        public void FadeIn(float duration)
        {
            this.enabled = true;
            this.StartCoroutine(fadeIn(duration));
        }

        public void FadeOut(float duration)
        {
            this.enabled = true;
            this.StartCoroutine(fadeOut(duration));
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!eyeTrans)
            {
                eyeTrans = Camera.main.transform;
            }

            if (!eyeTrans)
            {
                Debug.LogError("MainCamera is not found , overlay camera failed to start !");
                return;
            }
            bool isAndroid = Application.platform == RuntimePlatform.Android;
            Matrix4x4 projectionMatrix = default(Matrix4x4);
            float ipd = 0.062f;
            if (isAndroid)
            {
                float l = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Left_FLOAT);
                float r = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Right_FLOAT);
                float t = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Top_FLOAT);
                float b = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Bottom_FLOAT);
                float n = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Near_FLOAT);
                float f = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Far_FLOAT);
                projectionMatrix = GetPerspectiveProjectionMatrix(l, r, b, t, n, f);
                ipd = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_IPD_FLOAT);
            }
            else
            {
                projectionMatrix = Matrix4x4.Perspective(Camera.main.fieldOfView, Camera.main.aspect, 0.01f, 1000);
            }


            //Instantiate overlay cameras:
            for (int i = 0; i < kBufferCount; i++)
            {
                renderTexturesL[i] = new RenderTexture((int)(Screen.width * renderScale), (int)(Screen.height * renderScale), 24, RenderTextureFormat.Default);
                if (!renderTexturesL[i].IsCreated())
                {
                    renderTexturesL[i].Create();
                }

                renderTexturesR[i] = new RenderTexture((int)(Screen.width * renderScale), (int)(Screen.height * renderScale), 24, RenderTextureFormat.Default);
                if (!renderTexturesR[i].IsCreated())
                {
                    renderTexturesR[i].Create();
                }
            }


            GameObject overlayCamLGo = new GameObject("Overlay Camera L", new System.Type[] { typeof(Camera) });
            GameObject overlayCamRGo = new GameObject("Overlay Camera R", new System.Type[] { typeof(Camera) });

            overlayCamL = overlayCamLGo.GetComponent<Camera>();
            overlayCamL.clearFlags = CameraClearFlags.SolidColor;
            overlayCamL.depth = this.depth;
            overlayCamL.backgroundColor = new Color(0, 0, 0, 0);
            overlayCamL.cullingMask = this.cullingMask;
            overlayCamL.targetTexture = renderTexturesL[0];
            overlayCamL.projectionMatrix = projectionMatrix;

            overlayCamR = overlayCamRGo.GetComponent<Camera>();
            overlayCamR.clearFlags = CameraClearFlags.SolidColor;
            overlayCamR.depth = this.depth;
            overlayCamR.cullingMask = this.cullingMask;
            overlayCamR.backgroundColor = new Color(0, 0, 0, 0);
            overlayCamR.targetTexture = renderTexturesR[0];
            overlayCamR.projectionMatrix = projectionMatrix;

            overlayCamLGo.transform.SetParent(eyeTrans.transform, false);
            overlayCamRGo.transform.SetParent(eyeTrans.transform, false);

            overlayCamLGo.transform.localPosition = new Vector3(-ipd * 0.5f, 0, 0);
            overlayCamRGo.transform.localPosition = new Vector3(ipd * 0.5f, 0, 0);

            if (isAndroid)
            {
                XimmerseXR.OverlayRendering = true;
                XimmerseXR.SetOverlayRendererTextureID(overlayCamL.targetTexture.GetNativeTexturePtr().ToInt32(), overlayCamR.targetTexture.GetNativeTexturePtr().ToInt32());

                //StartCoroutine(swapFrame());
            }
        }

        private void OnDisable()
        {
            EnableOverlayCameras(false);
        }

        private void OnEnable()
        {
            EnableOverlayCameras(true);
        }

        private void EnableOverlayCameras(bool enabled)
        {
            if (overlayCamL)
            {
                overlayCamL.enabled = enabled;
            }
            if (overlayCamR)
            {
                overlayCamR.enabled = enabled;
            }
            if (Application.platform == RuntimePlatform.Android)
                XimmerseXR.OverlayRendering = enabled;
        }



        //private void OnDestroy()
        //{
        //  //  StopCoroutine(swapFrame());
        //}

        //IEnumerator swapFrame()
        //{
        //    WaitForEndOfFrame eof = new WaitForEndOfFrame();
        //    while (true)
        //    {
        //        yield return eof;
        //        rtIndex = (rtIndex + 1) % kBufferCount;
        //        overlayCamL.targetTexture = this.renderTexturesL[rtIndex];
        //        overlayCamR.targetTexture = this.renderTexturesR[rtIndex];
        //        XimmerseXR.SetOverlayRendererTextureID(overlayCamL.targetTexture.GetNativeTexturePtr().ToInt32(), overlayCamR.targetTexture.GetNativeTexturePtr().ToInt32());
        //    }
        //}

        static Matrix4x4 GetPerspectiveProjectionMatrix(float left, float right, float bottom, float top, float near, float far)
        {
            float x = 2.0F * near / (right - left);
            float y = 2.0F * near / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = -(2.0F * far * near) / (far - near);
            float e = -1.0F;
            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x;
            m[0, 1] = 0;
            m[0, 2] = a;
            m[0, 3] = 0;
            m[1, 0] = 0;
            m[1, 1] = y;
            m[1, 2] = b;
            m[1, 3] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = c;
            m[2, 3] = d;
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = e;
            m[3, 3] = 0;
            return m;
        }

        /// <summary>
        /// 逐渐变透明
        /// </summary>
        /// <returns></returns>
        IEnumerator fadeIn(float time)
        {
            EnableOverlayCameras(true);
            float st = Time.time;
            while ((Time.time - st) <= time)
            {
                float _alpha = 1 - (Time.time - st / 1);
                overlayBackgroundColor = new Color(0, 0, 0, _alpha);
               // Debug.LogFormat("FadeIn setting overlay alpha: {0}", _alpha);
                yield return null;
            }
            overlayBackgroundColor = new Color(0, 0, 0, 0);
            EnableOverlayCameras(false);
        }

        /// <summary>
        /// 逐渐变黑
        /// </summary>
        /// <returns></returns>
        IEnumerator fadeOut(float time)
        {
            EnableOverlayCameras(true);
            float st = Time.time;
            while ((Time.time - st) <= time)
            {
                float _alpha = (Time.time - st) / time;
                overlayBackgroundColor = new Color(0, 0, 0, _alpha);
               // Debug.LogFormat("FadeOut setting overlay alpha: {0}", _alpha);
                yield return null;
            }
            overlayBackgroundColor = new Color(0, 0, 0, 1);
            EnableOverlayCameras(true);
        }
    }
}