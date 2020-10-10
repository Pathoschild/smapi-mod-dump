/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jdusbabek/stardewvalley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewLib
{
    internal class ChestManager
    {
        /*********
        ** Properties
        *********/
        private readonly IMonitor Monitor;
        private ChestDef DefaultChest;
        private Dictionary<int, ChestDef> Chests;


        /*********
        ** Public methods
        *********/
        public ChestManager(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        public void ParseChests(string chestString)
        {
            this.Chests = new Dictionary<int, ChestDef>();

            string[] chestDefinitions = chestString.Split('|');

            foreach (string def in chestDefinitions)
            {
                string[] chestInfo = def.Split(',');
                if (chestInfo.Length == 3)
                {
                    // A Farm chest
                    ChestDef cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), "Farm");
                    this.Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    this.Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}", LogLevel.Trace);
                }
                else if (chestInfo.Length == 4)
                {
                    // A farm house chest
                    ChestDef cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), chestInfo[3]);
                    this.Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    this.Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}", LogLevel.Trace);
                }
            }
        }

        public void ParseChests(string[] chestList)
        {
            this.Chests = new Dictionary<int, ChestDef>();

            foreach (string def in chestList)
            {
                string[] chestInfo = def.Split(',');
                if (chestInfo.Length == 3)
                {
                    // A Farm chest
                    ChestDef cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), "Farm");
                    this.Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    this.Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}", LogLevel.Trace);
                }
                else if (chestInfo.Length == 4)
                {
                    // Another location.
                    ChestDef cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), chestInfo[3]);
                    this.Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    this.Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}", LogLevel.Trace);
                }
            }
        }

        public void ParseChests(List<string> chestList)
        {
            this.Chests = new Dictionary<int, ChestDef>();

            foreach (string def in chestList)
            {
                string[] chestInfo = def.Split(',');
                if (chestInfo.Length == 3)
                {
                    // A Farm chest
                    ChestDef cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), "Farm");
                    this.Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    this.Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}", LogLevel.Trace);
                }
                else if (chestInfo.Length == 4)
                {
                    // Another location.
                    ChestDef cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), chestInfo[3]);
                    this.Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    this.Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}", LogLevel.Trace);
                }
            }
        }

        public void SetDefault(Vector2 v)
        {
            this.DefaultChest = new ChestDef((int)v.X, (int)v.Y);
        }

        public Object GetDefaultChest()
        {
            return this.GetChest(-999999);
        }

        public Object GetChest(int itemId)
        {
            ChestDef def = this.GetChestDef(itemId);
            if (def == null)
                return null;

            GameLocation loc = Game1.getLocationFromName(def.Location);

            if (loc == null)
                return null;

            loc.objects.TryGetValue(def.Tile, out Object chest);
            return chest as Chest;
        }

        public SortedDictionary<int, ChestDef> ParseAllChests()
        {
            SortedDictionary<int, ChestDef> bestGuessChest = new SortedDictionary<int, ChestDef>();

            foreach (GameLocation loc in Game1.locations)
            {
                foreach (KeyValuePair<Vector2, Object> o in loc.Objects.Pairs)
                {
                    if (o.Value is Chest)
                    {
                        Chest c = (Chest)o.Value;
                        Dictionary<int, int> itemCounts = new Dictionary<int, int>();

                        foreach (Item i in ((Chest)o.Value).items)
                        {
                            if (i != null)
                            {
                                if (itemCounts.ContainsKey(i.ParentSheetIndex))
                                {
                                    itemCounts[i.ParentSheetIndex] += i.Stack;
                                }
                                else
                                {
                                    itemCounts.Add(i.ParentSheetIndex, i.Stack);
                                }
                            }
                        }

                        foreach (KeyValuePair<int, int> item in itemCounts)
                        {
                            if (bestGuessChest.ContainsKey(item.Key))
                            {
                                if (bestGuessChest[item.Key].Count < item.Value)
                                {
                                    bestGuessChest[item.Key] = new ChestDef((int)o.Key.X, (int)o.Key.Y, loc.Name, item.Value, c);
                                }
                            }
                            else
                            {
                                bestGuessChest.Add(item.Key, new ChestDef((int)o.Key.X, (int)o.Key.Y, loc.Name, item.Value, c));
                            }
                        }
                    }


                    if (loc.Name.Equals("Farm"))
                    {
                        foreach (Building bgl in Game1.getFarm().buildings)
                        {
                            if (bgl.indoors.Value?.Objects != null && bgl.indoors.Value.Objects.Any())
                            {
                                foreach (KeyValuePair<Vector2, Object> o2 in bgl.indoors.Value.objects.Pairs)
                                {
                                    if (o2.Value is Chest)
                                    {
                                        Chest c = (Chest)o2.Value;
                                        Dictionary<int, int> itemCounts = new Dictionary<int, int>();

                                        foreach (Item i in ((Chest)o2.Value).items)
                                        {
                                            if (i != null)
                                            {
                                                if (itemCounts.ContainsKey(i.ParentSheetIndex))
                                                {
                                                    itemCounts[i.ParentSheetIndex] += i.Stack;
                                                }
                                                else
                                                {
                                                    itemCounts.Add(i.ParentSheetIndex, i.Stack);
                                                }
                                            }
                                        }

                                        foreach (KeyValuePair<int, int> item2 in itemCounts)
                                        {
                                            if (bestGuessChest.ContainsKey(item2.Key))
                                            {
                                                if (bestGuessChest[item2.Key].Count < item2.Value)
                                                {
                                                    bestGuessChest[item2.Key] = new ChestDef((int)o2.Key.X, (int)o2.Key.Y, bgl.indoors.Value.Name, item2.Value, c);
                                                }
                                            }
                                            else
                                            {
                                                bestGuessChest.Add(item2.Key, new ChestDef((int)o2.Key.X, (int)o2.Key.Y, bgl.indoors.Value.Name, item2.Value, c));
                                            }
                                        }
                                    }
                                }
                            }

                        }

                    }
                }
            }
            return bestGuessChest;
        }


        /*********
        ** Private methods
        *********/
        private ChestDef GetChestDef(int itemId)
        {
            this.Chests.TryGetValue(itemId, out ChestDef def);
            return def ?? this.DefaultChest;
        }
    }
}
