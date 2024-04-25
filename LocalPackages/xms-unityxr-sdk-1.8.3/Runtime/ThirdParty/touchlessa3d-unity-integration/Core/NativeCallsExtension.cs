using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;

namespace TouchlessA3D
{
    public static partial class NativeCalls
    {
        /// <summary>
        /// Get skeleton 3d data by the pointer. Unsafe but free of GC.
        /// </summary>
        /// <param name="skeleton3DPtr"></param>
        /// <param name="skeleton3d"></param>
        public static unsafe void GetSkeleton3DPose(IntPtr skeleton3DPtr, ref ta3d_skeleton_3d_s skeleton3d)
        {
            void* ptr = skeleton3DPtr.ToPointer();
            int* intPtr = (int*)ptr;

            int resultType_Int = intPtr[0];
            skeleton3d.status = GetResultType(resultType_Int);

            float* pointsPtr = &((float*)ptr)[1];
            //ta3d_point_3_float_t points[21]
            for (int i = 0, iMax = 21; i < iMax; i++)
            {
                //each ta3d_point_3_float_t has float[3]
                float* pointPtr = &pointsPtr[i * 3];

                skeleton3d.points[i].coordinates[0] = pointPtr[0];
                skeleton3d.points[i].coordinates[1] = pointPtr[1];
                skeleton3d.points[i].coordinates[2] = pointPtr[2];
            }

            //1 + 21 * 3 = 64
            float* rotationsPtr = &((float*)ptr)[64];
            //ta3d_matrix_3_3_float_t rotations[21]
            for (int i = 0, iMax = 21; i < iMax; i++)
            {
                //each ta3d_matrix_3_3_float_t has float[9]
                float* rotationPtr = &rotationsPtr[i * 9];

                skeleton3d.rotations[i].elements[0] = rotationPtr[0];
                skeleton3d.rotations[i].elements[1] = rotationPtr[1];
                skeleton3d.rotations[i].elements[2] = rotationPtr[2];
                skeleton3d.rotations[i].elements[3] = rotationPtr[3];
                skeleton3d.rotations[i].elements[4] = rotationPtr[4];
                skeleton3d.rotations[i].elements[5] = rotationPtr[5];
                skeleton3d.rotations[i].elements[6] = rotationPtr[6];
                skeleton3d.rotations[i].elements[7] = rotationPtr[7];
                skeleton3d.rotations[i].elements[8] = rotationPtr[8];
            }

            //1 + 21 * 3 + 21 * 9 = 253
            float* bonesLengthPtr = &((float*)ptr)[253];
            for (int i = 0, iMax = 21; i < iMax; i++)
            {
                skeleton3d.bone_lengths[i] = bonesLengthPtr[i];
            }
        }

        public static unsafe void GetSkeleton2DPose(IntPtr skeleton2DPtr, ref ta3d_skeleton_2d_s skeleton2d)
        {
            void* ptr = skeleton2DPtr.ToPointer();
            int* intPtr = (int*)ptr;
            int resultType_Int = intPtr[0];
            skeleton2d.status = GetResultType(resultType_Int);
            float* pointsPtr = &((float*)ptr)[1];
            for (int i = 0, iMax = 21; i < iMax; i++)
            {
                skeleton2d.points[i].coordinates[0] = pointsPtr[i * 2];
                skeleton2d.points[i].coordinates[1] = pointsPtr[i * 2 + 1];
            }
        }



        internal static ResultType GetResultType(int resultTypeInt)
        {
            ResultType ret = ResultType.INVALID_ARGUMENT;
            switch (resultTypeInt)
            {
                case 0:
                    ret = ResultType.RESULT_UNAVAILABLE;
                    break;

                case 1:
                    ret = ResultType.RESULT_OK;
                    break;

                case 2:
                    ret = ResultType.BUSY;
                    break;

                case 3:
                    ret = ResultType.INVALID_ARGUMENT;
                    break;

                case 4:
                    ret = ResultType.TIMEOUT;
                    break;

                default:
                    ret = ResultType.RESULT_UNAVAILABLE;
                    break;
            }
            return ret;
        }
    }
}