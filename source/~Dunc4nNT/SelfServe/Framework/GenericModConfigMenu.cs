/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using NeverToxic.StardewMods.Common;
using StardewModdingAPI;
using System;

namespace NeverToxic.StardewMods.SelfServe.Framework
{
    internal class GenericModConfigMenu(IModRegistry modRegistry, IManifest manifest, IMonitor monitor, Func<ModConfig> config, Action reset, Action save)
    {
        public void Register()
        {
            IGenericModConfigMenuApi configMenu = modRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
                return;

            configMenu.Register(mod: manifest, reset: reset, save: save);


            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Shops_TownSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_PierresGeneralShop_Name,
                tooltip: I18n.Config_Shops_PierresGeneralShop_Tooltip,
                getValue: () => config().PierresGeneralShop,
                setValue: value => config().PierresGeneralShop = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_BlacksmithShop_Name,
                tooltip: I18n.Config_Shops_BlacksmithShop_Tooltip,
                getValue: () => config().BlacksmithShop,
                setValue: value => config().BlacksmithShop = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_SaloonShop_Name,
                tooltip: I18n.Config_Shops_SaloonShop_Tooltip,
                getValue: () => config().SaloonShop,
                setValue: value => config().SaloonShop = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_HospitalShop_Name,
                tooltip: I18n.Config_Shops_HospitalShop_Tooltip,
                getValue: () => config().HospitalShop,
                setValue: value => config().HospitalShop = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_IceCreamShop_Name,
                tooltip: I18n.Config_Shops_IceCreamShop_Tooltip,
                getValue: () => config().IceCreamShop,
                setValue: value => config().IceCreamShop = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_BooksellerShop_Name,
                tooltip: I18n.Config_Shops_BooksellerShop_Tooltip,
                getValue: () => config().BooksellerShop,
                setValue: value => config().BooksellerShop = value
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Shops_BeachSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_WillysFishShop_Name,
                tooltip: I18n.Config_Shops_WillysFishShop_Tooltip,
                getValue: () => config().WillysFishShop,
                setValue: value => config().WillysFishShop = value
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Shops_MountainSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_CarpentersShop_Name,
                tooltip: I18n.Config_Shops_CarpentersShop_Tooltip,
                getValue: () => config().CarpentersShop,
                setValue: value => config().CarpentersShop = value
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Shops_ForestSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_MarniesAnimalShop_Name,
                tooltip: I18n.Config_Shops_MarniesAnimalShop_Tooltip,
                getValue: () => config().MarniesAnimalShop,
                setValue: value => config().MarniesAnimalShop = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_TravelingMerchantShop_Name,
                tooltip: I18n.Config_Shops_TravelingMerchantShop_Tooltip,
                getValue: () => config().TravelingMerchantShop,
                setValue: value => config().TravelingMerchantShop = value
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Shops_DesertSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_SandyOasisShop_Name,
                tooltip: I18n.Config_Shops_SandyOasisShop_Tooltip,
                getValue: () => config().SandyOasisShop,
                setValue: value => config().SandyOasisShop = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_DesertTraderShop_Name,
                tooltip: I18n.Config_Shops_DesertTraderShop_Tooltip,
                getValue: () => config().DesertTraderShop,
                setValue: value => config().DesertTraderShop = value
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Shops_GingerIslandSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_ResortBarShop_Name,
                tooltip: I18n.Config_Shops_ResortBarShop_Tooltip,
                getValue: () => config().ResortBarShop,
                setValue: value => config().ResortBarShop = value
            );

            configMenu.AddSectionTitle(
                mod: manifest,
                text: I18n.Config_Shops_NightMarketSection
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_NightMarketPainterShop_Name,
                tooltip: I18n.Config_Shops_NightMarketPainterShop_Tooltip,
                getValue: () => config().NightMarketPainterShop,
                setValue: value => config().NightMarketPainterShop = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_NightMarketMagicBoatShop_Name,
                tooltip: I18n.Config_Shops_NightMarketMagicBoatShop_Tooltip,
                getValue: () => config().NightMarketMagicBoatShop,
                setValue: value => config().NightMarketMagicBoatShop = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_NightMarketTravelingMarchantShop_Name,
                tooltip: I18n.Config_Shops_NightMarketTravelingMarchantShop_Tooltip,
                getValue: () => config().NightMarketTravelingMerchantShop,
                setValue: value => config().NightMarketTravelingMerchantShop = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: I18n.Config_Shops_NightMarketDecorationBoatShop_Name,
                tooltip: I18n.Config_Shops_NightMarketDecorationBoatShop_Tooltip,
                getValue: () => config().NightMarketDecorationBoatShop,
                setValue: value => config().NightMarketDecorationBoatShop = value
            );
        }
    }
}
