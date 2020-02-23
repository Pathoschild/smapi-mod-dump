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

// TODO: Look at stardewvalley bash script, figure out command arguments and
// env variables for mono, see if can launch from debugger.
// TODO: Add Summaries and param desc to all functions


namespace MultitoolMod.Framework
{
    public class Multitool : Tool
    {
        public Axe axe;
        public Pickaxe pickaxe;
        public MeleeWeapon scythe;
        public WateringCan wateringCan;
        public Hoe hoe;
        public IDictionary<string, Tool> attachedTools;
        public MultitoolMod mod;

        public Multitool(MultitoolMod m)
        {
            this.mod = m;
            this.attachedTools = new Dictionary<string, Tool>();
            this.axe = new Axe();
            this.pickaxe = new Pickaxe();
            this.scythe = new MeleeWeapon(47);
            this.scythe = (MeleeWeapon)this.scythe.getOne();
            this.scythe.Category = -99;
            this.wateringCan = new WateringCan();
            this.hoe = new Hoe();
            attachedTools["axe"] = this.axe;
            attachedTools["pickaxe"] = this.pickaxe;
            attachedTools["melee"] = this.scythe;
            attachedTools["wateringcan"] = this.wateringCan;
            attachedTools["hoe"] = this.hoe;
        }

        public override Item getOne()
        {
            return new Multitool(null);
        }
        protected override string loadDisplayName()
        {
            return Game1.content.LoadString("A tool for all trades");
        }
        protected override string loadDescription()
        {
            return Game1.content.LoadString("A tool for all trades");
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
                                dirt.plant(Game1.player.CurrentItem.parentSheetIndex.Get(), xtile, ytile, Game1.player, false, Game1.currentLocation);
                                Game1.player.consumeObject(Game1.player.CurrentItem.parentSheetIndex.Get(), 1);
                            }
                            else if (Game1.player.CurrentItem.Category == StardewValley.Object.fertilizerCategory)
                            {
                                dirt.plant(Game1.player.CurrentItem.parentSheetIndex.Get(), xtile, ytile, Game1.player, true, Game1.currentLocation);
                                Game1.player.consumeObject(Game1.player.CurrentItem.parentSheetIndex.Get(), 1);
                            }
                        }
                        else
                        {
                            location.checkAction(new Location(xtile, ytile), Game1.viewport, Game1.player);
                        }
                    }
                    catch (System.Collections.Generic.KeyNotFoundException)
                    {
                        Game1.addHUDMessage(new HUDMessage($"{Game1.player.CurrentItem} {Game1.player.CurrentItem.parentSheetIndex.Get()} 201"));
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
            if (toolName == "melee")
            {
                //TODO this doesn't land at the right location
                //this.scythe.DoDamage(Game1.currentLocation, x, y, 0, power, who);
                return;
            } 
            else
            {
                tool.DoFunction(location, x, y, power, who);
                return;
            }
        }

        public void Refresh_Tools()
        {
            foreach (Item item in Game1.player.Items)
            {
                if (item is Tool)
                {
                    if (item is Pickaxe p)
                    {
                        this.pickaxe.upgradeLevel.Set(p.upgradeLevel.Get());
                    }
                    else if (item is Axe a)
                    {
                        this.axe.upgradeLevel.Set(a.upgradeLevel.Get());
                    }
                    else if (item is WateringCan w)
                    {
                        this.wateringCan.upgradeLevel.Set(w.upgradeLevel.Get());
                        this.wateringCan.waterCanMax = w.waterCanMax;
                        this.wateringCan.WaterLeft=w.WaterLeft;

                    }
                    else if (item is Hoe h)
                    {
                        this.hoe.upgradeLevel.Set(h.upgradeLevel.Get());
                    }
                    else if (((Tool)item).Name == "Scythe")
                    {
                        this.scythe = (MeleeWeapon)item;
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
                switch (clump.parentSheetIndex)
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
                    properties["string_useTool"] = (System.Object)"melee";
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
                    properties = Get_HoeDirtProperties(pot.hoeDirt, properties);
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
                        properties["string_useTool"] = (System.Object)"melee";
                        properties["bool_hasGrass"] = (System.Object)true;
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
                if (dirt.crop.dead.Get())
                {
                    properties["bool_hasDeadCrop"] = (System.Object)true;
                    properties["bool_hasLiveLCrop"] = (System.Object)false;
                    properties["string_useTool"] = (System.Object)"melee";
                }
                else
                {
                    properties["bool_hasLiveLCrop"] = (System.Object)true;
                    int harvestablePhase = dirt.crop.phaseDays.Count - 1;
                    bool canHarvestNow = dirt.readyForHarvest();
                    properties["bool_fullyGrownCrop"] = (System.Object)canHarvestNow;
                    if (canHarvestNow)
                    {
                        if (dirt.crop.harvestMethod.Value == Crop.sickleHarvest)
                        {
                            properties["string_useTool"] = (System.Object)"melee";
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
                if (clump.getBoundingBox(clump.tile.Value).Intersects(tileArea))
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
