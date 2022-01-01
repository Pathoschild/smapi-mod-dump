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
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Omegasis.Revitalize.Framework.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardustCore.Animations;
using StardustCore.UIUtilities;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines
{
    public class ChargingStation : Machine
    {

        public ChargingStation() { }

        public ChargingStation(BasicItemInformation info, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();

        }

        public ChargingStation(BasicItemInformation info, Vector2 TileLocation, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info, TileLocation)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            base.updateWhenCurrentLocation(time, environment);
        }


        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            int remaining = minutes;
            List<IEnergyManagerProvider> energySources = new List<IEnergyManagerProvider>();
            if (this.doesMachineConsumeEnergy() || this.GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Storage)
            {
                energySources = this.EnergyGraphSearchSources();
            }
            this.drainEnergyFromNetwork(energySources);
            foreach (Item I in this.GetInventoryManager().items)
            {
                if (I is null) continue;
                if (I is IEnergyManagerProvider == false) continue;
                IEnergyManagerProvider o = (IEnergyManagerProvider)I;
                if (o.GetEnergyManager().canReceieveEnergy)
                {
                    this.GetEnergyManager().transferEnergyToAnother(o.GetEnergyManager(), Math.Min(this.GetEnergyManager().remainingEnergy, o.GetEnergyManager().capacityRemaining));
                }
                if (this.GetEnergyManager().hasEnergy == false) break;
            }

            return false;

        }


        public override Item getOne()
        {
            ChargingStation component = new ChargingStation(this.getItemInformation().Copy(), this.TileLocation, this.producedResources, this.energyRequiredPer10Minutes, this.timeToProduce, this.craftingRecipeBook);
            return component;
        }


    }
}
