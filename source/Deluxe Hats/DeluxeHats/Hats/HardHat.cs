using StardewValley;
using StardewValley.Monsters;
using System;

namespace DeluxeHats.Hats
{
    public static class HardHat
    {
        public const string Name = "Hard Hat";
        public const string Description = "Reduce damage done by flying monsters by 25%.";
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                foreach (var character in Game1.currentLocation.getCharacters())
                {
                    if (character.IsMonster)
                    {
                        var monster = character as Monster;
                        if (monster.isGlider.Value && !monster.datingFarmer)
                        {
                            monster.age.Set(monster.damageToFarmer.Value);
                            monster.damageToFarmer.Set(Convert.ToInt32(monster.damageToFarmer * 0.85));
                            monster.datingFarmer = true;
                        }
                    }
                }
            };
        }

        public static void Disable()
        {
            foreach (var character in Game1.currentLocation.getCharacters())
            {
                if (character.IsMonster)
                {
                    var monster = character as Monster;
                    if (monster.isGlider.Value && monster.datingFarmer)
                    {
                        monster.damageToFarmer.Set(monster.age.Value);
                        monster.datingFarmer = false;
                    }
                }
            }
        }
    }
}
