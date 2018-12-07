using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BetterActivateSprinklers
{
    public class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            if (Config.ActivateOnAction)
            {
                InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            }

            if (Config.ActivateOnPlacement)
            {
                LocationEvents.ObjectsChanged += this.LocationEvents_ObjectsChanged;
            }
        }

        private void LocationEvents_ObjectsChanged(object sender, EventArgsLocationObjectsChanged e)
        {
            foreach (var pair in e.Added)
            {
                var obj = pair.Value;

                if (obj.Name.Contains("Sprinkler"))
                {
                    obj.DayUpdate(Game1.currentLocation);
                }
            }
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (e.IsActionButton)
            {
                var tile = e.Cursor.GrabTile;
                if (tile == null) return;

                var obj = Game1.currentLocation.getObjectAtTile((int) tile.X, (int) tile.Y);
                if (obj == null) return;
                
                if (obj.Name.Contains("Sprinkler"))
                {
                    obj.DayUpdate(Game1.currentLocation);
                }
            }
        }
    }
}
