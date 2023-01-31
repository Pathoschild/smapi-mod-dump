/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.StringHandler;

using AtraCore.Framework.ItemManagement;

using AtraShared.Utils.Extensions;

using Microsoft.Xna.Framework;

using StardewValley.Objects;

namespace CatGiftsRedux.Framework.Pickers;

/// <summary>
/// Picks a random seasonal crop item.
/// </summary>
internal static class SeasonalCropPicker
{
    internal static Item? Pick(Random random)
    {
        ModEntry.ModMonitor.DebugOnlyLog("Picked Seasonal Crops");

        List<KeyValuePair<int, string>>? content = Game1.content.Load<Dictionary<int, string>>("Data/Crops")
                                   .Where((kvp) => kvp.Value.GetNthChunk('/', 1).Contains(Game1.currentSeason, StringComparison.OrdinalIgnoreCase))
                                   .ToList();

        if (content.Count == 0)
        {
            return null;
        }

        int tries = 3;
        do
        {
            KeyValuePair<int, string> entry = content[random.Next(content.Count)];

            if (!Utils.IsQiQuestActive && entry.Value.GetNthChunk('/', SObject.objectInfoNameIndex).Contains("Qi", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (int.TryParse(entry.Value.GetNthChunk('/', 3), out int id) && id > 0)
            {
                // confirm the item exists.
                if (Utils.ForbiddenFromRandomPicking(id))
                {
                    continue;
                }

                if (DataToItemMap.IsActuallyRing(id))
                {
                    return new Ring(id);
                }

                ReadOnlySpan<char> colored = entry.Value.GetNthChunk('/', 8);
                if (colored.StartsWith("true", StringComparison.Ordinal))
                {
                    StreamSplit stream = colored.StreamSplit();
                    _ = stream.MoveNext(); // the original "true"

                    byte[] colorarray = new byte[3];
                    int index = 0;
                    foreach (SpanSplitEntry c in stream)
                    {
                        if (byte.TryParse(c, out byte colorbit))
                        {
                            colorarray[index++] = colorbit;
                            if (index >= 3)
                            {
                                break;
                            }
                        }
                        else
                        {
                            // can't parse the color, just return a noncolored object and hope for the best.
                            return new SObject(id, 1);
                        }
                    }
                    return new ColoredObject(id, 1, new Color(colorarray[0], colorarray[1], colorarray[2]));
                }
                return new SObject(id, 1);
            }
        }
        while (tries-- > 0);

        return null;
    }
}
