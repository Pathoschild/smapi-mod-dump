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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.CraftingIds;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.Illuminate;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Items;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.Furnaces
{
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Machines.Furnaces.ElectricFurnace")]
    public class ElectricFurnace : PoweredMachine
    {
        public const string ELECTRIC_WORKING_ANIMATION_KEY = "Electric_Working";
        public const string ELECTRIC_IDLE_ANIMATION_KEY = "Electric_Idle";

        public const string NUCLEAR_WORKING_ANIMATION_KEY = "Nuclear_Working";
        public const string NUCLEAR_IDLE_ANIMATION_KEY = "Nuclear_Idle";

        public const string MAGICAL_WORKING_ANIMATION_KEY = "Magical_Working";
        public const string MAGICAL_IDLE_ANIMATION_KEY = "Magical_Idle";

        public ElectricFurnace()
        {

        }


        public ElectricFurnace(BasicItemInformation info, PoweredMachineTier furnaceType) : base(info,furnaceType)
        {
            this.createStatusBubble();
        }

        public ElectricFurnace(BasicItemInformation info, Vector2 TileLocation, PoweredMachineTier furnaceType) : base(info, TileLocation,furnaceType)
        {
            this.createStatusBubble();
        }

        public override CraftingResult onSuccessfulRecipeFound(IList<Item> dropInItem, ProcessingRecipe craftingRecipe, Farmer who = null)
        {
            CraftingResult result = base.onSuccessfulRecipeFound(dropInItem, craftingRecipe, who);

            if (result.successful)
            {
                this.addLight(new Vector2(0, 0), Illuminate.LightManager.LightIdentifier.SconceLight, Color.DarkCyan.Invert(), 1.5f);
            }


            return result;
        }

        public override void playDropInSound()
        {
            SoundUtilities.PlaySound(Enums.StardewSound.furnace);
        }

        public virtual string getCraftingBookName()
        {
            return Constants.CraftingIds.MachineCraftingRecipeBooks.ElectricFurnaceCraftingRecipies;
        }

        /// <summary>
        /// Updates the animation manager to play the correct animation.
        /// </summary>
        public override void updateAnimation()
        {
            if (this.machineTier.Value == PoweredMachineTier.Electric)
            {
                if (this.MinutesUntilReady > 0)
                {
                    this.AnimationManager.playAnimation(ELECTRIC_WORKING_ANIMATION_KEY);
                    return;
                }
                else
                {
                    this.AnimationManager.playAnimation(ELECTRIC_IDLE_ANIMATION_KEY);
                    return;
                }

            }
            if (this.machineTier.Value == PoweredMachineTier.Nuclear)
            {
                if (this.MinutesUntilReady > 0)
                {
                    this.AnimationManager.playAnimation(NUCLEAR_WORKING_ANIMATION_KEY);
                    return;
                }
                else
                {
                    this.AnimationManager.playAnimation(NUCLEAR_IDLE_ANIMATION_KEY);
                    return;
                }
            }
            if (this.machineTier.Value == PoweredMachineTier.Magical)
            {
                if (this.MinutesUntilReady > 0)
                {
                    this.AnimationManager.playAnimation(MAGICAL_WORKING_ANIMATION_KEY);
                    return;
                }
                else
                {
                    this.AnimationManager.playAnimation(MAGICAL_IDLE_ANIMATION_KEY);
                    return;
                }
            }
        }

        public override int getElectricFuelChargeIncreaseAmount()
        {
            return 5;
        }

        public override int getNuclearFuelChargeIncreaseAmount()
        {
            return 25;
        }


        public override Item getOne()
        {
            return new ElectricFurnace(this.basicItemInformation.Copy(), this.machineTier.Value);
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            bool elapsed= base.minutesElapsed(minutes, environment);
            if (this.finishedProduction())
            {
                this.LightManager.removeLight(new Vector2(0, 0), this.getCurrentLocation());
            }
            return elapsed;
        }

        public override string getCraftingRecipeBookId()
        {
            return MachineCraftingRecipeBooks.ElectricFurnaceCraftingRecipies;
        }
    }
}
