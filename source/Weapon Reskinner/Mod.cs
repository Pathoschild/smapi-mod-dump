using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceShared;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace WeaponReskinner
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        public static Configuration Config;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = Monitor;

            Config = Helper.ReadConfig<Configuration>();

            Helper.Events.GameLoop.GameLaunched += onGameLaunched;
            helper.Events.Input.ButtonPressed += onButtonPressed;
        }

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (gmcm != null)
            {
                gmcm.RegisterModConfig(this.ModManifest, () => Config = new Configuration(), () => Helper.WriteConfig(Config));
                gmcm.RegisterSimpleOption(ModManifest, "Key", "The key to open the reskinning window.", () => Config.Key, (k) => Config.Key = k);
            }
        }

        private void onButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree || Game1.activeClickableMenu != null)
                return;

            if ( e.Button == Config.Key )
            {
                Game1.activeClickableMenu = new WeaponReskinMenu();
            }
        }
    }
}
