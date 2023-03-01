/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathoschild.Stardew.LookupAnything.Components;
using Pathoschild.Stardew.LookupAnything.Framework.Fields;
using Pathoschild.Stardew.LookupAnything.Framework.Lookups;
using Pathoschild.Stardew.LookupAnything.Framework.Lookups.Items;
using Newtonsoft.Json;

namespace LookupAnything_FTMIntegration
{
    internal static class LookupMenuPatches
    {
        static IModHelper Helper;
        public static void Init(Harmony harmony, IModHelper helper)
        {
            Helper = helper;

            harmony.Patch(
                original: AccessTools.Constructor(Type.GetType("Pathoschild.Stardew.LookupAnything.Components.LookupMenu,LookupAnything"), new Type[] { typeof(ISubject), typeof(IMonitor), typeof (IReflectionHelper), typeof(int), typeof(bool), typeof(bool), typeof(Action<ISubject>)}),
                postfix: new HarmonyMethod(typeof(LookupMenuPatches), nameof(cctr__Postfix))
            );
        }
        static void cctr__Postfix(LookupMenu __instance)
        {
            if (__instance.GetType() != typeof(LookupMenu)) { return; }
            var subject_r = Helper.Reflection.GetField<ISubject>(__instance, "Subject");
            var subject = subject_r.GetValue();
            if (subject is ItemSubject itemSubject)
            {
                var fields_r = Helper.Reflection.GetField<ICustomField[]>(__instance, "Fields");
                var fields = fields_r.GetValue();
                var item_r = Helper.Reflection.GetField<Item>(itemSubject, "Target");
                var item = item_r.GetValue();
                if (ModEntry.db.TryGetValue(item.ParentSheetIndex, out var ftmMetadata))
                {
                    fields_r.SetValue(fields.Append(new GenericField("SinZational Science", JsonConvert.SerializeObject(ftmMetadata, Formatting.Indented))).ToArray());
                }
            }
        }
    }
}
