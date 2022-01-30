/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/


namespace ModdedMinecarts
{
    public class ModConfig
    {
        /// <summary>Logging tool</summary>
        private static readonly Log log = ModEntry.Instance.log;

        /*********
        ** Accessors
        *********/
        /// <summary>Whether the mod is disabled.</summary>
        public bool DisableAllModEffects { get; set; } = false;

        /// <summary>Example option.</summary>
        public int Example { get; set; } = 0;


        /*********
        ** Public methods
        *********/
        /// <summary>Check for and reset any invalid configuration settings.</summary>
        public static void VerifyConfigValues(ModConfig config, Mod mod)
        {
            bool invalidConfig = false;

            if (config.Example != 0)
            {
                invalidConfig = true;
                config.Example = 0;
            }

            if (invalidConfig)
            {
                log.I("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        /// <summary>Set up Generic Mod Config Menu integration.</summary>
        public static void SetUpModConfigMenu(ModConfig config, Mod mod)
        {
            // Get the Generic Mod Config Menu API
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null) { return; }
            var manifest = mod.ModManifest;

            // Register the Generic Mod Config Menu API
            api.Register(manifest, () => config = new ModConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            // Options to display
            api.AddNumberOption(manifest, () => config.Example, (int val) => config.Example = val,
                    name: () => "Example config", tooltip: () => "Example tooltip");
        }
    }
}