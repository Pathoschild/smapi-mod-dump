using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

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

            //track each relevant StardewValley.Object at the player's current location
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in Game1.currentLocation.objects.Pairs)
            {
                if
                (
                    (MConfig.TrackDefaultForage && pair.Value.isSpawnedObject.Value) //if this is a spawned object to track
                    || (MConfig.TrackArtifactSpots && pair.Value.ParentSheetIndex == 590) //or if this an artifact spot to track
                    || TrackedObjectIDs.Contains(pair.Value.parentSheetIndex) //or if this object's ID is being tracked
                    || TrackedObjectNames.Contains(pair.Value.Name.ToLower()) //or if this object's name is being tracked
                    || TrackedObjectNames.Contains(pair.Value.DisplayName.ToLower()) //or if this object's display name is being tracked
                )
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

            //track the location's panning spot, if applicable
            if (MConfig.TrackPanningSpots && Game1.currentLocation.orePanPoint.Value != Point.Zero) //if an ore panning location should be tracked
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

            //track spring onions, if applicable
            if (MConfig.TrackSpringOnions) //if spring onions should be tracked
            {
                foreach (var feature in Game1.currentLocation.terrainFeatures.Values) //for each of this location's terrain features
                {
                    if (feature is HoeDirt dirt && dirt.crop?.whichForageCrop.Value == Crop.forageCrop_springOnion) //if this terrain feature has a spring onion
                    {
                        if (ForageIconMode) //if this is rendering forage icons
                        {
                            SpriteSource = GameLocation.getSourceRectForObject(399); //get the spring onion spritesheet source rectangle (using its hard-coded ID)

                            if (Background != null) //if a background was successfully loaded
                            {
                                BackgroundSource = new Rectangle(0, 0, Background.Width, Background.Height); //create a source rectangle covering the entire background spritesheet
                            }
                        }
                        else //if this is rendering the custom tracker
                        {
                            SpriteSource = new Rectangle(0, 0, Spritesheet.Width, Spritesheet.Height); //create a source rectangle covering the entire tracker spritesheet
                        }

                        DrawTracker(feature.currentTileLocation); //draw a tracker for this spring onion
                    }
                }
            }

            //track harvestable berry bushes, if applicable
            if (MConfig.TrackBerryBushes) //if harvestable berry bushes should be tracked
            {
                foreach (var feature in Game1.currentLocation.largeTerrainFeatures) //for each of this location's large terrain features
                {
                    if (feature is Bush bush) //if this feature is a bush
                    {
                        if (bush.size != 3 && bush.townBush.Value == false && bush.tileSheetOffset.Value == 1 && bush.inBloom(Game1.currentSeason, Game1.dayOfMonth)) //if the bush will drop a berry when shaken (based on code from the Bush.shake method)
                        {
                            if (ForageIconMode) //if this is rendering forage icons
                            {
                                int index = 296; //the object ID to display; default to salmonberry
                                if (Game1.currentSeason == "fall") //if the current season is fall
                                    index = 410; //use the blackberry ID

                                SpriteSource = GameLocation.getSourceRectForObject(index); //get the berry type's spritesheet source rectangle

                                if (Background != null) //if a background was successfully loaded
                                {
                                    BackgroundSource = new Rectangle(0, 0, Background.Width, Background.Height); //create a source rectangle covering the entire background spritesheet
                                }
                            }
                            else //if this is rendering the custom tracker
                            {
                                SpriteSource = new Rectangle(0, 0, Spritesheet.Width, Spritesheet.Height); //create a source rectangle covering the entire tracker spritesheet
                            }

                            DrawTracker(bush.tilePosition.Value); //draw a tracker for this berry bush
                        }
                    }
                }
            }
        }
    }
}
