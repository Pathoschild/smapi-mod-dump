using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using xTile;
using Newtonsoft.Json;
using SObject = StardewValley.Object;
using Microsoft.Xna.Framework.Graphics;

namespace CustomFarmTypes
{
    public class FarmType
    {
        public class FarmBehavior
        {
            public class SpawnBehaviorArea
            {
                public double Chance { get; set; } = 1.0;
                public MyRectangle Area { get; set; }
                public List<SpawnBehaviorEntry> Entries { get; set; } = new List<SpawnBehaviorEntry>();

                public SpawnBehaviorArea() { }
                public SpawnBehaviorArea(MyRectangle area)
                {
                    Area = area;
                }
            }

            // This work as sequential ifs. No elses
            public class SpawnBehaviorEntry
            {
                public double Chance { get; set; } = 0;
                public double LuckFactor { get; set; } = 0;
                public bool SkipChanceDecrease { get; set; } = false;

                public bool OnlyTryIfNoneSelectedYet { get; set; } = false;

                // Option A: A single choice
                public int MiningLevelRequirement { get; set; } = -1;
                public int ObjectID { get; set; }
                public int InitialStack { get; set; } = 1;
                public int OreHealth { get; set; } = 0;

                // Option B: More behavior entries, except these act as if, else if.
                // The last option will always be chosen if the others aren't.
                public List<SpawnBehaviorEntry> SubEntries { get; set; }

                public SpawnBehaviorEntry()
                {
                }

                public SObject getObject()
                {
                    if (SubEntries != null)
                    {
                        foreach (var entry in SubEntries)
                        {
                            if (Game1.random.NextDouble() < entry.Chance)
                                return entry.getObject();
                        }
                        return SubEntries.Last().getObject();
                    }

                    return new SObject(new Vector2(), ObjectID, InitialStack) { minutesUntilReady = OreHealth };
                }

                public static SpawnBehaviorEntry Forageable(double chance, int obj)
                {
                    SpawnBehaviorEntry s = new SpawnBehaviorEntry();
                    s.Chance = chance;
                    s.ObjectID = obj;
                    return s;
                }

                public static SpawnBehaviorEntry Ore(double chance, int levelReq, int obj, int health)
                {
                    SpawnBehaviorEntry s = new SpawnBehaviorEntry();
                    s.Chance = chance;
                    s.MiningLevelRequirement = levelReq;
                    s.ObjectID = obj;
                    s.InitialStack = 10;
                    s.OreHealth = health;
                    return s;
                }

                public static SpawnBehaviorEntry chooseEntry(List<SpawnBehaviorEntry> entries)
                {
                    SpawnBehaviorEntry ret = null;
                    foreach (var entry in entries)
                    {
                        if (entry.OnlyTryIfNoneSelectedYet && ret != null)
                            continue;

                        if (Game1.player != null && Game1.player.miningLevel < entry.MiningLevelRequirement)
                            continue;

                        if (Game1.random.NextDouble() <= entry.Chance)
                            ret = entry;
                    }
                    return ret == null ? entries.Last() : ret;
                }
            }

            // Fishing
            public class FishPoolDrawEntry
            {
                public MyRectangle Area { get; set; }
                public int PoolId { get; set; } = 0;
                public int ListFromLocationID { get; set; } = -1;

                public FishPoolDrawEntry() { }
                public FishPoolDrawEntry(MyRectangle area, int id, int list)
                {
                    Area = area;
                    PoolId = id;
                    ListFromLocationID = list;
                }

            }
            public class FishPoolEntry
            {
                public double Chance { get; set; }
                public double LuckFactor { get; set; } = 0;

                public bool OnlyTryIfNoneSelectedYet { get; set; } = false;

                // Option A: Just use the fish type from another location
                public string LocationPreset { get; set; }

                // Option B: A specific object
                public int ObjectID { get; set; }

                public static FishPoolEntry FinalPreset(string loc)
                {
                    FishPoolEntry fish = new FishPoolEntry();
                    fish.Chance = 1.0;
                    fish.OnlyTryIfNoneSelectedYet = true;
                    fish.LocationPreset = loc;
                    return fish;
                }

                public static FishPoolEntry Preset(double chance, string loc, double luck = 0)
                {
                    FishPoolEntry fish = new FishPoolEntry();
                    fish.Chance = chance;
                    fish.LuckFactor = luck;
                    fish.LocationPreset = loc;
                    return fish;
                }

                public static FishPoolEntry FinalObject(int obj)
                {
                    FishPoolEntry fish = new FishPoolEntry();
                    fish.Chance = 1.0;
                    fish.OnlyTryIfNoneSelectedYet = true;
                    fish.ObjectID = obj;
                    return fish;
                }

                public static FishPoolEntry Object(double chance, int obj, double luck = 0)
                {
                    FishPoolEntry fish = new FishPoolEntry();
                    fish.Chance = chance;
                    fish.LuckFactor = luck;
                    fish.ObjectID = obj;
                    return fish;
                }

                public SObject getObject()
                {
                    return new SObject(ObjectID, 1);
                }

                public static FishPoolEntry chooseEntry(List<FishPoolEntry> entries)
                {
                    FishPoolEntry ret = null;
                    foreach (var entry in entries)
                    {
                        if (entry.OnlyTryIfNoneSelectedYet && ret != null)
                            continue;

                        if (Game1.random.NextDouble() < entry.Chance + Game1.dailyLuck * entry.LuckFactor)
                            ret = entry;
                    }
                    return ret;
                }
            }
            public double FishingSplashChance { get; set; }
            public List<FishPoolDrawEntry> FishPoolToDrawFrom;
            public List<List<FishPoolEntry>> FishPools;

            // Foraging
            public bool RepopulateStumps { get; set; }
            public int SpecialWeedCount { get; set; }
            public double ForageableSpawnChanceBase { get; set; }
            public double ForageableSpawnChanceMultiplier { get; set; }
            public Dictionary<string, List<SpawnBehaviorArea>> ForageableSpawnBehavior { get; set; }

            // Mining
            public int NewSaveOreGenRuns { get; set; }
            public double OreSpawnChanceBase { get; set; }
            public double OreSpawnChanceMultiplier { get; set; }
            public List<SpawnBehaviorArea> OreSpawnBehavior { get; set; }

            // Combat
            public bool SpawnMonsters { get; set; }

            // ...
            public static SpawnBehaviorArea chooseSpawnArea(List<SpawnBehaviorArea> areas)
            {
                foreach (var area in areas)
                {
                    if (Game1.random.NextDouble() <= area.Chance)
                        return area;
                }
                return areas.Last();
            }
        }
        
        public class FarmhouseContents
        {
            public int WallpaperID { get; set; } = -1;
            public int FlooringID { get; set; } = -1;
            public class FurniturePiece
            {
                public int FurnitureID { get; set; }
                public Vector2 Position { get; set; }
                public int Rotations { get; set; }
                public int HeldFurnitureID { get; set; } = -1;

                public FurniturePiece() { }
                public FurniturePiece(int id, int x, int y, int rot = 0, int held = -1)
                {
                    FurnitureID = id;
                    Position = new Vector2(x, y);
                    Rotations = 0;
                    if (held != -1)
                        HeldFurnitureID = held;
                }
            }
            public List<FurniturePiece> Furniture { get; set; } = new List<FurniturePiece>();
            public class TVData
            {
                public int FurnitureID { get; set; } = 1466;
                public Vector2 Position { get; set; } = new Vector2(1, 4);
            }
            public TVData TV { get; set; } = new TVData();
            public class GiftboxData
            {
                public class Entry
                {
                    public int ObjectID;
                    public int Amount;

                    public Entry() { }
                    public Entry(int id, int amt)
                    {
                        ObjectID = id;
                        Amount = amt;
                    }
                }

                public Vector2 Position { get; set; } = new Vector2(3, 7);
                public List<Entry> Contents { get; set; } = new List<Entry>(new Entry[] { new Entry(472, 15) });
            }
            public GiftboxData Giftbox { get; set; } = new GiftboxData();
        }

        [JsonIgnore]
        public string Folder { get; set; }

        public string Name { get; set; }
        public string Description { get; set; } = "";
        public string ID { get; set; }

        [JsonIgnore]
        public virtual Texture2D Icon
        {
            get
            {
                return Mod.instance.Helper.Content.Load<Texture2D>(Folder + "/icon.png", ContentSource.ModFolder);
            }
        }

        // Only valid for farms using CustomFarm
        public int BehaviorPreset { get; set; } = Farm.default_layout;
        public FarmBehavior Behavior
        {
            get
            {
                if (behavior_ == null)
                    behavior_ = getFarmBehaviorFromPreset(BehaviorPreset);
                return behavior_;
            }
            set { behavior_ = value; }
        }
        private FarmBehavior behavior_;

        public int FarmhousePreset { get; set; } = Farm.default_layout;
        public FarmhouseContents Farmhouse
        {
            get
            {
                if (farmhouse_ == null)
                    farmhouse_ = getFarmhouseFromPreset(FarmhousePreset);
                return farmhouse_;
            }
            set { farmhouse_ = value; }
        }
        private FarmhouseContents farmhouse_;

        public virtual Map loadMap()
        {
            return Mod.instance.Helper.Content.Load<Map>( Folder + "/map.xnb", ContentSource.ModFolder);
        }

        public virtual Farm getFarm( string loc )
        {
            return new CustomFarm(this, loc);
        }

        public static FarmType getFarmTypeFromPreset( int vanillaId )
        {
            FarmType f = new FarmType();
            f.Name = Farm.getMapNameFromTypeInt(vanillaId);
            f.ID = "StardewValley." + f.Name;
            f.BehaviorPreset = vanillaId;
            f.FarmhousePreset = vanillaId;

            return f;
        }

        public static FarmBehavior getFarmBehaviorFromPreset(int vanillaId)
        {
            MyRectangle mapRect = new MyRectangle(0, 0, 80, 65);

            FarmBehavior b = new FarmBehavior();
            switch (vanillaId)
            {
                case Farm.default_layout:
                    break;
                case Farm.riverlands_layout:
                    {
                        b.FishingSplashChance = 0.5;
                        b.FishPoolToDrawFrom = new List<FarmBehavior.FishPoolDrawEntry>();
                        b.FishPoolToDrawFrom.Add(new FarmBehavior.FishPoolDrawEntry(mapRect, 0, 1));
                        var fish = new List<FarmBehavior.FishPoolEntry>();
                        fish.Add(FarmBehavior.FishPoolEntry.Preset(0.3, "Forest"));
                        fish.Add(FarmBehavior.FishPoolEntry.FinalPreset("Town"));
                        b.FishPools = new List<List<FarmBehavior.FishPoolEntry>>();
                        b.FishPools.Add(fish);
                    }
                    break;
                case Farm.forest_layout:
                    {
                        b.FishPoolToDrawFrom = new List<FarmBehavior.FishPoolDrawEntry>();
                        b.FishPoolToDrawFrom.Add(new FarmBehavior.FishPoolDrawEntry(mapRect, 0, 1));
                        var fish = new List<FarmBehavior.FishPoolEntry>();
                        fish.Add(FarmBehavior.FishPoolEntry.Object(0.5, 734, 1.0));
                        fish.Add(FarmBehavior.FishPoolEntry.Preset(0.5, "Forest"));
                        b.FishPools = new List<List<FarmBehavior.FishPoolEntry>>();
                        b.FishPools.Add(fish);

                        b.RepopulateStumps = true;
                        b.SpecialWeedCount = 6;

                        b.ForageableSpawnChanceBase = 0.75;
                        b.ForageableSpawnChanceMultiplier = 1.0;
                        b.ForageableSpawnBehavior = new Dictionary<string, List<FarmBehavior.SpawnBehaviorArea>>();

                        var spring = new FarmBehavior.SpawnBehaviorArea(mapRect);
                        spring.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.25, 16));
                        spring.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.25, 22));
                        spring.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.25, 20));
                        spring.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.00, 257)); // The last one is always chosen if none of the others are. No need to give it an extra 25% chance as well.
                        var springAreas = new List<FarmBehavior.SpawnBehaviorArea>();
                        springAreas.Add(spring);
                        b.ForageableSpawnBehavior.Add("spring", springAreas);

                        var summer = new FarmBehavior.SpawnBehaviorArea(mapRect);
                        summer.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.25, 402));
                        summer.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.25, 396));
                        summer.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.25, 398));
                        summer.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.00, 404));
                        var summerAreas = new List<FarmBehavior.SpawnBehaviorArea>();
                        summerAreas.Add(summer);
                        b.ForageableSpawnBehavior.Add("summer", summerAreas);

                        var fall = new FarmBehavior.SpawnBehaviorArea(mapRect);
                        fall.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.25, 281));
                        fall.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.25, 420));
                        fall.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.25, 422));
                        fall.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Forageable(0.00, 404));
                        var fallAreas = new List<FarmBehavior.SpawnBehaviorArea>();
                        fallAreas.Add(fall);
                        b.ForageableSpawnBehavior.Add("fall", fallAreas);
                    }
                    break;
                case Farm.mountains_layout:
                    {
                        b.FishPoolToDrawFrom = new List<FarmBehavior.FishPoolDrawEntry>();
                        b.FishPoolToDrawFrom.Add(new FarmBehavior.FishPoolDrawEntry(mapRect, 0, 1));
                        var fish = new List<FarmBehavior.FishPoolEntry>();
                        fish.Add(FarmBehavior.FishPoolEntry.Preset(0.5, "Forest"));
                        b.FishPools = new List<List<FarmBehavior.FishPoolEntry>>();
                        b.FishPools.Add(fish);

                        b.OreSpawnChanceBase = 1;
                        b.OreSpawnChanceMultiplier = 0.66;
                        b.NewSaveOreGenRuns = 28;

                        var chances = new FarmBehavior.SpawnBehaviorArea(new MyRectangle(5, 37, 22, 8));
                        var firstChance = FarmBehavior.SpawnBehaviorEntry.Ore(.15, -1, 590, 0);
                        firstChance.SkipChanceDecrease = true;
                        firstChance.InitialStack = 1;
                        chances.Entries.Add(firstChance);
                        chances.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Ore(.5, -1, 670, 0));
                        var multiChance = new FarmBehavior.SpawnBehaviorEntry();
                        multiChance.Chance = .1;
                        multiChance.SubEntries = new List<FarmBehavior.SpawnBehaviorEntry>();
                        multiChance.SubEntries.Add(FarmBehavior.SpawnBehaviorEntry.Ore(.33, 8, 77, 7));
                        multiChance.SubEntries.Add(FarmBehavior.SpawnBehaviorEntry.Ore(.5, 5, 76, 5));
                        multiChance.SubEntries.Add(FarmBehavior.SpawnBehaviorEntry.Ore(1, -1, 75, 3));
                        chances.Entries.Add(multiChance);
                        chances.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Ore(.21, -1, 751, 3));
                        chances.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Ore(.15, 4, 290, 4));
                        chances.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Ore(.1, 7, 764, 8));
                        chances.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Ore(.01, 10, 765, 16));
                        chances.Entries.Add(FarmBehavior.SpawnBehaviorEntry.Ore(1.0, -1, 668, 2));

                        b.OreSpawnBehavior = new List< FarmBehavior.SpawnBehaviorArea >();
                        b.OreSpawnBehavior.Add(chances);
                    }
                    break;
                case Farm.combat_layout:
                    {
                        b.FishPoolToDrawFrom = new List<FarmBehavior.FishPoolDrawEntry>();
                        b.FishPoolToDrawFrom.Add(new FarmBehavior.FishPoolDrawEntry(mapRect, 0, 0));
                        var fish = new List<FarmBehavior.FishPoolEntry>();
                        fish.Add(FarmBehavior.FishPoolEntry.Preset(0.35, "Mountain"));
                        b.FishPools = new List<List<FarmBehavior.FishPoolEntry>>();
                        b.FishPools.Add(fish);

                        b.SpawnMonsters = true;
                    }
                    break;
            }

            return b;
        }
        
        public static FarmhouseContents getFarmhouseFromPreset(int vanillaId)
        {
            FarmhouseContents c = new FarmhouseContents();
            switch (vanillaId)
            {
                case Farm.default_layout:
                    {
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1120, 5, 4, 0, 1364));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1376, 1, 10));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(0, 4, 10));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1614, 3, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1618, 6, 8));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1602, 5, 1));
                    }
                    break;
                case Farm.riverlands_layout:
                    {
                        c.WallpaperID = 11;
                        c.FlooringID = 1;
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1122, 1, 6, 0, 1367));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(3, 1, 5));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1673, 1, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1673, 3, 5));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1676, 5, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1737, 6, 8));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1742, 5, 5));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1675, 10, 1));
                        c.TV.FurnitureID = 1680;
                        c.TV.Position = new Vector2(5, 4);
                        c.Giftbox.Position = new Vector2(4, 7);
                    }
                    break;
                case Farm.forest_layout:
                    {
                        c.WallpaperID = 92;
                        c.FlooringID = 34;
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1134, 1, 7, 0, 1748));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(3, 1, 6));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1296, 1, 4));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1682, 3, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1777, 6, 5));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1745, 6, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1747, 5, 4));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1296, 10, 4));
                        c.TV.FurnitureID = 1680;
                        c.TV.Position = new Vector2(6, 4);
                        c.Giftbox.Position = new Vector2(4, 7);
                    }
                    break;
                case Farm.mountains_layout:
                    {
                        c.WallpaperID = 12;
                        c.FlooringID = 18;
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1218, 1, 6, 0, 1368));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1755, 1, 5));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1755, 3, 6, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1751, 5, 10));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1749, 3, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1753, 5, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1742, 5, 5));
                        c.TV.FurnitureID = 1680;
                        c.TV.Position = new Vector2(5, 4);
                        c.Giftbox.Position = new Vector2(2, 9);
                    }
                    break;
                case Farm.combat_layout:
                    {
                        c.WallpaperID = 12;
                        c.FlooringID = 18;
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1628, 1, 5));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1393, 1, 6, 0, 1369));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1678, 10, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1812, 3, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1630, 1, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1811, 6, 1));
                        c.Furniture.Add(new FarmhouseContents.FurniturePiece(1389, 10, 4));
                        c.TV.FurnitureID = 1680;
                        c.TV.Position = new Vector2(1, 4);
                        c.Giftbox.Position = new Vector2(4, 7);
                    }
                    break;
            }

            return c;
        }

        private static Dictionary<string, FarmType> types = new Dictionary<string, FarmType>();

        public static void register( FarmType type )
        {
            if ( types.ContainsKey( type.ID ) )
            {
                Log.error("Type \"" + type.ID + "\" already registered.");
                return;
            }
            types.Add(type.ID, type);
        }

        public static FarmType getType( string id )
        {
            return types.ContainsKey( id ) ? types[id] : null;
        }

        public static List< FarmType > getTypes()
        {
            return types.Values.ToList();
        }
    }
}
