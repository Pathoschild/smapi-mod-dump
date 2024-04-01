/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using StardewValley.GameData;
using System;
using System.Collections.Generic;

namespace SolidFoundations.Framework.Models.ContentPack
{
    public class ExtendedGenericSpawnItemDataWithCondition : GenericSpawnItemDataWithCondition
    {
        // TODO: Review these properties, convert to ModData     


        [Obsolete("Kept for backwards compatibility. Use PreserveId instead.")]
        public string PreserveID { set { PreserveId = value; } }
        public string PreserveId { get; set; }
        public string PreserveType { get; set; }


        [Obsolete("Kept for backwards compatibility. Use PriceModifiers instead.")]
        public int AddPrice { get; set; }
        [Obsolete("Kept for backwards compatibility. Use PriceModifiers instead.")]
        public float MultiplyPrice { get; set; } = 1f;

        public bool CopyPrice { get; set; }
        public List<QuantityModifier> PriceModifiers { get; set; }
        public QuantityModifier.QuantityModifierMode PriceModifierMode { get; set; }


        public string[] ModDataFlags { get; set; }
    }
}
