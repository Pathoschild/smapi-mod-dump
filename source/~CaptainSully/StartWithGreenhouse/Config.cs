/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using SullySDVcore;
using StardewModdingAPI;

namespace StartWithGreenhouse
{
    public class Config
    {
        public bool DisableAllModEffects { get; set; } = false;

        public static void SetUpModConfigMenu(Config config, Mod mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null) { return; }
            var manifest = mod.ModManifest;

            api.Register(manifest, () => config = new Config(), delegate { mod.Helper.WriteConfig(config); }, true);

            //Configs to display
            api.AddBoolOption(manifest, () => config.DisableAllModEffects, (bool val) => config.DisableAllModEffects = val,
                    name: () => "Disable mod", tooltip: () => null);
            api.AddParagraph(manifest, text: () => "Will not affect newly loaded saves if true.\n" +
                    "This does NOT reverse the effects if the greenhouse has already been unlocked in an existing save, even if it was through this mod.");

        }
    }
}