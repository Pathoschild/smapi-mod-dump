using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Dimensions;

namespace SadisticBundles
{
    class CheatManager : IAssetEditor
    {
        // TODO: think about multiplayer. Perks for each player vs master only vs global.

        const int BForage5Perk = 0;
        const int BForage10Perk = 1;

        const int BRobinHalf = 2;
        const int BMushroomTree = 3;

        const int BFarming5Perk = 6;
        const int BFarming10Perk = 11;
        const int BShopsOpen = 14;
        const int BAlwaysLucky = 15;
        const int BMining5Perk = 18;
        const int BMining10Perk = 20;

        const int BFishing5Perk = 24;
        const int BFishing10Perk = 28;

        const int BCombat5Perk = 30;
        const int BCombat10Perk = 31;

        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public CheatManager(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
            //Helper.Events.GameLoop.DayEnding += DayEnding;
            //Helper.Events.Display.MenuChanged += MenuChanged;
            //Helper.Events.Input.ButtonPressed += ButtonPressed;
            //Helper.Events.GameLoop.Saving += Saving;
            //Helper.Events.Player.Warped += Warped;
            //Helper.Events.GameLoop.DayStarted += DayStarted;
        }

        private void Warped(object sender, WarpedEventArgs e)
        {
            if(e.NewLocation.Name == "Forest" && bundleDone(BShopsOpen))
            {
                var tile = e.NewLocation.Map.GetLayer("Buildings").Tiles[90,15];
                tile.Properties["Action"] = tile.Properties["Action"].ToString().Replace("900","600");
            }
            if (e.NewLocation.Name == "Town" && bundleDone(BShopsOpen))
            {
                var tile = e.NewLocation.Map.GetLayer("Buildings").Tiles[44,56];
                tile.Properties["Action"] = tile.Properties["Action"].ToString().Replace("900", "600");
                tile = e.NewLocation.Map.GetLayer("Buildings").Tiles[43, 56];
                tile.Properties["Action"] = tile.Properties["Action"].ToString().Replace("900", "600");
                tile = e.NewLocation.Map.GetLayer("Buildings").Tiles[94,81];
                tile.Properties["Action"] = tile.Properties["Action"].ToString().Replace("900", "600");
            }
            if (e.NewLocation.Name == "Mountain" && bundleDone(BShopsOpen))
            {
                var tile = e.NewLocation.Map.GetLayer("Buildings").Tiles[8,20];
                tile.Properties["Action"] = tile.Properties["Action"].ToString().Replace("900", "600");
            }
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (GameState.Current?.Activated != true) return;
            if (bundleDone(BAlwaysLucky))
            {
                Game1.dailyLuck = Math.Abs(Game1.dailyLuck) + .02;
            }
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.currentLocation == null) return;
            if (GameState.Current?.Activated != true) return;
            var item = Game1.player.CurrentItem;
            var loc = Game1.currentLocation;
            if (item == null || loc == null) return;

            // plant mushrooms!
            // TODO: highlight placeable tiles like with real tree seeds
            if (bundleDone(BMushroomTree) && item.ParentSheetIndex == 420 && e.Button.IsActionButton())
            {
                if (canPlantMushroom(loc, e.Cursor.GrabTile))
                {
                    Helper.Input.Suppress(e.Button);
                    loc.terrainFeatures.Remove(e.Cursor.GrabTile);
                    loc.terrainFeatures.Add(e.Cursor.GrabTile, new Tree(7, 0));
                    loc.playSound("dirtyHit");
                    if (item.Stack == 1)
                    {
                        Game1.player.removeItemFromInventory(item);
                    }
                    else { item.Stack--; }
                }
            }
        }

        private bool canPlantMushroom(GameLocation loc, Vector2 placementTile)
        {
            var tfs = loc.terrainFeatures;
            bool isLocationOpenHoeDirt = (!tfs.ContainsKey(placementTile) || !(tfs[placementTile] is HoeDirt) ? false : (tfs[placementTile] as HoeDirt).crop == null);
            string noSpawn = loc.doesTileHaveProperty((int)placementTile.X, (int)placementTile.Y, "NoSpawn", "Back");
            if (!isLocationOpenHoeDirt && (loc.objects.ContainsKey(placementTile) || tfs.ContainsKey(placementTile) || !(loc is Farm) && !loc.IsGreenhouse || noSpawn != null && (noSpawn.Equals("Tree") || noSpawn.Equals("All") || noSpawn.Equals("True"))))
            {
                Monitor.Log("invalid location");
                return false;
            }
            if (noSpawn != null && (noSpawn.Equals("Tree") || noSpawn.Equals("All") || noSpawn.Equals("True")))
            {
                Monitor.Log("nospawn");
                return false;
            }
            var x = (int)placementTile.X;
            var y = (int)placementTile.Y;
            if (!isLocationOpenHoeDirt && (!loc.isTileLocationOpen(new Location(x * 64, y * 64)) || loc.isTileOccupied(placementTile, "") || loc.doesTileHaveProperty(x, y, "Water", "Back") != null))
            {
                Monitor.Log("333");
                return false;
            }
            return true;
        }

        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (GameState.Current?.Activated != true) return;
            var p = Game1.player;

            // Farm building construction use half materials
            var carpenter = e.NewMenu as CarpenterMenu;
            if (carpenter != null)
            {
                bool magic = Helper.Reflection.GetField<bool>(carpenter, "magicalConstruction").GetValue();
                if (!magic && bundleDone(BRobinHalf))
                {
                    var info = Helper.Reflection.GetField<List<BluePrint>>(carpenter, "blueprints");
                    foreach (var print in info.GetValue())
                    {
                        foreach (var ing in print.itemsRequired.ToList())
                        {
                            print.itemsRequired[ing.Key] /= 2;
                        }
                    }
                    carpenter.setNewActiveBlueprint();
                }
            }

            // on levelup, we need to remove and restore our added level 5 perks, so the level 10 selection is based on their first choice.
            var levelup = e.NewMenu as LevelUpMenu;
            if (levelup != null)
            {
                foreach (var skill in GameState.Current.Level5PerksAdded)
                {
                    p.professions.Remove(skill);
                }
            }
            levelup = e.OldMenu as LevelUpMenu;
            if (levelup != null)
            {
                foreach (var skill in GameState.Current.Level5PerksAdded)
                {
                    p.professions.Add(skill);
                }
            }

        }

        private bool bundleDone(int id)
        {
            return Game1.netWorldState.Value.Bundles[id].All(x => x);
        }

        private string rewardMail(int id)
        {
            return $"rewardLetter{id}";
        }

        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            if (GameState.Current?.Activated != true) return;

            var p = Game1.MasterPlayer;
            checkProfession(p, BFarming5Perk, p.FarmingLevel, 5, Farmer.tiller, Farmer.rancher);
            checkProfession(p, BMining5Perk, p.MiningLevel, 5, Farmer.miner, Farmer.geologist);
            checkProfession(p, BCombat5Perk, p.CombatLevel, 5, Farmer.fighter, Farmer.scout);
            checkProfession(p, BFishing5Perk, p.FishingLevel, 5, Farmer.fisher, Farmer.trapper);
            checkProfession(p, BForage5Perk, p.ForagingLevel, 5, Farmer.gatherer, Farmer.forester);

            checkProfession(p, BFarming10Perk, p.FarmingLevel, 10, Farmer.artisan, Farmer.shepherd, Farmer.agriculturist, Farmer.butcher);
            checkProfession(p, BMining10Perk, p.MiningLevel, 10, Farmer.blacksmith, Farmer.excavator, Farmer.gemologist, Farmer.burrower);
            checkProfession(p, BCombat10Perk, p.CombatLevel, 10, Farmer.brute, Farmer.acrobat, Farmer.defender, Farmer.desperado);
            checkProfession(p, BFishing10Perk, p.FishingLevel, 10, Farmer.angler, Farmer.mariner, Farmer.pirate, Farmer.baitmaster);
            checkProfession(p, BForage10Perk, p.ForagingLevel, 10, Farmer.lumberjack, Farmer.botanist, Farmer.tapper, Farmer.tracker);

            foreach (var b in new int[]
            {
                BRobinHalf, BMushroomTree, BAlwaysLucky, BShopsOpen,
            })
            {
                if (!p.hasOrWillReceiveMail(rewardMail(b)) && bundleDone(b))
                {
                    p.mailForTomorrow.Add(rewardMail(b));
                    ModEntry.InvalidateCache();
                }
            }

        }

        private void Saving(object sender, SavingEventArgs e)
        {
            if (GameState.Current?.Activated != true) return;

        }

        private void checkProfession(Farmer p, int bundle, int level, int required, params int[] choices)
        {
            // TODO: how does the sewer statue interact with added stuff.
            var mail = rewardMail(bundle);
            if (bundleDone(bundle) && level >= required && !p.hasOrWillReceiveMail(mail))
            {
                foreach (var choice in choices)
                {
                    if (!p.professions.Contains(choice))
                    {
                        if (required == 5)
                        {
                            GameState.Current.Level5PerksAdded.Add(choice);
                        }
                        p.professions.Add(choice);
                    }
                }
                p.mailForTomorrow.Add(mail);
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return false;
            // mail is always safe, since we are just adding new fields.
            if (asset.AssetNameEquals("Data/mail")) return true;
            if (bundleDone(BShopsOpen) && shopOpenAssets.Any(x => asset.AssetNameEquals(x))) return true;
            return false;
        }

        public static IList<string> shopOpenAssets = new List<string> { "Characters/schedules/Marnie", "Characters/schedules/Robin", "Characters/schedules/Pierre", "Characters/schedules/Clint" };

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/mail"))
            {
                var dict = asset.AsDictionary<string, string>().Data;
                // all reward mails
                var pre = Helper.Translation.Get("rewardLetterPrefix").ToString();
                var post = Helper.Translation.Get("rewardLetterSuffix").ToString();
                foreach (var id in Game1.netWorldState.Value.BundleRewards.Keys)
                {
                    var key = rewardMail(id);
                    dict[key] = pre + Helper.Translation.Get(key).ToString() + post;
                }
            }
            if (bundleDone(BShopsOpen))
            {
                string loc = "";
                if (asset.AssetNameEquals("Characters/schedules/Marnie"))
                {
                    loc = "600 AnimalShop 17 5 0";
                }
                if (asset.AssetNameEquals("Characters/schedules/Pierre"))
                {
                    loc = "600 SeedShop 10 20 0";
                }
                if (asset.AssetNameEquals("Characters/schedules/Robin"))
                {
                    loc = "600 ScienceHouse 8 18 2";
                }
                if (asset.AssetNameEquals("Characters/schedules/Clint"))
                {
                    loc = "600 Blacksmith 3 13 2";
                }
                if (loc != "")
                {
                    var dict = asset.AsDictionary<string, string>().Data;
                    foreach (var key in dict.Keys.ToList())
                    {
                        var val = dict[key];
                        if (val.StartsWith("GOTO")) continue;
                        dict[key] = loc + "/" + dict[key];
                    }
                }
            }

        }
    }
}
