using Harmony;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Patches
{
    internal class GameLocationDrawPatch
    {
        private static SpecialModEvents events;

        internal static void Postfix(ref GameLocation __instance, SpriteBatch b)
        {
            var args = new LocationRenderedEventArgs
            {
                SpriteBatch = b
            };

            events.HandleRenderedLocation(__instance, args);
        }

        internal static void Setup(HarmonyInstance harmony, ISpecialModEvents events)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "draw"),
                postfix: new HarmonyMethod(typeof(GameLocationDrawPatch), nameof(GameLocationDrawPatch.Postfix))
            );

            GameLocationDrawPatch.events = events as SpecialModEvents;
        }
    }
}
