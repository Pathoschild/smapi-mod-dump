using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarmHelper.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewValley.Objects;
using StardewValley.Characters;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;

namespace FarmHelper
{
    public class FarmHelper : Mod
    {
        //Properties Start
        //Is Mod Enabled
        private bool ModEnabled = true;

        //Acitvation Key Setting
        private Keys ActivationKey;

        //Where the mod runs at the beginning of eachday
        private bool AutomaticMode = true;

        //To pet or not to pet
        private bool EnablePetting = true;

        //Whether or not to Harvest Animal Products
        private bool HarvestProduct = true;

        //Whether or not to Harvest Truffle
        private bool HarvestTruffle = true;

        //Are Notifications Enables
        private bool EnableNotifications = true;

        //Where Charging is Enabled
        private bool EnableCost = true;

        //What is the charge
        private int HelperCost = 50;

        //Whether or not to Add Products right into the players Inventory.
        private bool AddItemsToInventory = true;

        //The location of the Chest if AddItemsToInventory == False
        private Vector2 ChestLocation = new Vector2(0, 0);

        //ModConfig
        private ModConfig Config;

        //Total Times Action Performed.
        private int count = 0;

        //GameLocations used for harvesting crops
        private GameLocation[] locations;
        //Properties End

        //Public Methods      
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>(); //Read the ModConfig.
                        
            //Events
            TimeEvents.AfterDayStarted += AfterDayStarted;
            ControlEvents.KeyReleased += KeyReleased;

        }

        //Private Methods
        private void InitiliseConfig()
        {
            if(!Enum.TryParse(this.Config.ActivationKey, true, out this.ActivationKey))
            {
                this.ActivationKey = Keys.NumPad7;
                this.Monitor.Log("There was an error parsing the ActivationKey. It was reset to NumPad7");
            }
            this.ModEnabled = this.Config.ModEnabled;
            this.AutomaticMode = this.Config.AutomaticMode;
            this.EnablePetting = this.Config.EnablePetting;
            this.HarvestProduct = this.Config.HarvestAnimals;
            this.HarvestTruffle = this.Config.HarvestTruffles;
            this.EnableNotifications = this.Config.EnableNotification;
            this.EnableCost = this.Config.EnableCost;
            this.HelperCost = this.Config.HelperCost;
            this.AddItemsToInventory = this.Config.AddItemsToInventory;
            this.ChestLocation = this.Config.ChestLocation;
            
            //Make sure Cost is positive
            if(this.Config.HelperCost < 0)
            {
                this.HelperCost = 0;
                this.Monitor.Log("HelperCost can't be negative. Setting it to 0");
            }
            else
            {
                this.HelperCost = this.Config.HelperCost;
            }
        
    }

        private void AfterDayStarted(object sender, EventArgs e)
        {
            //Initialise Config
            this.InitiliseConfig();
            startPetting();
        }

        private void KeyReleased(object sender, EventArgsKeyPressed e)
        {
            if (!Context.IsWorldReady)
                return;
            if(e.KeyPressed == Keys.F5)
            {
                this.Config = this.Helper.ReadConfig<ModConfig>();
                InitiliseConfig();
            }
            if(e.KeyPressed == this.ActivationKey)
            {
                startPetting();
            }
            if(e.KeyPressed == Keys.NumPad6)
            {
                CheckPets();
            }
        }

        private void startPetting()
        {
            SFarmer Player = Game1.player;
            this.locations = GetAllLocations().ToArray();
            foreach(FarmAnimal animal in this.getAnimals())
            {
                try
                {
                    //Pet Animal
                    if(!animal.wasPet && this.EnablePetting)
                    {
                        animal.pet(Player);
                    }
                    //Do Harvest
                    if(animal.currentProduce > 0 && this.HarvestProduct)
                    {
                        if (animal.type.Equals("Pig"))
                        {
                            if (this.HarvestTruffle)
                            {
                                SObject @object = new SObject(animal.currentProduce, 1, false, -1, animal.produceQuality);
                                this.AddToPlayer(@object, Player);
                                animal.currentProduce = 0;
                                this.count++;
                            }
                        }
                        else
                        {
                            SObject @object = new SObject(animal.currentProduce, 1, false, -1, animal.produceQuality);
                            this.AddToPlayer(@object, Player);
                            animal.currentProduce = 0;
                            this.count++;
                        }
                    }
                }
                catch(Exception e)
                {
                    this.Monitor.Log($"Enountered an Error: {e.ToString()}.");
                }
            }
            //Start Harvesting of crops.
            foreach(GameLocation loc in this.locations)
            {
                if(loc.name.Contains("FarmExpan") || loc.name.Contains("Greenhouse") || loc.isFarm)
                {
                    foreach(KeyValuePair<Vector2, TerrainFeature> pair in loc.terrainFeatures)
                    {
                        var harvest = true;
                        if(pair.Value is HoeDirt dirt)
                        {
                            if(dirt.crop != null)
                            {
                                Crop crop = dirt.crop;
                                if(crop.currentPhase >= crop.phaseDays.Count - 1 && (!crop.fullyGrown || crop.dayOfCurrentPhase <= 0))
                                {
                                    SObject i = this.HarvestedCrop(dirt, crop, (int)pair.Key.X, (int)pair.Key.Y);
                                    if (this.AddToPlayer(i, Player))
                                    {
                                        harvest = true;
                                        //Todo SunFlowers
                                    }
                                    else
                                        harvest = false;
                                    if (harvest)
                                    {
                                        int PlaceHolder = 0;
                                        if(PlaceHolder == 1)
                                        {
                                            //Will be to sale
                                        }
                                        else
                                        {
                                            if (crop.regrowAfterHarvest != -1)
                                                crop.dayOfCurrentPhase = crop.regrowAfterHarvest;
                                            else
                                                dirt.destroyCrop(pair.Key);
                                            if (crop.dead)
                                            {
                                                dirt.destroyCrop(pair.Key);
                                            }
                                        }
                                        float exp = (float)(16.0 * Math.Log(0.018 * Convert.ToInt32(Game1.objectInformation[crop.indexOfHarvest].Split('/')[1]) + 1.0, Math.E));
                                        Player.gainExperience(0, (int)Math.Round(exp));
                                        count++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.HarvestTruffles();
            this.HarvestProducts();
            this.CheckPets();
            if(this.count > 0 && this.HelperCost > 0)
            {
                int total = this.count * this.HelperCost;
                bool EnoughMoney = Player.Money >= total ? true : false;
                if (EnoughMoney)
                {
                    Player.Money = Math.Max(0, (Player.Money - total));
                    string msg = $"Dear {Player.name}^^ We came by while you were sleeping and took care of your farm for you. ^We did a total of {this.count} tasks for you at a cost of {total} Gold.^We also pet and watered your pet, if you have one. Free of charge.^^Sincerly MizzionInc";
                    this.count = 0;
                    Game1.activeClickableMenu = (IClickableMenu)new LetterViewerMenu(msg);
                }
            }
        }

        //Harvest Truffles
        private void HarvestTruffles()
        {
            Farm pFarm = Game1.getFarm();
            SFarmer Player = Game1.player;
            List<Vector2> truffles = new List<Vector2>();
            //Go through each truffle that needs to be removed.
            foreach(KeyValuePair<Vector2, SObject> pair in pFarm.Objects)
            {
                SObject @object = pair.Value;

                if (@object.name == "Truffle")
                {
                    bool doubleHarvest = false;

                    if (Player.professions.Contains(16))
                        @object.quality = 4;
                    double randomNum = Game1.random.NextDouble();
                    bool doubleChance = randomNum < 0.2 ? true : false;

                    if (Player.professions.Contains(13) && doubleChance)
                    {
                        @object.Stack = 2;
                        doubleHarvest = true;
                    }

                    if(this.AddToPlayer(@object, Player))
                    {
                        truffles.Add(pair.Key);
                        Player.gainExperience(2, 7);

                        if (doubleHarvest)
                        {
                            Player.gainExperience(2, 7);
                        }
                    }
                }
            }

            //Now we remove them
            foreach(Vector2 iLocation in truffles)
            {
                pFarm.removeObject(iLocation, false);
            }
        }
       
        //Harvest Coops
        private void HarvestProducts()
        {
            Farm pFarm = Game1.getFarm();
            SFarmer Player = Game1.player;

            foreach(Building building in pFarm.buildings)
            {
                if(building is Coop)
                {
                    List<Vector2> coops = new List<Vector2>();

                    foreach(KeyValuePair<Vector2, SObject> pair in building.indoors.Objects)
                    {
                        SObject @object = pair.Value;
                        if(@object.isAnimalProduct() || @object.parentSheetIndex == 107)
                        {
                            if(this.AddToPlayer(@object, Player))
                            {
                                coops.Add(pair.Key);
                                Player.gainExperience(0, 5);
                            }
                        }
                    }
                    foreach(Vector2 iLocation in coops)
                    {
                        building.indoors.removeObject(iLocation, false);
                    }
                }
            }
        }

        //Check for pets
        private void CheckPets()
        {
            Farm pFarm = Game1.getFarm();
            SFarmer Player = Game1.player;
            GameLocation location = Game1.currentLocation;
            if (Player.hasPet())
            {
                foreach (NPC character in Game1.getFarm().characters)
                {
                    if (character is Pet)
                    {
                        this.Helper.Reflection.GetField<bool>(character, "wasPetToday").SetValue(true);
                    }
                }
                foreach (NPC character in Utility.getHomeOfFarmer(Player).characters)
                {
                    if (character is Pet)
                    {
                        this.Helper.Reflection.GetField<bool>(character, "wasPetToday").SetValue(true);
                    }
                }
                pFarm.setMapTileIndex(54, 7, 1939, "Buildings", 0);
            }
        }

        //Harvest Crops
        private void HarvestCrops()
        {

        }
        //Add Item to Player
        private bool AddToPlayer(SObject @object, SFarmer Player)
        {
            if (this.AddItemsToInventory)
            {
                if (Player.couldInventoryAcceptThisItem(@object))
                {
                    Player.addItemToInventory(@object);
                    return true;
                }
            }
            //Need to add for chest.
            return false;
        }

        //HarvestedCrops
        private SObject HarvestedCrop(HoeDirt dirt, Crop crop, int x, int y)
        {
            SFarmer Player = Game1.player;
            int stack = 1;
            int iQuality = 0;
            int fBuff = 0;
            Random rnd = new Random(x * 7 + y +11 + (int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame);

            switch (dirt.fertilizer)
            {
                case 368:
                    fBuff = 1;
                    break;
                case 369:
                    fBuff = 2;
                    break;
            }
            double qMod = 0.2 * (Player.FarmingLevel / 10) + 0.2 * fBuff * ((Player.FarmingLevel + 2) / 12) + 0.01;
            double qModifier = Math.Min(0.75, qMod * 2.0);
            if (rnd.NextDouble() < qMod)
                iQuality = 1;
            if (crop.minHarvest > 1 || crop.maxHarvest > 1)
                stack = rnd.Next(crop.minHarvest, Math.Min(crop.minHarvest + 1, crop.maxHarvest + 1 + Player.FarmingLevel / crop.maxHarvestIncreasePerFarmingLevel));
            if(crop.chanceForExtraCrops > 0.0)
            {
                while (rnd.NextDouble() < Math.Min(0.9, crop.chanceForExtraCrops))
                    stack++;
            }
            if (rnd.NextDouble() < Player.luckLevel / 1500.0 + Game1.dailyLuck / 1200.0 + 9.99999974737875E-05)
                stack *= 2;
            if(crop.indexOfHarvest == 421)
            {
                crop.indexOfHarvest = 431;
                stack = rnd.Next(1, 4);
            }
            SObject @item = new SObject(crop.indexOfHarvest, stack, false, -1, iQuality);
            return item;

        }
        //Get a list of animals
        private List<FarmAnimal> getAnimals()
        {
            List<FarmAnimal> farmAnimals = Game1.getFarm().animals.Values.ToList();
            foreach(Building fBuilding in Game1.getFarm().buildings)
            {
                if (fBuilding.indoors != null && fBuilding.indoors.GetType() == typeof(AnimalHouse))
                    farmAnimals.AddRange(((AnimalHouse)fBuilding.indoors).animals.Values.ToList());
            }
            return farmAnimals;
        }
        //Get all locations
        public static IEnumerable<GameLocation> GetAllLocations()
        {
            foreach(GameLocation cLocation in Game1.locations)
            {
                yield return cLocation;

                if(cLocation is BuildableGameLocation bLocation)
                {
                    foreach (Building cBuilding in bLocation.buildings)
                    {
                        if (cBuilding.indoors != null)
                            yield return cBuilding.indoors;
                    }
                }
            }
        }
    }
}
