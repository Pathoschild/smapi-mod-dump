/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/


namespace StartWithGreenhouse
{
    // <summary>The raw mod configuration.</summary>
    internal class ModConfig
    {
       /*********
       ** Accessors
       *********/
        /// <summary>Whether the mod is disabled.</summary>
        public bool DisableAllModEffects { get; set; } = false;


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
            api.Register(manifest, () => config = new ModConfig(), delegate { mod.Helper.WriteConfig(config); }, true);

            // Options to display
            api.AddBoolOption(manifest, () => config.DisableAllModEffects, (bool val) => config.DisableAllModEffects = val,
                    name: () => "Disable mod", tooltip: () => null);
            api.AddParagraph(manifest, text: () => "Will not affect newly loaded saves if true.\n" +
                    "This does NOT reverse the effects if the greenhouse has already been unlocked in an existing save, even if it was through this mod.");

        }
    }
}