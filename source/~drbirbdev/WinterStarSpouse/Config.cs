/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbCore.Attributes;

namespace WinterStarSpouse;

[SConfig]
internal class Config
{

    [SConfig.Option(Min = 0, Max = 100, Interval = 1)]
    public int SpouseIsRecipientChance { get; set; } = 50;

    [SConfig.Option(Min = 0, Max = 100, Interval = 1)]
    public int SpouseIsGiverChance { get; set; } = 50;
}
