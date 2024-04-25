
# Touchless XR Skeleton Unity Integration
Crunchfish is providing this SDK for touchless interaction in the XR setting.
The detection is specially adopted for AR/VR glasses where the camera is
mounted on a the device, facing away from the user. The product has support for
detection of right or left hand and will output the absolut position of 21
points on the hand in relation to the camera. The SDK is
provided with a sample application that can be used as reference for how to use
the API's.

***Important notice!***
*The sample applications and reference code is not an integral part of the
licensed software libraries for this SDK and is provided "AS IS" and "AS
AVAILABLE" with all faults and without warranty of any kind. Crunchfish is only
providing limited documentation and no support related to the reference code.
The reference code might change at any time.*

The status of the integration is at an experimental stage and the API may change in future versions.

# Installation Instructions
To use the touchless\_a3d\_xr in a Unity project, after importing the touchlessa3d-unity-integration.unitypackage
the touchless\_a3d\_xr.aar from the Ta3dSDK should be put in a Plugins folder.
  Assets/touchlessa3d-unity-integration/Plugins/touchless\_a3d\_xr.aar

To get started there are simple sample scenes and prefabs at
  Assets/touchlessa3d-unity-integration/Setup

There is a dependency on android Min API Level 21 and this needs to be set in Unity "Player Settings > Other"

It is required to compile the apps for android ARM64 using the Unity IL2CPP backend which is also set in "Player Settings > Other"

The integration uses a Basic Camera integration with standard Unity components.
There are at the moment three parts to the calibration of the Touchless XR Skeleton Unity Integration
for a device camera to make the SkeletonPoints be positioned correctly in 3d space.

* The width and height of the image to operate on, defined in  UnityCameraIntegration.cs
* A Calibration class located in Calibration.cs and instantiated in UnityTouchlessSession.cs
* The FOV of The Main virtual Camera in unity should match the device camera.
