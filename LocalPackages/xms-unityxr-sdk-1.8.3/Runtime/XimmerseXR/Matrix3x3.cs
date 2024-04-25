using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ximmerse.XR.Internal.Mathmatics
{

    /// <summary>
    /// A 3x3 matrix struct.
    /// Performative to represent quaternion.
    /// </summary>
    [System.Serializable]
    internal struct Matrix3x3
    {
        /// <summary>
        /// Gets a identity matrix.
        /// </summary>
        public static Matrix3x3 identity
        {
            get
            {
                return new Matrix3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
            }
        }

        /// <summary>
        /// Row 0 
        /// </summary>
        [SerializeField]
        float m_m00, m_m01, m_m02;

        /// <summary>
        /// Row 1
        /// </summary>
        [SerializeField]
        float m_m10, m_m11, m_m12;

        /// <summary>
        /// Row 2
        /// </summary>
        [SerializeField]
        float m_m20, m_m21, m_m22;

        /// <summary>
        /// Row 0, Col 0
        /// </summary>
        public float m00
        {
            get => m_m00;
        }

        /// <summary>
        /// Row 0, Col 1
        /// </summary>
        public float m01
        {
            get => m_m01;
        }

        /// <summary>
        /// Row 0, Col 2
        /// </summary>
        public float m02
        {
            get => m_m02;
        }

        /// <summary>
        /// Row 1, Col 0
        /// </summary>
        public float m10
        {
            get => m_m10;
        }

        /// <summary>
        /// Row 1, Col 1
        /// </summary>
        public float m11
        {
            get => m_m11;
        }

        /// <summary>
        /// Row 1, Col 2
        /// </summary>
        public float m12
        {
            get => m_m12;
        }


        /// <summary>
        /// Row 2, Col 0
        /// </summary>
        public float m20
        {
            get => m_m20;
        }

        /// <summary>
        /// Row 2, Col 1
        /// </summary>
        public float m21
        {
            get => m_m21;
        }

        /// <summary>
        /// Row 2, Col 2
        /// </summary>
        public float m22
        {
            get => m_m22;
        }

        /// <summary>
        /// Gets element by row/col
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public float this[int row, int col]
        {
            get
            {
                if (row == 0 && col == 0)
                {
                    return m_m00;
                }
                if (row == 0 && col == 1)
                {
                    return m_m01;
                }
                if (row == 0 && col == 2)
                {
                    return m_m02;
                }

                if (row == 1 && col == 0)
                {
                    return m_m10;
                }
                if (row == 1 && col == 1)
                {
                    return m_m11;
                }
                if (row == 1 && col == 2)
                {
                    return m_m12;
                }

                if (row == 2 && col == 0)
                {
                    return m_m20;
                }
                if (row == 2 && col == 1)
                {
                    return m_m21;
                }
                if (row == 2 && col == 2)
                {
                    return m_m22;
                }

                throw new UnityException(string.Format("Invalid format : {0},{1}", row, col));
            }
        }

        /// <summary>
        /// Ctor of matrix 3x3
        /// </summary>
        /// <param name="m00"></param>
        /// <param name="m01"></param>
        /// <param name="m02"></param>
        /// <param name="m10"></param>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m20"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        public Matrix3x3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
        {
            m_m00 = m00;
            m_m01 = m01;
            m_m02 = m02;
            m_m10 = m10;
            m_m11 = m11;
            m_m12 = m12;
            m_m20 = m20;
            m_m21 = m21;
            m_m22 = m22;
        }

        /// <summary>
        /// Ctor a matrix3x3 by 2 vector3 of row and col.
        /// </summary>
        /// <param name="col">A vector3 of represents column.</param>
        /// <param name="row">A vector3 of represents row.</param>
        public Matrix3x3(Vector3 col, Vector3 row)
        {
            m_m00 = col.x * row.x;
            m_m01 = col.x * row.y;
            m_m02 = col.x * row.z;

            m_m10 = col.y * row.x;
            m_m11 = col.y * row.y;
            m_m12 = col.y * row.z;

            m_m20 = col.z * row.x;
            m_m21 = col.z * row.y;
            m_m22 = col.z * row.z;
        }

        /// <summary>
        /// Ctor of matrix 3x3 by quaternion.
        /// </summary>
        /// <param name="q"></param>
        public Matrix3x3(Quaternion q)
        {
            m_m00 = 1.0f - 2.0f * (q.y * q.y + q.z * q.z);
            m_m10 = (q.x * q.y + q.z * q.w) * 2.0f;
            m_m20 = (q.x * q.z - q.y * q.w) * 2.0f;

            m_m01 = (q.x * q.y - q.z * q.w) * 2.0f;
            m_m11 = 1.0f - 2.0f * (q.x * q.x + q.z * q.z);
            m_m21 = (q.y * q.z + q.x * q.w) * 2.0f;

            m_m02 = (q.x * q.z + q.y * q.w) * 2.0f;
            m_m12 = (q.y * q.z - q.x * q.w) * 2.0f;
            m_m22 = 1.0f - 2.0f * (q.x * q.x + q.y * q.y);
        }

        /// <summary>
        /// Sets this matrix3x3 by the quaternion.
        /// </summary>
        /// <param name="quaternion"></param>
        public void SetRotation(Quaternion q)
        {
            m_m00 = 1.0f - 2.0f * (q.y * q.y + q.z * q.z);
            m_m10 = (q.x * q.y + q.z * q.w) * 2.0f;
            m_m20 = (q.x * q.z - q.y * q.w) * 2.0f;

            m_m01 = (q.x * q.y - q.z * q.w) * 2.0f;
            m_m11 = 1.0f - 2.0f * (q.x * q.x + q.z * q.z);
            m_m21 = (q.y * q.z + q.x * q.w) * 2.0f;

            m_m02 = (q.x * q.z + q.y * q.w) * 2.0f;
            m_m12 = (q.y * q.z - q.x * q.w) * 2.0f;
            m_m22 = 1.0f - 2.0f * (q.x * q.x + q.y * q.y);
        }

        /// <summary>
        /// Transpose this 3x3 matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix3x3 transpose
        {
            get => new Matrix3x3(m00, m10, m20, m01, m11, m21, m02, m12, m22);
        }

        /// <summary>
        /// Composite a rotation 3x3 matrix and a position to a matrix4x4 translation matrix.
        /// </summary>
        /// <param name="RotationMatrix"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Matrix4x4 ToTRS(Matrix3x3 RotationMatrix, Vector3 position)
        {
            Matrix4x4 trs = Matrix4x4.identity;
            trs.SetRow(0, new Vector4(RotationMatrix.m00, RotationMatrix.m01, RotationMatrix.m02, position.x));
            trs.SetRow(1, new Vector4(RotationMatrix.m10, RotationMatrix.m11, RotationMatrix.m12, position.y));
            trs.SetRow(2, new Vector4(RotationMatrix.m20, RotationMatrix.m21, RotationMatrix.m22, position.z));
            return trs;
        }

        /// <summary>
        /// Assumes the matrix3x3 represents a quaternion rotation.
        /// Thread safe function.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Quaternion GetRotation()
        {
            float fourXSquaredMinus1 = m_m00 - m_m11 - m_m22;
            float fourYSquaredMinus1 = m_m11 - m_m00 - m_m22;
            float fourZSquaredMinus1 = m_m22 - m_m00 - m_m11;
            float fourWSquaredMinus1 = m_m00 + m_m11 + m_m22;

            int biggestIndex = 0;
            float fourBiggestSquaredMinus1 = fourWSquaredMinus1;
            if (fourXSquaredMinus1 > fourBiggestSquaredMinus1)
            {
                fourBiggestSquaredMinus1 = fourXSquaredMinus1;
                biggestIndex = 1;
            }
            if (fourYSquaredMinus1 > fourBiggestSquaredMinus1)
            {
                fourBiggestSquaredMinus1 = fourYSquaredMinus1;
                biggestIndex = 2;
            }
            if (fourZSquaredMinus1 > fourBiggestSquaredMinus1)
            {
                fourBiggestSquaredMinus1 = fourZSquaredMinus1;
                biggestIndex = 3;
            }

            float biggestVal = Mathf.Sqrt(fourBiggestSquaredMinus1 + 1) * 0.5f;
            float mult = 0.25f / biggestVal;

            Quaternion q = Quaternion.identity;
            switch (biggestIndex)
            {
                case 0:
                    q.w = biggestVal;
                    q.x = (m_m12 - m_m21) * mult;
                    q.y = (m_m20 - m_m02) * mult;
                    q.z = (m_m01 - m_m10) * mult;
                    break;
                case 1:
                    q.w = (m_m12 - m_m21) * mult;
                    q.x = biggestVal;
                    q.y = (m_m01 + m_m10) * mult;
                    q.z = (m_m20 + m_m02) * mult;
                    break;
                case 2:
                    q.w = (m_m20 - m_m02) * mult;
                    q.x = (m_m01 + m_m10) * mult;
                    q.y = biggestVal;
                    q.z = (m_m12 + m_m21) * mult;
                    break;
                case 3:
                    q.w = (m_m01 - m_m10) * mult;
                    q.x = (m_m20 + m_m02) * mult;
                    q.y = (m_m12 + m_m21) * mult;
                    q.z = biggestVal;
                    break;

                default:                    // Silence a -Wswitch-default warning in GCC. Should never actually get here. Assert is just for sanity.
                    break;
            }
            float e = 1 / (q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            return new Quaternion(-q.x * e, -q.y * e, -q.z * e, q.w * e);
        }

        /// <summary>
        /// Gets row at the index of the matrix3x3
        /// </summary>
        /// <param name="RowIndex"></param>
        /// <returns></returns>
        public Vector3 GetRow(int RowIndex)
        {
            switch (RowIndex)
            {
                case 0:
                    return new Vector3(m00, m01, m02);

                case 1:
                    return new Vector3(m10, m11, m12);

                case 2:
                    return new Vector3(m20, m21, m22);

                default:
                    throw new UnityException(string.Format("Invalid index: {0}", RowIndex));
            }
        }

        /// <summary>
        /// Gets column at the index of the matrix3x3
        /// </summary>
        /// <param name="ColumnIndex"></param>
        /// <returns></returns>
        public Vector3 GetColumn(int ColumnIndex)
        {
            switch (ColumnIndex)
            {
                case 0:
                    return new Vector3(m00, m10, m20);

                case 1:
                    return new Vector3(m01, m11, m21);

                case 2:
                    return new Vector3(m02, m12, m22);

                default:
                    throw new UnityException(string.Format("Invalid index: {0}", ColumnIndex));
            }
        }

        public static Matrix3x3 operator *(float lhs, Matrix3x3 rhs)
        {
            return new Matrix3x3(lhs * rhs.m00, lhs * rhs.m01, lhs * rhs.m02,
                lhs * rhs.m10, lhs * rhs.m11, lhs * rhs.m12,
                lhs * rhs.m20, lhs * rhs.m21, lhs * rhs.m22);
        }

        public static Matrix3x3 operator *(Matrix3x3 lhs, float rhs)
        {
            return new Matrix3x3(rhs * lhs.m00, rhs * lhs.m01, rhs * lhs.m02,
                rhs * lhs.m10, rhs * lhs.m11, rhs * lhs.m12,
                rhs * lhs.m20, rhs * lhs.m21, rhs * lhs.m22);
        }

        /// <summary>
        /// Multiple two matrix3x3
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Matrix3x3 operator *(Matrix3x3 lhs, Matrix3x3 rhs)
        {
            Vector3 lRow0 = lhs.GetRow(0);
            Vector3 lRow1 = lhs.GetRow(1);
            Vector3 lRow2 = lhs.GetRow(2);

            Vector3 rRow0 = rhs.GetRow(0);
            Vector3 rRow1 = rhs.GetRow(1);
            Vector3 rRow2 = rhs.GetRow(2);

            Matrix3x3 result = new Matrix3x3(
                m00: lRow0.x * rRow0.x + lRow0.y * rRow1.x + lRow0.z * rRow2.x,
                m01: lRow0.x * rRow0.y + lRow0.y * rRow1.y + lRow0.z * rRow2.y,
                m02: lRow0.x * rRow0.z + lRow0.y * rRow1.z + lRow0.z * rRow2.z,

                m10: lRow1.x * rRow0.x + lRow1.y * rRow1.x + lRow1.z * rRow2.x,
                m11: lRow1.x * rRow0.y + lRow1.y * rRow1.y + lRow1.z * rRow2.y,
                m12: lRow1.x * rRow0.z + lRow1.y * rRow1.z + lRow1.z * rRow2.z,

                m20: lRow2.x * rRow0.x + lRow2.y * rRow1.x + lRow2.z * rRow2.x,
                m21: lRow2.x * rRow0.y + lRow2.y * rRow1.y + lRow2.z * rRow2.y,
                m22: lRow2.x * rRow0.z + lRow2.y * rRow1.z + lRow2.z * rRow2.z
                );

            return result;
        }

        /// <summary>
        /// Plus lhs to rhs
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Matrix3x3 operator +(Matrix3x3 lhs, Matrix3x3 rhs)
        {
            return new Matrix3x3(lhs.m00 + rhs.m00, lhs.m01 + rhs.m01, lhs.m02 + rhs.m02,
                lhs.m10 + rhs.m10, lhs.m11 + rhs.m11, lhs.m12 + rhs.m12,
                lhs.m20 + rhs.m20, lhs.m21 + rhs.m21, lhs.m22 + rhs.m22);
        }

        /// <summary>
        /// Minus lhs to rhs
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Matrix3x3 operator -(Matrix3x3 lhs, Matrix3x3 rhs)
        {
            return new Matrix3x3(lhs.m00 - rhs.m00, lhs.m01 - rhs.m01, lhs.m02 - rhs.m02,
                lhs.m10 - rhs.m10, lhs.m11 - rhs.m11, lhs.m12 - rhs.m12,
                lhs.m20 - rhs.m20, lhs.m21 - rhs.m21, lhs.m22 - rhs.m22);
        }

        /// <summary>
        /// Multiple lhs by rhs Vector3
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector3 operator *(Matrix3x3 lhs, Vector3 rhs)
        {
            Vector3 lCol0 = lhs.GetColumn(0);
            Vector3 lCol1 = lhs.GetColumn(1);
            Vector3 lCol2 = lhs.GetColumn(2);

            return lCol0 * rhs.x + lCol1 * rhs.y + lCol2 * rhs.z;
        }


        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]\r\n[{3}, {4}, {5}]\r\n[{6}, {7}, {8}]", m00, m01, m02,
                m10, m11, m12,
                m20, m21, m22);
        }

        public string ToString(string format)
        {
            return string.Format("[{0}, {1}, {2}]\r\n[{3}, {4}, {5}]\r\n[{6}, {7}, {8}]", m00.ToString(format), m01.ToString(format), m02.ToString(format),
    m10.ToString(format), m11.ToString(format), m12.ToString(format),
    m20.ToString(format), m21.ToString(format), m22.ToString(format));
        }
    }

}