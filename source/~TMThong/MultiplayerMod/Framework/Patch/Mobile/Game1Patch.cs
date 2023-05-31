/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    public class Game1Patch : IPatch
    {
        public static readonly Type PATCH_TYPE = typeof(Game1);
        public static IReflectedField<int> xEdgeField;
        public static IReflectedField<string> savesPathField;
        public static IReflectedField<Texture2D> mobileSpriteSheetField;
        public Game1Patch()
        {
            xEdgeField = ModUtilities.Helper.Reflection.GetField<int>(PATCH_TYPE, "xEdge");
            savesPathField = ModUtilities.Helper.Reflection.GetField<string>(PATCH_TYPE, "savesPath");
            mobileSpriteSheetField = ModUtilities.Helper.Reflection.GetField<Texture2D>(PATCH_TYPE, "mobileSpriteSheet");
        }


        public static Texture2D mobileSpriteSheet
        {
            get
            {
                return mobileSpriteSheetField.GetValue();
            }
            set { mobileSpriteSheetField.SetValue(value); }
        }

        public static int xEdge
        {
            get
            {
                return xEdgeField.GetValue();
            }
            set
            {
                xEdgeField.SetValue(value);
            }
        }
        public static string savesPath
        {
            get
            {
                return savesPathField.GetValue();
            }
            set
            {
                savesPathField.SetValue(value);
            }
        }

        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.PropertySetter(typeof(Game1), "currentLocation"), postfix: new HarmonyMethod(this.GetType(), nameof(postfix_SetLocation)));
        }

        private static void postfix_SetLocation(GameLocation __instance)
        {

            if (__instance != null)
            {
                var property = __instance.GetType().GetProperty("tapToMove");
                object TapToMove = typeof(IClickableMenu).Assembly.GetType("StardewValley.Mobile.TapToMove").CreateInstance<object>(new object[] { __instance });
                property.SetValue(__instance, TapToMove);
            }

        }
    }
}
