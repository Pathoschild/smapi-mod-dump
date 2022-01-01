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

namespace Omegasis.Revitalize.Framework.World.Objects.Machines
{
    public class AlloyFurnace : Machine
    {

        public AlloyFurnace() { }

        public AlloyFurnace(BasicItemInformation info, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();

        }

        public AlloyFurnace(BasicItemInformation info, Vector2 TileLocation, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info, TileLocation)
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

            this.AnimationManager.prepareForNextUpdateTick();
        }


        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            List<IEnergyManagerProvider> energySources = new List<IEnergyManagerProvider>();
            if (this.doesMachineConsumeEnergy() || this.GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Storage)
            {
                //ModCore.log("This machine drains energy: " + this.info.name);
                energySources = this.EnergyGraphSearchSources(); //Only grab the network once.
            }
            if (this.MinutesUntilReady > 0)
            {
                this.MinutesUntilReady = Math.Max(0, this.MinutesUntilReady - minutes);

                if (this.GetInventoryManager().hasItemsInBuffer && this.MinutesUntilReady == 0)
                {
                    this.GetInventoryManager().dumpBufferToItems();
                }

                this.AnimationManager.playAnimation("Working");
            }
            else
            {
                this.AnimationManager.playDefaultAnimation();
            }

            return false;

            //return base.minutesElapsed(minutes, environment);
        }

        public override Item getOne()
        {
            AlloyFurnace component = new AlloyFurnace(this.getItemInformation().Copy(), this.producedResources, this.energyRequiredPer10Minutes, this.timeToProduce, this.craftingRecipeBook);
            return component;
        }

    }
}
