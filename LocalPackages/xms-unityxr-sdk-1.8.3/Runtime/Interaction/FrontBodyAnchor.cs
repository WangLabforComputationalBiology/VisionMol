using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace Ximmerse.XR.Interactions
{
    /// <summary>
    /// 身体前向锚点.
    /// 代表XR用户身体前面一定偏移量的坐标锚点。
    /// </summary>
    public class FrontBodyAnchor : MonoBehaviour
    {

#if UNITY_ANDROID || UNITY_EDITOR

        public static FrontBodyAnchor instance
        {
            get; private set;
        }

        Camera m_MainCamera;

        public Transform mainCameraT
        {
            get
            {
                if(!m_MainCamera)
                {
                    m_MainCamera = Camera.main;
                }

                return m_MainCamera.transform;
            }
        }

        [SerializeField]
        [Tooltip("Distance along Z-axis from head to dock position.")]
        float m_ZDepth = 4;

        /// <summary>
        /// Z-depth : canvas z-distance to head.
        /// </summary>
        /// <value>The z depth.</value>
        public float Z_Depth
        {
            get
            {
                return m_ZDepth;
            }
            set
            {
                m_ZDepth = value;
            }
        }

        [SerializeField, Tooltip("Force docking every frame")]
        bool m_ForceEveryFrame;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Ximmerse.RhinoX.FrontDocker"/> force every frame.
        /// </summary>
        /// <value><c>true</c> if force every frame; otherwise, <c>false</c>.</value>
        public bool ForceEveryFrame
        {
            get
            {
                return m_ForceEveryFrame;
            }
            set
            {
                m_ForceEveryFrame = value;
            }
        }

        [SerializeField, Tooltip("Angular diff error to re-dock canvas, smaller values cause frequent re-docking.")]
        float m_AngleDiffError = 35;

        /// <summary>
        /// Angular diff error to re-dock canvas, smaller values cause frequent re-docking.
        /// </summary>
        /// <value>The diff error.</value>
        public float AngleDiffError
        {
            get
            {
                return m_AngleDiffError;
            }
            set
            {
                m_AngleDiffError = value;
            }
        }


        [SerializeField, Tooltip("Delay time to start docking.")]
        float m_DockDelay = 0.5f;

        /// <summary>
        /// Gets or sets the dock delay.
        /// </summary>
        /// <value>The dock delay.</value>
        public float DockDelay
        {
            get
            {
                return m_DockDelay;
            }
            set
            {
                m_DockDelay = value;
            }
        }

        [SerializeField, Min(0.1f)]
        [Tooltip("The smaller damp time is , the faster the transform dock to eye's front.")]
        float m_DampTime = 0.3f;

        /// <summary>
        /// Gets or sets the damp time. The smaller damp time is , the faster the transform dock to eye's front.
        /// </summary>
        /// <value>The damp time.</value>
        public float DampTime
        {
            get
            {
                return m_DampTime;
            }
            set
            {
                m_DampTime = value;
            }
        }

        [SerializeField]
        //[Tooltip("The smaller damp time is , the faster the transform dock to eye's front.")]
        bool m_FollowWithPitch;

        /// <summary>
        /// Gets or sets the damp time. The smaller damp time is , the faster the transform dock to eye's front.
        /// </summary>
        /// <value>The damp time.</value>
        public bool FollowWithPitch
        {
            get
            {
                return m_FollowWithPitch;
            }
            set
            {
                m_FollowWithPitch = value;
            }
        }

        /// <summary>
        /// offset of docker position.
        /// modify by leaf.2019.08.30
        /// </summary>
        public Vector3 PositionOffset;

        bool m_Docking = false;

        float dampSpeed = 0;

        float dockEulerY;
        float dockEulerX;


        float? m_DockingStartTime;


        Transform m_HeadTransform;

        Transform headTransform
        {
            get
            {
                if (m_HeadTransform == null)
                {
                    m_HeadTransform = Camera.main.transform;
                }
                return m_HeadTransform;
            }
        }

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            while (!mainCameraT)
            {
                yield return null;
            }
            //calculate dock position and rotation:
            Quaternion fwdQ = Quaternion.Euler(0, headTransform.eulerAngles.y, 0);
            transform.position = headTransform.position + fwdQ * Vector3.forward * m_ZDepth;
            transform.rotation = fwdQ;
            m_Docking = false;
            dockEulerY = headTransform.eulerAngles.y;
            dockEulerX = headTransform.eulerAngles.x;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            var headT = headTransform;
            float headEulerY = headT.eulerAngles.y;
            float headEulerX = headT.eulerAngles.x;
            float eulerYDiff = Quaternion.Angle(Quaternion.Euler(0, headEulerY, 0), Quaternion.Euler(0, dockEulerY, 0));
            float eulerXDiff = Quaternion.Angle(Quaternion.Euler(headEulerX, 0, 0), Quaternion.Euler(dockEulerX, 0, 0));

            if (m_ForceEveryFrame || Mathf.Abs(eulerYDiff) >= m_AngleDiffError || (FollowWithPitch && Mathf.Abs(eulerXDiff) >= m_AngleDiffError))
            {
                m_Docking = true;
                if (m_DockingStartTime.HasValue == false)
                {
                    m_DockingStartTime = Time.time + m_DockDelay;
                }
            }


            if (m_Docking)
            {
                if (m_DockingStartTime.HasValue && Time.time < m_DockingStartTime.Value)
                {
                    //wait for delay time
                }
                else
                {
                    if (m_DampTime > 0)
                    {
                        dockEulerY = Mathf.SmoothDampAngle(dockEulerY, headEulerY, ref dampSpeed, m_DampTime);
                        float eulerYDiff2 = Quaternion.Angle(Quaternion.Euler(0, headEulerY, 0), Quaternion.Euler(0, dockEulerY, 0));
                        if (eulerYDiff2 <= 2.5f)
                        {
                            m_Docking = false;
                            m_DockingStartTime = null;//reset docking start time
                        }
                        if (FollowWithPitch)
                        {
                            dockEulerX = Mathf.SmoothDampAngle(dockEulerX, headEulerX, ref dampSpeed, m_DampTime);
                            float eulerXDiff2 = Quaternion.Angle(Quaternion.Euler(0, headEulerX, 0), Quaternion.Euler(0, dockEulerX, 0));
                            if (eulerXDiff2 <= 2.5f)
                            {
                                m_Docking = false;
                                m_DockingStartTime = null;//reset docking start time
                            }
                        }
                    }
                    else
                    {
                        dockEulerY = headEulerY;
                        if (FollowWithPitch)
                        {
                            dockEulerX = headEulerX;
                        }
                    }
                }

                if (FollowWithPitch)
                {
                    transform.rotation = Quaternion.Euler(dockEulerX, dockEulerY, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, dockEulerY, 0);
                }
            }

            //Update pos:
            // modify by leaf.2019.08.30
            if (FollowWithPitch)
            {
                transform.position = headTransform.position + Quaternion.Euler(dockEulerX, dockEulerY, 0) * Vector3.forward * m_ZDepth + PositionOffset;
            }
            else
            {
                transform.position = headTransform.position + Quaternion.Euler(0, dockEulerY, 0) * Vector3.forward * m_ZDepth + PositionOffset;
            }
        }
#endif

    }
}