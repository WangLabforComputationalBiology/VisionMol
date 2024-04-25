using System;
using System.Collections;
using System.Collections.Generic;
using Ximmerse.Wrapper.XDeviceService;
using System.Runtime.InteropServices;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ximmerse.Wrapper.XDeviceService.Client;
using static Ximmerse.XR.XDevicePlugin;

namespace Ximmerse.XR
{
    public class XRController3Dof : MonoBehaviour
    {
        #region Property
        enum ControllerIndex
        {
            Left = 0,
            Right = 1
        }
        [SerializeField]
        private ControllerIndex controller3Dof;
        private ClientController clientController;
        private XAttrControllerState _ControllerSate;
        private Quaternion imuQ;
        private Quaternion _recentRotation = Quaternion.identity;
        private Quaternion currentLerp = Quaternion.identity;
        bool isFirst=true;
        private int markerControllerIndex;
        public ClientController XRController
        {
            get => clientController;
        }
        public Quaternion RecentRotation
        {
            get => _recentRotation;
            set => _recentRotation = value;
        }
        #endregion

        #region Unity
        void Start()
        {
#if !UNITY_EDITOR
            XDeviceClientWrapper.Init();
            RefreshMarkerControllerIndex();
            XDeviceClientWrapper.onControllerConnectState += RefreshMarkerControllerIndex;
#endif
        }

        void Update()
        {
#if !UNITY_EDITOR
            UpdateController();
#endif
        }


        #endregion

        #region Method
        public void RefreshMarkerControllerIndex()
        {
            markerControllerIndex = -1;
            clientController = null;
            for (int i = 0; i < 6; i++)
            {
                if (controller3Dof ==ControllerIndex.Left&&(XControllerTypes)XDevicePlugin.xdevc_ctrl_get_device_type(XDevicePlugin.xdevc_get_controller(i)) == XControllerTypes.kControllerType_TagLeft)
                {
                    markerControllerIndex = i;
                    Debug.Log("QmQ  leftMarkerController Index:" + i);
                }

                if (controller3Dof == ControllerIndex.Right&&(XControllerTypes)XDevicePlugin.xdevc_ctrl_get_device_type(XDevicePlugin.xdevc_get_controller(i)) == XControllerTypes.kControllerType_TagRight)
                {
                    markerControllerIndex = i;
                    Debug.Log("QmQ  rightMarkerController Index:" + i);
                }
            }
            if (markerControllerIndex>=0)
            {
                clientController = XDeviceClientWrapper.ClientControllerDic[markerControllerIndex];
            }
        }
        private void UpdateController()
        {
            if (clientController != null)
            {
                _ControllerSate = clientController.GetControllerState();

                imuQ = Quaternion.Euler(new Vector3(-_ControllerSate.euler[0], -_ControllerSate.euler[1], _ControllerSate.euler[2]));

                if (isFirst)
                {
                    currentLerp = Quaternion.identity * Quaternion.Inverse(imuQ);
                    isFirst = false;
                }

                transform.rotation = imuQ * currentLerp;
            }
        }


        public void RecenterController()
        {
            if (clientController==null)
            {
                RefreshMarkerControllerIndex();
            }

            if (clientController!=null)
            {

                _ControllerSate = clientController.GetControllerState();

                imuQ = Quaternion.Euler(new Vector3(-_ControllerSate.euler[0], -_ControllerSate.euler[1], _ControllerSate.euler[2]));

                currentLerp = _recentRotation * Quaternion.Inverse(imuQ);

                transform.rotation = imuQ * currentLerp;
            }
        }
#endregion
    }
}

