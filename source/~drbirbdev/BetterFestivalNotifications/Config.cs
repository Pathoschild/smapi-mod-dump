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

namespace BetterFestivalNotifications;

[SConfig]
internal class Config
{
    [SConfig.Option]
    public bool PlayStartSound = true;
    [SConfig.Option]
    public string StartSound = "crystal";


    [SConfig.Option(1, 10)]
    public int WarnHoursAheadOfTime = 2;

    [SConfig.Option]
    public bool PlayWarnSound = true;

    [SConfig.Option]
    public bool ShowWarnNotification = true;

    [SConfig.Option]
    public string WarnSound = "phone";


    [SConfig.Option]
    public bool PlayOverSound = false;

    [SConfig.Option]
    public bool ShowOverNotification = false;

    [SConfig.Option]
    public string OverSound = "ghost";
}
