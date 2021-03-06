/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using FarmCleaner.Framework.Configs;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;
using STree = StardewValley.TerrainFeatures.Tree;

namespace FarmCleaner
{
    public class FarmCleaner : Mod
    {
        private FcConfig _config;

        //Dictionaries so we can revert back if need be. Will be added later
        private readonly Dictionary<Vector2, TerrainFeature> _grass = new Dictionary<Vector2, TerrainFeature>();
        private readonly Dictionary<Vector2, TerrainFeature> _saplings = new Dictionary<Vector2, TerrainFeature>();
        private readonly Dictionary<Vector2, SObject> _weeds = new Dictionary<Vector2, SObject>();
        private readonly Dictionary<Vector2, SObject> _stone = new Dictionary<Vector2, SObject>();
        private readonly Dictionary<Vector2, SObject> _twig = new Dictionary<Vector2, SObject>();
        private readonly Dictionary<Vector2, ResourceClump> _stump = new Dictionary<Vector2, ResourceClump>();
        private readonly Dictionary<Vector2, ResourceClump> _largelog = new Dictionary<Vector2, ResourceClump>();
        private readonly Dictionary<Vector2, ResourceClump> _largestone = new Dictionary<Vector2, ResourceClump>();
        private readonly Dictionary<Vector2, SObject> _ores = new Dictionary<Vector2,SObject>();
        public int[] NonNormalStones = {75, 290, 751, 765, 764};

        //Variables to control Amounts of items added
        private int _mixedSeedsCount, _coalCount, _geodeCount, _hayCount, _woodCount, _fiberCount, _stoneCount, _hardWoodCount, _missedWeeds, _acornCount, _mapleCount, _pineConeCount, _sapCount, _oreCount;
        

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<FcConfig>();

            
            //Events
            //helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            //Add the commands we will need. 
            //helper.ConsoleCommands.Add("fc_restore", "Restores your farm back to before the clean was ran.", Restore);

        }

        private void Restore(string command, string[] args)
        {
            Farm farm = Game1.getFarm();

            //Find chest on the map.
            farm.objects.TryGetValue(_config.ChestLocation, out SObject obj);
            Chest myChest;

            if (obj != null && obj is Chest c)
                myChest = c;
            else
            {
                Monitor.Log($"Couldn't find the chest with the items in it at: {_config.ChestLocation.ToString()}");
                return;
            }

            foreach (var o in myChest.items.ToList())
            {
               // Make sure the items are there.
            }
            foreach (var g in _grass.ToList())
            {
                farm.terrainFeatures.Add(g.Key, g.Value);
                _grass.Remove(g.Key);
            }
            foreach (var s in _saplings.ToList())
            {
                farm.terrainFeatures.Add(s.Key, s.Value);
                _saplings.Remove(s.Key);
            }

            foreach (var w in _weeds.ToList())
            {
                farm.objects.Add(w.Key, w.Value);
                _weeds.Remove(w.Key);
            }
            foreach (var st in _stone.ToList())
            {
                farm.objects.Add(st.Key, st.Value);
                _stone.Remove(st.Key);
            }
            foreach (var tw in _twig.ToList())
            {
                farm.objects.Add(tw.Key, tw.Value);
                _twig.Remove(tw.Key);
            }

            foreach (var stu in _stump.ToList())
            {
                farm.resourceClumps.Add(stu.Value);
                _stump.Remove(stu.Key);
            }
            foreach (var ll in _largelog.ToList())
            {
                farm.resourceClumps.Add(ll.Value);
                _largelog.Remove(ll.Key);
            }
            foreach (var ls in _largestone.ToList())
            {
                farm.resourceClumps.Add(ls.Value);
                _largestone.Remove(ls.Key);
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Game1.getFarm().IsOutdoors)
                return;

            if (e.IsDown(SButton.NumPad9))
            {
                DoClean();
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            DoClean();
        }
        /*
        TerrainFeatures
        grass = new Dictionary<Vector2, TerrainFeature>(); => Done
        saplings = new Dictionary<Vector2, TerrainFeature>(); => Done

        SObjects
        weeds = new Dictionary<Vector2, SObject>(); => Done
        stone = new Dictionary<Vector2, SObject>(); => Part Done => Need to add in Geodes, and Ores
        twig = new Dictionary<Vector2, SObject>(); => Done

        ResourceClumps
        stump = new Dictionary<Vector2, ResourceClump>(); => Done
        largelog = new Dictionary<Vector2, ResourceClump>(); => Done
        largestone = new Dictionary<Vector2, ResourceClump>(); => Done
        */
        
        //Method to gather Objects and TerrainFeatures
        private void FillDictionary()
        {
            Farm farm = Game1.getFarm();

            _weeds.Clear();
            _twig.Clear();
            _stone.Clear();
            _grass.Clear();
            _saplings.Clear();
            _stump.Clear();
            _largelog.Clear();
            _largestone.Clear();

            //Objects.
            foreach (var obj in farm.objects.Pairs.ToList())
            {
                if (obj.Value.Name.ToLower().Contains("weed"))
                    _weeds.Add(obj.Key, obj.Value);
                if (obj.Value.Name.ToLower().Contains("twig"))
                    _twig.Add(obj.Key, obj.Value);
                //Filter out Geodes, and Ore stones
                if (obj.Value.Name.ToLower().Contains("stone") && !NonNormalStones.Contains(obj.Value.ParentSheetIndex))
                    _stone.Add(obj.Key, obj.Value);
            }

            
            //TerrainFeatures
            foreach (var ter in farm.terrainFeatures.Pairs.ToList())
            {
                //STree t = (STree) ter.Value;
                if (ter.Value is Grass)
                    _grass.Add(ter.Key, ter.Value);
                if (ter.Value is Tree t)
                    _saplings.Add(ter.Key, ter.Value);
            }
            
            //Resource Clumps
            foreach (var clumps in farm.resourceClumps.ToList())
            {
                if (clumps.parentSheetIndex.Value == 600)
                    _stump.Add(clumps.tile.Value, clumps);
                if (clumps.parentSheetIndex.Value == 602)
                    _largelog.Add(clumps.tile.Value, clumps);
                if (clumps.parentSheetIndex.Value == 672)
                    _largestone.Add(clumps.tile.Value, clumps);
            }

        }


        //CutWeed, will determine what to give fiber or mixed seeds

        private void DoClean()
        {
            Monitor.Log("Running Cleaner");
            FillDictionary();
            //See if this should be ran at all.
            /*
            if (!(_weeds.Any() && _grass.Any() && _saplings.Any() && _stone.Any() && _twig.Any() && _stump.Any() &&
                  _largelog.Any() && _largestone.Any()))
                return;*/
            //Reset the counts
            _mixedSeedsCount =
                _coalCount = _geodeCount = _hayCount = _woodCount = _fiberCount = _stoneCount = _hardWoodCount = _missedWeeds = _acornCount = _mapleCount = _pineConeCount = _sapCount = 0;

            //Lets make sure the chest exists.

            Farm farm = Game1.getFarm();
            Random rnd;
            farm.objects.TryGetValue(_config.ChestLocation, out SObject obj);
            Chest myChest;

            if (obj != null && obj is Chest c)
                myChest = c;
            else
            {
                myChest = new Chest(true);
                farm.objects.Add(_config.ChestLocation, myChest);
            }

            Monitor.Log("Passed the Chest Check");
            //Remove grass and add hay.
            if (_config.GrassRemoval && _grass.Any())
            {
                rnd = new Random();
                foreach (var gr in _grass.ToList())
                {
                    int amt = 1 * rnd.Next(1, 2);

                    farm.terrainFeatures.Remove(gr.Key);
                    Item i = new SObject(178, amt);
                    _hayCount += amt;
                    myChest.addItem(i);
                    Game1.player.gainExperience(2, 12);
                }
            }
            //Remove trees and add wood
            if (_config.TreeConfigs.ClearTrees && _saplings.Any())
            {
                foreach (var trees in _saplings.ToList())
                {
                    STree t = (STree)trees.Value;
                    int tType = t.treeType.Value;
                    int tGrowth = t.growthStage.Value;
                    int tSeed = GetSeed(t);
                    rnd = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)trees.Key.X * 7 + (int)trees.Key.Y * 11);


                    //Check tGrowth values
                    if (tGrowth == (int)TreeStage.Seed && _config.TreeConfigs.ClearSeeds)
                        farm.terrainFeatures.Remove(trees.Key);
                    if (tGrowth == (int)TreeStage.Sprout && _config.TreeConfigs.ClearSprout)
                        farm.terrainFeatures.Remove(trees.Key);
                    if (tGrowth == (int)TreeStage.Sapling && _config.TreeConfigs.ClearSapling)
                        farm.terrainFeatures.Remove(trees.Key);
                    if (tGrowth == (int)TreeStage.Bush && _config.TreeConfigs.ClearBush)
                        farm.terrainFeatures.Remove(trees.Key);
                    if (tGrowth == (int)TreeStage.SmallTree && _config.TreeConfigs.ClearSmallTree)
                        farm.terrainFeatures.Remove(trees.Key);
                    if (tGrowth >= (int)TreeStage.Tree && _config.TreeConfigs.ClearFullTree)
                        farm.terrainFeatures.Remove(trees.Key);

                    //Now we check to see if anything else should have dropped

                    //Check to see if Hardwood should drop and how much
                    int hwAmt = 0;
                    while (Game1.player.professions.Contains(14) && rnd.NextDouble() < 0.4)
                        hwAmt++;
                    if (hwAmt > 0)
                    {
                        myChest.addItem(DoItem(709, hwAmt));
                        _hardWoodCount += hwAmt;
                    }

                    int r = rnd.Next(1, 4);
                    Monitor.Log($"{r} => {Math.PI / 2.0}");
                    //Check to see if Sap should have dropped
                    if (r > Math.PI / 2.0)
                    {
                        myChest.addItem(DoItem(92, r));
                        _sapCount += r;
                    }

                    //Check to see if a seed should have dropped
                    if (Game1.player.getEffectiveSkillLevel(2) >= 1 && rnd.NextDouble() < 0.75 || tGrowth == (int)TreeStage.Seed)
                    {
                        switch (tSeed)
                        {
                            case 309:
                                myChest.addItem(DoItem(309));
                                _acornCount++;
                                break;
                            case 310:
                                myChest.addItem(DoItem(310));
                                _mapleCount++;
                                break;
                            case 311:
                                myChest.addItem(DoItem(311));
                                _pineConeCount++;
                                break;
                        }
                    }


                    farm.terrainFeatures.Remove(trees.Key);
                    _woodCount += 12;
                    myChest.addItem(DoItem(388, 12));

                    Game1.player.gainExperience(2, 12);
                }
            }
            //Remove weeds and adds them
            if (_config.WeedRemoval && _weeds.Any())
            {
                foreach (var weed in _weeds.ToList())
                {
                    farm.objects.Remove(weed.Key);
                    int pid = CutWeeds();
                    if (pid == 0)
                    {
                        _missedWeeds++;
                        continue;
                    }

                    Item i = new SObject(pid, 1);
                    if (pid == 770)
                        _mixedSeedsCount++;
                    else
                        _fiberCount++;
                    myChest.addItem(i);
                }
            }
            //Remove stones and adds them
            if (_config.StoneRemoval && _stone.Any())
            {
                rnd = new Random();
                foreach (var st in _stone.ToList())
                {
                    farm.objects.Remove(st.Key);
                    int pid = BreakStone((int)st.Key.X, (int)st.Key.Y);
                    int amt = (pid == 382 || pid == 390) ? rnd.Next(1, 3) : 1;
                    if (pid == 382)
                        _coalCount += amt;
                    if (pid >= 535 && pid < 538)
                        _geodeCount++;
                    if (pid == 390)
                        _stoneCount += amt;

                    Item i = new SObject(pid, amt);
                    myChest.addItem(i);
                    Game1.player.gainExperience(2, 12);
                }
            }
            //Remove twigs and adds wood
            if (_config.TwigRemoval && _twig.Any())
            {
                foreach (var twigs in _twig.ToList())
                {
                    farm.objects.Remove(twigs.Key);
                    Item i = new SObject(388, 1);
                    _woodCount++;
                    myChest.addItem(i);
                    Game1.player.gainExperience(2, 12);
                }
            }
            //Remove stumps and adds HardWood
            if (_config.StumpRemoval && _stump.Any())
            {
                foreach (var stumps in _stump.ToList())
                {
                    farm.resourceClumps.Remove(stumps.Value);
                    Item i = new SObject(709, 2);
                    _hardWoodCount += 2;
                    myChest.addItem(i);
                    Game1.player.gainExperience(2, 12);
                }
            }
            //Remove large logs and adds hardwood
            if (_config.LargeLogRemoval && _largelog.Any())
            {
                foreach (var logs in _largelog.ToList())
                {
                    farm.resourceClumps.Remove(logs.Value);
                    Item i = new SObject(709, 8);
                    _hardWoodCount += 8;
                    myChest.addItem(i);
                    Game1.player.gainExperience(2, 12);
                }
            }
            //Remove large stones and adds stone
            if (_config.LargeStoneRemoval && _largestone.Any())
            {
                foreach (var stones in _largestone.ToList())
                {
                    farm.resourceClumps.Remove(stones.Value);
                    Item i = new SObject(390, 15);
                    _stoneCount += 15;
                    myChest.addItem(i);
                    Game1.player.gainExperience(2, 12);
                }
            }

            //Show the message at the end.

            string addedToChest =
                $"Added to Chest:\n\n Hay: {_hayCount}\n Wood: {_woodCount}\n Fiber: {_fiberCount} \n Stone: {_stoneCount}\n Hardwood: {_hardWoodCount}";
            //Check to see if certain items were added
            addedToChest += _mixedSeedsCount > 0 ? $"\n MixedSeeds: {_mixedSeedsCount}" : "";
            addedToChest += _sapCount > 0 ? $"\n Sap: {_sapCount}" : "";
            addedToChest += _acornCount > 0 ? $"\n Acorns: {_acornCount}" : "";
            addedToChest += _mapleCount > 0 ? $"\n Maple Seeds: {_mapleCount}" : "";
            addedToChest += _pineConeCount > 0 ? $"\n Pine Cones: {_pineConeCount}" : "";
            addedToChest += _coalCount > 0 ? $"\n Coal: {_coalCount}" : "";
            addedToChest += _geodeCount > 0 ? $"\n Geodes: {_geodeCount}" : "";

            //Throw out the final message.
            Monitor.Log(
                $"Removed the Following Items:\n\n Grass: {_grass.Count} \n Trees: {_saplings.Count} \n Weeds: {_weeds.Count} \n Stones: {_stone.Count} \n Twigs: {_twig.Count} \n Stumps: {_stump.Count} \n LargeLogs: {_largelog.Count} \n Large Stone: {_largestone.Count}\n\n {addedToChest}");
        }

        private int CutWeeds()
        {
            if (Game1.random.NextDouble() < 0.5)
                return 771;
            if (Game1.random.NextDouble() < 0.05)
                return 770;

            return 0;
        }

        private int BreakStone(/*int stoneType, */int x, int y)
        {
            Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + x * 2000 + y);
            if (random.NextDouble() < 0.035 && Game1.stats.DaysPlayed > 1U)
                return 535 + (Game1.stats.DaysPlayed <= 60U || random.NextDouble() >= 0.2 ? (Game1.stats.DaysPlayed <= 120U || random.NextDouble() >= 0.2 ? 0 : 2) : 1);
            if (random.NextDouble() < 0.035 * (Game1.player.professions.Contains(21) ? 2.0 : 1.0) &&
                Game1.stats.DaysPlayed > 1U)
                return 382;
            if (random.NextDouble() < 0.01 && Game1.stats.DaysPlayed > 1U)
                return 390;

            return 390;
        }

        private int GetSeed(Tree tree)
        {
            int seedId = 0;
            switch (tree.treeType.Value)
            {
                case 1://Oak
                    seedId = 309;
                    break;
                case 2://Maple
                    seedId = 310;
                    break;
                case 3://Pine
                    seedId = 311;
                    break;
                default:
                    seedId = 309;
                    break;
            }

            return seedId;
        }

        private Item DoItem(int itemId)
        {
            Item i = new SObject(itemId, 1);
            return i;
        }
        private Item DoItem(int itemId, int amount)
        {
            Item i = new SObject(itemId, amount);
            return i;
        }
        internal enum TreeType
        {
            Oak = Tree.bushyTree,
            Maple = Tree.leafyTree,
            Pine = Tree.pineTree,
            Palm = Tree.palmTree,
            BigMushroom = Tree.mushroomTree
        }

        internal enum TreeStage
        {
            Seed = STree.seedStage,
            Sprout = STree.sproutStage,
            Sapling = STree.saplingStage,
            Bush = STree.bushStage,
            SmallTree = STree.treeStage - 1, // an intermediate stage between bush and tree, no constant
            Tree = STree.treeStage
        }
    }
}
