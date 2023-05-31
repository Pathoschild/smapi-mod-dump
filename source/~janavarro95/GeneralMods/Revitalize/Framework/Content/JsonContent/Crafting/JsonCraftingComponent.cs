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
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles.Json;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Crafting.JsonContent
{
    /// <summary>
    /// A recipe component written into a stucture to be used for json serialization/deserialization.
    /// </summary>
    public class JsonCraftingComponent
    {
        /// <summary>
        /// A reference to the item loaded in from json.
        /// </summary>
        public ItemReference item;

        public JsonCraftingComponent()
        {
            this.item = new ItemReference();
        }

        public JsonCraftingComponent(ItemReference item)
        {
            this.item = item;
        }


        /// <summary>
        /// Creates a <see cref="CraftingRecipeComponent"/> from a json version loaded from disk.
        /// </summary>
        /// <returns></returns>
        public virtual CraftingRecipeComponent createCraftingRecipeComponent()
        {
            if (!string.IsNullOrEmpty(this.item.RegisteredObjectId))
            {
                return this.toCraftingRecipeComponent();
            }
            throw new InvalidJsonCraftingComponentException("A json crafting component must have one one of the following: a stardewValleyItemId, a stardewValleyBigCraftableId or a registeredObjectId set to be valid!");
        }

        /// <summary>
        /// Validates the state of the json crafting component before creating a crafting component from disk.
        /// </summary>
        public virtual void validate()
        {
            if (!string.IsNullOrEmpty(this.item.RegisteredObjectId))
            {
                if (!RevitalizeModCore.ModContentManager.objectManager.itemsById.ContainsKey(this.item.RegisteredObjectId))
                {
                    throw new InvalidJsonCraftingComponentException(string.Format("A json crafting component requests that it uses or gives an item with the registered id of {0} but no object with that id has been registered to the ModContentManager.ObjectManager's (or the Stardew Valley Item Registry) registered items list.", this.item.RegisteredObjectId));
                }
            }
            else
            {
                throw new InvalidJsonCraftingComponentException("A json crafting component must have at least one field set to be valid!");
            }
        }

        /// <summary>
        /// Gets the item that is referenced by this crafting component.
        /// </summary>
        /// <returns></returns>
        public virtual Item getItem()
        {
            Item item = this.item.getItem();
            if (item == null)
            {
                throw new InvalidJsonCraftingComponentException("A json crafting component must have one one of the following: a stardewValleyItemId, a stardewValleyBigCraftableId or a registeredObjectId set to be valid!");
            }
            return item;
        }

        /// <summary>
        /// Converts this json information file to the actual <see cref="CraftingRecipeComponent"/> that is used in crafting.
        /// </summary>
        /// <returns></returns>
        public virtual CraftingRecipeComponent toCraftingRecipeComponent()
        {
            return new CraftingRecipeComponent(this.getItem(), this.item.StackSize);
        }

        /// <summary>
        /// Returns the required amount for the crafting recipe component.
        /// </summary>
        /// <returns></returns>
        public virtual int getRequiredAmount()
        {

            return this.item.StackSize;
        }
    }
}
