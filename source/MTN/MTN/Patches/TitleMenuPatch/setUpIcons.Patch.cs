using Harmony;
using MTN.Menus;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.TitleMenuPatch
{
    //[HarmonyPatch(typeof(TitleMenu))]
    //[HarmonyPatch("setUpIcons")]
    public class setUpIconsPatch
    {
        public static bool Prefix()
        {
            if (TitleMenu.subMenu is CharacterCustomizationWithCustom)
            {
                return false;
            }
            return true;
        }
    }
}
