using Harmony;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.Events;
using NpcAdventure.Internal;
using StardewValley;

namespace NpcAdventure.Patches
{
    internal class GameLocationDrawPatch
    {
        private static readonly SetOnce<SpecialModEvents> events = new SetOnce<SpecialModEvents>();
        private static SpecialModEvents Events { get => events.Value; set => events.Value = value; }

        internal static void After_draw(ref GameLocation __instance, SpriteBatch b)
        {
            Events.FireRenderedLocation(__instance, new LocationRenderedEventArgs(b));
        }

        internal static void Setup(HarmonyInstance harmony, ISpecialModEvents events)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.draw)),
                postfix: new HarmonyMethod(typeof(GameLocationDrawPatch), nameof(GameLocationDrawPatch.After_draw))
            );

            Events = events as SpecialModEvents;
        }
    }
}
