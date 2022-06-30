/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/DefaultFarmer
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace DefaultFarmer
{
    public class FarmerCustomizationData
    {
        public string Name { get; set; } = "";

        public string FarmName { get; set; } = "";

        public string FavThing { get; set; } = "";

        public bool Gender { get; set; } = Game1.player.IsMale;

        public Color EyeColor { get; set; } = Game1.player.newEyeColor.Value;

        public Color HairColor { get; set; } = Game1.player.hairstyleColor.Value;

        public Color PantsColor { get; set; } = Game1.player.pantsColor.Value;

        public int Skin { get; set; } = Game1.player.skin.Value;

        public int Hair { get; set; } = Game1.player.hair.Value;

        public int Shirt { get; set; } = Game1.player.shirt.Value;

        public int Pants { get; set; } = Game1.player.pants.Value;

        public int Accessory { get; set; } = Game1.player.accessory.Value;

        public bool CatPerson { get; set; } = Game1.player.catPerson;

        public int Pet { get; set; } = Game1.player.whichPetBreed;

        public bool SkipIntro { get; set; } = false;
    }
}
