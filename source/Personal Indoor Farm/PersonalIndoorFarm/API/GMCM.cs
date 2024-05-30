/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericModConfigMenu;
using GMCMOptions;
using PersonalIndoorFarm;
using static PersonalIndoorFarm.ModEntry;

namespace PersonalIndoorFarm.API
{
    internal class GMCM
    {
        public static void Initialize()
        {
            Helper.Events.GameLoop.GameLaunched += GameLaunched;
        }

        private static void GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
               mod: ModManifest,
               reset: () => Config = new ModConfig(),
               save: () => Helper.WriteConfig(Config)
           );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Use Vanilla Doors",
                tooltip: () => "Whether Vanilla Decorative Doors should act as Dimension Doors",
                getValue: () => Config.UseVanillaDoors,
                setValue: value => Config.UseVanillaDoors = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Use VMV Doors",
                tooltip: () => "Whether Visit Mount Vapius Decorative Doors should act as Dimension Doors",
                getValue: () => Config.UseVMVDoors,
                setValue: value => Config.UseVMVDoors = value
            );

            var configMenuExt = Helper.ModRegistry.GetApi<IGMCMOptionsAPI>("jltaylor-us.GMCMOptions");
            if (configMenuExt is null)
                return;

            configMenuExt.AddColorOption(
                    mod: ModManifest,
                    name: () => "Locked Door Color",
                    tooltip: () => "Accessibility option. Changes the color of the locked door icon.",
                    getValue: () => Config.LockedDoorColor,
                    setValue: value => Config.LockedDoorColor = value,
                    colorPickerStyle: (uint)IGMCMOptionsAPI.ColorPickerStyle.RGBSliders
                );

            configMenuExt.AddColorOption(
                    mod: ModManifest,
                    name: () => "Unlocked Door Color",
                    tooltip: () => "Accessibility option. Changes the color of the unlocked door icon.",
                    getValue: () => Config.UnlockedDoorColor,
                    setValue: value => Config.UnlockedDoorColor = value,
                    colorPickerStyle: (uint)IGMCMOptionsAPI.ColorPickerStyle.RGBSliders
                );
        }
    }
}
