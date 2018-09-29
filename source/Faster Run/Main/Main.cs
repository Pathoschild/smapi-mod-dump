using System;
using StardewModdingAPI;
using StardewValley;

namespace Main
{
    public class Main : Mod
    {
        ModConfig config;
        int addedSpeed;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();
            StardewModdingAPI.Events.SaveEvents.AfterLoad += Event_AfterLoad;
        }

        private void Event_AfterLoad(object sender, EventArgs e)
        {
            int totalSpeed = config.runSpeed;

            if (totalSpeed > 1)
            {
                addedSpeed = totalSpeed - 5;
                Monitor.Log("FasterRun run speed is set to " + totalSpeed, LogLevel.Debug);
                StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
            }
            else
            {
                Monitor.Log("Speed value of " + config.runSpeed + " provided in config.JSON" + "is an invalid speed value. Only intergers (whole numbers) that are higher than 0 are allowed.", LogLevel.Error);
            }
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            Game1.player.addedSpeed = addedSpeed;
        }
    }

    class ModConfig
    {
        public int runSpeed = 7;
    }
}
