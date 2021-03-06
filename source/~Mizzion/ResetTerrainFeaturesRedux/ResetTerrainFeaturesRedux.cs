/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResetTerrainFeaturesRedux.Framework;
using ResetTerrainFeaturesRedux.Framework.Configs;
using ResetTerrainFeaturesRedux.Framework.Menu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ResetTerrainFeaturesRedux
{
    public class ResetTerrainFeaturesRedux : Mod
    {
        public bool debug = false;
        public Config _config;
        public override void Entry(IModHelper helper)
        {
            DoLog.monitor = Monitor;
            _config = helper.ReadConfig<Config>();

            helper.Events.Input.ButtonPressed += ButtonPressed;
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if ((Game1.currentLocation != null || debug) && !Game1.menuUp && Game1.activeClickableMenu == null &&
                e.IsDown(_config.MenuActivationKey))
                Game1.activeClickableMenu = new Menu(20, 20, 200, 200);
        }
    }
}
