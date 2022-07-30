/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/


namespace InfiniteWateringCan
{
    // <summary>The raw mod configuration.</summary>
    internal class ModConfig
    {
       /*********
       ** Accessors
       *********/
        /// <summary>Whether the mod is disabled.</summary>
        public bool InfiniteWater { get; set; } = true;


        /*********
        ** Public methods
        *********/
        /// <summary>Set up Generic Mod Config Menu integration.</summary>
        public static void SetUpModConfigMenu(ModConfig config, Mod mod)
        {
            // Get the Generic Mod Config Menu API
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null) { return; }
            var manifest = mod.ModManifest;

            // Register the Generic Mod Config Menu API
            api.Register(manifest, () => config = new ModConfig(), delegate { mod.Helper.WriteConfig(config); });

            // Options to display
            api.AddBoolOption(manifest, () => config.InfiniteWater, (bool val) => config.InfiniteWater = val,
                    name: () => "Infinite Water", tooltip: () => null);
            api.AddParagraph(manifest, text: () => "Infinite water in watering cans while true.");

        }
    }
}