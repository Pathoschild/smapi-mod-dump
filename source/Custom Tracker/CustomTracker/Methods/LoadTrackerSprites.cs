/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomTracker
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;

namespace CustomTracker
{
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {
        /// <summary>Loads the currently selected tracking icon spritesheet to <see cref="Spritesheet"/> and updates <see cref="ForageIconMode"/>.</summary>
        public void LoadTrackerSprites()
        {
            if (!Config.ReplaceTrackersWithForageIcons) //if the forage icons should NOT be used
            {
                try
                {
                    Spritesheet = Game1.content.Load<Texture2D>(TrackerLoadString); //load the custom tracker spritesheet
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to load the custom tracker texture \"{TrackerLoadString}\". There might be a problem with the \"[CP] CustomTracker\" folder.", LogLevel.Warn);
                    Monitor.Log($"Forage icons will be displayed instead. Full error message:", LogLevel.Warn);
                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                    return;
                }
            }

            if (Config.ReplaceTrackersWithForageIcons || Spritesheet == null) //if the forage icons should be used (due to settings OR because the custom tracker failed to load)
            {
                ForageIconMode = true; //enable forage icon mode
                Spritesheet = Game1.objectSpriteSheet; //get the object spritesheet

                try
                {
                    Background = Game1.content.Load<Texture2D>(BackgroundLoadString); //load the forage icons' custom background
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to load the forage mode background texture \"{BackgroundLoadString}\". There might be a problem with the \"[CP] CustomTracker\" folder.", LogLevel.Warn);
                    Monitor.Log($"Forage icons will be displayed without a background. Full error message:", LogLevel.Warn);
                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                    return;
                }
            }
            else
            {
                ForageIconMode = false; //disable forage icon mode
            }
        }
    }
}
