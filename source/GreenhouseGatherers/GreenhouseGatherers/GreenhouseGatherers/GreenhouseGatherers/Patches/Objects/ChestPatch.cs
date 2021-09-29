/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/


using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace GreenhouseGatherers.GreenhouseGatherers.Patches.Objects
{
    internal class ChestPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Chest);

        internal ChestPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Chest.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
        }

        [HarmonyBefore(new string[] { "spacechase0.DynamicGameAssets", "furyx639.ExpandedStorage" })]
        private static bool DrawPrefix(Object __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (__instance.modData.ContainsKey(ModEntry.harvestStatueFlag) && __instance is Chest chest)
            {
                float draw_x = x;
                float draw_y = y;
                if (chest.localKickStartTile.HasValue)
                {
                    draw_x = Utility.Lerp(chest.localKickStartTile.Value.X, draw_x, chest.kickProgress);
                    draw_y = Utility.Lerp(chest.localKickStartTile.Value.Y, draw_y, chest.kickProgress);
                }
                float base_sort_order = System.Math.Max(0f, ((draw_y + 1f) * 64f - 24f) / 10000f) + draw_x * 1E-05f;
                if (chest.localKickStartTile.HasValue)
                {
                    spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((draw_x + 0.5f) * 64f, (draw_y + 0.5f) * 64f)), Game1.shadowTexture.Bounds, Color.Black * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 0.0001f);
                    draw_y -= (float)System.Math.Sin((double)chest.kickProgress * System.Math.PI) * 0.5f;
                }

                // Show a "filled" sprite or not, based on if the Harvest Statues has items
                spriteBatch.Draw(chest.items.Any() ? ModResources.filledStatue : ModResources.emptyStatue, Game1.GlobalToLocal(Game1.viewport, new Vector2(draw_x * 64f + (float)((chest.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (draw_y - 1f) * 64f)), new Rectangle(0, 0, 16, 32), chest.tint.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, base_sort_order);

                return false;
            }

            return true;
        }

        internal static void DayUpdatePostfix(Object __instance, GameLocation location)
        {
            if (__instance.modData.ContainsKey(ModEntry.harvestStatueFlag) && __instance is Chest chest)
            {
                // Do harvest logic here
            }
        }

        [HarmonyBefore(new string[] { "spacechase0.DynamicGameAssets", "furyx639.ExpandedStorage" })]
        internal static bool PlacementActionPrefix(Object __instance, GameLocation location, int x, int y, Farmer who = null)
        {
            if (__instance.DisplayName == "Harvest Statue")
            {
                if (location.IsOutdoors)
                {
                    _monitor.Log("Attempted to place Harvest Statue outdoors!", LogLevel.Trace);
                    Game1.showRedMessage("Harvest Statues can only be placed indoors!");

                    return false;
                }

                if (location.objects.Values.Any(o => o.modData.ContainsKey(ModEntry.harvestStatueFlag)))
                {
                    _monitor.Log("Attempted to place another Harvest Statue where there already is one!", LogLevel.Trace);
                    Game1.showRedMessage("You can only place one Harvest Statue per building!");

                    return false;
                }
            }

            return true;
        }
    }
}
