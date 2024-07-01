/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.UI;

internal class TotalOnlineTimeBox : Box
{
    protected override Color TextColor => this.Config.TotalOnlineTime.TextColor;
    protected override string Text => Utility.getHoursMinutesStringFromMilliseconds(this.Cabin.owner.millisecondsPlayed);
    protected override Point Offset => new(this.Config.TotalOnlineTime.XOffset, this.Config.TotalOnlineTime.YOffset);

    public TotalOnlineTimeBox(Building building, Cabin cabin, ModConfig config) 
        : base(building, cabin, config)
    {
    }
}