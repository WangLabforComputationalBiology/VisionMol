using UnityEngine;
public class DeviceParam
{

    public struct Vector4Int
    {
        [SerializeField]
        public int x, y, z, w;

        public Vector4Int(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }



    /// <summary>
    /// The index to retrieve the raw positional float[] array from VPU.
    /// [1,0,2] : for vertical VPU.
    /// [0,1,2] : for horizontal VPU.
    /// </summary>
    public static Vector3Int RawPositionIndex = new Vector3Int(1, 0, 2);

    public static Vector4Int RawRotationIndex = new Vector4Int(0, 1, 2, 3);

    public static Vector3  MarkerPosePreTiltEuler = new Vector3(-180, 0, 90);

}
