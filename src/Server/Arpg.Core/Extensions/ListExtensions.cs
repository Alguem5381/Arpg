namespace Arpg.Core.Extensions;

public static class ListExtensions
{
    extension<T>(List<T> values)
    {
        public void AddNullableRange(IEnumerable<T>? collection)
        {
            if (collection is null)
                return;

            values.AddRange(collection);
        }
    }
}