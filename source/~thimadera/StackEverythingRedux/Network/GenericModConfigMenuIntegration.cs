/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using StackEverythingRedux.Models;
using StardewModdingAPI;

namespace StackEverythingRedux.Network
{
    internal class GenericModConfigMenuIntegration
    {
        private static ModConfig Config = StackEverythingRedux.Config;

        public static void AddConfig()
        {
            IGenericModConfigMenuApi genericModConfigApi = StackEverythingRedux.Registry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            IManifest mod = StackEverythingRedux.Manifest;

            if (genericModConfigApi is null)
            {
                Log.Trace("GMCM not available, skipping Mod Config Menu");
                return;
            }

            if (Config is null)
            {
                return;
            }

            I18n.Init(StackEverythingRedux.I18n);

            genericModConfigApi.Register(
                mod,
                reset: () => Config = new ModConfig(),
                save: () => StackEverythingRedux.ModHelper.WriteConfig(Config)
            );

            genericModConfigApi.AddNumberOption(
                mod,
                name: I18n.Config_MaxStackingNumber_Name,
                tooltip: I18n.Config_MaxStackingNumber_Tooltip,
                getValue: () => Config.MaxStackingNumber,
                setValue: value => Config.MaxStackingNumber = value
            );

            genericModConfigApi.AddSectionTitle(mod, () => "Stack Split Redux");

            genericModConfigApi.AddBoolOption(
                mod,
                name: I18n.Config_EnableStackSplitRedux_Name,
                tooltip: I18n.Config_EnableStackSplitRedux_Tooltip,
                getValue: () => Config.EnableStackSplitRedux,
                setValue: value => Config.EnableStackSplitRedux = value
            );

            genericModConfigApi.AddNumberOption(
                mod,
                name: I18n.Config_DefaultCraftingAmount_Name,
                tooltip: I18n.Config_DefaultCraftingAmount_Tooltip,
                getValue: () => Config.DefaultCraftingAmount,
                setValue: value => Config.DefaultCraftingAmount = value
            );

            genericModConfigApi.AddNumberOption(
                mod,
                name: I18n.Config_DefaultShopAmount_Name,
                tooltip: I18n.Config_DefaultShopAmount_Tooltip,
                getValue: () => Config.DefaultShopAmount,
                setValue: value => Config.DefaultShopAmount = value
            );
        }
    }
}
