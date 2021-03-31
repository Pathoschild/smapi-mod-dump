/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Harmony;
using ImJustMatt.Common.Patches;
using ImJustMatt.UtilityChest.Framework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace ImJustMatt.UtilityChest.Framework.Patches
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ChestPatches : BasePatch<UtilityChest>
    {
        private static PerScreen<Chest> CurrentChest;

        public ChestPatches(IMod mod, HarmonyInstance harmony) : base(mod, harmony)
        {
            CurrentChest = Mod.CurrentChest;

            harmony.Patch(
                AccessTools.Method(typeof(Chest), nameof(Chest.drawInMenu), new[] {typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool)}),
                postfix: new HarmonyMethod(GetType(), nameof(DrawInMenuPostfix))
            );
        }

        public static void DrawInMenuPostfix(Chest __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (!ReferenceEquals(__instance, CurrentChest.Value) || __instance.CurrentItem() is not { } item) return;
            item.drawInMenu(spriteBatch, location, scaleSize * 0.8f, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }
    }
}