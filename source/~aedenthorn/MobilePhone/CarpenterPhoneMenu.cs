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
using StardewValley.Menus;

namespace MobilePhone
{
    internal class CarpenterPhoneMenu : CarpenterMenu
    {
        private IModHelper helper;

        public CarpenterPhoneMenu(bool magicalConstruction, Farmer farmer, IModHelper helper) : base(magicalConstruction)
        {
            this.helper = helper;
            exitFunction = OnExit;

        }

        public void OnExit()
        {
            MobilePhoneCall.ShowMainCallDialogue(ModEntry.callingNPC);
        }
    }
}