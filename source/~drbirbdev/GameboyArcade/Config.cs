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
#pragma warning disable CS0414 // Field is assigned but its value is never used

namespace GameboyArcade;

[SConfig]
class Config
{

    [SConfig.Option]
    public SButton Up = SButton.W;

    [SConfig.Option]
    public SButton Down = SButton.S;

    [SConfig.Option]
    public SButton Left = SButton.A;

    [SConfig.Option]
    public SButton Right = SButton.D;

    [SConfig.Option]
    public SButton A = SButton.MouseLeft;

    [SConfig.Option]
    public SButton B = SButton.MouseRight;

    [SConfig.Option]
    public SButton Start = SButton.Space;

    [SConfig.Option]
    public SButton Select = SButton.Tab;

    [SConfig.Option]
    public SButton Power = SButton.Escape;

    [SConfig.Option]
    public SButton Turbo = SButton.F1;

    [SConfig.Option(10000, 22050)]
    public int MusicSampleRate = 22050;
}
