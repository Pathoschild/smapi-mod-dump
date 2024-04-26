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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Linq;
using System.Text;

namespace EasyIslandPuzzle
{
    internal class ModEntry : Mod
    {
        private static readonly Queue<Vector2> Sequence = [];
        private static readonly Rectangle sourceRect = new(403, 496, 5, 14);
        private static Vector2 Next = Vector2.Zero;

        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.GameLaunched += onGameLaunch;
        }

        private void onGameLaunch(object? sender, GameLaunchedEventArgs e)
        {
            Harmony harmony = new(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(IslandWestCave1.CaveCrystal), nameof(IslandWestCave1.CaveCrystal.activate)),
                postfix: new(typeof(ModEntry), nameof(activatePostfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(IslandWestCave1.CaveCrystal), nameof(IslandWestCave1.CaveCrystal.draw)),
                postfix: new(typeof(ModEntry), nameof(drawPostfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(IslandWestCave1), nameof(IslandWestCave1.resetPuzzle)),
                postfix: new(typeof(ModEntry), nameof(resetPuzzlePostfix))
            );
        }
        
        private static void activatePostfix(IslandWestCave1.CaveCrystal __instance)
        {
            if (Game1.didPlayerJustClickAtAll())
            {
                if (Sequence.TryDequeue(out var tile))
                    Next = tile;
                else
                    Next = Vector2.Zero;
                return;
            }
            if (Next == Vector2.Zero)
                Next = __instance.tileLocation;
            else
                Sequence.Enqueue(__instance.tileLocation);
        }

        private static void drawPostfix(IslandWestCave1.CaveCrystal __instance, SpriteBatch b)
        {
            if (__instance.tileLocation != Next)
                return;
            float yBob = (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500.0f + (__instance.tileLocation.X / 16)) * 2f;
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(__instance.tileLocation * 64f + new Vector2(32f, -12f + yBob) * 4f), sourceRect, Color.White, 0f, new Vector2(69f, 44f) / 2, 3f, SpriteEffects.None, (__instance.tileLocation.Y * 64 + 64 - 8) / 10000 + .1f);
        }

        private static void resetPuzzlePostfix()
        {
            Sequence.Clear();
            Next = Vector2.Zero;
        }
    }
}
