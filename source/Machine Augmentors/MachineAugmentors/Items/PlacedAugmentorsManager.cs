/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-MachineAugmentors
**
*************************************************/

using Microsoft.Xna.Framework;
using Object = StardewValley.Object;
using static MachineAugmentors.Harmony.GamePatches;
using StardewValley;
using System.Collections.Generic;
using System;
using StardewModdingAPI;
using System.Linq;

namespace MachineAugmentors.Items
{
    /// <summary>
    /// The game does not allow placing multiple Objects on the same tile. So when the player attempts to place an <see cref="Augmentor"/>,<para/>
    /// the mod will remove it from their inventory and then micro-manage it in this data structure (which is saved/loaded using SMAPI's <see cref="IDataHelper.WriteSaveData{TModel}(string, TModel)"/>
    /// </summary>
    public class PlacedAugmentorsManager
    {
        public const string SavedDataKey = "PlacedAugmentors";
        public static PlacedAugmentorsManager Instance { get; internal set; }

        /// <summary>Key = Name of a <see cref="GameLocation"/>, Value = A data structure that keeps track of all <see cref="AugmentedTile"/>s within that <see cref="GameLocation"/></summary>
        public Dictionary<string, AugmentedLocation> Locations { get; private set; }

        public PlacedAugmentorsManager()
        {
            this.Locations = new Dictionary<string, AugmentedLocation>();

            try
            {
                if (!Context.IsMultiplayer || Context.IsMainPlayer)
                { 
                    SerializablePlacedAugmentors SavedData = MachineAugmentorsMod.ModInstance.Helper.Data.ReadSaveData<SerializablePlacedAugmentors>(SavedDataKey);
                    LoadSettings(SavedData);
                }
            }
            catch (Exception ex)
            {
                MachineAugmentorsMod.ModInstance.Monitor.Log("Error while loading augmentor data from ReadSaveData: " + ex, LogLevel.Error);
            }
        }

        internal void LoadSettings(SerializablePlacedAugmentors Data)
        {
            if (Data != null)
            {
                foreach (SerializableAugmentedLocation SavedLocation in Data.Locations)
                {
                    string LocationName = SavedLocation.UniqueLocationName;
                    GameLocation GL = Game1.getLocationFromName(LocationName);
                    if (GL == null)
                    {
                        MachineAugmentorsMod.ModInstance.Monitor.Log(string.Format("Warning - Could not find a GameLocation named '{0}'. Some of your augmentors may not have been loaded from your save file!", LocationName), LogLevel.Warn);
                    }
                    else
                    {
                        AugmentedLocation AL = new AugmentedLocation(this, LocationName);
                        Locations.Add(LocationName, AL);

                        foreach (SerializableAugmentedTile SavedTile in SavedLocation.Tiles)
                        {
                            if (!GL.Objects.TryGetValue(new Vector2(SavedTile.TileXPosition, SavedTile.TileYPosition), out Object Item))
                            {
                                string Warning = string.Format("Warning - GameLocation '{0}' does not have a machine at ({1},{2}). Some of your augmentors may not have been loaded from your save file!",
                                    LocationName, SavedTile.TileXPosition, SavedTile.TileYPosition);
                                MachineAugmentorsMod.ModInstance.Monitor.Log(Warning, LogLevel.Warn);
                            }
                            else
                            {
                                AugmentedTile AT = new AugmentedTile(AL, SavedTile.TileXPosition, SavedTile.TileYPosition);
                                AL.Tiles.Add(AugmentedLocation.EncodeTileToString(SavedTile.TileXPosition, SavedTile.TileYPosition), AT);

                                for (int i = 0; i < SavedTile.AugmentorTypes.Length; i++)
                                {
                                    AugmentorType Type = SavedTile.AugmentorTypes[i];
                                    int Quantity = SavedTile.AugmentorQuantities[i];
                                    AT.Quantities.Add(Type, Quantity);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the KeyValuePairs in the order that their Augmentors should be handled.<para/>
        /// The order that they are handled can affect the outcome since not all modifications made to Machine properties are transitive.<para/>
        /// For example, if one augmentor applied an offset to a value, and another augmentor applied a multiplier, the order that they apply their changes in will affect the outcome.<para/>
        /// (X+Offset)*Multiplier != (X*Multiplier)+Offset
        /// </summary>
        internal static IOrderedEnumerable<KeyValuePair<AugmentorType, int>> GetOrderedEnumerable(Dictionary<AugmentorType, int> PlacedQuantities)
        {
            //  Processing order dependencies:
            //  1. SpeedAugmentors must be processed before DuplicationAugmentors, since the chance of duplication depends on MinutesUntilReady, which is modified by SpeedAugmentors.
            //  If we process Duplication first, then you'd have a higher chance of duplication to occur, even if the MinutesUntilReady got reduced to a small value.
            //  2. ProductionAugmentors must be processed before EfficiencyAugmentors, so that the efficiency affects the entire stack after it's been increased by production.
            //      EX: If 50% efficiency and x10 production, we want to save 50% of x10, rather than 50% of x1
            //  3. QualityAugmentors must be processed last, since they may completely change the Output Item.
            IOrderedEnumerable<KeyValuePair<AugmentorType, int>> OrderedPairs = PlacedQuantities.OrderBy(x => 
            {
                if (x.Key == AugmentorType.Quality)
                    return int.MaxValue;
                else if (x.Key == AugmentorType.Efficiency)
                    return int.MaxValue - 1;
                else
                    return (int)x.Key;
                //return x.Key == AugmentorType.Quality ? int.MaxValue : (int)x.Key;
            });
            return OrderedPairs;
        }

        internal void OnProductsCollected(CheckForActionData CFAData)
        {
            if (!CFAData.IsLocalPlayer)
                return;

            if (CFAData.CurrentHeldObject != null)
            {
                //  This point of the code should be reached when collecting from a machine that does not require inputs
                //  such as when collecting from a Bee Hive / Crystalarium / Tapper, a new product is automatically placed back into the machine and the MinutesUntilReady gets reset
                if (TryFindAugmentedTile(CFAData.Machine, CFAData.Machine.TileLocation, out AugmentedTile AT))
                {
                    foreach (KeyValuePair<AugmentorType, int> KVP in GetOrderedEnumerable(AT.Quantities))
                    {
                        AugmentorType Type = KVP.Key;
                        int AttachedQuantity = KVP.Value;

                        if (AttachedQuantity > 0)
                        {
                            if (Type == AugmentorType.Output)
                                OutputAugmentor.OnProductsCollected(CFAData, AttachedQuantity);
                            else if (Type == AugmentorType.Speed)
                                SpeedAugmentor.OnProductsCollected(CFAData, AttachedQuantity);
                            else if (Type == AugmentorType.Efficiency)
                                EfficiencyAugmentor.OnProductsCollected(CFAData, AttachedQuantity);
                            else if (Type == AugmentorType.Quality)
                                QualityAugmentor.OnProductsCollected(CFAData, AttachedQuantity);
                            else if (Type == AugmentorType.Production)
                                ProductionAugmentor.OnProductsCollected(CFAData, AttachedQuantity);
                            else if (Type == AugmentorType.Duplication)
                                DuplicationAugmentor.OnProductsCollected(CFAData, AttachedQuantity);
                            else
                                throw new NotImplementedException(string.Format("Unrecognized AugmentorType: {0}", Type.ToString()));
                        }
                    }
                }
            }
        }

        internal void OnInputsInserted(PerformObjectDropInData PODIData)
        {
            if (!PODIData.IsLocalPlayer)
                return;

            //  This point of the code should be reached when placing an input item into a machine that requires inputs,
            //  such as when placing Copper Ore into a furnace
            if (TryFindAugmentedTile(PODIData.Machine, PODIData.Machine.TileLocation, out AugmentedTile AT))
            {
                foreach (KeyValuePair<AugmentorType, int> KVP in GetOrderedEnumerable(AT.Quantities))
                {
                    AugmentorType Type = KVP.Key;
                    int AttachedQuantity = KVP.Value;

                    if (AttachedQuantity > 0)
                    {
                        if (Type == AugmentorType.Output)
                            OutputAugmentor.OnInputsInserted(PODIData, AttachedQuantity);
                        else if (Type == AugmentorType.Speed)
                            SpeedAugmentor.OnInputsInserted(PODIData, AttachedQuantity);
                        else if (Type == AugmentorType.Efficiency)
                            EfficiencyAugmentor.OnInputsInserted(PODIData, AttachedQuantity);
                        else if (Type == AugmentorType.Quality)
                            QualityAugmentor.OnInputsInserted(PODIData, AttachedQuantity);
                        else if (Type == AugmentorType.Production)
                            ProductionAugmentor.OnInputsInserted(PODIData, AttachedQuantity);
                        else if (Type == AugmentorType.Duplication)
                            DuplicationAugmentor.OnInputsInserted(PODIData, AttachedQuantity);
                        else
                            throw new NotImplementedException(string.Format("Unrecognized AugmentorType: {0}", Type.ToString()));
                    }
                }
            }
        }

        internal void Update()
        {
            foreach (AugmentedLocation Location in Locations.Values)
                Location.Update();
        }

        public void OnAugmentorPlaced(string UniqueLocationName, AugmentorType Type, int Qty, bool RemoveFromInventory, Vector2 Tile, Augmentor Instance = null) { OnAugmentorPlaced(UniqueLocationName, Type, Qty, RemoveFromInventory, (int)Tile.X, (int)Tile.Y, Instance); }
        public void OnAugmentorPlaced(string UniqueLocationName, AugmentorType Type, int Qty, bool RemoveFromInventory, int TileX, int TileY, Augmentor Instance = null)
        {
            if (!Locations.TryGetValue(UniqueLocationName, out AugmentedLocation Location))
            {
                Location = new AugmentedLocation(this, UniqueLocationName);
                Locations.Add(UniqueLocationName, Location);
            }

            Location.OnAugmentorPlaced(Type, Qty, RemoveFromInventory, TileX, TileY, Instance);
        }

        public bool HasAugmentors(string UniqueLocationName, Vector2 Tile) { return HasAugmentors(UniqueLocationName, (int)Tile.X, (int)Tile.Y); }
        public bool HasAugmentors(string UniqueLocationName, int TileX, int TileY)
        {
            if (Locations != null && Locations.TryGetValue(UniqueLocationName, out AugmentedLocation Location))
            {
                return Location.HasAugmentors(TileX, TileY);
            }
            else
            {
                return false;
            }
        }

        public Dictionary<AugmentorType, int> GetAugmentorQuantities(string UniqueLocationName, Vector2 Tile) { return GetAugmentorQuantities(UniqueLocationName, (int)Tile.X, (int)Tile.Y); }
        public Dictionary<AugmentorType, int> GetAugmentorQuantities(string UniqueLocationName, int TileX, int TileY)
        {
            if (Locations != null && Locations.TryGetValue(UniqueLocationName, out AugmentedLocation Location))
            {
                return Location.GetAugmentorQuantities(TileX, TileY);
            }
            else
            {
                Dictionary<AugmentorType, int> Result = new Dictionary<AugmentorType, int>();
                foreach (AugmentorType Type in Enum.GetValues(typeof(AugmentorType)).Cast<AugmentorType>())
                {
                    Result.Add(Type, 0);
                }
                return Result;
            }
        }

        /// <summary>Checks the given tile position of each augmented location, to find a <see cref="AugmentedTile"/> with the matching Machine.</summary>
        public bool TryFindAugmentedTile(Object Machine, Vector2 TileLocation, out AugmentedTile Result) { return TryFindAugmentedTile(Machine, (int)TileLocation.X, (int)TileLocation.Y, out Result); }
        /// <summary>Checks the given tile position of each augmented location, to find a <see cref="AugmentedTile"/> with the matching Machine.</summary>
        public bool TryFindAugmentedTile(Object Machine, int TileX, int TileY, out AugmentedTile Result)
        {
            Result = null;
            if (Machine == null || !Machine.bigCraftable.Value || !Machine.isPlaceable())
                return false;

            string Key = AugmentedLocation.EncodeTileToString(TileX, TileY);
            foreach (AugmentedLocation AL in this.Locations.Values)
            {
                AugmentedTile Tile;
                if (AL.Tiles.TryGetValue(Key, out Tile) && Tile.Machine == Machine)
                {
                    Result = Tile;
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>Represents a <see cref="GameLocation"/> that has at least one tile with a placed <see cref="StardewValley.Object"/> with at least one <see cref="Augmentor"/> attached to it.</summary>
    public class AugmentedLocation
    {
        public PlacedAugmentorsManager Manager { get; }
        /// <summary>The name of the <see cref="GameLocation"/></summary>
        public string UniqueLocationName { get; }
        public GameLocation Location { get { return Game1.getLocationFromName(UniqueLocationName); } }

        /// <summary>
        /// Key = a string encoding of a Tile X,Y Point (use <see cref="EncodeTileToString(int, int)"/>),<para/>
        /// Value = A data structure that keeps track of all <see cref="Augmentor"/>s placed on that Tile
        /// </summary>
        public Dictionary<string, AugmentedTile> Tiles { get; private set; }

        public AugmentedLocation(PlacedAugmentorsManager Manager, string UniqueLocationName)
        {
            this.Manager = Manager;
            this.UniqueLocationName = UniqueLocationName;
            this.Tiles = new Dictionary<string, AugmentedTile>();
        }

        internal void Update()
        {
            foreach (AugmentedTile Tile in Tiles.Values.ToList()) // ToList is necessary since Tile.Update may modify the collection if OnMachineRemoved is detected
                Tile.Update();
        }

        /// <summary>Returns the Key used to index the <see cref="Tiles"/> Dictionary.</summary>
        public static string EncodeTileToString(Vector2 Position) { return EncodeTileToString((int)Position.X, (int)Position.Y); }
        /// <summary>Returns the Key used to index the <see cref="Tiles"/> Dictionary.</summary>
        public static string EncodeTileToString(Point Position) { return EncodeTileToString(Position.X, Position.Y); }
        /// <summary>Returns the Key used to index the <see cref="Tiles"/> Dictionary.</summary>
        public static string EncodeTileToString(int X, int Y) { return string.Format("{0}|{1}", X, Y); }

        internal void OnAugmentorPlaced(AugmentorType Type, int Qty, bool RemoveFromInventory, Vector2 Tile, Augmentor Instance = null) { OnAugmentorPlaced(Type, Qty, RemoveFromInventory, (int)Tile.X, (int)Tile.Y, Instance); }
        internal void OnAugmentorPlaced(AugmentorType Type, int Qty, bool RemoveFromInventory, int TileX, int TileY, Augmentor Instance = null)
        {
            string Key = EncodeTileToString(TileX, TileY);
            if (!Tiles.TryGetValue(Key, out AugmentedTile Tile))
            {
                Tile = new AugmentedTile(this, TileX, TileY);
                Tiles.Add(Key, Tile);
            }

            Tile.OnAugmentorPlaced(Type, Qty, RemoveFromInventory, Instance);
        }

        internal void OnMachineRemoved(AugmentedTile Tile)
        {
            if (Tile.Location != this)
                throw new InvalidOperationException("Object instance mismatch in AugmentedLocation.OnMachineRemoved.");

            Tiles.Remove(EncodeTileToString(Tile.Position));

            if (!Context.IsMultiplayer || Context.IsMainPlayer)
            {
                //  Now that the managed machine has been removed, refund the player with the augmentors that were placed there
                //  by spawning them on the ground
                foreach (KeyValuePair<AugmentorType, int> KVP in Tile.Quantities)
                {
                    int Quantity = KVP.Value;
                    if (Quantity > 0)
                    {
                        Augmentor Refund = Augmentor.CreateInstance(KVP.Key, Quantity);
                        int SpawnDirection;
                        //  When spawning items at the edge of the map, sometimes it seems to move them off the map. Mostly only happens when removing augmentors from incubators in coops,
                        //  so as a temporary workaround, spawn the items in the direction of the player when handling indestructible machines like incubators.
                        if (!MachineInfo.IsDestructible(Tile.Machine))
                            SpawnDirection = Game1.MasterPlayer.getGeneralDirectionTowards(Tile.VectorPosition, 0, true);
                        else
                            SpawnDirection = Augmentor.Randomizer.Next(4);
                        Game1.createItemDebris(Refund, new Vector2(Tile.Position.X * Game1.tileSize, Tile.Position.Y * Game1.tileSize), SpawnDirection, Location, -1);
                    }
                }
            }
        }

        public bool HasAugmentors(Vector2 Tile) { return HasAugmentors((int)Tile.X, (int)Tile.Y); }
        public bool HasAugmentors(int TileX, int TileY)
        {
            string Key = EncodeTileToString(TileX, TileY);
            if (Tiles != null && Tiles.TryGetValue(Key, out AugmentedTile Tile))
            {
                return Tile.HasAugmentors();
            }
            else
            {
                return false;
            }
        }

        public Dictionary<AugmentorType, int> GetAugmentorQuantities(Vector2 Tile) { return GetAugmentorQuantities((int)Tile.X, (int)Tile.Y); }
        public Dictionary<AugmentorType, int> GetAugmentorQuantities(int TileX, int TileY)
        {
            string Key = EncodeTileToString(TileX, TileY);
            if (Tiles != null && Tiles.TryGetValue(Key, out AugmentedTile Tile))
            {
                return Tile.GetAugmentorQuantities();
            }
            else
            {
                Dictionary<AugmentorType, int> Result = new Dictionary<AugmentorType, int>();
                foreach (AugmentorType Type in Enum.GetValues(typeof(AugmentorType)).Cast<AugmentorType>())
                {
                    Result.Add(Type, 0);
                }
                return Result;
            }
        }
    }

    /// <summary>Represents a single tile of a <see cref="GameLocation"/> that has a placed <see cref="StardewValley.Object"/> with at least one <see cref="Augmentor"/> attached to it.</summary>
    public class AugmentedTile
    {
        public AugmentedLocation Location { get; }
        public Point Position { get; }
        public Vector2 VectorPosition { get { return new Vector2(Position.X, Position.Y); } }
        public Object Machine { get; }
        //public MachineInfo MachineInfo { get; }

        /// <summary>Key = A type of augmentor, Value = How many of that Augmentor is placed on this tile</summary>
        public Dictionary<AugmentorType, int> Quantities { get; private set; }

        public AugmentedTile(AugmentedLocation Location, int X, int Y)
        {
            if (!Location.Location.Objects.TryGetValue(new Vector2(X, Y), out Object Item))
                throw new InvalidOperationException(string.Format("The tile at {0},{1} of {2} does not have a placed machine.", X, Y, Location.UniqueLocationName));

            this.Location = Location;
            this.Machine = Item;
            //MachineInfo.TryGetMachineInfo(Machine, out MachineInfo MI);
            //this.MachineInfo = MI;
            this.Position = new Point(X, Y);
            this.Quantities = new Dictionary<AugmentorType, int>();
        }

        private MachineState PreviousMachineState { get; set; } = null;

        internal void Update()
        {
            //  Check if the Machine has been removed from this tile
            if (!Location.Location.Objects.TryGetValue(new Vector2(Position.X, Position.Y), out Object CurrentMachine) || CurrentMachine != Machine)
            {
                Location.OnMachineRemoved(this);
                return;
            }

            try
            {
                if (!Context.IsMultiplayer || Context.IsMainPlayer)
                {
                    if (PreviousMachineState != null)
                    {
                        if (PreviousMachineState.PreviousMinutesUntilReady <= 0 && PreviousMachineState.CurrentMinutesUntilReady > 0)
                        {
                            foreach (KeyValuePair<AugmentorType, int> KVP in PlacedAugmentorsManager.GetOrderedEnumerable(Quantities))
                            {
                                AugmentorType Type = KVP.Key;
                                int AttachedQuantity = KVP.Value;

                                if (AttachedQuantity > 0)
                                {
                                    if (Type == AugmentorType.Output)
                                        OutputAugmentor.OnMinutesUntilReadySet(PreviousMachineState, AttachedQuantity);
                                    else if (Type == AugmentorType.Speed)
                                        SpeedAugmentor.OnMinutesUntilReadySet(PreviousMachineState, AttachedQuantity);
                                    else if (Type == AugmentorType.Efficiency)
                                        EfficiencyAugmentor.OnMinutesUntilReadySet(PreviousMachineState, AttachedQuantity);
                                    else if (Type == AugmentorType.Quality)
                                        QualityAugmentor.OnMinutesUntilReadySet(PreviousMachineState, AttachedQuantity);
                                    else if (Type == AugmentorType.Production)
                                        ProductionAugmentor.OnMinutesUntilReadySet(PreviousMachineState, AttachedQuantity);
                                    else if (Type == AugmentorType.Duplication)
                                        DuplicationAugmentor.OnMinutesUntilReadySet(PreviousMachineState, AttachedQuantity);
                                    else
                                        throw new NotImplementedException(string.Format("Unrecognized AugmentorType: {0}", Type.ToString()));
                                }
                            }
                        }
                    }
                }
            }
            finally { PreviousMachineState = new MachineState(this.Machine); }
        }

        /// <param name="RemoveFromInventory">True if the given <paramref name="Qty"/> should be removed from the player's inventory.</param>
        /// <param name="Instance">If <paramref name="RemoveFromInventory"/>=true, this Instance's Stack will be reduced. If not specified, the first instance found in the player's inventory will be reduced.</param>
        internal void OnAugmentorPlaced(AugmentorType Type, int Qty, bool RemoveFromInventory, Augmentor Instance = null)
        {
            int MaxQuantity = MachineAugmentorsMod.UserConfig.GetConfig(Type).MaxAttachmentsPerMachine;
            int CurrentQty = 0;
            Quantities.TryGetValue(Type, out CurrentQty);
            int ActualQtyPlaced = Math.Max(0, Math.Min(MaxQuantity - CurrentQty, Qty));
            if (ActualQtyPlaced <= 0)
                return;

            if (Quantities.ContainsKey(Type))
            {
                Quantities[Type] += ActualQtyPlaced;
            }
            else
            {
                Quantities.Add(Type, ActualQtyPlaced);
            }

            if (RemoveFromInventory)
            {
                int PendingRemoval = ActualQtyPlaced;
                if (Instance != null && Game1.player.Items.Contains(Instance))
                {
                    int Amt = Math.Min(Instance.Stack, PendingRemoval);
                    if (Amt == Instance.Stack)
                    {
                        Game1.player.Items[Game1.player.Items.IndexOf(Instance)] = null;
                    }
                    else
                    {
                        Instance.Stack -= Amt;
                    }
                }
                else
                {
                    for (int i = 0; i < Game1.player.Items.Count; i++)
                    {
                        Item Item = Game1.player.Items[i];
                        if (Item != null && Item is Augmentor Augmentor && Augmentor.AugmentorType == Type)
                        {
                            int Amt = Math.Min(Augmentor.Stack, PendingRemoval);
                            if (Amt == Augmentor.Stack)
                            {
                                Game1.player.Items[i] = null;
                            }
                            else
                            {
                                Item.Stack -= Amt;
                            }

                            PendingRemoval -= Amt;
                            if (PendingRemoval <= 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        public bool HasAugmentors()
        {
            return Quantities != null && Quantities.Any(x => x.Value > 0);
        }

        public Dictionary<AugmentorType, int> GetAugmentorQuantities()
        {
            Dictionary<AugmentorType, int> Result = new Dictionary<AugmentorType, int>();
            foreach (AugmentorType Type in Enum.GetValues(typeof(AugmentorType)).Cast<AugmentorType>())
            {
                int Qty = 0;
                if (Quantities != null)
                    Quantities.TryGetValue(Type, out Qty);
                Result.Add(Type, Qty);
            }
            return Result;
        }
    }

    public class MachineState
    {
        public Object Machine { get; }

        public Object PreviousHeldObject { get; }
        public Object CurrentHeldObject { get { return Machine?.heldObject.Value; } }
        public int PreviousHeldObjectQuantity { get; }
        public int CurrentHeldObjectQuantity { get { return CurrentHeldObject == null ? 0 : CurrentHeldObject.Stack; } }

        public bool PreviousIsReadyForHarvest { get; }
        public bool CurrentIsReadyForHarvest { get { return Machine.readyForHarvest.Value; } }
        public int PreviousMinutesUntilReady { get; }
        public int CurrentMinutesUntilReady { get { return Machine.MinutesUntilReady; } }

        public MachineState(Object Machine)
        {
            this.Machine = Machine;

            this.PreviousHeldObject = Machine.heldObject.Value;
            this.PreviousHeldObjectQuantity = PreviousHeldObject == null ? 0 : PreviousHeldObject.Stack;
            this.PreviousIsReadyForHarvest = Machine.readyForHarvest.Value;
            this.PreviousMinutesUntilReady = Machine.MinutesUntilReady;
        }
    }
}
