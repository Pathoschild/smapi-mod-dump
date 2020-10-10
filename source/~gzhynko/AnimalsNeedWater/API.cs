/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace AnimalsNeedWater
{
    public interface IAnimalsNeedWaterAPI
    {
        List<ModEntry.AnimalLeftThirsty> GetAnimalsLeftThirstyYesterday();

        List<string> GetCoopsWithWateredTrough();
        List<string> GetBarnsWithWateredTrough();

        bool IsAnimalFull(string displayName);
        List<string> GetFullAnimals();
    }

    public class API : IAnimalsNeedWaterAPI
    {
        public List<ModEntry.AnimalLeftThirsty> GetAnimalsLeftThirstyYesterday()
        {
            return ModEntry.instance.AnimalsLeftThirstyYesterday;
        }

        public List<string> GetCoopsWithWateredTrough()
        {
            return ModData.CoopsWithWateredTrough;
        }

        public List<string> GetBarnsWithWateredTrough()
        {
            return ModData.BarnsWithWateredTrough;
        }

        public bool IsAnimalFull(string displayName)
        {
            // just to make this stuff non-case-sensitive
            List<string> LowerFullAnimals = ModData.FullAnimals.ConvertAll(s => s.ToLower());

            return LowerFullAnimals.Contains(displayName.ToLower());
        }

        public List<string> GetFullAnimals()
        {
            return ModData.FullAnimals;
        }
    }
}
