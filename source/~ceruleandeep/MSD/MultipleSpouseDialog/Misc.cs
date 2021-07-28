/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

// cut down from the original in Multiple Spouses
// https://github.com/aedenthorn/StardewValleyMods/blob/master/MultipleSpouses/Misc.cs
// but still quite possible that there's code in here doing nothing

namespace MultipleSpouseDialog
{
    public class Misc
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static ModConfig Config;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }

        public static Dictionary<string, NPC> GetSpouses(Farmer farmer, int all)
        {
            Dictionary<string, NPC> spouses = new Dictionary<string, NPC>();
            if (all < 0)
            {
                NPC ospouse = farmer.getSpouse();
                if (ospouse != null)
                {
                    spouses.Add(ospouse.Name, ospouse);
                }
            }
            foreach (string friend in farmer.friendshipData.Keys)
            {
                if (farmer.friendshipData[friend].IsMarried() && (all > 0 || friend != farmer.spouse))
                {
                    spouses.Add(friend, Game1.getCharacterFromName(friend, true));
                }
            }

            return spouses;
        }

        public static Dictionary<string, Dictionary<string, string>> relationships = new Dictionary<string, Dictionary<string, string>>();

        public static void SetNPCRelations()
        {
            relationships.Clear();
            Dictionary<string, string> NPCDispositions = Helper.Content.Load<Dictionary<string, string>>("Data\\NPCDispositions", ContentSource.GameContent);
            foreach (KeyValuePair<string, string> kvp in NPCDispositions)
            {
                string[] relations = kvp.Value.Split('/')[9].Split(' ');
                if (relations.Length > 0)
                {
                    relationships.Add(kvp.Key, new Dictionary<string, string>());
                    for (int i = 0; i < relations.Length; i += 2)
                    {
                        try
                        {
                            relationships[kvp.Key].Add(relations[i], relations[i + 1].Replace("'", ""));
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        public static string[] relativeRoles = new string[]
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
            "niece",
        };

        public static bool AreSpousesRelated(string npc1, string npc2)
        {
            if (relationships.ContainsKey(npc1) && relationships[npc1].ContainsKey(npc2))
            {
                string relation = relationships[npc1][npc2];
                foreach (string r in relativeRoles)
                {
                    if (relation.Contains(r))
                    {
                        return true;
                    }
                }
            }
            if (relationships.ContainsKey(npc2) && relationships[npc2].ContainsKey(npc1))
            {
                string relation = relationships[npc2][npc1];
                foreach (string r in relativeRoles)
                {
                    if (relation.Contains(r))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ResetSpouses(Farmer f)
        {
            Dictionary<string, NPC> spouses = GetSpouses(f, 1);
            if (f.spouse == null)
            {
                if (spouses.Count > 0)
                {
                    f.spouse = spouses.First().Key;
                }
            }

            foreach (string name in f.friendshipData.Keys)
            {
                if (f.friendshipData[name].IsEngaged())
                {
                    if (f.friendshipData[name].WeddingDate.TotalDays < new WorldDate(Game1.Date).TotalDays)
                    {
                        f.friendshipData[name].WeddingDate.TotalDays = new WorldDate(Game1.Date).TotalDays + 1;
                    }
                    if (f.spouse != name)
                    {
                        f.spouse = name;
                    }
                }
                if (f.friendshipData[name].IsMarried() && f.spouse != name)
                {
                    if (f.spouse != null && f.friendshipData[f.spouse] != null && !f.friendshipData[f.spouse].IsMarried() && !f.friendshipData[f.spouse].IsEngaged())
                    {
                        f.spouse = name;
                    }
                    if (f.spouse == null)
                    {
                        f.spouse = name;
                    }
                }
            }
        }

        public static void ShuffleList<T>(ref List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ModEntry.myRand.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void ShuffleDic<T1, T2>(ref Dictionary<T1, T2> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ModEntry.myRand.Next(n + 1);
                var value = list[list.Keys.ToArray()[k]];
                list[list.Keys.ToArray()[k]] = list[list.Keys.ToArray()[n]];
                list[list.Keys.ToArray()[n]] = value;
            }
        }
    }
}
