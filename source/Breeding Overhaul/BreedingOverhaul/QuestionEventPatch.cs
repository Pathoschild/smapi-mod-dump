/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StarAmy/BreedingOverhaul
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using System.Reflection;

namespace BreedingOverhaul
{
    public class QuestionEventPatch
    {
        internal static FieldInfo GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field;
        }
        internal static object GetInstanceFieldValue(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        public static bool setUpPrefix(StardewValley.Events.QuestionEvent __instance, ref int __state, ref bool __result)
        {
            ModEntry.MyMonitor.Log($"QuestionEvent setUp prefix", LogLevel.Trace);
            __state = new int();
            __state = 0;
            int? whichQuestion = GetInstanceFieldValue(typeof(StardewValley.Events.QuestionEvent), __instance, "whichQuestion") as int?;
            if (whichQuestion != null)
            {
                ModEntry.MyMonitor.Log($"QuestionEvent setUp prefix, whichQuestion is {whichQuestion}", LogLevel.Trace);
                __state = (int)whichQuestion;
                if (whichQuestion == 2)
                {
                    GetInstanceField(typeof(StardewValley.Events.QuestionEvent), __instance, "whichQuestion").SetValue(__instance, 0);
                    whichQuestion = GetInstanceFieldValue(typeof(StardewValley.Events.QuestionEvent), __instance, "whichQuestion") as int?;
                    ModEntry.MyMonitor.Log($"QuestionEvent setUp prefix, whichQuestion is now {whichQuestion}", LogLevel.Trace);
                }
            }
            return true;
        }

        public static void setUpPostfix(StardewValley.Events.QuestionEvent __instance, ref int __state, ref bool __result)
        {
            ModEntry.MyMonitor.Log($"QuestionEvent setUp postfix, state is {__state}", LogLevel.Trace);
            if (__state != 0)   
            {
                GetInstanceField(typeof(StardewValley.Events.QuestionEvent), __instance, "whichQuestion").SetValue(__instance, __state);
                int? whichQuestion = GetInstanceFieldValue(typeof(StardewValley.Events.QuestionEvent), __instance, "whichQuestion") as int?;
                ModEntry.MyMonitor.Log($"QuestionEvent setUp postfix, whichQuestion is now {whichQuestion}", LogLevel.Trace);
            }
        }
    }
}