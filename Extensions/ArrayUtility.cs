namespace Minerva.Module
{
    public static class ArrayUtility
    {
        public static T[] Of<T>(params T[] values)
        {
            return values;
        }
    }
}