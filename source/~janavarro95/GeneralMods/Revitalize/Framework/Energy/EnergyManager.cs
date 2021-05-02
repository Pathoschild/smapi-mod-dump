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

namespace Revitalize.Framework.Energy
{
    public class EnergyManager
    {

        /// <summary>
        /// The remaining energy left in this system.
        /// </summary>
        public int remainingEnergy;
        /// <summary>
        /// The maximum amount of energy this system can store.
        /// </summary>
        public int maxEnergy;

        public bool requiresUpdate;
        /// <summary>
        /// How does this energy manager interface energy systems.
        /// </summary>
        public Enums.EnergyInteractionType energyInteractionType;

        /// <summary>
        /// Checks to see if this energy manager consumes energy.
        /// </summary>
        public bool consumesEnergy
        {
            get
            {
                return this.energyInteractionType == Enums.EnergyInteractionType.Consumes;
            }
        }
        /// <summary>
        /// Checks to see if this energy manager produces energy.
        /// </summary>
        public bool producesEnergy
        {
            get
            {
                return this.energyInteractionType == Enums.EnergyInteractionType.Produces;
            }
        }

        /// <summary>
        /// Checks to see if this energy manager transfers energy.
        /// </summary>
        public bool transfersEnergy
        {
            get
            {
                return this.energyInteractionType == Enums.EnergyInteractionType.Transfers;
            }
        }
        /// <summary>
        /// Does this energy system have energy.
        /// </summary>
        public bool hasEnergy
        {
            get
            {
                return this.remainingEnergy > 0;
            }
        }
        /// <summary>
        /// Checks to see if this energy system has any energy left.
        /// </summary>
        public bool hasMaxEnergy
        {
            get
            {
                return this.remainingEnergy == this.maxEnergy;
            }
        }
        /// <summary>
        /// Checks to see if this system can receive any energy externally.
        /// </summary>
        public bool canReceieveEnergy
        {
            get
            {
                return !this.hasMaxEnergy;
            }
        }

        public int capacityRemaining
        {
            get
            {
                return this.maxEnergy - this.remainingEnergy;
            }
        }

        /// <summary>
        /// Returns the energy remaining as a percent value.
        /// </summary>
        public double energyPercentRemaining
        {
            get
            {
                return (double)this.remainingEnergy / (double)this.maxEnergy;
            }
        }

        public string energyDisplayString
        {
            get
            {
                StringBuilder b = new StringBuilder();
                b.Append(this.remainingEnergy);
                b.Append("/");
                b.Append(this.maxEnergy);
                return b.ToString();
            }
        }

        public EnergyManager()
        {

        }

        public EnergyManager(int Capacity, Enums.EnergyInteractionType EnergyType) : this(0, Capacity, EnergyType)
        {
        }

        public EnergyManager(int CurrentEnergy, int MaxEnergy, Enums.EnergyInteractionType EnergyType)
        {
            this.remainingEnergy = CurrentEnergy;
            this.maxEnergy = MaxEnergy;
            this.energyInteractionType = EnergyType;
        }


        /// <summary>
        /// Checks to see if this energy source has enough energy remaining.
        /// </summary>
        /// <param name="Required"></param>
        /// <returns></returns>
        public bool hasEnoughEnergy(int Required)
        {
            return this.remainingEnergy >= Required;
        }


        public void consumeEnergy(int amount)
        {
            int amountBeforeConsumption = this.remainingEnergy;
            this.remainingEnergy = Math.Max(0, this.remainingEnergy - amount);
            this.requiresUpdate = true;
        }

        public void produceEnergy(int amount)
        {
            int amountBeforeProduction = this.remainingEnergy;
            this.remainingEnergy = Math.Min(this.maxEnergy, this.remainingEnergy + amount);
            this.requiresUpdate = true;
        }

        public void transferEnergyFromAnother(EnergyManager other, int amount)
        {
            if (this.canReceieveEnergy)
            {
                int actualAmount = Math.Min(amount, other.remainingEnergy);
                int selfCapacity = this.capacityRemaining;
                this.produceEnergy(Math.Min(actualAmount, selfCapacity));
                other.consumeEnergy(Math.Min(actualAmount, selfCapacity));
            }
            else
            {
                return;
            }
        }

        public void transferEnergyToAnother(EnergyManager other, int amount)
        {
            if (other.canReceieveEnergy)
            {
                int actualAmount = Math.Min(amount, this.remainingEnergy);
                int selfCapacity = other.capacityRemaining;
                other.produceEnergy(Math.Min(actualAmount, selfCapacity));
                this.consumeEnergy(Math.Min(actualAmount, selfCapacity));
            }
            else
            {
                return;
            }
        }

        public EnergyManager Copy()
        {
            return new EnergyManager(this.maxEnergy, this.energyInteractionType);
        }

    }
}
