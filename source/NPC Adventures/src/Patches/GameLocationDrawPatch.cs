/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.Events;
using PurrplingCore.Patching;
using StardewValley;
using System;

namespace NpcAdventure.Patches
{
    internal class GameLocationDrawPatch : Patch<GameLocationDrawPatch>
    {
        private SpecialModEvents Events { get; set; }

        public override string Name => nameof(GameLocationDrawPatch);

        /// <summary>
        /// Creates instance of location draw game patch
        /// </summary>
        /// <param name="events"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public GameLocationDrawPatch(SpecialModEvents events)
        {
            this.Events = events ?? throw new ArgumentNullException(nameof(events));
            Instance = this;
        }

        private static void After_draw(ref GameLocation __instance, SpriteBatch b)
        {
            try
            {
                Instance.Events.FireRenderedLocation(__instance, new LocationRenderedEventArgs(b));
            }
            catch (Exception ex)
            {
                Instance.LogFailure(ex, nameof(After_draw));
            }
        }

        protected override void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.draw)),
                postfix: new HarmonyMethod(typeof(GameLocationDrawPatch), nameof(GameLocationDrawPatch.After_draw))
            );
        }
    }
}
