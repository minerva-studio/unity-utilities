using System;
using Minerva.Module.WeightedRandom;
using System.Collections.Generic;
using UnityEngine;

namespace Minerva.Module.WeightedRandom
{
    public class SegmentTree<T>
    {
        private struct WeightInterval
        {
            public int min;
            public int max;
        }

        private WeightInterval[] data;
        private WeightInterval[] nodes;
        private int[] add;
        private List<IWeightable<T>> items;

        public SegmentTree(List<IWeightable<T>> nodes)
        {
            data = new WeightInterval[nodes.Count + 1];
            items = nodes;
            add = new int[nodes.Count * 4 + 1]; // array of lazy lag
            this.nodes = new WeightInterval[nodes.Count * 4 + 1];
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i == 0)
                    data[i + 1].min = 1;
                else
                    data[i + 1].min = data[i].max + 1;
                data[i + 1].max = data[i + 1].min + nodes[i].Weight - 1;
            }
            //for (int i = 1; i <= nodes.Count; i++)
            //    Debug.Log(data[i].min + "  " + data[i].max);
            Build(1, 1, nodes.Count);
        }

        /// <summary>
        /// used to build a new segment tree
        /// </summary>
        /// <param name="k"> current index</param>
        /// <param name="l"> left end of current node </param>
        /// <param name="r"> right end of current node </param>
        private void Build(int k, int l, int r)
        {
            if (l == r)
            {
                nodes[k].min = data[l].min;
                nodes[k].max = data[l].max;
                add[k] = 0;
                return;
            }

            int mid = (l + r) / 2;
            Build(k * 2, l, mid);
            Build(k * 2 + 1, mid + 1, r);
            nodes[k].min = nodes[k * 2].min;
            nodes[k].max = nodes[k * 2 + 1].max;
        }

        /// <summary>
        /// interval modification --> add "v" to interval [x,y] 
        /// </summary>
        /// <param name="k"> current node</param>
        /// <param name="l"> left end of current node</param>
        /// <param name="r"> right end of current node</param>
        /// <param name="x"> left end of target interval </param>
        /// <param name="y"> right end of target interval </param>
        /// <param name="v"> the value needed to be added </param>
        private void Modify(int k, int l, int r, int x, int y, int v)
        {
            if (l >= x && r <= y) // [l,r] is already in the [x,y]
            {
                Add(k, v);
             //   if (nodes[])
               //     maintainMinValue(k, l, r, v);
                return;
            }

            int mid = (l + r) / 2;
            Pushdown(k, l, r);
            if (x <= mid)
                Modify(k * 2, l, mid, x, y, v);
            if (mid < y)
                Modify(k * 2 + 1, mid + 1, r, x, y, v);
            nodes[k].max += v; // since r of each modification always be the length of "nodes" in this project
                               // so as long as current node overlap with target interval, the max value must need to be changed
        }

        /*
        private void maintainMinValue (int k, int l, int r, int v)
        {
            //Debug.Log("maintainMinValue: " + l + " " + r);
            nodes[k].min -= v;
            if (l == r)
                return;
            int mid = (l + r) / 2;
            if (queryIndex <= mid)
                maintainMinValue(k * 2, l, mid, v);
            else
                maintainMinValue(k * 2 + 1, mid + 1, r, v);
        }*/

        private void Add(int k, int v)
        {
            add[k] += v;
            nodes[k].min += v;
            nodes[k].max += v;
        }

        private void Pushdown(int k, int l, int r)
        {
            if (add[k] == 0)
                return;
            Add(k * 2, add[k]);
            Add(k * 2 + 1, add[k]);
            add[k] = 0;
        }


        public IWeightable<T> Query(int v)
        {
            int index = QueryHelper(1, 1, items.Count, v);
            //Debug.Log("modify value: " + items[index - 1].Weight);
            Modify(1, 1, items.Count, index, items.Count, (-1) * items[index - 1].Weight);
            //Debug.Log(index);
            return items[index - 1]; // segement tree's index starts at one
        }

        private int QueryHelper(int k, int l, int r, int v)
        {
            //Debug.Log(l + " " + r + " " + nodes[k].min + " " + nodes[k].max);
            if (l == r)
            {
                //lastQueryMin = nodes[k].min;
                return l;
            }
            Pushdown(k, l, r);
            int mid = (l + r) / 2;
            WeightInterval leftSon = nodes[k * 2];
            if (v < leftSon.min)
                Debug.LogError("segment tree query bug: lefson.min: " + leftSon.min + " value: " + v);
            if (v > nodes[k * 2 + 1].max)
                Debug.LogError("segment tree query bugL rightson max: " + nodes[k * 2 + 1].max + " value " + v);
            if (leftSon.min <= v && leftSon.max >= v)
                return QueryHelper(k * 2, l, mid, v);
            return QueryHelper(k * 2 + 1, mid + 1, r, v);
        }
    }

}