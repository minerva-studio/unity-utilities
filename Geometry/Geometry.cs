using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Minerva.Module
{
    /// <summary>
    /// A class representing a polygon
    /// </summary>
    public struct Polygon : IEnumerable<Vector3>, ICloneable
    {
        public int Length => vertices.Length;
        public Vector3 this[int index]
        {
            get { return vertices[index]; }
            set { vertices[index] = value; }
        }

        public Vector3[] vertices;


        public (Vector3, Vector3, Vector3)[] Edges
        {
            get
            {
                (Vector3, Vector3, Vector3)[] values = new (Vector3, Vector3, Vector3)[Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    values[i] = (vertices[i], vertices[(i + 1) % Length], vertices[(i + 1) % Length] - vertices[i]);
                }
                return values;
            }
        }


        public static implicit operator Polygon(Vector3[] vertices)
        {
            return new Polygon() { vertices = vertices };
        }
        public static implicit operator Polygon(Vector2[] vertices)
        {
            var poly = new Polygon();
            int i = 0;
            Array.ForEach(vertices, (v) => poly[i++] = v);
            return poly;
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            return ((IEnumerable<Vector3>)vertices).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return vertices.GetEnumerator();
        }

        public object Clone()
        {
            return vertices.Clone();
        }
    }

    /// <summary>
    /// Utility class for geometry calculations
    /// </summary>
    public static class Geometry
    {
        public const int CIRCLE_DEGREE = 360;

        /// <summary>
        /// is a point inside a polygon?
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        //public static bool IsPointInPolygon(Vector2[] vertex, Vector2 point)
        //{
        //    int i;
        //    int j;
        //    bool c = false;
        //    for (i = 0, j = vertex.Length - 1; i < vertex.Length; j = i++)
        //    {
        //        if (vertex[i].y > point.y != vertex[j].y > point.y &&
        //         point.x < (vertex[j].x - vertex[i].x) * (point.y - vertex[i].y) / (vertex[j].y - vertex[i].y) + vertex[i].x)
        //            c = !c;
        //    }
        //    return c;
        //}

        /// <summary>
        /// is a point inside a polygon?
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool IsPointInPolygon(Polygon polygon, Vector3 point)
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
        public static float DistanceToPolygonBorder(Polygon polygon, Vector3 point)
        {
            var dist = float.MaxValue;
            for (int i = 0; i < polygon.Length; i++)
            {
                dist = Mathf.Min(dist, DistanceToSegment(polygon[i], polygon[(i + 1) % polygon.Length], point));
            }
            return dist;
        }

        /// <summary>
        /// get the distance to a polygon, return negative if the point is inside the polygon
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float DistanceToPolygonBorderSigned(Polygon polygon, Vector3 point)
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
        public static float DistanceToPolygon(Polygon polygon, Vector3 point)
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
        public static float DistanceToSegment(Vector3 a, Vector3 b, Vector3 point)
        {
            Vector3 segment = a - b;
            Vector3 pointSegment = point - b;
            if (segment.sqrMagnitude == 0)
            {
                return Mathf.Min((a - point).magnitude, (b - point).magnitude);
            }
            Vector3 projection = Vector3.Project(pointSegment, segment);
            Vector3 intersection = b + projection;
            float xRange = Mathf.InverseLerp(a.x, b.x, intersection.x);
            float yRange = Mathf.InverseLerp(a.y, b.y, intersection.y);
            //not in range
            if (xRange >= 1 || xRange <= 0 || yRange >= 1 || yRange <= 0)
            {
                return Mathf.Min((a - point).magnitude, (b - point).magnitude);
            }
            else return (point - intersection).magnitude;
        }

        /// <summary>
        /// is angle within a angle musk
        /// </summary>
        /// <param name="angleMask"></param>
        /// <param name="rotation"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
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
        public static float AngleDifference(float a, float b)
        {
            float angleA = Mathf.Abs(a - b);
            return Mathf.Min(angleA, 360 - angleA);
        }

        //public static float AngleInverseLerp(float lower, float upper, float t)
        //{
        //    lower = (lower % CIRCLE_DEGREE + CIRCLE_DEGREE) % CIRCLE_DEGREE;
        //    upper = (upper % CIRCLE_DEGREE + CIRCLE_DEGREE) % CIRCLE_DEGREE;
        //    t = (t % CIRCLE_DEGREE + CIRCLE_DEGREE) % CIRCLE_DEGREE;
        //    if (t < lower)
        //    {
        //        return 0;
        //    }
        //    if (t > upper)
        //    {
        //        return 1;
        //    }
        //}

        //bool IsPointInPolygon(int nvert, float[] vertx, float[] verty, float testx, float testy)
        //{
        //    int i;
        //    int j;
        //    bool c = false;
        //    for (i = 0, j = nvert - 1; i < nvert; j = i++)
        //    {
        //        if (((verty[i] > testy) != (verty[j] > testy)) &&
        //         (testx < (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
        //            c = !c;
        //    }
        //    return c;
        //}
        /// <summary>
        /// Get all point inside a polygon
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static List<Vector2Int> GetPointsInPolygon(Polygon polygon)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            PolygonTriangulation polygonTriangulation = new PolygonTriangulation();
            polygonTriangulation.SetPoints(polygon.ToList());
            polygonTriangulation.Triangulate();
            var result = polygonTriangulation.Result;

            Vector3[] triangle = new Vector3[3];
            for (int i = 0; i < result.Count; i += 3)
            {
                triangle[0] = result[i];
                triangle[1] = result[i + 1];
                triangle[2] = result[i + 2];
                int xMin = Mathf.FloorToInt(triangle.Min(p => p.x));
                int xMax = Mathf.CeilToInt(triangle.Max(p => p.x));
                int yMin = Mathf.FloorToInt(triangle.Min(p => p.y));
                int yMax = Mathf.CeilToInt(triangle.Max(p => p.y));
                RectInt rect = new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);

                for (int x = xMin; x <= xMax; x++)
                    for (int y = yMin; y <= yMax; y++)
                    {
                        var item = new Vector2Int(x, y);
                        if (IsPointInPolygon(triangle, (Vector2)item))
                        {
                            points.Add(item);
                        }
                    }
            }
            return points;
        }

        [Obsolete("use IsPointInPolygon instead")]
        public static bool IsPointInTriangle(List<Vector2> triangle, Vector2 point)
        {
            return IsPointInTriangle(triangle[0], triangle[1], triangle[2], point);
        }

        [Obsolete("use IsPointInPolygon instead")]
        public static bool IsPointInTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 point)
        {
            Vector2 c = A - B;
            Vector2 a = C - B;
            Vector2 x = A - point;
            float angleAlpha = Vector2.Angle(x, c);
            float angleB = Vector2.Angle(a, c);
            float angleBeta = 180 - angleAlpha - angleB;
            var dist = c.magnitude / Mathf.Sin(angleBeta) * Mathf.Sin(angleB);
            return x.magnitude < dist;
        }
    }
}