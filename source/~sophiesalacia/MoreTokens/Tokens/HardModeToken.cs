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
using StardewModdingAPI;
using StardewValley;

namespace MoreTokens.Tokens;

internal class HardModeToken : SimpleToken
{
    public override string GetName() => "IsHardMode";

    public override IEnumerable<string> GetValue()
    {
        return !Context.IsWorldReady ? null : new[] {(Game1.netWorldState.Value.MinesDifficulty > 0).ToString()};
    }
}
