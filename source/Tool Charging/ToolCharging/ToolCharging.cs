/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mralbobo/stardew-tool-charging
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using System.IO;

namespace ToolCharging
{
    public class ToolChargingMainClass : Mod
    {
        private int extraRemove = 40;
        //public ToolChargingConfig config;

        public override void Entry(IModHelper helper)
        {
            ModConfig config = helper.ReadConfig<ModConfig>();

            //config = new ToolChargingConfig().InitializeConfig(BaseConfigPath);
            extraRemove = config.IncreaseBy;
            debugLog("extraRemove" + extraRemove);
            StardewModdingAPI.Events.GameEvents.UpdateTick += Event_Update;
        }

        private void debugLog(String theString)
        {
#if DEBUG
            this.Monitor.Log(theString);
#endif

        }

        private void Event_Update(object sender, EventArgs e)
        {
            if(Game1.player != null)
            {
                int hold = Game1.player.toolHold;
                if(!Game1.player.canReleaseTool || hold <= 0) { return; } //either maxed or not being held

                if (hold - extraRemove <= 0)
                {
                    Game1.player.toolHold = 1;
                }
                else {
                    Game1.player.toolHold -= extraRemove;
                }
            }
        }

    }

    public class ModConfig
    {
        private int _increaseBy;
        public int IncreaseBy {
            get { return _increaseBy; }
            set {
                if (value < -15) { _increaseBy = -15;}
                else if(value > 599) { _increaseBy = 599; }
                else { _increaseBy = value; }
            }
        }

        public ModConfig()
        {
            this._increaseBy = 40;
        }

    }
}
