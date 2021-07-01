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
            Monitor = monitor;
        }

        public void ParseChests(string chestString)
        {
            Chests = new Dictionary<int, ChestDef>();

            var chestDefinitions = chestString.Split('|');

            foreach (var def in chestDefinitions)
            {
                var chestInfo = def.Split(',');
                if (chestInfo.Length == 3)
                {
                    // A Farm chest
                    var cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), "Farm");
                    Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}");
                }
                else if (chestInfo.Length == 4)
                {
                    // A farm house chest
                    var cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), chestInfo[3]);
                    Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}");
                }
            }
        }

        public void ParseChests(string[] chestList)
        {
            Chests = new Dictionary<int, ChestDef>();

            foreach (var def in chestList)
            {
                var chestInfo = def.Split(',');
                if (chestInfo.Length == 3)
                {
                    // A Farm chest
                    var cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), "Farm");
                    Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}");
                }
                else if (chestInfo.Length == 4)
                {
                    // Another location.
                    var cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), chestInfo[3]);
                    Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}");
                }
            }
        }

        public void ParseChests(List<string> chestList)
        {
            Chests = new Dictionary<int, ChestDef>();

            foreach (var def in chestList)
            {
                var chestInfo = def.Split(',');
                if (chestInfo.Length == 3)
                {
                    // A Farm chest
                    var cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), "Farm");
                    Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}");
                }
                else if (chestInfo.Length == 4)
                {
                    // Another location.
                    var cd = new ChestDef(Convert.ToInt32(chestInfo[1]), Convert.ToInt32(chestInfo[2]), chestInfo[3]);
                    Chests.Add(Convert.ToInt32(chestInfo[0]), cd);
                    Monitor.Log($"Parsing chest: {cd.X},{cd.Y} for item: {chestInfo[0]}, location: {cd.Location}");
                }
            }
        }

        public void SetDefault(Vector2 v)
        {
            DefaultChest = new ChestDef((int) v.X, (int) v.Y);
        }

        public Object GetDefaultChest()
        {
            return GetChest(-999999);
        }

        public Object GetChest(int itemId)
        {
            var def = GetChestDef(itemId);
            if (def == null)
                return null;

            var loc = Game1.getLocationFromName(def.Location);

            if (loc == null)
                return null;

            loc.objects.TryGetValue(def.Tile, out var chest);
            return chest as Chest;
        }

        public SortedDictionary<int, ChestDef> ParseAllChests()
        {
            var bestGuessChest = new SortedDictionary<int, ChestDef>();

            foreach (var loc in Game1.locations)
            foreach (var o in loc.Objects.Pairs)
            {
                if (o.Value is Chest)
                {
                    var c = (Chest) o.Value;
                    var itemCounts = new Dictionary<int, int>();

                    foreach (var i in ((Chest) o.Value).items)
                        if (i != null)
                        {
                            if (itemCounts.ContainsKey(i.ParentSheetIndex))
                                itemCounts[i.ParentSheetIndex] += i.Stack;
                            else
                                itemCounts.Add(i.ParentSheetIndex, i.Stack);
                        }

                    foreach (var item in itemCounts)
                        if (bestGuessChest.ContainsKey(item.Key))
                        {
                            if (bestGuessChest[item.Key].Count < item.Value)
                                bestGuessChest[item.Key] =
                                    new ChestDef((int) o.Key.X, (int) o.Key.Y, loc.Name, item.Value, c);
                        }
                        else
                        {
                            bestGuessChest.Add(item.Key,
                                new ChestDef((int) o.Key.X, (int) o.Key.Y, loc.Name, item.Value, c));
                        }
                }


                if (loc.Name.Equals("Farm"))
                    foreach (var bgl in Game1.getFarm().buildings)
                        if (bgl.indoors.Value?.Objects != null && bgl.indoors.Value.Objects.Any())
                            foreach (var o2 in bgl.indoors.Value.objects.Pairs)
                                if (o2.Value is Chest)
                                {
                                    var c = (Chest) o2.Value;
                                    var itemCounts = new Dictionary<int, int>();

                                    foreach (var i in ((Chest) o2.Value).items)
                                        if (i != null)
                                        {
                                            if (itemCounts.ContainsKey(i.ParentSheetIndex))
                                                itemCounts[i.ParentSheetIndex] += i.Stack;
                                            else
                                                itemCounts.Add(i.ParentSheetIndex, i.Stack);
                                        }

                                    foreach (var item2 in itemCounts)
                                        if (bestGuessChest.ContainsKey(item2.Key))
                                        {
                                            if (bestGuessChest[item2.Key].Count < item2.Value)
                                                bestGuessChest[item2.Key] = new ChestDef((int) o2.Key.X, (int) o2.Key.Y,
                                                    bgl.indoors.Value.Name, item2.Value, c);
                                        }
                                        else
                                        {
                                            bestGuessChest.Add(item2.Key,
                                                new ChestDef((int) o2.Key.X, (int) o2.Key.Y, bgl.indoors.Value.Name,
                                                    item2.Value, c));
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
            Chests.TryGetValue(itemId, out var def);
            return def ?? DefaultChest;
        }
    }
}