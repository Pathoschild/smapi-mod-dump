/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;

namespace FurnitureDisplayFramework
{
    public class FurnitureDisplayData
    {
        public string name;
        public FurnitureDisplaySlot[] slots;
    }

    public class FurnitureDisplaySlot
    {
        public Rectangle slotRect;
        public Rectangle itemRect;
    }
}