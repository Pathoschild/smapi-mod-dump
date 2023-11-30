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
using StardewModdingAPI;

namespace LookToTheSky;

[SConfig]
class Config
{
    [SConfig.Option]
    public SButton Button { get; set; } = SButton.U;

    [SConfig.Option(Min = 0, Max = 100)]
    public int SpawnChancePerSecond { get; set; } = 5;

    [SConfig.Option]
    public bool DoNotificationNoise { get; set; } = true;
}
