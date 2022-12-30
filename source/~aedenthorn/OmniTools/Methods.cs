/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace OmniTools
{
    public partial class ModEntry
    {
        public static string GetToolSound(Tool t)
        {
            if (t is MeleeWeapon)
            {
                if ((t as MeleeWeapon).type.Value == 3)
                    return "swordswipe";
                if ((t as MeleeWeapon).type.Value == 2)
                    return "clubswipe";
                if ((t as MeleeWeapon).type.Value == 1)
                    return "daggerswipe";
            }
            return toolSoundList[toolList.IndexOf(t.GetType())];
        }

        public static Tool SwitchTool(Tool currentTool, Type type, List<ToolInfo> tools = null)
        {
            int index = type is null ? toolList.IndexOf(typeof(MeleeWeapon)) : toolList.IndexOf(type);
            if (index < 0)
                return null;
            if (!currentTool.modData.TryGetValue(toolsKey, out var toolsString))
                return null;
            if (tools is null)
                tools = JsonConvert.DeserializeObject<List<ToolInfo>>(toolsString);
            for (int i = tools.Count - 1; i >= 0; i--)
            {
                if (tools[i].description.index == index)
                {
                    Tool t = currentTool;
                    Tool newTool = GetToolFromInfo(tools[i]);
                    if (newTool is null)
                    {
                        SMonitor.Log($"Invalid tool {tools[i].displayName}, removing", StardewModdingAPI.LogLevel.Warn);
                        tools.RemoveAt(i);
                        if (tools.Count > 0)
                        {
                            currentTool.modData[toolsKey] = JsonConvert.SerializeObject(tools);
                        }
                        else
                        {
                            currentTool.modData.Remove(toolsKey);
                        }
                        return null;
                    }
                    if ((type == typeof(MeleeWeapon) && (newTool as MeleeWeapon).isScythe(newTool.ParentSheetIndex)) || (type is null && !(newTool as MeleeWeapon).isScythe(newTool.ParentSheetIndex)))
                        continue;
                    tools.RemoveAt(i);
                    tools.Add(new ToolInfo(t));
                    var outTools = new List<ToolInfo>();
                    for (int j = 0; j < tools.Count; j++)
                    {
                        var idx = (j + i) % tools.Count;
                        outTools.Add(tools[idx]);
                    }
                    newTool.modData[toolsKey] = JsonConvert.SerializeObject(outTools);
                    return newTool;
                }
            }
            return null;
        }

        public static Tool SmartSwitch(Tool currentTool, GameLocation currentLocation, Vector2 tile, List<ToolInfo> tools = null)
        {
            if (Config.SwitchForMonsters && currentTool.getLastFarmerToUse() is not null)
            {
                var f = currentTool.getLastFarmerToUse();
                foreach (var c in currentLocation.characters)
                {
                    foreach(var t in GetToolsFromTool(currentTool))
                    {
                        if(t is MeleeWeapon && !(t as MeleeWeapon).isScythe(t.ParentSheetIndex))
                        {
                            Vector2 tileLocation = Vector2.Zero;
                            Vector2 tileLocation2 = Vector2.Zero;
                            var aoe = (t as MeleeWeapon).getAreaOfEffect((int)tile.X * 64, (int)tile.Y * 64, f.FacingDirection, ref tileLocation, ref tileLocation2, f.GetBoundingBox(), f.FarmerSprite.currentAnimationIndex);
                            if (c is Monster && c.GetBoundingBox().Intersects(aoe))
                            {
                                Tool tool = SwitchTool(currentTool, typeof(MeleeWeapon), tools);
                                if (tool != null)
                                    return tool;
                            }
                        }
                    }
                }
            }
            if (Config.SwitchForAnimals)
            {
                FarmAnimal[] animals = new FarmAnimal[0];
                if (currentLocation is Farm)
                {
                    animals = (currentLocation as Farm).animals.Values.ToArray();
                }
                else if (currentLocation is AnimalHouse)
                {
                    animals = (currentLocation as AnimalHouse).animals.Values.ToArray();
                }
                foreach (var c in animals)
                {
                    Rectangle r = new Rectangle((int)tile.X * 64 - 32, (int)tile.Y * 64 - 32, 64, 64);
                    if (c.GetHarvestBoundingBox().Intersects(r))
                    {
                        Tool tool = SwitchForAnimal(currentTool, c, tools);
                        if (tool is not null)
                            return tool;
                    }
                }
            }
            if (Config.SwitchForObjects && currentLocation.objects.TryGetValue(tile, out Object obj))
            {
                Tool tool = SwitchForObject(currentTool, obj, tools);
                if (tool is not null)
                    return tool;
            }
            if (currentLocation.terrainFeatures.TryGetValue(tile, out TerrainFeature tf))
            {
                Tool tool = SwitchForTerrainFeature(currentTool, tf, tools);
                if (tool is not null)
                    return tool;
                if (currentLocation is Farm && Config.SwitchForWatering && tf is HoeDirt && (tf as HoeDirt).state.Value == 0)
                {
                    tool = SwitchTool(currentTool, typeof(WateringCan), tools);
                    if (tool != null)
                        return tool;
                }

            }
            if (Config.SwitchForResourceClumps)
            {
                Rectangle tileRect = new Rectangle((int)tile.X * 64, (int)tile.Y * 64, 64, 64);

                foreach (ResourceClump clump in currentLocation.resourceClumps)
                {
                    var bb = clump.getBoundingBox(clump.tile.Value);
                    if (bb.Intersects(tileRect))
                    {
                        Tool tool = SwitchForClump(currentTool, clump, tools);
                        if (tool is not null)
                            return tool;
                    }
                }
            }
            if (Config.SwitchForPan && currentTool.getLastFarmerToUse() is not null)
            {
                Rectangle orePanRect = new Rectangle(currentLocation.orePanPoint.X * 64 - 64, currentLocation.orePanPoint.Y * 64 - 64, 256, 256);
                if (orePanRect.Contains((int)tile.X * 64, (int)tile.Y * 64) && Utility.distance((float)currentTool.getLastFarmerToUse().getStandingX(), (float)orePanRect.Center.X, (float)currentTool.getLastFarmerToUse().getStandingY(), (float)orePanRect.Center.Y) <= 192f) 
                { 
                    Tool tool = SwitchTool(currentTool, typeof(Pan), tools); 
                    if (tool != null) 
                        return tool; 
                }

            }
            if (Config.SwitchForWateringCan && currentLocation.CanRefillWateringCanOnTile((int)tile.X, (int)tile.Y)) 
            { 
                Tool tool = SwitchTool(currentTool, typeof(WateringCan), tools); 
                if (tool != null) return tool; 
            }

            if (Config.SwitchForFishing && currentLocation.waterTiles is not null)
            {
                try
                {
                    if(currentLocation.waterTiles[(int)tile.X, (int)tile.Y])
                    {
                        Tool tool = SwitchTool(currentTool, typeof(FishingRod), tools);
                        if (tool != null) return tool;
                    }
                }
                catch { }
            }
            
            if (Config.SwitchForTilling && currentLocation.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") != null && !currentLocation.isTileOccupied(tile, "", false) && currentLocation.isTilePassable(new Location((int)tile.X, (int)tile.Y), Game1.viewport))
            { 
                Tool tool = SwitchTool(currentTool, typeof(Hoe), tools); 
                if (tool != null) return tool; 
            }

            return null;
        }

        public static Tool SwitchForAnimal(Tool currentTool, FarmAnimal c, List<ToolInfo> tools = null)
        {
            //SMonitor.Log($"harvesting {c.Name}; age {c.age.Value}/{c.ageWhenMature.Value}; produce {c.currentProduce.Value} ({c.defaultProduceIndex.Value}); tool {Game1.player.CurrentTool?.Name} ({c.toolUsedForHarvest.Value} - {toolsString})");
            if (c.toolUsedForHarvest.Value.Equals("Shears")) 
            { 
                Tool tool = SwitchTool(currentTool, typeof(Shears), tools); 
                if (tool != null) 
                    return tool; 
            }
            else if (c.toolUsedForHarvest.Value.Equals("Milk Pail")) 
            { 
                Tool tool = SwitchTool(currentTool, typeof(MilkPail), tools); 
                if (tool != null) 
                    return tool; 
            }
            
            return null;
            //if (c.currentProduce.Value > 0 && c.age.Value >= (int)c.ageWhenMature.Value)
            //{

            //}
        }

        public static Tool SwitchForClump(Tool currentTool, ResourceClump clump, List<ToolInfo> tools = null)
        {
            if ((clump.parentSheetIndex.Value == 600 || clump.parentSheetIndex.Value == 602)) 
            { 
                Tool tool = SwitchTool(currentTool, typeof(Axe), tools); 
                if (tool != null) 
                    return tool; 
            }
            else if (new int[] { 622, 672, 752, 754, 756, 758 }.Contains(clump.parentSheetIndex.Value)) 
            { 
                Tool tool = SwitchTool(currentTool, typeof(Pickaxe), tools); 
                if (tool != null) 
                    return tool; 
            }
            return null;
        }

        public static Tool SwitchForObject(Tool currentTool, Object obj, List<ToolInfo> tools = null)
        {
            if (obj.Name.Equals("Stone"))
            {
                Tool tool = SwitchTool(currentTool, typeof(Pickaxe), tools); 
                if (tool != null) 
                    return tool;
            }
            else if (obj.Name.Contains("Twig"))
            {
                Tool tool = SwitchTool(currentTool, typeof(Axe), tools); 
                if (tool != null) 
                    return tool;
            }
            else if (obj.ParentSheetIndex == 590)
            {
                Tool tool = SwitchTool(currentTool, typeof(Hoe), tools); 
                if (tool != null) 
                    return tool;
            }
            else if (obj.Name.Contains("Weeds"))
            {
                Tool tool = SwitchTool(currentTool, null, tools);
                if (tool != null)
                    return tool;
            }
            return null;
        }

        public static Tool SwitchForTerrainFeature(Tool currentTool, TerrainFeature tf, List<ToolInfo> tools = null)
        {
            if (Config.SwitchForTrees && tf is Tree) 
            { 
                if((tf as Tree).growthStage.Value >= 3)
                {
                    Tool tool = SwitchTool(currentTool, typeof(Axe), tools);
                    if (tool != null)
                        return tool;

                }
                else if((tf as Tree).growthStage.Value >= 1)
                {
                    Tool tool = SwitchTool(currentTool, null, tools);
                    if (tool != null)
                        return tool;

                }
                else
                {
                    Tool tool = SwitchTool(currentTool, typeof(Axe), tools);
                    if (tool != null)
                        return tool;
                    tool = SwitchTool(currentTool, typeof(Pickaxe), tools);
                    if (tool != null)
                        return tool;
                    tool = SwitchTool(currentTool, typeof(Hoe), tools);
                    if (tool != null)
                        return tool;
                }
            }
            else if (Config.SwitchForGrass && tf is Grass) 
            { 
                Tool tool = SwitchTool(currentTool, null, tools); 
                if (tool != null) 
                    return tool; 
            }
            else if (Config.SwitchForCrops && tf is HoeDirt && (tf as HoeDirt).crop?.harvestMethod.Value == 1 && (tf as HoeDirt).crop.currentPhase.Value >= (tf as HoeDirt).crop.phaseDays.Count - 1 && (!(tf as HoeDirt).crop.fullyGrown.Value || (tf as HoeDirt).crop.dayOfCurrentPhase.Value <= 0)) 
            { 
                Tool tool = SwitchTool(currentTool, null, tools); 
                if (tool != null) 
                    return tool; 
            }
            return null;
        }

        public static Tool CycleTool(Tool currentTool, string toolsString)
        {
            List<ToolInfo> tools = JsonConvert.DeserializeObject<List<ToolInfo>>(toolsString);
            Tool t = GetToolFromInfo(tools[0]);
            if (t is null)
            {
                SMonitor.Log($"Invalid tool {tools[0].displayName}, removing", StardewModdingAPI.LogLevel.Warn);
                tools.RemoveAt(0);
                if (tools.Count > 0)
                {
                    currentTool.modData[toolsKey] = JsonConvert.SerializeObject(tools);
                }
                else
                {
                    currentTool.modData.Remove(toolsKey);
                }
                return currentTool;
            }

            tools.Add(new ToolInfo(currentTool));
            t.modData[toolsKey] = JsonConvert.SerializeObject(tools.Skip(1));
            Game1.playSound(GetToolSound(t));
            return t;
        }

        public static Tool RemoveTool(Tool currentTool, string toolsString)
        {
            List<ToolInfo> tools = JsonConvert.DeserializeObject<List<ToolInfo>>(toolsString);
            Tool t = GetToolFromInfo(tools[0]);
            if (t is null)
            {
                SMonitor.Log($"Invalid tool {tools[0].displayName}, removing", StardewModdingAPI.LogLevel.Warn);
                tools.RemoveAt(0);
                if (tools.Count > 0)
                {
                    currentTool.modData[toolsKey] = JsonConvert.SerializeObject(tools);
                }
                else
                {
                    currentTool.modData.Remove(toolsKey);
                }
                return currentTool;
            }
            if (tools.Count > 1)
            {
                t.modData[toolsKey] = JsonConvert.SerializeObject(tools.Skip(1));
            }
            else
            {
                t.modData.Remove(toolsKey);
            }
            Game1.playSound(GetToolSound(t));
            currentTool.modData.Remove(toolsKey);
            if (!Game1.player.addItemToInventoryBool(currentTool))
            {
                Game1.createItemDebris(currentTool, Game1.player.getStandingPosition(), Game1.player.FacingDirection, null, -1);
            }
            return t;
        }

        public static Tool GetToolFromInfo(ToolInfo toolInfo)
        {
            Tool t = GetToolFromDescription(toolInfo.description.index, toolInfo.description.upgradeLevel);
            foreach (var s in toolInfo.enchantments)
            {
                try
                {
                    var type = typeof(Game1).Assembly.GetType(s);
                    AccessTools.Method(t.enchantments.GetType(), "Add").Invoke(t.enchantments, new object[] { Activator.CreateInstance(type) });
                }
                catch { }
            }
            foreach (var oi in toolInfo.attachments)
            {
                try
                {
                    t.attachments.Add(oi is not null ? new Object(oi.parentSheetIndex, oi.stack, false, -1, oi.quality) : null);
                }
                catch(Exception ex) 
                {
                    SMonitor.Log(ex.ToString());
                }
            }
            if(t is WateringCan && toolInfo.vars.Count > 0)
                (t as WateringCan).WaterLeft = (int)(long)toolInfo.vars[0];
            return t;
        }

        public static ToolDescription? GetDescriptionFromTool(Tool t)
        {
            if (!toolList.Contains(t.GetType()))
                return null;
            if (t is MeleeWeapon)
            {
                return new ToolDescription((byte)toolList.IndexOf(typeof(MeleeWeapon)), t.ParentSheetIndex == -1 ? (byte)(t as MeleeWeapon).InitialParentTileIndex : (byte)t.ParentSheetIndex);
            }
            if (t is Slingshot)
            {
                return new ToolDescription((byte)toolList.IndexOf(typeof(Slingshot)), (byte)(t as Slingshot).InitialParentTileIndex);
            }
            return new ToolDescription((byte)toolList.IndexOf(t.GetType()), (byte)t.UpgradeLevel);
        }

        public static Tool GetToolFromDescription(int index, byte upgradeLevel)
        {
            Tool t = null;
            switch (index)
            {
                case 0:
                    t = new Axe();
                    break;
                case 1:
                    t = new Hoe();
                    break;
                case 2:
                    t = new FishingRod();
                    break;
                case 3:
                    t = new Pickaxe();
                    break;
                case 4:
                    t = new WateringCan();
                    break;
                case 5:
                    return new MeleeWeapon(upgradeLevel);
                case 6:
                    t = new Slingshot(upgradeLevel);
                    break;
                case 7:
                    t = new MilkPail();
                    break;
                case 8:
                    t = new Pan();
                    break;
                case 9:
                    t = new Shears();
                    break;
                case 10:
                    t = new Wand();
                    break;
            }
            if (t is not null)
                t.UpgradeLevel = upgradeLevel;
            return t;
        }
        public static Tool[] GetToolsFromTool(Tool tool)
        {
            if (!tool.modData.TryGetValue(toolsKey, out var toolsString))
                return null; 
            var infos = JsonConvert.DeserializeObject<List<ToolInfo>>(toolsString);
            var list = new List<Tool>();
            foreach (var i in infos)
            {
                Tool t = ModEntry.GetToolFromInfo(i);
                if (t is not null)
                    list.Add(t);
            }
            return list.ToArray();
        }
        public static List<ToolInfo> GetToolInfosFromTool(Tool tool)
        {
            if (!tool.modData.TryGetValue(toolsKey, out var toolsString))
                return null; 
            return JsonConvert.DeserializeObject<List<ToolInfo>>(toolsString);
        }
        public static List<ToolInfo> GetToolInfosFromString(string toolsString)
        {
            if (toolsString is null)
                return null; 
            return JsonConvert.DeserializeObject<List<ToolInfo>>(toolsString);
        }
    }
}