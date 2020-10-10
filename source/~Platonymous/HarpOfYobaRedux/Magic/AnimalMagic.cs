/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using PyTK;
using StardewValley;

namespace HarpOfYobaRedux
{
    class AnimalMagic : IMagic
    {
        public AnimalMagic()
        {

        }

        public void petAnimals()
        {
            if (Game1.currentLocation is AnimalHouse || Game1.currentLocation is Farm)
            {
                var animals = Game1.getFarm().animals;

                if (Game1.currentLocation is AnimalHouse)
                    animals = (Game1.currentLocation as AnimalHouse).animals;

                foreach (FarmAnimal animal in animals.Values)
                    if (!animal.wasPet.Value)
                    {
                        Game1.player.FarmerSprite.PauseForSingleAnimation = false;
                        animal.pet(Game1.player);
                    }
            }
        }

        public void doMagic(bool playedToday)
        {
            PyUtils.setDelayedAction(6000, petAnimals);
        }       
    }
}
