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
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Event;
using StardewDruid.Journal;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Minecarts;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using xTile.Tiles;
using static StardewValley.Minigames.TargetGame;
using static System.Net.WebRequestMethods;

namespace StardewDruid.Cast.Weald
{
    public class Cultivate : EventHandle
    {

        public int radialCounter = 0;

        public Cultivate()
        {

        }

        public override bool EventActive()
        {

            if (!inabsentia && !eventLocked)
            {

                if (Mod.instance.Config.riteButtons.GetState() != SButtonState.Held)
                {

                    return false;

                }

                if (Vector2.Distance(origin, Game1.player.Position) > 32)
                {

                    return false;

                }

            }
            
            return base.EventActive();

        }

        public override void EventDecimal()
        {

            if (!EventActive())
            {

                RemoveAnimations();

                return;

            }

            if (!inabsentia && !eventLocked)
            {
                
                decimalCounter++;

                if (decimalCounter == 5)
                {

                    TemporaryAnimatedSprite skyAnimation = Mod.instance.iconData.SkyIndicator(location, origin, IconData.skies.valley, 1f, new() { interval = 1000,});

                    skyAnimation.scaleChange = 0.002f;

                    skyAnimation.motion = new(-0.064f, -0.064f);

                    skyAnimation.timeBasedMotion = true;

                    animations.Add(skyAnimation);

                }

                if (decimalCounter == 15)
                {

                    eventLocked = true;

                    SpellHandle spellHandle = new(origin, 320, IconData.impacts.nature, new());

                    Mod.instance.spellRegister.Add(spellHandle);

                }

                return;

            }

            Cultivation();

            radialCounter++;

            if (radialCounter == 9)
            {

                expireEarly = true;
            
            }

        }

        public void Cultivation()
        {

            int cultivationCost = 2;

            if(Game1.player.FarmingLevel > 5)
            {

                cultivationCost = 1;
            }

            if (!Mod.instance.rite.targetCasts.ContainsKey(location.Name))
            {

                Mod.instance.rite.targetCasts[location.Name] = new();

            }

            List<Vector2> affected = ModUtility.GetTilesWithinRadius(location, ModUtility.PositionToTile(origin), radialCounter);

            foreach(Vector2 tile in affected)
            {

                if (!location.terrainFeatures.ContainsKey(tile))
                {

                    continue;

                }

                // =========================================================================
                // crops

                if (location.terrainFeatures[tile] is StardewValley.TerrainFeatures.HoeDirt hoeDirt)
                {

                    // ----------------------------------------------------------------------
                    // check recasts

                    if (Mod.instance.rite.targetCasts[location.Name].ContainsKey(tile))
                    {

                        if (Mod.instance.rite.targetCasts[location.Name][tile].Contains("Crop"))
                        {

                            if (hoeDirt.crop != null)
                            {

                                string cropName = "Crop" + hoeDirt.crop.indexOfHarvest.Value.ToString();

                                if (cropName == Mod.instance.rite.targetCasts[location.Name][tile])
                                {

                                    continue;

                                }

                            }

                        }
                        else if (Mod.instance.rite.targetCasts[location.Name][tile] == "Hoed")
                        {

                            if (hoeDirt.crop == null)
                            {

                                continue;

                            }

                        }

                    }
 
                    // ----------------------------------------------------------------------
                    // cultivate crop

                    if (hoeDirt.crop != null)
                    {

                        if (hoeDirt.crop.dead.Value)
                        {

                            hoeDirt.destroyCrop(true);

                            if (Game1.currentSeason == "winter" && !location.IsGreenhouse)
                            {

                                Mod.instance.rite.targetCasts[location.Name][tile] = "Hoed";

                                continue;

                            }

                            string wildSeed = "498";

                            switch (Game1.currentSeason)
                            {

                                case "spring":

                                    wildSeed = "495";
                                    break;

                                case "summer":

                                    wildSeed = "496";
                                    break;

                                case "fall":

                                    wildSeed = "497";
                                    break;

                            }

                            hoeDirt.plant(wildSeed, Game1.player, false);

                        }

                    }

                    if (!hoeDirt.HasFertilizer())
                    {

                        int powerLevel = Mod.instance.PowerLevel;

                        if (Mod.instance.Config.cultivateBehaviour == 1)
                        {
                            
                            if (powerLevel >= 4)
                            {

                                hoeDirt.plant("918", Game1.player, true);

                            }
                            else
                            {

                                hoeDirt.plant("466", Game1.player, true);

                            }

                        }
                        else if (Mod.instance.Config.cultivateBehaviour == 2)
                        {

                            if (powerLevel >= 4)
                            {

                                hoeDirt.plant("466", Game1.player, true);

                            }
                            else
                            {

                                hoeDirt.plant("465", Game1.player, true);

                            }

                        }
                        else if (Mod.instance.Config.cultivateBehaviour == 3)
                        {

                            if (powerLevel >= 4)
                            {

                                hoeDirt.plant("919", Game1.player, true);

                            }
                            else
                            {

                                hoeDirt.plant("369", Game1.player, true);

                            }

                        }

                    }

                    if (hoeDirt.crop == null)
                    {

                        Mod.instance.rite.targetCasts[location.Name][tile] = "Hoed";

                        continue;

                    }

                    if (hoeDirt.crop.isWildSeedCrop() && hoeDirt.crop.currentPhase.Value <= 1 && (Game1.currentSeason != "winter" || location.isGreenhouse.Value))
                    {

                        int quality = Mod.instance.questHandle.IsComplete(QuestHandle.wealdFour) ? Mod.instance.Config.cultivateBehaviour : 0;

                        ModUtility.UpgradeCrop(hoeDirt, Game1.player, location, quality);

                        if (hoeDirt.crop == null)
                        {

                            Mod.instance.rite.targetCasts[location.Name][tile] = "Hoed";

                            continue;

                        }

                        if (!Mod.instance.questHandle.IsComplete(QuestHandle.wealdFour))
                        {

                            Mod.instance.questHandle.UpdateTask(QuestHandle.wealdFour, 1);

                        }

                    }

                    if (hoeDirt.crop.currentPhase.Value <= 1)
                    {

                        if(Mod.instance.Config.cultivateBehaviour <= 2)
                        {

                            hoeDirt.crop.currentPhase.Value = 2;

                            hoeDirt.crop.dayOfCurrentPhase.Value = 0;

                            hoeDirt.crop.updateDrawMath(hoeDirt.Tile);

                        }
                        else if (hoeDirt.crop.currentPhase.Value == 0)
                        {

                            hoeDirt.crop.currentPhase.Value = 1;

                            hoeDirt.crop.dayOfCurrentPhase.Value = 0;

                            hoeDirt.crop.updateDrawMath(hoeDirt.Tile);
                        }

                    }

                    Mod.instance.rite.targetCasts[location.Name][tile] = "Crop" + hoeDirt.crop.indexOfHarvest.Value.ToString();

                    if (!inabsentia)
                    {

                        Mod.instance.rite.castCost += cultivationCost;

                    }

                    Vector2 cursorVector = tile * 64 + new Vector2(32, 40);

                    Microsoft.Xna.Framework.Color randomColour;

                    switch (Mod.instance.randomIndex.Next(3))
                    {

                        case 0:

                            randomColour = Color.LightGreen;
                            break;

                        case 1:

                            randomColour = Color.White;
                            break;

                        default:

                            randomColour = Color.YellowGreen;
                            break;

                    }

                    Mod.instance.iconData.ImpactIndicator(location, cursorVector, IconData.impacts.glare, 0.75f + Mod.instance.randomIndex.Next(5)*0.25f, new() { color = randomColour,});

                }

                // =========================================================================
                // other features

                if (Mod.instance.rite.targetCasts[location.Name].ContainsKey(tile))
                {

                    continue;

                }

                if (location.terrainFeatures[tile] is StardewValley.TerrainFeatures.FruitTree fruitTree)
                {

                    fruitTree.dayUpdate();

                    Mod.instance.rite.targetCasts[location.Name][tile] = "Tree";

                    if (!inabsentia)
                    {

                        Mod.instance.rite.castCost += cultivationCost;

                    }

                    Vector2 cursorVector = tile * 64 + new Vector2(0,8);

                    Mod.instance.iconData.CursorIndicator(location, cursorVector, IconData.cursors.weald, new());

                }

                if (inabsentia)
                {

                    continue;

                }

                if (location.terrainFeatures[tile] is StardewValley.TerrainFeatures.Tree treeFeature)
                {

                    if (treeFeature.growthStage.Value < 3)
                    {

                        treeFeature.growthStage.Value++;

                    }
                    else
                    {

                        treeFeature.dayUpdate();

                    }

                    Mod.instance.rite.targetCasts[location.Name][tile] = "Tree";

                    if (!inabsentia)
                    {

                        Mod.instance.rite.castCost += cultivationCost;

                    }

                    Vector2 cursorVector = tile * 64 + new Vector2(0,8);

                    Mod.instance.iconData.CursorIndicator(location, cursorVector, IconData.cursors.weald, new());

                }

            }

            // ---------------------------------------------
            // Large Feature iteration
            // ---------------------------------------------

            if(radialCounter != 8)
            {

                return;

            }

            if (location.largeTerrainFeatures.Count > 0)
            {

                foreach (LargeTerrainFeature largeTerrainFeature in location.largeTerrainFeatures)
                {

                    if (largeTerrainFeature is not StardewValley.TerrainFeatures.Bush bushFeature)
                    {

                        continue;

                    }

                    if (bushFeature.size.Value == 3)
                    {

                        int age = bushFeature.getAge();

                        if (age < 20)
                        {

                            Vector2 teaTile = bushFeature.Tile;

                            if (Mod.instance.rite.targetCasts[location.Name].ContainsKey(teaTile))
                            {

                                continue;

                            }

                            if (Vector2.Distance(bushFeature.getBoundingBox().Center.ToVector2(),origin) <= 512)
                            {

                                int newdate = Math.Max(1, (bushFeature.datePlanted.Value - 1));

                                bushFeature.datePlanted.Set(newdate);

                                Mod.instance.rite.targetCasts[location.Name][teaTile] = "bush";

                            }

                            return;

                        }

                    }

                }

            }

        }

    }

}
