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
using AnimalsNeedWater.Types;
using StardewValley;

namespace AnimalsNeedWater
{
    public interface IAnimalsNeedWaterAPI
    {
        List<long> GetAnimalsLeftThirstyYesterday();
        bool WasAnimalLeftThirstyYesterday(FarmAnimal animal);

        List<string> GetCoopsWithWateredTrough();
        List<string> GetBarnsWithWateredTrough();

        bool IsAnimalFull(FarmAnimal animal);
        bool DoesAnimalHaveAccessToWater(FarmAnimal animal);
        List<long> GetFullAnimals();
    }

    public class API : IAnimalsNeedWaterAPI
    {
        public List<long> GetAnimalsLeftThirstyYesterday()
        {
            return ModEntry.Instance.AnimalsLeftThirstyYesterday.ConvertAll(i => i.myID.Value);
        }

        public bool WasAnimalLeftThirstyYesterday(FarmAnimal animal)
        {
            return ModEntry.Instance.AnimalsLeftThirstyYesterday.Contains(animal);
        }

        public List<string> GetCoopsWithWateredTrough()
        {
            return ModData.CoopsWithWateredTrough;
        }

        public List<string> GetBarnsWithWateredTrough()
        {
            return ModData.BarnsWithWateredTrough;
        }

        public bool IsAnimalFull(FarmAnimal animal)
        {
            return ModData.FullAnimals.Contains(animal);
        }
        
        public bool DoesAnimalHaveAccessToWater(FarmAnimal animal)
        {
            var houseTroughFull = ModData.CoopsWithWateredTrough.Contains(animal.home.GetIndoorsName().ToLower()) ||
                                  ModData.BarnsWithWateredTrough.Contains(animal.home.GetIndoorsName().ToLower());
            return houseTroughFull || ModData.FullAnimals.Contains(animal);
        }

        public List<long> GetFullAnimals()
        {
            return ModData.FullAnimals.ConvertAll(i => i.myID.Value);
        }
    }
}
