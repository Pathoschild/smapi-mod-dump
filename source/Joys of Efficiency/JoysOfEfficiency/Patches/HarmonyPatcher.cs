using System;
using System.Reflection;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using StardewModdingAPI;
using StardewValley;

namespace JoysOfEfficiency.Patches
{
    using Player = Farmer;
    internal class HarmonyPatcher
    {
        public static bool Init()
        {
            IMonitor mon = InstanceHolder.Monitor;
            try
            {
                MethodInfo methodBase;
                MethodInfo methodPatcher;
                {
                    mon.Log("Started patching Farmer");
                    methodBase = typeof(Player).GetMethod("hasItemInInventory", BindingFlags.Instance | BindingFlags.Public);
                    methodPatcher = typeof(FarmerPatcher).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
                    mon.Log("Trying to patch...");
                    if (!HarmonyHelper.Patch(methodBase, methodPatcher))
                    {
                        return false;
                    }
                }
                {
                    mon.Log("Started patching CraftingRecipe");
                    methodBase = typeof(CraftingRecipe).GetMethod("consumeIngredients", BindingFlags.Instance | BindingFlags.Public);
                    methodPatcher = typeof(CraftingRecipePatcher).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
                    mon.Log("Trying to patch...");
                    if (!HarmonyHelper.Patch(methodBase, methodPatcher))
                    {
                        return false;
                    }
                }
            }
            catch(Exception e)
            {
                InstanceHolder.Monitor.Log(e.ToString(), LogLevel.Error);
                return false;
            }
            return true;
        }
    }
}
