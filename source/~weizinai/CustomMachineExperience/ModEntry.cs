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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Machines;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.CustomMachineExperience.Framework;

namespace weizinai.StardewValleyMod.CustomMachineExperience;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.config = helper.ReadConfig<ModConfig>();
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo("Data/Machines"))
        {
            e.Edit(asset =>
                {
                    var machineData = asset.AsDictionary<string, MachineData>().Data;
                    foreach (var (id, data) in machineData) data.ExperienceGainOnHarvest = this.config.MachineExperienceData[id].ToString();
                }
            );
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.InitConfig();
        
        new GenericModConfigMenuIntegrationForMoreExperience(
            this.Helper,
            this.ModManifest,
            () => this.config,
            () =>
            {
                this.config = new ModConfig();
                this.InitConfig();
            },
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }

    private void InitConfig()
    {
        var machineData = Game1.content.Load<Dictionary<string, MachineData>>("Data/Machines");
        foreach (var (id, _) in machineData) this.config.MachineExperienceData.TryAdd(id, new ExperienceData());
        this.Helper.WriteConfig(this.config);
    }
}