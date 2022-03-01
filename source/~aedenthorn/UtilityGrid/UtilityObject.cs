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
        public bool onlyMorning;
        public bool onlyDay;
        public bool onlyNight;
        public bool mustBeFull;
        public bool mustNeedOther;
        public string mustContain;
        public bool mustBeWorking;
        public bool onlyInWater;
        public bool mustHaveSun;
        public bool mustHaveRain;
        public bool mustHaveLightning;
        public int waterChargeCapacity;
        public int electricChargeCapacity;
        public float waterChargeRate;
        public float electricChargeRate;
        public float waterDischargeRate;
        public float electricDischargeRate;
        public bool fillWaterFromRain;
    }
}