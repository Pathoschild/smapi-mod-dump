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
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Netcode;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Objects;
using static Omegasis.Revitalize.Framework.Constants.Enums;
using static StardewValley.Object;

namespace Omegasis.Revitalize.Framework.World.Objects.Items.Utilities
{
    /// <summary>
    /// Class that references different types of artisinal goods made in the game.
    /// </summary>
    public class ArtisanGoodItemReference : StardustCore.Networking.NetObject
    {
        [JsonIgnore]
        public readonly NetEnum<SDVPreserveType> preserveType = new NetEnum<SDVPreserveType>(SDVPreserveType.NULL);
        [JsonIgnore]
        public readonly NetString preservedRegisteredObjectId = new NetString("");
        /// <summary>
        /// The type of object that is preserved such as Wine or Jelly.
        /// </summary>
        [JsonProperty("preserveType")]
        public SDVPreserveType PreserveType
        {
            get
            {
                return this.preserveType.Value;
            }
            set
            {
                this.preserveType.Value = value;
            }
        }

        /// <summary>
        /// The id of the item that was used to preserve the item. This will usually be something like apples, or BlueJazz. <see cref="getPreservedObjectTypeRegisteredObjectId"/> for the id of the object "container" for the preserved good.
        /// </summary>
        [JsonProperty("preservedRegisteredObjectId")]
        public string PreservedRegisteredObjectId
        {
            get
            {
                return this.preservedRegisteredObjectId.Value;
            }
            set
            {
                this.preservedRegisteredObjectId.Value = value;
            }
        }

        [JsonIgnore]
        public bool dynamicallyGenerated;
        [JsonIgnore]
        public string objectDisplayName;
        [JsonIgnore]
        public int basePrice;
        [JsonIgnore]
        public string preserveTypeObjectId;

        public ArtisanGoodItemReference()
        {

        }

        public ArtisanGoodItemReference(SDVObject PreservedRegisteredObjectId, PreserveType PreserveType) : this(Revitalize.RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(PreservedRegisteredObjectId), Enum.Parse<SDVPreserveType>(((int)PreserveType).ToString()))
        {

        }

        public ArtisanGoodItemReference(int PreservedRegisteredObjectId, PreserveType PreserveType) : this(Revitalize.RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(PreservedRegisteredObjectId),Enum.Parse<SDVPreserveType>(((int)PreserveType).ToString()))
        {

        }

        public ArtisanGoodItemReference(string ObjectId, SDVPreserveType PreserveType)
        {
            this.PreservedRegisteredObjectId = ObjectId;
            this.PreserveType = PreserveType;
        }

        public ArtisanGoodItemReference(string ObjectDisplayName, int BaseOrFinalizedPrice, SDVPreserveType PreserveType):this(ObjectDisplayName,BaseOrFinalizedPrice, GetPreservedObjectTypeRegisteredObjectId(PreserveType))
        {
            this.dynamicallyGenerated= true;

            this.objectDisplayName= ObjectDisplayName;
            this.basePrice= BaseOrFinalizedPrice;

            this.PreservedRegisteredObjectId = "";
            this.PreserveType = PreserveType;
        }

        public ArtisanGoodItemReference(string ObjectDisplayName, int BaseOrFinalizedPrice, string PreserveTypeObjectId)
        {
            this.dynamicallyGenerated = true;

            this.objectDisplayName = ObjectDisplayName;
            this.basePrice = BaseOrFinalizedPrice;

            this.PreservedRegisteredObjectId = "";
            this.preserveTypeObjectId = PreserveTypeObjectId;
        }



        protected override void initializeNetFields()
        {
            base.initializeNetFields();
            this.NetFields.AddFields(this.preserveType, this.preservedRegisteredObjectId);
        }

        public virtual void clearItemReference()
        {
            this.PreservedRegisteredObjectId = "";
            this.PreserveType = SDVPreserveType.NULL;
        }

        public virtual bool isNotNull()
        {
            if (this.dynamicallyGenerated)
            {
                return true;
            }
            return !string.IsNullOrEmpty(this.PreservedRegisteredObjectId) && this.PreserveType != SDVPreserveType.NULL;
        }

        public virtual Item getItem()
        {
            return this.getItem(1);
        }

        public virtual Item getItem(int Stack=1)
        {

            if (this.PreserveType == SDVPreserveType.NULL && string.IsNullOrEmpty(this.preserveTypeObjectId))
            {
                throw new Exception("Preserve type can not be null for Artisan Good Item Reference!");
                return null;
            }

            return this.getPreserveItem();
        }

        public virtual Item getPreserveItem()
        {
            StardewValley.Object obj = null;
            StardewValley.Object preservedObjectType = null;
            int preserveObjectPrice = 0;
            string displayName = "";

            if (this.dynamicallyGenerated)
            {
                preserveObjectPrice = this.basePrice;
                displayName = this.objectDisplayName;
            }
            else
            {
                preservedObjectType= RevitalizeModCore.ModContentManager.objectManager.getObject<StardewValley.Object>(this.PreservedRegisteredObjectId);
                preserveObjectPrice= preservedObjectType.Price;
                displayName = preservedObjectType.DisplayName;
            }

            if (!string.IsNullOrEmpty(this.getPreservedObjectTypeRegisteredObjectId()))
            {
                obj = RevitalizeModCore.ModContentManager.objectManager.getObject<StardewValley.Object>(this.getPreservedObjectTypeRegisteredObjectId());
                obj.Price = preserveObjectPrice;
            }

            if (this.PreserveType == SDVPreserveType.AgedRoe)
            {
                obj = new ColoredObject((int)SDVObject.AgedRoe, 1, Color.Orange);
                obj.Price = 60 + preserveObjectPrice;
            }
            if (this.PreserveType == SDVPreserveType.Honey)
            {
                obj = (StardewValley.Object)new ItemReference(SDVObject.Honey, 1).getItem();
                if (this.PreservedRegisteredObjectId.Equals("-1") || string.IsNullOrEmpty(this.PreservedRegisteredObjectId))
                {
                    if (obj.Name == "Honey")
                    {
                        obj.Name = "Wild Honey";
                    }
                    obj.Name = Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12750");
                    return obj;
                }

                //Base honey sell price + flower sell price * 2;
                obj.Price = obj.Price + preserveObjectPrice * 2;
                obj.DisplayName = displayName + " " + obj.DisplayName;
                obj.Name = displayName + " " + obj.Name;
            }
            if (this.PreserveType == SDVPreserveType.Jelly)
            {
                obj = (StardewValley.Object)new ItemReference(SDVObject.Jelly, 1).getItem();
                obj.Price = 50 + preserveObjectPrice * 2;
            }
            if (this.PreserveType == SDVPreserveType.Juice)
            {
                obj = (StardewValley.Object)new ItemReference(SDVObject.Juice, 1).getItem();
                obj.Price = (int)(preserveObjectPrice * 2.25f);
            }
            if (this.PreserveType == SDVPreserveType.Pickle)
            {
                obj = (StardewValley.Object)new ItemReference(SDVObject.Pickles, 1).getItem();
                obj.Price = 50 + (preserveObjectPrice * 2);
            }
            if (this.PreserveType == SDVPreserveType.Roe)
            {
                obj = new ColoredObject((int)SDVObject.Roe, 1, preservedObjectType!=null? StardewValley.Menus.TailoringMenu.GetDyeColor(preservedObjectType) ?? Color.Orange:Color.Orange);
                obj.Price = 30 + (preserveObjectPrice / 2);
            }
            if (this.PreserveType == SDVPreserveType.Wine)
            {
                obj = (StardewValley.Object)new ItemReference(SDVObject.Wine, 1).getItem();
                obj.Price = preserveObjectPrice * 3;
            }

            obj.preservedParentSheetIndex.Value = RevitalizeModCore.ModContentManager.objectManager.convertSDVStringObjectIdToIntObjectId(this.PreservedRegisteredObjectId);
            (obj as StardewValley.Object).preserve.Value = (PreserveType?)(int)this.PreserveType;

            obj.DisplayName = displayName + " " + obj.DisplayName;
            obj.Name = displayName + " " + obj.Name;


            return obj;
        }


        public virtual bool equalsOtherArtisinalGood(ArtisanGoodItemReference other)
        {
            if (this.dynamicallyGenerated)
            {
                return this.objectDisplayName.Equals(other.objectDisplayName) && this.basePrice.Equals(other.basePrice);
            }

            return this.PreservedRegisteredObjectId.Equals(other.PreservedRegisteredObjectId) && this.PreserveType == other.PreserveType;
        }

        /// <summary>
        /// Gets the ObjectId for the type of object that is preserved. I.E honey or wine.
        /// </summary>
        /// <returns></returns>
        public virtual string getPreservedObjectTypeRegisteredObjectId()
        {
            if (!string.IsNullOrEmpty(this.preserveTypeObjectId))
            {
                return this.preserveTypeObjectId;
            }
            string preserveObjectTypeId = GetPreservedObjectTypeRegisteredObjectId(this.PreserveType);
            if (!string.IsNullOrEmpty(preserveObjectTypeId))
            {
                return preserveObjectTypeId;
            }
            return "";
        }

        public static string GetPreservedObjectTypeRegisteredObjectId(SDVPreserveType PreserveType)
        {
            if (PreserveType == SDVPreserveType.NULL)
            {
                return null;
            }
            if (PreserveType == SDVPreserveType.AgedRoe)
            {
                return RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(SDVObject.AgedRoe);
            }
            if (PreserveType == SDVPreserveType.Honey)
            {
                return RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(SDVObject.Honey);
            }
            if (PreserveType == SDVPreserveType.Jelly)
            {
                return RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(SDVObject.Jelly);
            }
            if (PreserveType == SDVPreserveType.Juice)
            {
                return RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(SDVObject.Juice);
            }
            if (PreserveType == SDVPreserveType.Pickle)
            {
                return RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(SDVObject.Pickles);
            }
            if (PreserveType == SDVPreserveType.Roe)
            {
                return RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(SDVObject.Roe);
            }
            if (PreserveType == SDVPreserveType.Wine)
            {
                return RevitalizeModCore.ModContentManager.objectManager.createVanillaObjectId(SDVObject.Wine);
            }
            return "";
        }
    }
}
