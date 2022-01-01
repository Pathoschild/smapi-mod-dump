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

namespace SullySDVcore  // **** Rename namespace and uncomment log, and good to go ****
{
    public class Config
    {
    //    private static readonly Log log = ModClassName.Instance.log;
        public bool DisableAllModEffects { get; set; } = false;
        public int Example { get; set; } = 0;

        public static void VerifyConfigValues(Config config, Mod mod)
        {
            bool invalidConfig = false;

            if (config.Example != 0)
            {
                invalidConfig = true;
                config.Example = 0;
            }

            if (invalidConfig)
            {
    //            log.I("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        public static void SetUpModConfigMenu(Config config, Mod mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null) { return; }
            var manifest = mod.ModManifest;

            // Add 'true' as last parameter for title screen only config menu
            api.Register(manifest, () => config = new Config(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            //Configs to display
            api.AddNumberOption(manifest, () => config.Example, (int val) => config.Example = val,
                    name: () => "Example config", tooltip: () => "Example tooltip");
        }
    }
}