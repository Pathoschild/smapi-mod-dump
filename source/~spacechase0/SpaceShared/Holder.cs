/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace SpaceShared
{
    public class Holder<T>
    {
        public T Value;

        public Holder() { }

        public Holder(T value)
        {
            this.Value = value;
        }
    }
}
