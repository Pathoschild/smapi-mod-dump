/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace PyTK.Types
{
    public class DictionaryEntry<TKey, TValue>
    { 
        public TKey key;
        public TValue value;

        public DictionaryEntry(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
