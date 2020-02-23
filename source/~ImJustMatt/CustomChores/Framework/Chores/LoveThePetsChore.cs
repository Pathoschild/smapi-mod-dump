using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewValley;
using StardewValley.Characters;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class LoveThePetsChore: BaseChore
    {
        private IList<Pet> _pets = new List<Pet>();
        private readonly bool _fillWaterBowl;
        private readonly bool _enablePetting;
        private int _petsPetted;

        public LoveThePetsChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("FillWaterBowl", out var waterBowl);
            ChoreData.Config.TryGetValue("EnablePetting", out var enablePetting);

            _fillWaterBowl = !(waterBowl is bool b1) || b1;
            _enablePetting = !(enablePetting is bool b2) || b2;
        }

        public override bool CanDoIt(bool today = true)
        {
            _petsPetted = 0;
            _pets.Clear();

            _pets = Game1.getFarm().characters
                .OfType<Pet>()
                .ToList();

            return _pets.Any();
        }

        public override bool DoIt()
        {
            if (_fillWaterBowl && !Game1.isRaining && !Game1.getFarm().petBowlWatered.Value)
            {
                Game1.getFarm().petBowlWatered.Set(true);
            }

            if (!_enablePetting)
                return _petsPetted > 0;

            var farmer = Game1.player;
            foreach (var pet in _pets)
            {
                if (!pet.lastPetDay.ContainsKey(farmer.UniqueMultiplayerID))
                    pet.lastPetDay.Add(farmer.UniqueMultiplayerID, -1);
                if (pet.lastPetDay[farmer.UniqueMultiplayerID] == Game1.Date.TotalDays)
                    continue;
                pet.lastPetDay[farmer.UniqueMultiplayerID] = Game1.Date.TotalDays;
                if (pet.grantedFriendshipForPet.Value)
                    continue;
                pet.grantedFriendshipForPet.Set(true);
                pet.friendshipTowardFarmer.Set(Math.Min(1000, pet.friendshipTowardFarmer.Value + 12));
                ++_petsPetted;
            }

            return _petsPetted > 0;
        }

        public override IDictionary<string, Func<string>> GetTokens()
        {
            var tokens = base.GetTokens();
            tokens.Add("PetCount", GetPetCount);
            tokens.Add("WorkDone", GetWorkDone);
            tokens.Add("WorkNeeded", GetWorkNeeded);
            return tokens;
        }

        private string GetPetCount() =>
            _pets?.Count.ToString(CultureInfo.InvariantCulture);
        private string GetWorkDone() =>
            _petsPetted.ToString(CultureInfo.InvariantCulture);
        private string GetWorkNeeded() =>
            (_fillWaterBowl || _enablePetting ? _pets.Count : 0).ToString(CultureInfo.InvariantCulture);
    }
}
