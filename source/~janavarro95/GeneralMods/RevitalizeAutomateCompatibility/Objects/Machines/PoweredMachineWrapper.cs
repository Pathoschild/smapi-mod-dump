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
using Omegasis.Revitalize.Framework.Constants.Ids.Items;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Machines;
using Pathoschild.Stardew.Automate;
using StardewValley;
using static Omegasis.Revitalize.Framework.World.Objects.Machines.PoweredMachine;

namespace Omegasis.RevitalizeAutomateCompatibility.Objects.Machines
{
    public class PoweredMachineWrapper<T> : MachineWrapper<T> where T : PoweredMachine
    {

        public PoweredMachineWrapper()
        {

        }

        /// <summary>
        /// Used to automate <see cref="CustomObject"/>s for the mod Revitalize.
        /// </summary>
        /// <param name="CustomObject"></param>
        public PoweredMachineWrapper(T CustomObject, GameLocation location, in Vector2 TileLocation) : base(CustomObject, location, TileLocation)
        {
        }

        /// <summary>
        /// Used to set the inputs for this machine to begin crafting.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool SetInput(IStorage input)
        {
            //Optimize to ensure that we don't do unnecessary logic for setting inputs or for manual machines.
            if (this.customObject.isWorking() || this.customObject.finishedProduction() || this.customObject.MachineTier== PoweredMachineTier.Manual) return false;

            this.TryGetFuelItem(input, out IConsumable fuelType);
            if (this.customObject.fuelChargesRemaining.Value <= 0 && fuelType != null)
            {
                fuelType.Reduce();
                this.customObject.increaseFuelCharges();
            }

            if (this.customObject.hasFuel())
            {
                base.SetInput(input);
            }
            this.customObject.updateAnimation();

            return false;
        }

        /// <summary>
        /// Attempts to get a single fuel source item (or not if the furnace is magical) from a chest.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="consumable"></param>
        /// <returns></returns>
        public virtual bool TryGetFuelItem(IStorage input, out IConsumable? consumable)
        {
            if (this.customObject.machineTier.Value == PoweredMachineTier.Magical || this.customObject.machineTier.Value == PoweredMachineTier.Galaxy)
            {
                consumable = null;
                return true;
            }

            if (this.customObject.hasFuel())
            {
                consumable = null;
                return true;
            }

            if (this.customObject.machineTier.Value == PoweredMachineTier.Coal)
            {
                bool itemFound = input.TryGetVanillaIngredient(Enums.SDVObject.Coal, 1, out IConsumable? coal);
                consumable = coal;
                return itemFound;
            }
            if (this.customObject.machineTier.Value == PoweredMachineTier.Electric)
            {
                bool itemFound = input.TryGetVanillaIngredient(Enums.SDVObject.BatteryPack, 1, out IConsumable? batteryPack);
                consumable = batteryPack;
                return itemFound;
            }
            if (this.customObject.machineTier.Value == PoweredMachineTier.Nuclear)
            {
                bool itemFound = input.TryGetRevitalizeItemIngredient(MiscItemIds.RadioactiveFuel, 1, out IConsumable? fuelCell);
                consumable = fuelCell;
                return itemFound;
            }
            consumable = null;
            return false;
        }

    }
}
