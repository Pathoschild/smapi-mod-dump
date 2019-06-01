using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;


namespace CropColorCombiner
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == this.Config.Unify_Color_Keybind)
            {
                this.Monitor.Log("Caught button pressed event; commencing color unification", LogLevel.Trace);

                foreach (Item i in Game1.player.Items)
                {
                    if (i is ColoredObject c)
                    {
                        if (c.Category == -80)
                        {
                            // -80 indicates "Flower"
                            if (this.Config.Unify_Color_For_Flowers)
                            {
                                c.color.Value = default;
                            }
                        }
                        else
                        {
                            if (this.Config.Unify_Color_For_Everything_Else)
                            {
                                c.color.Value = default;
                            }
                        }
                    }
                }
            }
            else if (e.Button == this.Config.Reduce_Quality_Keybind)
            {
                this.Monitor.Log("Caught button pressed event; commencing quality reduction", LogLevel.Trace);

                foreach (Item i in Game1.player.Items)
                {
                    if (i is StardewValley.Object o)
                    {
                        if (o.Category == -80)
                        {
                            // -80 indicates "Flower"
                            if (this.Config.Reduce_Quality_For_Flowers)
                            {
                                o.Quality = 0;
                            }
                        }
                        else
                        {
                            if (this.Config.Reduce_Quality_For_Everything_Else)
                            {
                                o.Quality = 0;
                            }
                        }
                    }
                }
            }
        }
    }
}