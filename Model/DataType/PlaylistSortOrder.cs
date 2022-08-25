using System.ComponentModel;
using Common;

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
                return LanguageUtil.GiveLocalizedString("Str_Alphabetical");
            case PlaylistSortOrder.Individual:
                return LanguageUtil.GiveLocalizedString("Str_Individual");
            case PlaylistSortOrder.TitleNumber:
                return LanguageUtil.GiveLocalizedString("Str_TitleNumber");
        }
        return sortOrder.ToString();
    }
}