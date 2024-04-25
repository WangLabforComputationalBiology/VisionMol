using UnityEngine;
using Ximmerse.XR.Utils;
using Ximmerse.XR.Internal;
using System.Collections;
using System.Linq;
using Unity.XR.CoreUtils;

namespace Ximmerse.XR.Tag
{
    /// <summary>
    /// Implement tracing capabilities and handle appropriate events.
    /// </summary>
    public class XRTracking : MonoBehaviour
    {
        #region Property
        [Header("--- Marker Setting ---")]
        [SerializeField]
        protected int trackID = 65;
        [SerializeField]
        protected LostState trackingIsLost = LostState.Stay;
        [Header("--- Debug Setting ---")]
        [SerializeField]
        protected bool m_debugView = false;
        [SerializeField]
        protected float m_size = 0.17f;


        GameObject tracking_clone;
        private long predTimestampNano = 0;
        private Vector3 postrackingfix;

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

        Vector3 posDifference;
        Quaternion rotDifference;

        private XROrigin xr;

        private bool isTracked = false;

        protected Vector3 posOffset = Vector3.zero;
        protected Quaternion rotOffset = Quaternion.identity;

        #endregion

        #region Unity
        private void Start()
        {
            xr = FindObjectOfType<XROrigin>();
        }

        private void Update()
        {
            TagTracking();
        }
        #endregion

        #region Method

        /// <summary>
        /// Tag Tracking function
        /// </summary>
        private void TagTracking()
        {
            if (TagProfileLoading.Instance != null && XMR_Manager.Instance != null && XMR_Manager.Instance.TagRecognition)
            {
                trackingstate = IsTracking();
                if (trackingstate)
                {
#if !UNITY_EDITOR
                    for (int i = 0; i < TagProfileLoading.Instance.TrackingTagList.Count; i++)
                    {
                        bool ret2 = NativePluginApi.Unity_GetTagPredict(i,
                            ref index, ref timestamp, ref state,
                            ref posX, ref posY, ref posZ,
                            ref rotX, ref rotY, ref rotZ, ref rotW,
                            ref confidence, ref marker_distance);

                        if (index==trackID)
                        {
                            break;
                        }
                    }
#endif
                    if (state!=0)
                    {
                        if (xr != null)
                        {
                            postrackingfix = xr.CameraFloorOffsetObject.transform.TransformPoint(new Vector3(posX, posY, posZ));
                            gameObject.transform.position = postrackingfix+ posOffset;
                            gameObject.transform.rotation = xr.CameraFloorOffsetObject.transform.rotation * new Quaternion(rotX, rotY, rotZ, rotW) * rotOffset;
                        }
                        else
                        {
                            gameObject.transform.position = Camera.main.transform.parent.transform.TransformPoint(new Vector3(posX, posY, posZ));
                            gameObject.transform.rotation = Camera.main.transform.parent.transform.rotation * new Quaternion(rotX, rotY, rotZ, rotW);
                        }
                        isTracked = true;
                        RefreshCurrentDifference();

                    }
                }
                else
                {
                    if (trackingIsLost == LostState.FollowHead&& isTracked)
                    {
                        FollowHead();
                    }
                }
            }
            else
            {
                trackingstate = false;
            }
        }
        private void OnRenderObject()
        {
            if (m_debugView&& trackingstate)
            {
                DrawDebugView(transform.position, transform.rotation, m_size);
            }
        }
        /// <summary>
        /// Plot the axes
        /// </summary>
        /// <param name="size"></param>
        private void DrawDebugView(Vector3 viewPos,Quaternion viewRot,float size)
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

        private void RefreshCurrentDifference()
        {
            posDifference = xr.Camera.transform.InverseTransformDirection(transform.position - xr.Camera.transform.position);
            rotDifference = Quaternion.Inverse(xr.Camera.transform.rotation) * transform.rotation;
        }

        private void FollowHead()
        {
            transform.position = xr.Camera.transform.position + xr.Camera.transform.TransformDirection(posDifference);
            transform.rotation = xr.Camera.transform.rotation * rotDifference;
        }


        #endregion
    }
}

