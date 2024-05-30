/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewValley.Delegates;
using StardewValley.Events;
using StardewValley.TokenizableStrings;
using StardewValley.Triggers;

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TestingTools
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig? Config;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            //var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            helper.Events.GameLoop.UpdateTicked += EveryTick;
            helper.Events.Input.ButtonPressed += OnPress;
            helper.Events.GameLoop.DayEnding += OnDayEnd;
        }

        private void EveryTick(object? sender, UpdateTickedEventArgs e)
        {
            //if(Game1.CurrentEvent != null) Monitor.Log(Game1.CurrentEvent.id, LogLevel.Debug);
            //IReflectedMethod busLeave = Helper.Reflection.GetMethod(BusStop.busDriveOff());
            //IReflectedMethod busLeave = Helper.Reflection.GetMethod(Game1.location);
        }

        private void OnPress(object? sender, ButtonPressedEventArgs e)
        {
            if(e.Button == SButton.J)
            {
                if(Game1.currentLocation is BusStop)
                {
                    BusStop bs = (BusStop)Game1.currentLocation;
                    bs.busDriveOff();
                }
                if(Game1.currentLocation is Desert)
                {
                    Desert ds = (Desert)Game1.currentLocation;
                    ds.busDriveOff();
                }
            }

            if(e.Button == SButton.B)
            {
                
            }
        }

        private void OnDayEnd(object? sender, DayEndingEventArgs e)
        {

        }

        /*public void Listen(Action act)
        {
            Helper.Events.GameLoop.UpdateTicked += act;
        }*/
    }
}
