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
        public JsonItemReference item;

        public JsonCraftingComponent()
        {
            this.item = new JsonItemReference();
        }

        /// <summary>
        /// Creates a <see cref="CraftingRecipeComponent"/> from a json version loaded from disk.
        /// </summary>
        /// <returns></returns>
        public CraftingRecipeComponent createCraftingRecipeComponent()
        {
            if (this.item.stardewValleyItemId > 0)
            {
                return new CraftingRecipeComponent(RevitalizeModCore.ModContentManager.objectManager.getItem(this.item.stardewValleyItemId, 1), this.item.amount);
            }
            if (this.item.stardewValleyBigCraftableId > 0)
            {
                return new CraftingRecipeComponent(RevitalizeModCore.ModContentManager.objectManager.getItem(this.item.stardewValleyBigCraftableId, 1), this.item.amount);
            }
            if (!string.IsNullOrEmpty(this.item.registeredObjectId))
            {
                return new CraftingRecipeComponent(RevitalizeModCore.ModContentManager.objectManager.getItem(this.item.registeredObjectId, 1), this.item.amount);
            }
            throw new InvalidJsonCraftingComponentException("A json crafting component must have one one of the following: a stardewValleyItemId, a stardewValleyBigCraftableId or a registeredObjectId set to be valid!");
        }

        /// <summary>
        /// Validates the state of the json crafting component before creating a crafting component from disk.
        /// </summary>
        public virtual void validate()
        {
            int numberOfValidIdFieldsSet = 0;
            if(this.item.stardewValleyItemId!= Enums.SDVObject.NULL)
            {
                numberOfValidIdFieldsSet++;
            }
            if (this.item.stardewValleyBigCraftableId != Enums.SDVBigCraftable.NULL)
            {
                numberOfValidIdFieldsSet++;
            }
            if (!string.IsNullOrEmpty(this.item.registeredObjectId))
            {
                if (!RevitalizeModCore.ModContentManager.objectManager.itemsById.ContainsKey(this.item.registeredObjectId))
                {
                    throw new InvalidJsonCraftingComponentException(string.Format("A json crafting component requests that it uses or gives an item with the registered id of {0} but no object with that id has been registered to the ModContentManager.ObjectManager's registered items list.", this.item.registeredObjectId));
                }
                numberOfValidIdFieldsSet++;
            }

            if (numberOfValidIdFieldsSet == 0)
            {
                throw new InvalidJsonCraftingComponentException("A json crafting component must have either a stardewValleyItemId, a stardewValleyBigCraftableId or a registeredObjectId set to be valid!");
            }

            if (numberOfValidIdFieldsSet > 1)
            {
                throw new InvalidJsonCraftingComponentException("A json crafting component must have one one of the following: a stardewValleyItemId, a stardewValleyBigCraftableId or a registeredObjectId set to be valid!");
            }

        }

        /// <summary>
        /// Gets the item that is referenced by this crafting component.
        /// </summary>
        /// <returns></returns>
        public virtual Item getItem()
        {
            if(this.item.stardewValleyItemId!= Enums.SDVObject.NULL)
            {
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.item.stardewValleyItemId, this.item.amount);
            }
            if(this.item.stardewValleyBigCraftableId!= Enums.SDVBigCraftable.NULL)
            {
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.item.stardewValleyBigCraftableId, this.item.amount);
            }
            if (!string.IsNullOrEmpty(this.item.registeredObjectId))
            {
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.item.registeredObjectId, this.item.amount);
            }
            throw new InvalidJsonCraftingComponentException("A json crafting component must have one one of the following: a stardewValleyItemId, a stardewValleyBigCraftableId or a registeredObjectId set to be valid!");
        }

        /// <summary>
        /// Converts this json information file to the actual <see cref="CraftingRecipeComponent"/> that is used in crafting.
        /// </summary>
        /// <returns></returns>
        public virtual CraftingRecipeComponent toCraftingRecipeComponent()
        {
            return new CraftingRecipeComponent(this.getItem(), this.item.amount);
        }
    }
}
