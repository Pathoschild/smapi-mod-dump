/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewRoguelike
{
    public class MerchantFloor
    {
        public readonly int SwordsToPick;
        public readonly int RingsToPick;
        public readonly int SpecialRingsToPick;
        public readonly int BootsToPick;
        public readonly int SpecialFoodToPick;

        // Dict key is item id, tuple is (minPrice, maxPrice, quantity)
        public readonly Dictionary<int, Tuple<int, int, int>> Swords = new();
        public readonly Dictionary<int, Tuple<int, int, int>> Rings = new();
        public readonly Dictionary<int, Tuple<int, int, int>> SpecialRings = new();
        public readonly Dictionary<int, Tuple<int, int, int>> Boots = new();
        public readonly Dictionary<int, Tuple<int, int, int>> SpecialFood = new();

        public MerchantFloor() { }

        public MerchantFloor(int swordsToPick, int ringsToPick, int specialRingsToPick, int bootsToPick, int specialFoodToPick)
        {
            SwordsToPick = swordsToPick;
            RingsToPick = ringsToPick;
            SpecialRingsToPick = specialRingsToPick;
            BootsToPick = bootsToPick;
            SpecialFoodToPick = specialFoodToPick;
        }

        public void AddSword(int id, int priceFrom, int priceTo, int quantity=-1)
        {
            Swords[id] = new(priceFrom, priceTo, quantity);
        }

        public void AddRing(int id, int priceFrom, int priceTo, int quantity = -1)
        {
            Rings[id] = new(priceFrom, priceTo, quantity);
        }

        public void AddSpecialRing(int id, int priceFrom, int priceTo, int quantity = -1)
        {
            SpecialRings[id] = new(priceFrom, priceTo, quantity);
        }

        public void AddBoots(int id, int priceFrom, int priceTo, int quantity = -1)
        {
            Boots[id] = new(priceFrom, priceTo, quantity);
        }

        public void AddSpecialFood(int id, int priceFrom, int priceTo, int quantity = -1)
        {
            SpecialFood[id] = new(priceFrom, priceTo, quantity);
        }

        public static List<T> PickNFromList<T>(IList<T> source, int numToPick, Random? random = null)
        {
            random ??= Game1.random;

            if (numToPick > source.Count)
                return (List<T>)source;

            List<T> pickedElements = new();
            while (pickedElements.Count < numToPick)
            {
                int pickedIndex = random.Next(source.Count);
                if (pickedElements.Contains(source[pickedIndex]))
                    continue;

                pickedElements.Add(source[pickedIndex]);
            }

            return pickedElements;
        }

        public Item PickAnyRandom(Random? random = null)
        {
            random ??= Game1.random;

            List<Item> validItems = new();

            foreach (int swordId in Swords.Keys)
                validItems.Add(new MeleeWeapon(swordId));

            foreach (int ringId in Rings.Keys)
                validItems.Add(new Ring(ringId));

            foreach (int ringId in SpecialRings.Keys)
                validItems.Add(new Ring(ringId));

            foreach (int bootId in Boots.Keys)
                validItems.Add(new Boots(bootId));

            foreach (int specialFoodId in SpecialFood.Keys)
                validItems.Add(new SObject(specialFoodId, 1));

            return Utility.GetRandom(validItems, random);
        }

        public Item PickAnyRandomAvoiding(List<string> toAvoid, Random? random = null)
        {
            random ??= Game1.random;

            List<Item> validItems = new();

            foreach (int swordId in Swords.Keys)
                validItems.Add(new MeleeWeapon(swordId));

            foreach (int ringId in Rings.Keys)
                validItems.Add(new Ring(ringId));

            foreach (int ringId in SpecialRings.Keys)
                validItems.Add(new Ring(ringId));

            foreach (int bootId in Boots.Keys)
                validItems.Add(new Boots(bootId));

            foreach (int specialFoodId in SpecialFood.Keys)
                validItems.Add(new SObject(specialFoodId, 1));

            while (toAvoid.Count >= validItems.Count)
                toAvoid.RemoveAt(Game1.random.Next(toAvoid.Count));

            validItems.RemoveAll(item => toAvoid.Contains(item.DisplayName));

            return Utility.GetRandom(validItems, random);
        }

        public MeleeWeapon PickAnySword(Random? random = null)
        {
            random ??= Game1.random;
            int swordId = Utility.GetRandom(PickNFromList(Swords.Keys.ToList(), SwordsToPick), random);
            return new(swordId);
        }

        public Ring PickAnyRing(Random? random = null)
        {
            random ??= Game1.random;
            var allRingIds = Rings.Keys.ToList().Concat(SpecialRings.Keys.ToList()).ToList();
            int ringId = Utility.GetRandom(PickNFromList(allRingIds, RingsToPick + SpecialRingsToPick), random);
            return new(ringId);
        }

        public Boots PickAnyBoots(Random? random = null)
        {
            random ??= Game1.random;
            int bootId = Utility.GetRandom(PickNFromList(Boots.Keys.ToList(), BootsToPick), random);
            return new(bootId);
        }

        public SObject PickAnyFood(Random? random = null)
        {
            random ??= Game1.random;
            List<int> regularFoodIds = new() { 194, 196, 773 };
            var allFoodIds = SpecialFood.Keys.ToList().Concat(regularFoodIds).ToList();
            int specialFoodId = Utility.GetRandom(PickNFromList(allFoodIds, SpecialFoodToPick + 3, random), random);
            return new(specialFoodId, 1);
        }


        public static int Randint(int from, int to, Random? random = null)
        {
            random ??= Game1.random;
            return random.Next(from, to + 1);
        }

        public static int CalculateBuyPrice(int priceFrom, int priceTo, float priceAdjustment = 1f, Random? random = null)
        {
            random ??= Game1.random;
            int result;
            if (Curse.HasCurse(CurseType.CheaperMerchant))
                result = Randint((int)Math.Round(priceFrom * 0.6f), (int)Math.Round(priceTo * 0.6f), random);
            else
                result = Randint(priceFrom, priceTo, random);

            return (int)(result * priceAdjustment);
        }

        public void AddToStock(Dictionary<ISalable, int[]> stock, float priceAdjustment = 1f, Random? random = null)
        {
            random ??= Game1.random;

            int swordsToPick = SwordsToPick;
            int ringsToPick = RingsToPick;
            int specialRingsToPick = SpecialRingsToPick;
            int bootsToPick = BootsToPick;
            int specialFoodToPick = SpecialFoodToPick;

            if (Curse.HasCurse(CurseType.CheaperMerchant))
            {
                swordsToPick = Math.Min(1, SwordsToPick);
                ringsToPick = Math.Min(1, RingsToPick);
                specialRingsToPick = Math.Min(1, SpecialRingsToPick);
                bootsToPick = Math.Min(1, BootsToPick);
                specialFoodToPick = Math.Min(1, SpecialFoodToPick);
            }

            List<int> chosenSwords = PickNFromList(Swords.Keys.ToList(), swordsToPick, random);
            List<int> chosenRings = PickNFromList(Rings.Keys.ToList(), ringsToPick, random);
            List<int> chosenSpecialRings = PickNFromList(SpecialRings.Keys.ToList(), specialRingsToPick, random);
            List<int> chosenBoots = PickNFromList(Boots.Keys.ToList(), bootsToPick, random);
            List<int> chosenSpecialFood = PickNFromList(SpecialFood.Keys.ToList(), specialFoodToPick, random);

            foreach (int swordId in chosenSwords)
                Utility.AddStock(stock, new MeleeWeapon(swordId), buyPrice: CalculateBuyPrice(Swords[swordId].Item1, Swords[swordId].Item2, priceAdjustment, random), limitedQuantity: Swords[swordId].Item3);

            foreach (int ringId in chosenRings)
                Utility.AddStock(stock, new Ring(ringId), buyPrice: CalculateBuyPrice(Rings[ringId].Item1, Rings[ringId].Item2, priceAdjustment, random), limitedQuantity: Rings[ringId].Item3);

            foreach (int ringId in chosenSpecialRings)
                Utility.AddStock(stock, new Ring(ringId), buyPrice: CalculateBuyPrice(SpecialRings[ringId].Item1, SpecialRings[ringId].Item2, priceAdjustment, random), limitedQuantity: SpecialRings[ringId].Item3);

            foreach (int bootId in chosenBoots)
                Utility.AddStock(stock, new Boots(bootId), buyPrice: CalculateBuyPrice(Boots[bootId].Item1, Boots[bootId].Item2, priceAdjustment, random), limitedQuantity: Boots[bootId].Item3);

            foreach (int specialFoodId in chosenSpecialFood)
                Utility.AddStock(stock, new SObject(specialFoodId, SpecialFood[specialFoodId].Item3), buyPrice: CalculateBuyPrice(SpecialFood[specialFoodId].Item1, SpecialFood[specialFoodId].Item2, random: random), limitedQuantity: SpecialFood[specialFoodId].Item3);
        }
    }
}
