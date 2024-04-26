/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomTracker
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

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

            if (!Config.EnableTrackersWithoutProfession && !Game1.player.professions.Contains(Farmer.tracker)) //if the player needs to unlock the Tracker profession
                return;

            if ((!Config.EnableTrackingIndoors && !Game1.currentLocation.IsOutdoors) || Game1.eventUp || Game1.farmEvent != null) //if tracking is disabled due to the player being indoors, or because an event is active
                return;

            //track each relevant StardewValley.Object at the player's current location
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in Game1.currentLocation.objects.Pairs)
            {
                if
                (
                    (Config.TrackDefaultForage && pair.Value.IsSpawnedObject) //if this is a spawned object to track
                    || (Config.TrackArtifactSpots && (pair.Value.QualifiedItemId is "(O)590" or "(O)SeedSpot")) //or if this an artifact/seed spot
                    || TrackedObjectIDs.Contains(pair.Value.ParentSheetIndex) //or if this object's ID is being tracked
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
            if (Config.TrackPanningSpots && Game1.currentLocation.orePanPoint.Value != Point.Zero) //if an ore panning location should be tracked
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
            if (Config.TrackSpringOnions) //if spring onions should be tracked
            {
                foreach (var feature in Game1.currentLocation.terrainFeatures.Values) //for each of this location's terrain features
                {
                    if (feature is HoeDirt dirt && dirt.crop?.whichForageCrop.Value == Crop.forageCrop_springOnionID) //if this terrain feature has a spring onion
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

                        DrawTracker(feature.Tile); //draw a tracker for this spring onion
                    }
                }
            }

            //track harvestable berry bushes, if applicable
            if (Config.TrackBerryBushes) //if harvestable berry bushes should be tracked
            {
                foreach (var feature in Game1.currentLocation.largeTerrainFeatures) //for each of this location's large terrain features
                {
                    if (feature is Bush bush) //if this feature is a bush
                    {
                        if (bush.size.Value != Bush.greenTeaBush //if this is NOT a tea bush
                            && (Config.TrackWalnutBushes || bush.size.Value != Bush.walnutBush) //AND this bush is NOT excluded by the walnut setting
                            && bush.townBush.Value == false //AND this is NOT flagged as a town bush
                            && bush.tileSheetOffset.Value == 1 //AND this bush is currently displaying berries/walnuts
                            && bush.inBloom()) //AND this bush is currently blooming
                        {
                            if (ForageIconMode) //if this is rendering forage icons
                            {
                                int index; //the object ID to display

                                if (bush.size.Value == Bush.walnutBush) //if this is a walnut bush
                                    index = 73; //use the walnut ID
                                else if (Game1.currentSeason.Equals("fall", StringComparison.OrdinalIgnoreCase)) //else if the current season is fall
                                    index = 410; //use the blackberry ID
                                else
                                    index = 296; //use the salmonberry ID

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

                            DrawTracker(bush.Tile); //draw a tracker for this berry bush
                        }
                    }
                }
            }
        }
    }
}
