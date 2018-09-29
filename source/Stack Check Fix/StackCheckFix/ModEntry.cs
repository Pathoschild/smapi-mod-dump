using System.Linq;
using System.Reflection;
using Harmony;
using StardewModdingAPI;
using StardewValley;

namespace StackCheckFix
{
    internal class ModEntry: Mod
    {
        public override void Entry(IModHelper helper)
        {
            HarmonyInstance harmony = HarmonyInstance.Create("punyo.StackCheckFix");

            MethodInfo methodBase = typeof(Farmer).GetMethod("couldInventoryAcceptThisObject", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo methodPatcher = typeof(FarmerFix).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
            if (methodBase == null)
            {
                Monitor.Log("Base method null. What's wrong?");
                return;
            }
            if (methodPatcher == null)
            {
                Monitor.Log("Patcher method null. What's wrong?");
                return;
            }
            harmony.Patch(methodBase, new HarmonyMethod(methodPatcher), null);
            Monitor.Log($"Patched {methodBase.DeclaringType?.FullName}.{methodBase.Name} by {methodPatcher.DeclaringType?.FullName}.{methodPatcher.Name}");
        }
    }

    internal class FarmerFix
    {
        private static bool Prefix(Farmer __instance, ref bool __result, ref int index, ref int stack, ref int quality)
        {
            __result = CouldInventoryAcceptThisObject(__instance, index, stack, quality);
            return false;
        }

        private static bool CouldInventoryAcceptThisObject(Farmer farmer, int index, int stack, int quality = 0)
        {
            bool bigObject = index < 0;
            if (bigObject)
            {
                // Nagate index because in inventory, it should be positive number.
                index = -index;
            }

            for (int i = 0; i < farmer.maxItems.Value; i++)
            {
                Item item;
                if (i < farmer.Items.Count && farmer.Items[i] != null)
                {
                    item = farmer.Items[i];
                }
                else
                {
                    return true;
                }
                if(item is Object obj && obj.bigCraftable.Value == bigObject && obj.ParentSheetIndex == index && obj.Stack + stack <= obj.maximumStackSize() && obj.Quality == quality)
                {
                    return true;
                }
            }
            if (farmer.IsLocalPlayer && farmer.isInventoryFull() && !Game1.hudMessages.Any())
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }
            return false;
        }
    }
}
