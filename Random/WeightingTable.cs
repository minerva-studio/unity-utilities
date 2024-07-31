using System.Collections.Generic;

namespace Minerva.Module.WeightedRandom
{
    public class WeightingTable<T>
    {
        List<IWeightable<T>> nodes;
        int sum;

        public IWeightable<T> this[int index]
        {
            get => nodes[index];
        }

        public int Count => nodes.Count;

        public int Sum => sum;



        public WeightingTable()
        {
            this.nodes = new List<IWeightable<T>>();
            this.sum = 0;
        }

        public IWeightable<T> WeightNode()
        {
            return nodes.WeightNode(sum);
        }

        public T Weight()
        {
            return nodes.WeightNode(sum).Item;
        }

        public IWeightable<T> PopWeightNode()
        {
            return PopWeightNode(UnityEngine.Random.Range(0, sum));
        }

        public IWeightable<T> PopWeightNode(int position)
        {
            if (nodes.Count == 0)
            {
                return null;
            }

            IWeightable<T> weightable = nodes.PopWeightNodePos(sum, position);
            sum -= weightable.Weight;
            return weightable;
        }

        public T PopWeight()
        {
            return PopWeightNode().Item;
        }

        public T PopWeight(int position)
        {
            return PopWeightNode(position).Item;
        }

        public void Add(T item, int w)
        {
            var weight = new Weight<T>
            {
                item = item,
                weight = w
            };
            nodes.Add(weight);
            sum += w;
        }

        public void Add(IWeightable<T> item)
        {
            nodes.Add(item);
            sum += item.Weight;
        }

        public void AddRange<TW>(IEnumerable<TW> items) where TW : IWeightable<T>
        {
            if (items is IList<TW> list)
            {
                AddRange(list);
                return;
            }
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void AddRange<TW>(IList<TW> item) where TW : IWeightable<T>
        {
            for (int i = 0; i < item.Count; i++)
            {
                nodes.Add(item[i]);
                sum += item[i].Weight;
            }
        }

        public void Remove(IWeightable<T> item)
        {
            if (nodes.Remove(item))
                sum -= item.Weight;
        }

        public void Clear()
        {
            nodes.Clear();
            sum = 0;
        }
    }

}