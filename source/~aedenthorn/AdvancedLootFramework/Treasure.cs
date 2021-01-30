/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

namespace AdvancedLootFramework
{
    public class Treasure
    {
        public int index;
        public int value;
        public string type;

        public Treasure(int index, int value, string type)
        {
            this.index = index;
            this.value = value;
            this.type = type;
        }
    }
}