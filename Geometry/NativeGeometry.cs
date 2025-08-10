using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Minerva.Module.Geomtry.Native
{
    /// <summary>
    /// Utility class for geometry calculations
    /// </summary>
#if ENABLE_BURST_AOT
    [Unity.Burst.BurstCompile(CompileSynchronously = true)]
#endif
    public static class NativeGeometry
    {
        public const int CIRCLE_DEGREE = 360;


        /// <summary>
        /// is a point inside a polygon?
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointInPolygon(NativePolygon polygon, float3 point)
        {
            int i;
            int j;
            bool c = false;
            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (polygon[i].y > point.y != polygon[j].y > point.y &&
                 point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x)
                    c = !c;
            }
            return c;
        }

        /// <summary>
        /// get the distance to a polygon border
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceToPolygonBorder(NativePolygon polygon, float3 point)
        {
            var dist = float.MaxValue;
            for (int i = 0; i < polygon.Length; i++)
            {
                dist = math.min(dist, DistanceToSegment(polygon[i], polygon[(i + 1) % polygon.Length], point));
            }
            return dist;
        }

        /// <summary>
        /// get the distance to a polygon, return negative if the point is inside the polygon
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceToPolygonBorderSigned(NativePolygon polygon, float3 point)
        {
            var dist = DistanceToPolygonBorder(polygon, point);
            //return a negative distance representing how inside this point is
            if (IsPointInPolygon(polygon, point))
            {
                return -dist;
            }
            return dist;
        }

        /// <summary>
        /// get the distance to a polygon, return 0 if the point is inside the polygon
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceToPolygon(NativePolygon polygon, float3 point)
        {
            var dist = DistanceToPolygonBorder(polygon, point);
            //return a negative distance representing how inside this point is
            if (IsPointInPolygon(polygon, point))
            {
                return 0;
            }
            return dist;
        }

        /// <summary>
        /// get the distance to a point
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceToSegment(float3 a, float3 b, float3 point)
        {
            float3 segment = a - b;
            float3 pointSegment = point - b;
            if (math.length(segment) == 0)
            {
                return math.min(math.length(a - point), math.length(b - point));
            }
            float3 projection = math.project(pointSegment, segment);
            float3 intersection = b + projection;
            float xRange = math.unlerp(a.x, b.x, intersection.x);
            float yRange = math.unlerp(a.y, b.y, intersection.y);
            //not in range
            if (xRange >= 1 || xRange <= 0 || yRange >= 1 || yRange <= 0)
            {
                return math.min(math.length(a - point), math.length(b - point));
            }
            else return math.length(point - intersection);
        }

        /// <summary>
        /// is angle within a angle musk
        /// </summary>
        /// <param name="angleMask"></param>
        /// <param name="rotation"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithinAngleMask(float angleMask, float rotation, float angle)
        {
            float lowerBound = (rotation - angleMask / 2) % CIRCLE_DEGREE;
            float upperBound = (lowerBound + angleMask) % CIRCLE_DEGREE;
            lowerBound = (lowerBound + CIRCLE_DEGREE) % CIRCLE_DEGREE;
            upperBound = (upperBound + CIRCLE_DEGREE) % CIRCLE_DEGREE;
            angle %= CIRCLE_DEGREE;
            angle += CIRCLE_DEGREE;
            angle %= CIRCLE_DEGREE;

            // accross 360
            if (lowerBound > upperBound)
            {
                return lowerBound < angle || angle < upperBound;
            }
            // not accross 360
            else
            {
                return lowerBound < angle && angle < upperBound;
            }
        }

        /// <summary>
        /// get the difference between two angles (the smallest angle formed by two angle)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleDifference(float a, float b)
        {
            float angleA = math.abs(a - b);
            return math.min(angleA, 360 - angleA);
        }
    }


    /// <summary>
    /// A class representing a polygon
    /// </summary>
    public struct NativePolygon : IEnumerable<float3>, IDisposable
    {
        public NativeArray<float3> vertices;

        public int Length => vertices.Length;
        public float3 this[int index]
        {
            get { return vertices[index]; }
            set { vertices[index] = value; }
        }


        public NativePolygon(float3[] vertices, Allocator allocator)
        {
            this.vertices = new NativeArray<float3>(vertices, allocator);
        }

        public NativePolygon(Vector3[] vertices, Allocator allocator)
        {
            this.vertices = new NativeArray<float3>(vertices.Length, allocator);
            for (int i = 0; i < vertices.Length; i++)
            {
                this.vertices[i] = new float3(vertices[i]);
            }
        }


        public NativePolygon(Vector2[] vertices, Allocator allocator)
        {
            this.vertices = new NativeArray<float3>(vertices.Length, allocator);
            for (int i = 0; i < vertices.Length; i++)
            {
                this.vertices[i] = new float3(vertices[i], 0);
            }
        }

        public NativePolygon(float2[] vertices, Allocator allocator)
        {
            this.vertices = new NativeArray<float3>(vertices.Length, allocator);
            for (int i = 0; i < vertices.Length; i++)
            {
                this.vertices[i] = new float3(vertices[i], 0);
            }
        }

        public NativePolygon(int length, Allocator allocator)
        {
            this.vertices = new NativeArray<float3>(length, allocator);
        }

        public static implicit operator NativePolygon(NativeArray<float3> vertices)
        {
            return new NativePolygon() { vertices = vertices };
        }



        //public (float3, float3, float3)[] Edges
        //{
        //    get
        //    {
        //        (float3, float3, float3)[] values = new (float3, float3, float3)[Length];
        //        for (int i = 0; i < vertices.Length; i++)
        //        {
        //            values[i] = (vertices[i], vertices[(i + 1) % Length], vertices[(i + 1) % Length] - vertices[i]);
        //        }
        //        return values;
        //    }
        //}

        public IEnumerator<float3> GetEnumerator()
        {
            return ((IEnumerable<float3>)vertices).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return vertices.GetEnumerator();
        }

        public void Dispose()
        {
            vertices.Dispose();
        }

        public readonly void Dispose(JobHandle handle)
        {
            vertices.Dispose(handle);
        }
    }
}