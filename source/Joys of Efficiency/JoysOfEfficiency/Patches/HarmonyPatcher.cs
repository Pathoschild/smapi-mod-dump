using System;
using System.Reflection;
using Harmony;
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
            IMonitor Mon = Util.Monitor;
            try
            {
                HarmonyInstance harmony = HarmonyInstance.Create("punyo.JOE");
                MethodInfo methodBase;
                MethodInfo methodPatcher;
                {
                    Mon.Log("Started patching Farmer");
                    methodBase = typeof(Player).GetMethod("hasItemInInventory", BindingFlags.Instance | BindingFlags.Public);
                    methodPatcher = typeof(FarmerPatcher).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
                    Mon.Log("Trying to patch...");
                    harmony.Patch(methodBase, new HarmonyMethod(methodPatcher), null);
                    if (methodBase == null)
                    {
                        Mon.Log("Original method null, what's wrong?");
                        return false;
                    }
                    if (methodPatcher == null)
                    {
                        Mon.Log("Patcher null, what's wrong?");
                        return false;
                    }
                    Mon.Log($"Patched {methodBase.DeclaringType?.FullName}.{methodBase.Name} by {methodPatcher.DeclaringType?.FullName}.{methodPatcher.Name}");
                }
                {
                    Mon.Log("Started patching CraftingRecipe");
                    methodBase = typeof(CraftingRecipe).GetMethod("consumeIngredients", BindingFlags.Instance | BindingFlags.Public);
                    methodPatcher = typeof(CraftingRecipePatcher).GetMethod("Prefix", BindingFlags.Static | BindingFlags.NonPublic);
                    Mon.Log("Trying to patch...");
                    harmony.Patch(methodBase, new HarmonyMethod(methodPatcher), null);
                    if (methodBase == null)
                    {
                        Mon.Log("Original method null, what's wrong?");
                        return false;
                    }
                    if (methodPatcher == null)
                    {
                        Mon.Log("Patcher null, what's wrong?");
                        return false;
                    }
                    Mon.Log($"Patched {methodBase.DeclaringType?.FullName}.{methodBase.Name} by {methodPatcher.DeclaringType?.FullName}.{methodPatcher.Name}");
                }
            }
            catch(Exception e)
            {
                Util.Monitor.Log(e.ToString(), LogLevel.Error);
                return false;
            }
            return true;
        }
    }
}
