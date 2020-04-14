using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTracker
{
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed at the start of each in-game day.</summary>
        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!MConfig.ReplaceTrackersWithForageIcons) //if the forage icons should NOT be used
            {
                try
                {
                    Spritesheet = Game1.content.Load<Texture2D>(TrackerLoadString); //load the custom tracker spritesheet
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to load the custom tracker texture \"{TrackerLoadString}\". There may be a problem with the Content Patcher pack or its settings.", LogLevel.Warn);
                    Monitor.Log($"Forage icons will be displayed instead. Original error message:", LogLevel.Warn);
                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                    return;
                }
            }

            if (MConfig.ReplaceTrackersWithForageIcons || Spritesheet == null) //if the forage icons should be used (due to settings OR because the custom tracker failed to load)
            {
                ForageIconMode = true; //enable forage icon mode
                Spritesheet = Game1.objectSpriteSheet; //get the object spritesheet

                try
                {
                    Background = Game1.content.Load<Texture2D>(BackgroundLoadString); //load the forage icons' custom background
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Failed to load the forage mode background texture \"{BackgroundLoadString}\". There may be a problem with the Content Patcher pack or its settings.", LogLevel.Warn);
                    Monitor.Log($"Forage icons will be displayed without a background. Original error message:", LogLevel.Warn);
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
