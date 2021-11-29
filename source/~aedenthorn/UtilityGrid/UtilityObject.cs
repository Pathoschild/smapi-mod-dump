/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace UtilityGrid
{
    public class UtilityObject
    {
        public float water;
        public float electric;
        public bool mustBeOn;
        public bool mustBeFull;
        public string mustContain;
        public bool mustBeWorking;
        public bool onlyInWater;
        public bool mustHaveSun;
        public bool mustHaveRain;
        public bool mustHaveLightning;
        public Object worldObj;
    }
}