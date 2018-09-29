using Netcode;
using StardewMods.ArchaeologyHouseContentManagementHelper.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Patches
{
    public class CouldInventoryAcceptThisObjectPatch : Patch
    {
        private static Farmer farmer;

        protected override PatchDescriptor GetPatchDescriptor()
        {
            return new PatchDescriptor(Game1.player.GetType(), "couldInventoryAcceptThisObject", new Type[] { typeof(int), typeof(int), typeof(int) });
        }

        public static bool Prefix(Farmer __instance)
        {
            farmer = __instance;

            return false;
        }

        public static bool Postfix(bool __result, int index, int stack, int quality = 0)
        {
            for (int index1 = 0; index1 < farmer.maxItems.Value; ++index1)
            {
                // Patch: If the object to check for is a [Lost Book], we pretend the inventory can always accept it.
                if (index == Constants.GAME_OBJECT_LOST_BOOK_ID)
                {
                    return true;
                }

                if (farmer.Items.Count > index1 && 
                    (farmer.Items[index1] == null || farmer.Items[index1] is Object && farmer.Items[index1].Stack + stack <= farmer.Items[index1].maximumStackSize() && (farmer.Items[index1] as Object).ParentSheetIndex == index && (farmer.Items[index1] as Object).Quality == quality))
                {
                    return true;
                }
            }

            if (farmer.IsLocalPlayer && farmer.isInventoryFull() && Game1.hudMessages.Count<HUDMessage>() == 0)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }

            return false;
        }
    }
}
