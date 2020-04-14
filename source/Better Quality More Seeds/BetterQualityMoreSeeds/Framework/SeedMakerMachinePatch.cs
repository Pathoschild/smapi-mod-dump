using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using SObject = StardewValley.Object;
using Netcode;
using System.ComponentModel;
using StardewValley.Menus;

namespace BetterQualityMoreSeeds.Framework
{
    class SeedMakerMachinePatch
    {
        private static IMonitor Monitor;
        private static Type MachineType;
        private static IReflectionHelper Reflection;

        //Call Method from Entry Class
        public static void Initialize(IMonitor monitor, Type machine, IReflectionHelper reflection)
        {
            Monitor = monitor;
            MachineType = machine;
            Reflection = reflection;
        }

        public static void SetInputPrefix(object __instance, object input, out KeyValuePair<SObject, SObject> __state)
        {
            __state = new KeyValuePair<SObject, SObject>(null, null);
            Type IStorageType = __instance.GetType().Assembly.GetType("Pathoschild.Stardew.Automate.IStorage");
            Type ITrackedStack = __instance.GetType().Assembly.GetType("Pathoschild.Stardew.Automate.ITrackedStack");
            Type IConsumable = __instance.GetType().Assembly.GetType("Pathoschild.Stardew.Automate.IConsumable");


            MethodInfo Instance_IsValidCrop = __instance.GetType().GetMethod("IsValidCrop");

            Func<dynamic, bool> func = x => (bool)Instance_IsValidCrop.Invoke(__instance, new object[] { x });
            Type[] TryGetIngredientParams = new Type[] { func.GetType(), typeof(int), IConsumable.MakeByRefType() };

            MethodInfo IStorage_TryGetIngredient = input.GetType().GetMethod("TryGetIngredient", TryGetIngredientParams);

            object[] TryGetIngredientArgs = new object[] { func, 1, null };

            bool ret = (bool)IStorage_TryGetIngredient.Invoke(input, TryGetIngredientArgs );

            if(ret)
            {

                //Save the State
                SObject Machine = (SObject)(MachineType.GetProperty("Machine", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));
                SObject crop = (SObject)IConsumable.GetProperty("Sample").GetValue(TryGetIngredientArgs[2]);

                __state = new KeyValuePair<SObject, SObject>(Machine, crop);
            }

        }

        public static void SetInputPostfix(KeyValuePair<SObject, SObject> __state)
        {
            PatchCommon.PostFix(__state, Monitor);
        }

        private static string GetQualityName(NetInt quality)
        {
            switch (quality.Value)
            {
                case 0: return "Normal";
                case 1: return "Silver";
                case 2: return "Gold";
                case 3: throw new InvalidEnumArgumentException();
                case 4: return "Iridium";
                default: throw new InvalidEnumArgumentException();

            }
        }
    }
}
