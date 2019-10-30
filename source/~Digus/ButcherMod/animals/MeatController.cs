using System;
using System.Collections.Generic;
using System.Linq;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.meats;
using AnimalHusbandryMod.tools;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace AnimalHusbandryMod.animals
{
    public class MeatController : AnimalStatusController
    {
        public static bool CanGetMeatFrom(FarmAnimal farmAnimal)
        {
            if (farmAnimal.type.Value == "Dinosaur" && !DataLoader.ModConfig.DisableMeatFromDinosaur)
            {
                return true;
            }
            try
            {
                AnimalItem animalItem = GetAnimalItem(farmAnimal);
                return ((animalItem is MeatAnimalItem meatAnimalItem) && meatAnimalItem.MaximumNumberOfMeat>0);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static AnimalItem GetAnimalItem(FarmAnimal farmAnimal)
        {
            List<Item> itemsToReturn = new List<Item>();

            return DataLoader.AnimalData.GetAnimalItem(farmAnimal);
        }

        public static List<Item> CreateMeat(FarmAnimal farmAnimal)
        {
            List<Item> itemsToReturn = new List<Item>();

            Animal animal;
            Animal? foundAnimal = AnimalExtension.GetAnimalFromType(farmAnimal.type.Value);
            if (foundAnimal == null || !CanGetMeatFrom(farmAnimal)) 
            {
                return itemsToReturn;
            }
            else
            {
                animal = (Animal)foundAnimal;
            }

            AnimalItem animalItem = DataLoader.AnimalData.GetAnimalItem(farmAnimal);
            int minimumNumberOfMeat;
            int maxNumberOfMeat;
            int meatPrice;
            int debrisType;
            if (animal == Animal.Dinosaur)
            {
                if (DataLoader.ModConfig.DisableMeatFromDinosaur)
                {
                    return itemsToReturn;
                }
                var meats = Enum.GetValues(typeof(Meat));
                Meat meat = ((Meat)meats.GetValue(new Random((int)farmAnimal.myID.Value).Next(meats.Length)));
                meatPrice = DataLoader.MeatData.getMeatItem(meat).Price;
                minimumNumberOfMeat = 1;
                maxNumberOfMeat = 1 + (1300 / meatPrice);
                debrisType = (int)meat;
            }
            else if (animal == Animal.CustomAnimal)
            {
                MeatAnimalItem meatAnimalItem = (MeatAnimalItem) animalItem;
                debrisType = farmAnimal.meatIndex.Value;
                if (Enum.IsDefined(typeof(Meat), debrisType))
                {
                    meatPrice = DataLoader.MeatData.getMeatItem((Meat)debrisType).Price;
                }
                else
                {
                    var objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
                    meatPrice = Convert.ToInt32(objects[debrisType].Split('/')[1]);
                }
                minimumNumberOfMeat = meatAnimalItem.MinimalNumberOfMeat;
                maxNumberOfMeat = meatAnimalItem.MaximumNumberOfMeat;
            }
            else
            {
                MeatAnimalItem meatAnimalItem = (MeatAnimalItem) animalItem;
                Meat meat = animal.GetMeat();
                meatPrice = DataLoader.MeatData.getMeatItem(meat).Price;
                minimumNumberOfMeat = meatAnimalItem.MinimalNumberOfMeat;
                maxNumberOfMeat = meatAnimalItem.MaximumNumberOfMeat;
                debrisType = (int)meat;
            }
            var numberOfMeat = minimumNumberOfMeat;

            numberOfMeat += (int)((farmAnimal.getSellPrice() / ((double)farmAnimal.price.Value) - 0.3) * (maxNumberOfMeat - minimumNumberOfMeat));

            Random random = new Random((int)farmAnimal.myID.Value * 10000 + (int)Game1.stats.DaysPlayed);
            int[] quality = { 0, 0, 0, 0, 0 };
            for (int i = 0; i < numberOfMeat; i++)
            {
                var produceQuality = ProduceQuality(random, farmAnimal);
                quality[produceQuality]++;
            }

            var tempTotal = meatPrice * quality[0] + meatPrice * quality[1] * 1.25 + meatPrice * quality[2] * 1.5 + meatPrice * quality[4] * 2;
            while (tempTotal < farmAnimal.getSellPrice() && quality[4] != numberOfMeat)
            {
                if (numberOfMeat < maxNumberOfMeat)
                {
                    numberOfMeat++;
                    quality[0]++;
                    tempTotal += meatPrice;
                }
                else if (quality[0] > 0)
                {
                    quality[0]--;
                    quality[1]++;
                    tempTotal += meatPrice * 0.25;
                }
                else if ((quality[1] > 0))
                {
                    quality[1]--;
                    quality[2]++;
                    tempTotal += meatPrice * 0.25;
                }
                else if ((quality[2] > 0))
                {
                    quality[2]--;
                    quality[4]++;
                    tempTotal += meatPrice * 0.50;
                }
            }

            for (; numberOfMeat > 0; --numberOfMeat)
            {
                Object newItem = new Object(Vector2.Zero, debrisType, 1);
                newItem.Quality = quality[4] > 0 ? 4 : quality[2] > 0 ? 2 : quality[1] > 0 ? 1 : 0;
                quality[newItem.Quality]--;

                itemsToReturn.Add(newItem);
            }

            if (animalItem is WoolAnimalItem woolAnimalItem)
            {
                int numberOfWools = farmAnimal.currentProduce.Value > 0 ? 1 : 0;
                numberOfWools += (int)(woolAnimalItem.MinimumNumberOfExtraWool + (farmAnimal.getSellPrice() / ((double)farmAnimal.price.Value) - 0.3) * (woolAnimalItem.MaximumNumberOfExtraWool - woolAnimalItem.MinimumNumberOfExtraWool));

                for (; numberOfWools > 0; --numberOfWools)
                {
                    Object newItem = new Object(Vector2.Zero, farmAnimal.defaultProduceIndex.Value, 1);
                    newItem.Quality = ProduceQuality(random, farmAnimal);
                    itemsToReturn.Add(newItem);
                }
            }

            if (animalItem is FeatherAnimalItem featherAnimalItem)
            {
                int numberOfFeather = (int)(featherAnimalItem.MinimumNumberOfFeatherChances + (farmAnimal.getSellPrice() / ((double)farmAnimal.price.Value) - 0.3) * (featherAnimalItem.MaximumNumberOfFeatherChances - featherAnimalItem.MinimumNumberOfFeatherChances));
                float num1 = (int)farmAnimal.happiness.Value > 200 ? (float)farmAnimal.happiness.Value * 1.5f : ((int)farmAnimal.happiness.Value <= 100 ? (float)((int)farmAnimal.happiness.Value - 100) : 0.0f);
                for (; numberOfFeather > 0; --numberOfFeather)
                {
                    if (random.NextDouble() < (double)farmAnimal.happiness.Value / 150.0)
                    {
                        if (random.NextDouble() < ((double)farmAnimal.friendshipTowardFarmer.Value + (double)num1) / 5000.0 + Game1.dailyLuck + (double)Game1.player.LuckLevel * 0.01)
                        {
                            Object newItem = new Object(Vector2.Zero, farmAnimal.deluxeProduceIndex.Value, 1);
                            newItem.Quality = ProduceQuality(random, farmAnimal);
                            itemsToReturn.Add(newItem);
                        }
                    }
                }
            }

            if (animalItem is FeetAnimalItem feetAnimalItem)
            {
                int numberOfFeet = (int)(feetAnimalItem.MinimumNumberOfFeetChances + (farmAnimal.getSellPrice() / ((double)farmAnimal.price.Value) - 0.3) * (feetAnimalItem.MaximumNumberOfFeetChances - feetAnimalItem.MinimumNumberOfFeetChances));
                float num1 = (int)farmAnimal.happiness.Value > 200 ? (float)farmAnimal.happiness.Value * 1.5f : ((int)farmAnimal.happiness.Value <= 100 ? (float)((int)farmAnimal.happiness.Value - 100) : 0.0f);
                for (; numberOfFeet > 0; --numberOfFeet)
                {
                    if (random.NextDouble() < (double)farmAnimal.happiness.Value / 150.0)
                    {
                        if (random.NextDouble() < ((double)farmAnimal.friendshipTowardFarmer.Value + (double)num1) / 5000.0 + Game1.dailyLuck + (double)Game1.player.LuckLevel * 0.01)
                        {
                            Object newItem = new Object(Vector2.Zero, farmAnimal.deluxeProduceIndex.Value, 1);
                            newItem.Quality = ProduceQuality(random, farmAnimal);
                            itemsToReturn.Add(newItem);
                        }
                    }
                }
            }
            if (AnimalContestController.CanChangeParticipant(farmAnimal))
            {
                AnimalContestController.RemoveAnimalParticipant(farmAnimal);
                itemsToReturn.Add(new ParticipantRibbon());
            }

            return itemsToReturn;
        }

        public static void ThrowItem(List<Item> newItems, FarmAnimal farmAnimal)
        {
            GameLocation location = Game1.currentLocation;
            int xTile = farmAnimal.getTileX() - 1;
            int yTile = farmAnimal.getTileY() - 1;
            Vector2 debrisOrigin = new Vector2((float)(xTile * Game1.tileSize + Game1.tileSize),(float)(yTile * Game1.tileSize + Game1.tileSize));

            foreach (Item newItem in newItems)
            {
                switch (Game1.random.Next(4))
                {
                    case 0:
                        location.debris.Add(new Debris(newItem, debrisOrigin,
                            debrisOrigin + new Vector2(-Game1.tileSize, 0.0f)));
                        break;
                    case 1:
                        location.debris.Add(new Debris(newItem, debrisOrigin,
                            debrisOrigin + new Vector2(Game1.tileSize, 0.0f)));
                        break;
                    case 2:
                        location.debris.Add(new Debris(newItem, debrisOrigin,
                            debrisOrigin + new Vector2(0.0f, Game1.tileSize)));
                        break;
                    case 3:
                        location.debris.Add(new Debris(newItem, debrisOrigin,
                            debrisOrigin + new Vector2(0.0f, -Game1.tileSize)));
                        break;
                }
            }
        }

        public static void AddItemsToInventoryByMenuIfNecessary(List<Item> items, ItemGrabMenu.behaviorOnItemSelect itemSelectedCallback = null)
        {
            List<Item> listItensToAdd = items.Where(i => !(i is Object)).ToList();
            List<Object> listObjects = items
                    .Where(i => i is Object)
                    .GroupBy(i => new {id = i.ParentSheetIndex, (i as Object).Quality })
                    .Select(g => new Object(Vector2.Zero, g.Key.id, g.Count()) {Quality = g.Key.Quality })
                    .ToList();
            listItensToAdd.AddRange(listObjects);
            Game1.player.addItemsByMenuIfNecessary(
                listItensToAdd,
                itemSelectedCallback
            );
        }

        private static int ProduceQuality(Random random, FarmAnimal farmAnimal)
        {
            if (!DataLoader.ModConfig.DisableContestBonus && AnimalContestController.HasProductionBonus(farmAnimal))
            {
                return 4;
            }

            double chance = (double)farmAnimal.friendshipTowardFarmer.Value / 1000.0 - (1.0 - (double)farmAnimal.happiness.Value / 225.0);
            if (!farmAnimal.isCoopDweller() && Game1.getFarmer(farmAnimal.ownerID.Value).professions.Contains(3) ||
                farmAnimal.isCoopDweller() && Game1.getFarmer(farmAnimal.ownerID.Value).professions.Contains(2))
                chance += 0.33;
            var produceQuality = chance < 0.95 || random.NextDouble() >= chance / 2.0
                ? (random.NextDouble() >= chance / 2.0 ? (random.NextDouble() >= chance ? 0 : 1) : 2)
                : 4;
            return produceQuality;
        }
    }
}
