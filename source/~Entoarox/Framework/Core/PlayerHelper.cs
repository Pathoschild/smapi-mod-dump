/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Characters;

namespace Entoarox.Framework.Core
{
    internal class PlayerHelper : IPlayerHelper
    {
        /*********
        ** Accessors
        *********/
        public IPlayerModifierHelper Modifiers { get; }


        /*********
        ** Public methods
        *********/
        public PlayerHelper(IPlayerModifierHelper modifiers)
        {
            this.Modifiers = modifiers;
        }

        public void MoveTo(int x, int y)
        {
            Game1.warpFarmer(Game1.player.currentLocation.Name, Convert.ToInt32(x), Convert.ToInt32(y), Game1.player.facingDirection, Game1.player.currentLocation.isStructure.Value);
        }

        public void MoveTo(string location, int x, int y)
        {
            GameLocation loc = Game1.getLocationFromName(location);
            if (loc == null)
                throw new ArgumentNullException(nameof(location));
            this.MoveTo(loc, x, y);
        }

        public void MoveTo(GameLocation location, int x, int y)
        {
            Game1.warpFarmer(location.Name, Convert.ToInt32(x), Convert.ToInt32(y), Game1.player.facingDirection, location.isStructure.Value);
        }

        public bool HasPet(bool vanillaOnly)
        {
            Pet[] pets = this.GetAllPets().ToArray();
            return vanillaOnly
                ? pets.Any(a => (Game1.player.catPerson ? a is Cat : a is Dog) && a.Manners == 0 && a.Age == 0)
                : pets.Any();
        }

        public Pet GetPet()
        {
            Pet[] pets = this.GetAllPets().ToArray();
            return pets.FirstOrDefault(a => (Game1.player.catPerson ? a is Cat : a is Dog) && a.Manners == 0 && a.Age == 0);
        }

        public IEnumerable<Pet> GetAllPets()
        {
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc is Pet pet)
                    yield return pet;
            }
        }
    }
}
