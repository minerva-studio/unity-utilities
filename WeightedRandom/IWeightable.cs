namespace Minerva.Module.WeightedRandom
{
    /// <summary>
    /// Interface for weightable items
    /// </summary>
    public interface IWeightable
    {
        int Weight { get; }
        object Item { get; }

        public int CompareTo(IWeightable weightable)
        {
            return Weight - weightable.Weight;
        }

    }

    /// <summary>
    /// Interface for weightable items
    /// </summary>
    public interface IWeightable<T> : IWeightable
    {
        new T Item { get; }
    }
}