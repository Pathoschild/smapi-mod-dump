/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.Internal;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace GeodePreview
{
    internal static class Patches
    {
        private static readonly Item goldenCoconut = ItemRegistry.Create("(O)73");
        private static bool preview = false;

        internal static void Patch(string id)
        {
            Harmony harmony = new(id);

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.drawInMenu), [typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool)]),
                postfix: new(typeof(Patches), nameof(Object_DrawInMenu_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.CreateRandom)),
                prefix: new(typeof(Patches), nameof(Utility_CreateRandom_Prefix))
            );
        }

        internal static void Object_DrawInMenu_Postfix(Object __instance, SpriteBatch spriteBatch, Vector2 location, float layerDepth)
        {
            if (!Utility.IsGeode(__instance) || __instance.QualifiedItemId.Contains("MysteryBox") || !shouldShow())
                return;
            preview = true; //I don't like this, but I'm not re-writing getTreasureFromGeode just for a preview
            Item? treasure = Utility.getTreasureFromGeode(__instance);
            preview = false; //It will work -famous last words
            if (treasure == null) 
                return;
            if (__instance.QualifiedItemId == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked)
                treasure = goldenCoconut;
            treasure.drawInMenu(spriteBatch, location + new Vector2(24, -24), .5f, 1f, layerDepth + .1f, StackDrawType.Hide, Color.White, false);
            if (ModEntry.Config.ShowStack)
                Utility.drawTinyDigits(treasure.Stack, spriteBatch, location + new Vector2(56, 8), 2f, layerDepth + .15f, Color.White);
        }

        private static bool shouldShow() => ModEntry.Config.ShowAlways || Game1.activeClickableMenu is GeodeMenu;

        internal static bool Utility_CreateRandom_Prefix(double seedA, double seedB, ref Random __result)
        {
            if (preview)
            {
                __result = new Random(Utility.CreateRandomSeed(seedA + 1, seedB));
                return false;
            }
            return true;
        }
    }
}
