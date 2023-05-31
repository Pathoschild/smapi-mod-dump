/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models.Generic;
using System;

namespace Archery.Framework.Models.Crafting
{
    public class ShopModel : QueryableModel
    {
        public string Owner { get; set; }
        public string Context { get; set; }

        public int Stock { get; set; } = -1;
        internal int? RemainingStock { get; set; }
        public int Price { get; set; }

        internal int GetActualStock()
        {
            return RemainingStock is null ? Stock : RemainingStock.Value;
        }

        internal bool HasInfiniteStock()
        {
            return Stock == -1;
        }

        internal bool IsValid()
        {
            if (String.IsNullOrEmpty(Owner) && String.IsNullOrEmpty(Context))
            {
                return false;
            }

            if (HasInfiniteStock() is false)
            {
                if (Stock <= 0 || (RemainingStock is not null && RemainingStock <= 0))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
