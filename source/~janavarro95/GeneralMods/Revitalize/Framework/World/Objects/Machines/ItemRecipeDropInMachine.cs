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
using Netcode;
using Newtonsoft.Json;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.HUD;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.Utilities.Extensions;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Items;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines
{
    public class ItemRecipeDropInMachine : Machine
    {


        public ItemRecipeDropInMachine()
        {

        }

        public ItemRecipeDropInMachine(BasicItemInformation info) : base(info)
        {
        }

        public ItemRecipeDropInMachine(BasicItemInformation info, Vector2 TileLocation) : base(info, TileLocation)
        {

        }

        public override bool performItemDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            if (probe) return false;
            //Prevent overriding and destroying the previous operation.
            if (who != null && this.finishedProduction())
            {
                bool added = this.tryToAddHeldItemsToFarmersInventory();
                //Either player's inventory is full or not all outputs were gathered.
                if (!added || this.heldObject.Value!=null)
                {
                    return false;
                }
            }
            bool success = base.performItemDropInAction(dropInItem, probe, who);
            if (!success) return false;

            IList<Item> items = new List<Item>() { dropInItem };

            CraftingResult result = this.processInput(who != null ? who.Items : items, who, true);


            return result.successful;
        }

        public virtual bool tryToAddHeldItemsToFarmersInventory()
        {
            List<Item> addedItems= this.getMachineOutputs(true, false, true);
            if(addedItems.Count>0)
            {
                SoundUtilities.PlaySound(Enums.StardewSound.coin);
                return true;
            }
            return false;
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity)
        {
            if (who != null && this.finishedProduction())
            {
                bool added = this.tryToAddHeldItemsToFarmersInventory();
                if (!added || this.heldObject.Value!=null)
                {
                    return false;
                }
                return true;
            }
            return base.checkForAction(who,justCheckingForActivity);
        }

        /// <summary>
        /// Processes a player's item that they are holding to set recipe to be processed for the machine.
        /// </summary>
        /// <param name="inputItems"></param>
        /// <param name="who"></param>
        /// <param name="ShowRedMessage"></param>
        /// <returns></returns>
        public override CraftingResult processInput(IList<Item> inputItems, Farmer who, bool ShowRedMessage = true)
        {
            if (string.IsNullOrEmpty(this.getCraftingRecipeBookId()) || this.isWorking() || this.finishedProduction())
            {
                return new CraftingResult(false);
            }

            List<KeyValuePair<IList<Item>, ProcessingRecipe>> validRecipes = this.getListOfValidRecipes(inputItems,who,ShowRedMessage);

            if(validRecipes.Count > 0)
            {
                return this.onSuccessfulRecipeFound(validRecipes.ElementAt(0).Key, validRecipes.ElementAt(0).Value, who);
            }

            return new CraftingResult(false);
        }

        public virtual List<ProcessingRecipe> getListOfPotentialRecipes(IList<Item> inputItems,Farmer who=null)
        {
            return RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId());
        }

        public virtual List<KeyValuePair<IList<Item>,ProcessingRecipe>> getListOfValidRecipes(IList<Item> inputItems, Farmer who, bool ShowRedMessage = true)
        {
            List<KeyValuePair<IList<Item>, ProcessingRecipe>> validRecipes = new List<KeyValuePair<IList<Item>, ProcessingRecipe>>();

            foreach (ProcessingRecipe craftingRecipe in this.getListOfPotentialRecipes(inputItems,who))
            {
                IList<Item> consumedItems = new List<Item>();
                bool activeObjectChecked = false;
                bool invalid = false;
                foreach (ItemReference requiredItem in craftingRecipe.inputs)
                {
                    Item neededDropInItem = requiredItem.getItem();
                    int amountRequired = requiredItem.StackSize;


                    //If the farmer is passed in, we know that this will be the farmer's inventory. If so, we want to prioritize passing in the ActiveObject as part of the recipe, otherwise other items in the inventory may be prioritized.
                    if (who != null && activeObjectChecked == false)
                    {
                        if (who.ActiveObject != null)
                        {
                            if (neededDropInItem.canStackWith(who.ActiveObject) || requiredItem.itemEquals(who.ActiveObject))
                            {
                                //Check to make sure the player has enough, otherwise display an error!
                                if (amountRequired > who.ActiveObject.Stack)
                                {
                                    if (ShowRedMessage)
                                    {
                                        Game1.showRedMessage(this.getErrorString_NeedMoreInputItems(amountRequired, who.ActiveObject));
                                    }
                                    invalid= true;
                                    break;
                                }
                                consumedItems.Add(who.ActiveObject);
                                activeObjectChecked = true;
                                continue;
                            }
                        }
                    }

                    if (inputItems != null)
                    {
                        foreach (Item inputItem in inputItems)
                        {
                            if (inputItem == null)
                            {
                                continue;
                            }
                            //Since we already checked the Player's active object we don't want to check it twice.
                            if (activeObjectChecked)
                            {
                                if (who.ActiveObject == inputItem)
                                {
                                    continue;
                                }
                            }

                            if (neededDropInItem.canStackWith(inputItem) || requiredItem.itemEquals(inputItem))
                            {
                                //Check to make sure the player has enough, otherwise display an error!
                                if (amountRequired > inputItem.Stack)
                                {
                                    if (ShowRedMessage)
                                    {
                                        Game1.showRedMessage(this.getErrorString_NeedMoreInputItems(amountRequired, inputItem));
                                    }
                                    invalid = true;
                                    break;
                                }
                                consumedItems.Add(inputItem);
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }

                if (invalid)
                {
                    continue;
                }

                if (consumedItems.Count == craftingRecipe.inputs.Count)
                {
                    //If the player is the one interacting with this object, force the recipe to be used only if the active object is part of the recipe.
                    if (who != null && activeObjectChecked)
                    {
                        validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(consumedItems, craftingRecipe));
                    }
                    else if (who != null && activeObjectChecked == false)
                    {
                        continue;
                    }
                    else
                    {
                        validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(consumedItems, craftingRecipe));
                    }

                }
            }
            return validRecipes;
        }

        public virtual CraftingResult onSuccessfulRecipeFound(IList<Item> consumedItems, ProcessingRecipe craftingRecipe, Farmer who = null)
        {
            foreach (ItemReference requiredItem in craftingRecipe.inputs)
            {
                if (requiredItem.isNotNull())
                {
                    PlayerUtilities.ReduceInventoryItemStackSize(consumedItems, requiredItem.getItem(), requiredItem.StackSize,who);
                }
            }

            foreach (LootTableEntry outputItem in craftingRecipe.outputs)
            {
                Item item = outputItem.getOutputItem();
                if (item != null)
                {
                    this.addItemToHeldItemQueue(item);
                }
            }

            this.heldObject.Value = (StardewValley.Object)this.getItemFromHeldItemQueue();
            this.MinutesUntilReady = (int)(craftingRecipe.timeToProcess.toInGameMinutes());

            if (who != null)
            {
                this.playDropInSound();
            }
            this.updateAnimation();

            return new CraftingResult(craftingRecipe.inputs, true); //Found a sucessful recipe.
        }

        public virtual void playDropInSound()
        {
            //SoundUtilities.PlaySound(Enums.StardewSound.Ship);
        }

        public override void updateAnimation()
        {
            if (this.isWorking())
            {
                this.AnimationManager.playAnimation(Machine.WORKING_ANIMATION_KEY);
            }
            else
            {
                this.AnimationManager.playDefaultAnimation();
            }
        }


        public override Item getOne()
        {
            ItemRecipeDropInMachine component = new ItemRecipeDropInMachine(this.basicItemInformation.Copy());
            return component;
        }

        public virtual string getCraftingRecipeBookId()
        {
            return this.Id;
        }

    }
}
