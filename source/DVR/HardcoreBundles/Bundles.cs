using HardcoreBundles.Perks;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using C = HardcoreBundles.Constants;

namespace HardcoreBundles
{
    public class BundleModel
    {
        public string Name { get; set; }
        public string Room { get; set; }
        public int ID { get; set; }
        public int Reward { get; set; } = C.Diamond;
        public int RewardQty { get; set; } = 1;

        public string PerkDesc { get; set; } // mostly for docs
        public PerkBase Perk { get; set; } = new PerkBase();

        public BundleItem[] Items;

        public BundleModel(string room, string name, params BundleItem[] items)
        {
            Room = room;
            Name = name;
            Items = items;
        }

        public string Key()
        {
            return $"{Room}/{ID}";
        }

        private string desc() { 
            return ModEntry.Instance.Helper.Translation.Get(Name);
        }

        public string Resource()
        {
            var items = string.Join(" ", Items.Select(x => $"{x.ItemID} {x.Qty} {x.Quality}"));
            return $"{desc()}/O {Reward} {RewardQty}/{items}/{ID % 6}";
        }

        public string Markdown()
        {
            return $"## {desc()} Bundle:\n\n" +
                $"Reward: {Reward} {new Object(Reward,1).Name}\n\n" +
                $"Perk: {PerkDesc}\n\n" +
                $"Items: \n\n" +
                $"{string.Join("\n", Items.Select(x => x.Markdown()))}\n\n";
      
        }
    }

    public class BundleItem
    {
        public BundleItem(int id, int qty, int quality = 0)
        {
            ItemID = id;
            Qty = qty;
            Quality = quality;
        }
        public int ItemID { get; }
        public int Qty { get; }
        public int Quality { get; }

        private string quality()
        {
            switch (Quality)
            {
                case 0:
                    return "";
                case 1:
                    return "Silver ";
                case 2:
                    return "Gold ";
                case 4:
                    return "Iridium ";
            }
            return "ERROR";
        }
        public string Markdown()
        {
            var o = new StardewValley.Object(ItemID, 1);
            return $"- {Qty} {quality()}{o.Name}";
        }
    }

    public partial class Bundles
    {
        private const int Silver = 1;
        private const int Gold = 2;
        private const int Iridium = 4;

        private const string CraftRoom = "Crafts Room";
        private const string Pantry = "Pantry";
        private const string Bulletin = "Bulletin Board";
        private const string Boiler = "Boiler Room";
        private const string FishTank = "Fish Tank";
        private const string Vault = "Vault";

        

        static Bundles()
        {
            for (var i = 0; i < List.Count; i++)
            {
                List[i].ID = i;
                List[i].Perk.BundleID = i;
            }
        }

        public static IDictionary<string, string> Data()
        {
            return List.ToDictionary(x => x.Key(), x => x.Resource());
        }

        public static void SaveMarkdown([CallerFilePath] string sourceFilePath = "")
        {
            var md = string.Join("\n\n", List.Select(x => x.Markdown()));
            var f = Path.Combine(Path.GetDirectoryName(sourceFilePath), "bundles.md");
            File.WriteAllText(f, md);
        }

        public static void Fix(bool hard)
        {
            // this makes the "shape" of the game's internal bundle tracking representation match the bundles we have.
            var bundles = Game1.netWorldState.Value.Bundles;
            var rewards = Game1.netWorldState.Value.BundleRewards;
            if (hard)
            {
                bundles.Clear();
                rewards.Clear();
            }
            // remove anything in the existing collections that doesn't exist in our set.
            var desiredIds = List.Select(x => x.ID).ToList();
            foreach (var nuke in bundles.Keys.Where(x => !desiredIds.Contains(x)).ToList())
            {
                bundles.Remove(nuke);
            }
            foreach (var nuke in rewards.Keys.Where(x => !desiredIds.Contains(x)).ToList())
            {
                rewards.Remove(nuke);
            }

            // now add anything not in existing collections. 
            // making sure to adjust ingredient counts as needed (so we can ever update mod).
            foreach (var bun in List)
            {
                var count = bun.Items.Length;
                var id = bun.ID;
                if (!bundles.ContainsKey(id))
                {
                    bundles[id] = new bool[count];
                }
                else if (count != bundles[id].Count())
                {
                    // gotta make a new array and copy old values into it.
                    // Also can't just set new value. Gotta remove old array and re-add.
                    var b = new bool[count];
                    var old = bundles[id];
                    for (var i = 0; i < old.Length && i < count; i++)
                    {
                        b[i] = old[i];
                    }
                    bundles.Remove(id);
                    bundles[id] = b;
                }
                if (!rewards.ContainsKey(id))
                {
                    rewards[id] = false;
                }
            }
        }
    }
}
