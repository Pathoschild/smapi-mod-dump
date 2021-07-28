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
using System.Threading.Tasks;

namespace PlatoTK.Patching
{
    internal class ConstructorPatches
    {
        internal static void HandleConstruction<TInstance>(TInstance __instance, MethodInfo __originalMethod, params object[] args)
        {
            foreach (TypeForwarding observer in HarmonyHelper.LinkedConstructors.Where(o => o.FromType == __instance.GetType()))
                if (observer.ToType.GetConstructor(__originalMethod.GetParameters().Select(p => p.ParameterType).ToArray()) is ConstructorInfo constructor)
                {
                    observer.Helper.SetTickDelayedUpdateAction(1, () =>
                    {
                        bool canLink = true;
                        if (observer.ToType.GetMethod("TypeCanLinkWith", BindingFlags.Public | BindingFlags.Static) is MethodInfo canLinkMethod)
                            canLink = (bool)canLinkMethod.Invoke(null, new object[] { __instance });

                        if (canLink && constructor.Invoke(args) is object newObject)
                        {
                            if (!observer.Helper.Harmony.TryGetLink(__instance, out object priorLink) || priorLink.GetType() != newObject.GetType())
                            {
                                if (observer.Helper.Harmony.LinkObjects(__instance, newObject) && newObject is IOnConstruction constructed)
                                    constructed.OnConstruction(observer.Helper, __instance);
                            }
                        }
                    });
                }
        }

        public static void ConstructorPatch<TInstance>(TInstance __instance, MethodInfo __originalMethod)
        {
            HandleConstruction(__instance, __originalMethod);
        }

        public static void ConstructorPatch<TInstance, T0>(TInstance __instance, MethodInfo __originalMethod,
           T0 __0
           )
        {
            HandleConstruction(__instance, __originalMethod,
                __0
                );
        }

        public static void ConstructorPatch<TInstance, T0, T1>(TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1
           )
        {
            HandleConstruction(__instance, __originalMethod,
                __0,
                __1
                );
        }

        public static void ConstructorPatch<TInstance, T0, T1, T2>(TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1,
           T2 __2
           )
        {
            HandleConstruction(__instance, __originalMethod,
                __0,
                __1,
                __2
                );
        }

        public static void ConstructorPatch<TInstance, T0, T1, T2, T3>(TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1,
           T2 __2,
           T3 __3
           )
        {
            HandleConstruction(__instance, __originalMethod,
                __0,
                __1,
                __2,
                __3
                );
        }

        public static void ConstructorPatch<TInstance, T0, T1, T2, T3, T4>(TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1,
           T2 __2,
           T3 __3,
           T4 __4
           )
        {
            HandleConstruction(__instance, __originalMethod,
                __0,
                __1,
                __2,
                __3,
                __4
                );
        }

        public static void ConstructorPatch<TInstance, T0, T1, T2, T3, T4, T5>(TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1,
           T2 __2,
           T3 __3,
           T4 __4,
           T5 __5
           )
        {
            HandleConstruction(__instance, __originalMethod,
                __0,
                __1,
                __2,
                __3,
                __4,
                __5
                );
        }

        public static void ConstructorPatch<TInstance, T0, T1, T2, T3, T4, T5, T6>(TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1,
           T2 __2,
           T3 __3,
           T4 __4,
           T5 __5,
           T6 __6
           )
        {
            HandleConstruction(__instance, __originalMethod,
                __0,
                __1,
                __2,
                __3,
                __4,
                __5,
                __6
                );
        }

        public static void ConstructorPatch<TInstance, T0, T1, T2, T3, T4, T5, T6, T7>(TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1,
           T2 __2,
           T3 __3,
           T4 __4,
           T5 __5,
           T6 __6,
           T7 __7
           )
        {
            HandleConstruction(__instance, __originalMethod,
                __0,
                __1,
                __2,
                __3,
                __4,
                __5,
                __6,
                __7
                );
        }

        public static void ConstructorPatch<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8>(TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1,
           T2 __2,
           T3 __3,
           T4 __4,
           T5 __5,
           T6 __6,
           T7 __7,
           T8 __8
           )
        {
            HandleConstruction(__instance, __originalMethod,
                __0,
                __1,
                __2,
                __3,
                __4,
                __5,
                __6,
                __7,
                __8
                );
        }
        public static void ConstructorPatch<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(TInstance __instance, MethodInfo __originalMethod,
           T0 __0,
           T1 __1,
           T2 __2,
           T3 __3,
           T4 __4,
           T5 __5,
           T6 __6,
           T7 __7,
           T8 __8,
           T9 __9
           )
        {
            HandleConstruction(__instance, __originalMethod,
                __0,
                __1,
                __2,
                __3,
                __4,
                __5,
                __6,
                __7,
                __8,
                __9
                );
        }

        public static void ConstructorPatch<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TInstance __instance, MethodInfo __originalMethod,
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
            HandleConstruction(__instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8, __9, __10);
        }

        public static void ConstructorPatch<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(TInstance __instance, MethodInfo __originalMethod,
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
            HandleConstruction(__instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8, __9, __10, __11);
        }

        public static void ConstructorPatch<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(TInstance __instance, MethodInfo __originalMethod,
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
            HandleConstruction(__instance, __originalMethod, __0, __1, __2, __3, __4, __5, __6, __7, __8, __9, __10, __11, __12);
        }
    }
}
