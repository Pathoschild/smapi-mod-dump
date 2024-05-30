/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using FasterPathSpeed.Patches;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FasterPathSpeed
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }

        public static ModConfig Config { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Harmony patches
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.getMovementSpeed)),
                postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.GetMovementSpeed_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction)),
                postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.PlacementAction_Postfix))
            );

            // Get Generic Mod Config Menu API (if it's installed)
            IGenericModConfigMenuApi api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.Register(
                    mod: ModManifest,
                    reset: () => Config = new ModConfig(),
                    save: () => Helper.WriteConfig(Config)
                );

                api.AddNumberOption(
                    mod: ModManifest,
                    getValue: () => Config.DefaultPathSpeedBuff,
                    setValue: value => Config.DefaultPathSpeedBuff = value,
                    name: () => "Default Path Speed Buff",
                    tooltip: () => "Extra movement speed obtained from walking on a path",
                    min: 0f,
                    max: 5f,
                    interval: 0.1f
                );
                api.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.IsPathSpeedBuffOnlyOnTheFarm,
                    setValue: value => Config.IsPathSpeedBuffOnlyOnTheFarm = value,
                    name: () => "Only On Farm?",
                    tooltip: () => "If enabled, the path speed buff is only obtained while on your farm"
                );
                api.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.IsPathAffectHorseSpeed,
                    setValue: value => Config.IsPathAffectHorseSpeed = value,
                    name: () => "Affect Horse Speed?",
                    tooltip: () => "If enabled, the path speed buff is also obtained while riding a horse"
                );
                api.AddNumberOption(
                    mod: ModManifest,
                    getValue: () => Config.HorsePathSpeedBuffModifier,
                    setValue: value => Config.HorsePathSpeedBuffModifier = value,
                    name: () => "Horse Speed Multiplier",
                    tooltip: () => "Multiplier for path speed buff while riding a horse",
                    min: 0,
                    max: 2,
                    interval: 0.05f
                );
                api.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.IsEnablePathReplace,
                    setValue: value => Config.IsEnablePathReplace = value,
                    name: () => "Enable Path Replace?",
                    tooltip: () => "If enabled, placing a path on an existing one replaces it"
                );
                api.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.IsTownPathSpeedBuff,
                    setValue: value => Config.IsTownPathSpeedBuff = value,
                    name: () => "Town Path Speed Buff?",
                    tooltip: () => "If enabled, the existing town path provides the default path speed buff"
                );

                api.AddSectionTitle(
                    mod: ModManifest,
                    text: () => "Custom Path Values"
                );
                api.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.IsUseCustomPathSpeedBuffValues,
                    setValue: value => Config.IsUseCustomPathSpeedBuffValues = value,
                    name: () => "Use Custom Path Values?",
                    tooltip: () => "If enabled, each path has its own buff amount (listed below). Otherwise, the default path speed buff is used for all paths"
                );
                foreach (var prop in Config.CustomPathSpeedBuffValues.GetType().GetProperties())
                {
                    api.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)prop.GetValue(Config.CustomPathSpeedBuffValues),
                        setValue: value => prop.SetValue(Config.CustomPathSpeedBuffValues, value),
                        name: () => $" - {prop.Name}",
                        tooltip: () => $"Extra movement speed obtained from walking on {prop.Name}",
                        min: 0,
                        max: 5,
                        interval: 0.1f
                    );
                }
            }
        }
    }
}
