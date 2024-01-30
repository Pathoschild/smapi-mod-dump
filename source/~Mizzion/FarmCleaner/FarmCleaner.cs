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
using System.Globalization;
using System.Linq;
using FarmCleaner.Framework.Configs;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;
using STree = StardewValley.TerrainFeatures.Tree;

namespace FarmCleaner
{
    public class FarmCleaner : Mod
    {
        private FcConfig _config;
        public ITranslationHelper I18N;

        //Dictionaries so we can revert back if need be. Will be added later
        private readonly Dictionary<Vector2, TerrainFeature> _grass = new();
        private readonly Dictionary<Vector2, TerrainFeature> _saplings = new();
        private readonly Dictionary<Vector2, SObject> _weeds = new();
        private readonly Dictionary<Vector2, SObject> _stone = new();
        private readonly Dictionary<Vector2, SObject> _twig = new();
        private readonly Dictionary<Vector2, ResourceClump> _stump = new();
        private readonly Dictionary<Vector2, ResourceClump> _largelog = new();
        private readonly Dictionary<Vector2, ResourceClump> _largestone = new();
        private readonly Dictionary<Vector2, SObject> _forage = new();
        private readonly Dictionary<Vector2, SObject> _ores = new();

        private readonly List<Vector2> _mineLadders = new();
        public int[] NonNormalStones = {75, 290, 751, 765, 764};

        //Variables to control Amounts of items added
        private int _mixedSeedsCount, _coalCount, _geodeCount, _hayCount, _woodCount, _fiberCount, _stoneCount, _hardWoodCount, _missedWeeds, _acornCount, _mapleCount, _pineConeCount, _sapCount, _oreCount;
        

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<FcConfig>();
            I18N = helper.Translation;

            //Events
            //helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            //Add the commands we will need. 
            //helper.ConsoleCommands.Add("fc_restore", "Restores your farm back to before the clean was ran.", Restore);

        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            //Lets set this up
            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this._config = new FcConfig(),
                save: () => this.Helper.WriteConfig(this._config)
            );

            //Removal Settings
            configMenu.AddSectionTitle(ModManifest, I18N.Get("gconfig.itemstoremove").ToString);
            //Grass Removal
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Grass",
                tooltip: () => "To remove grass or not to.",
                getValue: () => this._config.GrassRemoval,
                setValue: value => this._config.GrassRemoval = value
            );
            //Weed Removal
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Weeds",
                tooltip: () => "To remove weeds or not to.",
                getValue: () => this._config.WeedRemoval,
                setValue: value => this._config.WeedRemoval = value
            );
            //Stone Removal
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Stone",
                tooltip: () => "To remove stone or not to.",
                getValue: () => this._config.StoneRemoval,
                setValue: value => this._config.StoneRemoval = value
            );
            //Twig Removal
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Twigs",
                tooltip: () => "To remove twigs or not to.",
                getValue: () => this._config.TwigRemoval,
                setValue: value => this._config.TwigRemoval = value
            );
            //Forage Removal
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Forage",
                tooltip: () => "To remove forage or not to.",
                getValue: () => this._config.ForageRemoval,
                setValue: value => this._config.ForageRemoval = value
            );
            //Stump Removal
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Stumps",
                tooltip: () => "To remove stumps or not to.",
                getValue: () => this._config.StumpRemoval,
                setValue: value => this._config.StumpRemoval = value
            );
            //Large Log Removal
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Large Logs",
                tooltip: () => "To remove large logs or not to.",
                getValue: () => this._config.LargeLogRemoval,
                setValue: value => this._config.LargeLogRemoval = value
            );
            //Large Stone Removal
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Large Stones",
                tooltip: () => "To remove large stones or not to.",
                getValue: () => this._config.LargeStoneRemoval,
                setValue: value => this._config.LargeStoneRemoval = value
            );

            //Tree settings
            configMenu.AddSectionTitle(ModManifest, I18N.Get("gconfig.treeremovesettings").ToString);
            //Clear Trees
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Trees",
                tooltip: () => "To remove trees or not to.",
                getValue: () => this._config.TreeConfigs.ClearTrees,
                setValue: value => this._config.TreeConfigs.ClearTrees = value
            );
            //Leave Random trees
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Leave Random Trees",
                tooltip: () => "To leave random trees or not to.",
                getValue: () => this._config.TreeConfigs.LeaveRandomTrees,
                setValue: value => this._config.TreeConfigs.LeaveRandomTrees = value
            );
            //RandomTreeChance
            /*
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Trees",
                tooltip: () => "To remove trees or not to.",
                getValue: () => this._config.TreeConfigs.,
                setValue: value => this._config.TreeConfigs. = value
            );*/
            //ClearSeeds
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Clear Seeds",
                tooltip: () => "To clear seeds or not to.",
                getValue: () => this._config.TreeConfigs.ClearSeeds,
                setValue: value => this._config.TreeConfigs.ClearSeeds = value
            );
            //ClearSprouts
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Sprouts",
                tooltip: () => "To remove tree sprouts or not to.",
                getValue: () => this._config.TreeConfigs.ClearSprout,
                setValue: value => this._config.TreeConfigs.ClearSprout = value
            );
            //ClearSapling
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Saplings",
                tooltip: () => "To remove tree sapling or not to.",
                getValue: () => this._config.TreeConfigs.ClearSapling,
                setValue: value => this._config.TreeConfigs.ClearSapling = value
            );
            //Shake Bushes
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Shake Bushes",
                tooltip: () => "To shake bushes or not to.",
                getValue: () => this._config.TreeConfigs.ClearBush,
                setValue: value => this._config.TreeConfigs.ClearBush = value
            );
            //ClearSmallTrees
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Small Trees",
                tooltip: () => "To remove small trees or not to.",
                getValue: () => this._config.TreeConfigs.ClearSmallTree,
                setValue: value => this._config.TreeConfigs.ClearSmallTree = value
            );
            //ClearFullTree
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Remove Full Grown Trees",
                tooltip: () => "To remove full grown trees or not to.",
                getValue: () => this._config.TreeConfigs.ClearFullTree,
                setValue: value => this._config.TreeConfigs.ClearFullTree = value
            );
        }
        private void Restore(string command, string[] args)
        {
            var farm = Game1.getFarm();

            //Find chest on the map.
            farm.objects.TryGetValue(_config.ChestLocation, out var obj);
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

            if (e.IsDown(SButton.NumPad3))
            {
                if (Game1.player.currentLocation is not null && !Game1.player.currentLocation.Name.Contains("Farm"))
                    DoLocationClean();
                else
                    DoClean();
            }

            if (e.IsDown(SButton.NumPad8))
            {
                GetObjects(Game1.player.currentLocation);
            }
            if (e.IsDown(SButton.NumPad7))
            {
                if(Game1.player.currentLocation is MineShaft mine)
                {
                    int X = (Game1.player.getStandingX() / 64) + 1;
                    int Y = (Game1.player.getStandingY() / 64) + 1;
                    mine.createLadderAt(new Vector2(X, Y));
                    Monitor.Log($"X: {X}, Y: {Y}", LogLevel.Debug);
                }
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
            var farm = Game1.getFarm();
            var loc = Game1.player.currentLocation;
            if (loc is null)
                return;
            _weeds.Clear();
            _twig.Clear();
            _stone.Clear();
            _grass.Clear();
            _saplings.Clear();
            _stump.Clear();
            _largelog.Clear();
            _largestone.Clear();
            _forage.Clear();
            _mineLadders.Clear();
            if (loc.Name.Contains("Farm"))
            {
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
                    if (ter.Value is Tree)
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
            else
            {
                //Objects.
                foreach (var obj in loc.objects.Pairs.ToList())
                {
                    if (obj.Value.Name.ToLower().Contains("weed"))
                        _weeds.Add(obj.Key, obj.Value);
                    if (obj.Value.Name.ToLower().Contains("twig"))
                        _twig.Add(obj.Key, obj.Value);
                    if(obj.Value.isForage(loc))
                        _forage.Add(obj.Key, obj.Value);
                    //Filter out Geodes, and Ore stones
                    if (obj.Value.Name.ToLower().Contains("stone") && !NonNormalStones.Contains(obj.Value.ParentSheetIndex))
                        _stone.Add(obj.Key, obj.Value);
                }


                //TerrainFeatures
                foreach (var ter in loc.terrainFeatures.Pairs.ToList())
                {
                    //STree t = (STree) ter.Value;
                    if (ter.Value is Grass)
                        _grass.Add(ter.Key, ter.Value);
                    if (ter.Value is Tree)
                        _saplings.Add(ter.Key, ter.Value);
                }

                //Resource Clumps
                foreach (var clumps in loc.resourceClumps.ToList())
                {
                    if (clumps.parentSheetIndex.Value == 600)
                        _stump.Add(clumps.tile.Value, clumps);
                    if (clumps.parentSheetIndex.Value == 602)
                        _largelog.Add(clumps.tile.Value, clumps);
                    if (clumps.parentSheetIndex.Value == 672)
                        _largestone.Add(clumps.tile.Value, clumps);
                }
            }
        }


        //CutWeed, will determine what to give fiber or mixed seeds

        private void DoClean()
        {
            Monitor.Log("Running Cleaner on Farm");
            FillDictionary();
            
            //Reset the counts
            _mixedSeedsCount =
                _coalCount = _geodeCount = _hayCount = _woodCount = _fiberCount = _stoneCount = _hardWoodCount = _missedWeeds = _acornCount = _mapleCount = _pineConeCount = _sapCount = 0;

            //Lets make sure the chest exists.

            var farm = Game1.getFarm();
            Random rnd;
            farm.objects.TryGetValue(_config.ChestLocation, out var obj);
            Chest myChest;

            if (obj is Chest c)
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
                    var amt = 1 * rnd.Next(1, 2);

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
                    var t = (STree)trees.Value;
                    var tType = t.treeType.Value;
                    var tGrowth = t.growthStage.Value;
                    var tSeed = GetSeed(t);
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
                    var hwAmt = 0;
                    while (Game1.player.professions.Contains(14) && rnd.NextDouble() < 0.4)
                        hwAmt++;
                    if (hwAmt > 0)
                    {
                        myChest.addItem(DoItem(709, hwAmt));
                        _hardWoodCount += hwAmt;
                    }

                    var r = rnd.Next(1, 4);
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
                    var pid = CutWeeds();
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
                    var pid = BreakStone((int)st.Key.X, (int)st.Key.Y);
                    var amt = (pid == 382 || pid == 390) ? rnd.Next(1, 3) : 1;
                    if (pid == 382)
                        _coalCount += amt;
                    if (pid is >= 535 and < 538)
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

            var addedToChest =
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

        private void DoLocationClean()
        {
            Monitor.Log("Running Cleaner");
            FillDictionary();

            //Reset the counts
            _mixedSeedsCount =
                _coalCount = _geodeCount = _hayCount = _woodCount = _fiberCount = _stoneCount = _hardWoodCount = _missedWeeds = _acornCount = _mapleCount = _pineConeCount = _sapCount = 0;

            //Lets make sure the chest exists.

            var loc = Game1.player.currentLocation;
            //Make sure loc isn't null
            if (loc is null)
            {
                Monitor.Log($"Couldn't find a game location.", LogLevel.Trace);
                return;
            }
            var farm = Game1.getFarm();
            Random rnd;
            farm.objects.TryGetValue(_config.ChestLocation, out var obj);
            Chest myChest;

            if (obj is Chest c)
                myChest = c;
            else
            {
                myChest = new Chest(true);
                loc.objects.Add(_config.ChestLocation, myChest);
            }

            Monitor.Log("Passed the Chest Check");
            //Remove grass and add hay.
            if (_config.GrassRemoval && _grass.Any())
            {
                rnd = new Random();
                foreach (var gr in _grass.ToList())
                {
                    var amt = 1 * rnd.Next(1, 2);

                    loc.terrainFeatures.Remove(gr.Key);
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
                    var t = (STree)trees.Value;
                    var tType = t.treeType.Value;
                    var tGrowth = t.growthStage.Value;
                    var tSeed = GetSeed(t);
                    rnd = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + (int)trees.Key.X * 7 + (int)trees.Key.Y * 11);


                    //Check tGrowth values
                    if (tGrowth == (int)TreeStage.Seed && _config.TreeConfigs.ClearSeeds)
                        loc.terrainFeatures.Remove(trees.Key);
                    if (tGrowth == (int)TreeStage.Sprout && _config.TreeConfigs.ClearSprout)
                        loc.terrainFeatures.Remove(trees.Key);
                    if (tGrowth == (int)TreeStage.Sapling && _config.TreeConfigs.ClearSapling)
                        loc.terrainFeatures.Remove(trees.Key);
                    if (tGrowth == (int)TreeStage.Bush && _config.TreeConfigs.ClearBush)
                        loc.terrainFeatures.Remove(trees.Key);
                    if (tGrowth == (int)TreeStage.SmallTree && _config.TreeConfigs.ClearSmallTree)
                        loc.terrainFeatures.Remove(trees.Key);
                    if (tGrowth >= (int)TreeStage.Tree && _config.TreeConfigs.ClearFullTree)
                        loc.terrainFeatures.Remove(trees.Key);

                    //Now we check to see if anything else should have dropped

                    //Check to see if Hardwood should drop and how much
                    var hwAmt = 0;
                    while (Game1.player.professions.Contains(14) && rnd.NextDouble() < 0.4)
                        hwAmt++;
                    if (hwAmt > 0)
                    {
                        myChest.addItem(DoItem(709, hwAmt));
                        _hardWoodCount += hwAmt;
                    }

                    var r = rnd.Next(1, 4);
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


                    loc.terrainFeatures.Remove(trees.Key);
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
                    loc.objects.Remove(weed.Key);
                    var pid = CutWeeds();
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
                    if (loc.Name.Contains("UndergroundMine"))
                    {

                        int r = rnd.Next(1, _stone.Keys.Count);
                        var l = Game1.player.currentLocation as MineShaft;
                        if (r == 1)
                        {
                            //l.createLadderAt(new Vector2(st.Key.X, st.Key.Y));
                            if(!_mineLadders.Contains(new Vector2(st.Key.X, st.Key.Y)))
                            {
                                _mineLadders.Add(new Vector2(st.Key.X, st.Key.Y));
                            }
                        }
                    }
                    loc.objects.Remove(st.Key);
                    var pid = BreakStone((int)st.Key.X, (int)st.Key.Y);
                    var amt = (pid == 382 || pid == 390) ? rnd.Next(1, 3) : 1;
                    if (pid == 382)
                        _coalCount += amt;
                    if (pid is >= 535 and < 538)
                        _geodeCount++;
                    if (pid == 390)
                        _stoneCount += amt;

                    //DoMineStuff
                    if (loc.Name.Contains("UndergroundMine"))
                    {
                        var lo = loc as MineShaft;
                        foreach(var mine in _mineLadders)
                        {
                            lo.createLadderAt(new Vector2(mine.X, mine.Y));
                        }
                    }
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
                    loc.objects.Remove(twigs.Key);
                    Item i = new SObject(388, 1);
                    _woodCount++;
                    myChest.addItem(i);
                    Game1.player.gainExperience(2, 12);
                }
            }

            if (_config.ForageRemoval && _forage.Any())
            {
                foreach (var forage in _forage.ToList())
                {
                    loc.objects.Remove(forage.Key);
                    Item i = new SObject(forage.Value.ParentSheetIndex, 1);
                    myChest.addItem(i);
                    Game1.player.gainExperience(2, 12);
                }
            }
            //Remove stumps and adds HardWood
            if (_config.StumpRemoval && _stump.Any())
            {
                foreach (var stumps in _stump.ToList())
                {
                    loc.resourceClumps.Remove(stumps.Value);
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
                    loc.resourceClumps.Remove(logs.Value);
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
                    loc.resourceClumps.Remove(stones.Value);
                    Item i = new SObject(390, 15);
                    _stoneCount += 15;
                    myChest.addItem(i);
                    Game1.player.gainExperience(2, 12);
                }
            }

            //Show the message at the end.

            var addedToChest =
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

        private void GetObjects(GameLocation loc)
        {
            string foundObjects = "Found Objects: ";
            string foundTerrain = "Found Terrain: ";
            string terrain = "";
            string forage = "";
            string foundForage = "Found Forage: ";
            int objNum = 0, terNum = 0, forNum = 0;
            //Objects.
            foreach (var obj in loc.objects.Pairs.ToList())
            {
                foundObjects += $" {obj.Value.Name} X:{obj.Key.X.ToString(CultureInfo.InvariantCulture)} Y: {obj.Key.Y.ToString(CultureInfo.InvariantCulture)}in Map: {loc.Name}.";
                objNum++;
            }


            //TerrainFeatures
            foreach (var ter in loc.terrainFeatures.Pairs.ToList())
            {
                if (ter.Value is Grass)
                    terrain = "Grass";
                if (ter.Value is Tree tree)
                    terrain = "Tree(More Coming Soon)";
                foundTerrain += $" {terrain} X:{ter.Key.X.ToString(CultureInfo.InvariantCulture)} Y: {ter.Key.Y.ToString(CultureInfo.InvariantCulture)}in Map: {loc.Name}.";
                terNum++;
            }

            //Resource Clumps
            /*
            foreach (var fora in loc.)
            {
                
            }*/

            //Spit out the totals.
            Monitor.Log($"Objects ({objNum}): {foundObjects}");
            Monitor.Log($"Terrain Feature {terNum} {foundTerrain}");
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
            var random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + x * 2000 + y);
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
            var seedId = tree.treeType.Value switch
            {
                1 => //Oak
                    309,
                2 => //Maple
                    310,
                3 => //Pine
                    311,
                _ => 309
            };

            return seedId;
        }

        private static Item DoItem(int itemId)
        {
            Item i = new SObject(itemId, 1);
            return i;
        }
        private static Item DoItem(int itemId, int amount)
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
