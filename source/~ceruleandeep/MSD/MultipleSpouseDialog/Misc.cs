/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

// cut down from the original in Multiple Spouses
// https://github.com/aedenthorn/StardewValleyMods/blob/master/MultipleSpouses/Misc.cs
// but still quite possible that there's code in here doing nothing

namespace MultipleSpouseDialog
{
    public static class Misc
    {
        private static IModHelper Helper;

        private static readonly Dictionary<string, Dictionary<string, string>> relationships = new();

        private static readonly string[] relativeRoles =
        {
            "son",
            "daughter",
            "sister",
            "brother",
            "dad",
            "mom",
            "father",
            "mother",
            "aunt",
            "uncle",
            "cousin",
            "nephew",
            "niece"
        };

        // call this method from your Entry class
        public static void Initialize(IModHelper helper)
        {
            Helper = helper;
        }

        private static Dictionary<string, NPC> GetSpouses(Farmer farmer, int all)
        {
            var spouses = new Dictionary<string, NPC>();
            if (all < 0)
            {
                var otherSpouse = farmer.getSpouse();
                if (otherSpouse != null) spouses.Add(otherSpouse.Name, otherSpouse);
            }

            foreach (var friend in farmer.friendshipData.Keys.Where(friend =>
                farmer.friendshipData[friend].IsMarried() && (all > 0 || friend != farmer.spouse)))
                spouses.Add(friend, Game1.getCharacterFromName(friend));

            return spouses;
        }

        public static void SetNPCRelations()
        {
            relationships.Clear();
            var NPCDispositions =
                Helper.GameContent.Load<Dictionary<string, string>>("Data\\NPCDispositions");
            foreach (var (key, value) in NPCDispositions)
            {
                var relations = value.Split('/')[9].Split(' ');
                if (relations.Length <= 0) continue;
                relationships.Add(key, new Dictionary<string, string>());
                for (var i = 0; i < relations.Length; i += 2)
                    try
                    {
                        relationships[key].Add(relations[i], relations[i + 1].Replace("'", ""));
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
            }
        }

        public static bool AreSpousesRelated(string npc1, string npc2)
        {
            if (relationships.ContainsKey(npc1) && relationships[npc1].ContainsKey(npc2))
            {
                if (relativeRoles.Any(r => relationships[npc1][npc2].Contains(r))) return true;
            }

            if (!relationships.ContainsKey(npc2) || !relationships[npc2].ContainsKey(npc1)) return false;
            return relativeRoles.Any(r => relationships[npc2][npc1].Contains(r));
        }

        public static void ResetSpouses(Farmer f)
        {
            var spouses = GetSpouses(f, 1);
            if (f.spouse == null)
                if (spouses.Count > 0)
                    f.spouse = spouses.First().Key;

            foreach (var name in f.friendshipData.Keys)
            {
                if (f.friendshipData[name].IsEngaged())
                {
                    if (f.friendshipData[name].WeddingDate.TotalDays < new WorldDate(Game1.Date).TotalDays)
                        f.friendshipData[name].WeddingDate.TotalDays = new WorldDate(Game1.Date).TotalDays + 1;

                    if (f.spouse != name) f.spouse = name;
                }

                if (!f.friendshipData[name].IsMarried() || f.spouse == name) continue;
                if (f.spouse != null && f.friendshipData[f.spouse] != null &&
                    !f.friendshipData[f.spouse].IsMarried() && !f.friendshipData[f.spouse].IsEngaged())
                    f.spouse = name;

                f.spouse ??= name;
            }
        }

        public static void ShuffleList<T>(ref List<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = ModEntry.myRand.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}