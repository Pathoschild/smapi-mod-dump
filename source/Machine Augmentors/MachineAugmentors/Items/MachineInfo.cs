/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-MachineAugmentors
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Object = StardewValley.Object;

namespace MachineAugmentors.Items
{
    /*public enum MachineType
    {
        None,
        MayonnaiseMachine,
        BeeHouse,
        PreservesJar,
        CheesePress,
        Loom,
        Keg,
        OilMaker,
        Cask,
        CharcoalKiln,
        Crystalarium,
        Furnace,
        //  Lightning Rods are a bit weird - they don't require any inputs, but they also don't automatically get an output item after collecting the output
        //  so properly detecting when to modify it's Object.heldObject is different than for the other machines. 
        //  I guess we need to detect it and modify the heldObject during ObjectPatches.CheckForAction_Prefix instead of in the Postfix function?
        //  Whatever, I'll add support for it some other time if anybody cares enough to request it.
        //LightningRod,
        RecyclingMachine,
        SeedMaker,
        //  Who even uses these? What a waste of dev time lol
        //SlimeEggPress,
        Tapper,
        WormBin,
        //  Too lazy to test these
        //CrabPot,
        //  Not sure if this is a machine (might be a "Furniture" Object, which likely isn't a problem but whatever). If it is a regular Object, it probably works just like a Crystalarium. 
        //  Too lazy to test though. If you do add support for these, don't allow placing duplication augmentors on them - Would be way too overpowered
        //StatueOfEndlessFortune
    }*/

    public class MachineInfo
    {
        private static AugmentorType[] AllAugmentorTypes = Enum.GetValues(typeof(AugmentorType)).Cast<AugmentorType>().ToArray();
        private static ReadOnlyCollection<MachineInfo> BuiltInMachines = new List<MachineInfo>()
        {
            new MachineInfo("Mayonnaise Machine", 24, true, true, AllAugmentorTypes),
            new MachineInfo("Bee House", new List<int>() { 10, 11 }, false, false, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Duplication),
            new MachineInfo("Preserves Jar", 15, false, true, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Efficiency, AugmentorType.Production, AugmentorType.Duplication),
            new MachineInfo("Cheese Press", 16, true, true, AllAugmentorTypes),
            new MachineInfo("Loom", new List<int>() { 17, 18 }, false, true, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Efficiency, AugmentorType.Production, AugmentorType.Duplication),
            new MachineInfo("Keg", 12, false, true, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Efficiency, AugmentorType.Production, AugmentorType.Duplication),
            new MachineInfo("Oil Maker", 19, false, true, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Efficiency, AugmentorType.Production, AugmentorType.Duplication),
            new MachineInfo("Cask", 163, true, true, AllAugmentorTypes),
            new MachineInfo("Charcoal Kiln", new List<int>() { 114, 115 }, false, true, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Efficiency, AugmentorType.Production, AugmentorType.Duplication),
            new MachineInfo("Crystalarium", 21, false, false, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Duplication),
            new MachineInfo("Furnace", new List<int>() { 13, 14 }, false, true, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Efficiency, AugmentorType.Production, AugmentorType.Duplication, AugmentorType.Quality),
            new MachineInfo("Recycling Machine", 20, false, true, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Efficiency, AugmentorType.Production, AugmentorType.Duplication),
            new MachineInfo("Seed Maker", 25, false, true, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Efficiency, AugmentorType.Production, AugmentorType.Duplication),
            new MachineInfo("Tapper", 105, false, false, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Production, AugmentorType.Duplication),
            new MachineInfo("Worm Bin", 154, false, false, AugmentorType.Output, AugmentorType.Speed, AugmentorType.Production, AugmentorType.Duplication)
        }.AsReadOnly();

        static MachineInfo() { LoadAugmentableMachineData(); }
        internal static void LoadAugmentableMachineData()
        {
            IModRegistry ModRegistry = MachineAugmentorsMod.ModInstance.Helper.ModRegistry;
            IEnumerable<MachineInfo> CustomMachines = MachineAugmentorsMod.MachineConfig.Machines.Where(x => !string.IsNullOrEmpty(x.RequiredModUniqueId) && ModRegistry.IsLoaded(x.RequiredModUniqueId)).Select(x => x.ToMachineInfo());
            RegisteredMachines = new ReadOnlyCollection<MachineInfo>(BuiltInMachines.Union(CustomMachines).ToList());
        }
        internal static ReadOnlyCollection<MachineInfo> RegisteredMachines { get; private set; }

        public string Name { get; }

        /// <summary>The values of <see cref="Item.ParentSheetIndex"/> that are valid for this machine. I think there should only be one Id, but I wasn't sure.<para/>
        /// For example, furnaces have 2 sprites in TileSheets\Craftables.xnb, one at Index=13 (empty furnace) and one at Index=14 (full furnace).</summary>
        public ReadOnlyCollection<int> Ids { get; }
        public const int InvalidId = -1;

        public ReadOnlyCollection<AugmentorType> AttachableAugmentors { get; }

        /// <summary>True if this machine produces output items that can have different <see cref="StardewValley.Object.Quality"/> values.<para/>
        /// For example, this would be true for Mayonnaise machines, but false for Furnaces</summary>
        public bool HasQualityProducts { get; }

        /// <summary>True if the machine requires an input item for each cycle of processing.<para/>This would be false for things like Bee Hives or Tappers, and also for Crystalarium  
        /// (since it only needs an initial input, and then will continue producing forever)<para/>True for things like Furnaces.</summary>
        public bool RequiresInput { get; }

        public MachineInfo(string Name, int Id, bool HasQualityProducts, bool RequiresInput, params AugmentorType[] AttachableAugmentors)
            : this(Name, new List<int>() { Id }, HasQualityProducts, RequiresInput, AttachableAugmentors) { }

        public MachineInfo(string Name, IEnumerable<int> Ids, bool HasQualityProducts, bool RequiresInput, params AugmentorType[] AttachableAugmentors)
        {
            this.Name = Name;
            this.Ids = Ids.Where(x => x != InvalidId).ToList().AsReadOnly();
            this.HasQualityProducts = HasQualityProducts;
            this.RequiresInput = RequiresInput;

            List<AugmentorType> ValidatedTypes = AttachableAugmentors.Distinct().ToList();
            if (!HasQualityProducts && !IsFurnace())
                ValidatedTypes.Remove(AugmentorType.Quality);
            if (!RequiresInput)
            {
                ValidatedTypes.Remove(AugmentorType.Efficiency);
                //ValidatedTypes.Remove(AugmentorType.Production);
            }
            this.AttachableAugmentors = ValidatedTypes.AsReadOnly();
        }

        public bool IsFurnace()
        {
            return Ids.Contains(13) || Ids.Contains(14);
        }

        internal static bool IsPrismaticToolsModInstalled { get; set; } = false;
        private const int PrismaticBarId = 1112;

        public bool TryGetUpgradedQuality(Object Original, out Object Upgraded)
        {
            Upgraded = null;
            if (Original == null)
                return false;
            else if (IsFurnace() && Original.Quality == 0 && !Original.bigCraftable.Value && !Original.IsRecipe)
            {
                if (Original.ParentSheetIndex == 334) // Copper Bar -> Iron Bar
                    Upgraded = new Object("335", Original.Stack, false, -1, 0);
                else if (Original.ParentSheetIndex == 335) // Iron Bar -> Gold Bar
                    Upgraded = new Object("336", Original.Stack, false, -1, 0);
                else if (Original.ParentSheetIndex == 336) // Gold Bar -> Iridium Bar
                    Upgraded = new Object("337", Original.Stack, false, -1, 0);
                //else if (IsPrismaticToolsModInstalled && Original.ParentSheetIndex == 337) // Iridium Bar -> Prismatic Bar (from "Prismatic Tools" mod)
                //    Upgraded = new Object(PrismaticBarId, Original.Stack, false, -1, 0);
                return Upgraded != null;
            }
            else
                return false;
        }

        public bool IsMatch(Object Item)
        {
            return Item != null && Item.bigCraftable.Value && (Name == Item.Name || Ids.Contains(Item.ParentSheetIndex));
        }

        public static bool TryGetMachineInfo(Object Item, out MachineInfo Result)
        {
            Result = RegisteredMachines.FirstOrDefault(x => x.IsMatch(Item));
            return Result != null;
        }

        private static readonly ReadOnlyCollection<int> IndestructibleMachineIds = new List<int>() {
            101 // Incubator
        }.AsReadOnly();

        public static bool IsDestructible(Object Machine)
        {
            return Machine != null && Machine.bigCraftable.Value && Machine.CanBeSetDown && Machine.isPlaceable() && !IndestructibleMachineIds.Contains(Machine.ParentSheetIndex);
        }
    }
}
