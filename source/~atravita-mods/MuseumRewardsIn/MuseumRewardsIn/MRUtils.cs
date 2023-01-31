/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.StringHandler;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

namespace MuseumRewardsIn;

/// <summary>
/// The utility class for this mod.
/// </summary>
internal static class MRUtils
{
    /// <summary>
    /// Finds the items attached to a mail string.
    /// </summary>
    /// <param name="mail">The mail data to process.</param>
    /// <returns>The items attached.</returns>
    internal static IEnumerable<SObject> ParseItemsFromMail(this string mail)
    {
        int startindex = mail.IndexOf("%item");
        if (startindex < 0)
        {
            return Enumerable.Empty<SObject>();
        }

        int endindex = mail.IndexOf("%%", startindex);
        if (endindex < 0)
        {
            return Enumerable.Empty<SObject>();
        }

        ReadOnlySpan<char> substring = mail.AsSpan(startindex, endindex - startindex).Trim();
        if (substring.Length <= 0)
        {
            return Enumerable.Empty<SObject>();
        }

        if (substring.StartsWith("object ", StringComparison.OrdinalIgnoreCase))
        {
            List<SObject> ret = new();

            bool isItem = true;
            foreach (SpanSplitEntry split in substring["object ".Length..].Trim().StreamSplit())
            {
                if (isItem && int.TryParse(split, out int index) && index > 0)
                {
                    ret.Add(new SObject(index, 1));
                }
                isItem = !isItem;
            }
            return ret;
        }
        else if (substring.StartsWith("bigobject ", StringComparison.OrdinalIgnoreCase))
        {
            List<SObject> ret = new();

            foreach (SpanSplitEntry split in substring["bigobject ".Length..].Trim().StreamSplit())
            {
                if (int.TryParse(split, out int index) && index > 0)
                {
                    ret.Add(new SObject(Vector2.Zero, index));
                }
            }

            return ret;
        }
        else if (substring.StartsWith("furniture ", StringComparison.OrdinalIgnoreCase))
        {
            List<SObject> ret = new();

            foreach (SpanSplitEntry split in substring["furniture ".Length..].Trim().StreamSplit())
            {
                if (int.TryParse(split, out int index) && index > 0)
                {
                    ret.Add(Furniture.GetFurnitureInstance(index));
                }
            }

            return ret;
        }

        return Enumerable.Empty<SObject>();
    }
}
