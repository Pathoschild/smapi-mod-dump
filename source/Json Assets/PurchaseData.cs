/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/JsonAssets
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonAssets
{
    public class PurchaseData
    {
        public int PurchasePrice { get; set; }
        public string PurchaseFrom { get; set; } = "Pierre";
        public IList<string> PurchaseRequirements { get; set; } = new List<string>();

        internal string GetPurchaseRequirementString()
        {
            if ( PurchaseRequirements == null )
                return "";
            var str = $"1234567890";
            foreach ( var cond in PurchaseRequirements )
                str += $"/{cond}";
            return str;
        }
    }
}
