using System.Collections.ObjectModel;

namespace Common;

public static class SortUtil
{
    /// <summary>
    /// Sorts the given collection without assigning a new collection.
    /// This is important when binding a GUI element to a collection which is not immediately managed by the ViewModel
    /// but by another object the ViewModel uses. In this case, the new assignment of the collection would not be
    /// forwarded to the GUI because the GUI still binds to the old collection (since the reference is still the old
    /// one in the ViewModel).
    /// </summary>
    /// <param name="collection">The collection to sort.</param>
    /// <param name="keySelector">The function for extracting a key used for sorting.</param>
    /// <typeparam name="TSource">The type of the collection's items.</typeparam>
    /// <typeparam name="TKey">The type of the key used for sorting.</typeparam>
    public static void SortWithoutNewCollection<TSource, TKey>(ObservableCollection<TSource> collection, Func<TSource, TKey> keySelector)
    {
        // Get a sorted copy of the list
        ObservableCollection<TSource> sorted = new ObservableCollection<TSource>(collection.OrderBy(keySelector));

        // Clear and re-add the items to avoid assigning a new collection which would result in a loss of all
        // observers of the current collection since they still observe to the first referenced collection.
        collection.Clear();
        foreach (var item in sorted)
        {
            collection.Add(item);
        }
    }
}