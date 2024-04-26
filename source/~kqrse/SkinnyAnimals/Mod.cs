/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SkinnyAnimals;

internal partial class Mod: StardewModdingAPI.Mod {
    public static Configuration Config;

    public override void Entry(IModHelper helper) {
        Config = Helper.ReadConfig<Configuration>();

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        ApplyHarmonyPatches();
    }
    
    private void ApplyHarmonyPatches() {
        var harmony = new Harmony(ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.farmerPushing)),
            postfix: new HarmonyMethod(typeof(FarmAnimal_farmerPushing_Patch),
                nameof(FarmAnimal_farmerPushing_Patch.Postfix))
        );
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is not null) RegisterConfig(configMenu);
    }

    private void RegisterConfig(IGenericModConfigMenuApi configMenu) {
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new Configuration(),
            save: () => Helper.WriteConfig(Config)
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Enabled", 
            getValue: () => Config.Enabled,
            setValue: value => Config.Enabled = value
        );
        
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "Push Speed Multiplier", 
            getValue: () => Config.PushSpeedMultiplier,
            setValue: value => Config.PushSpeedMultiplier = value,
            tooltip: () => "Multiplies the speed of you pushing past animals, only applies when ignore collision is off",
            min: 1,
            max: 10
        );
        
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Ignore Collision", 
            getValue: () => Config.IgnoreCollision,
            setValue: value => Config.IgnoreCollision = value
        );
    }
}