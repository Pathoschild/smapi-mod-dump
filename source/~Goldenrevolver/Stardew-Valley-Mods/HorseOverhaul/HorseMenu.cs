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

    public class HorseMenu : BaseMenu
    {
        private readonly HorseWrapper horse;

        private readonly HorseOverhaul mod;

        public HorseMenu(HorseOverhaul mod, HorseWrapper horse)
          : base(Game1.player.horseName)
        {
            this.mod = mod;
            this.horse = horse;
        }

        public override string GetStatusMessage()
        {
            string yes = mod.Helper.Translation.Get("Yes");
            string no = mod.Helper.Translation.Get("No");

            string petAnswer = horse.WasPet ? yes : no;
            string waterAnswer = horse.GotWater ? yes : no;
            string foodAnswer = horse.GotFed ? yes : no;

            string friendship = mod.Helper.Translation.Get("Friendship", new { value = horse.Friendship }) + "\n";
            string petted = mod.Config.Petting ? mod.Helper.Translation.Get("GotPetted", new { value = petAnswer }) + "\n" : string.Empty;
            string water = mod.Config.Water ? mod.Helper.Translation.Get("GotWater", new { value = waterAnswer }) + "\n" : string.Empty;
            string food = mod.Config.Feeding ? mod.Helper.Translation.Get("GotFood", new { value = foodAnswer }) : string.Empty;

            return $"{friendship}{petted}{water}{food}";
        }

        public override int GetFriendship()
        {
            return horse.Friendship;
        }
    }
}