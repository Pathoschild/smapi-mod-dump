/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Integrations;
using AchtuurCore.Utility;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace MultiplayerExpShare;

internal enum ExpShareType
{
    /// <summary>
    /// All players share exp globally
    /// </summary>
    Global,
    /// <summary>
    /// Players on the same map share exp
    /// </summary>
    Map,
    /// <summary>
    /// Players within a certain tile range share exp
    /// </summary>
    Tile
}
internal class ModConfig
{

    private readonly SliderRange PercentageToActorSlider = new SliderRange(0.25f, 0.75f, 0.05f);
    private readonly SliderRange NearbyPlayerTileRangeSlider = new SliderRange(10, 50, 5);
    private readonly SliderRange OverlayOpacitySlider = new SliderRange(0f, 1f, 0.05f);
    private readonly SliderRange ExpPerParticleSlider = new SliderRange(1, 10, 1);


    private static bool isRegistered = false;
    /// <summary>
    /// If this is true, then two players on the same map will always count as being nearby
    /// </summary>
    public ExpShareType ExpShareType { get; set; }

    public SButton OverlayButton { get; set; }
    public float OverlayOpacity { get; set; }

    public bool ShareAllExpAtMaxLevel { get; set; }

    /// <summary>
    /// Other farmers must be within this range to count as nearby
    /// </summary>
    public int NearbyPlayerTileRange { get; set; }

    /// <summary>
    /// Percentage of exp that goes to actor, rest of exp is divided equally between nearby players
    /// </summary>
    public float ExpPercentageToActor { get; set; }

    /// <summary>
    /// Whether Exp sharing is enabled per vanilla skill
    /// </summary>
    public bool[] VanillaSkillEnabled { get; set; }

    public Dictionary<string, bool> SpaceCoreSkillEnabled { get; set; }

    public int ExpPerParticle { get; set; }

    public ModConfig()
    {
        // Changable by player
        this.NearbyPlayerTileRange = 25;
        this.ExpPercentageToActor = 0.75f;
        this.OverlayOpacity = 0.5f;
        this.ExpShareType = ExpShareType.Tile;
        this.OverlayButton = SButton.K;
        this.ShareAllExpAtMaxLevel = true;

        this.ExpPerParticle = 3;

        this.VanillaSkillEnabled = new[] {
            true,  // Farming
            false, // Fishing
            true,  // Foraging
            true,  // Mining
            true,  // Combat
        };

        InitSpacecoreDict();

    }

    private void InitSpacecoreDict()
    {
        SpaceCoreSkillEnabled = new Dictionary<string, bool>();

        if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore") || ModEntry.Instance.SpaceCoreAPI is null)
            return;

        foreach (string s in ModEntry.Instance.SpaceCoreAPI.GetCustomSkills())
        {
            SpaceCoreSkillEnabled.Add(s, true);
        }
    }

    /// <summary>
    /// Constructs config menu for GenericConfigMenu mod
    /// </summary>
    /// <param name="instance"></param>
    public void createMenu()
    {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = ModEntry.Instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // Unregister in case mod is already registered
        if (ModConfig.isRegistered)
            configMenu.Unregister(ModEntry.Instance.ModManifest);


        // register mod
        configMenu.Register(
            mod: ModEntry.Instance.ModManifest,
            reset: () => ModEntry.Instance.Config = new ModConfig(),
            save: () =>
            {
                ModEntry.Instance.Helper.WriteConfig<ModConfig>(ModEntry.Instance.Config);
            }
        );

        isRegistered = true;

        /// General travel skill settings header
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgGeneral_Name,
            tooltip: I18n.CfgGeneral_Desc
        );

        // Exp share type
        configMenu.AddTextOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgSharetype_Name,
            tooltip: I18n.CfgSharetype_Desc,
            getValue: GetExpShareType,
            setValue: SetExpShareType,
            allowedValues: new string[] { "Tile", "Map", "Global" }
         );

        // nearby player tile range
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgNearbyplayertilerange_Name,
            tooltip: I18n.CfgNearbyplayertilerange_Desc,
            getValue: () => NearbyPlayerTileRange,
            setValue: value => NearbyPlayerTileRange = value,
            min: (int)NearbyPlayerTileRangeSlider.min,
            max: (int)NearbyPlayerTileRangeSlider.max,
            interval: (int)NearbyPlayerTileRangeSlider.interval
         );

        // exp percentage to actor
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgExptoactor_Name,
            tooltip: I18n.CfgExptoactor_Desc,
            getValue: () => ExpPercentageToActor,
            setValue: value => ExpPercentageToActor = value,
            min: PercentageToActorSlider.min,
            max: PercentageToActorSlider.max,
            interval: PercentageToActorSlider.interval,
            formatValue: displayAsPercentage
         );

        // share all exp at max level
        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgShareallexpmaxlevel_Name,
            tooltip: I18n.CfgShareallexpmaxlevel_Desc,
            getValue: () => ShareAllExpAtMaxLevel,
            setValue: value => ShareAllExpAtMaxLevel = value
        );


        /// General travel skill settings header
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgParticlesection_Name
        );

        // nearby player tile range
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgExpperparticle_Name,
            tooltip: I18n.CfgExpperparticle_Desc,
            getValue: () => ExpPerParticle,
            setValue: value => ExpPerParticle = value,
            min: (int)ExpPerParticleSlider.min,
            max: (int)ExpPerParticleSlider.max,
            interval: (int)ExpPerParticleSlider.interval
         );

        configMenu.AddSectionTitle(
            ModEntry.Instance.ModManifest,
            text: I18n.CfgOverlay_Name
        );

        configMenu.AddKeybind(
           mod: ModEntry.Instance.ModManifest,
           name: I18n.CfgOverlaybutton_Name,
           tooltip: I18n.CfgOverlaybutton_Desc,
           getValue: () => this.OverlayButton,
           setValue: value => this.OverlayButton = value
        );


        // exp percentage to actor
        configMenu.AddNumberOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgOverlayopacity_Name,
            tooltip: I18n.CfgOverlayopacity_Desc,
            getValue: () => OverlayOpacity,
            setValue: value => OverlayOpacity = value,
            min: OverlayOpacitySlider.min,
            max: OverlayOpacitySlider.max,
            interval: OverlayOpacitySlider.interval,
            formatValue: displayAsPercentage
         );

        // Enable/disable menu
        configMenu.AddSectionTitle(
            mod: ModEntry.Instance.ModManifest,
            text: I18n.CfgEnablesection,
            tooltip: null
        );

        // farming
        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgEnablefarming_Name,
            tooltip: I18n.CfgEnablefarming_Desc,
            getValue: () => VanillaSkillEnabled[0],
            setValue: value => VanillaSkillEnabled[0] = value
        );

        // fishing
        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgEnablefishing_Name,
            tooltip: I18n.CfgEnablefishing_Desc,
            getValue: () => VanillaSkillEnabled[1],
            setValue: value => VanillaSkillEnabled[1] = value
        );

        // foraging
        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgEnableforaging_Name,
            tooltip: I18n.CfgEnableforaging_Desc,
            getValue: () => VanillaSkillEnabled[2],
            setValue: value => VanillaSkillEnabled[2] = value
        );

        // mining
        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgEnablemining_Name,
            tooltip: I18n.CfgEnablemining_Desc,
            getValue: () => VanillaSkillEnabled[3],
            setValue: value => VanillaSkillEnabled[3] = value
        );

        // combat
        configMenu.AddBoolOption(
            mod: ModEntry.Instance.ModManifest,
            name: I18n.CfgEnablecombat_Name,
            tooltip: I18n.CfgEnablecombat_Desc,
            getValue: () => VanillaSkillEnabled[4],
            setValue: value => VanillaSkillEnabled[4] = value
        );

        // SPACECORE SKILLS

        // Initialise spacecore skills dictionary
        InitSpacecoreDict();

        if (ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
        {
            // Enable/disable menu
            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: I18n.CfgEnableSpacecoresection,
                tooltip: null
            );

            // Add config for each skill
            foreach (string skill_name in SpaceCoreSkillEnabled.Keys)
            {
                // [0] contains mod author, [1] contains skill name
                var skill_name_split = skill_name.Split('.');

                configMenu.AddBoolOption(
                    mod: ModEntry.Instance.ModManifest,
                    name: () => $"Enable {skill_name_split[1]}",
                    tooltip: () => $"Whether to enable exp sharing for {skill_name_split[1]} skill (by {skill_name_split[0]})",
                    getValue: () => SpaceCoreSkillEnabled[skill_name],
                    setValue: value => SpaceCoreSkillEnabled[skill_name] = value
                );
            }
        }


    }

    private string GetExpShareType()
    {
        switch (this.ExpShareType)
        {
            case ExpShareType.Tile: return "Tile";
            case ExpShareType.Map: return "Map";
            case ExpShareType.Global: return "Global";
        }
        // should be unreachable, if this ever appears then you made a mistake sir programmer
        return "Something went wrong... :(";
    }
    private void SetExpShareType(string option)
    {
        switch (option)
        {
            case "Map": ExpShareType = ExpShareType.Map; break;
            case "Tile": ExpShareType = ExpShareType.Tile; break;
            case "Global": ExpShareType = ExpShareType.Global; break;
            default: ExpShareType = ExpShareType.Tile; break;
        }
    }

    /// <summary>
    /// Displays <paramref name="value"/> as a percentage, rounded to two decimals.
    /// <c>ModConfig.displayAsPercentage(0.02542); // returns 2.54%</c>
    /// </summary>
    public static string displayAsPercentage(float value)
    {
        return Math.Round(100f * value, 2).ToString() + "%";
    }

}
