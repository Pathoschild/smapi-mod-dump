/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using System.Collections.Generic;

namespace CustomStarterFurniture
{
    public class StarterFurnitureData
    {
        public int FarmType = -1;
        public bool Clear;
        public List<FurnitureData> Furniture;
    }

    public class FurnitureData
    {
        public string NameOrIndex;
        public string HeldObjectType;
        public string HeldObjectNameOrIndex;
        public int X;
        public int Y;
        public int Rotation;
    }
}