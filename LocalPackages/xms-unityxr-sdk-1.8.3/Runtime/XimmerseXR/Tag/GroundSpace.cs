using Unity.XR.CoreUtils;
using UnityEngine;
using Ximmerse.XR.Utils;
using Ximmerse.XR.Internal;
using System.Collections;

namespace Ximmerse.XR.Tag
{
    /// <summary>
    /// Implement large spatial positioning capabilities and handle appropriate events.
    /// </summary>
    public class GroundSpace : MonoBehaviour
    {
        #region Property

        private XROrigin xr;
        private GameObject tagground_clone;
        private float px, py, pz, rx, ry, rz, rw;
        private long predTimestampNano = 0;
        private bool first = false;
        private bool exit = false;
        private bool onfirstTrackingEnter = false;
        private bool onTrackingEnter = false;
        private bool onTrackingStay = false;
        private bool onTrackingExit = false;
        private bool isTrakingState;
        protected bool isvalid;

        private GameObject debugaxis;
        protected TrackingEvent trackingEvent;
        [Header("--- Marker Setting ---")]
        [SerializeField]
        protected int trackID = 65;
        [SerializeField]
        protected float m_Confidence = 0.85f;
        [Header("--- Vio Drift Threshold ---")]
        [SerializeField]
        protected float m_distance = 0.2f;
        [SerializeField]
        protected float m_angle = 20f;
        [Header("--- Tracking distance ---")]
        [SerializeField]
        protected float m_minDistance = 0.1f;
        [SerializeField]
        protected float m_maxDistance = 2f;
        [Header("--- Debug Setting ---")]
        [SerializeField]
        protected bool m_debugView;
        [SerializeField]
        protected float m_size = 0.17f;
        //private XDevicePlugin.XAttrTrackingInfo trackingInfo;

        protected int groupId = -1;

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

        Vector3 debugPos;
        Quaternion debugRot;

        Vector3 virturePos;
        Quaternion virtureRot;


        #endregion

        #region Unity

        protected virtual void Awake()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        private void Start()
        {
            GetXRComponent();
        }

        private void Update()
        {
            if (TagProfileLoading.Instance != null && XMR_Manager.Instance != null && XMR_Manager.Instance.TagRecognition)
            {
                isTrakingState = IsTracking();
                IsValid();

            }
            else
            {
                isTrakingState = false;
                isvalid = false;
            }

        }

        private void OnRenderObject()
        {
            if (m_debugView && isTrakingState)
            {
                for (int i = 0; i < TagProfileLoading.Instance.TrackingTagList.Count; i++)
                {
                    bool ret2 = NativePluginApi.Unity_GetTagPredict(i,
                        ref index, ref timestamp, ref state,
                        ref posX, ref posY, ref posZ,
                        ref rotX, ref rotY, ref rotZ, ref rotW,
                        ref confidence, ref marker_distance);

                    if (index == trackID)
                    {
                        break;
                    }
                }

                if (state != 0)
                {
                    debugPos.x = posX;
                    debugPos.y = posY;
                    debugPos.z = posZ;
                    debugRot.x = rotX;
                    debugRot.y = rotY;
                    debugRot.z = rotZ;
                    debugRot.w = rotW;
                    virturePos = xr.CameraFloorOffsetObject.transform.TransformPoint(debugPos);
                    virtureRot = xr.CameraFloorOffsetObject.transform.rotation * debugRot;
                    DrawDebugView(virturePos, virtureRot, m_size);
                }

            }
        }
        #endregion

        #region Method

        private void GetXRComponent()
        {
            if (xr == null)
            {
                xr = FindObjectOfType<XROrigin>();
            }
        }

        /// <summary>
        /// Plot the axes
        /// </summary>
        /// <param name="size"></param>
        private void DrawDebugView(Vector3 viewPos, Quaternion viewRot, float size)
        {
            RxDraw.DrawWirePlane(viewPos, viewRot, size, size, Color.green);
            if (xr != null)
            {
                var textRotation = Quaternion.LookRotation(viewPos - xr.Camera.transform.position);
                textRotation = textRotation.PitchNYaw();
                string debugTxt = trackID.ToString();
                RxDraw.Text3D(viewPos, textRotation, 0.02f, debugTxt, Color.green);
            }
            else
            {
                var textRotation = Quaternion.LookRotation(viewPos - Camera.main.transform.position);
                textRotation = textRotation.PitchNYaw();
                string debugTxt = trackID.ToString();
                RxDraw.Text3D(viewPos, textRotation, 0.02f, debugTxt, Color.green);
            }

            RxDraw.DrawTranslateGizmos(viewPos, viewRot, size * 0.85f);
        }
        /// <summary>
        /// Get the Tag tracking status
        /// </summary>
        /// <returns></returns>
        protected bool IsTracking()
        {
            if (TagProfileLoading.Instance.TrackingTagList.Contains(trackID))
            {
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Whether the tag is valid
        /// </summary>
        /// <returns></returns>
        protected bool IsValid()
        {
            if (trackID ==TagProfileLoading.Instance.TagFusion)
            {
                isvalid = true;
            }
            else
            {
                isvalid = false;
            }
            return isvalid;
        }
       
        #endregion
    }
}

