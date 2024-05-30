/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley;
using System;
using System.Collections.Generic;
using StardewDruid.Data;
using StardewDruid.Journal;
using System.Linq;
using StardewValley.TerrainFeatures;
using Netcode;

namespace StardewDruid.Cast.Mists
{
    public class Artifice
    {

        public Artifice()
        {


        }

        public void CastActivate(Vector2 castVector)
        {

            GameLocation location = Game1.player.currentLocation;

            string locationName = location.Name;

            if (!Mod.instance.rite.specialCasts.ContainsKey(locationName))
            {

                Mod.instance.rite.specialCasts[locationName] = new();

            }

            List<Vector2> tileVectors;

            int casts = 0;

            Vector2 warpVector = WarpData.WarpTiles(location);

            Vector2 fireVector = WarpData.FireVectors(location);

            for (int i = 0; i < 4; i++)
            {

                tileVectors = ModUtility.GetTilesWithinRadius(location, castVector, i);

                List<Vector2> betweenVectors = ModUtility.GetTilesBetweenPositions(location, Game1.player.Position, castVector * 64);

                tileVectors.AddRange(betweenVectors);

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (warpVector != Vector2.Zero && !Mod.instance.rite.specialCasts[locationName].Contains("warp"))
                    {

                        if (tileVector == warpVector)
                        {

                            int targetIndex = WarpData.WarpTotems(location);

                            int extractionChance = Mod.instance.randomIndex.Next(1, 3);

                            for (int t = 0; t < extractionChance; t++)
                            {

                                ThrowHandle throwObject = new(Game1.player, warpVector * 64, targetIndex, 0);

                                throwObject.register();

                            }

                            Vector2 boltVector = new(warpVector.X, warpVector.Y - 2);

                            Mod.instance.iconData.AnimateBolt(location, boltVector * 64 + new Vector2(32));

                            Mod.instance.rite.specialCasts[locationName].Add("warp");

                            casts++;

                        }

                    }

                    if (fireVector != Vector2.Zero && !Mod.instance.rite.specialCasts[locationName].Contains("fire"))
                    {

                        if (tileVector == fireVector)
                        {

                            Torch campFire = new("278", true)
                            {
                                Fragility = 1,
                                destroyOvernight = true
                            };

                            if (location.objects.ContainsKey(fireVector))
                            {

                                location.objects.Remove(fireVector);

                            }

                            location.objects.Add(fireVector, campFire);

                            Game1.playSound("fireball");

                            Mod.instance.iconData.AnimateBolt(location, fireVector * 64 + new Vector2(32));

                            Mod.instance.rite.specialCasts[locationName].Add("fire");

                            casts++;


                        }

                    }

                    if (location.objects.Count() > 0)
                    {

                        if (location.objects.ContainsKey(tileVector))
                        {

                            StardewValley.Object targetObject = location.objects[tileVector];

                            if (location.IsFarm && targetObject.bigCraftable.Value && targetObject.ParentSheetIndex == 9)
                            {

                                if (targetObject.MinutesUntilReady > 1)
                                {
                                    continue;
                                }

                                for (int j = 0; j <= Mod.instance.PowerLevel; j++)
                                {

                                    if (Mod.instance.rite.specialCasts[locationName].Contains("rod" + j.ToString()))
                                    {

                                        continue;

                                    }

                                    targetObject.heldObject.Value = new StardewValley.Object("787", 1);

                                    targetObject.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);

                                    targetObject.shakeTimer = 1000;

                                    Mod.instance.iconData.AnimateBolt(location, tileVector * 64 + new Vector2(32));

                                    Mod.instance.rite.specialCasts[locationName].Add("rod" + j.ToString());

                                    casts++;

                                    break;

                                }

                            }
                            else if (targetObject.Name.Contains("Campfire"))
                            {

                                string fireLocation = location.Name;

                                if (!Mod.instance.rite.specialCasts[locationName].Contains("campfire"))
                                {

                                    location.objects.Remove(tileVector);

                                    Torch campFire = new("278", true)
                                    {
                                        Fragility = 1,
                                        destroyOvernight = true
                                    };

                                    location.objects.Add(tileVector, campFire);

                                    Game1.playSound("fireball");

                                    Mod.instance.iconData.AnimateBolt(location, tileVector * 64 + new Vector2(32));

                                    Mod.instance.rite.specialCasts[locationName].Add("campfire");

                                    casts++;

                                }
                            }
                            else if (targetObject.IsScarecrow())
                            {

                                string scid = "scarecrow_" + tileVector.X.ToString() + "_" + tileVector.Y.ToString();

                                if (!Game1.isRaining && !Mod.instance.rite.specialCasts[locationName].Contains(scid))
                                {

                                    ArtificeScarecrow(location,tileVector);

                                    Mod.instance.rite.castCost += Math.Max(8, 32 - Game1.player.FarmingLevel * 3);

                                    Mod.instance.rite.specialCasts[locationName].Add(scid);

                                    casts++;

                                }

                            }
                            else if(targetObject.QualifiedItemId == "(BC)MushroomLog")
                            {

                                for (int j = 0; j <= Mod.instance.PowerLevel; j++)
                                {

                                    if (Mod.instance.rite.specialCasts[locationName].Contains("mushroomlog" + j.ToString()))
                                    {

                                        continue;

                                    }

                                    if (targetObject.MinutesUntilReady == 0)
                                    {
                                        
                                        targetObject.DayUpdate();
    
                                    }

                                    targetObject.shakeTimer = 1000;

                                    targetObject.MinutesUntilReady = 1;

                                    Mod.instance.iconData.AnimateBolt(location, tileVector * 64 + new Vector2(32));

                                    Mod.instance.rite.specialCasts[locationName].Add("mushroomlog" + j.ToString());

                                    casts++;

                                    break;

                                }

                            }
                            /*else if(targetObject.QualifiedItemId.Contains("93"))
                            {

                                Utility.trySpawnRareObject(Game1.player, tileVector * 64, location, 9.0);

                                location.objects.Remove(tileVector);

                                Mod.instance.questHandle.AssignQuest("relicsMists");

                                //new ThrowHandle(Game1.player, tileVector * 64, WarpData.WarpTotems(Game1.getLocationFromName("Beach")), 0).register();

                                StardewDruid.Monster.Boss newBoss = new StardewDruid.Monster.DarkShooter(tileVector, Mod.instance.CombatDifficulty());

                                location.characters.Add(newBoss);

                                newBoss.update(Game1.currentGameTime, location);

                                newBoss = new StardewDruid.Monster.DarkBrute(tileVector + new Vector2(0, 1), Mod.instance.CombatDifficulty());

                                location.characters.Add(newBoss);

                                newBoss.update(Game1.currentGameTime, location);

                                newBoss = new StardewDruid.Monster.DarkBrute(tileVector + new Vector2(-1, 1), Mod.instance.CombatDifficulty());

                                location.characters.Add(newBoss);

                                newBoss.update(Game1.currentGameTime, location);

                                newBoss = new StardewDruid.Monster.DarkBrute(tileVector + new Vector2(1, 1), Mod.instance.CombatDifficulty());

                                location.characters.Add(newBoss);

                                newBoss.update(Game1.currentGameTime, location);

                            }*/

                            continue;

                        }

                    }

                }

            }

            if(casts > 0)
            {

                if (!Mod.instance.questHandle.IsComplete(QuestHandle.mistsTwo))
                {

                    Mod.instance.questHandle.UpdateTask(QuestHandle.mistsTwo, casts);

                }

            }

        }

        public void ArtificeScarecrow(GameLocation location, Vector2 targetVector)
        {

            bool animate = true;

            if (Game1.player.currentLocation.Name != location.Name && !Utility.isOnScreen(targetVector * 64, 0))
            {

                animate = false;

            }

            int radius = ((int)(Mod.instance.PowerLevel) + 2);

            radius = Math.Min(8, radius);

            for (int i = 1; i < radius; i++)
            {

                List<Vector2> hoeVectors = ModUtility.GetTilesWithinRadius(Game1.player.currentLocation, targetVector, i);

                foreach (Vector2 hoeVector in hoeVectors)
                {

                    if (Game1.player.currentLocation.terrainFeatures.ContainsKey(hoeVector))
                    {

                        var terrainFeature = Game1.player.currentLocation.terrainFeatures[hoeVector];

                        if (terrainFeature is HoeDirt)
                        {

                            HoeDirt hoeDirt = terrainFeature as HoeDirt;

                            if (hoeDirt.state.Value == 0)
                            {

                                hoeDirt.state.Value = 1;

                                if (animate)
                                {

                                    TemporaryAnimatedSprite newAnimation = new(
                                        "TileSheets\\animations",
                                        new(0, 51 * 64, 64, 64), 
                                        75f, 
                                        8, 
                                        1,
                                        new(hoeVector.X * 64 + 10, hoeVector.Y * 64 + 10), 
                                        false, 
                                        false,
                                        hoeVector.X * 1000 + hoeVector.Y, 
                                        0f,
                                        new(0.8f, 0.8f, 1f, 1f), 
                                        0.7f, 
                                        0f, 
                                        0f, 
                                        0f)
                                    {

                                        delayBeforeAnimationStart = (i * 200) + 200,

                                    };

                                    Game1.currentLocation.temporarySprites.Add(newAnimation);


                                }

                            }

                        }

                    }

                    continue;

                }

            }

            if (animate)
            {

                Mod.instance.iconData.AnimateBolt(Game1.player.currentLocation, targetVector * 64 - new Vector2(0, 96));

                Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, targetVector * 64 - new Vector2(0, 416), IconData.impacts.cloud, 3f, new() { interval = 125, color = Microsoft.Xna.Framework.Color.White, flip = true, layer = 997f, });

                Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, targetVector * 64 - new Vector2(0, 416), IconData.impacts.cloud, 4f, new() { interval = 125, color = Microsoft.Xna.Framework.Color.Gray, layer = 996f, });

            }

            return;

        }

    }

}
