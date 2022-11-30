/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

namespace ColoredCrystalariums;

internal class GenericModConfigMenuHelper
{

    internal static void BuildConfigMenu()
    {
        // register mod
        Globals.GmcmApi.Register(
            mod: Globals.Manifest,
            reset: () => Globals.Config = new ModConfig(),
            save: () => Globals.Helper.WriteConfig(Globals.Config)
        );

        Globals.GmcmApi.AddBoolOption(
            mod: Globals.Manifest,
            name: () => "Draw Simplified Gem Bubbles",
            tooltip: () => "If enabled, crystalariums will simply show the gem floating above them when ready.",
            getValue: () => Globals.Config.DrawSimplifiedGemBubbles,
            setValue: val => Globals.Config.DrawSimplifiedGemBubbles = val
        );
    }
}
