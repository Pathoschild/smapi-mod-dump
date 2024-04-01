/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace ItemBags.Community_Center
{
    public class BundleItem
    {
        public BundleTask Task { get; }
        public string Id { get; }
        public int Quantity { get; }
        public ObjectQuality MinQuality { get; }
        public bool IsCompleted { get; set; }
        /// <summary>True if this Item requirement must be satisfied to complete the <see cref="BundleTask"/>.<para/>
        /// (Some tasks (such as "Exotic Foraging Bundle") give the player several options, like only needing to complete 5/9 items)</summary>
        public bool IsRequired { get { return Task.AreAllItemsRequired; } }

        public BundleItem(BundleTask Task, string Id, int Quantity, ObjectQuality MinQuality, bool IsCompleted)
        {
            this.Task = Task;
            this.Id = Id;
            this.Quantity = Quantity;
            this.MinQuality = MinQuality;
            this.IsCompleted = IsCompleted;
        }

        /// <param name="RawData">The raw data string from the game's bundle content. EX: "16 1 0".<para/>
        /// This format is described here: <see cref="https://stardewvalleywiki.com/Modding:Bundles"/></param>
        public BundleItem(BundleTask Task, string RawData)
        {
            this.Task = Task;
            List<string> Entries = RawData.Split(' ').ToList();
            this.Id = Entries[0];
            this.Quantity = int.Parse(Entries[1]);
            this.MinQuality = (ObjectQuality)int.Parse(Entries[2]);
            this.IsCompleted = false;
        }

        public Object ToObject()
        {
            return new Object(this.Id, 0, false, -1, (int)this.MinQuality);
        }
    }
}
