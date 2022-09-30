/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Effects;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
    public enum AnimalType
    {
        Unknown = -1,
        Any = 0,
        Chicken,
        Duck,
        Rabbit,
        Dinosaur,
        Cow,
        Goat,
        Pig,
        Hog,
        Sheep,
        Ostrich
    }

    static class AnimalTypeExtensions
    {
        public static AnimalType GetAnimalType(this FarmAnimal animal)
        {
            foreach(AnimalType at in Enum.GetValues(typeof(AnimalType)))
            {
                if (StrContains(animal.type.Value, at.ToString()))
                {
                    return at;
                }
            }

            return AnimalType.Unknown;
        }

        public static EffectIcon GetPetEffectIcon(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.Chicken: return EffectIcon.Animal_Chicken;
                case AnimalType.Cow: return EffectIcon.Animal_Cow;
                default: return EffectIcon.None;
            }
        }

        public static string GetPetEffectDescription(this AnimalType animalType)
        {
            switch (animalType)
            {
                case AnimalType.Any:
                    return $"Pet any animal by touching it";
                case AnimalType.Chicken:                    
                case AnimalType.Duck:                    
                case AnimalType.Rabbit:                    
                case AnimalType.Dinosaur:                    
                case AnimalType.Cow:                    
                case AnimalType.Goat:                    
                case AnimalType.Pig:                    
                case AnimalType.Hog:
                    return $"Pet {animalType.ToString().ToLower()}s by touching them";
                case AnimalType.Sheep:
                    return $"Pet sheep by touching them";
                case AnimalType.Ostrich:
                    return $"Pet ostriches by touching them";                    
                default:
                    return "";

            }
        }

        /// <summary>
        /// Case-insensitive contains
        /// </summary>
        /// <param name="s"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        static bool StrContains(string s, string word)
        {
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(s, word, CompareOptions.IgnoreCase) >= 0;
        }
    }
}
