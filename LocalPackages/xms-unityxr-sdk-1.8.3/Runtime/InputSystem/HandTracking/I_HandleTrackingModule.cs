using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.XR.InputSystems
{
    public struct InitializeHandTrackingModuleParameter
    {
        public Transform TrackingAnchor;

        public bool smoothHandRotation;

        /// <summary>
        /// Smooth hand rotation angle diff, min - max.
        /// </summary>
        public Vector2 smoothAngleRange;

        /// <summary>
        /// Angular speed when smoothing hand rotation.
        /// </summary>
        public float smoothHandRotationAngularSpeed;

        /// <summary>
        /// Smooth hand control curve.
        /// </summary>
        public AnimationCurve smoothControlCurve;
    }

    /// <summary>
    /// Handle tracking module interface.
    /// </summary>
    public interface I_HandleTrackingModule
    {
        /// <summary>
        /// Enables hand tracking module
        /// </summary>
        bool EnableModule(InitializeHandTrackingModuleParameter initParameter);

        /// <summary>
        /// Disable hand tracking module.
        /// </summary>
        void DisableModule();

        /// <summary>
        /// Gets the left hand track info
        /// </summary>
        HandTrackingInfo LeftHandTrackingInfo
        {
            get;
        }


        /// <summary>
        /// Gets the right hand track info
        /// </summary>
        HandTrackingInfo RightHandTrackingInfo
        {
            get;
        }


        /// <summary>
        /// Gets the previous frame left hand track info
        /// </summary>
        HandTrackingInfo PrevLeftHandTrackingInfo
        {
            get;
        }


        /// <summary>
        /// Gets the previous frame right hand track info
        /// </summary>
        HandTrackingInfo PrevRightHandTrackingInfo
        {
            get;
        }

        /// <summary>
        /// Call per frame to tick the hande tracking module.
        /// </summary>
        void Tick();

        /// <summary>
        /// is the module currently enabled 
        /// </summary>
        bool IsModuleEnabled
        {
            get;
        }
    }
}