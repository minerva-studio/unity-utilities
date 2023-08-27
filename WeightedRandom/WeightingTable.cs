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
            return nodes.PopWeightNode(sum);
        }

        public T PopWeight()
        {
            return PopWeightNode().Item;
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

        public void AddRange<TW>(List<TW> item) where TW : IWeightable<T>
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