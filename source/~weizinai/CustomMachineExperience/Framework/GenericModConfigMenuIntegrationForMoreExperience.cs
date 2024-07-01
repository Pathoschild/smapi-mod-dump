/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using weizinai.StardewValleyMod.Common.Integration;

namespace weizinai.StardewValleyMod.CustomMachineExperience.Framework;

internal class GenericModConfigMenuIntegrationForMoreExperience
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForMoreExperience(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action reset, Action save)
    {
        this.configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, reset, save);
    }

    public void Register()
    {
        if (!this.configMenu.IsLoaded) return;
        
        this.configMenu.Register();

        foreach (var (id, _) in this.configMenu.GetConfig().MachineExperienceData)
        {
            this.configMenu
                .AddSectionTitle(() => ItemRegistry.GetData(id).DisplayName)
                .AddNumberOption(
                    config => config.MachineExperienceData[id].FarmingExperience,
                    (config, value) => config.MachineExperienceData[id].FarmingExperience = value,
                    I18n.Config_FarmingSkill_Name
                )
                .AddNumberOption(
                    config => config.MachineExperienceData[id].FishingExperience,
                    (config, value) => config.MachineExperienceData[id].FishingExperience = value,
                    I18n.Config_FishingSkill_Name
                    )
                .AddNumberOption(
                    config => config.MachineExperienceData[id].ForagingExperience,
                    (config, value) => config.MachineExperienceData[id].ForagingExperience = value,
                    I18n.Config_ForagingSkill_Name
                )
                .AddNumberOption(
                    config => config.MachineExperienceData[id].MiningExperience,
                    (config, value) => config.MachineExperienceData[id].MiningExperience = value,
                    I18n.Config_MiningSkill_Name
                )
                .AddNumberOption(
                    config => config.MachineExperienceData[id].CombatExperience,
                    (config, value) => config.MachineExperienceData[id].CombatExperience = value,
                    I18n.Config_CombatSkill_Name
                );
        }
    }
}