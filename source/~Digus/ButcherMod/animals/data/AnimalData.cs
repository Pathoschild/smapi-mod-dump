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
using StardewValley.GameData.Objects;

namespace AnimalHusbandryMod.animals.data
{
    public class AnimalData
    {
        public static readonly string[] BaseGameAnimals = new string[] { "White Chicken", "Brown Chicken", "Blue Chicken", "Void Chicken", "Golden Chicken", "Duck", "Rabbit", "Dinosaur", "White Cow", "Brown Cow", "Goat", "Pig", "Hog", "Sheep", "Ostrich" };

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
        public ISet<string> SyringeItemsIds;

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
            SyringeItemsIds = new HashSet<string>();
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
            AddTreatIdsFromTreatItems(Cow);
            AddTreatIdsFromTreatItems(Pig);
            AddTreatIdsFromTreatItems(Chicken);
            AddTreatIdsFromTreatItems(Duck);
            AddTreatIdsFromTreatItems(Rabbit);
            AddTreatIdsFromTreatItems(Sheep);
            AddTreatIdsFromTreatItems(Goat);
            AddTreatIdsFromTreatItems(Ostrich);
            AddTreatIdsFromTreatItems(Dinosaur);
            AddTreatIdsFromTreatItems(Pet);
            CustomAnimals.ForEach(customAnimal => AddTreatIdsFromTreatItems(customAnimal));
        }

        private static void AddTreatIdsFromTreatItems(TreatItem treatItem)
        {
            foreach (object likedTreat in treatItem.LikedTreats)
            {
                if (likedTreat is string s)
                {
                    if (ItemRegistry.Exists(ItemRegistry.type_object + s))
                    {
                        treatItem.LikedTreatsId.Add(s);
                    }
                    else
                    {
                        KeyValuePair<string, ObjectData> pair = Game1.objectData.FirstOrDefault(o => s.Equals(o.Value.Name));
                        if (pair.Value != null)
                        {
                            treatItem.LikedTreatsId.Add(pair.Key);
                        }
                    }
                }
                else if (likedTreat is long l)
                {
                    treatItem.LikedTreatsId.Add(l.ToString());
                }
                else if (likedTreat is int i)
                {
                    treatItem.LikedTreatsId.Add(i.ToString());
                }
            }
        }
    }
}
