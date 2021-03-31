/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/explosivetortellini/StardewValleyDRP
**
*************************************************/

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Discord
{
    public partial class StoreManager
    {
        public IEnumerable<Entitlement> GetEntitlements()
        {
            var count = CountEntitlements();
            var entitlements = new List<Entitlement>();
            for (var i = 0; i < count; i++)
            {
                entitlements.Add(GetEntitlementAt(i));
            }
            return entitlements;
        }

        public IEnumerable<Sku> GetSkus()
        {
            var count = CountSkus();
            var skus = new List<Sku>();
            for (var i = 0; i < count; i++)
            {
                skus.Add(GetSkuAt(i));
            }
            return skus;
        }
    }
}
