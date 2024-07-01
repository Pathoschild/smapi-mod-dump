/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using StardewValley.GameData.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles.Lib.WalletCurrency
{
    public class WalletCurrencyModel
    {
        public string Id;
        public bool Shared = true;
        public bool HoldItemUpOnDiscovery = false;

        public bool DrawOverheadPickupAnimation = false;
        public string OverheadPickupTexture = "";
        public int OverheadPickupTextureSize = 16;
        public string OverheadPickupAnimation = "";

        public string PickupSound = "";
        public bool PlayMoneyRollSound = false;

        public string BillboardTexture = "";
        public int BillboardTextureSize = 16;
        public string BillboardAnimation = "";
        public int BillboardDigits = 3;

        public PowersData PowersData = new();
        public List<WalletCurrencyItem> Items = new();
    }
}
