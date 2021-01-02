/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBags.Community_Center
{
    /// <summary>Represents a Room in the Community Center, and consists of 1 or more <see cref="BundleTask"/></summary>
    public class BundleRoom
    {
        public CommunityCenterBundles Bundles { get; }
        /// <summary>EX: "Crafts Room" (This value is always in English. See also: <see cref="DisplayName"/>)</summary>
        public string Name { get; }
        /// <summary>The name of the room, translated to the active language.</summary>
        public string DisplayName { get; }

        public ReadOnlyCollection<BundleTask> Tasks { get; }
        public bool IsCompleted { get { return Tasks.All(x => x.IsCompleted); } }

        public BundleRoom(CommunityCenterBundles Bundles, string Name, List<Tuple<int, string>> Tasks)
        {
            this.Bundles = Bundles;
            this.Name = Name;
            int RoomNumber = CommunityCenter.getAreaNumberFromName(Name);
            this.DisplayName = CommunityCenter.getAreaDisplayNameFromNumber(RoomNumber);
            this.Tasks = Tasks.Select(x => new BundleTask(this, x.Item1, x.Item2)).ToList().AsReadOnly();
        }
    }
}
