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
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.InformationFiles.Json
{
    /// <summary>
    /// Used to get items from the ObjectManager for json related purposes.
    /// </summary>
   public  class JsonItemReference
    {
        /// <summary>
        /// The stardew valley item id used. 
        /// </summary>
        public Enums.SDVObject stardewValleyItemId;
        /// <summary>
        /// The parent sheet index of the big craftable used.
        /// </summary>
        public Enums.SDVBigCraftable stardewValleyBigCraftableId;
        /// <summary>
        /// The object id of the item registered in the object manager.
        /// </summary>
        public string registeredObjectId;

        /// <summary>
        /// The amound of items necessary or given for this crafting component.
        /// </summary>
        public int amount;


        public JsonItemReference()
        {
            this.stardewValleyItemId = Enums.SDVObject.NULL;
            this.stardewValleyBigCraftableId = Enums.SDVBigCraftable.NULL;
            this.amount = 0;
            this.registeredObjectId = "";
        }

        public JsonItemReference(int ParentSheetIndex, bool IsBigCraftable, int AmountNeeded) : this()
        {
            if (IsBigCraftable)
            {
                this.stardewValleyBigCraftableId = (Enums.SDVBigCraftable)ParentSheetIndex;
            }
            else
            {
                this.stardewValleyItemId = (Enums.SDVObject)ParentSheetIndex;
            }
            this.amount = AmountNeeded;
        }

        public JsonItemReference(Enums.SDVObject objectId, int AmountNeeded) : this()
        {
            this.stardewValleyItemId = objectId;
            this.amount = AmountNeeded;
        }

        public JsonItemReference(Enums.SDVBigCraftable objectId, int AmountNeeded) : this()
        {
            this.stardewValleyBigCraftableId = objectId;
            this.amount = AmountNeeded;
        }

        public JsonItemReference(string registeredObjectId, int AmountNeeded) : this()
        {
            this.registeredObjectId = registeredObjectId;
            this.amount = AmountNeeded;
        }

        /// <summary>
        /// Gets the item that is referenced by this crafting component.
        /// </summary>
        /// <returns></returns>
        public virtual Item getItem()
        {
            if (this.stardewValleyItemId != Enums.SDVObject.NULL)
            {
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.stardewValleyItemId, this.amount);
            }
            if (this.stardewValleyBigCraftableId != Enums.SDVBigCraftable.NULL)
            {
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.stardewValleyBigCraftableId, this.amount);
            }
            if (!string.IsNullOrEmpty(this.registeredObjectId))
            {
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.registeredObjectId, this.amount);
            }
            return null;
        }
    }
}
