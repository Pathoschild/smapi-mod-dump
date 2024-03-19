/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

namespace CarWarp;

internal class GenericModConfigMenuHelper
{
    internal static void BuildConfigMenu()
    {
        if (Globals.GMCMApi is null)
        {
            Log.Info("Failed to fetch GMCM API.");
            return;
        }

        // register mod
        Globals.GMCMApi.Register(
            mod: Globals.Manifest,
            reset: () => Globals.Config = new ModConfig(),
            save: () => Globals.Helper.WriteConfig(Globals.Config)
        );

        // add brief description
        Globals.GMCMApi.AddSectionTitle(
            mod: Globals.Manifest,
            text: () => "Car Interior"
        );

        Globals.GMCMApi.AddParagraph(
            mod: Globals.Manifest,
            text: () => "Right: Steering wheel is on the right side of the vehicle (when facing down). The player will sit in the driver's seat." +
                        "\nLeft: Steering wheel is on the left side of the vehicle (when facing down). The player will sit in the driver's seat." +
                        "\nNone: Steering wheel is not present. The player will sit in whichever seat is closest to them." +
                        "\nEmpty: Steering wheel and dashboard are not present. The player will sit in whichever seat is closest to them."
        );

        // add dropdown for interior configuration option
        Globals.GMCMApi.AddTextOption(
            mod: Globals.Manifest,
            name: () => "Configuration",
            getValue: () => Globals.Config.Configuration,
            setValue: value => Globals.Config.Configuration = value,
            allowedValues: new[] {"Right", "Left", "None", "Empty"}
        );

        Globals.GMCMApi.AddSectionTitle(
            mod: Globals.Manifest,
            text: () => "Seasonal Overlay"
        );

        Globals.GMCMApi.AddParagraph(
            mod: Globals.Manifest,
            text: () => "Adds a light dusting of fallen leaves in autumn, or snow in winter."
        );

        Globals.GMCMApi.AddBoolOption(
            mod: Globals.Manifest,
            name: () => "Enabled:",
            getValue: () => Globals.Config.SeasonalOverlay,
            setValue: value => Globals.Config.SeasonalOverlay = value
        );
    }
}
