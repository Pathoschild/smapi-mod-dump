/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace TMXLoader
{
    public class PersistentData
    {
        public string Type { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public PersistentData(string type, string key, string value)
        {
            Type = type;
            Key = key;
            Value = value;
        }
    }
}
