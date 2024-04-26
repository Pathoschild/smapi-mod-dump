/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MobilePhone
{
    public class CarpenterPhoneMenu : CarpenterMenu
    {
        private IModHelper helper;

        public CarpenterPhoneMenu(bool magicalConstruction, Farmer farmer, IModHelper helper) : base(Game1.builder_robin)
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