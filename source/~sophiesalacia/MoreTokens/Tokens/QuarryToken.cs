/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace MoreTokens.Tokens;

internal class QuarryToken : SimpleToken
{
    private static readonly PropertyInfo IsQuarryAreaProperty = typeof(MineShaft).GetProperty("isQuarryArea", BindingFlags.NonPublic | BindingFlags.Instance);

    public override string GetName() => "IsQuarry";

    public override IEnumerable<string> GetValue()
    {
        switch (Context.IsWorldReady)
        {
            // save is loaded and we are in a mineshaft
            case true when Game1.currentLocation is MineShaft ms:
                {
                    bool? quarryArea = (bool?)IsQuarryAreaProperty.GetValue(ms);

                    return quarryArea is null ? null : new[] { quarryArea.ToString() };
                }

            // save is loaded but we are not in a mineshaft
            case true:
                return new[] { false.ToString() };

            // no save loaded (e.g. on the title screen)
            default:
                return null;
        }
    }
}
