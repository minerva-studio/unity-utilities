using System.Collections.Generic;

namespace Minerva.Module
{
    public interface INameable
    {
        string Name { get; }
    }

    public static class NameableExtensions
    {
        public static bool Contains(this IEnumerable<INameable> nameables, string name)
        {
            foreach (var nameable in nameables)
            {
                if (nameable.Name == name) return true;
            }
            return false;
        }
    }
}