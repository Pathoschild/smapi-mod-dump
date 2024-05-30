/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

using StardewValley.GameData.GarbageCans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using LogLevel = StardewModdingAPI.LogLevel;

namespace GarbageCanTweaks
{
    public partial class ModEntry
    {
        public static List<string> packs = new();

        public static bool foundPacks = true;

        public static GarbageCanData garbageData = new();

        /// <summary>
        /// Initialize garbage can data content packs
        /// </summary>
        public static void Load()
        {
            if (!FindPacks(out var list))
            {
                Log("Failed to find any garbage packs.", LogLevel.Error);
                foundPacks = false;
                return;
            }
            if (list.Count - packs.Count > 0)
            {
                Log($"Loading {list.Count - packs.Count} packs.");
                packs.Clear();
                packs.AddRange(list);
            }
            foundPacks &= packs.Count > 0;
        }

        /// <summary>
        /// Reload garbage can data content packs and verify any changes
        /// </summary>
        public static void Reload()
        {
            if (!FindPacks(out var list))
            {
                Log("Failed to find any garbage lists.", LogLevel.Error);
                foundPacks = false;
                packs.Clear();
                return;
            }
            if (list.Count - packs.Count > 0)
            {
                packs.Clear();
                packs.AddRange(list);
                Log($"Loaded {list.Count - packs.Count} packs.");
            }
            else VerifyPacks(list);
            foundPacks &= packs.Count > 0;
        }

        /// <summary>
        /// Find any garbage can data content packs in /assets/
        /// </summary>
        /// <param name="list"></param>
        /// <returns>True if any garbage can data jsons were found, false if none were</returns>
        public static bool FindPacks( out List<string> list)
        {
            list = new List<string>();
            string assets = Path.Combine(SHelper.DirectoryPath, "assets");
            foreach (string fpath in Directory.GetFiles(assets))
            {
                string fname = (Path.GetFileName(fpath).EndsWith(".json")) ? Path.GetFileNameWithoutExtension(fpath) : "";
                if (!string.IsNullOrEmpty(fname))
                {
                    list.Add(fname);
                    Log($"found {fname}", debugOnly: true);
                }
            }

            return list.Count > 0;
        }

        /// <summary>
        /// Verify content of garbage can data packs if <see cref="ModEntry.Reload"/> found packs were removed and consolidate the list
        /// </summary>
        /// <param name="list"></param>
        public static void VerifyPacks(List<string> list)
        {
            int removed = 0;
            for (int i = 0; i < Math.Max(list.Count, packs.Count); i++)
            {
                if (!list[i].Equals(packs[i + removed]))
                {
                    Log($"{packs[i]} was removed or replaced", debugOnly: true);
                    removed++;
                    i--;
                }
            }
            packs.Clear();
            packs.AddRange(list);
            Log($"Garbage packs were verified! Found changes in {removed} files.", LogLevel.Info);
        }
    }
}
