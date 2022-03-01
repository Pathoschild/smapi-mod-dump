/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace HelpForHire.Chores;

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Characters;

internal class FeedPet : GenericChore
{
    public FeedPet(ServiceLocator serviceLocator)
        : base("feed-pet", serviceLocator)
    {
    }

    protected override bool DoChore()
    {
        var petted = false;
        Game1.getFarm().petBowlWatered.Set(true);

        foreach (var pet in FeedPet.GetPets())
        {
            if (!pet.lastPetDay.ContainsKey(Game1.player.UniqueMultiplayerID))
            {
                pet.lastPetDay.Add(Game1.player.UniqueMultiplayerID, -1);
            }

            pet.lastPetDay[Game1.player.UniqueMultiplayerID] = Game1.Date.TotalDays;
            pet.grantedFriendshipForPet.Set(true);
            pet.friendshipTowardFarmer.Set(Math.Min(1000, pet.friendshipTowardFarmer.Value + 12));
            petted = true;
        }

        return petted;
    }

    protected override bool TestChore()
    {
        return FeedPet.GetPets().Any();
    }

    private static IEnumerable<Pet> GetPets()
    {
        return
            from pet in Game1.getFarm().characters.OfType<Pet>()
            where !pet.grantedFriendshipForPet.Value && pet.lastPetDay[Game1.player.UniqueMultiplayerID] != Game1.Date.TotalDays
            select pet;
    }
}