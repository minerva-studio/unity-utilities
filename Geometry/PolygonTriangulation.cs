using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minerva.Module.Geomtry
{
    /// <summary>
    /// Implementation of Triangulation by Ear Clipping
    /// <br/>
    /// by David Eberly
    /// <br/>
    /// <see cref="https://github.com/NMO13/earclipper"/>
    /// </summary>
    public sealed class PolygonTriangulation
    {
        private Polygon _mainPointList;
        private List<Polygon> _holes;
        private Vector3 Normal;
        Dictionary<Vector3, List<ConnectionEdge>> dict = new();

        public List<Vector3> Result { get; private set; }

        public void SetPoints(List<Vector3> points, List<List<Vector3>> holes = null, Vector3 normal = default)
        {
            if (points == null || points.Count < 3)
            {
                throw new ArgumentException("No list or an empty list passed");
            }
            if (normal == default)
                CalcNormal(points);
            else
            {
                Normal = normal;
            }
            _mainPointList = new Polygon();
            LinkAndAddToList(_mainPointList, points);

            if (holes != null)
            {
                _holes = new List<Polygon>();
                for (int i = 0; i < holes.Count; i++)
                {
                    Polygon p = new Polygon();
                    LinkAndAddToList(p, holes[i]);
                    _holes.Add(p);
                }
            }
            Result = new List<Vector3>();
        }

        // calculating normal using Newell's method
        private void CalcNormal(List<Vector3> points)
        {
            Vector3 normal = Vector3.zero;
            for (var i = 0; i < points.Count; i++)
            {
                var j = (i + 1) % points.Count;
                normal.x += (points[i].y - points[j].y) * (points[i].z + points[j].z);
                normal.y += (points[i].z - points[j].z) * (points[i].x + points[j].x);
                normal.z += (points[i].x - points[j].x) * (points[i].y + points[j].y);
            }
            Normal = normal;
        }

        private void LinkAndAddToList(Polygon polygon, List<Vector3> points)
        {
            ConnectionEdge prev = null, first = null;
            Dictionary<Vector3, Vector3> pointsHashSet = new Dictionary<Vector3, Vector3>();
            int pointCount = 0;
            for (int i = 0; i < points.Count; i++)
            {
                // we don't wanna have duplicates
                Vector3 p0;
                if (pointsHashSet.ContainsKey(points[i]))
                {
                    p0 = pointsHashSet[points[i]];
                }
                else
                {
                    p0 = points[i];
                    pointsHashSet.Add(p0, p0);
                    List<ConnectionEdge> list = new List<ConnectionEdge>();
                    dict.Add(p0, list);
                    pointCount++;
                }
                ConnectionEdge current = new ConnectionEdge(p0, polygon, dict[p0]);

                first = i == 0 ? current : first; // remember first

                if (prev != null)
                {
                    prev.Next = current;
                }
                current.Prev = prev;
                prev = current;
            }
            first.Prev = prev;
            prev.Next = first;
            polygon.Start = first;
            polygon.PointCount = pointCount;
        }

        public void Triangulate()
        {
            if (Normal.Equals(Vector3.zero))
                throw new Exception("The input is not a valid polygon");
            if (_holes != null && _holes.Count > 0)
            {
                ProcessHoles();
            }

            List<ConnectionEdge> nonConvexPoints = FindNonConvexPoints(_mainPointList);

            if (nonConvexPoints.Count == _mainPointList.PointCount)
                throw new ArgumentException("The triangle input is not valid");

            while (_mainPointList.PointCount > 2)
            {
                bool guard = false;
                foreach (var cur in _mainPointList.GetPolygonCirculator())
                {
                    if (!IsConvex(cur))
                        continue;

                    if (!IsPointInTriangle(cur.Prev.Origin, cur.Origin, cur.Next.Origin, nonConvexPoints))
                    {
                        // cut off ear
                        guard = true;
                        Result.Add(cur.Prev.Origin);
                        Result.Add(cur.Origin);
                        Result.Add(cur.Next.Origin);

                        // Check if prev and next are still nonconvex. If not, then remove from non convex list
                        if (IsConvex(cur.Prev))
                        {
                            int index = nonConvexPoints.FindIndex(x => x == cur.Prev);
                            if (index >= 0)
                                nonConvexPoints.RemoveAt(index);
                        }
                        if (IsConvex(cur.Next))
                        {
                            int index = nonConvexPoints.FindIndex(x => x == cur.Next);
                            if (index >= 0)
                                nonConvexPoints.RemoveAt(index);
                        }
                        _mainPointList.Remove(cur, dict[cur.Origin]);
                        break;
                    }
                }

                if (PointsOnLine(_mainPointList))
                    break;
                if (!guard)
                {
                    throw new Exception("No progression. The input must be wrong");
                }
            }
        }

        private bool PointsOnLine(Polygon pointList)
        {
            foreach (var connectionEdge in pointList.GetPolygonCirculator())
            {
                if (Misc.GetOrientation(connectionEdge.Prev.Origin, connectionEdge.Origin, connectionEdge.Next.Origin, Normal) != 0)
                    return false;
            }
            return true;
        }

        private bool IsConvex(ConnectionEdge curPoint)
        {
            int orientation = Misc.GetOrientation(curPoint.Prev.Origin, curPoint.Origin, curPoint.Next.Origin, Normal);
            return orientation == 1;
        }

        private void ProcessHoles()
        {
            for (int h = 0; h < _holes.Count; h++)
            {
                List<Polygon> polygons = new List<Polygon>();
                polygons.Add(_mainPointList);
                polygons.AddRange(_holes);
                ConnectionEdge M, P;
                GetVisiblePoints(h + 1, polygons, out M, out P);
                if (M.Origin.Equals(P.Origin))
                    throw new Exception();

                var insertionEdge = P;
                InsertNewEdges(insertionEdge, M);
                _holes.RemoveAt(h);
                h--;
            }
        }

        private void InsertNewEdges(ConnectionEdge insertionEdge, ConnectionEdge m)
        {
            insertionEdge.Polygon.PointCount += m.Polygon.PointCount;
            var cur = m;
            var forwardEdge = new ConnectionEdge(insertionEdge.Origin, insertionEdge.Polygon, dict[insertionEdge.Origin]);
            forwardEdge.Prev = insertionEdge.Prev;
            forwardEdge.Prev.Next = forwardEdge;
            forwardEdge.Next = m;
            forwardEdge.Next.Prev = forwardEdge;
            var end = insertionEdge;
            ConnectionEdge prev = null;
            do
            {
                cur.Polygon = insertionEdge.Polygon;
                prev = cur;
                cur = cur.Next;
            } while (m != cur);
            var backEdge = new ConnectionEdge(cur.Origin, insertionEdge.Polygon, dict[cur.Origin]);
            cur = prev;
            cur.Next = backEdge;
            backEdge.Prev = cur;
            backEdge.Next = end;
            end.Prev = backEdge;
        }

        private void GetVisiblePoints(int holeIndex, List<Polygon> polygons, out ConnectionEdge M, out ConnectionEdge P)
        {
            M = FindLargest(polygons[holeIndex]);

            var direction = Vector3.Cross(polygons[holeIndex].Start.Next.Origin - polygons[holeIndex].Start.Origin, Normal);
            var I = FindPointI(M, polygons, holeIndex, direction);

            Vector3 res;
            if (polygons[I.PolyIndex].Contains(I.I, out res))
            {
                var incidentEdges = dict[res];
                //(List<ConnectionEdge>)res.DynamicProperties.GetValue(PropertyConstants.IncidentEdges);
                foreach (var connectionEdge in incidentEdges)
                {
                    if (Misc.IsBetween(connectionEdge.Origin, connectionEdge.Next.Origin, connectionEdge.Prev.Origin, M.Origin, Normal) == 1)
                    {
                        P = connectionEdge;
                        return;
                    }
                }
                throw new Exception();
            }
            else
            {
                P = FindVisiblePoint(I, polygons, M, direction);
            }
        }

        private ConnectionEdge FindVisiblePoint(Candidate I, List<Polygon> polygons, ConnectionEdge M, Vector3 direction)
        {
            ConnectionEdge P = null;

            if (I.Origin.Origin.x > I.Origin.Next.Origin.x)
            {
                P = I.Origin;
            }
            else
            {
                P = I.Origin.Next;
            }

            List<ConnectionEdge> nonConvexPoints = FindNonConvexPoints(polygons[I.PolyIndex]);


            nonConvexPoints.Remove(P);

            var m = M.Origin;
            var i = I.I;
            var p = P.Origin;
            List<ConnectionEdge> candidates = new List<ConnectionEdge>();

            // invert i and p if triangle is oriented CW
            if (Misc.GetOrientation(m, i, p, Normal) == -1)
            {
                var tmp = i;
                i = p;
                p = tmp;
            }

            foreach (var nonConvexPoint in nonConvexPoints)
            {
                if (Misc.PointInOrOnTriangle(m, i, p, nonConvexPoint.Origin, Normal))
                {
                    candidates.Add(nonConvexPoint);
                }
            }
            if (candidates.Count == 0)
                return P;
            return FindMinimumAngle(candidates, m, direction);
        }

        private ConnectionEdge FindMinimumAngle(List<ConnectionEdge> candidates, Vector3 M, Vector3 direction)
        {
            float angle = -float.MaxValue;
            ConnectionEdge result = null;
            foreach (var R in candidates)
            {
                var a = direction;
                var b = R.Origin - M;
                var num = Vector3.Dot(a, b) * Vector3.Dot(a, b);
                var denom = Vector3.Dot(b, b);
                var res = num / denom;
                if (res > angle)
                {
                    result = R;
                    angle = res;
                }
            }
            return result;
        }

        private Candidate FindPointI(ConnectionEdge M, List<Polygon> polygons, int holeIndex, Vector3 direction)
        {
            Candidate candidate = new Candidate();
            for (int i = 0; i < polygons.Count; i++)
            {
                if (i == holeIndex) // Don't test the hole with itself
                    continue;
                foreach (var connectionEdge in polygons[i].GetPolygonCirculator())
                {
                    float rayDistanceSquared;
                    Vector3 intersectionPoint;

                    if (RaySegmentIntersection(out intersectionPoint, out rayDistanceSquared, M.Origin, direction, connectionEdge.Origin,
                        connectionEdge.Next.Origin, direction))
                    {
                        if (rayDistanceSquared == candidate.currentDistance)  // if this is an M/I edge, then both edge and his twin have the same distance; we take the edge where the point is on the left side
                        {
                            if (Misc.GetOrientation(connectionEdge.Origin, connectionEdge.Next.Origin, M.Origin, Normal) == 1)
                            {
                                candidate.currentDistance = rayDistanceSquared;
                                candidate.Origin = connectionEdge;
                                candidate.PolyIndex = i;
                                candidate.I = intersectionPoint;
                            }
                        }
                        else if (rayDistanceSquared < candidate.currentDistance)
                        {
                            candidate.currentDistance = rayDistanceSquared;
                            candidate.Origin = connectionEdge;
                            candidate.PolyIndex = i;
                            candidate.I = intersectionPoint;
                        }
                    }
                }

            }
            return candidate;
        }

        private ConnectionEdge FindLargest(Polygon testHole)
        {
            float maximum = 0;
            ConnectionEdge maxEdge = null;
            Vector3 v0 = testHole.Start.Origin;
            Vector3 v1 = testHole.Start.Next.Origin;
            foreach (var connectionEdge in testHole.GetPolygonCirculator())
            {
                // we take the first two points as a reference line

                if (Misc.GetOrientation(v0, v1, connectionEdge.Origin, Normal) < 0)
                {
                    var r = Misc.PointLineDistance(v0, v1, connectionEdge.Origin);
                    if (r > maximum)
                    {
                        maximum = r;
                        maxEdge = connectionEdge;
                    }
                }
            }
            if (maxEdge == null)
                return testHole.Start;
            return maxEdge;
        }

        private bool IsPointInTriangle(Vector3 prevPoint, Vector3 curPoint, Vector3 nextPoint, List<ConnectionEdge> nonConvexPoints)
        {
            foreach (var nonConvexPoint in nonConvexPoints)
            {
                if (nonConvexPoint.Origin == prevPoint || nonConvexPoint.Origin == curPoint || nonConvexPoint.Origin == nextPoint)
                    continue;
                if (Misc.PointInOrOnTriangle(prevPoint, curPoint, nextPoint, nonConvexPoint.Origin, Normal))
                    return true;
            }
            return false;
        }

        private List<ConnectionEdge> FindNonConvexPoints(Polygon p)
        {
            List<ConnectionEdge> resultList = new List<ConnectionEdge>();
            foreach (var connectionEdge in p.GetPolygonCirculator())
            {
                if (Misc.GetOrientation(connectionEdge.Prev.Origin, connectionEdge.Origin, connectionEdge.Next.Origin, Normal) != 1)
                    resultList.Add(connectionEdge);
            }
            return resultList;
        }

        public bool RaySegmentIntersection(out Vector3 intersection, out float distanceSquared, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint3, Vector3 linePoint4, Vector3 direction)
        {
            var lineVec2 = linePoint4 - linePoint3;
            Vector3 lineVec3 = linePoint3 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            var res = Misc.PointLineDistance(linePoint3, linePoint4, linePoint1);
            if (res == 0) // line and ray are collinear
            {
                var p = linePoint1 + lineVec1;
                var res2 = Misc.PointLineDistance(linePoint3, linePoint4, p);
                if (res2 == 0)
                {
                    var s = linePoint3 - linePoint1;
                    if (s.x == direction.x && s.y == direction.y && s.z == direction.z)
                    {
                        intersection = linePoint3;
                        distanceSquared = s.sqrMagnitude;
                        return true;
                    }
                }
            }
            //is coplanar, and not parallel
            if (/*planarFactor == 0.0f && */crossVec1and2.sqrMagnitude > 0)
            {
                var s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                if (s >= 0)
                {
                    intersection = linePoint1 + lineVec1 * s;
                    distanceSquared = (lineVec1 * s).sqrMagnitude;
                    if ((intersection - linePoint3).sqrMagnitude + (intersection - linePoint4).sqrMagnitude <=
                        lineVec2.sqrMagnitude)
                        return true;
                }
            }
            intersection = Vector3.zero;
            distanceSquared = 0;
            return false;
        }



        internal sealed class Polygon
        {
            internal ConnectionEdge Start;
            internal int PointCount = 0;

            internal IEnumerable<ConnectionEdge> GetPolygonCirculator()
            {
                if (Start == null) { yield break; }
                var h = Start;
                do
                {
                    yield return h;
                    h = h.Next;
                }
                while (h != Start);
            }

            internal void Remove(ConnectionEdge cur, List<ConnectionEdge> incidentEdges)
            {
                cur.Prev.Next = cur.Next;
                cur.Next.Prev = cur.Prev;
                //var incidentEdges = (List<ConnectionEdge>)cur.Origin.DynamicProperties.GetValue(PropertyConstants.IncidentEdges);
                int index = incidentEdges.FindIndex(x => x.Equals(cur));
                //Debug.Log(index >= 0);
                incidentEdges.RemoveAt(index);
                if (incidentEdges.Count == 0)
                    PointCount--;
                if (cur == Start)
                    Start = cur.Prev;
            }

            public bool Contains(Vector3 vector2M, out Vector3 res)
            {
                foreach (var connectionEdge in GetPolygonCirculator())
                {
                    if (connectionEdge.Origin.Equals(vector2M))
                    {
                        res = connectionEdge.Origin;
                        return true;
                    }
                }
                res = default;
                return false;
            }
        }

        internal sealed class Candidate
        {
            internal float currentDistance = float.MaxValue;
            internal Vector3 I;
            internal ConnectionEdge Origin;
            internal int PolyIndex;
        }

        internal sealed class ConnectionEdge
        {
            public bool Equals(ConnectionEdge other)
            {
                return Next.Origin.Equals(other.Next.Origin) && Origin.Equals(other.Origin);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((ConnectionEdge)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Next.Origin != null ? Next.Origin.GetHashCode() : 0) * 397 ^ (Origin != null ? Origin.GetHashCode() : 0);
                }
            }

            internal Vector3 Origin { get; private set; }
            internal ConnectionEdge Prev;
            internal ConnectionEdge Next;
            internal Polygon Polygon { get; set; }

            public ConnectionEdge(Vector3 p0, Polygon parentPolygon, List<ConnectionEdge> incidentEdge)
            {
                Origin = p0;
                Polygon = parentPolygon;
                //AddIncidentEdge(this);
                incidentEdge.Add(this);
            }

            public override string ToString()
            {
                return "Origin: " + Origin + " Next: " + Next.Origin;
            }

            internal void AddIncidentEdge(ConnectionEdge next)
            {
                //var list = (List<ConnectionEdge>)Origin.DynamicProperties.GetValue(PropertyConstants.IncidentEdges);
                //list.Add(next);
            }


        }

        internal static class Misc
        {
            public static int GetOrientation(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 normal)
            {
                var res = Vector3.Cross(v0 - v1, v2 - v1);
                if (res.magnitude == 0)
                    return 0;
                if (Mathf.Sign(res.x) != Mathf.Sign(normal.x) || Mathf.Sign(res.y) != Mathf.Sign(normal.y) || Mathf.Sign(res.z) != Mathf.Sign(normal.z))
                    return 1;
                return -1;
            }

            // Is testPoint between a and b in ccw order?
            // > 0 if strictly yes
            // < 0 if strictly no
            // = 0 if testPoint lies either on a or on b
            public static int IsBetween(Vector3 Origin, Vector3 a, Vector3 b, Vector3 testPoint, Vector3 normal)
            {
                var psca = GetOrientation(Origin, a, testPoint, normal);
                var pscb = GetOrientation(Origin, b, testPoint, normal);

                // where does b in relation to a lie? Left, right or collinear?
                var psb = GetOrientation(Origin, a, b, normal);
                if (psb > 0) // left
                {
                    // if left then testPoint lies between a and b iff testPoint left of a AND testPoint right of b
                    if (psca > 0 && pscb < 0)
                        return 1;
                    if (psca == 0)
                    {
                        var t = a - Origin;
                        var t2 = testPoint - Origin;
                        if (Mathf.Sign(t.x) != Mathf.Sign(t2.x) || Mathf.Sign(t.y) != Mathf.Sign(t2.y))
                            return -1;
                        return 0;
                    }
                    else if (pscb == 0)
                    {
                        var t = b - Origin;
                        var t2 = testPoint - Origin;
                        if (Mathf.Sign(t.x) != Mathf.Sign(t2.x) || Mathf.Sign(t.y) != Mathf.Sign(t2.y))
                            return -1;
                        return 0;
                    }
                }
                else if (psb < 0) // right
                {
                    // if right then testPoint lies between a and b iff testPoint left of a OR testPoint right of b
                    if (psca > 0 || pscb < 0)
                        return 1;
                    if (psca == 0)
                    {
                        var t = a - Origin;
                        var t2 = testPoint - Origin;
                        if (Mathf.Sign(t.x) != Mathf.Sign(t2.x) || Mathf.Sign(t.y) != Mathf.Sign(t2.y))
                            return 1;
                        return 0;
                    }
                    else if (pscb == 0)
                    {
                        var t = b - Origin;
                        var t2 = testPoint - Origin;
                        if (Mathf.Sign(t.x) != Mathf.Sign(t2.x) || Mathf.Sign(t.y) != Mathf.Sign(t2.y))
                            return 1;
                        return 0;
                    }
                }
                else if (psb == 0)
                {
                    if (psca > 0)
                        return 1;
                    else if (psca < 0)
                        return -1;
                    else
                        return 0;
                }
                return -1;
            }

            public static bool PointInOrOnTriangle(Vector3 prevPoint, Vector3 curPoint, Vector3 nextPoint, Vector3 nonConvexPoint, Vector3 normal)
            {
                var res0 = GetOrientation(prevPoint, nonConvexPoint, curPoint, normal);
                var res1 = GetOrientation(curPoint, nonConvexPoint, nextPoint, normal);
                var res2 = GetOrientation(nextPoint, nonConvexPoint, prevPoint, normal);
                return res0 != 1 && res1 != 1 && res2 != 1;
            }

            public static float PointLineDistance(Vector3 p1, Vector3 p2, Vector3 p3)
            {
                return Vector3.Cross(p2 - p1, p3 - p1).sqrMagnitude;
            }

            internal enum PropertyConstants
            {
                Marked, FaceListIndex, Median, IncidentEdges, HeVertexIndex
            }
        }
    }
}