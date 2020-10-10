/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewValleyMP.States
{
    public class FarmAnimalState : State
    {
        public string name = "";
        public bool reproduce = false;
        public byte fullness = 255;
        public int product = -1;
        public bool pet = false;
        public int friendship = 0;
        public Vector2 homeLoc = new Vector2(0, 0);

        public FarmAnimalState()
        {
        }

        public FarmAnimalState(FarmAnimal animal)
        {
            name = animal.name;
            reproduce = animal.allowReproduction;
            fullness = animal.fullness;
            product = animal.currentProduce;
            pet = animal.wasPet;
            friendship = animal.friendshipTowardFarmer;
            homeLoc = animal.homeLocation;
        }

        public override bool isDifferentEnoughFromOldStateToSend(State obj)
        {
            FarmAnimalState state = obj as FarmAnimalState;
            if (obj == null) return false;

            if (name != state.name) return true;
            if (reproduce != state.reproduce) return true;
            if (fullness > state.fullness) return true;
            if (product != state.product) return true;
            if (pet != state.pet) return true;
            if (friendship > state.friendship) return true;
            if (homeLoc != state.homeLoc) return true;

            return false;
        }

        public override string ToString()
        {
            return base.ToString() + " " + name + " " + reproduce + " " + fullness + " " + product + " " + pet + " " + friendship + " " + homeLoc;
        }
    }
}
