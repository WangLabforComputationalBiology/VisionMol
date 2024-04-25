using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.XR.Utils
{
    /// <summary>
    /// GL draw helper.
    /// </summary>
    public class RxDraw : MonoBehaviour
    {

        public enum RxDrawAlignment
        {
            LeftBottom = 0,

            Center = 1,

            RightTop = 2,
        }

        public enum RxCameraSpaceType
        {
            ViewPort = 0,

            ScreenSpace = 1,

            WorldSpace = 2,
        }

        private struct RxDrawRequest
        {
            internal PEDrawType drawType;
            internal Vector3 position;
            internal Vector3 position2;
            internal Quaternion rotation;
            internal float scale;
            internal Color color;
            internal Color wireColor;
            internal bool noDepth;
            internal Vector3 v3Size;
            internal float endTime;

            internal Material customMaterial;
            internal int customMaterialPass;

            internal Vector2Int scaleInt;

            internal Mesh meshReference;

            internal RxDrawAlignment alignment;

            internal Camera targetCamera;

            internal RxCameraSpaceType GUISpaceType;

            /// <summary>
            /// UI Verts : data structure wrap for UI/Text drawing
            /// </summary>
            internal UIVertex[] uiVerts;
        }

        private enum PEDrawType
        {
            Line,

            Sphere,

            WireSphere,

            WiredSphere,

            Cube,

            WireCube,

            WiredCube,

            Cuboid,

            WireCuboid,

            WiredCuboid,

            Quad,

            WireQuad,

            WiredQuad,

            TranslateGizmos,

            RotationGizmos,

            ScalingGizmos,

            Plane,

            WirePlane,

            WiredPlane,

            Grids,

            Arrow,

            Cone,

            Cylinder,

            Pyramid,

            Mesh,

            /// <summary>
            /// Draw a human skeleton, where original model height = 1m.
            /// </summary>
            HumanSkeleton,

            /// <summary>
            /// Draw a human skeleton line, where original model height = 1m
            /// </summary>
            HumanSkeletonLine,

            /// <summary>
            /// Draw a human skeleton, where original model width = 1m
            /// </summary>
            DogSkeleton,
            /// <summary>
            /// Draw a human skeleton line, where original model width = 1m
            /// </summary>
            DogSkeletonLine,

            MeshWireFrame,

            /// <summary>
            /// Draws a GUI circle.
            /// </summary>
            GUICircle,

            /// <summary>
            /// Draws full screen rect.
            /// </summary>
            ScreenRect,

            /// <summary>
            /// Draws 3d text mesh
            /// </summary>
            TextMesh3D,

            WireCone,
        }

        static List<RxDrawRequest> sDrawRequests = new List<RxDrawRequest>();

        static bool m_DepthRendering = false;

        public static bool DepthRendering
        {
            get
            {
                return m_DepthRendering;
            }
            set
            {
                m_DepthRendering = value;
            }
        }
        static TextGenerator sTextGen = null;
        static TextGenerator TextGen
        {
            get
            {
                if (sTextGen == null)
                    sTextGen = new TextGenerator();

                return sTextGen;
            }
        }

        static List<UIVertex> sTextVertices = new List<UIVertex>();
        static List<Vector3> sTextMeshVertices = new List<Vector3>();
        static List<Vector3> sTextMeshNormals = new List<Vector3>();
        static List<Vector4> sTextMeshTangents = new List<Vector4>();
        static List<int> sTextMeshTris = new List<int>();
        static List<Vector2> sTextMeshUVs = new List<Vector2>();
        static List<Color> sTextMeshColor = new List<Color>();

        static Material matUI = null;

        static Material MaterialUI 
        {
            get
            {
                if (matUI == null)
                    matUI = new Material(Shader.Find("UI/Default"));
                return matUI;
            }
        }

        static Mesh sTextMesh = null;
        static Mesh TextMesh
        {
            get
            {
                if (sTextMesh == null)
                {
                    sTextMesh = new Mesh();
                    sTextMesh.name = "PEDrawTextMesh";
                    TextMesh.MarkDynamic();
                }
                return sTextMesh;
            }
        }

        Coroutine clearQueueCoroutine = null;

        private void OnEnable()
        {
            clearQueueCoroutine = StartCoroutine(ClearQueue());
        }

        private void OnDisable()
        {
            if(clearQueueCoroutine != null)
            {
                StopCoroutine(clearQueueCoroutine);
                clearQueueCoroutine = null;
            }
        }



        /// <summary>
        /// Sets the custom material for previous draw command.
        /// </summary>
        /// <param name="CustomMaterial">Custom material.</param>
        /// <param name="Pass">Pass.</param>
        public static void SetCustomMaterial(Material CustomMaterial, int Pass = 0)
        {
            if (sDrawRequests.Count == 0)
            {
                Debug.Log("Draw command queue is empty, can't set custom material.");
                return;
            }
            var drawCommand = sDrawRequests[sDrawRequests.Count - 1];
            drawCommand.customMaterial = CustomMaterial;
            drawCommand.customMaterialPass = Pass;
            sDrawRequests[sDrawRequests.Count - 1] = drawCommand;
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="color">Color.</param>
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawLine(start, end, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Line,
                position = start,
                position2 = end,
                color = color,
                endTime = Time.time + duration,
            }
                        );

            if (singleton == null)
            {
                InitializeBehavior();
            }

        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="color">Color.</param>
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float lineWidth, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawLine(start, end, lineWidth, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Line,
                position = start,
                position2 = end,
                color = color,
                scale = lineWidth,
                endTime = Time.time + duration,
            }
                        );

            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a ray.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="direction">Direction.</param>
        /// <param name="color">Color.</param>
        public static void DrawRay(Vector3 start, Vector3 direction, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawLine(start, start + direction, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Line,
                position = start,
                position2 = start + direction,
                color = color,
                endTime = Time.time + duration,
            }
                        );

            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a ray.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="direction">Direction.</param>
        /// <param name="color">Color.</param>
        public static void DrawRay(Vector3 start, Vector3 direction, Color color, float lineWidth, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawLine(start, start + direction, lineWidth, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Line,
                position = start,
                position2 = start + direction,
                color = color,
                scale = lineWidth,
                endTime = Time.time + duration,
            }
                        );

            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a sphere.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawSphere(Vector3 center, float radius, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawSphere(center, Quaternion.identity, radius, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Sphere,
                position = center,
                scale = radius,
                color = color,
                endTime = Time.time + duration,
            }
                        );

            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a wire sphere.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWireSphere(Vector3 center, float radius, Color wireColor, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWireSphere(center, Quaternion.identity, radius, wireColor);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WireSphere,
                position = center,
                scale = radius,
                wireColor = wireColor,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }

        }


        /// <summary>
        /// Draws a sphere with wired.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWiredSphere(Vector3 center, float radius, Color color, Color wireColor, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWiredSphere(center, Quaternion.identity, radius, color, wireColor);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WiredSphere,
                position = center,
                scale = radius,
                color = color,
                wireColor = wireColor,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }



        /// <summary>
        /// Draws an arrow.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawArrow(Vector3 center, Quaternion rotation, float tipPivot, float shaftWidth, float tipWidth, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawArrow(center, rotation * Vector3.forward, tipPivot, shaftWidth, tipWidth, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Arrow,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(tipPivot, shaftWidth, tipWidth),
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws mesh directly
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawMesh(Mesh m, Vector3 center, Quaternion rotation, Vector3 scale, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawMesh(m, center, rotation, scale, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Mesh,
                position = center,
                rotation = rotation,
                v3Size = scale,
                color = color,
                endTime = Time.time + duration,
                meshReference = m,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws mesh wire frame
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawMeshWireframe(Mesh m, Vector3 center, Quaternion rotation, Vector3 scale, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDrawExt.DrawMeshWireFrame(center, rotation, scale, m, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.MeshWireFrame,
                position = center,
                rotation = rotation,
                v3Size = scale,
                color = color,
                endTime = Time.time + duration,
                meshReference = m,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a cube.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawCube(Vector3 center, Quaternion rotation, float size, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawCube(center, rotation, size, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Cube,
                position = center,
                rotation = rotation,
                scale = size,
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }

        }


        /// <summary>
        /// Draws a cone.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawCone(Vector3 center, Quaternion rotation, float width, float height, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawCone(center, rotation, width, height, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Cone,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(width, height),
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }

        }

        /// <summary>
        /// Draws a wire cone.
        /// </summary>
        public static void DrawWireCone(Vector3 center, Quaternion rotation, float width, float height, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWireCone(center, rotation, width, height, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WireCone,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(width, height),
                wireColor = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }

        }



        /// <summary>
        /// Draws a cylinder.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawCylinder(Vector3 center, Quaternion rotation, float width, float height, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawCylinder(center, rotation, width, height, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Cylinder,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(width, height),
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a pyramid.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawPyramid(Vector3 center, Quaternion rotation, float width, float height, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawPyramid(center, rotation, width, height, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Pyramid,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(width, height),
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }

        }

        /// <summary>
        /// Draws a wire cube.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWireCube(Vector3 center, Quaternion rotation, float size, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWireCube(center, rotation, size, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WireCube,
                position = center,
                rotation = rotation,
                scale = size,
                wireColor = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a wired cube.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWiredCube(Vector3 center, Quaternion rotation, float size, Color color, Color wireColor, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWiredCube(center, rotation, size, color, wireColor);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WiredCube,
                position = center,
                rotation = rotation,
                scale = size,
                wireColor = wireColor,
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a cuboid.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawCuboid(Vector3 center, Quaternion rotation, Vector3 size, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawCuboid(center, rotation, size, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Cuboid,
                position = center,
                rotation = rotation,
                v3Size = size,
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a wire Cuboid.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWireCuboid(Vector3 center, Quaternion rotation, Vector3 size, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWireCuboid(center, rotation, size, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WireCuboid,
                position = center,
                rotation = rotation,
                v3Size = size,
                wireColor = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a wired Cuboid.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWiredCuboid(Vector3 center, Quaternion rotation, Vector3 size, Color color, Color wireColor, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWiredCuboid(center, rotation, size, color, wireColor);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WiredCuboid,
                position = center,
                rotation = rotation,
                v3Size = size,
                wireColor = wireColor,
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a quad.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawQuad(Vector3 center, Quaternion rotation, float width, float height, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawQuad(center, rotation, width, height, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Quad,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(width, height, 0),
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a quad of wire.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWireQuad(Vector3 center, Quaternion rotation, float width, float height, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWireQuad(center, rotation, width, height, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WireQuad,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(width, height, 0),
                wireColor = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a wired and filled quad.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWiredQuad(Vector3 center, Quaternion rotation, float width, float height, Color color, Color wireColor, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWiredQuad(center, rotation, width, height, color, wireColor);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WiredQuad,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(width, height, 0),
                color = color,
                wireColor = wireColor,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }



        /// <summary>
        /// Draws a plane.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawPlane(Vector3 center, Quaternion rotation, float width, float height, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawPlane(center, rotation, width, height, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Plane,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(width, height, 0),
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a plane of wire.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWirePlane(Vector3 center, Quaternion rotation, float width, float height, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWirePlane(center, rotation, width, height, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WirePlane,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(width, height, 0),
                wireColor = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a wired and filled plane.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="color">Color.</param>
        public static void DrawWiredPlane(Vector3 center, Quaternion rotation, float width, float height, Color color, Color wireColor, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawWiredPlane(center, rotation, width, height, color, wireColor);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.WiredPlane,
                position = center,
                rotation = rotation,
                v3Size = new Vector3(width, height, 0),
                color = color,
                wireColor = wireColor,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }



        /// <summary>
        /// Draws the translate(positional) gizmos.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        public static void DrawTranslateGizmos(Vector3 position, Quaternion rotation, float scale, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawTranslateGizmo(position, rotation, scale);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.TranslateGizmos,
                position = position,
                rotation = rotation,
                scale = scale,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a 3D text mesh.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="text">Text.</param>
        /// <param name="Duration">Duration.</param>
        /// <param name="color">Color.</param>
        /// <param name="font">Font.If null, the default arial is used.</param>
        /// <param name="Style">Style.</param>
        public static void Text3D(Vector3 position, Quaternion rotation, float scale, string text, Color color, float Duration = 0, Font font = null, FontStyle style = default(FontStyle))
        {
            Font drawFont = font != null ? font : Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            Material drawMaterial = drawFont.material;

            TextGenerationSettings setting = new TextGenerationSettings();
            setting.textAnchor = TextAnchor.MiddleCenter;
            setting.color = color;
            setting.generationExtents = Vector2.zero;
            setting.pivot = new Vector2(0.5f, 0.5f);
            setting.richText = false;
            setting.font = drawFont;
            setting.fontStyle = style;
            setting.fontSize = 1;
            setting.horizontalOverflow = HorizontalWrapMode.Overflow;
            setting.verticalOverflow = VerticalWrapMode.Overflow;
            setting.alignByGeometry = true;
            setting.lineSpacing = 1;
            setting.scaleFactor = 120;
            setting.resizeTextForBestFit = false;
            setting.resizeTextMaxSize = 1;
            setting.resizeTextMinSize = 1;

            TextGen.Invalidate();
            TextGen.Populate(text, setting);
            sTextVertices.Clear();
//            TextGen.GetVertices(sTextVertices);
            UIVertex[] _uiVerts = TextGen.GetVerticesArray();

            float rescaleFactor = scale * 0.01f;
            RxDrawRequest drawTextReq = new RxDrawRequest()
            {
                    drawType = PEDrawType.TextMesh3D,
                    uiVerts = _uiVerts,
                    position = position,
                    rotation = rotation,
                    scale = rescaleFactor,
                    color = color,
                    customMaterial = drawMaterial,
                    customMaterialPass = 0,
                    endTime = Time.time + Duration,
            };


            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UIVerticesToTextMesh(new List<UIVertex>(_uiVerts), TextMesh);
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.CustomMaterial = drawMaterial;
                UltiDraw.Begin();
                UltiDraw.DrawMesh(TextMesh, position, rotation, Vector3.one * rescaleFactor, setting.color);
                UltiDraw.End();
                UltiDraw.CustomMaterial = null;
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(drawTextReq);
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        ///// <summary>
        ///// Draws text using default setting:
        ///// alignment = middle center, 
        ///// rich text = true,
        ///// Best fit = false,
        ///// Vertical and horizontal wrap mode = Overflow
        ///// </summary>
        ///// <param name="position">Position.</param>
        ///// <param name="rotation">Rotation.</param>
        ///// <param name="scale">Scale.</param>
        ///// <param name="text">Text.</param>
        ///// <param name="setting">Setting.</param>
        ///// <param name="duration">Duration.</param>
        //[System.Obsolete ("Deprecated method, call PEDraw.Text3D()")]
        //public static void DrawText(Vector3 position, Quaternion rotation, Vector3 scale, string text, Font font, FontStyle fontStyle, Color color, float duration = 0)
        //{
        //    TextGenerationSettings setting = new TextGenerationSettings();
        //    setting.textAnchor = TextAnchor.MiddleCenter;
        //    setting.color = color;
        //    setting.generationExtents = Vector2.zero;
        //    setting.pivot = new Vector2(0.5f, 0.5f);
        //    setting.richText = false;
        //    setting.font = font;
        //    setting.fontStyle = fontStyle;
        //    setting.fontSize = 1;
        //    setting.horizontalOverflow = HorizontalWrapMode.Overflow;
        //    setting.verticalOverflow = VerticalWrapMode.Overflow;
        //    setting.alignByGeometry = true;
        //    setting.lineSpacing = 1;
        //    setting.scaleFactor = 120;
        //    setting.resizeTextForBestFit = false;
        //    setting.resizeTextMaxSize = 1;
        //    setting.resizeTextMinSize = 1;

        //    DrawText(position, rotation, scale, text, setting, duration);
        //}

        ///// <summary>
        ///// Draws text using custom setting.
        ///// </summary>
        ///// <param name="position">Position.</param>
        ///// <param name="rotation">Rotation.</param>
        ///// <param name="scale">Scale.</param>
        ///// <param name="text">Text.</param>
        ///// <param name="setting">Setting.</param>
        ///// <param name="color">Color.</param>
        ///// <param name="duration">Duration.</param>
        //[System.Obsolete ("Deprecated method, call PEDraw.Text3D()")]
        //public static void DrawText(Vector3 position, Quaternion rotation, Vector3 scale, string text, TextGenerationSettings setting, float duration = 0)
        //{
        //    TextGen.Invalidate();
        //    TextGen.Populate(text, setting);
        //    sTextVertices.Clear();
        //    TextGen.GetVertices(sTextVertices);

        //    //Convert UIVertices to Mesh:
        //    if (sTextVertices.Count == 0)
        //        return;

        //    //UIVertice to text mesh:
        //    if ((sTextVertices.Count % 4) != 0)
        //    {
        //        Debug.LogErrorFormat("Error drawing text:{0}. Reason : UIVertices to mesh error: {1} not multiplier of 4 !", text, sTextVertices.Count);
        //        return;
        //    }

        //    if (setting.font == null || setting.font.material == null)
        //    {
        //        Debug.LogError("Font's invalid !");
        //        return;
        //    }


        //    //Fill up text mesh:
        //    UIVerticesToTextMesh(sTextVertices, TextMesh);
        //    //For editor call, direct draw:
        //    if (Application.isEditor)
        //    {
        //        UltiDraw.SetDepthRendering(DepthRendering);
        //        UltiDraw.CustomMaterial = setting.font.material;
        //        UltiDraw.Begin();
        //        UltiDraw.DrawMesh(TextMesh, position, rotation, scale * 0.01f, setting.color);
        //        UltiDraw.End();
        //        UltiDraw.CustomMaterial = null;
        //    }
        //    if (!Application.isPlaying) return;
        //    sDrawRequests.Add(new PEDrawRequest()
        //    {
        //        drawType = PEDrawType.Mesh,
        //        position = position,
        //        rotation = rotation,
        //        v3Size = scale * 0.01f,
        //        endTime = Time.time + duration,
        //        meshReference = TextMesh,
        //        color = setting.color,
        //        customMaterial = setting.font.material,
        //    }
        //    );
        //    if (singleton == null)
        //    {
        //        InitializeBehavior();
        //    }
        //}

        /// <summary>
        /// Convert UI vertices to mesh
        /// </summary>
        /// <param name="verticesLst">Vertices lst.</param>
        /// <param name="TextMesh">Text mesh.</param>
        static void UIVerticesToTextMesh(List<UIVertex> verticesLst, Mesh TextMesh)
        {
            int charCount = (verticesLst.Count / 4);

            sTextMeshVertices.Clear();
            sTextMeshVertices.Capacity = verticesLst.Count;

            sTextMeshTris.Clear();
            sTextMeshTris.Capacity = charCount * 2 * 3;

            sTextMeshUVs.Clear();
            sTextMeshUVs.Capacity = verticesLst.Count;

            sTextMeshColor.Clear();
            sTextMeshColor.Capacity = verticesLst.Count;

            sTextMeshNormals.Clear();
            sTextMeshNormals.Capacity = verticesLst.Count;

            sTextMeshTangents.Clear();
            sTextMeshTangents.Capacity = verticesLst.Count;

            for (int c = 0; c < charCount; c++)
            {
                var c1 = verticesLst[c * 4];
                var c2 = verticesLst[c * 4 + 1];
                var c3 = verticesLst[c * 4 + 2];
                var c4 = verticesLst[c * 4 + 3];

                Vector3 v1 = c1.position;
                Vector3 v2 = c2.position;
                Vector3 v3 = c3.position;
                Vector3 v4 = c4.position;

                sTextMeshVertices.Add(v1);
                sTextMeshVertices.Add(v2);
                sTextMeshVertices.Add(v3);
                sTextMeshVertices.Add(v4);

                int t1 = sTextMeshVertices.Count - 4;
                int t2 = sTextMeshVertices.Count - 3;
                int t3 = sTextMeshVertices.Count - 2;
                int t4 = sTextMeshVertices.Count - 1;

                sTextMeshTris.Add(t1);
                sTextMeshTris.Add(t2);
                sTextMeshTris.Add(t3);

                sTextMeshTris.Add(t1);
                sTextMeshTris.Add(t3);
                sTextMeshTris.Add(t4);

                sTextMeshUVs.Add(c1.uv0);
                sTextMeshUVs.Add(c2.uv0);
                sTextMeshUVs.Add(c3.uv0);
                sTextMeshUVs.Add(c4.uv0);

                sTextMeshColor.Add(c1.color);
                sTextMeshColor.Add(c2.color);
                sTextMeshColor.Add(c3.color);
                sTextMeshColor.Add(c4.color);

                sTextMeshNormals.Add(c1.normal);
                sTextMeshNormals.Add(c2.normal);
                sTextMeshNormals.Add(c3.normal);
                sTextMeshNormals.Add(c4.normal);

                sTextMeshTangents.Add(c1.tangent);
                sTextMeshTangents.Add(c2.tangent);
                sTextMeshTangents.Add(c3.tangent);
                sTextMeshTangents.Add(c4.tangent);

            }
            TextMesh.Clear();
            TextMesh.SetVertices(sTextMeshVertices);
            TextMesh.SetUVs(0, sTextMeshUVs);
            TextMesh.SetColors(sTextMeshColor);
            TextMesh.SetNormals(sTextMeshNormals);
            TextMesh.SetTangents(sTextMeshTangents);
            TextMesh.SetTriangles(sTextMeshTris, 0);

            //            TextMesh.UploadMeshData (false);
        }

        /// <summary>
        /// Draws the rotation gizmos.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        public static void DrawRotationGizmos(Vector3 position, Quaternion rotation, float scale, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawRotateGizmo(position, rotation, scale);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.RotationGizmos,
                position = position,
                rotation = rotation,
                scale = scale,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws the grid gizmos.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        public static void DrawGrids(Vector3 position, Quaternion rotation, int gridX, int gridY,
            float gridXSize, float gridYSize, Color gridColor, float duration = 0, RxDrawAlignment drawAlignment = RxDrawAlignment.LeftBottom)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();

                switch (drawAlignment)
                {
                    case RxDrawAlignment.Center:
                        UltiDraw.DrawGrid(position, rotation, gridX, gridY, gridXSize, gridYSize, color: gridColor);
                        break;

                    case RxDrawAlignment.LeftBottom:
                        UltiDraw.DrawGridLBOrigin(position, rotation, gridX, gridY, gridXSize, gridYSize, color: gridColor);
                        break;

                    case RxDrawAlignment.RightTop:
                        UltiDraw.DrawGridRTOrigin(position, rotation, gridX, gridY, gridXSize, gridYSize, color: gridColor);
                        break;
                }

                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.Grids,
                position = position,
                rotation = rotation,
                scaleInt = new Vector2Int(gridX, gridY),
                v3Size = new Vector3(gridXSize, gridYSize, 0),
                color = gridColor,
                endTime = Time.time + duration,
                alignment = drawAlignment,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a human skeleton.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        public static void DrawHumanSkeleton(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDrawExt.DrawHumanSkeleton(position, rotation, scale, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.HumanSkeleton,
                position = position,
                rotation = rotation,
                v3Size = scale,
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a human skeleton line.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        public static void DrawHumanSkeletonLine(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDrawExt.DrawHumanSkeletonLine(position, rotation, scale, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.HumanSkeletonLine,
                position = position,
                rotation = rotation,
                v3Size = scale,
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a GUI circle at the screen point.
        /// If drawSpace = ViewPort, Position and Radius is measured in screen coordination.
        /// Else If drawSpace = ScreenSpace, Screen point and Radius is measured in pixel coordination.
        /// Else If drawSpace = WorldSpace, Screen point and Radius is measured in 3D global coordination.
        /// </summary>
        /// <param name="ScreenPoint">Screen point.</param>
        /// <param name="Radius">Radius.</param>
        /// <param name="color">Color.</param>
        /// <param name="cameraTarget">Camera target.</param>
        /// <param name="duration">Duration.</param>
        public static void DrawGUICircle(Vector3 Position, float Radius, Color color,  Camera cameraTarget = null, float duration = 0)
        {
            if (!Application.isPlaying)
            {
                return;
            }
            var drawReq = new RxDrawRequest()
            {
                drawType = PEDrawType.GUICircle,
                position = Position,
                scale = Radius,
                color = color,
                endTime = Time.time + duration,
                targetCamera = cameraTarget,
                    GUISpaceType = RxCameraSpaceType.ScreenSpace,
            };
            ConvertPixelSpace(ref drawReq);
            sDrawRequests.Add(drawReq);
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        /// <summary>
        /// Draws a dog skeleton.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        public static void DrawDogSkeleton(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDrawExt.DrawDogSkeleton(position, rotation, scale, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.DogSkeleton,
                position = position,
                rotation = rotation,
                v3Size = scale,
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a dog skeleton line.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        public static void DrawDogSkeletonLine(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDrawExt.DrawDogSkeletonLine(position, rotation, scale, color);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.DogSkeletonLine,
                position = position,
                rotation = rotation,
                v3Size = scale,
                color = color,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a scale gizmos.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        public static void DrawScaleGizmos(Vector3 position, Quaternion rotation, float scale, float duration = 0)
        {
            //For editor call, direct draw:
            if (Application.isEditor)
            {
                UltiDraw.SetDepthRendering(DepthRendering);
                UltiDraw.Begin();
                UltiDraw.DrawScaleGizmo(position, rotation, scale);
                UltiDraw.End();
            }
            if (!Application.isPlaying) return;
            sDrawRequests.Add(new RxDrawRequest()
            {
                drawType = PEDrawType.ScalingGizmos,
                position = position,
                rotation = rotation,
                scale = scale,
                endTime = Time.time + duration,
            }
                        );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }

        /// <summary>
        /// Draws a screen rect, when scale = 1, the rect fully covers screen viewport.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="duration">Duration.</param>
        public static void DrawScreenRect (Color Color, float Scale = 1, float Duration = 0)
        {
            sDrawRequests.Add(new RxDrawRequest()
                {
                    drawType = PEDrawType.ScreenRect,
                    scale = Scale,
                    color = Color,
                    endTime = Time.time + Duration,
                }
            );
            if (singleton == null)
            {
                InitializeBehavior();
            }
        }


        static RxDraw singleton;

        /// <summary>
        /// Initializes the behavior.
        /// </summary>
        static void InitializeBehavior()
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType<RxDraw>();
                if (!singleton)
                {
                    var go = new GameObject("RxDraw");
                    singleton = go.AddComponent<RxDraw>();
                    //go.hideFlags = HideFlags.HideAndDontSave;
                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(go);
                    }
                }
            }
        }

        void OnRenderObject()
        {
            if (sDrawRequests.Count > 0)
            {
                UltiDraw.Begin();
                //                UltiDraw.SetCurvature(0f);
                UltiDraw.SetDepthRendering(DepthRendering);

                for (int i = sDrawRequests.Count - 1; i >= 0; i--)
                {
                    var dReq = sDrawRequests[i];
                    //                    Debug.Log ("Draw: " + dReq.drawType.ToString());

                    if (dReq.customMaterial != null)
                    {
                        UltiDraw.CustomMaterial = dReq.customMaterial;
                        UltiDraw.CustomMaterialPass = dReq.customMaterialPass;
                    }
                    switch (dReq.drawType)
                    {
                        case PEDrawType.Line:
                            if (dReq.scale <= 0)
                            {
                                UltiDraw.DrawLine(dReq.position, dReq.position2, dReq.color);
                            }
                            else
                            {
                                UltiDraw.DrawLine(dReq.position, dReq.position2, dReq.scale, dReq.color);
                            }
                            break;

                        case PEDrawType.Sphere:
                            UltiDraw.DrawSphere(dReq.position, Quaternion.identity, dReq.scale, dReq.color);
                            break;
                        case PEDrawType.WireSphere:
                            UltiDraw.DrawWireSphere(dReq.position, Quaternion.identity, dReq.scale, dReq.wireColor);
                            break;
                        case PEDrawType.WiredSphere:
                            UltiDraw.DrawWiredSphere(dReq.position, Quaternion.identity, dReq.scale, dReq.color, dReq.wireColor);
                            break;

                        case PEDrawType.Cube:
                            UltiDraw.DrawCube(dReq.position, dReq.rotation, dReq.scale, dReq.color);
                            break;
                        case PEDrawType.WireCube:
                            UltiDraw.DrawWireCube(dReq.position, dReq.rotation, dReq.scale, dReq.wireColor);
                            break;
                        case PEDrawType.WiredCube:
                            UltiDraw.DrawWiredCube(dReq.position, dReq.rotation, dReq.scale, dReq.color, dReq.wireColor);
                            break;

                        case PEDrawType.Cuboid:
                            UltiDraw.DrawCuboid(dReq.position, dReq.rotation, dReq.v3Size, dReq.color);
                            break;
                        case PEDrawType.WireCuboid:
                            UltiDraw.DrawWireCuboid(dReq.position, dReq.rotation, dReq.v3Size, dReq.wireColor);
                            break;
                        case PEDrawType.WiredCuboid:
                            UltiDraw.DrawWiredCuboid(dReq.position, dReq.rotation, dReq.v3Size, dReq.color, dReq.wireColor);
                            break;

                        case PEDrawType.Quad:
                            UltiDraw.DrawQuad(dReq.position, dReq.rotation, dReq.v3Size.x, dReq.v3Size.y, dReq.color);
                            break;
                        case PEDrawType.WireQuad:
                            UltiDraw.DrawWireQuad(dReq.position, dReq.rotation, dReq.v3Size.x, dReq.v3Size.y, dReq.wireColor);
                            break;
                        case PEDrawType.WiredQuad:
                            UltiDraw.DrawWiredQuad(dReq.position, dReq.rotation, dReq.v3Size.x, dReq.v3Size.y, dReq.color, dReq.wireColor);
                            break;


                        case PEDrawType.Plane:
                            UltiDraw.DrawPlane(dReq.position, dReq.rotation, dReq.v3Size.x, dReq.v3Size.y, dReq.color);
                            break;
                        case PEDrawType.WirePlane:
                            UltiDraw.DrawWirePlane(dReq.position, dReq.rotation, dReq.v3Size.x, dReq.v3Size.y, dReq.wireColor);
                            break;
                        case PEDrawType.WiredPlane:
                            UltiDraw.DrawWiredPlane(dReq.position, dReq.rotation, dReq.v3Size.x, dReq.v3Size.y, dReq.color, dReq.wireColor);
                            break;

                        case PEDrawType.TranslateGizmos:
                            UltiDraw.DrawTranslateGizmo(dReq.position, dReq.rotation, dReq.scale);
                            break;

                        case PEDrawType.RotationGizmos:
                            UltiDraw.DrawRotateGizmo(dReq.position, dReq.rotation, dReq.scale);
                            break;

                        case PEDrawType.ScalingGizmos:
                            UltiDraw.DrawScaleGizmo(dReq.position, dReq.rotation, dReq.scale);
                            break;

                        case PEDrawType.Grids:

                            switch (dReq.alignment)
                            {
                                case RxDrawAlignment.Center:
                                    UltiDraw.DrawGrid(dReq.position, dReq.rotation, dReq.scaleInt.x, dReq.scaleInt.y, dReq.v3Size.x, dReq.v3Size.y, dReq.color);
                                    break;

                                case RxDrawAlignment.LeftBottom:
                                    UltiDraw.DrawGridLBOrigin(dReq.position, dReq.rotation, dReq.scaleInt.x, dReq.scaleInt.y, dReq.v3Size.x, dReq.v3Size.y, dReq.color);
                                    break;

                                case RxDrawAlignment.RightTop:
                                    UltiDraw.DrawGridRTOrigin(dReq.position, dReq.rotation, dReq.scaleInt.x, dReq.scaleInt.y, dReq.v3Size.x, dReq.v3Size.y, dReq.color);
                                    break;
                            }
                            break;

                        case PEDrawType.Arrow:
                            UltiDraw.DrawArrow(dReq.position, dReq.rotation * Vector3.forward, dReq.v3Size.x, dReq.v3Size.y, dReq.v3Size.z, dReq.color);
                            break;

                        case PEDrawType.Cone:
                            UltiDraw.DrawCone(dReq.position, dReq.rotation, dReq.v3Size.x, dReq.v3Size.y, dReq.color);
                            break;

                        case PEDrawType.Cylinder:
                            UltiDraw.DrawCylinder(dReq.position, dReq.rotation, dReq.v3Size.x, dReq.v3Size.y, dReq.color);
                            break;

                        case PEDrawType.Pyramid:
                            UltiDraw.DrawPyramid(dReq.position, dReq.rotation, dReq.v3Size.x, dReq.v3Size.y, dReq.color);
                            break;

                        case PEDrawType.Mesh:
                            UltiDraw.DrawMesh(dReq.meshReference, dReq.position, dReq.rotation, dReq.v3Size, dReq.color);
                            break;

                        case PEDrawType.MeshWireFrame:
                            UltiDrawExt.DrawMeshWireFrame(dReq.position, dReq.rotation, dReq.v3Size, dReq.meshReference, dReq.color);
                            break;

                        case PEDrawType.HumanSkeleton:
                            UltiDrawExt.DrawHumanSkeleton(dReq.position, dReq.rotation, dReq.v3Size, dReq.color);
                            break;

                        case PEDrawType.HumanSkeletonLine:
                            UltiDrawExt.DrawHumanSkeletonLine(dReq.position, dReq.rotation, dReq.v3Size, dReq.color);
                            break;

                        case PEDrawType.DogSkeleton:
                            UltiDrawExt.DrawDogSkeleton(dReq.position, dReq.rotation, dReq.v3Size, dReq.color);
                            break;

                        case PEDrawType.DogSkeletonLine:
                            UltiDrawExt.DrawDogSkeletonLine(dReq.position, dReq.rotation, dReq.v3Size, dReq.color);
                            break;

                        case PEDrawType.GUICircle:
                            UltiDraw.DrawGUICircle((Vector2)dReq.position, dReq.scale, dReq.color, dReq.targetCamera);
                            break;

                        case PEDrawType.ScreenRect:
                            GL.PushMatrix();
                            MaterialUI.SetPass(0);
                            MaterialUI.color = dReq.color;
                            GL.LoadOrtho();
                            GL.Begin(GL.TRIANGLE_STRIP);
                            if (dReq.scale != 1)
                            {
                                GL.Vertex(new Vector3(0.5f - 0.5f * dReq.scale,  0.5f * dReq.scale + 0.5f, 0));
                                GL.Vertex(new Vector3(0.5f * dReq.scale + 0.5f, 0.5f * dReq.scale + 0.5f, 0));
                                GL.Vertex(new Vector3(0.5f - 0.5f * dReq.scale, 0.5f - 0.5f * dReq.scale, 0));
                                GL.Vertex(new Vector3(0.5f * dReq.scale + 0.5f, 0.5f - 0.5f * dReq.scale, 0));
                            }
                            else
                            {
                                GL.Vertex(new Vector3(0, 1, 0));
                                GL.Vertex(new Vector3(1, 1, 0));
                                GL.Vertex(new Vector3(0, 0, 0));
                                GL.Vertex(new Vector3(1, 0, 0));
                            }
                            GL.End();
                            GL.PopMatrix();
                            break;

                        case PEDrawType.TextMesh3D:
                            GL.PushMatrix();
                            GL.MultMatrix(Matrix4x4.TRS(dReq.position, dReq.rotation, dReq.scale * Vector3.one));
                            dReq.customMaterial.SetPass(dReq.customMaterialPass);

                            var vLst = dReq.uiVerts;
                            int charCount = (vLst.Length / 4); //每个character占据4个vertices
                            GL.Begin(GL.TRIANGLES);
                            //draw characters by characters
                            for (int c = 0; c < charCount; c++)
                            {
                                //draw triangle first
                                {
                                    var ct11 = vLst[c * 4];
                                    var ct12 = vLst[c * 4 + 1];
                                    var ct13 = vLst[c * 4 + 2];
                                    GL.Color(dReq.color);GL.MultiTexCoord(0,ct11.uv0);GL.Vertex(ct11.position);
                                    GL.Color(dReq.color);GL.MultiTexCoord(0,ct12.uv0);GL.Vertex(ct12.position);
                                    GL.Color(dReq.color);GL.MultiTexCoord(0,ct13.uv0);GL.Vertex(ct13.position);
                                }
                                //draw triangle second
                                {
                                    var ct21 = vLst[c * 4];
                                    var ct22 = vLst[c * 4 + 2];
                                    var ct23 = vLst[c * 4 + 3];
                                    GL.Color(dReq.color);GL.TexCoord(ct21.uv0);GL.Vertex(ct21.position);
                                    GL.Color(dReq.color);GL.TexCoord(ct22.uv0);GL.Vertex(ct22.position);
                                    GL.Color(dReq.color);GL.TexCoord(ct23.uv0);GL.Vertex(ct23.position);
                                }

                            }
                            GL.End();
                            GL.PopMatrix();
                            break;

                        case PEDrawType.WireCone:
                            UltiDraw.DrawWireCone(dReq.position, dReq.rotation, dReq.v3Size.x, dReq.v3Size.y, dReq.wireColor);
                            break;

                        default:
                            break;
                    }

                    UltiDraw.CustomMaterial = null;//Clear after draw
                }

                UltiDraw.End();
            }
        }

        /// <summary>
        /// Converts the position from screen normalize space to pixel space.
        /// </summary>
        /// <param name="drawRequest">Draw request.</param>
        static void ConvertPixelSpace(ref RxDrawRequest drawRequest)
        {
            var _Camera = drawRequest.targetCamera ?? Camera.main;
            if (_Camera)
            {
                if (drawRequest.GUISpaceType == RxCameraSpaceType.ScreenSpace)
                {
                    drawRequest.position = _Camera.ScreenToViewportPoint(drawRequest.position);
                }
                else if (drawRequest.GUISpaceType == RxCameraSpaceType.WorldSpace)
                {
                    drawRequest.position = _Camera.WorldToViewportPoint(drawRequest.position);
                }
            }
        }

        void OnDestroy()
        {
            sDrawRequests.Clear();
            if (singleton)
            {
                if (Application.isEditor && Application.isPlaying == false)
                {
                    DestroyImmediate(singleton.gameObject);
                }
                else
                {
                    Destroy(singleton.gameObject);
                }
            }
            singleton = null;
        }

        IEnumerator ClearQueue ()
        {
            var waitEndFrame = new WaitForEndOfFrame();
            while (true)
            {
                yield return waitEndFrame;
                if (sDrawRequests.Count > 0)
                {
                    for (int i = sDrawRequests.Count - 1; i >= 0; i--)
                    {
                        if (sDrawRequests[i].endTime < Time.time)
                        {
                            sDrawRequests.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}