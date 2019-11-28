using System;
using System.Reflection;
using Harmony;

namespace JoysOfEfficiency.Utils
{
    public class HarmonyHelper
    {
        private const string UniqueId = "punyo.JOE";
        private  static readonly HarmonyInstance Harmony = HarmonyInstance.Create(UniqueId);

        private static readonly Logger Logger = new Logger("HarmonyHelper");

        public static bool Patch(MethodInfo methodObjective, MethodInfo methodPatcher)
        {
            try
            {
                if (methodObjective == null)
                {
                    Logger.Error("Object method is null.");
                    return false;
                }

                if (methodPatcher == null)
                {
                    Logger.Error("Patcher method is null.");
                    return false;
                }

                Harmony.Patch(methodObjective, new HarmonyMethod(methodPatcher));
                Logger.Log($"Method:{GetMethodString(methodObjective)} has been patched by {GetMethodString(methodPatcher)}");

                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"An Exception Occured: {e}");
                return false;
            }
        }

        private static string GetMethodString(MethodBase method)
        {
            return $"{method.DeclaringType?.FullName}::{method.Name}";
        }
    }
}
