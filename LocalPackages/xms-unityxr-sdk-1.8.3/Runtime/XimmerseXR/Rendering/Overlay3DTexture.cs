using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SXR;
namespace Ximmerse.XR.Rendering
{
    public class Overlay3DTexture : MonoBehaviour
    {
        [Tooltip("The texture to be displayed at world space.")]
        public Texture2D texture;

        [Tooltip("The world anchor transform to display the texture")]
        public Transform anchor;

        [Range(0, 3)]
        [Tooltip("Layer index to display the texture, must be 0-3")]
        public int layerIndex = 1;

        [Tooltip("Size of the displayed texture.")]
        public Vector2 size = Vector2.one;

        Camera m_MainCamera;

        float[] anchorLL = new float[4];//mvp(4) + uv(2)
        float[] anchorLT = new float[4];//mvp(4) + uv(2)
        float[] anchorRT = new float[4];//mvp(4) + uv(2)
        float[] anchorRB = new float[4];//mvp(4) + uv(2)
        float[] matrixInFloats = new float[16];

        private void LateUpdate()
        {
            SetLayerData();
            //SetLayerData();
        }

        void SetLayerData()
        {
            bool isAndroid = Application.platform == RuntimePlatform.Android;
            if (!isAndroid || !anchor || !texture)
            {
                return;
            }
            if (!m_MainCamera)
            {
                m_MainCamera = Camera.main;
            }
            if (!m_MainCamera)
            {
                Debug.LogError("OverlayTexture : no main camera !");
                return;
            }
            Matrix4x4 projectionMatrix = default(Matrix4x4);
            float ipd = 0.062f;
            float l = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Left_FLOAT);
            float r = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Right_FLOAT);
            float t = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Top_FLOAT);
            float b = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Bottom_FLOAT);
            float n = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Near_FLOAT);
            float f = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Far_FLOAT);
            projectionMatrix = GetPerspectiveProjectionMatrix(l, r, b, t, n, f);
            Matrix4x4 mvp = projectionMatrix * m_MainCamera.worldToCameraMatrix * anchor.localToWorldMatrix;

            Vector4 clipLL = mvp * new Vector4(-size.x * 0.5f, -size.y * 0.5f, 0, 1);
            Vector4 clipLT = mvp * new Vector4(-size.x * 0.5f, size.y * 0.5f, 0, 1);
            Vector4 clipRT = mvp * new Vector4(size.x * 0.5f, size.y * 0.5f, 0, 1);
            Vector4 clipRB = mvp * new Vector4(size.x * 0.5f, -size.y * 0.5f, 0, 1);

            anchorLL[0] = clipLL[0];
            anchorLL[1] = clipLL[1];
            anchorLL[2] = clipLL[2];
            anchorLL[3] = clipLL[3];
            //anchorLL[4] = 0;
            //anchorLL[5] = 0;

            anchorLT[0] = clipLT[0];
            anchorLT[1] = clipLT[1];
            anchorLT[2] = clipLT[2];
            anchorLT[3] = clipLT[3];
            //anchorLT[4] = 0;
            //anchorLT[5] = 1;

            anchorRT[0] = clipRT[0];
            anchorRT[1] = clipRT[1];
            anchorRT[2] = clipRT[2];
            anchorRT[3] = clipRT[3];
            //anchorRT[4] = 1;
            //anchorRT[5] = 1;

            anchorRB[0] = clipRB[0];
            anchorRB[1] = clipRB[1];
            anchorRB[2] = clipRB[2];
            anchorRB[3] = clipRB[3];
            //anchorRB[4] = 1;
            //anchorRB[5] = 0;

            //Call SVR api :
            {
                var mtx = mvp;
                for (int i = 0; i < 16; i++)
                {
                    matrixInFloats[i] = mtx[i];
                }
                SvrPluginAndroid.Unity_setWorldOverlayTexture(true, this.layerIndex, this.texture.GetNativeTexturePtr().ToInt32(),
                    this.size.x, this.size.y, matrixInFloats, anchorLL, anchorLT, anchorRT, anchorRB
                    );
            }
        }


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
    }
}