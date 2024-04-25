using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class UnityCameraIntegration : MonoBehaviour
{
    private bool camAvailable;
    [HideInInspector]
    public WebCamTexture cameraTexture;

    public RawImage background;
    public AspectRatioFitter fit;
    public readonly static int width = 352;
    public readonly static int height = 288;
    void Start()
    {
        SetupCamera();
    }

    private void SetupCamera()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
            return;

        for (int i = 0; i < devices.Length; i++)
        {
            var curr = devices[i];
            if (curr.isFrontFacing == true)
            { //RhinoX using front facing camera
                cameraTexture = new WebCamTexture(curr.name, width, height, 60);
                break;
            }
        }

        if (cameraTexture == null)
        {
            Debug.LogError("Failed to aquire Camera");
            Destroy(this);
            return;
        }

        cameraTexture.Play();
        //if (null == background)
        //	return;
        background.texture = cameraTexture;
        camAvailable = true;

        Debug.Log("RGB texture has been setup for T3D hand tracking");
    }

    //void Update () {
    //	if (!camAvailable) {
    //		SetupCamera ();
    //		return;
    //	}
    //	if (null == background || null == fit) {
    //		Destroy (this);
    //		return;
    //	}
    //	fit.aspectRatio = (float) cameraTexture.width / (float) cameraTexture.height;

    //	float scaleY = cameraTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
    //	background.rectTransform.localScale = new Vector3 (1f, scaleY, 1f); // Swap the preview of the mirrored camera

    //	int orient = -cameraTexture.videoRotationAngle;
    //	background.rectTransform.localEulerAngles = new Vector3 (0, 0, orient);
    //}
}