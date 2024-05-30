/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Ponds.Framework;

internal record PondFish(string Id, int Quality) : IComparable<PondFish>
{
    internal static PondFish? FromString(string data)
    {
        var split = data.Split(',');
        return split.Length != 2 || !int.TryParse(split[1], out var quality)
            ? null
            : new PondFish(split[0], quality);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Id},{this.Quality}";
    }

    /// <inheritdoc />
    public int CompareTo(PondFish? other)
    {
        return other is null
            ? -1
            : this.Id == other.Id
                ? this.Quality.CompareTo(other.Quality)
                : string.Compare(this.Id, other.Id, StringComparison.InvariantCulture);
    }
}
