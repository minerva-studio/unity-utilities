using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// a utility class for more vector support 
    /// Author : Wendi Cai 
    /// </summary>
#if ENABLE_BURST_AOT
    [BurstCompile(CompileSynchronously = true)]
#endif
    public static class VectorUtility
    {
        public const char DEFAULT_SEPERATOR = ',';

        #region Vector Parsing
        /**
         * Update: Avoiding allocation in parsing string into vectors
         */
        #region split
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ReadOnlySpan<char> ReadSpan(string vetorLike)
        {
            var span = vetorLike.AsSpan().Trim();
            // Remove the parentheses
            if (span.StartsWith("(") && span.EndsWith(")")) { span = span[1..^1]; }
            return span;
        }

        static bool SplitToFloats(ReadOnlySpan<char> vectorLike, char seperator, Span<float> arr)
        {
            var span = vectorLike;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int index = span.IndexOf(seperator);
                if (index < 0) return false;
                arr[i] = float.Parse(span[..index], provider: CultureInfo.InvariantCulture);
                span = span[(index + 1)..];
            }
            if (span.Length <= 0) return false;
            arr[^1] = float.Parse(span, provider: CultureInfo.InvariantCulture);
            return true;
        }

        static bool SplitToIntegers(ReadOnlySpan<char> vectorLike, char seperator, Span<int> arr)
        {
            var span = vectorLike;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int index = span.IndexOf(seperator);
                if (index < 0) return false;
                arr[i] = int.Parse(span[..index], provider: CultureInfo.InvariantCulture);
                span = span[(index + 1)..];
            }
            if (span.Length <= 0) return false;
            arr[^1] = int.Parse(span, provider: CultureInfo.InvariantCulture);
            return true;
        }
        #endregion  

        public static Vector3 ToVector3(string stringVector, char seperator = DEFAULT_SEPERATOR)
        {
            ToVector3(stringVector, seperator, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ToVector3(string stringVector, char seperator, out Vector3 vector)
        {
            var span = ReadSpan(stringVector);
            int size = 3;
            Span<float> arr = stackalloc float[size];
            if (SplitToFloats(span, seperator, arr))
            {
                vector = new Vector3(arr[0], arr[1], arr[2]);
                return;
            }
            throw new InvalidDataException();
        }

        public static bool TryParseVector3(string defaultValue, out Vector3 val, char seperator = DEFAULT_SEPERATOR)
        {
            try
            {
                val = ToVector3(defaultValue, seperator);
                return true;
            }
            catch (Exception)
            {
                val = default;
                return false;
            }
        }





        public static Vector4 ToVector4(string stringVector, char seperator = DEFAULT_SEPERATOR)
        {
            ToVector4(stringVector, seperator, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ToVector4(string stringVector, char seperator, out Vector4 vector)
        {
            var span = ReadSpan(stringVector);
            int size = 4;
            Span<float> arr = stackalloc float[size];
            if (SplitToFloats(span, seperator, arr))
            {
                vector = new Vector4(arr[0], arr[1], arr[2], arr[3]);
                return;
            }
            throw new InvalidDataException();
        }

        public static bool TryParseVector4(string defaultValue, out Vector4 val, char seperator = DEFAULT_SEPERATOR)
        {
            try
            {
                val = ToVector4(defaultValue, seperator);
                return true;
            }
            catch (Exception)
            {
                val = default;
                return false;
            }
        }




        public static Vector2 ToVector2(string stringVector, char seperator = DEFAULT_SEPERATOR)
        {
            ToVector2(stringVector, seperator, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ToVector2(string stringVector, char seperator, out Vector2 vector)
        {
            var span = ReadSpan(stringVector);
            int size = 2;
            Span<float> arr = stackalloc float[size];
            if (SplitToFloats(span, seperator, arr))
            {
                vector = new Vector2(arr[0], arr[1]);
                return;
            }
            throw new InvalidDataException();
        }

        public static bool TryParseVector2(string stringVector, out Vector2 val, char seperator = DEFAULT_SEPERATOR)
        {
            try
            {
                val = ToVector2(stringVector, seperator);
                return true;
            }
            catch (Exception)
            {
                val = default;
                return false;
            }
        }




        public static Vector2Int ToVector2Int(string stringVector, char seperator = DEFAULT_SEPERATOR)
        {
            ToVector2Int(stringVector, seperator, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ToVector2Int(string stringVector, char seperator, out Vector2Int vector)
        {
            var span = ReadSpan(stringVector);
            int size = 2;
            Span<int> arr = stackalloc int[size];
            if (SplitToIntegers(span, seperator, arr))
            {
                vector = new Vector2Int(arr[0], arr[1]);
                return;
            }
            throw new InvalidDataException();
        }

        public static bool TryParseVector2Int(string stringVector, out Vector2Int val, char seperator = DEFAULT_SEPERATOR)
        {
            try
            {
                val = ToVector2Int(stringVector, seperator);
                return true;
            }
            catch (Exception)
            {
                val = default;
                return false;
            }
        }




        public static Vector3Int ToVector3Int(string stringVector, char seperator = DEFAULT_SEPERATOR)
        {
            ToVector3Int(stringVector, seperator, out var value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ToVector3Int(string stringVector, char seperator, out Vector3Int vector)
        {
            var span = ReadSpan(stringVector);
            int size = 3;
            Span<int> arr = stackalloc int[size];
            if (SplitToIntegers(span, seperator, arr))
            {
                vector = new Vector3Int(arr[0], arr[1], arr[2]);
                return;
            }
            throw new InvalidDataException();
        }

        public static bool TryParseVector3Int(string stringVector, out Vector3Int val, char seperator = DEFAULT_SEPERATOR)
        {
            try
            {
                val = ToVector3Int(stringVector, seperator);
                return true;
            }
            catch (Exception)
            {
                val = default;
                return false;
            }
        }

        #endregion

#if ENABLE_BURST_AOT
        [BurstCompile(CompileSynchronously = true)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToAngle(this in Vector2 vector2)
        {
            return Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;
        }

#if ENABLE_BURST_AOT
        [BurstCompile(CompileSynchronously = true)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadian(this in Vector2 vector2)
        {
            return Mathf.Atan2(vector2.y, vector2.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Rotate(this in Vector2 v, float rad)
        {
            Rotate(v, rad, out var result); return result;
        }

#if ENABLE_BURST_AOT
        [BurstCompile(CompileSynchronously = true)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Rotate(this in Vector2 v, float rad, out Vector2 result)
        {
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            result = new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

#if ENABLE_BURST_AOT
        [BurstCompile(CompileSynchronously = true)]
#endif
        public static void RotateX(this in Vector3 v, float rad, out Vector3 result)
        {
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            result = new Vector3(v.x, v.y * cos - v.z * sin, v.y * sin + v.z * cos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RotateX(this in Vector3 v, float rad)
        {
            RotateX(v, rad, out Vector3 result);
            return result;
        }

#if ENABLE_BURST_AOT
        [BurstCompile(CompileSynchronously = true)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateY(this in Vector3 v, float rad, out Vector3 result)
        {
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            result = new Vector3(v.x * cos - v.z * sin, v.y, v.x * sin + v.z * cos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RotateY(this in Vector3 v, float rad)
        {
            RotateY(v, rad, out Vector3 result);
            return result;
        }

#if ENABLE_BURST_AOT
        [BurstCompile(CompileSynchronously = true)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateZ(this in Vector3 v, float rad, out Vector3 result)
        {
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            result = new Vector3(v.x * cos - v.y * sin, v.x * sin + v.y * cos, v.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RotateZ(this in Vector3 v, float rad)
        {
            RotateZ(v, rad, out Vector3 result);
            return result;
        }

        #region Old


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 YVector(this Vector2 vector2)
        {
            return new Vector2(0, vector2.y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int YVector(this Vector2Int vector2Int)
        {
            return new Vector2Int(0, vector2Int.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XVector(this Vector2 vector2)
        {
            return new Vector2(vector2.x, 0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int XVector(this Vector2Int vector2Int)
        {
            return new Vector2Int(vector2Int.x, 0);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ReflectY(this Vector2Int vector2Int)
        {
            return new Vector2Int(vector2Int.x, -vector2Int.y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ReflectY(this Vector2 vector2)
        {
            return new Vector2(vector2.x, -vector2.y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ReflectX(this Vector2Int vector2Int)
        {
            return new Vector2Int(-vector2Int.x, vector2Int.y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ReflectX(this Vector2 vector2)
        {
            return new Vector2(-vector2.x, vector2.y);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Reverse(this Vector2 v)
        {
            return new Vector2(v.y, v.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Abs(this Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Modulo(this Vector3 v, float a)
        {
            return new Vector3(v.x % a, v.y % a, v.z % a);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Modulo(this Vector2 v, float a)
        {
            return new Vector2(v.x % a, v.y % a);
        }



        public static IEnumerable<Vector2Int> Enumerate(this Vector2Int max)
        {
            for (int x = 0; x < max.x; x++)
            {
                for (int y = 0; y < max.y; y++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }
        public static IEnumerable<Vector3Int> Enumerate(this Vector3Int max)
        {
            for (int x = 0; x < max.x; x++)
            {
                for (int y = 0; y < max.y; y++)
                {
                    for (int z = 0; z < max.z; z++)
                    {
                        yield return new Vector3Int(x, y, z);
                    }
                }
            }
        }
        public static IEnumerable<Vector3Int> Enumerate(Vector3Int min, Vector3Int max)
        {
            for (int x = min.x; x < max.x; x++)
            {
                for (int y = min.y; y < max.y; y++)
                {
                    for (int z = min.z; z < max.z; z++)
                    {
                        yield return new Vector3Int(x, y, z);
                    }
                }
            }
        }
        public static IEnumerable<Vector2Int> Enumerate(Vector2Int min, Vector2Int max)
        {
            for (int x = min.x; x < max.x; x++)
            {
                for (int y = min.y; y < max.y; y++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }



        public static IEnumerable<Vector3Int> Range(Vector3Int start, Vector3Int count)
        {
            int upperX = start.x + count.x;
            int upperY = start.y + count.y;
            int upperZ = start.z + count.z;

            for (int x = start.x; x < upperX; x++)
            {
                for (int y = start.y; y < upperY; y++)
                {
                    for (int z = start.z; z < upperZ; z++)
                    {
                        yield return new Vector3Int(x, y, z);
                    }
                }
            }
        }

        public static IEnumerable<Vector2Int> Range(Vector2Int start, Vector2Int count)
        {
            int upperX = start.x + count.x;
            int upperY = start.y + count.y;
            for (int x = start.x; x < upperX; x++)
            {
                for (int y = start.y; y < upperY; y++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int ToVector3Int(this Vector2Int vector2Int, int z = 0)
        {
            return new Vector3Int(vector2Int.x, vector2Int.y, z);
        }






        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Random(float x, float y)
        {
            return new Vector2(UnityEngine.Random.value * x, UnityEngine.Random.value * y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Random(Vector2 min, Vector2 max) => Random(min.x, max.x, min.y, max.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Random(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector2(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Random(int x, int y)
        {
            return new Vector2Int(UnityEngine.Random.Range(0, x), UnityEngine.Random.Range(0, y));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Random(Vector2Int min, Vector2Int max) => Random(min.x, max.x, min.y, max.y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Random(int xMin, int xMax, int yMin, int yMax)
        {
            return new Vector2Int(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Random(float x, float y, float z)
        {
            return new Vector3(UnityEngine.Random.value * x, UnityEngine.Random.value * y, UnityEngine.Random.value * z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Random(Vector3 min, Vector3 max)
        {
            return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Random(int x, int y, int z)
        {
            return new Vector3Int(UnityEngine.Random.Range(0, x), UnityEngine.Random.Range(0, y), UnityEngine.Random.Range(0, z));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Random(Vector3Int min, Vector3Int max)
        {
            return new Vector3Int(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
        }




        public static IEnumerable<Vector2Int> FourNeighbors(this Vector2Int vector2)
        {
            yield return vector2 + Vector2Int.right;
            yield return vector2 + Vector2Int.up;
            yield return vector2 + Vector2Int.left;
            yield return vector2 + Vector2Int.down;
        }
        public static IEnumerable<Vector2Int> EightNeighbors(this Vector2Int vector2)
        {
            yield return vector2 + Vector2Int.right;
            yield return vector2 + Vector2Int.right + Vector2Int.up;
            yield return vector2 + Vector2Int.up;
            yield return vector2 + Vector2Int.left + Vector2Int.up;
            yield return vector2 + Vector2Int.left;
            yield return vector2 + Vector2Int.left + Vector2Int.down;
            yield return vector2 + Vector2Int.down;
            yield return vector2 + Vector2Int.down + Vector2Int.right;
        }
        public static IEnumerable<Vector2Int> NeighborInRadiusOf(this Vector2Int vector2, float radius)
        {
            for (int i = (int)(vector2.x - radius); i < (int)(vector2.x + radius); i++)
            {
                for (int j = (int)(vector2.y - radius); j < (int)(vector2.y + radius); j++)
                {
                    Vector2Int vector2Int = new(i, j);
                    if ((vector2Int - vector2).magnitude <= radius)
                    {
                        yield return vector2Int;
                    }
                }
            }
        }




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RandomRotate(this in Vector2 vector, float rad)
        {
            RandomRotate(vector, rad, out var result);
            return result;
        }

#if ENABLE_BURST_AOT
        [BurstCompile(CompileSynchronously = true)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RandomRotate(in Vector2 vector, float rad, out Vector2 result)
        {
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            result = new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
        }
        #endregion
    }
}