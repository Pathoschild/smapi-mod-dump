using System;
using System.IO;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace CustomFarmTypes
{
    public class Mod : StardewModdingAPI.Mod
    {
        public const int MY_FARM_TYPE = 6;

        public static Mod instance;
        public static SaveData Data = new SaveData();

        public override void Entry(IModHelper helper)
        {
            instance = this;
            
            SaveEvents.AfterLoad += afterLoad;
            SaveEvents.BeforeCreate += beforeSave;
            SaveEvents.AfterCreate += afterSave;
            SaveEvents.BeforeSave += beforeSave;
            SaveEvents.AfterSave += afterSave;
            GameEvents.UpdateTick += onUpdate;
            try
            {
                TypeFixes.fix();
            }
            catch ( Exception e )
            {
                Log.error("EXCEPTION FIXING FARM TYPES: " + e);
            }

            /*
            var fish = FarmType.getFarmBehaviorFromPreset(Farm.riverlands_layout);
            var forage = FarmType.getFarmBehaviorFromPreset(Farm.forest_layout);
            var mine = FarmType.getFarmBehaviorFromPreset(Farm.mountains_layout);
            var fight = FarmType.getFarmBehaviorFromPreset(Farm.combat_layout);

            FarmType.FarmBehavior b = new FarmType.FarmBehavior();
            b.FishingSplashChance = fish.FishingSplashChance;
            b.FishPools = fish.FishPools;
            b.FishPoolToDrawFrom = fish.FishPoolToDrawFrom;
            b.FishPools[0].Insert(0, forage.FishPools[0][0]);
            b.RepopulateStumps = forage.RepopulateStumps;
            b.SpecialWeedCount = forage.SpecialWeedCount;
            b.ForageableSpawnBehavior = forage.ForageableSpawnBehavior;
            b.ForageableSpawnChanceBase = forage.ForageableSpawnChanceBase;
            b.ForageableSpawnChanceMultiplier = forage.ForageableSpawnChanceMultiplier;
            b.NewSaveOreGenRuns = mine.NewSaveOreGenRuns;
            b.OreSpawnBehavior = mine.OreSpawnBehavior;
            b.OreSpawnBehavior[0].Area = new MyRectangle(52, 50, 11, 11);
            b.OreSpawnChanceBase = mine.OreSpawnChanceBase;
            b.OreSpawnChanceMultiplier = mine.OreSpawnChanceMultiplier;
            b.SpawnMonsters = fight.SpawnMonsters;

            FarmType.FarmhouseContents c = new FarmType.FarmhouseContents();
            c.WallpaperID = 3;
            c.FlooringID = 40;
            c.Furniture.Add(new FarmType.FarmhouseContents.FurniturePiece(1130, 1, 6, 0, 1368));
            c.Furniture.Add(new FarmType.FarmhouseContents.FurniturePiece(91, 1, 5, 0));
            c.TV.FurnitureID = 1468;
            c.TV.Position = new Vector2( 5, 4 );
            c.Giftbox.Position = new Vector2(4, 10);
            c.Giftbox.Contents.Add(new FarmType.FarmhouseContents.GiftboxData.Entry(94, 5));

            FarmType o = new FarmType();
            o.Name = "Everything";
            o.Description = "A farm incorporating features from every type, located in the forest.";
            o.ID = "spacechase0.FarmType.Everything";
            o.BehaviorPreset = 0;
            o.Behavior = b;
            o.FarmhousePreset = 0;
            o.Farmhouse = c;

            Helper.WriteJsonFile(Path.Combine(Helper.DirectoryPath, "everything.json"), o);
            //*/

            compileChoices();
        }

        private void afterLoad(object sender, EventArgs args)
        {
            if (!(Game1.year == 1 && Game1.currentSeason == "spring" && Game1.dayOfMonth == 0))
            {
                Log.debug("Loading save data... " + Game1.year + " " + Game1.currentSeason + " " + Game1.dayOfMonth);
                Data = Helper.ReadJsonFile<SaveData>(Path.Combine(Constants.CurrentSavePath, "custom-farm-type.json"));
                if (Data == null)
                {
                    Data = new SaveData();
                    return;
                }
            }

            foreach ( var entry in Data.FarmTypes )
            {
                GameLocation loc = Game1.getLocationFromName( entry.Key );
                if ( loc.GetType() != typeof( Farm ) )
                    continue;

                Log.info("Custom farm type " + entry.Value + " for " + entry.Key);
                FarmType type = FarmType.getType(entry.Value);
                if (type == null)
                {
                    Log.error("Custom farm type is missing!");
                    return;
                }

                var newFarm = new CustomFarm(type, loc.name);
                if (Game1.year == 1 && Game1.currentSeason == "spring" && Game1.dayOfMonth == 1)
                {
                    Log.debug("First day? from load");
                    doFirstDayStuff(newFarm, type);
                }
                else CustomFarm.swapFarms(loc as Farm, newFarm);
                Game1.locations.Remove(loc);
                Game1.locations.Add(newFarm);

                Game1.setGraphicsForSeason();
            }
        }

        private void beforeSave(object sender, EventArgs args)
        {
            Log.info("Before save, use vanilla farms...");

            foreach (var entry in Data.FarmTypes)
            {
                GameLocation loc = Game1.getLocationFromName(entry.Key);
                if (loc.GetType() == typeof(CustomFarm))
                {
                    Log.debug("Putting vanilla farm over " + entry.Key + "=" + entry.Value);
                    var farm = loc as CustomFarm;
                    Farm newFarm = new Farm(Helper.Content.Load<xTile.Map>("Maps\\Farm", ContentSource.GameContent), loc.name);
                    CustomFarm.swapFarms(farm, newFarm);
                    Game1.locations.Remove(farm);
                    Game1.locations.Add(newFarm);
                }
            }
        }

        private void afterSave(object sender, EventArgs args)
        {
            Log.info("Saving, putting custom farms back...");
            Helper.WriteJsonFile(Path.Combine(Constants.CurrentSavePath, "custom-farm-type.json"), Data);
            foreach (var entry in Data.FarmTypes)
            {
                GameLocation loc = Game1.getLocationFromName(entry.Key);
                if (loc.GetType() != typeof(Farm))
                    continue;

                Log.debug("Putting custom farm back for " + entry.Key + "=" + entry.Value);
                FarmType type = FarmType.getType(entry.Value);
                if (type == null)
                {
                    Log.error("Custom farm type is missing!");
                    return;
                }

                var newFarm = new CustomFarm(type, loc.name);
                if ( Game1.year == 1 && Game1.currentSeason == "spring" && Game1.dayOfMonth == 1 )
                {
                    Log.debug("First day? from save");
                    doFirstDayStuff(newFarm, type);
                }
                else CustomFarm.swapFarms(loc as Farm, newFarm);
                Game1.locations.Remove(loc);
                Game1.locations.Add(newFarm);
            }
        }

        private IClickableMenu prevMenu = null;
        private void onUpdate(object sender, EventArgs args)
        {
            if (Game1.activeClickableMenu is TitleMenu)
            {
                if (TitleMenu.subMenu is CharacterCustomization)
                {
                    Log.debug("Found vanilla new game window, replacing with our own.");

                    var oldMenu = (CharacterCustomization)TitleMenu.subMenu;
                    var shirts = Helper.Reflection.GetField<List<int>>(oldMenu, "shirtOptions").GetValue();
                    var hairs = Helper.Reflection.GetField<List<int>>(oldMenu, "hairStyleOptions").GetValue();
                    var accessories = Helper.Reflection.GetField<List<int>>(oldMenu, "accessoryOptions").GetValue();
                    var wizard = Helper.Reflection.GetField<bool>(oldMenu, "wizardSource").GetValue();
                    var newMenu = new NewCharacterCustomizeMenu(shirts, hairs, accessories, wizard);

                    TitleMenu.subMenu = newMenu;
                }
            }
            prevMenu = Game1.activeClickableMenu;
        }

        private void compileChoices()
        {
            Log.info("Creating list of custom farm types...");
            var choices = Directory.GetDirectories(Path.Combine(Helper.DirectoryPath, "FarmTypes"));
            foreach (var choice in choices)
            {
                if (!File.Exists(Path.Combine(choice, "type.json")) || !File.Exists(Path.Combine(choice, "map.xnb")) || !File.Exists(Path.Combine(choice, "icon.png")) )
                {
                    Log.error("A required file is missing for custom farm type \"" + choice + "\".");
                    continue;
                }

                FarmType type = Helper.ReadJsonFile<FarmType>(Path.Combine(choice, "type.json"));
                if ( type == null )
                {
                    Log.error("Problem reading type.json for custom farm type \"" + choice + "\".");
                    continue;
                }

                type.Folder = Path.Combine("FarmTypes", Path.GetFileName(choice));
                FarmType.register(type);
                Log.info("\tFarm type: " + type.Name + " (" + type.ID + ")");
            }
        }

        private void doFirstDayStuff( CustomFarm newFarm, FarmType type )
        {
            for (int index = 0; index < type.Behavior.NewSaveOreGenRuns; ++index)
                newFarm.doOreSpawns();

            var house = (FarmHouse)Game1.getLocationFromName("FarmHouse");
            house.furniture.Clear();
            house.objects.Clear();
            if (type.Farmhouse.WallpaperID != -1)
            {
                Log.debug("Wallpaper: " + type.Farmhouse.WallpaperID);
                house.setWallpaper(type.Farmhouse.WallpaperID, -1, true);
            }
            if (type.Farmhouse.FlooringID != -1)
            {
                Log.debug("Flooring: " + type.Farmhouse.FlooringID);
                house.setFloor(type.Farmhouse.FlooringID, -1, true);
            }
            foreach (var fp in type.Farmhouse.Furniture)
            {
                var furn = new Furniture(fp.FurnitureID, fp.Position, fp.Rotations);
                if (fp.HeldFurnitureID != -1)
                    furn.heldObject = new Furniture(fp.HeldFurnitureID, fp.Position);
                Log.debug("Furniture: " + fp.FurnitureID + "(" + fp.HeldFurnitureID + ") @ " + fp.Position + " " + fp.Rotations );
                house.furniture.Add(furn);
            }
            if (type.Farmhouse.TV.FurnitureID != -1)
            {
                Log.debug("TV: " + type.Farmhouse.TV.FurnitureID + " " + type.Farmhouse.TV.Position);
                house.furniture.Add(new TV(type.Farmhouse.TV.FurnitureID, type.Farmhouse.TV.Position));
            }
            if (type.Farmhouse.Giftbox.Contents.Count > 0)
            {
                List<Item> items = new List<Item>();
                foreach (var e in type.Farmhouse.Giftbox.Contents)
                {
                    Log.debug("Giftbox item: " + e.ObjectID + " x " + e.Amount);
                    items.Add(new SObject(e.ObjectID, e.Amount, false, -1, 0));
                }
                Log.debug("Giftbox position: " + type.Farmhouse.Giftbox.Position);
                var giftbox = new Chest(0, items, type.Farmhouse.Giftbox.Position, items.Count == 1);
                house.objects.Add(type.Farmhouse.Giftbox.Position, giftbox);
            }
        }
    }
}
