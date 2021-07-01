/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StackSplitRedux.MenuHandlers;

namespace StackSplitRedux
    {
    static class OtherMods {
        //                         mod_ID    menu_class_name  type_of_handler
        private static readonly Dictionary<string, List<Tuple<string, Type>>> MenuAndHandlerByMods = new();
        private static void Add(string modUniqueID, params object[] args) {
            var lst = new List<Tuple<string, Type>>();
            for (int i = 0; i < args.Length; i += 2) lst.Add(new Tuple<string, Type>((string)args[i], (Type)args[i + 1]));
            MenuAndHandlerByMods[modUniqueID] = lst;
            }
        static OtherMods() {
            Add(
                // Other Mod's UniqueID
                "CJBok.ItemSpawner",
                // Pairwise arg: Menu classes to intercept, suitable handler
                "CJBItemSpawner.Framework.ItemMenu", typeof(ItemGrabMenuHandler)
                );
            // Repeat calls to Add() as necessary
            }
        public static IEnumerable<KeyValuePair<string, List<Tuple<string, Type>>>> AsEnumerable() => MenuAndHandlerByMods.AsEnumerable();
        }
    }
