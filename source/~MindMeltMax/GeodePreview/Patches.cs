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
using StardewModdingAPI.Utilities;
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
        private static readonly Item randomSeedDisplay = ItemRegistry.Create("(O)472");
        private static readonly Rectangle guntherSourceRect = new(27, 117, 9, 9);
        private static readonly Texture2D emojis = Game1.content.Load<Texture2D>("LooseSprites\\emojis");
        private static readonly PerScreen<bool> recursionFix = new(() => false);

        internal static void Patch(string id)
        {
            Harmony harmony = new(id);

            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.drawInMenu), [typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool)]),
                postfix: new(typeof(Patches), nameof(Object_DrawInMenu_Postfix))
            );
        }

        internal static void Object_DrawInMenu_Postfix(Object __instance, SpriteBatch spriteBatch, Vector2 location, float layerDepth)
        {
            if (!Utility.IsGeode(__instance) || !shouldShow() || recursionFix.Value)
                return;
            Item? treasure = getTreasure(__instance); //Better solution (possibly) provided by https://www.nexusmods.com/stardewvalley/users/122551688, Attempt #3
            if (treasure == null) 
                return;
            if (__instance.QualifiedItemId == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked)
                treasure = goldenCoconut;
            recursionFix.Value = true;
            if (treasure.Category == Object.SeedsCategory)
                randomSeedDisplay.drawInMenu(spriteBatch, location + new Vector2(24, -24), .5f, 1f, layerDepth + .1f, StackDrawType.Hide, Color.Black, false);
            else
                treasure.drawInMenu(spriteBatch, location + new Vector2(24, -24), .5f, 1f, layerDepth + .1f, StackDrawType.Hide, Color.White, false);
            recursionFix.Value = false;
            if (ModEntry.Config.ShowStack)
                Utility.drawTinyDigits(treasure.Stack, spriteBatch, location + new Vector2(56, 8), 2f, layerDepth + .15f, Color.White);
            if (!hasDonatedToMuseumOrNotDonatable(treasure) && ModEntry.Config.ShowMuseumHint)
                spriteBatch.Draw(emojis, location + new Vector2(56, 0), guntherSourceRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth + .15f);
        }

        private static bool shouldShow() => ModEntry.Config.ShowAlways || Game1.activeClickableMenu is GeodeMenu;

        private static bool hasDonatedToMuseumOrNotDonatable(Item o)
        {
            if (!LibraryMuseum.IsItemSuitableForDonation(o.ItemId))
                return true;
            foreach (var item in Game1.netWorldState.Value.MuseumPieces.Values)
                if (item == o.ItemId)
                    return true;
            return false;
        }

        private static Item? getTreasure(Item geode)
        {
            Item? output;
            if (geode.QualifiedItemId.Contains("MysteryBox"))
            {
                var opened = Game1.stats.Get("MysteryBoxesOpened");
                Game1.stats.Set("MysteryBoxesOpened", opened + (uint)ModEntry.Config.Offset);
                output = Utility.getTreasureFromGeode(geode);
                Game1.stats.Set("MysteryBoxesOpened", opened);
                return output;
            }
            Game1.stats.GeodesCracked += (uint)ModEntry.Config.Offset;
            output = Utility.getTreasureFromGeode(geode);
            Game1.stats.GeodesCracked -= (uint)ModEntry.Config.Offset;
            return output;
        }
    }
}
