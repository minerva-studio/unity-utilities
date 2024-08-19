using Unity.Collections;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// A struct representing a 2d native array, just a wrapper of <see cref="NativeArray{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Native2DArray<T> where T : struct
    {
        Vector2Int size;
        NativeArray<T> array;

        public T this[int x, int y]
        {
            get => array[x + y * size.y];
            set => array[x + y * size.y] = value;
        }

        public int Length => array.Length;
        public Vector2Int RectSize => size;
        public NativeArray<T> Array => array;



        public unsafe Native2DArray(int length1, int length2, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
        {
            this.size = new Vector2Int(length1, length2);
            this.array = new NativeArray<T>(length1 * length2, allocator, options);
        }

        public Native2DArray(T[,] array, Allocator allocator)
        {
            this.size = new Vector2Int(array.GetLength(0), array.GetLength(1));
            this.array = new NativeArray<T>(size.x * size.y, allocator);
        }

        public Native2DArray(Native2DArray<T> array, Allocator allocator)
        {
            this.size = array.size;
            this.array = new NativeArray<T>(array.array, allocator);
        }

    }
}