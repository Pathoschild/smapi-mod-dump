/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

namespace stardew_access.Features.Tracker;

using StardewValley;
using System.Collections.Generic;
using System.Linq;

internal class TTAnimals(object? arg = null) : TileTrackerBase(arg)
{
    public override void FindObjects(object? arg = null)
    {

        string category = "animals";
        GameLocation location = Game1.player.currentLocation;

        List<FarmAnimal>? farmAnimals = null;

        if (location is Farm farm)
            farmAnimals = farm.animals?.Values.ToList();
        else if (location is AnimalHouse)
            farmAnimals = [.. (location as AnimalHouse)!.animals.Values];

        if (farmAnimals != null) {

            foreach (FarmAnimal animal in farmAnimals) {
                if(animal != null) {

                    string moodMessage = animal.getMoodMessage();

                    string is_hungry = "";
                    if (moodMessage.ToLower().Contains("thin")) {
                        is_hungry = "Hungry ";
                    }

                    AddFocusableObject(category, $"{animal.displayName}, {is_hungry}{animal.displayType}, {animal.age}", animal.getTileLocation());
                }
            }

        }

        base.FindObjects(arg);
    }
}