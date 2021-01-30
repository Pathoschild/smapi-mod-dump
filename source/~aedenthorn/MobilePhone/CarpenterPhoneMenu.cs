/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Linq;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace MobilePhone
{
    internal class CarpenterPhoneMenu : CarpenterMenu
    {
        private IModHelper helper;
        private LocationRequest locationRequest;

        public CarpenterPhoneMenu(bool magicalConstruction, Farmer farmer, IModHelper helper) : base(magicalConstruction)
        {
            this.helper = helper;
            exitFunction = OnExit;

        }

        private void OnExit()
        {
            MobilePhoneCall.ShowMainCallDialogue(ModEntry.callingNPC);
        }
    }
}