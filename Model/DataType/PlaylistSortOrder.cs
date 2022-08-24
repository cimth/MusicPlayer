using System.ComponentModel;

namespace Model.DataType;

public enum PlaylistSortOrder
{
    Alphabetical,
    Individual,
    TitleNumber
}

/// <summary>
/// Use this class for getting string representations of the enum values above without using
/// custom attributes like [Description] and reflection.
/// </summary>
public static class PlaylistSortOrderToString
{
    public static string ToString(PlaylistSortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case PlaylistSortOrder.Alphabetical:
                return "Alphabetical";
            case PlaylistSortOrder.Individual:
                return "Individual";
            case PlaylistSortOrder.TitleNumber:
                return "Title number";
        }
        return sortOrder.ToString();
    }
}