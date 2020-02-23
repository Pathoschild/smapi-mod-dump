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
        /// <summary>Renders all necessary custom trackers for the current frame.</summary>
        /// <param name="forageIcon">If true, render the targeted forage object instead of the custom tracker icon.</param>
        public void RenderCustomTrackers(bool forageIcon = false)
        {
            if (!Context.IsPlayerFree) //if the world isn't ready or the player isn't free
                return;

            if (!MConfig.EnableTrackersWithoutProfession && !Game1.player.professions.Contains(17)) //if the player needs to unlock the Tracker profession
                return;

            if (!Game1.currentLocation.IsOutdoors || Game1.eventUp || Game1.farmEvent != null) //if the player is indoors or an event is happening
                return;

            //render custom trackers for each relevant object at the player's current location
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in Game1.currentLocation.objects.Pairs)
            {
                if (pair.Value.isSpawnedObject.Value || pair.Value.ParentSheetIndex == 590) //if this is a "spawned object" or a buried artifact
                {
                    if (ForageIconMode) //if this is rendering forage icons
                    {
                        SpriteSource = GameLocation.getSourceRectForObject(pair.Value.ParentSheetIndex); //get this object's spritesheet source rectangle

                        if (Background != null) //if a background was successfully loaded
                        {
                            BackgroundSource = new Rectangle(0, 0, Background.Width, Background.Height); //create a source rectangle covering the entire background spritesheet
                        }
                    }
                    else //if this is rendering the custom tracker
                    {
                        SpriteSource = new Rectangle(0, 0, Spritesheet.Width, Spritesheet.Height); //create a source rectangle covering the entire tracker spritesheet
                    }

                    DrawTracker(pair.Key); //draw a tracker for this object
                }
            }

            if (Game1.currentLocation.orePanPoint.Value != Point.Zero) //if the current location has an ore panning site
            {
                Texture2D objectSheet = Spritesheet; //store the spritesheet, in case it needs to be changed during this process

                if (ForageIconMode) //if this is rendering forage icons
                {
                    Spritesheet = Game1.currentLocation.orePanAnimation.Texture; //get the ore animation's spritesheet
                    SpriteSource = Game1.currentLocation.orePanAnimation.sourceRect; //get the ore animation's current source rectangle

                    if (Background != null) //if a background was successfully loaded
                    {
                        BackgroundSource = new Rectangle(0, 0, Background.Width, Background.Height); //create a source rectangle covering the entire background spritesheet
                    }
                }
                else //if this is rendering the custom tracker
                {
                    SpriteSource = new Rectangle(0, 0, Spritesheet.Width, Spritesheet.Height); //create a source rectangle covering the entire tracker spritesheet
                }

                Vector2 panVector = new Vector2(Game1.currentLocation.orePanPoint.Value.X, Game1.currentLocation.orePanPoint.Value.Y); //convert the point into a vector
                DrawTracker(panVector); //draw a tracker for the panning site

                Spritesheet = objectSheet; //restore the previous spritesheet, in case it was changed for this process
            }
        }
    }
}
