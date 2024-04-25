using System;
using System.Collections.Generic;
using UnityEngine;
namespace TouchlessA3D {
  public enum SkeletonPointsID {
    THUMB1 = 0,
    THUMB2 = 1,
    THUMB3 = 2,
    THUMB4 = 3,
    INDEX1 = 4,
    INDEX2 = 5,
    INDEX3 = 6,
    INDEX4 = 7,
    MIDDLE1 = 8,
    MIDDLE2 = 9,
    MIDDLE3 = 10,
    MIDDLE4 = 11,
    RING1 = 12,
    RING2 = 13,
    RING3 = 14,
    RING4 = 15,
    PINKY1 = 16,
    PINKY2 = 17,
    PINKY3 = 18,
    PINKY4 = 19,
    WRIST = 20,
    SKELETON_POINTS_END = 21,
  }
  public static class SkeletonInfo {
    public const int POINTS_END = 21;
    public const int BONES_END = 21;
  }
  public class Skeleton {
    public readonly Dictionary<SkeletonPointsID, Vector3> points;
    public Skeleton(NativeCalls.ta3d_point_3_float_t[] listOfPoints) {
      points = new Dictionary<SkeletonPointsID, Vector3>();
      for (int i = 0; i < (int)SkeletonPointsID.SKELETON_POINTS_END; i++) {
        points.Add((SkeletonPointsID)i, new Vector3(listOfPoints[i].coordinates[0], -listOfPoints[i].coordinates[1],
          listOfPoints[i].coordinates[2]));
      }
    }
    public Vector3 Position(SkeletonPointsID id, Transform camera) {
      return camera.TransformPoint(points[id]);
    }
    public Vector3 Position(int index, Transform camera) {
      return Position((SkeletonPointsID)index, camera);
    }
  }
}