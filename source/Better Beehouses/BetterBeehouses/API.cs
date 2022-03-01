/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using StardewValley;

namespace BetterBeehouses
{
    public class API : IBetterBeehousesAPI
    {
        public int GetDaysToProduce()
        {
            return ModEntry.config.DaysToProduce;
        }
        public bool GetEnabledHere(GameLocation location)
        {
            return !ObjectPatch.CantProduceToday(location.GetSeasonForLocation() == "Winter", location);
        }
        public bool GetEnabledHere(GameLocation location, bool isWinter)
        {
            return !ObjectPatch.CantProduceToday(isWinter, location);
        }
        public int GetSearchRadius()
        {
            return ModEntry.config.FlowerRange;
        }
        public float GetValueMultiplier()
        {
            return ModEntry.config.ValueMultiplier;
        }
    }
}
