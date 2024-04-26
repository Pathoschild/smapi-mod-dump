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
using StardewValley.Locations;
using StardewValley.Menus;
using System.Diagnostics;
using Object = StardewValley.Object;

namespace GeodePreview
{
    internal static class Patches
    {
        private static readonly Item goldenCoconut = ItemRegistry.Create("(O)73");
        private static readonly Rectangle guntherSourceRect = new(27, 117, 9, 9);
        private static readonly Texture2D emojis = Game1.content.Load<Texture2D>("LooseSprites\\emojis");

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
            Item? treasure = Utility.getTreasureFromGeode(__instance); //It didn't work... Attempt #2
            if (treasure == null) 
                return;
            if (__instance.QualifiedItemId == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked)
                treasure = goldenCoconut;
            treasure.drawInMenu(spriteBatch, location + new Vector2(24, -24), .5f, 1f, layerDepth + .1f, StackDrawType.Hide, Color.White, false);
            if (ModEntry.Config.ShowStack)
                Utility.drawTinyDigits(treasure.Stack, spriteBatch, location + new Vector2(56, 8), 2f, layerDepth + .15f, Color.White);
            if (!hasDonatedToMuseum(treasure) && ModEntry.Config.ShowMuseumHint)
                spriteBatch.Draw(emojis, location + new Vector2(56, 0), guntherSourceRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth + .15f);
        }

        private static bool shouldShow() => ModEntry.Config.ShowAlways || Game1.activeClickableMenu is GeodeMenu;

        internal static bool Utility_CreateRandom_Prefix(double seedA, double seedB, ref Random __result)
        {
            //If you find a better way to check the call stack at runtime, please tell me
            bool flag = false;
            foreach (var frame in new StackTrace().GetFrames())
            {
                if (frame.GetMethod().Name == nameof(Object_DrawInMenu_Postfix))
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                __result = new Random(Utility.CreateRandomSeed(seedA + ModEntry.Config.Offset, seedB));
                return false;
            }
            return true;
        }

        private static bool hasDonatedToMuseum(Item o)
        {
            if (!LibraryMuseum.IsItemSuitableForDonation(o.ItemId))
                return true;
            foreach (var item in Game1.netWorldState.Value.MuseumPieces.Values)
                if (item == o.ItemId)
                    return true;
            return false;
        }
    }
}
