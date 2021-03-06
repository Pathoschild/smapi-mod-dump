/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using AnimalHusbandryMod.common;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;

namespace AnimalHusbandryMod.animals.data
{
    public class AnimalData
    {
        public const long PetId = -10;

        public CowItem Cow;
        public PigItem Pig;
        public ChickenItem Chicken;
        public DuckItem Duck;
        public RabbitItem Rabbit;
        public SheepItem Sheep;
        public GoatItem Goat;
        public PetItem Pet;
        public DinosaurItem Dinosaur;
        public OstrichItem Ostrich;
        public List<CustomAnimalItem> CustomAnimals;
        [JsonIgnore]
        public ISet<int> SyringeItemsIds;

        public AnimalData()
        {
            Cow = new CowItem();
            Pig = new PigItem();
            Chicken = new ChickenItem();
            Duck = new DuckItem();
            Rabbit = new RabbitItem();
            Sheep = new SheepItem();
            Goat = new GoatItem();
            Pet = new PetItem();
            Dinosaur = new DinosaurItem();
            Ostrich = new OstrichItem();
            CustomAnimals = new List<CustomAnimalItem>();
            SyringeItemsIds = new HashSet<int>();
        }

        public AnimalItem GetAnimalItem(FarmAnimal farmAnimal)
        {
            return GetAnimalItem(farmAnimal.type.Value);
        }

        public AnimalItem GetAnimalItem(String farmAnimalType)
        {
            Animal? animalEnum = AnimalExtension.GetAnimalFromType(farmAnimalType);
            switch (animalEnum)
            {
                case Animal.Cow:
                    return Cow;
                case Animal.Pig:
                    return Pig;
                case Animal.Chicken:
                    return Chicken;
                case Animal.Duck:
                    return Duck;
                case Animal.Rabbit:
                    return Rabbit;
                case Animal.Sheep:
                    return Sheep;
                case Animal.Goat:
                    return Goat;
                case Animal.Ostrich:
                    return Ostrich;
                case Animal.Dinosaur:
                    return Dinosaur;
                case Animal.CustomAnimal:
                    return CustomAnimals.Find(a => farmAnimalType.Contains(a.Name));
                default:
                    return null;
            }
        }

        public void FillLikedTreatsIds()
        {
            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
            AddTreatIdsFromTreatItems(objects, Cow);
            AddTreatIdsFromTreatItems(objects, Pig);
            AddTreatIdsFromTreatItems(objects, Chicken);
            AddTreatIdsFromTreatItems(objects, Duck);
            AddTreatIdsFromTreatItems(objects, Rabbit);
            AddTreatIdsFromTreatItems(objects, Sheep);
            AddTreatIdsFromTreatItems(objects, Goat);
            AddTreatIdsFromTreatItems(objects, Ostrich);
            AddTreatIdsFromTreatItems(objects, Dinosaur);
            AddTreatIdsFromTreatItems(objects, Pet);
            CustomAnimals.ForEach(customAnimal => AddTreatIdsFromTreatItems(objects, customAnimal));
        }

        private static void AddTreatIdsFromTreatItems(Dictionary<int, string> objects, TreatItem treatItem)
        {
            foreach (object likedTreat in treatItem.LikedTreats)
            {
                if (likedTreat is string s)
                {
                    KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(s + "/"));
                    if (pair.Value != null)
                    {
                        treatItem.LikedTreatsId.Add(pair.Key);
                    }
                }
                else if (likedTreat is long l)
                {
                    treatItem.LikedTreatsId.Add((int)l);
                }
                else if (likedTreat is int i)
                {
                    treatItem.LikedTreatsId.Add(i);
                }
            }
        }
    }
}
