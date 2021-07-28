/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaphodil/BetterJukebox
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaphodil.BetterJukebox.Framework
{
    public class BetterJukeboxItem : IComparable<BetterJukeboxItem>
    {
        public string Name;
        public bool IsLocked;
        public int LockCount;
        public int ShakeTimer = 0;

        public BetterJukeboxItem(string name, bool isLocked = false, int lockCount = 0)
        {
            Name = name;
            IsLocked = isLocked;
            LockCount = lockCount;
        }

        public int CompareTo(BetterJukeboxItem itemB)
        {
            if (itemB is null)
                return 1;
            return Name.CompareTo(itemB.Name);
        }
        public bool Equals(BetterJukeboxItem item)
        {
            if (item is null)
                return false;
            return Name.Equals(item.Name);
        }
    }
}
