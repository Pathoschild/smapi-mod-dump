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
                Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            }

            if (Config.ActivateOnPlacement)
            {
                Helper.Events.World.ObjectListChanged += this.OnWorld_ObjectListChanged;
            }
        }

        private void OnWorld_ObjectListChanged(object sender, ObjectListChangedEventArgs e)
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

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.IsActionButton())
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
