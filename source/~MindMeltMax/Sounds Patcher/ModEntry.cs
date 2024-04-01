/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using SoundsPatcher.Utility;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace SoundsPatcher
{
    public class ModEntry : Mod
    {
        public static IModHelper IHelper;
        public static IMonitor IMonitor;
        public static Config IConfig;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;
            IConfig = Helper.ReadConfig<Config>();

            if (IConfig.Sounds is null || IConfig.Sounds.Count <= 0)
                IConfig.Sounds = Utilities.GetSoundsDict();
            if (IConfig.Songs is null || IConfig.Songs.Count <= 0)
                IConfig.Songs = Utilities.GetSongsDict();
            IConfig.UnknownSounds ??= new();
            IConfig.MenuKeys ??= KeybindList.Parse("O, RightStick");

            helper.Events.GameLoop.GameLaunched += (_, _) => Patches.Patch(helper.ModRegistry.ModID);
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            Helper.Events.GameLoop.Saving += (_, _) => Helper.WriteConfig(IConfig);
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree) 
                return;
            if (IConfig.MenuKeys.JustPressed())
                Game1.activeClickableMenu = new SoundsMenu(IHelper, IMonitor, IConfig);
        }
    }
}
