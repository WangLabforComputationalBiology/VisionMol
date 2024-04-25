using System;
namespace TouchlessA3D {
  public enum FrameRotation {
    /// No rotation.
    ROTATION_NONE = 0,
    /// Rotated 180 degrees.
    ROTATION_180 = 180
  }

  public interface IFrame : System.IDisposable {
    IntPtr getNativeFrame ();
  }

  public struct Frame : IFrame {
    IntPtr m_nativeFrame;
    public Frame (IntPtr src_y, int src_stride_y,
      IntPtr src_u, int src_stride_u,
      IntPtr src_v, int src_stride_v,
      int pixel_stride_uv, int width, int height,
      long timestamp_ms, FrameRotation rotation) {

      m_nativeFrame = NativeCalls.ta3d_frame_create_from_android_420 (
        src_y, src_stride_y, src_u, src_stride_u,
        src_v, src_stride_v, pixel_stride_uv, width, height,
        timestamp_ms, rotation);
    }

    public Frame (
      IntPtr src_rgba, int src_stride,
      int width, int height,
      long timestamp_ms, FrameRotation rotation,
      bool flip_vertically) {
      m_nativeFrame = NativeCalls.ta3d_frame_create_from_rgba (
        src_rgba, src_stride, width, height,
        timestamp_ms, rotation, flip_vertically);
    }

    public IntPtr getNativeFrame () {
      return m_nativeFrame;
    }

    public void Dispose () {
      NativeCalls.ta3d_frame_destroy (m_nativeFrame);
    }
  }
}