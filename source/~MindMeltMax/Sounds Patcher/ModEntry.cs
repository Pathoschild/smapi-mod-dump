/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Sounds_Patcher.Patches;
using Sounds_Patcher.Utility;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sounds_Patcher
{
    public class ModEntry : Mod
    {
        public static IModHelper StaticHelper;
        public static IMonitor StaticMonitor;
        public static Config StaticConfig;

        public override void Entry(IModHelper helper)
        {
            StaticHelper = Helper;
            StaticMonitor = Monitor;
            StaticConfig = Helper.ReadConfig<Config>();

            if (StaticConfig.Sounds == null || StaticConfig.Sounds.Count <= 0)
                StaticConfig.Sounds = Utilities.GetSoundsDict();
            if (StaticConfig.Songs == null || StaticConfig.Songs.Count <= 0)
                StaticConfig.Songs = Utilities.GetSongsDict();
            if (string.IsNullOrWhiteSpace(StaticConfig.MenuKey))
                StaticConfig.MenuKey = "O";

            Helper.WriteConfig(StaticConfig);

            Patcher.Init(helper);

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree) return;
            if (IsButtonValid(e.Button))
                Game1.activeClickableMenu = new SoundsMenu(StaticHelper, StaticMonitor, StaticConfig);
        }

        private bool IsButtonValid(SButton button)
        {
            string buttonAsString = button.ToString().ToLower();
            return ((IEnumerable<string>)StaticConfig.MenuKey.ToLower().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Any(Item => buttonAsString.Equals(Item.Trim()));
        }
    }
}
