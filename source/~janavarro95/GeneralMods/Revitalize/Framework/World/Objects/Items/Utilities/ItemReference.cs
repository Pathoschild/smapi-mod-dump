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
using Netcode;
using Newtonsoft.Json;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Items.Utilities
{
    /// <summary>
    /// Used to reference the many types of items that can be used. For a class that can be used anywhere that wraps the ObjectManager for easier use, use the <see cref="ItemReference"/> class instead.
    ///
    /// TODO: Add in support for quality?
    /// TODO: Add in support for colored objects?
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Items.Utilities.ItemReference")]
    public class ItemReference : StardustCore.Networking.NetObject
    {
        [JsonIgnore]
        /// <summary>
        /// The default stack size for getting the item when using <see cref="getItem()"/>
        /// </summary>
        public readonly NetInt stackSize = new NetInt(1);
        [JsonIgnore]
        public readonly NetString registeredObjectId = new NetString("");

        [JsonIgnore]
        public readonly NetInt quality= new NetInt(0);

        [JsonIgnore]
        public readonly NetEnum<Enums.SDVObject> sdvObjectId = new NetEnum<Enums.SDVObject>(Enums.SDVObject.NULL);
        [JsonIgnore]
        public readonly NetEnum<Enums.SDVBigCraftable> sdvBigCraftableId = new NetEnum<Enums.SDVBigCraftable>(Enums.SDVBigCraftable.NULL);

        [JsonIgnore]
        public readonly NetRef<ArtisanGoodItemReference> artisanGoodItemReference = new NetRef<ArtisanGoodItemReference>();

        [JsonProperty("amount")]
        public virtual int StackSize
        {
            get
            {
                return this.stackSize.Value;
            }
            set
            {
                this.stackSize.Value = value;
            }
        }

        /// <summary>
        /// The item id that should be compared to all item's ids.
        /// </summary>
        [JsonProperty("registeredObjectId")]
        public virtual string RegisteredObjectId
        {
            get
            {
                if (this.ArtisanGoodItemReference!=null && !string.IsNullOrEmpty(this.ArtisanGoodItemReference.getPreservedObjectTypeRegisteredObjectId()))
                {
                    return this.ArtisanGoodItemReference.getPreservedObjectTypeRegisteredObjectId();
                }

                if (string.IsNullOrEmpty(this.registeredObjectId.Value))
                {
                    if(this.StardewValleyItemId!= Enums.SDVObject.NULL)
                    {
                        return RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(this.StardewValleyItemId);
                    }
                    if (this.StardewValleyBigCraftableId != Enums.SDVBigCraftable.NULL)
                    {
                        return RevitalizeModCore.ModContentManager.objectManager.createVanillaBigCraftableId(this.StardewValleyBigCraftableId);
                    }
                }

                return this.registeredObjectId.Value;
            }
            set
            {
                this.registeredObjectId.Value = value;
            }
        }

        [JsonProperty("quality")]
        public virtual int Quality
        {
            get
            {
                return this.quality.Value;
            }
            set
            {
                this.quality.Value = value;
            }
        }

        [JsonProperty("stardewValleyItemId")]
        public virtual Enums.SDVObject StardewValleyItemId
        {
            get
            {
                return this.sdvObjectId.Value;
            }
            set
            {
                this.sdvObjectId.Value = value;
            }
        }

        [JsonProperty("stardewValleyBigCraftableId")]
        public virtual Enums.SDVBigCraftable StardewValleyBigCraftableId
        {
            get
            {
                return this.sdvBigCraftableId.Value;
            }
            set
            {
                this.sdvBigCraftableId.Value = value;
            }
        }

        [JsonProperty("artisanGoodItemReference")]
        public virtual ArtisanGoodItemReference ArtisanGoodItemReference
        {
            get
            {
                return this.artisanGoodItemReference.Value;
            }
            set
            {
                this.artisanGoodItemReference.Value = value;
            }
        }
        


        public ItemReference()
        {

        }

        public ItemReference(string ObjectId, int StackSize = 1, int Quality=0)
        {
            this.setItemReference(ObjectId, StackSize,Quality);
        }

        public ItemReference(Enums.SDVObject ObjectId, int StackSize = 1, int Quality=0)
        {
            this.setItemReference(ObjectId, StackSize,Quality);
        }

        public ItemReference(Enums.SDVBigCraftable ObjectId, int StackSize = 1, int Quality=0)
        {
            this.setItemReference(ObjectId, StackSize,Quality);
        }

        /// <summary>
        /// Attempts to convert an item into an item reference!
        /// </summary>
        /// <param name="item"></param>
        public ItemReference(Item item)
        {
            this.setItemReference(item);
        }

        /// <summary>
        /// Attempts to convert an item into an item reference!
        /// </summary>
        /// <param name="item"></param>
        public ItemReference(Item item, int StackSize, int Quality=0)
        {
            this.setItemReference(item);
            this.StackSize = StackSize;
            this.Quality= Quality;
        }

        /// <summary>
        /// Attempts to convert an item that is being sold into an item reference.
        /// </summary>
        /// <param name="salableItem"></param>
        public ItemReference(ISalable salableItem)
        {
            if(salableItem is StardewValley.Item)
            {
                this.setItemReference((StardewValley.Item)salableItem);
            }
        }

        public ItemReference(ArtisanGoodItemReference artisanGoodItemReference, int Stack=1, int Quality=0)
        {
            this.ArtisanGoodItemReference = artisanGoodItemReference;
            this.StackSize = Stack;
            this.Quality = Quality;
        }

        protected override void initializeNetFields()
        {
            base.initializeNetFields();
            this.NetFields.AddFields(this.stackSize, this.registeredObjectId, this.quality ,this.sdvObjectId, this.sdvBigCraftableId,this.artisanGoodItemReference);
        }

        public virtual void setItemReference(string ObjectId, int StackSize = 1, int Quality=0)
        {
            this.registeredObjectId.Value = ObjectId;
            this.stackSize.Value = StackSize;
            this.Quality= Quality;
        }

        public virtual void setItemReference(Enums.SDVObject ObjectId, int StackSize = 1, int Quality = 0)
        {
            this.sdvObjectId.Value = ObjectId;
            this.stackSize.Value = StackSize;
            this.Quality = Quality;
        }

        public virtual void setItemReference(Enums.SDVBigCraftable ObjectId, int StackSize = 1, int Quality = 0)
        {
            this.sdvBigCraftableId.Value = ObjectId;
            this.stackSize.Value = StackSize;
            this.Quality = Quality;
        }

        public virtual void setItemReference(Item item)
        {
            if (item == null)
            {
                this.clearItemReference();
                return;
            }

            if (item is IBasicItemInfoProvider)
            {
                string id = (item as IBasicItemInfoProvider).Id;
                this.registeredObjectId.Value = id;
            }

            //TODO: Replace the following section with just using the Item's Id field once SDV 1.6 is released.
            else if (item.GetType().Equals(typeof(StardewValley.Objects.Furniture)) || item.GetType().Equals(typeof(StardewValley.Objects.BedFurniture)) || item.GetType().Equals(typeof(StardewValley.Objects.FishTankFurniture)) || item.GetType().Equals(typeof(StardewValley.Objects.StorageFurniture)))
            {
                //TODO: Maybe add in references for furniture types?
                return;
            }
            else if (item is StardewValley.Object)
            {
                StardewValley.Object obj = (item as StardewValley.Object);

                if (obj.bigCraftable.Value)
                {
                    this.StardewValleyBigCraftableId = (Enums.SDVBigCraftable)obj.ParentSheetIndex;
                }
                else
                {
                    this.StardewValleyItemId = (Enums.SDVObject)item.ParentSheetIndex;
                    //Note that this is changed to a string in SDV 1.6, so this will break with that update. Change the condition to check against a string of "-1" instead.
                    if (obj.preservedParentSheetIndex.Value !=-1 && obj.preserve.Value.HasValue)
                    {
                        this.ArtisanGoodItemReference = new ArtisanGoodItemReference(obj.ParentSheetIndex, obj.preserve.Value.Value);
                    }
                    
                }
                this.Quality = obj.Quality;
            }
            else if (item is StardewValley.Tool)
            {
                //Don't really need item references for tools as far as I'm aware of. Skipping it.
                return;
            }
            else
            {
                throw new Exception("Item can not be cleanly converted to an item reference!");
            }
            //End TODO;

            this.stackSize.Value = item.Stack;
        }

        /// <summary>
        /// Checks to see if this item reference is null or not.
        /// </summary>
        /// <returns></returns>
        public virtual bool isNotNull()
        {
            return !string.IsNullOrEmpty(this.RegisteredObjectId);
        }

        /// <summary>
        /// Clears the fields for this item reference.
        /// </summary>
        public virtual void clearItemReference()
        {
            this.sdvObjectId.Value = Enums.SDVObject.NULL;
            this.sdvBigCraftableId.Value = Enums.SDVBigCraftable.NULL;
            this.registeredObjectId.Value = "";
            this.stackSize.Value = 1;
            this.Quality = 0;

            if (this.ArtisanGoodItemReference != null)
            {
                this.ArtisanGoodItemReference.clearItemReference();
            }
        }

        public virtual bool itemEquals(Item other, bool QualityMustMatch=false)
        {
            Item self = this.getItem();

            if (self == null && other == null) return true;
            if (self == null && other != null) return false;
            if (self != null && other == null) return false;

            if (!self.GetType().Equals(other.GetType())) return false;

            //Custom mod objects should have the same id.
            if (self is IBasicItemInfoProvider && (other is IBasicItemInfoProvider))
            {
                return (self as IBasicItemInfoProvider).Id.Equals((other as IBasicItemInfoProvider).Id);
            }

            if (self is StardewValley.Object && other is StardewValley.Object)
            {
                StardewValley.Object sObj = (self as StardewValley.Object);
                StardewValley.Object oObj = (other as StardewValley.Object);
                bool match= sObj.bigCraftable == oObj.bigCraftable && sObj.ParentSheetIndex == oObj.ParentSheetIndex && sObj.preservedParentSheetIndex.Value== oObj.preservedParentSheetIndex.Value && sObj.preserve.Value.Equals(oObj.preserve.Value);
                if (QualityMustMatch)
                {
                    match = sObj.Quality == oObj.Quality;
                }
                return match;
            }
            return false;

        }


        public virtual Item getItem(int StackSize = 1, int Quality=0)
        {
            if (this.isNotNull())
            {
                if (this.ArtisanGoodItemReference!=null && this.ArtisanGoodItemReference.isNotNull())
                {
                    return this.ArtisanGoodItemReference.getItem(StackSize);
                }
                return RevitalizeModCore.ModContentManager.objectManager.getItem(this.RegisteredObjectId, StackSize, Quality);
            }
            throw new InvalidObjectManagerItemException("An ItemReference must have one of the id fields set to be valid.");
        }

        public virtual Item getItem()
        {
            return this.getItem(this.StackSize, this.Quality);
        }

        public virtual ItemReference readItemReference(BinaryReader reader)
        {
            this.stackSize.Value = reader.ReadInt32();
            this.registeredObjectId.Value = reader.ReadString();
            this.Quality = reader.ReadInt32();
            this.sdvObjectId.Value = reader.ReadEnum<Enums.SDVObject>();
            this.sdvBigCraftableId.Value = reader.ReadEnum<Enums.SDVBigCraftable>();
            //TODO? REMOVE THIS?
            return this;
        }

        public virtual void writeItemReference(BinaryWriter writer)
        {
            writer.Write(this.stackSize.Value);
            writer.Write(this.registeredObjectId.Value);
            writer.Write(this.Quality);
            writer.WriteEnum<Enums.SDVObject>(this.sdvObjectId.Value);
            writer.WriteEnum<Enums.SDVBigCraftable>(this.sdvBigCraftableId.Value);
            //TODO? REMOVE THIS?

        }
    }
}
