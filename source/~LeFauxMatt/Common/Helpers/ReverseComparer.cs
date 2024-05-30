/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Helpers;
#else
namespace StardewMods.Common.Helpers;
#endif

/// <inheritdoc />
internal sealed class ReverseComparer<T> : Comparer<T>
    where T : IComparable<T>
{
    /// <inheritdoc />
    public override int Compare(T? x, T? y)
    {
        if (x == null && y == null)
        {
            return 0;
        }

        if (x == null)
        {
            return 1;
        }

        if (y == null)
        {
            return -1;
        }

        return y.CompareTo(x);
    }
}