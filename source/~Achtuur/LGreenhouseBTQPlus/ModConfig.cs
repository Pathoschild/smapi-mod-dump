/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Integrations;

namespace LGreenhouseBTQPlus;

public enum Flags
{
    Rainbow,
    Bi,
    Trans,
    Ace,
    Lesbian,
    Gay,
    Nonbinary,
    Pan,


};
internal class ModConfig
{

    public Flags FlagEnabled;

    public ModConfig()
    {
        FlagEnabled = Flags.Rainbow;

    }

    /// <summary>
    /// Constructs config menu for GenericConfigMenu mod
    /// </summary>
    public void createMenu()
    {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = ModEntry.Instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // register mod
        configMenu.Register(
            mod: ModEntry.Instance.ModManifest,
            reset: () => ModEntry.Instance.Config = new ModConfig(),
            save: () => ModEntry.Instance.Helper.WriteConfig(ModEntry.Instance.Config)
        );

        /// General travel skill settings header
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgSectionEnableflag,
            tooltip: null
        );

        //configMenu.AddTextOption(
        //    mod: ModEntry.Instance.ModManifest,
        //    name: I18n.CfgExpgain_Name,
        //    tooltip: I18n.CfgExpgain_Desc,
        //    getValue: () => StepsPerExp.ToString(),
        //    setValue: value => StepsPerExp = int.Parse(value),
        //    allowedValues: new string[] { "5", "10", "25", "50", "100" },
        //    formatAllowedValue: displayExpGainValues
        // );

    }

}
