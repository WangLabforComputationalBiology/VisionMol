using System;
using System.Runtime.InteropServices;
using UnityEngine;
namespace TouchlessA3D {
  using static NativeCalls;
  public enum GestureType {
    NO_HAND = 0,
    HAND = 1,
    OPEN_HAND = 2,
    CLOSED_HAND = 3,
    CLOSED_PINCH = 4,
    GESTURE_TYPE_END = 5
  }

  public enum HandednessType{
    HANDEDNESS_UNKNOWN = 0,
    LEFT_HAND = 1,
    RIGHT_HAND = 2
  }

  public class GestureEvent : EventArgs {
    public readonly GestureType type;
    public readonly bool skeletonValid;
    public readonly Skeleton skeleton;
    public readonly HandednessType handedness;
    public GestureEvent (IntPtr ta3d_event) {
      type = ta3d_event_get_type (ta3d_event); 
      handedness = ta3d_get_handedness(ta3d_event);
      IntPtr skeletonPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ta3d_skeleton_3d_s)));
      try{
        //ta3d_skeleton_points_3d(ta3d_event, skeletonPtr);
        ta3d_skeleton_3d_s skeletonPoints;
        skeletonPoints = (ta3d_skeleton_3d_s)Marshal.PtrToStructure(skeletonPtr,typeof(ta3d_skeleton_3d_s));
        skeletonValid = skeletonPoints.status == ResultType.RESULT_OK;
        if(!skeletonValid) {
          return;
        }
        skeleton = new Skeleton(skeletonPoints.points);
      } finally {
        Marshal.FreeHGlobal(skeletonPtr);
      }
    }

    public GestureEvent (GestureEvent toCopy) {
      type = toCopy.type;
      skeletonValid = toCopy.skeletonValid;
      skeleton = toCopy.skeleton;
      handedness = toCopy.handedness;
    }
  }
}