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
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.EnergyGeneration
{
    public class SteamBoiler : Machine
    {

        public SteamBoiler() { }

        public SteamBoiler(BasicItemInformation info, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();

        }

        public SteamBoiler(BasicItemInformation info, Vector2 TileLocation, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, bool UpdatesContainer = false, string CraftingBook = "") : base(info, TileLocation)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
        }


        public override bool minutesElapsed(int minutes, GameLocation environment)
        {

            if (this.MinutesUntilReady > 0)
            {
                this.AnimationManager.playAnimation("Working");
            }
            else
            {
                this.AnimationManager.playDefaultAnimation();
            }

            this.pullFluidFromNetworkOutputs(ModCore.ObjectManager.resources.getFluid("Water"));

            int remaining = minutes;
            if (this.MinutesUntilReady <= 0)
            {
                this.searchInventoryForBurnableObjects();
            }
            while (remaining > 0 && this.MinutesUntilReady > 0)
            {
                remaining -= 10;
                this.processFluidLogic();
                if (this.MinutesUntilReady <= 0)
                {
                    this.searchInventoryForBurnableObjects();
                }
            }
            if (this.GetEnergyManager().energyInteractionType == Enums.EnergyInteractionType.Produces)
            {
                this.storeEnergyToNetwork();
            }
            return false;
        }


        public override Item getOne()
        {
            SteamBoiler component = new SteamBoiler( this.getItemInformation().Copy(), this.producedResources, this.energyRequiredPer10Minutes, this.timeToProduce, this.craftingRecipeBook);
            return component;
        }

        public override void produceEnergy()
        {
        }

        public virtual void processFluidLogic()
        {
            if (this.GetFluidManager().doTheInputTanksHaveEnoughFluid(ModCore.ObjectManager.resources.getFluid("Water"), ModCore.Configs.machinesConfig.steamBoilerV1_requiredWaterPerOperation))
            {
                this.GetFluidManager().consumeFluid(ModCore.ObjectManager.resources.getFluid("Water"), ModCore.Configs.machinesConfig.steamBoilerV1_requiredWaterPerOperation);
                this.GetFluidManager().produceFluid(ModCore.ObjectManager.resources.getFluid("Steam"), ModCore.Configs.machinesConfig.steamBoilerV1_producedSteamPerOperation);
                this.MinutesUntilReady -= 10;
            }
        }

        protected virtual void searchInventoryForBurnableObjects()
        {
            Item removed = null;
            foreach (Item I in this.GetInventoryManager().items)
            {
                if (ModCore.ObjectManager.resources.burnableObjects.ContainsKey(I.Name))
                {
                    this.MinutesUntilReady = ModCore.ObjectManager.resources.burnableObjects[I.Name];
                    removed = I;
                    break;
                }
            }
            if (removed == null) return;
            if (removed.Stack == 1) this.GetInventoryManager().items.Remove(removed);
            else removed.Stack -= 1;
        }
    }
}
