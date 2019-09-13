using Harmony;
using FelixDev.StardewMods.Common.StardewValley;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SObject = StardewValley.Object;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Patches
{
    public class CouldInventoryAcceptThisObjectPatch
    {
        private static Farmer farmer;

        /// <summary>Apply the Harmony patch.</summary>
        /// <param name="harmony">The Harmony instance.</param>
        public void Apply(HarmonyInstance harmony)
        {
            MethodBase method = AccessTools.Method(typeof(Farmer), "couldInventoryAcceptThisObject", new Type[] { typeof(int), typeof(int), typeof(int) });

            MethodInfo prefix = AccessTools.Method(this.GetType(), nameof(AddItemToInventoryBoolPatch.Prefix));
            MethodInfo postfix = AccessTools.Method(this.GetType(), nameof(AddItemToInventoryBoolPatch.Postfix));

            harmony.Patch(method, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
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
                if (index == Constants.ID_GAME_OBJECT_LOST_BOOK)
                {
                    return true;
                }

                if (farmer.Items.Count > index1 && 
                    (farmer.Items[index1] == null || farmer.Items[index1] is SObject && farmer.Items[index1].Stack + stack <= farmer.Items[index1].maximumStackSize() && (farmer.Items[index1] as SObject).ParentSheetIndex == index && (farmer.Items[index1] as SObject).Quality == quality))
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
