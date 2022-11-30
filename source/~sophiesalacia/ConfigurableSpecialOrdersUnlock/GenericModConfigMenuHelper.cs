/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

namespace ConfigurableSpecialOrdersUnlock;

class GenericModConfigMenuHelper
{
    private static readonly string[] Seasons = { "Spring", "Summer", "Fall", "Winter" };

    internal static void BuildConfigMenu()
    {
        // register mod
        Globals.GmcmApi.Register(
            Globals.Manifest,
            () => Globals.Config = new ModConfig(),
            () => Globals.Helper.WriteConfig(Globals.Config)
        );

        Globals.GmcmApi.AddBoolOption(
            Globals.Manifest,
            name: () => "Skip Installation Cutscene",
            tooltip: () => "Skip cutscene where Robin and Lewis install the Special Orders board",
            getValue: () => Globals.Config.SkipCutscene,
            setValue: val => Globals.Config.SkipCutscene = val
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Unlock Year",
            tooltip: () => "Year in which Special Orders board unlocks (default is Year 1)",
            getValue: () => Globals.Config.UnlockYear,
            setValue: val => Globals.Config.UnlockYear = val,
            min: 1
        );

        Globals.GmcmApi.AddTextOption(
            Globals.Manifest,
            name: () => "Unlock Season",
            tooltip: () => "Season in which Special Orders board unlocks (default is Fall)",
            getValue: () => Globals.Config.UnlockSeason,
            setValue: val => Globals.Config.UnlockSeason = val,
            allowedValues: Seasons
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Unlock Day",
            tooltip: () => "Day of the month on which Special Orders board unlocks (default is day 2)",
            getValue: () => Globals.Config.UnlockDay,
            setValue: val => Globals.Config.UnlockDay = val,
            min: 1,
            max: 28
        );
    }
}
