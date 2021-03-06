/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using StardewValley;
    using StardewValley.Characters;

    public class PetMenu : BaseMenu
    {
        private readonly Pet pet;

        private readonly HorseOverhaul mod;

        public PetMenu(HorseOverhaul mod, Pet pet) : base(pet.displayName)
        {
            this.mod = mod;
            this.pet = pet;
        }

        public override string GetStatusMessage()
        {
            string yes = mod.Helper.Translation.Get("Yes");
            string no = mod.Helper.Translation.Get("No");

            string petAnswer = pet.grantedFriendshipForPet.Value ? yes : no;
            string waterAnswer = Game1.getFarm().petBowlWatered.Value ? yes : no;
            string foodAnswer = pet?.modData?.TryGetValue($"{mod.ModManifest.UniqueID}/gotFed", out _) == true ? yes : no;

            string friendship = mod.Helper.Translation.Get("Friendship", new { value = pet.friendshipTowardFarmer.Value }) + "\n";
            string petted = mod.Config.Petting ? mod.Helper.Translation.Get("GotPetted", new { value = petAnswer }) + "\n" : string.Empty;
            string water = mod.Config.Water ? mod.Helper.Translation.Get("GotWater", new { value = waterAnswer }) + "\n" : string.Empty;
            string food = mod.Config.PetFeeding ? mod.Helper.Translation.Get("GotFood", new { value = foodAnswer }) : string.Empty;

            return $"{friendship}{petted}{water}{food}";
        }

        public override int GetFriendship()
        {
            return pet.friendshipTowardFarmer.Value;
        }
    }
}