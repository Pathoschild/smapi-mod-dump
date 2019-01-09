using MTN2.Menus;
using StardewValley.Menus;


namespace MTN2.Patches.TitleMenuPatches
{
    public class setUpIconsPatch
    {
        public static bool Prefix() {
            return (TitleMenu.subMenu is CharacterCustomizationMTN) ? false : true;
        }
    }
}
