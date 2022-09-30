/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.ItemIds.Items;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.World.Objects.Machines.Furnaces;
using Omegasis.RevitalizeAutomateCompatibility.Recipes;
using Pathoschild.Stardew.Automate;
using StardewValley;

namespace Omegasis.RevitalizeAutomateCompatibility.MachineWrappers.Objects.Machines.Furnaces
{
    /// <summary>
    /// Wrapper class to add Automate compatibility for 
    /// </summary>
    public class ElectricFurnaceWrapper : CustomObjectMachineWrapper<ElectricFurnace>
    {
        /// <summary>
        /// List of items that can be smelted.
        /// </summary>
        public static List<Enums.SDVObject> SmeltableItems = new List<Enums.SDVObject>() {
            Enums.SDVObject.CopperOre,
            Enums.SDVObject.IronOre,
            Enums.SDVObject.GoldOre,
            Enums.SDVObject.IridiumOre,
            Enums.SDVObject.RadioactiveOre,
            Enums.SDVObject.Quartz,
            Enums.SDVObject.FireQuartz,
            Enums.SDVObject.WiltedBouquet,
        };

        /// <summary>
        /// Constructor.
        /// </summary>
        public ElectricFurnaceWrapper()
        {

        }

        /// <summary>
        /// Base item.
        /// </summary>
        /// <param name="furnace"></param>
        /// <param name="location"></param>
        /// <param name="TileLocation"></param>
        public ElectricFurnaceWrapper(ElectricFurnace furnace, GameLocation location, Vector2 TileLocation) : base(furnace, location, TileLocation)
        {

        }

        /// <summary>
        /// Used to set the inputs for this machine to begin crafting.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool SetInput(IStorage input)
        {
            bool fuelTypeFound = this.TryGetFuelItem(input, out IConsumable fuelType);

            if (fuelTypeFound)
            {
                bool smelted;
                foreach (Enums.SDVObject obj in SmeltableItems)
                {
                    smelted = this.TryToSmeltItem(input, obj, fuelType);
                    if (smelted) return true;
                }

            }

            return false;


        }

        /// <summary>
        /// Tries to smelt a single item 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="obj"></param>
        /// <param name="fuelType"></param>
        /// <returns></returns>
        public virtual bool TryToSmeltItem(IStorage input, Enums.SDVObject obj, IConsumable fuelType)
        {
            if (this.TryGetSmeltingItem(input, obj, out IConsumable? itemToConsume))
            {
                if (this.customObject.chargesRemaining.Value <= 0 && fuelType != null)
                {
                    fuelType.Reduce();
                }
                itemToConsume.Reduce();
                this.customObject.smeltItem(obj);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Attempts to get a single item to be smelted item from a chest.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="obj"></param>
        /// <param name="consumable"></param>
        /// <returns></returns>
        public virtual bool TryGetSmeltingItem(IStorage input, Enums.SDVObject obj, out IConsumable? consumable)
        {
            bool itemFound = input.TryGetVanillaIngredient(obj, this.customObject.getStackSizeNecessaryForSmelting(obj), out IConsumable? consumableItem);
            consumable = consumableItem;
            return itemFound;
        }

        /// <summary>
        /// Attempts to get a single fuel source item (or not if the furnace is magical) from a chest.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="consumable"></param>
        /// <returns></returns>
        public virtual bool TryGetFuelItem(IStorage input, out IConsumable? consumable)
        {
            if (this.customObject.furnaceType.Value == ElectricFurnace.FurnaceType.Magical)
            {
                consumable = null;
                return true;
            }

            if (this.customObject.chargesRemaining.Value > 0)
            {
                consumable = null;
                return true;
            }

            if (this.customObject.furnaceType.Value == ElectricFurnace.FurnaceType.Electric)
            {
                bool itemFound = input.TryGetVanillaIngredient(Enums.SDVObject.BatteryPack, 1, out IConsumable? batteryPack);
                consumable = batteryPack;
                return itemFound;
            }
            if (this.customObject.furnaceType.Value == ElectricFurnace.FurnaceType.Nuclear)
            {
                bool itemFound = input.TryGetRevitalizeItemIngredient(MiscItemIds.RadioactiveFuel, 1, out IConsumable? fuelCell);
                consumable = fuelCell;
                return itemFound;
            }
            consumable = null;
            return false;
        }

        /// <summary>Get the output item.</summary>
        public override ITrackedStack GetOutput()
        {
            //Returns a tracked object which is set to modify the machine's held object value to be null when complete.
            return new TrackedItem(this.customObject.heldObject.Value, onEmpty: item =>
            {
                this.customObject.cleanOutFurnace(false);
            });
        }


    }
}
