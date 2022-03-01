/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using PyTK.Types;
using StardewValley;
using StardewValley.Menus;

namespace Portraiture
{

    internal class FixHelper
    {
        public static Type getTypeFullSDV(string type)
        {
            Type defaulSDV = Type.GetType(type + ", Stardew Valley");

            if (defaulSDV != null)
                return defaulSDV;
            else
                return Type.GetType(type + ", StardewValley");

        }
    }

    [HarmonyPatch]
    internal class PortraitFix
    {
        internal static MethodInfo TargetMethod()
        {
            return FixHelper.getTypeFullSDV("StardewValley.NPC").GetProperty("Portrait").GetMethod;
        }

        internal static bool Prefix(NPC __instance, ref Texture2D __result)
        {
            __result = TextureLoader.getPortrait(__instance.Name);

            return __result == null;
        }


    }

    /*
    [HarmonyPatch]
    internal class DialogueBoxFix
    {
        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(FixHelper.getTypeFullSDV("StardewValley.Menus.DialogueBox"), "drawPortrait");
        }

        internal static bool Prefix(DialogueBox __instance, SpriteBatch b)
        {
            Dialogue characterDialogue = PortraitureMod.helper.Reflection.GetField<Dialogue>(__instance, "characterDialogue").GetValue();
            Texture2D texture = TextureLoader.getPortrait(characterDialogue.speaker.Name);
            characterDialogue.speaker.Portrait = texture;
            return true;
        }

       
    }

    [HarmonyPatch]
    internal class ShopMenuFix
    {
        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method(FixHelper.getTypeFullSDV("StardewValley.Menus.ShopMenu"), "draw");
        }

        internal static void Postfix(ShopMenu __instance, SpriteBatch b)
        {
            if (__instance.portraitPerson == null || !(Game1.viewport.Width > 800 && Game1.options.showMerchantPortraits))
                return;

            Texture2D texture = TextureLoader.getPortrait(__instance.portraitPerson.Name);
            __instance.portraitPerson.Portrait = texture;
            return;
        }
        
    }
    */

}
