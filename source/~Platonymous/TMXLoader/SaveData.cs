/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace TMXLoader
{
    public class SaveData
    {
        public List<SaveLocation> Locations = new List<SaveLocation>();
        public List<SaveBuildable> Buildables = new List<SaveBuildable>();
        public List<PersistentData> Data = new List<PersistentData>();
    }
}
