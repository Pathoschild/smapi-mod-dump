/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewValley;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;

namespace PlatoTK.Patching
{
    public class MethodPatches
    {
        internal static bool ForwardMethodVoid<TInstance>(TInstance __instance, MethodInfo __originalMethod, params object[] args)
        {
            object instance = __instance;
            Type iType = __instance.GetType();
            if (HarmonyHelper.TracedTypes.FirstOrDefault(t => t.FromType == iType.GetType() && t.TargetForAllInstances != null) is TypeForwarding allForward
                && AccessTools.DeclaredMethod(allForward.TargetForAllInstances.GetType(), __originalMethod.Name, __originalMethod
                .GetParameters()?.Select(p => p.ParameterType)?.ToArray() ?? new Type[0]) is MethodInfo targetMethod)
            {
                if (allForward.TargetForAllInstances is ILinked linkedTarget)
                    linkedTarget.Link = new Link(__instance, linkedTarget, allForward.Helper);


                targetMethod.Invoke(allForward.TargetForAllInstances, args);

                return false;
            }
            else if (HarmonyHelper.TracedObjects.FirstOrDefault(t => t.Original == instance) is TracedObject registred
                && AccessTools.DeclaredMethod(registred.Target.GetType(), __originalMethod.Name, __originalMethod.GetParameters()?.Select(p => p.ParameterType)?.ToArray() ?? new Type[0]) is MethodInfo targetMethod2)
            {
                if (registred.Target is ILinked linkedTarget)
                    linkedTarget.Link = new Link(__instance, linkedTarget, registred.Helper);

                targetMethod2.Invoke(registred.Target, args);

                return false;
            }

            return true;
        }

        internal static bool ForwardMethod<TResult, TInstance>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod, params object[] args)
        {
            object instance = __instance;
            Type iType = __instance.GetType();
            if (HarmonyHelper.TracedTypes.FirstOrDefault(t => t.FromType == iType.GetType() && t.TargetForAllInstances != null) is TypeForwarding allForward
                && AccessTools.DeclaredMethod(allForward.TargetForAllInstances.GetType(), __originalMethod.Name, __originalMethod
                .GetParameters()?.Select(p => p.ParameterType)?.ToArray() ?? new Type[0]) is MethodInfo targetMethod)
            {
                if (allForward.TargetForAllInstances is ILinked linkedTarget)
                    linkedTarget.Link = new Link(__instance, linkedTarget, allForward.Helper);

                __result = (TResult)targetMethod.Invoke(allForward.TargetForAllInstances, args);

                return false;
            }
            else if (HarmonyHelper.TracedObjects.FirstOrDefault(t => t.Original == instance) is TracedObject registred
                && AccessTools.DeclaredMethod(registred.Target.GetType(), __originalMethod.Name, __originalMethod.GetParameters()?.Select(p => p.ParameterType)?.ToArray() ?? new Type[0]) is MethodInfo targetMethod2)
            {
                if (registred.Target is ILinked linkedTarget)
                    linkedTarget.Link = new Link(__instance, linkedTarget, registred.Helper);

                __result = (TResult)targetMethod2.Invoke(registred.Target, args);

                return false;
            }

            return true;
        }

        //----------

        internal static bool ForwardMethodPatch<TResult, TInstance>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod);
        }

        internal static bool ForwardMethodPatch<TResult, TInstance, T0>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
            T0 __0)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0);
        }
        internal static bool ForwardMethodPatch<TResult, TInstance, T0, T1>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0, __1);
        }

        internal static bool ForwardMethodPatch<TResult, TInstance, T0, T1, T2>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
            T1 __0,
            T1 __1,
            T2 __2)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0, __1, __2);
        }

        internal static bool ForwardMethodPatch<TResult, TInstance, T0, T1, T2, T3>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0, __1, __2, __3);
        }

        internal static bool ForwardMethodPatch<TResult, TInstance, T0, T1, T2, T3, T4>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0, __1, __2, __3, __4);
        }

        internal static bool ForwardMethodPatch<TResult, TInstance, T0, T1, T2, T3, T4, T5>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0, __1, __2, __3, __4, __5);
        }

        internal static bool ForwardMethodPatch<TResult, TInstance, T0, T1, T2, T3, T4, T5, T6>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1,
           T2 __2,
           T3 __3,
           T4 __4,
           T5 __5,
           T6 __6)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6);
        }

        internal static bool ForwardMethodPatch<TResult, TInstance, T0, T1, T2, T3, T4, T5, T6, T7>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1,
           T2 __2,
           T3 __3,
           T4 __4,
           T5 __5,
           T6 __6,
           T7 __7)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7);
        }
        internal static bool ForwardMethodPatch<TResult, TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5,
            T6 __6,
            T7 __7,
            T8 __8)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8);
        }
        internal static bool ForwardMethodPatch<TResult, TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5,
            T6 __6,
            T7 __7,
            T8 __8,
            T9 __9)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8, __9);
        }
        internal static bool ForwardMethodPatch<TResult, TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref TResult __result, TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5,
            T6 __6,
            T7 __7,
            T8 __8,
            T9 __9,
            T10 __10)
        {
            return ForwardMethod(ref __result, __instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8, __9, __10);
        }


        //---------

        internal static bool ForwardMethodPatchVoid<TInstance>(TInstance __instance, MethodInfo __originalMethod)
        {
            return ForwardMethodVoid(__instance, __originalMethod);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0);
        }

        public static bool ForwardMethodPatchVoid<TInstance, T0, T1>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2, T3>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2, __3);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2, T3, T4>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2, __3, __4);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2, T3, T4, T5>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2, __3, __4, __5);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2, T3, T4, T5, T6>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5,
            T6 __6)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2, T3, T4, T5, T6, T7>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5,
            T6 __6,
            T7 __7)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5,
            T6 __6,
            T7 __7,
            T8 __8)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5,
            T6 __6,
            T7 __7,
            T8 __8,
            T9 __9)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8, __9);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5,
            T6 __6,
            T7 __7,
            T8 __8,
            T9 __9,
            T10 __10)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8, __9, __10);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5,
            T6 __6,
            T7 __7,
            T8 __8,
            T9 __9,
            T10 __10,
            T11 __11)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8, __9, __10, __11);
        }

        internal static bool ForwardMethodPatchVoid<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(TInstance __instance, MethodInfo __originalMethod,
            T0 __0,
            T1 __1,
            T2 __2,
            T3 __3,
            T4 __4,
            T5 __5,
            T6 __6,
            T7 __7,
            T8 __8,
            T9 __9,
            T10 __10,
            T11 __11,
            T12 __12)
        {
            return ForwardMethodVoid(__instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8, __9, __10, __11, __12);
        }

    }
}
