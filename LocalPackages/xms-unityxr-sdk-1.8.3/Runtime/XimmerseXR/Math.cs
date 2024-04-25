using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SXR;

namespace Ximmerse.XR.Internal
{
    /// <summary>
    /// Polyengine mathf methods library.
    /// </summary>
    internal static class Math
    {
        /// <summary>
        /// Approximately the specified a and b.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        public static bool Approximately(Vector4 a, Vector4 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z) && Mathf.Approximately(a.w, b.w);
        }

        /// <summary>
        /// Approximately the specified a and b.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        public static bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }

        /// <summary>
        /// Approximately the specified a and b.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        public static bool Approximately(Vector2 a, Vector2 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
        }

        /// <summary>
        /// Makes the angle a pretty value between [-180 ... 180]
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="angle">Angle.</param>
        public static float PrettyAngle(this float angle)
        {
            if (angle >= -180 && angle <= 180)
            {
                return angle;
            }
            if (angle <= -180)
            {
                float mod = Mathf.Repeat(-angle, 360);
                if (mod >= 180)
                {
                    return -360 + mod;
                }
                else
                {
                    return -mod;
                }
            }
            else if (angle > 180 && angle <= 360)
            {
                return angle - 360;
            }
            else
            {
                float mod = Mathf.Repeat(angle, 360);
                if (mod >= 180)
                {
                    return -360 + mod;
                }
                else
                {
                    return mod;
                }
            }
        }


        /// <summary>
        /// Makes the euler a pretty value between [-180 ... 180]
        /// </summary>
        /// <returns>The euler.</returns>
        /// <param name="euler">Euler.</param>
        public static Vector3 PrettyAngle(this Vector3 euler)
        {
            return new Vector3(PrettyAngle(euler.x),
                PrettyAngle(euler.y),
                PrettyAngle(euler.z));
        }

        /// <summary>
        /// Makes the euler a pretty value between [-180 ... 180]
        /// </summary>
        /// <returns>The euler.</returns>
        public static Vector3 PrettyAngle(this Quaternion q)
        {
            return q.eulerAngles.PrettyAngle();
        }

        /// <summary>
        /// 把 rotation 的X和Z轴旋转角度设置为0。
        /// Flatten 之后的方向只有水平旋转角度。
        /// </summary>
        /// <param name="rotation">Rotation.</param>
        public static void FlattenXZ(ref Quaternion rotation)
        {
            var euler = rotation.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            rotation.eulerAngles = euler;
        }

        /// <summary>
        /// 把 rotation 的X和Z轴旋转角度设置为0。
        /// Flatten 之后的方向只有水平旋转角度。
        /// </summary>
        /// <param name="rotation">Rotation.</param>
        public static void FlattenXZ(this Quaternion rotation)
        {
            FlattenXZ(ref rotation);
        }

        /// <summary>
        /// Get the XZ d of dir1 and dir2 (on XZ surface)
        /// </summary>
        /// <returns>The X.</returns>
        /// <param name="dir1">Dir1.</param>
        /// <param name="dir2">Dir2.</param>
        public static float DistanceXZ(this Vector3 pos1, Vector3 pos2)
        {
            Vector3 newPos2 = new Vector3(pos2.x, pos1.y, pos2.z);
            return Vector3.Distance(pos1, newPos2);
        }


        /// <summary>
        /// Get the XZ DOT of dir1 and dir2 (on XZ surface)
        /// </summary>
        /// <returns>The X.</returns>
        /// <param name="dir1">Dir1.</param>
        /// <param name="dir2">Dir2.</param>
        public static float DotXZ(this Vector3 dir1, Vector3 dir2)
        {
            Vector3 newDir1 = new Vector3(dir1.x, dir2.y, dir1.z);
            return Vector3.Dot(newDir1, dir2);
        }


        /// <summary>
        /// Get the XZ signed angle of dir1 and dir2 (on XZ surface)
        /// </summary>
        /// <returns>The X.</returns>
        /// <param name="dir1">Dir1.</param>
        /// <param name="dir2">Dir2.</param>
        public static float SignedAngleXZ(this Vector3 dir1, Vector3 dir2)
        {
            Vector2 newDir1 = new Vector2(dir1.x, dir1.z);
            Vector2 newDir2 = new Vector2(dir2.x, dir2.z);
            return SignedAngle(newDir1, newDir2);
        }

        /// <summary>
        /// Steps the input, return a float that is multiple step to stepValue, and not bigger than input.
        /// For example, input value = 0.7, step = 0.5, return = 0.5. Input vlaue = 1.2. step = 0.5, return = 1
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="step">Step.</param>
        public static float FloorStep(float input, float step)
        {
            int stepCount = Mathf.FloorToInt(input / step);
            return stepCount * step;
        }

        /// <summary>
        /// Steps the input, return a float that is multiple step to stepValue, and not smaller than input.
        /// For example, input value = 0.7, step = 0.5, return = 1. Input value = 1.2, step = 0.5, return = 1.5
        /// </summary>
        /// <returns>The step.</returns>
        /// <param name="input">Input.</param>
        /// <param name="step">Step.</param>
        public static float CeilStep(float input, float step)
        {
            int stepCount = Mathf.CeilToInt(input / step);
            return stepCount * step;
        }

        /// <summary>
        /// Get the XZ angle of dir1 and dir2 (on XZ surface)
        /// </summary>
        /// <returns>The X.</returns>
        /// <param name="dir1">Dir1.</param>
        /// <param name="dir2">Dir2.</param>
        public static float AngleXZ(this Vector3 dir1, Vector3 dir2)
        {
            Vector2 newDir1 = new Vector3(dir1.x, dir1.z);
            Vector2 newDir2 = new Vector3(dir2.x, dir2.z);
            return Vector2.Angle(newDir1, newDir2);
        }

        /// <summary>
        /// Rounds the float.
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="f">F.</param>
        /// <param name="digit">Digit.</param>
        public static float RoundSingle(this float f, int digit = 2)
        {
            if (digit <= 0)
            {
                throw new System.Exception("Digit must GE 0");
            }
            double d = (double)f;
            d = System.Math.Round(d, digit);
            float round = (float)d;
            return round;
        }


        /// <summary>
        /// Rounds the vector2.
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="f">F.</param>
        /// <param name="digit">Digit.</param>
        public static Vector2 RoundVector2(this Vector2 vector2, int digit = 2)
        {
            if (digit <= 0)
            {
                throw new System.Exception("Digit must GE 0");
            }
            float roundX = vector2.x.RoundSingle(digit);
            float roundY = vector2.y.RoundSingle(digit);
            return new Vector2(roundX, roundY);
        }


        /// <summary>
        /// Rounds the vector3.
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="f">F.</param>
        /// <param name="digit">Digit.</param>
        public static Vector3 RoundVector3(this Vector3 vector3, int digit = 2)
        {
            if (digit <= 0)
            {
                throw new System.Exception("Digit must GE 0");
            }
            float roundX = vector3.x.RoundSingle(digit);
            float roundY = vector3.y.RoundSingle(digit);
            float roundZ = vector3.z.RoundSingle(digit);
            return new Vector3(roundX, roundY, roundZ);
        }



        /// <summary>
        /// Rounds the vector4.
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="f">F.</param>
        /// <param name="digit">Digit.</param>
        public static Vector4 RoundVector4(this Vector4 vector4, int digit = 2)
        {
            if (digit <= 0)
            {
                throw new System.Exception("Digit must GE 0");
            }
            float roundX = vector4.x.RoundSingle(digit);
            float roundY = vector4.y.RoundSingle(digit);
            float roundZ = vector4.z.RoundSingle(digit);
            float roundW = vector4.w.RoundSingle(digit);
            return new Vector4(roundX, roundY, roundZ, roundW);
        }


        /// <summary>
        /// Gets the signed angle of baseDir and dir2
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="baseDir">Base dir.</param>
        /// <param name="dir2">Dir2.</param>
        public static float SignedAngle(Vector3 baseDir, Vector3 dir2)
        {
            var angle = Vector3.Angle(baseDir, dir2);
            return angle * Mathf.Sign(Vector3.Cross(baseDir, dir2).y);
        }

        public static float SignedAngle(Quaternion qBase, Quaternion qDir)
        {
            Vector3 v1 = qBase * Vector3.forward;
            Vector3 v2 = qDir * Vector3.forward;
            return SignedAngle(v1, v2);
        }

        /// <summary>
        /// 返回 dir2 到 baseDir 的带符号角度。
        /// 如果在dir2在baseDir右边，返回1.
        /// 否则返回-1.
        /// 如果方向相同，返回0
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="baseDir">Base dir.</param>
        /// <param name="dir2">Dir2.</param>
        public static float SignedAngle(Vector2 baseDir, Vector2 dir2)
        {
            Vector3 vB = new Vector3(baseDir.x, 0, baseDir.y);
            Vector3 v2 = new Vector3(dir2.x, 0, dir2.y);
            var angle = Vector3.Angle(vB, v2);
            return angle * Mathf.Sign(Vector3.Cross(vB, v2).y);
        }

        /// <summary>
        /// 计算两个float 的绝对值距离
        /// </summary>
        /// <returns>The diff.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        public static float AbsDiff(float a, float b)
        {
            if (a == b || Mathf.Approximately(a, b))
            {
                return 0;
            }
            if (Mathf.Sign(a) == Mathf.Sign(b))
            {
                var a1 = Mathf.Abs(a);
                var a2 = Mathf.Abs(b);
                return Mathf.Abs(a1 - a2);
            }
            else
            {
                var bigger = a > b ? a : b;
                var smaller = a > b ? b : a;
                return bigger - smaller;
            }
        }

        /// <summary>
        /// Minimum of two vectors
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        public static Vector3 Min(Vector3 v1, Vector3 v2)
        {
            Vector3 ret = new Vector3(Mathf.Min(v1.x, v2.x),
                Mathf.Min(v1.y, v2.y),
                Mathf.Min(v1.z, v2.z));
            return ret;
        }


        /// <summary>
        /// 把 vect.x,y,z 的值  Clamp 在 [0..1]
        /// </summary>
        /// <param name="vect">Vect.</param>
        public static Vector3 Clamp01(Vector3 vect)
        {
            return new Vector3(Mathf.Clamp01(vect.x),
                Mathf.Clamp01(vect.y),
                Mathf.Clamp01(vect.z));
        }

        /// <summary>
        /// Maximum of two vectors
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        public static Vector3 Max(Vector3 v1, Vector3 v2)
        {
            Vector3 ret = new Vector3(Mathf.Max(v1.x, v2.x),
                Mathf.Max(v1.y, v2.y),
                Mathf.Max(v1.z, v2.z));
            return ret;
        }

        /// <summary>
        /// 对 rotation 做Yaw(水平旋转 N个角度)
        /// </summary>
        /// <param name="rotation">Rotation.</param>
        public static Quaternion YawByAngle(this Quaternion rotation, float yaw)
        {
            rotation = rotation * Quaternion.Euler(0, yaw, 0);
            return rotation;
        }

        /// <summary>
        /// 对 rotation 做 Pitch (以X为轴旋转 N个角度)
        /// </summary>
        /// <param name="rotation">Rotation.</param>
        public static Quaternion PitchByAngle(this Quaternion rotation, float pitch)
        {
            rotation = rotation * Quaternion.Euler(pitch, 0, 0);
            return rotation;
        }

        /// <summary>
        /// Rolls by angle.
        /// </summary>
        /// <returns>The by angle.</returns>
        /// <param name="rotation">Rotation.</param>
        /// <param name="roll">Roll.</param>
        public static Quaternion RollByAngle(this Quaternion rotation, float roll)
        {
            rotation = rotation * Quaternion.Euler(0, 0, roll);
            return rotation;
        }

        /// <summary>
        /// Clamps the vector2.
        /// </summary>
        /// <returns>The vector2.</returns>
        /// <param name="v2">V2.</param>
        /// <param name="minV2">Minimum v2.</param>
        /// <param name="maxV2">Max v2.</param>
        public static Vector2 ClampVector2(this Vector2 v2, Vector2 minV2, Vector2 maxV2)
        {
            v2.x = Mathf.Clamp(v2.x, minV2.x, maxV2.x);
            v2.y = Mathf.Clamp(v2.y, minV2.y, maxV2.y);
            return v2;
        }

        /// <summary>
        /// 给出原数值 single, 和目标值 target, 速度 speed, 令 single 以speed的速度逼近 target。
        /// </summary>
        /// <param name="single">Single.</param>
        /// <param name="target">Target.</param>
        /// <param name="speed">Speed.</param>
        public static float Approach(this float single, float target, float speed, float deltaTime)
        {
            if (single == target || Mathf.Approximately(single, target))
            {
                return target;
            }

            int dir = target > single ? 1 : -1;//1:正向逼近, -1:负向逼近
            float velocity = speed * dir * deltaTime;
            single += velocity;
            if (single == target || Mathf.Approximately(single, target) ||
                (dir == 1 && single > target)//正向超越
                || (dir == -1 && single < target)) //负向超越
            {
                return target;
            }
            else
            {
                return single;
            }
        }

        /// <summary>
        /// 转换整形数字为小数点后的浮点值： 12345 --> 0.12345
        /// </summary>
        /// <returns>The fractional float.</returns>
        /// <param name="Int">Int.</param>
        public static float Int2FractionalFloat(this int Int)
        {
            if (Int == 0)
                return 0;

            var absInt = Mathf.Abs(Int);
            int seed = 1;
            float sign = Mathf.Sign(Int);
            for (int i = 1; seed <= int.MaxValue; i++)
            {
                seed *= 10;
                if (absInt < seed)
                {
                    float ret = (((float)absInt) / ((float)seed)) * sign;
                    return ret;
                }
            }
            //畸大数: 溢出
            return -1;
        }

        /// <summary>
        /// Calculate three plane's intersection position.
        /// </summary>
        /// <returns>The plane intersection.</returns>
        /// <param name="p1">P1.</param>
        /// <param name="p2">P2.</param>
        /// <param name="p3">P3.</param>
        public static Vector3 ThreePlaneIntersection(Plane p1, Plane p2, Plane p3)
        {
            //get the intersection point of 3 planes
            return ((-p1.distance * Vector3.Cross(p2.normal, p3.normal)) +
                (-p2.distance * Vector3.Cross(p3.normal, p1.normal)) +
                (-p3.distance * Vector3.Cross(p1.normal, p2.normal))) /
                (Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal)));
        }

        /// <summary>
        /// Calculate two line segment's intersection point.
        /// Do not calculate the intersection point, faster than another version. 
        /// </summary>
        /// <returns><c>true</c>, if intersection was lined, <c>false</c> otherwise.</returns>
        /// <param name="p1">P1 - Line 1 start point</param>
        /// <param name="p2">P2 - Line 1 end point</param>
        /// <param name="p3">P3 - Line 2 start point</param>
        /// <param name="p4">P4 - Line 2 end point</param>
        /// <param name="intersection">Intersection.</param>
        public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float Ax, Bx, Cx, Ay, By, Cy, d, e, f/*, num,offset*/;
            float x1lo, x1hi, y1lo, y1hi;
            Ax = p2.x - p1.x;
            Bx = p3.x - p4.x;
            // X bound box test/
            if (Ax < 0)
            {
                x1lo = p2.x;
                x1hi = p1.x;
            }
            else
            {
                x1hi = p2.x;
                x1lo = p1.x;
            }

            if (Bx > 0)
            {
                if (x1hi < p4.x || p3.x < x1lo)
                    return false;
            }
            else
            {

                if (x1hi < p3.x || p4.x < x1lo)
                    return false;
            }
            Ay = p2.y - p1.y;
            By = p3.y - p4.y;
            // Y bound box test//
            if (Ay < 0)
            {
                y1lo = p2.y;
                y1hi = p1.y;
            }
            else
            {
                y1hi = p2.y;
                y1lo = p1.y;
            }
            if (By > 0)
            {
                if (y1hi < p4.y || p3.y < y1lo)
                    return false;
            }
            else
            {
                if (y1hi < p3.y || p4.y < y1lo)
                    return false;
            }
            Cx = p1.x - p3.x;
            Cy = p1.y - p3.y;
            d = By * Cx - Bx * Cy;  // alpha numerator//
            f = Ay * Bx - Ax * By;  // both denominator//
            // alpha tests//
            if (f > 0)
            {
                if (d < 0 || d > f)
                    return false;
            }
            else
            {
                if (d > 0 || d < f)
                    return false;
            }
            e = Ax * Cy - Ay * Cx;  // beta numerator//
            // beta tests //
            if (f > 0)
            {
                if (e < 0 || e > f)
                    return false;
            }
            else
            {
                if (e > 0 || e < f)
                    return false;
            }
            // check if they are parallel
            if (f == 0)
                return false;

            return true;
        }
        /// <summary>
        /// Calculate two line segment's intersection point.
        /// </summary>
        /// <returns><c>true</c>, if intersection was lined, <c>false</c> otherwise.</returns>
        /// <param name="p1">P1 - Line 1 start point</param>
        /// <param name="p2">P2 - Line 1 end point</param>
        /// <param name="p3">P3 - Line 2 start point</param>
        /// <param name="p4">P4 - Line 2 end point</param>
        /// <param name="intersection">Intersection.</param>
        public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
        {
            float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;
            float x1lo, x1hi, y1lo, y1hi;
            Ax = p2.x - p1.x;
            Bx = p3.x - p4.x;
            // X bound box test/
            if (Ax < 0)
            {
                x1lo = p2.x;
                x1hi = p1.x;
            }
            else
            {
                x1hi = p2.x;
                x1lo = p1.x;
            }

            if (Bx > 0)
            {
                if (x1hi < p4.x || p3.x < x1lo)
                    return false;
            }
            else
            {

                if (x1hi < p3.x || p4.x < x1lo)
                    return false;
            }
            Ay = p2.y - p1.y;
            By = p3.y - p4.y;
            // Y bound box test//
            if (Ay < 0)
            {
                y1lo = p2.y;
                y1hi = p1.y;
            }
            else
            {
                y1hi = p2.y;
                y1lo = p1.y;
            }
            if (By > 0)
            {
                if (y1hi < p4.y || p3.y < y1lo)
                    return false;
            }
            else
            {
                if (y1hi < p3.y || p4.y < y1lo)
                    return false;
            }
            Cx = p1.x - p3.x;
            Cy = p1.y - p3.y;
            d = By * Cx - Bx * Cy;  // alpha numerator//
            f = Ay * Bx - Ax * By;  // both denominator//
            // alpha tests//
            if (f > 0)
            {
                if (d < 0 || d > f)
                    return false;
            }
            else
            {
                if (d > 0 || d < f)
                    return false;
            }
            e = Ax * Cy - Ay * Cx;  // beta numerator//
            // beta tests //
            if (f > 0)
            {
                if (e < 0 || e > f)
                    return false;
            }
            else
            {
                if (e > 0 || e < f)
                    return false;
            }
            // check if they are parallel
            if (f == 0)
                return false;

            // compute intersection coordinates //
            num = d * Ax; // numerator //
            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //
            //    intersection.x = p1.x + (num+offset) / f;
            intersection.x = p1.x + num / f;
            num = d * Ay;
            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;
            //    intersection.y = p1.y + (num+offset) / f;
            intersection.y = p1.y + num / f;
            return true;
        }


        /// <summary>
        /// Remove roll from euler angle.
        /// </summary>
        /// <returns>The roll.</returns>
        /// <param name="quaternion">Quaternion.</param>
        public static Quaternion PitchNYaw(this Quaternion quaternion)
        {
            Vector3 euler = quaternion.eulerAngles;
            return Quaternion.Euler(euler.x, euler.y, 0);
        }

        /// <summary>
        /// Remove yaw and roll from euler angle.
        /// </summary>
        /// <returns>The roll.</returns>
        /// <param name="quaternion">Quaternion.</param>
        public static Quaternion Pitch(this Quaternion quaternion)
        {
            Vector3 euler = quaternion.eulerAngles;
            return Quaternion.Euler(euler.x, 0, 0);
        }

        /// <summary>
        /// Remove pitch and roll from euler angle.
        /// </summary>
        /// <returns>The roll.</returns>
        /// <param name="quaternion">Quaternion.</param>
        public static Quaternion Yaw(this Quaternion quaternion)
        {
            Vector3 euler = quaternion.eulerAngles;
            return Quaternion.Euler(0, PrettyAngle(euler.y), 0);
        }

        /// <summary>
        /// Remove pitch and yaw from euler angle.
        /// </summary>
        /// <returns>The roll.</returns>
        /// <param name="quaternion">Quaternion.</param>
        public static Quaternion Roll(this Quaternion quaternion)
        {
            Vector3 euler = quaternion.eulerAngles;
            return Quaternion.Euler(0, 0, euler.z);
        }

        /// <summary>
        /// Calculate quaternion diff = lhs - rhs
        /// </summary>
        /// <returns>The iff.</returns>
        /// <param name="lhs">Lhs.</param>
        /// <param name="rhs">Rhs.</param>
        public static Quaternion QDiff(this Quaternion lhs, Quaternion rhs)
        {
            return Quaternion.Inverse(rhs) * lhs;
        }

        /// <summary>
        /// Calculate quaternion sum : lhs + rhs[0] + rhs[1] + rhs[2] ...
        /// </summary>
        /// <returns>The plus.</returns>
        /// <param name="lhs">Lhs.</param>
        /// <param name="rhs">Rhs.</param>
        public static Quaternion QSum(this Quaternion lhs, params Quaternion[] rhs)
        {
            Quaternion qSum = lhs;
            for (int i = 0; i < rhs.Length; i++)
            {
                qSum = qSum * rhs[i];
            }
            return qSum;
        }

        /// <summary>
        /// yaw angle diff : lhs.yaw - rhs.yaw
        /// </summary>
        /// <returns>The diff.</returns>
        /// <param name="lhs">lhs.</param>
        /// <param name="rhs">rhs.</param>
        public static float YawDiff(this Quaternion lhs, Quaternion rhs)
        {
            Quaternion yawlhs = lhs.Yaw();
            Quaternion yawrhs = rhs.Yaw();
            var qdiff = yawlhs.QDiff(yawrhs);
            return PrettyAngle(qdiff.eulerAngles.y);
        }


        /// <summary>
        /// 输出内部归一化参数配置的双目 View Frustum
        /// </summary>
        /// <param name="near"></param>
        /// <param name="far"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="hFov"></param>
        /// <param name="vFov"></param>
        internal static void GetViewFrustum(out float near, out float far,
            out float left, out float right, out float top, out float bottom, out float hFov, out float vFov)
        {
            near = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Near_FLOAT);
            far = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Far_FLOAT);
            left = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Left_FLOAT);
            right = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Right_FLOAT);
            top = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Top_FLOAT);
            bottom = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_Frustum_Bottom_FLOAT);
            hFov = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_EyeBufferFovX_FLOAT);
            vFov = ParamLoader.ParamLoaderGetFloat((int)ParamType.Render_EyeBufferFovY_FLOAT);
        }

        /// <summary>
        /// Gets view frustum from ParamLoader
        /// </summary>
        /// <returns></returns>
        internal static Matrix4x4 GetViewFrustum()
        {
            GetViewFrustum(out float near, out float far,
            out float left, out float right, out float top, out float bottom, out float hFov, out float vFov);
            return Perspective(left, right, bottom, top, near, far);
        }


        static internal Matrix4x4 Perspective(float left, float right, float bottom, float top, float near, float far)
        {
            float x = 2.0F * near / (right - left);
            float y = 2.0F * near / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = -(2.0F * far * near) / (far - near);
            float e = -1.0F;
            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x;
            m[0, 1] = 0;
            m[0, 2] = a;
            m[0, 3] = 0;
            m[1, 0] = 0;
            m[1, 1] = y;
            m[1, 2] = b;
            m[1, 3] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = c;
            m[2, 3] = d;
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = e;
            m[3, 3] = 0;
            return m;
        }
    }
}