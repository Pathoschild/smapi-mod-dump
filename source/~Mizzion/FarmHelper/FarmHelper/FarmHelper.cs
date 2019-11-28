using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FarmHelper.Framework;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace FarmHelper
{
    public class FarmHelper : Mod
    {
        //Properties Start
        //Is Mod Enabled
        private bool _modEnabled;

        //Acitvation Key Setting
        private SButton _activationKey;

        private SButton _useToolKey;

        private SButton _clearLocationKey;

        private SButton _forageKey;

        private SButton _singleKey;

        //Where the mod runs at the beginning of eachday
        private bool _automaticMode;

        //To pet or not to pet
        private bool _enablePetting;

        //Whether or not to Harvest Animal Products
        private bool _harvestProduct;

        //Are Notifications Enables
        private bool _enableNotifications;

        //Where Charging is Enabled
        private bool _enableCost;

        //What is the charge
        private int _helperCost;

        //Whether or not to Add Products right into the players Inventory.
        private bool _addItemsToInventory;

        //The location of the Chest if AddItemsToInventory == False
        private Vector2 _chestLocation;

        //ModConfig
        private ModConfig _config;

        //Total Times Action Performed.
        private int _count;

        //GameLocations used for harvesting crops
        private GameLocation[] _locations;
        //Properties End

        //Public Methods 
        
        /// <summary>
        /// The main Entry for the mod. It loads before anything else here.
        /// </summary>
        /// <param name="helper">Helper class to make things easier</param>
        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<ModConfig>(); //Read the ModConfig.

            //Events
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

        }

        //Event Methods

        /// <summary>
        /// Event that runs when the day starts
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event args</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            //Initialise Config
            InitiliseConfig();
            ProcessLiveStock();
        }
        
        /// <summary>
        /// Event that runs when a button is pressed.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event args</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !_modEnabled)
                return;

            //Check to see if Action Key was pressed
            if(e.IsDown(_activationKey))
            {
                ProcessLiveStock();
                HarvestCrops();
                CheckDogCat();
            }
                
            //Check to see if Clear location key was pressed
            if (e.IsDown(_clearLocationKey))
                ClearCurrentLocation(Game1.currentLocation);
            if (e.IsDown(_useToolKey))
            {
                ICursorPosition cur = Helper.Input.GetCursorPosition();
                UseTool(Game1.player.getTileLocation(), cur.Tile, Game1.currentLocation);
            }
                
            if(e.IsDown(_forageKey))
                DoForageHarvest();
            if (e.IsDown(_singleKey))
            {
                ICursorPosition cur = Helper.Input.GetCursorPosition();
                SingleToolUse(cur.Tile, Game1.currentLocation);
            }

            if (e.IsDown(SButton.F3))
            {
                ICursorPosition c = Helper.Input.GetCursorPosition();
                Game1.player.position.Value  = c.Tile * Game1.tileSize;
            }

            if (e.IsDown(SButton.F4))
            {
                GameLocation loc = Game1.currentLocation;

                if (loc != null && loc is MineShaft shaft && shaft.mineLevel > 120)
                {
                    shaft.enterMineShaft();
                }
            }
                
        }


        //Private Methods

        /// <summary>
        /// Sets up the config settings. This way we can reload them any time.
        /// </summary>
        private void InitiliseConfig()
        {
            if(!Enum.TryParse(_config.ActivationKey, true, out _activationKey))
            {
                _activationKey = SButton.Q;
                Monitor.Log("There was an error parsing the ActivationKey. It was reset to Q");
            }
            if (!Enum.TryParse(_config.UseToolKey, true, out _useToolKey))
            {
                _useToolKey = SButton.Z;
                Monitor.Log("There was an error parsing the UseToolKey. It was reset to Z");
            }
            if (!Enum.TryParse(_config.ClearLocationKey, true, out _clearLocationKey))
            {
                _clearLocationKey = SButton.R;
                Monitor.Log("There was an error parsing the ClearLocationKey. It was reset to R");
            }
            if (!Enum.TryParse(_config.GatherForageKey, true, out _forageKey))
            {
                _forageKey = SButton.X;
                Monitor.Log("There was an error parsing the GatherForageKey. It was reset to X");
            }
            if (!Enum.TryParse(_config.SingleUseKey, true, out _singleKey))
            {
                _singleKey = SButton.V;
                Monitor.Log("There was an error parsing the SingleUseKey. It was reset to V");
            }
            _modEnabled = _config.ModEnabled;
            _automaticMode = _config.AutomaticMode;
            _enablePetting = _config.EnablePetting;
            _harvestProduct = _config.HarvestAnimalProducts;
            _enableNotifications = _config.EnableNotification;
            _enableCost = _config.EnableCost;
            _helperCost = _config.HelperCost;
            _addItemsToInventory = _config.AddItemsToInventory;
            _chestLocation = _config.ChestLocation;
            
            //Make sure Cost is positive
            if(_config.HelperCost < 0)
            {
                _helperCost = 0;
                Monitor.Log("HelperCost can't be negative. Setting it to 0");
            }
            else
            {
                _helperCost = _config.HelperCost;
            }
        
    }

        /// <summary>
        /// Bool that tells us if it can add an Item to the players inventory
        /// </summary>
        /// <param name="obj">The Object</param>
        /// <param name="who">The Player</param>
        /// <returns></returns>
        private bool GiveItemToPlayer(SObject obj, SFarmer who)
        {

            if (_addItemsToInventory)
            {
                if (who.couldInventoryAcceptThisItem(obj))
                {
                    who.addItemToInventory(obj);
                    return true;
                }
            }

            //Player hasn't edited the config, we should try the chest.
            //Find chest on the map.
            Game1.getFarm().objects.TryGetValue(_config.ChestLocation, out SObject _obj);
            

            if (_obj != null && _obj is Chest c)
            {
                Item item = c.addItem(obj);
                if (item == null)
                    return true;
            }

            return false;
        }

        private void SingleToolUse(Vector2 pos, GameLocation loc)
        {
            var player = Game1.player;

            Axe fakeAxe = new Axe { UpgradeLevel = 4 };
            Pickaxe fakePick = new Pickaxe { UpgradeLevel = 4 };
            MeleeWeapon fakeSickle = new MeleeWeapon { UpgradeLevel = 4 };
            Hoe fakeHoe = new Hoe { UpgradeLevel = 4 };
            WateringCan fakeCan = new WateringCan { UpgradeLevel = 4 };
            Vector2 useAt = (new Vector2(pos.X, pos.Y) * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);
            Game1.player.lastClick = useAt;

            loc.objects.TryGetValue(new Vector2(pos.X, pos.Y), out var obj);
            loc.terrainFeatures.TryGetValue(new Vector2(pos.X, pos.Y), out var ter);

            //Check obj
            if (obj != null)
            {
                bool forage = obj.IsSpawnedObject;//checkForAction(Game1.player);
                if (forage)
                    DoForageHarvest(obj, loc, player);
                if (obj.Name.ToLower().Contains("twig"))
                    fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, player);
                if (obj.Name.ToLower().Contains("stone"))
                    fakePick.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, player);
                if (obj.Name.ToLower().Contains("weed"))
                    fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, player);
            }
            //Check ter
            if (ter != null)
            {
                if (ter is Tree tree)
                    fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, player);

                if (ter is Grass)
                {
                    Random rdn = new Random();
                    loc.terrainFeatures.Remove(new Vector2(pos.X, pos.Y));
                    Game1.createMultipleObjectDebris(178, (int)pos.X, (int)pos.Y, rdn.Next(2), loc);
                }
            }

            if (ter != null && ter is HoeDirt dirt)
            {
                if (dirt.crop == null &&
                    player.ActiveObject != null &&
                    ((player.ActiveObject.Category == SObject.SeedsCategory || player.ActiveObject.Category == -19) &&
                     dirt.canPlantThisSeedHere(player.ActiveObject.ParentSheetIndex, (int)useAt.X, (int)useAt.Y, player.ActiveObject.Category == -19)))
                {
                    if ((dirt.plant(player.ActiveObject.ParentSheetIndex, (int)useAt.X, (int)useAt.Y, player, player.ActiveObject.Category == -19, loc) && player.IsLocalPlayer))
                        player.reduceActiveItemByOne();
                    Game1.haltAfterCheck = false;
                }
                else if (dirt.crop != null)
                {
                    if (dirt.crop.fullyGrown.Value)
                    {
                        dirt.crop.harvest((int)useAt.X, (int)useAt.Y, dirt);
                    }
                    else if (player.ActiveObject != null && player.ActiveObject.Category == -19)
                    {
                        dirt.fertilizer.Value = player.ActiveObject.ParentSheetIndex;
                        player.reduceActiveItemByOne();
                    }
                    else
                        fakeCan.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, Game1.player);
                }
                else
                    fakePick.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, Game1.player);
            }
            else
                fakeHoe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 1, Game1.player);


            //Lets try bush shit
            Rectangle rectangle = new Rectangle((int)useAt.X + 32, (int)useAt.Y - 32, 4, 192);
            foreach (LargeTerrainFeature largeTerrainFeature in loc.largeTerrainFeatures)
            {
                if (largeTerrainFeature is Bush bush && bush.getBoundingBox().Intersects(rectangle))
                {
                    bush.performUseAction(bush.tilePosition.Value, loc);
                }
            }
            
            Game1.player.Stamina = Game1.player.MaxStamina;//So we dont passout lol
        }
        private void UseTool(Vector2 startPos, Vector2 endPos, GameLocation loc)
        {
            var player = Game1.player;

            Axe fakeAxe = new Axe { UpgradeLevel = 4 };
            Pickaxe fakePick = new Pickaxe { UpgradeLevel = 4 };
            MeleeWeapon fakeSickle = new MeleeWeapon { UpgradeLevel = 4 };
            Hoe fakeHoe = new Hoe { UpgradeLevel = 4 };
            WateringCan fakeCan = new WateringCan { UpgradeLevel = 4 };
            Game1.player.MagneticRadius = 650;

            for (int xTile = Convert.ToInt32(startPos.X); xTile <= Convert.ToInt32(endPos.X); ++xTile)
            {
                for (int yTile = Convert.ToInt32(startPos.Y); yTile <= Convert.ToInt32(endPos.Y); ++yTile)
                {

                    Vector2 useAt = (new Vector2(xTile, yTile) * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);
                    Game1.player.lastClick = useAt;

                    loc.objects.TryGetValue(new Vector2(xTile, yTile), out var obj);
                    loc.terrainFeatures.TryGetValue(new Vector2(xTile, yTile), out var ter);

                    //Check obj
                    if (obj != null)
                    {
                        bool forage = obj.IsSpawnedObject;//checkForAction(Game1.player);
                        if (forage)
                            DoForageHarvest(obj, loc, player);
                        if (obj.Name.ToLower().Contains("twig"))
                            fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, player);
                        if (obj.Name.ToLower().Contains("stone"))
                            fakePick.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, player);
                        if (obj.Name.ToLower().Contains("weed"))
                            fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, player);
                    }
                    //Check ter
                    if(ter != null)
                    {
                        if(ter is Tree tree)
                            fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, player);

                        if (ter is Grass)
                        {
                            Random rdn = new Random();
                            loc.terrainFeatures.Remove(new Vector2(xTile, yTile));
                            Game1.createMultipleObjectDebris(178, xTile, yTile, rdn.Next(2), loc);
                        }
                    }

                    if (ter != null && ter is HoeDirt dirt)
                    {
                        if (dirt.crop == null &&
                            player.ActiveObject != null &&
                            ((player.ActiveObject.Category == SObject.SeedsCategory || player.ActiveObject.Category == -19) &&
                             dirt.canPlantThisSeedHere(player.ActiveObject.ParentSheetIndex, (int)useAt.X, (int)useAt.Y, player.ActiveObject.Category == -19)))
                        {
                            if ((dirt.plant(player.ActiveObject.ParentSheetIndex, (int)useAt.X, (int)useAt.Y, player, player.ActiveObject.Category == -19, loc) && player.IsLocalPlayer))
                                player.reduceActiveItemByOne();
                            Game1.haltAfterCheck = false;
                        }
                        else if (dirt.crop != null)
                        {
                            if (dirt.crop.fullyGrown.Value)
                            {
                                dirt.crop.harvest((int)useAt.X, (int)useAt.Y, dirt);
                            }
                            else if (player.ActiveObject != null && player.ActiveObject.Category == -19)
                            {
                                dirt.fertilizer.Value = player.ActiveObject.ParentSheetIndex;
                                player.reduceActiveItemByOne();
                            }
                            else
                                fakeCan.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, Game1.player);
                        }
                        else
                            fakePick.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, Game1.player);
                    }
                    else
                        fakeHoe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 1, Game1.player);


                    //Lets try bush shit
                    Rectangle rectangle = new Rectangle((int)useAt.X + 32, (int)useAt.Y - 32, 4, 192);
                    foreach (LargeTerrainFeature largeTerrainFeature in loc.largeTerrainFeatures)
                    {
                        if (largeTerrainFeature is Bush bush && bush.getBoundingBox().Intersects(rectangle))
                        {
                            bush.performUseAction(bush.tilePosition.Value, loc);
                        }
                    }

                    //Resource Clumps
                    ResourceClump clump = GetResourceClumpCoveringTile(loc, new Vector2(useAt.X, useAt.Y));
                    if(clump != null)
                    {
                        if (clump.parentSheetIndex.Value == 600)
                        {
                            clump.health.Value = 1;
                            fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 1, Game1.player);
                        }

                        if (clump.parentSheetIndex.Value == 602)
                        {
                            clump.health.Value = 1;
                            fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 1, Game1.player);
                        }

                        if (clump.parentSheetIndex.Value == 672)
                        {
                            clump.health.Value = 1;
                            fakePick.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 1, Game1.player);
                        }
                    }

                    Game1.player.Stamina = Game1.player.MaxStamina;//So we dont passout lol
                }
            }
        }
        /// <summary>
        /// Checks and pets the players Dog/Cat
        /// </summary>
        private void CheckDogCat()
        {
            Farm pFarm = Game1.getFarm();
            SFarmer player = Game1.player;

            if (!player.hasPet()) return;

            foreach (NPC character in Game1.getFarm().characters)
                if (character is Pet pet)
                    PetPet(pet);

            foreach (NPC character in Utility.getHomeOfFarmer(player).characters)
                if (character is Pet pet)
                    PetPet(pet);
            pFarm.petBowlWatered.Value = true;
            //pFarm.setMapTileIndex(54, 7, 1939, "Buildings");
        }
        
        private void ClearCurrentLocation(GameLocation loc)
        {
            Axe fakeAxe = new Axe { UpgradeLevel = 4 };
            Pickaxe fakePick = new Pickaxe { UpgradeLevel = 4 };
            Game1.player.MagneticRadius = 650;
            int curStam = Convert.ToInt32(Game1.player.Stamina);
            for (int xTile = 0; xTile < loc.Map.Layers[0].LayerWidth; ++xTile)
            {
                for (int yTile = 0; yTile < loc.Map.Layers[0].LayerHeight; ++yTile)
                {
                    
                    loc.objects.TryGetValue(new Vector2(xTile, yTile), out var obj);
                    loc.terrainFeatures.TryGetValue(new Vector2(xTile, yTile), out var ter);
                    SFarmer who = Game1.player;
                    Vector2 useAt = (new Vector2(xTile, yTile) * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);
                    if (obj != null)
                    {
                        bool forage = obj.IsSpawnedObject;//checkForAction(Game1.player);
                        if (forage)
                            DoForageHarvest(obj, loc, who);
                        if (obj.Name.ToLower().Contains("twig"))
                            fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, who);
                        if (obj.Name.ToLower().Contains("stone"))
                            fakePick.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, who);
                        if (obj.Name.ToLower().Contains("weed"))
                            fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, who);

                    }

                    if (ter != null)
                    {
                        if (ter is Tree tree)
                        {
                            tree.health.Value = 1;
                            fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, who);
                            fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, who);

                        }

                        if (ter is Grass)
                        {
                            Random rdn = new Random();
                            loc.terrainFeatures.Remove(new Vector2(xTile, yTile));
                            Game1.createMultipleObjectDebris(178, xTile, yTile, rdn.Next(2), loc);
                        }
                        if (ter is HoeDirt dirt)
                        {
                            if (dirt.crop != null)
                            {
                                dirt.fertilizer.Value = 369;
                                dirt.state.Value = 1;
                            }
                        }
                    }

                    //Lets try bush shit

                    Rectangle rectangle = new Rectangle((int)useAt.X + 32, (int)useAt.Y - 32, 4, 192);
                    foreach (LargeTerrainFeature largeTerrainFeature in loc.largeTerrainFeatures)
                    {
                        if (largeTerrainFeature is Bush bush && bush.getBoundingBox().Intersects(rectangle))
                        {
                            bush.performUseAction(bush.tilePosition.Value, loc);
                        }
                    }


                    //Try to do monsters
                    Rectangle rect = new Rectangle((int)useAt.X, (int)useAt.Y, 4, 4);
                    loc.damageMonster(rect, 1000, 1000, false, Game1.player);
                    /*var monster = loc.characters;
                    
                    foreach (var m in monster)
                    {
                        if (m is Monster mon)
                        {
                            mon.Health = 1;
                            fakeAxe.DoFunction(loc, (int)useAt.X, (int)useAt.Y, 0, who);
                            //MeleeWeapon wep = new MeleeWeapon();
                            //wep.minDamage.Value = 100;
                            //wep.maxDamage.Value = 1000;
                            //wep.DoDamage(loc, mon.getTileX(), mon.getStandingY(), Game1.player.facingDirection.Value,  1, Game1.player);
                        }
                    }*/

                    //Reset Stamina
                    Game1.player.Stamina = curStam;
                }
            }
        }

        /// <summary>
        /// Gathers the Animal Products
        /// </summary>
        private void GatherProducts()
        {
            Farm farm = Game1.getFarm();
            SFarmer who = Game1.player;
            List<Vector2> produceLoc = new List<Vector2>();
            //Scan Coops
            foreach (Building building in farm.buildings)
            {
                if (building is Coop)
                {
                    foreach (var produce in building.indoors.Value.objects.Pairs)
                    {
                        SObject obj = produce.Value;

                        if (obj.isAnimalProduct() || obj.ParentSheetIndex == 107)
                        {
                            if (GiveItemToPlayer(obj, who))
                            {
                                produceLoc.Add(produce.Key);
                                who.gainExperience(0, 5);
                            }
                            else
                                Monitor.Log("Couldnt add the item to the players inventory, and couldnt find a chest.", LogLevel.Trace);
                        }
                    }
                    //Now we remove the produce from the coops
                    foreach (var pro in produceLoc)
                        building.indoors.Value.removeObject(pro, false);
                }
            }

            produceLoc.Clear();

            //Now we scan for Truffles
            foreach (var obj in farm.Objects.Pairs)
            {
                if (obj.Value.Name.Contains("Truffle"))
                {
                    bool doubleChance = Game1.random.NextDouble() < 0.2;


                    obj.Value.Quality = who.professions.Contains(16) ? 4 : obj.Value.Quality;

                    if (Game1.player.professions.Contains(13) && doubleChance)
                        obj.Value.Stack = 2;

                    //Try to add to inventory or chest
                    if (GiveItemToPlayer(obj.Value, who))
                    {
                        produceLoc.Add(obj.Key);
                        int amt = obj.Value.Stack > 1 ? 14 : 7;

                        who.gainExperience(2, amt);
                    }
                    else
                        Monitor.Log($"Inventory was full, couldnt add {obj.Value.Name}.");

                }
            }

            foreach (var p in produceLoc)
                farm.removeObject(p, false);
        }


        private void HarvestCrops()
        {
            SFarmer who = Game1.player;
            _locations = GetAllLocations().ToArray();

            foreach (GameLocation loc in _locations)
            {
                if (loc.Name.Contains("FarmExpan") || loc.Name.Contains("Greenhouse") || loc.IsFarm)
                {
                    foreach (var pair in loc.terrainFeatures.Pairs)
                    {
                        if (pair.Value is HoeDirt dirt)
                        {
                            if (dirt.crop != null)
                            {
                                Crop crop = dirt.crop;
                                if (crop.currentPhase.Value >= crop.phaseDays.Count - 1 && (!crop.fullyGrown.Value || crop.dayOfCurrentPhase.Value <= 0))
                                {
                                    SObject i = HarvestedCrop(dirt, crop, (int)pair.Key.X, (int)pair.Key.Y);
                                    var harvest = GiveItemToPlayer(i, who);
                                    if (harvest)
                                    {
                                        int placeHolder = 0;
                                        if (placeHolder == 1)
                                        {
                                            //Will be to sale
                                        }
                                        else
                                        {
                                            if (crop.regrowAfterHarvest.Value != -1)
                                                crop.dayOfCurrentPhase.Value = crop.regrowAfterHarvest.Value;
                                            else
                                                dirt.destroyCrop(pair.Key, false, loc);
                                            if (crop.dead.Value)
                                            {
                                                dirt.destroyCrop(pair.Key, false, loc);
                                            }
                                        }
                                        float exp = (float)(16.0 * Math.Log(0.018 * Convert.ToInt32(Game1.objectInformation[crop.indexOfHarvest.Value].Split('/')[1]) + 1.0, Math.E));
                                        who.gainExperience(0, (int)Math.Round(exp));
                                        _count++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets the Harvested Crop
        /// </summary>
        /// <param name="dirt">The HoeDirt</param>
        /// <param name="crop">The Crop</param>
        /// <param name="x">X tile of Crop</param>
        /// <param name="y">Y tile of Crop</param>
        /// <returns></returns>
        private SObject HarvestedCrop(HoeDirt dirt, Crop crop, int x, int y)
        {
            SFarmer player = Game1.player;
            int stack = 1;
            int iQuality = 0;
            int fBuff = 0;
            Random rnd = new Random(x * 7 + y + 11 + (int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame);

            switch (dirt.fertilizer.Value)
            {
                case 368:
                    fBuff = 1;
                    break;
                case 369:
                    fBuff = 2;
                    break;
            }
            double qMod = 0.2 * (player.FarmingLevel / 10.0) + 0.2 * fBuff * ((player.FarmingLevel + 2) / 12.0) + 0.01;
            //double qModifier = Math.Min(0.75, qMod * 2.0);
            if (rnd.NextDouble() < qMod)
                iQuality = 1;
            if (crop.minHarvest.Value > 1 || crop.maxHarvest.Value > 1)
                stack = rnd.Next(crop.minHarvest.Value, Math.Min(crop.minHarvest.Value + 1, crop.maxHarvest.Value + 1 + player.FarmingLevel / crop.maxHarvestIncreasePerFarmingLevel.Value));
            if (crop.chanceForExtraCrops.Value > 0.0)
            {
                while (rnd.NextDouble() < Math.Min(0.9, crop.chanceForExtraCrops.Value))
                    stack++;
            }
            if (rnd.NextDouble() < player.luckLevel.Value / 1500.0 + player.DailyLuck / 1200.0 + 9.99999974737875E-05)
                stack *= 2;
            if (crop.indexOfHarvest.Value == 421)
            {
                crop.indexOfHarvest.Value = 431;
                stack = rnd.Next(1, 4);
            }

            return new SObject(crop.indexOfHarvest.Value, stack, false, -1, iQuality);

        }


        /// <summary>
        /// Harvests Foragable items from a given location
        /// </summary>
        /// <param name="obj">The Forage Object</param>
        /// <param name="loc">Location of object</param>
        /// <param name="who">The player</param>
        private void DoForageHarvest(SObject obj, GameLocation loc, SFarmer who)
        {
            loc.checkAction(new Location((int)obj.TileLocation.X, (int)obj.TileLocation.Y), Game1.viewport, Game1.player);
            
            /*
            int quality = obj.Quality;
            Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)obj.TileLocation.X + (int)obj.TileLocation.Y * 777);
            if (who.professions.Contains(16) && obj.isForage(loc))
                obj.Quality = 4;
            else if (obj.isForage(loc))
            {
                if (random.NextDouble() < who.ForagingLevel / 30.0)
                    obj.Quality = 2;
                else if (random.NextDouble() < who.ForagingLevel / 15.0)
                    obj.Quality = 1;
            }
            if (who.couldInventoryAcceptThisItem(obj))
            {
                if (who.IsLocalPlayer)
                {
                    DelayedAction.playSoundAfterDelay("coin", 300);
                }
                who.animateOnce(279 + who.FacingDirection);
                if (!loc.isFarmBuildingInterior())
                {
                    if (obj.isForage(loc))
                        who.gainExperience(2, 7);
                }
                else
                    who.gainExperience(0, 5);
                who.addItemToInventoryBool(obj.getOne());
                ++Game1.stats.ItemsForaged;
                if (who.professions.Contains(13) && random.NextDouble() < 0.2 && who.couldInventoryAcceptThisItem(obj) && !loc.isFarmBuildingInterior())
                {
                    who.addItemToInventoryBool(obj.getOne());
                    who.gainExperience(2, 7);
                }
                loc.objects.Remove(new Vector2(obj.TileLocation.X, obj.TileLocation.Y));
            }
            obj.Quality = quality;

            */
        }

        /// <summary>
        /// Harvests Forage items from the players current map. Is meant to be used alone.
        /// </summary>
        private void DoForageHarvest()
        {
            GameLocation loc = Game1.currentLocation;
            Game1.player.MagneticRadius = 650;
            int curStam = Convert.ToInt32(Game1.player.Stamina);
            for (int xTile = 0; xTile < loc.Map.Layers[0].LayerWidth; ++xTile)
            {
                for (int yTile = 0; yTile < loc.Map.Layers[0].LayerHeight; ++yTile)
                {
                    loc.objects.TryGetValue(new Vector2(xTile, yTile), out var obj);
                    loc.terrainFeatures.TryGetValue(new Vector2(xTile, yTile), out var ter);
                    SFarmer who = Game1.player;
                    Vector2 useAt = (new Vector2(xTile, yTile) * Game1.tileSize) + new Vector2(Game1.tileSize / 2f);
                    if (obj != null)
                    {
                        bool forage = obj.IsSpawnedObject; //checkForAction(Game1.player);
                        if (forage)
                            DoForageHarvest(obj, loc, who);
                    }

                    //Reset Stamina
                        Game1.player.Stamina = curStam;
                }
            }
        }
        /// <summary>
        /// Processes the animals on the farm.
        /// </summary>
        private void ProcessLiveStock()
        {
            SFarmer who = Game1.player;
            _count = 0;
            foreach (var animal in GetLivestock())
            {
                try
                {
                    //Check to see if the Livestock needs to be pet.
                    if (!animal.wasPet.Value && _enablePetting)
                    {
                        animal.pet(who);
                        _count++;
                    }

                    if (animal.currentProduce.Value > 0 && _harvestProduct)
                    {
                        SObject obj = new SObject(animal.currentProduce.Value, 1, false, -1, animal.produceQuality.Value);
                        animal.currentProduce.Value = 0;
                        GiveItemToPlayer(obj, who);
                        _count++;
                    }

                }
                catch (Exception ex)
                {
                    Monitor.Log($"Ran into a problem ProcessingLiveStock....\n {ex}", LogLevel.Trace);
                }
                //Start processeing buildings and getting truffles from the ground.
                HarvestCrops();
                GatherProducts();
                CheckDogCat();
                
                //ToDo add in costs

            }
            Monitor.Log($"Processed {_count} tasks.");
        }
        
        /// <summary>
        /// Gathers a list of all Livestock on the farm.
        /// </summary>
        /// <returns></returns>
        private List<FarmAnimal> GetLivestock()
        {
            Farm farm = Game1.getFarm();
            List<FarmAnimal> animals = farm.getAllFarmAnimals().ToList();

            foreach(Building building in farm.buildings)
                if(building.indoors.Value != null && building.indoors.Value.GetType() == typeof(AnimalHouse))
                    animals.AddRange(((AnimalHouse)building.indoors.Value).animals.Values.ToList());
            return animals;
        }

        /// <summary>
        /// Gathers all the locations. Taken from Pathos code :P
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GameLocation> GetAllLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }

        protected Rectangle GetAbsoluteTileArea(Vector2 tile)
        {
            Vector2 pos = tile * Game1.tileSize;
            return new Rectangle((int)pos.X, (int)pos.Y, Game1.tileSize, Game1.tileSize);
        }

        //Ripping from Patos Tractor Mod

        /// <summary>Get resource clumps in a given location.</summary>
        /// <param name="location">The location to search.</param>
        protected IEnumerable<ResourceClump> GetResourceClumps(GameLocation location)
        {
            switch (location)
            {
                case Farm farm:
                    return farm.resourceClumps;

                case Forest forest:
                    return forest.log != null
                        ? new[] { forest.log }
                        : new ResourceClump[0];

                case Woods woods:
                    return woods.stumps;

                case MineShaft mineshaft:
                    return mineshaft.resourceClumps;

                default:
                    if (location.Name == "DeepWoods" || location.Name.StartsWith("DeepWoods_"))
                        return Helper.Reflection.GetField<IList<ResourceClump>>(location, "resourceClumps", required: false)?.GetValue() ?? new ResourceClump[0];
                    return new ResourceClump[0];
            }
        }

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
        public void PetPet(Pet pet)
        {
            if (!pet.lastPetDay.ContainsKey(Game1.player.UniqueMultiplayerID))
                pet.lastPetDay.Add(Game1.player.UniqueMultiplayerID, Game1.Date.TotalDays);
            else
                pet.lastPetDay[Game1.player.UniqueMultiplayerID] = Game1.Date.TotalDays;
        }
    }
}
