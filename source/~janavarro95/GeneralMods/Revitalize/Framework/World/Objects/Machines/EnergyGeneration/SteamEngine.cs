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
using Omegasis.Revitalize.Framework.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Revitalize.Framework.Managers;
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.EnergyGeneration
{
    public class SteamEngine:Machine
    {

        public SteamEngine() { }

        public SteamEngine(BasicItemInformation info, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "",Fluid FluidRequiredForOperation = null, int FluidAmountRequiredPerOperation=0) : base(info)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
            this.requiredFluidForOperation = FluidRequiredForOperation;
            this.amountOfFluidRequiredForOperation = FluidAmountRequiredPerOperation;
        }

        public SteamEngine(BasicItemInformation info, Vector2 TileLocation, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "", Fluid FluidRequiredForOperation = null, int FluidAmountRequiredPerOperation = 0) : base(info, TileLocation)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
            this.requiredFluidForOperation = FluidRequiredForOperation;
            this.amountOfFluidRequiredForOperation = FluidAmountRequiredPerOperation;
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            base.updateWhenCurrentLocation(time, environment);

            this.AnimationManager.prepareForNextUpdateTick();
        }


        public override bool minutesElapsed(int minutes, GameLocation environment)
        {

            this.pullFluidFromNetworkOutputs(ModCore.ObjectManager.resources.getFluid("Steam"));

                int remaining = minutes;
                while (remaining > 0)
                {
                    if (this.GetEnergyManager().canReceieveEnergy == false) return false;
                    remaining -= 10;
                    int fluidAmount = this.GetFluidManager().getAmountOfFluidInInputTanks(this.requiredFluidForOperation);
                    if (this.GetFluidManager().doTheInputTanksHaveEnoughFluid(ModCore.ObjectManager.resources.getFluid(this.requiredFluidForOperation.name), this.amountOfFluidRequiredForOperation))
                    {
                        this.GetFluidManager().consumeFluid(ModCore.ObjectManager.resources.getFluid(this.requiredFluidForOperation.name), this.amountOfFluidRequiredForOperation);
                        //this.GetFluidManager().produceFluid(ModCore.ObjectManager.resources.getFluid("Steam"), ModCore.Configs.machinesConfig.steamBoilerV1_producedSteamPerOperation);
                        this.produceEnergy();
                        this.storeEnergyToNetwork();
                    }
                    else if(fluidAmount>0)
                    {
                        //Try to always consume fluid if possible.
                        double ratio = (double)fluidAmount / (double)ModCore.Configs.machinesConfig.steamEngineV1_requiredSteamPerOperation;
                        int liquidToConsume = fluidAmount;
                        this.GetFluidManager().consumeFluid(ModCore.ObjectManager.resources.getFluid(this.requiredFluidForOperation.name), liquidToConsume);
                        this.produceEnergy(ratio);
                        this.storeEnergyToNetwork();
                    }
                }
                return false;

            //return base.minutesElapsed(minutes, environment);
        }


        public override Item getOne()
        {
            SteamEngine component = new SteamEngine(this.getItemInformation().Copy(), this.producedResources, this.energyRequiredPer10Minutes, this.timeToProduce, this.craftingRecipeBook,this.requiredFluidForOperation,this.amountOfFluidRequiredForOperation);
            //component.containerObject = this.containerObject;
            //component.offsetKey = this.offsetKey;
            return component;
            return component;
        }

        public override void produceEnergy()
        {
            
            if (this.GetEnergyManager().canReceieveEnergy)
            {
                this.GetEnergyManager().produceEnergy(this.energyRequiredPer10Minutes);
            }
            
        }

        public virtual void processFluidLogic()
        {
            return;
        }

    }
}
