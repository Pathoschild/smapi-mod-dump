/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AdeelTariq/ObjectProgressBars
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace ObjectProgressBars
{
    public class ModEntry : Mod
    {

        private static readonly string STONE = "Stone";
        private static readonly string INCUBATOR = "Incubator";
        private static readonly int COOPMASTER = 2;

        private Configuration config;
        private bool showing = true;

        private static readonly Dictionary<string, int> MACHINE_TIMES = new Dictionary<string, int>();
        static ModEntry() {
            MACHINE_TIMES.Add("Bee House", 6100);
            MACHINE_TIMES.Add("Cheese Press", 200);
            MACHINE_TIMES.Add("Loom", 240);
            MACHINE_TIMES.Add("Mayonnaise Machine", 180);
            MACHINE_TIMES.Add("Preserves Jar", 4000);
            MACHINE_TIMES.Add("Charcoal Kiln", 30);
            MACHINE_TIMES.Add("Recycling Machine", 60);
            MACHINE_TIMES.Add("Seed Maker", 20);
            MACHINE_TIMES.Add("Slime Egg-Press", 1260);
            MACHINE_TIMES.Add("Lightning Rod ", 24 * 60);

            MACHINE_TIMES.Add("Keg_Beer", 2250);
            MACHINE_TIMES.Add("Keg_Pale Ale", 2360);
            MACHINE_TIMES.Add("Keg_Wine", 10000);
            MACHINE_TIMES.Add("Keg_Juice", 6000);
            MACHINE_TIMES.Add("Keg_Mead", 600);
            MACHINE_TIMES.Add("Keg_Coffee", 120);

            MACHINE_TIMES.Add("Crystalarium_Diamond", 5 * 24 * 60);
            MACHINE_TIMES.Add("Crystalarium_Star Shards", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Helvite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Neptunite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Fire Opal", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Bixite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Dolomite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Tigerseye", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Emerald", 2 * 24 * 60 + 2 * 60);
            MACHINE_TIMES.Add("Crystalarium_Ruby", 2 * 24 * 60 + 2 * 60);
            MACHINE_TIMES.Add("Crystalarium_Kyanite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Fairy Stone", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Ocean Stone", 3 * 24 * 60 + 11 * 60 + 20);       
            MACHINE_TIMES.Add("Crystalarium_Jade", 1 * 24 * 60 + 16 * 60    );
            MACHINE_TIMES.Add("Crystalarium_Fluorapatite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Lunarite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Lemon Stone", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Ghost Crystal", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Obsidian", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Aquamarine", 1 * 24 * 60 + 13 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Basalt", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Opal", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Alamite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Geminite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Jamborite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Jasper", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Hematite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Aerinite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Celestine", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Petrified Slime", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Pyrite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Soapstone", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Jagoite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Marble", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Amethyst", 22 * 60 + 40);
            MACHINE_TIMES.Add("Crystalarium_Fire Quartz", 21 * 60 + 40);
            MACHINE_TIMES.Add("Crystalarium_Esperite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Malachite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Thunder Egg", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Slate", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Topaz", 18 * 60 + 40);
            MACHINE_TIMES.Add("Crystalarium_Nekoite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Orpiment", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Frozen Tear", 18 * 60 + 40);
            MACHINE_TIMES.Add("Crystalarium_Calcite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Granite", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Sandstone", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Earth Crystal", 13 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Baryte", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Quartz", 7 * 60);
            MACHINE_TIMES.Add("Crystalarium_Mudstone", 3 * 24 * 60 + 11 * 60 + 20);
            MACHINE_TIMES.Add("Crystalarium_Limestone", 3 * 24 * 60 + 11 * 60 + 20);
                     
            MACHINE_TIMES.Add("Tapper_Maple Syrup", 8 * 24 * 60 + 12);
            MACHINE_TIMES.Add("Tapper_Oak Resin", 7 * 24 * 60 + 12);
            MACHINE_TIMES.Add("Tapper_Pine Tar", 5 * 24 * 60 + 12);

            MACHINE_TIMES.Add("Furnace_Copper Bar", 30);
            MACHINE_TIMES.Add("Furnace_Iron Bar", 120);
            MACHINE_TIMES.Add("Furnace_Gold Bar", 300);
            MACHINE_TIMES.Add("Furnace_Refined Quartz", 90);
            MACHINE_TIMES.Add("Furnace_Iridium Bar", 480);

            MACHINE_TIMES.Add("Incubator_Large Egg", 9000);
            MACHINE_TIMES.Add("Incubator_Void Egg", 9000);
            MACHINE_TIMES.Add("Incubator_Duck Egg", 9000);
            MACHINE_TIMES.Add("Incubator_Dinosaur Egg", 18000);
            MACHINE_TIMES.Add("Slime Incubator_Slime Egg", 4000);

            MACHINE_TIMES.Add("Machine_", 20);

        }

        private static Dictionary<string, int> GUESSED_MACHINE_TIMES = new Dictionary<string, int>();

  
        public override void Entry(IModHelper helper)
        {
            this.config = this.Helper.ReadConfig<Configuration>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.RenderingHud += this.Draw;
            this.showing = this.config.DisplayProgressBars;
        }


        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (Context.IsPlayerFree && e.Button.Equals(config.ToggleDisplay.key.ToSButton())) {
                this.showing = !this.showing;
                this.config.DisplayProgressBars = this.showing;
                this.Helper.WriteConfig(this.config);
            }
        }

        private void Draw(object sender, EventArgs args)
        {

            Vector2 size1 = Utility.ModifyCoordinatesForUIScale(new Vector2(52, 20));
            Vector2 size2 = Utility.ModifyCoordinatesForUIScale(new Vector2(48, 16));
            Vector2 size3 = Utility.ModifyCoordinatesForUIScale(new Vector2(44, 12));
            Vector2 size4 = Utility.ModifyCoordinatesForUIScale(new Vector2(40, 8));
            Vector2 size6 = Utility.ModifyCoordinatesForUIScale(new Vector2(4, 8));

            if (this.showing && Context.IsWorldReady && Context.IsPlayerFree)
            {
                SpriteBatch spriteBatch = Game1.spriteBatch;
                foreach (Vector2 item in (IEnumerable<Vector2>)(Game1.currentLocation.netObjects).Keys) {
                    StardewValley.Object gameObject = Game1.currentLocation.netObjects[item];
                    if (gameObject.heldObject.Value != null && gameObject.MinutesUntilReady > 0 && gameObject.MinutesUntilReady != 999999) {

                        if (gameObject.Name.Equals (STONE)) {
                            continue;
                        }
                        
                        float x = item.X;
                        float y = item.Y;
                        Vector2 val2 = Game1.GlobalToLocal(Game1.uiViewport, new Vector2(x * 64, y * 64));
                        x = val2.X;
                        y = val2.Y;

                        x += 12; // adjusting the position a bit

                        float percentage = 0;

                        if (MACHINE_TIMES.ContainsKey(gameObject.Name)) {   // check if a predetermined time is stored for this machine
                            float totalMachineTime = MACHINE_TIMES[gameObject.Name] / 10.0f;
                            percentage = (totalMachineTime - gameObject.MinutesUntilReady / 10.0f) / totalMachineTime;
                        
                        } else if (MACHINE_TIMES.ContainsKeyPattern(gameObject.Name + "_" + ((StardewValley.Object)gameObject.heldObject).Name)) {  // check if a predetermined time is stored for this machine and the item it holds
                            float totalMachineTime = MACHINE_TIMES.GetItemByKeyPattern(gameObject.Name 
                            + "_" + ((StardewValley.Object)gameObject.heldObject).Name) / 10.0f;

                            if (gameObject.Name.Contains(INCUBATOR) && Game1.player.professions.Contains(COOPMASTER)) {
                                totalMachineTime = totalMachineTime / 2.0f;
                            }

                            percentage = (totalMachineTime - gameObject.MinutesUntilReady / 10.0f) / totalMachineTime;

                        } else if (GUESSED_MACHINE_TIMES.ContainsKey(gameObject.Name + "_" + gameObject.TileLocation)) {    // check if a guessed time is stored for this machine at the current tile location 
                            // (location used as a unique identifier, maybe problemtic if the same location is occupied in a different map)

                            float totalMachineTime = GUESSED_MACHINE_TIMES[gameObject.Name + "_" + gameObject.TileLocation] / 10.0f;
       
                            percentage = (totalMachineTime - gameObject.MinutesUntilReady / 10.0f) / totalMachineTime;

                        } else {    // save a guessed time for this machine at this location
                            GUESSED_MACHINE_TIMES.Add (gameObject.Name + "_" + 
                                                       gameObject.TileLocation, gameObject.MinutesUntilReady);
                        }

                        if (percentage > 1) {   // don't show progress bar for 100 or more percentage
                            continue;
                        }
                        Vector2 pos1 = Utility.ModifyCoordinatesForUIScale(new Vector2(x - 6, y - 6));
                        Vector2 pos2 = Utility.ModifyCoordinatesForUIScale(new Vector2(x - 4, y - 4));
                        Vector2 pos3 = Utility.ModifyCoordinatesForUIScale(new Vector2(x - 2, y - 2));
                        Vector2 pos4 = Utility.ModifyCoordinatesForUIScale(new Vector2(x, y));
                        Vector2 pos5 = Utility.ModifyCoordinatesForUIScale(new Vector2(x + (36f * percentage), y));

                        Vector2 size5 = Utility.ModifyCoordinatesForUIScale(new Vector2(40f * percentage, 8));


                        spriteBatch.Draw(Game1.staminaRect, new Rectangle(pos1.ToPoint(), size1.ToPoint()), (Rectangle)((Texture2D)Game1.staminaRect).Bounds, new Color(0.357f, 0.169f, 0.165f), 0f, Vector2.Zero, (SpriteEffects)0, 0.887f);
                        spriteBatch.Draw(Game1.staminaRect, new Rectangle(pos2.ToPoint(), size2.ToPoint()), (Rectangle)((Texture2D)Game1.staminaRect).Bounds, new Color(0.863f, 0.482f, 0.02f), 0f, Vector2.Zero, (SpriteEffects)0, 0.887f);
                        spriteBatch.Draw(Game1.staminaRect, new Rectangle(pos3.ToPoint(), size3.ToPoint()), (Rectangle)((Texture2D)Game1.staminaRect).Bounds, new Color(0.694f, 0.306f, 0.02f), 0f, Vector2.Zero, (SpriteEffects)0, 0.887f);
                        spriteBatch.Draw(Game1.staminaRect, new Rectangle(pos4.ToPoint(), size4.ToPoint()), (Rectangle)((Texture2D)Game1.staminaRect).Bounds, new Color (1.0f, 0.843f, 0.537f), 0f, Vector2.Zero, (SpriteEffects)0, 0.887f);
                        spriteBatch.Draw(Game1.staminaRect, new Rectangle(pos4.ToPoint(), size5.ToPoint()), (Rectangle)((Texture2D) Game1.staminaRect).Bounds, Utility.getRedToGreenLerpColor(percentage), 0f, Vector2.Zero, (SpriteEffects) 0, 0.887f);

                        Color progressColor = Utility.getRedToGreenLerpColor(percentage);
                        Vector3 colorVector = progressColor.ToVector3();
                        colorVector.X = DarkenColor (colorVector.X); colorVector.Y = DarkenColor(colorVector.Y); colorVector.Z = DarkenColor(colorVector.Z);
                        Color darkenedColor = new Color(colorVector);
                        spriteBatch.Draw(Game1.staminaRect, new Rectangle(pos5.ToPoint(), size6.ToPoint()), (Rectangle)((Texture2D)Game1.staminaRect).Bounds, darkenedColor, 0f, Vector2.Zero, (SpriteEffects) 0, 0.887f);
                                          
                    } else if (gameObject.MinutesUntilReady == 0) { // remove from guessed times
                        if (!string.Equals($"{gameObject.heldObject}", "null")) {
                            GUESSED_MACHINE_TIMES.Remove(gameObject.Name + "_" + gameObject.TileLocation);
                        }
                    }
                }
            }
        }

        private float DarkenColor (float color) {
            int toSubtract = 50;
            int intColor = (int)(color * 255);
            intColor = intColor - toSubtract;
            if (intColor < 0) {
                return 0;
            }
            return intColor / 255.0f;
        }

    }
}