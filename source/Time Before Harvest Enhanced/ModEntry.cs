/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/enom/time-before-harvest-enhanced
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace TimeBeforeHarvestEnhanced
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private ITranslationHelper I18n;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            I18n = helper.Translation;

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            GameLocation location = Game1.player.currentLocation;

            if (!location.IsFarm)
                return;

            if (e.Button != Config.TriggerButton)
                return;

            ICursorPosition pos = Helper.Input.GetCursorPosition();
            Vector2 vector = new((int) pos.Tile.X, (int) pos.Tile.Y);

            location.terrainFeatures.TryGetValue(vector, out TerrainFeature terrain);

            if (terrain is HoeDirt dirt)
            {
                Crop crop = dirt.crop;

                if (crop != null)
                {
                    if (crop.fullyGrown.Value == true)
                    {
                        if (crop.dayOfCurrentPhase.Value == 0)
                        {
                            CropReady();
                        }
                        else
                        {
                            int growProgress = crop.regrowAfterHarvest.Value - crop.dayOfCurrentPhase.Value;

                            CropDaysRemaining(days: crop.regrowAfterHarvest.Value - growProgress);
                        }
                    }
                    else
                    {
                        if (crop.currentPhase.Value >= crop.phaseDays.Count() - 1 && crop.dayOfCurrentPhase.Value == 0)
                        {
                            CropReady();
                        }
                        else
                        {
                            int totalDays = crop.phaseDays.Take(crop.phaseDays.Count() - 1).Sum();
                            int growProgress = crop.phaseDays.Take(crop.currentPhase.Value).Sum() + crop.dayOfCurrentPhase.Value;

                            if (growProgress >= totalDays)
                            {
                                CropReady();
                            }
                            else
                            {
                                CropDaysRemaining(days: totalDays - growProgress);
                            }
                        }
                    }
                }
                else
                {
                    CropNotHovered();
                }
            }
            else
            {
                CropNotHovered();
            }
        }

        /// <summary>Creates a popup message with the number of days remaining until next harvest.</summary>
        /// <param name="days">Number of days remaining until next harvest.</param>
        private void CropDaysRemaining (int days)
        {
            string key = days > 1 ? "crop.remaining-days" : "crop.remaining-days.1";
            object tokens = new { days = days.ToString() };

            HUDMessage message = new(I18n.Get(key, tokens), HUDMessage.stamina_type);
            Game1.addHUDMessage(message);
        }

        /// <summary>Creates a popup message that no crop is hovered.</summary>
        private void CropNotHovered ()
        {
            HUDMessage message = new(I18n.Get("crop.not-hovered"), HUDMessage.error_type);
            Game1.addHUDMessage(message);
        }

        /// <summary>Creates a popup message that the crop is ready for harvest.</summary>
        private void CropReady ()
        {
            HUDMessage message = new(I18n.Get("crop.ready"), HUDMessage.achievement_type);
            Game1.addHUDMessage(message);
        }
    }
}
