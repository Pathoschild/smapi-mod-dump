using System.Collections.Generic;
using TehPers.CoreMod.Api.Items;

namespace TehPers.CoreMod.ContentPacks.Data {
    // TODO: Use IContentPackValue instead and support tokens that change values
    internal class SObjectData : ItemData {
        /// <summary>The cost of the item at normal quality.</summary>
        public int Cost { get; set; } = 0;

        /// <summary>The item's edibility.</summary>
        public int Edibility { get; set; } = -300;

        /// <summary>The name of the category the item is in.</summary>
        public string CategoryName { get; set; } = Api.Items.Category.Trash.Name;

        /// <summary>The number associated with the category the item is in.</summary>
        public int CategoryNumber { get; set; } = Api.Items.Category.Trash.Index;

        /// <summary>The buffs the farmer gets when consuming this item.</summary>
        public BuffDescription? Buffs { get; set; } = null;
    }
}
