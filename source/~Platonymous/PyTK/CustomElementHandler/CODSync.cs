/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace PyTK.CustomElementHandler
{
    public class CODSync
    {
        public string Id { get; set; }
        public int Index { get; set; }

        public CODSync()
        {

        }

        public CODSync(string id, int index)
        {
            Id = id;
            Index = index;
        }
    }
}
