using System;
using System.Globalization;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// a 3d version of <see cref="RectInt"/>
    /// <br/>
    /// use <see cref="BoundsInt"/> instead
    /// </summary>
    [Serializable]
    [Obsolete]
    public struct CubeInt
    {
        [SerializeField] private int m_XMin;
        [SerializeField] private int m_YMin;
        [SerializeField] private int m_ZMin;
        [SerializeField] private int m_Width;
        [SerializeField] private int m_Height;
        [SerializeField] private int m_Depth;

        //
        // 摘要:
        //     Shorthand for writing new CubeInt(0,0,0,0,0,0).
        public static CubeInt zero => new CubeInt(0, 0, 0, 0, 0, 0);

        //
        // 摘要:
        //     The X coordinate of the rectangle.
        public int x
        {
            get
            {
                return m_XMin;
            }
            set
            {
                m_XMin = value;
            }
        }

        //
        // 摘要:
        //     The Y coordinate of the rectangle.
        public int y
        {
            get
            {
                return m_YMin;
            }
            set
            {
                m_YMin = value;
            }
        }

        //
        // 摘要:
        //     The Z coordinate of the rectangle.
        public int z
        {
            get
            {
                return m_ZMin;
            }
            set
            {
                m_ZMin = value;
            }
        }

        //
        // 摘要:
        //     The X and Y position of the rectangle.
        public Vector3Int position
        {
            get
            {
                return new Vector3Int(m_XMin, m_YMin, m_ZMin);
            }
            set
            {
                m_XMin = value.x;
                m_YMin = value.y;
                m_ZMin = value.z;
            }
        }

        //
        // 摘要:
        //     The position of the center of the rectangle.
        public Vector3 center
        {
            get
            {
                return new Vector3(x + m_Width / 2f, y + m_Height / 2f, z + m_Depth / 2f);
            }
        }

        //
        // 摘要:
        //     The position of the minimum corner of the rectangle.
        public Vector3Int min
        {
            get
            {
                return new Vector3Int(xMin, yMin, zMin);
            }
            set
            {
                xMin = value.x;
                yMin = value.y;
                zMin = value.z;
            }
        }

        //
        // 摘要:
        //     The position of the maximum corner of the rectangle.
        public Vector3Int max
        {
            get
            {
                return new Vector3Int(xMax, yMax, zMax);
            }
            set
            {
                xMax = value.x;
                yMax = value.y;
                zMax = value.z;
            }
        }

        //
        // 摘要:
        //     The width of the rectangle, measured from the X position.
        public int width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = value;
            }
        }

        //
        // 摘要:
        //     The height of the rectangle, measured from the Y position.
        public int height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = value;
            }
        }

        //
        // 摘要:
        //     The depth of the rectangle, measured from the Z position.
        public int depth
        {
            get
            {
                return m_Depth;
            }
            set
            {
                m_Depth = value;
            }
        }

        //
        // 摘要:
        //     The width and height of the rectangle.
        public Vector3Int size
        {
            get
            {
                return new Vector3Int(m_Width, m_Height, m_Depth);
            }
            set
            {
                m_Width = value.x;
                m_Height = value.y;
                m_Depth = value.z;
            }
        }

        //
        // 摘要:
        //     The minimum X coordinate of the rectangle.
        public int xMin
        {
            get
            {
                return m_XMin;
            }
            set
            {
                int num = xMax;
                m_XMin = value;
                m_Width = num - m_XMin;
            }
        }

        //
        // 摘要:
        //     The minimum Y coordinate of the rectangle.
        public int yMin
        {
            get
            {
                return m_YMin;
            }
            set
            {
                int num = yMax;
                m_YMin = value;
                m_Height = num - m_YMin;
            }
        }

        //
        // 摘要:
        //     The minimum Z coordinate of the rectangle.
        public int zMin
        {
            get
            {
                return m_ZMin;
            }
            set
            {
                int num = zMax;
                m_ZMin = value;
                m_Depth = num - m_ZMin;
            }
        }

        //
        // 摘要:
        //     The maximum X coordinate of the rectangle.
        public int xMax
        {
            get
            {
                return m_Width + m_XMin;
            }
            set
            {
                m_Width = value - m_XMin;
            }
        }

        //
        // 摘要:
        //     The maximum Y coordinate of the rectangle.
        public int yMax
        {
            get
            {
                return m_Height + m_YMin;
            }
            set
            {
                m_Height = value - m_YMin;
            }
        }

        //
        // 摘要:
        //     The maximum Z coordinate of the rectangle.
        public int zMax
        {
            get
            {
                return m_Depth + m_ZMin;
            }
            set
            {
                m_Depth = value - m_ZMin;
            }
        }


        //
        // 摘要:
        //     Creates a new rectangle.
        //
        // 参数:
        //   x:
        //     The X value the rect is measured from.
        //
        //   y:
        //     The Y value the rect is measured from.
        //
        //   width:
        //     The width of the rectangle.
        //
        //   height:
        //     The height of the rectangle.
        public CubeInt(int x, int y, int z, int width, int height, int depth)
        {
            m_XMin = x;
            m_YMin = y;
            m_ZMin = z;
            m_Width = width;
            m_Height = height;
            m_Depth = depth;
        }

        //
        // 摘要:
        //     Creates a rectangle given a size and position.
        //
        // 参数:
        //   position:
        //     The position of the minimum corner of the rect.
        //
        //   size:
        //     The width and height of the rect.
        public CubeInt(Vector3Int position, Vector3Int size)
        {
            m_XMin = position.x;
            m_YMin = position.y;
            m_ZMin = position.z;
            m_Width = size.x;
            m_Height = size.y;
            m_Depth = size.z;
        }

        //
        // 参数:
        //   source:
        public CubeInt(CubeInt source)
        {
            m_XMin = source.m_XMin;
            m_YMin = source.m_YMin;
            m_ZMin = source.m_ZMin;
            m_Width = source.m_Width;
            m_Height = source.m_Height;
            m_Depth = source.m_Depth;
        }

        //
        // 摘要:
        //     Creates a rectangle from min/max coordinate values.
        //
        // 参数:
        //   xmin:
        //     The minimum X coordinate.
        //
        //   ymin:
        //     The minimum Y coordinate.
        //
        //   xmax:
        //     The maximum X coordinate.
        //
        //   ymax:
        //     The maximum Y coordinate.
        //
        // 返回结果:
        //     A rectangle matching the specified coordinates.
        public static CubeInt MinMaxCubeInt(int xmin, int ymin, int zmin, int xmax, int ymax, int zmax)
        {
            return new CubeInt(xmin, ymin, zmin, xmax - xmin, ymax - ymin, zmax - zmin);
        }

        //
        // 摘要:
        //     Set components of an existing CubeInt.
        //
        // 参数:
        //   x:
        //
        //   y:
        //
        //   width:
        //
        //   height:
        public void Set(int x, int y, int width, int height)
        {
            m_XMin = x;
            m_YMin = y;
            m_Width = width;
            m_Height = height;
        }

        //
        // 摘要:
        //     Returns true if the x and y components of point is a point inside this rectangle.
        //     If allowInverse is present and true, the width and height of the CubeInt are allowed
        //     to take negative values (ie, the min value is greater than the max), and the
        //     test will still work.
        //
        // 参数:
        //   point:
        //     Point to test.
        //
        //   allowInverse:
        //     Does the test allow the CubeInt's width and height to be negative?
        //
        // 返回结果:
        //     True if the point lies within the specified rectangle.
        public bool Contains(Vector3Int point)
        {
            return point.x >= xMin && point.x < xMax && point.y >= yMin && point.y < yMax && point.z >= zMin && point.z < zMax;
        }

        //
        // 摘要:
        //     Returns true if the x and y components of point is a point inside this rectangle.
        //     If allowInverse is present and true, the width and height of the CubeInt are allowed
        //     to take negative values (ie, the min value is greater than the max), and the
        //     test will still work.
        //
        // 参数:
        //   point:
        //     Point to test.
        //
        //   allowInverse:
        //     Does the test allow the CubeInt's width and height to be negative?
        //
        // 返回结果:
        //     True if the point lies within the specified rectangle.
        public bool Contains(Vector3Int point, bool allowInverse)
        {
            if (!allowInverse)
            {
                return Contains(point);
            }

            bool flag = width < 0f && point.x <= xMin && point.x > xMax || width >= 0f && point.x >= xMin && point.x < xMax;
            bool flag2 = height < 0f && point.y <= yMin && point.y > yMax || height >= 0f && point.y >= yMin && point.y < yMax;
            bool flag3 = depth < 0f && point.z <= zMin && point.z > zMax || depth >= 0f && point.z >= zMin && point.z < zMax;
            return flag && flag2 && flag3;
        }

        private static CubeInt OrderMinMax(CubeInt rect)
        {
            if (rect.xMin > rect.xMax)
            {
                int num = rect.xMin;
                rect.xMin = rect.xMax;
                rect.xMax = num;
            }

            if (rect.yMin > rect.yMax)
            {
                int num2 = rect.yMin;
                rect.yMin = rect.yMax;
                rect.yMax = num2;
            }

            if (rect.zMin > rect.zMax)
            {
                int num3 = rect.zMin;
                rect.zMin = rect.zMax;
                rect.zMax = num3;
            }

            return rect;
        }

        //
        // 摘要:
        //     Returns true if the other rectangle overlaps this one. If allowInverse is present
        //     and true, the widths and heights of the CubeInts are allowed to take negative values
        //     (ie, the min value is greater than the max), and the test will still work.
        //
        // 参数:
        //   other:
        //     Other rectangle to test overlapping with.
        //
        //   allowInverse:
        //     Does the test allow the widths and heights of the CubeInts to be negative?
        public bool Overlaps(CubeInt other)
        {
            return other.xMax > xMin && other.xMin < xMax && other.yMax > yMin && other.yMin < yMax && other.zMax > zMin && other.zMin < zMax;
        }

        //
        // 摘要:
        //     Returns true if the other rectangle overlaps this one. If allowInverse is present
        //     and true, the widths and heights of the CubeInts are allowed to take negative values
        //     (ie, the min value is greater than the max), and the test will still work.
        //
        // 参数:
        //   other:
        //     Other rectangle to test overlapping with.
        //
        //   allowInverse:
        //     Does the test allow the widths and heights of the CubeInts to be negative?
        public bool Overlaps(CubeInt other, bool allowInverse)
        {
            CubeInt rect = this;
            if (allowInverse)
            {
                rect = OrderMinMax(rect);
                other = OrderMinMax(other);
            }

            return rect.Overlaps(other);
        }

        //
        // 摘要:
        //     Returns the normalized coordinates cooresponding the the point.
        //
        // 参数:
        //   rectangle:
        //     CubeIntangle to get normalized coordinates inside.
        //
        //   point:
        //     A point inside the rectangle to get normalized coordinates for.
        public static Vector3 PointToNormalized(CubeInt rectangle, Vector3Int point)
        {
            return new Vector3(Mathf.InverseLerp(rectangle.x, rectangle.xMax, point.x), Mathf.InverseLerp(rectangle.y, rectangle.yMax, point.y), Mathf.InverseLerp(rectangle.z, rectangle.zMax, point.z));
        }

        public static bool operator !=(CubeInt lhs, CubeInt rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(CubeInt lhs, CubeInt rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.width == rhs.width && lhs.height == rhs.height && lhs.depth == rhs.depth;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ width.GetHashCode() << 2 ^ y.GetHashCode() >> 2 ^ height.GetHashCode() >> 1 ^ z.GetHashCode() ^ depth.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is CubeInt))
            {
                return false;
            }

            return Equals((CubeInt)other);
        }

        public bool Equals(CubeInt other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && width.Equals(other.width) && height.Equals(other.height) && depth.Equals(other.depth);
        }

        //
        // 摘要:
        //     Returns a formatted string for this CubeInt.
        //
        // 参数:
        //   format:
        //     A numeric format string.
        //
        //   formatProvider:
        //     An object that specifies culture-specific formatting.
        public override string ToString()
        {
            return ToString(null, null);
        }

        //
        // 摘要:
        //     Returns a formatted string for this CubeInt.
        //
        // 参数:
        //   format:
        //     A numeric format string.
        //
        //   formatProvider:
        //     An object that specifies culture-specific formatting.
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        //
        // 摘要:
        //     Returns a formatted string for this CubeInt.
        //
        // 参数:
        //   format:
        //     A numeric format string.
        //
        //   formatProvider:
        //     An object that specifies culture-specific formatting.
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "F2";
            }

            if (formatProvider == null)
            {
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            }

            return Format("(x:{0}, y:{1}, z:{2}, width:{3}, height:{4}, depth:{4})", x.ToString(format, formatProvider), y.ToString(format, formatProvider), z.ToString(format, formatProvider), width.ToString(format, formatProvider), height.ToString(format, formatProvider), depth.ToString(format, formatProvider));
        }
        public static string Format(string fmt, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture.NumberFormat, fmt, args);
        }
    }
}