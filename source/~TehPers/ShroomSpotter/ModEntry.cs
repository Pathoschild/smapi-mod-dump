using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TehPers.Stardew.ShroomSpotter {

    public class ModEntry : Mod {
        public static ModEntry INSTANCE;

        public ModConfig Config;
        public List<UpdateEvent> UpdateEvents = new List<UpdateEvent>();
        public delegate bool UpdateEvent();

        public ModEntry() {
            ModEntry.INSTANCE = this;
        }

        public override void Entry(IModHelper helper) {
            this.Config = helper.ReadConfig<ModConfig>();
            if (!this.Config.ModEnabled) return;

            //this.Monitor.Log("It is *HIGHLY* recommended you install a Health Bars mod for enemies!", LogLevel.Info);

            ControlEvents.KeyPressed += this.KeyPressed;
            GraphicsEvents.OnPreRenderGuiEvent += this.OnPreRenderGuiEvent;
            GraphicsEvents.OnPostRenderGuiEvent += this.OnPostRenderGuiEvent;
            //MenuEvents.MenuChanged += MenuEvents_MenuChanged;
        }

        #region Events
        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            if (Enum.TryParse(this.Config.GetShroomLevels, out Keys getShroomLevels) && e.KeyPressed == getShroomLevels) {
                // Find all shroom levels
                List<int> shroomLevels = new List<int>();
                int daysTilShroom = -1;
                while (shroomLevels.Count == 0 && ++daysTilShroom < 50) shroomLevels = this.GetShroomLayers(daysTilShroom);

                if (shroomLevels.Count > 0) {
                    if (daysTilShroom == 0)
                        Game1.showGlobalMessage("Shroom layers will spawn on these mine levels: " + string.Join<int>(", ", shroomLevels));
                    else
                        Game1.showGlobalMessage("Shrooms will spawn in " + daysTilShroom + " day(s) on these mine levels: " + string.Join<int>(", ", shroomLevels));
                } else Game1.showGlobalMessage("No shroom layers will spawn in the next 50 days!");
            }
        }

        private void OnPreRenderGuiEvent(object sender, EventArgs e) {
            if (Game1.activeClickableMenu is Billboard) {
                Billboard menu = (Billboard) Game1.activeClickableMenu;
                FieldInfo calendarField = menu.GetType().GetField("calendarDays", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (calendarField == null) {
                    this.Monitor.Log("Could not find field 'calendarDays' in Billboard!", LogLevel.Error);
                    return;
                }
                List<ClickableTextureComponent> calendarDays = (List<ClickableTextureComponent>) calendarField.GetValue(menu);
                IPrivateField<string> hoverField = this.Helper.Reflection.GetPrivateField<string>(menu, "hoverText");
                string hoverText = hoverField.GetValue();
                if (calendarDays != null && !(hoverText.Contains("Shrooms") || hoverText.Contains("shrooms"))) {
                    for (int day = 1; day <= 28; day++) {
                        ClickableTextureComponent component = calendarDays[day - 1];
                        if (component.bounds.Contains(Game1.getMouseX(), Game1.getMouseY())) {
                            List<int> shrooms = this.GetShroomLayers(day - Game1.dayOfMonth);

                            if (hoverText.Length > 0)
                                hoverText += "\n";

                            if (shrooms.Count > 0)
                                hoverText += "Shrooms: " + string.Join(", ", shrooms);
                            else
                                hoverText += "No shrooms";

                            break;
                        }
                    }

                    hoverField.SetValue(hoverText);
                }
            }
        }

        private void OnPostRenderGuiEvent(object sender, EventArgs e) {
            if (Game1.activeClickableMenu is Billboard) {
                Billboard menu = (Billboard) Game1.activeClickableMenu;
                FieldInfo calendarField = menu.GetType().GetField("calendarDays", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (calendarField == null) {
                    this.Monitor.Log("Could not find field 'calendarDays' in Billboard!", LogLevel.Error);
                    return;
                }
                List<ClickableTextureComponent> calendarDays = (List<ClickableTextureComponent>) calendarField.GetValue(menu);
                if (calendarDays == null) return;
                string hoverText = this.Helper.Reflection.GetPrivateValue<string>(menu, "hoverText");
                SpriteBatch b = Game1.spriteBatch;

                for (int day = 1; day <= 28; day++) {
                    ClickableTextureComponent component = calendarDays[day - 1];
                    List<int> shrooms = this.GetShroomLayers(day - Game1.dayOfMonth);

                    if (shrooms.Count > 0) {
                        const int id = 422;
                        Rectangle source = Game1.currentLocation.getSourceRectForObject(id);
                        Vector2 dest = new Vector2(component.bounds.X, component.bounds.Y + 10f * Game1.pixelZoom);
                        b.Draw(Game1.objectSpriteSheet, dest, new Rectangle?(source), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom / 2f, SpriteEffects.None, 1f);
                    }
                }

                IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
        }
        #endregion

        public List<int> GetShroomLayers(int relativeDay) {
            List<int> shroomLevels = new List<int>();
            for (int mineLevel = 1; mineLevel < 120; mineLevel++) {
                Random random = new Random((int) Game1.stats.DaysPlayed + relativeDay + mineLevel + (int) Game1.uniqueIDForThisGame / 2);

                // Simulate all the random values grabbed before the shrooms
                if (random.NextDouble() < 0.3 && mineLevel > 2) random.NextDouble();
                random.NextDouble();
                if (random.NextDouble() < 0.035 && mineLevel >= 80 && mineLevel <= 120 && mineLevel % 5 != 0)
                    shroomLevels.Add(mineLevel);
            }

            return shroomLevels;
        }
    }
}
