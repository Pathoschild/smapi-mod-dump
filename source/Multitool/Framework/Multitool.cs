/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miome/MultitoolMod
**
*************************************************/

using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Tools;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;
using xTile.ObjectModel;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;
using SObject = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using MultitoolMod;
using StardewValley.GameData.Crops;
using System.ComponentModel.Design;
using static StardewValley.Minigames.CraneGame;

// TODO: Add Summaries and param desc to all functions


namespace MultitoolMod.Framework
{
    public class Multitool : Tool
    {
        public IDictionary<string, Tool> attachedTools;
        public MultitoolMod mod;
        public ICursorPosition cursor;

        public Multitool(MultitoolMod m)
        {
            this.mod = m;
            this.attachedTools = new Dictionary<string, Tool>();
            attachedTools["axe"] = new Axe();
            attachedTools["pickaxe"] = new Pickaxe();
            attachedTools["melee"] = new MeleeWeapon("47");
            attachedTools["scythe"] = new MeleeWeapon("47");
            attachedTools["scythe"].Category = -99;
            attachedTools["wateringcan"] = new WateringCan();
            attachedTools["hoe"] = new Hoe();
        }

        protected override Item GetOneNew()
        {
            return new Multitool(null);
        }
        protected override string loadDisplayName()
        {
            return "A tool for all trades";
        }
        protected override string loadDescription()
        {
            return "A tool for all trades";
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            this.Refresh_Tools();
            Tool tool;
            IDictionary<string, object> properties = this.Get_Properties(x, y);
            string toolName = "";
            int xtile = (int)x / Game1.tileSize;
            int ytile = (int)y / Game1.tileSize;

            toolName = (string)properties["string_useTool"];
            if (toolName == null)
            {
                if ((bool)properties["bool_canPlant"])
                {
                    try
                    {
                        if (Game1.player.CurrentItem != null)
                        {
                            HoeDirt dirt = (HoeDirt)properties["hoedirt_dirt"];
                            if (Game1.player.CurrentItem.Category == StardewValley.Object.SeedsCategory)
                            {
                                if (dirt.plant(Game1.player.CurrentItem.ItemId, Game1.player, false))
                                {
                                    //Only take item if planting was successful
                                    Game1.player.Items.ReduceId(Game1.player.CurrentItem.QualifiedItemId, 1);
                                }
                                else
                                {
                                    Game1.addHUDMessage(new HUDMessage($"{Game1.player.CurrentItem.DisplayName} can't be planted here."));
                                }
                            }
                            else if (Game1.player.CurrentItem.Category == StardewValley.Object.fertilizerCategory)
                            {
                                if (dirt.plant(Game1.player.CurrentItem.ItemId, Game1.player, true))
                                {
                                    Game1.player.Items.ReduceId(Game1.player.CurrentItem.QualifiedItemId, 1);
                                }
                                else
                                {
                                    Game1.addHUDMessage(new HUDMessage($"{Game1.player.CurrentItem.DisplayName} can't be placed here."));
                                }
                            }
                        }
                        else
                        {
                            location.checkAction(new Location(xtile, ytile), Game1.viewport, Game1.player);
                        }
                    }
                    catch (System.Collections.Generic.KeyNotFoundException)
                    {
                        Game1.addHUDMessage(new HUDMessage($"{Game1.player.CurrentItem} {Game1.player.CurrentItem.QualifiedItemId} 201"));
                        return;
                    }
                }
                return;
            }
            try
            {
                tool = this.attachedTools[toolName];
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                if (toolName == "grab")
                {
                    location.checkAction(new Location(xtile, ytile), Game1.viewport, Game1.player);
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage($"{toolName} not found"));
                }
                return;
            }
            if (toolName == "scythe")
            {
                MeleeWeapon scythe = (MeleeWeapon)attachedTools["scythe"];
                if ((bool)properties["bool_fullyGrownCrop"] || (bool)properties["bool_hasDeadCrop"])
                {
                    HoeDirt dirt = (HoeDirt)properties["hoedirt_dirt"];
                    who.playNearbySoundLocal("swordswipe");
                    dirt.performToolAction(scythe, 1, new Vector2(xtile, ytile));
                    return;
                }
                if ((bool)properties["bool_isWeed"])
                {
                    SObject weed = (SObject)properties["object_tileObj"];
                    scythe.lastUser = who;
                    who.playNearbySoundLocal("swordswipe");
                    weed.performToolAction(scythe);
                    location.objects.Remove(new Vector2(xtile, ytile));
                    return;
                }
                //This doesn't land at the right location, don't use it
                //this.scythe.DoDamage(Game1.currentLocation, x, y, 0, power, who);
                return;
                /*
 * Attempted to get scythe an area. Doesn't land at correct location.
 * 
 * Vector2 vector = new Vector2(x,y);
Vector2 tileLocation = Vector2.Zero;
Vector2 tileLocation2 = Vector2.Zero;
Rectangle boundingBox = new Rectangle((int)vector.X + 8, (int)vector.Y + who.FarmerSprite.getHeight() - 32, 48, 32);
Rectangle areaOfEffect = this.scythe.getAreaOfEffect(x, y, who.FacingDirection, ref tileLocation, ref tileLocation2, who.GetBoundingBox(), who.FarmerSprite.currentAnimationIndex);
foreach (Vector2 item in Utility.removeDuplicates(Utility.getListOfTileLocationsForBordersOfNonTileRectangle(areaOfEffect)))
{
    if (location.terrainFeatures.TryGetValue(item, out var value) && value.performToolAction(this, 0, item))
    {
        location.terrainFeatures.Remove(item);
    }

    if (location.objects.TryGetValue(item, out var value2) && value2.performToolAction(this))
    {
        location.objects.Remove(item);
    }

    if (location.performToolAction(this, (int)item.X, (int)item.Y))
    {
        break;
    }
}*/
            }
            else
            {
                tool.DoFunction(location, x, y, power, who);
                return;
                /* From Game1.pressUseToolButton
                *       if (!(player.CurrentTool is MeleeWeapon) || didPlayerJustLeftClick(ignoreNonMouseHeldInput: true))
           {
               int facingDirection = player.FacingDirection;
               Vector2 toolLocation = player.GetToolLocation(position);
               player.FacingDirection = player.getGeneralDirectionTowards(new Vector2((int)toolLocation.X, (int)toolLocation.Y));
               player.lastClick = new Vector2((int)position.X, (int)position.Y);
               player.BeginUsingTool();
               if (!player.usingTool)
               {
                   player.FacingDirection = facingDirection;
               }
               else if (player.FarmerSprite.IsPlayingBasicAnimation(facingDirection, carrying: true) || player.FarmerSprite.IsPlayingBasicAnimation(facingDirection, carrying: false))
               {
                   player.FarmerSprite.StopAnimation();
               }
           } */
            }
        }

        public void Refresh_Tools()
        {
            foreach (Item item in Game1.player.Items)
            {
                if (item is Tool)
                {
                    if (((Tool)item).isScythe())
                    {
                        this.attachedTools["scythe"] = (MeleeWeapon)item;
                        return;
                    }
                    else
                    {
                        this.attachedTools[item.GetType().Name.ToLower()] = (Tool)item;
                    }
                }
            }
        }


        public IDictionary<string, System.Object> Get_Properties(int x, int y)
        {
            IDictionary<string, System.Object> properties = new Dictionary<string, System.Object>()
            {
                {"ResourceClump_clump", null},
                {"hoedirt_dirt", null},
                {"bool_canPlant", (System.Object)false},
                {"bool_fullyGrownCrop", (System.Object)false},
                {"bool_hasDeadCrop", (System.Object)false},
                {"bool_hasGrass", (System.Object)false},
                {"bool_hasLiveCrop", (System.Object)false},
                {"bool_hasTree", (System.Object)false},
                {"bool_isArtifactSpot", (System.Object)false},
                {"bool_isBoulder", (System.Object)false},
                {"bool_isDirt", (System.Object)false},
                {"bool_isHollowLog", (System.Object)false},
                {"bool_isMeteorite", (System.Object)false},
                {"bool_isMineRock", (System.Object)false},
                {"bool_isResourceClump", (System.Object)false},
                {"bool_isStone", (System.Object)false},
                {"bool_isStump", (System.Object)false},
                {"bool_isTilledDirt", (System.Object)false},
                {"bool_isTwig", (System.Object)false},
                {"bool_isWeed", (System.Object)false},
                {"bool_needsWater", (System.Object)false},
                {"object_tileObj", null},
                {"string_terrainFeatureName",null},
                {"string_tileObjName", null},
                {"string_useTool", null},
                {"type_tileObjType", null},
                {"crop_liveCrop", null},
                {"grass_Grass", null }
            };

            int xtile = (int)x / Game1.tileSize;
            int ytile = (int)y / Game1.tileSize;
            GameLocation location = Game1.player.currentLocation;
            Vector2 tileVec = new Vector2(xtile, ytile);

            location.terrainFeatures.TryGetValue(tileVec, out TerrainFeature tileFeature);
            location.objects.TryGetValue(tileVec, out SObject tileObj);
            properties["object_tileObj"] = tileObj;
            if (tileObj != null)
            {
                properties["type_tileObjType"] = (System.Object)tileObj.GetType();
            }
            properties["terrainFeature_tileFeature"] = tileFeature;
            ResourceClump clump = this.GetResourceClumpCoveringTile(location, tileVec);

            properties["bool_isResourceClump"] = (System.Object)clump != null;
            if ((bool)properties["bool_isResourceClump"])
            {
                properties["ResourceClump_clump"] = (System.Object)clump;
                switch (clump.parentSheetIndex.Get())
                {
                    //TODO: Add large crops
                    case ResourceClump.boulderIndex:
                        properties["string_useTool"] = (System.Object)"pickaxe";
                        properties["bool_isBoulder"] = (System.Object)true;
                        break;
                    case ResourceClump.hollowLogIndex:
                        properties["string_useTool"] = (System.Object)"axe";
                        properties["bool_isHollowLog"] = (System.Object)true;
                        break;
                    case ResourceClump.meteoriteIndex:
                        properties["string_useTool"] = (System.Object)"pickaxe";
                        properties["bool_isMeteorite"] = (System.Object)true;
                        break;
                    case ResourceClump.stumpIndex:
                        properties["string_useTool"] = (System.Object)"axe";
                        properties["bool_Stump"] = (System.Object)true;
                        break;
                    case ResourceClump.mineRock1Index:
                    case ResourceClump.mineRock2Index:
                    case ResourceClump.mineRock3Index:
                    case ResourceClump.mineRock4Index:
                        properties["string_useTool"] = (System.Object)"pickaxe";
                        properties["bool_isMineRock"] = (System.Object)true;
                        break;
                }
            }
            else if (tileFeature == null && tileObj == null)
            {
                properties["string_useTool"] = (System.Object)"hoe";
                properties["bool_isDirt"] = (System.Object)true;
            }
            else if (tileFeature == null && tileObj != null)
            {
                properties["string_tileObjName"] = (System.Object)tileObj.Name;
                if (tileObj.Name == "Twig")
                {
                    properties["string_useTool"] = (System.Object)"axe";
                    properties["bool_isTwig"] = (System.Object)true;
                }
                else if (tileObj.Name.ToLower().Contains("weed"))
                {
                    properties["string_useTool"] = (System.Object)"scythe";
                    properties["bool_isWeed"] = (System.Object)true;
                    properties["bool_isDirt"] = (System.Object)false;
                }
                else if (tileObj.Name == "Stone")
                {
                    properties["string_useTool"] = (System.Object)"pickaxe";
                    properties["bool_isStone"] = (System.Object)true;
                }
                else if (tileObj is StardewValley.Objects.IndoorPot pot)
                {
                    properties = Get_HoeDirtProperties(pot.hoeDirt.Get(), properties);
                }
                else if (tileObj.ParentSheetIndex == 590)
                {
                    properties["bool_isArtifactSpot"] = (System.Object)true;
                    properties["string_useTool"] = (System.Object)"hoe";
                }
            }
            else if (tileObj == null && tileFeature != null)
            {
                properties["terrainFeature_tileFeature"] = (System.Object)tileFeature;
                properties["string_terrainFeatureName"] = (System.Object)tileFeature.ToString();
                properties["bool_isWeed"] = (System.Object)false;
                if (tileFeature is HoeDirt dirt)
                {
                    properties["bool_isDirt"] = (System.Object)true;
                    properties["bool_isTilledDirt"] = (System.Object)true;
                    properties = Get_HoeDirtProperties(dirt, properties);

                }
                else
                {
                    properties["bool_hasDeadCrop"] = (System.Object)false;
                    properties["bool_hasLiveLCrop"] = (System.Object)false;

                    if (tileFeature is Tree tree)
                    {
                        properties["string_useTool"] = (System.Object)"axe";
                        properties["bool_hasTree"] = (System.Object)true;
                    }
                    else if (tileFeature is Grass grass)
                    {
                        properties["string_useTool"] = (System.Object)"scythe";
                        properties["bool_hasGrass"] = (System.Object)true;
                        properties["grass_Grass"] = grass;
                    }
                }
            }
            if (tileObj == null && location.doesTileHaveProperty(xtile, ytile, "Water", "Back") != null)
            {
                properties["bool_isWater"] = (System.Object)true;
                properties["string_useTool"] = (System.Object)"wateringcan";
            }

            return properties;
        }

        public IDictionary<string, System.Object> Get_HoeDirtProperties(HoeDirt dirt, IDictionary<string, System.Object> properties)
        {
            properties["hoedirt_dirt"] = dirt;
            if (dirt.crop != null)
            {
                properties["crop_Crop"] = (System.Object)dirt.crop;
                if (dirt.crop.dead.Get())
                {
                    properties["bool_hasDeadCrop"] = (System.Object)true;
                    properties["bool_hasLiveLCrop"] = (System.Object)false;
                    properties["string_useTool"] = (System.Object)"scythe";
                }
                else
                {
                    properties["bool_hasLiveLCrop"] = (System.Object)true;
                    int harvestablePhase = dirt.crop.phaseDays.Count - 1;
                    bool canHarvestNow = dirt.readyForHarvest();
                    properties["bool_fullyGrownCrop"] = (System.Object)canHarvestNow;
                    if (canHarvestNow)
                    {
                        if (dirt.crop.GetHarvestMethod() == HarvestMethod.Scythe)
                        {
                            properties["string_useTool"] = (System.Object)"scythe";
                        }
                        else
                        {
                            properties["string_useTool"] = (System.Object)"grab";
                        }
                    }
                    else if (dirt.needsWatering())
                    {
                        properties["bool_needsWater"] = (System.Object)true;
                        properties["string_useTool"] = (System.Object)"wateringcan";
                    }
                }
            }
            else
            {
                properties["bool_canPlant"] = (System.Object)true;
            }

            return properties;
        }
        public string Format_Properties(IDictionary<string, System.Object> properties)
        {
            /* 
                TODO: HUDMessage to small for text!
                Moo - Today at 6:59 AM
                Game1.chatBox.addInfoMessage ?
                Moo - Today at 7:03 AM
                Or you could maybe do something like spriteBatch.DrawString in 
                OnPostRenderHudEvent or something, to draw text directly on the 
                screen, if you want a constant display of cursor info rather 
                than something that appears when you press a key.
                */

            string formatted_output = "";
            foreach (KeyValuePair<string, System.Object> kvp in properties)
            {
                if (kvp.Value != null && !(kvp.Value is bool b && b == false))
                {
                    formatted_output += $"{kvp.Key} = {kvp.Value}, " + System.Environment.NewLine;
                }
            }
            return formatted_output;
        }
        /// This function provided by Protector
        /// https://github.com/maxvollmer
        /// <summary>Get resource clumps in a given location.</summary>
        /// <param name="location">The location to search.</param>
        public IEnumerable<ResourceClump> GetResourceClumps(GameLocation location)
        {
            foreach (FieldInfo field in location.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.GetValue(location) is IEnumerable<ResourceClump> enumerable)
                {
                    foreach (ResourceClump resourceClump in enumerable)
                    {
                        if (resourceClump != null)
                        {
                            yield return resourceClump;
                        }
                    }
                }
            }

        }
        ///The functions below are taking directly from Tractor Mod by PathosChild
        /// https://github.com/Pathoschild/StardewMods/tree/develop/TractorMod

        /// <summary>Get the resource clump which covers a given tile, if any.</summary>
        /// <param name="location">The location to check.</param>
        /// <param name="tile">The tile to check.</param>
        protected ResourceClump GetResourceClumpCoveringTile(GameLocation location, Vector2 tile)
        {
            Rectangle tileArea = this.GetAbsoluteTileArea(tile);
            foreach (ResourceClump clump in this.GetResourceClumps(location))
            {
                if (clump.getBoundingBox().Intersects(tileArea))
                    return clump;
            }

            return null;
        }

        /// <summary>Get a rectangle representing the tile area in absolute pixels from the map origin.</summary>
        /// <param name="tile">The tile position.</param>
        protected Rectangle GetAbsoluteTileArea(Vector2 tile)
        {
            Vector2 pos = tile * Game1.tileSize;
            return new Rectangle((int)pos.X, (int)pos.Y, Game1.tileSize, Game1.tileSize);
        }

    }

}
