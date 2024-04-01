/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/desert-bloom-farm
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Desert_Bloom.API
{
    public class GenericModConfigMenuHandler
    {
        public static Mod Mod;
        public static IMonitor Monitor;
        public static IModHelper Helper;
        public static ModConfig Config;
        public static void main()
        {
            Mod = ModEntry.Mod;
            Monitor = ModEntry._Monitor;
            Helper = ModEntry._Helper;
            Config = ModEntry.Config;

            Helper.Events.GameLoop.GameLaunched += gameLaunched;
            Helper.Events.GameLoop.DayStarted += dayStarted;
        }

        private static void dayStarted(object sender, DayStartedEventArgs e)
        {
            checkFieldsChanges();
        }

        private static void checkFieldsChanges()
        {
            if (!ModEntry.IsMyFarm())
                return;

            var farm = Game1.getFarm();
            var props = farm.Map.Properties;

            if (Config.PlayDesertTune && !props.ContainsKey("Music"))
                props.Add("Music", "wavy");
            else if (!Config.PlayDesertTune && props.ContainsKey("Music"))
                props.Remove("Music");
        }

        private static void gameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: Mod.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(ModEntry.Config)
            );

            configMenu.AddBoolOption(
                mod: Mod.ModManifest,
                name: () => "Play Desert Tune",
                getValue: () => Config.PlayDesertTune,
                setValue: value => Config.PlayDesertTune = value,
                fieldId: "PlayDesertTune"
                );

            configMenu.OnFieldChanged(Mod.ModManifest, delegate { checkFieldsChanges(); });
        }
    }
}
