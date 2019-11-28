using System;
using System.Reflection;
using JoysOfEfficiency.Utils;
using StardewValley;

namespace JoysOfEfficiency.Patches
{
    using Player = Farmer;
    internal class HarmonyPatcher
    {
        private static readonly Logger Logger = new Logger("HarmonyPatcher");

        public static bool Init()
        {
            try
            {
                MethodInfo methodBase;
                MethodInfo methodPatcher;
                {
                    Logger.Log("Started patching Farmer");
                    methodBase = typeof(Player).GetMethod("hasItemInInventory", BindingFlags.Instance | BindingFlags.Public);
                    methodPatcher = typeof(FarmerPatcher).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
                    Logger.Log("Trying to patch...");
                    if (!HarmonyHelper.Patch(methodBase, methodPatcher))
                    {
                        return false;
                    }
                }
                {
                    Logger.Log("Started patching CraftingRecipe");
                    methodBase = typeof(CraftingRecipe).GetMethod("consumeIngredients", BindingFlags.Instance | BindingFlags.Public);
                    methodPatcher = typeof(CraftingRecipePatcher).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
                    Logger.Log("Trying to patch...");
                    if (!HarmonyHelper.Patch(methodBase, methodPatcher))
                    {
                        return false;
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Error(e.ToString());
                return false;
            }
            return true;
        }
    }
}
