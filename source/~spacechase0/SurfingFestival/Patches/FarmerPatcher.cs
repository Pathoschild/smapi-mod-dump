/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;

namespace SurfingFestival.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Farmer"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class FarmerPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Farmer>(nameof(Farmer.draw), new[] { typeof(SpriteBatch) }),
                prefix: this.GetHarmonyMethod(nameof(Before_Draw)),
                postfix: this.GetHarmonyMethod(nameof(After_Draw))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="Farmer.draw(SpriteBatch)"/>.</summary>
        public static void Before_Draw(Character __instance, SpriteBatch b)
        {
            if (Game1.CurrentEvent?.FestivalName != Mod.FestivalName || Game1.CurrentEvent?.playerControlSequenceID != "surfingRace")
                return;

            Mod.DrawSurfboard(__instance, b);
        }

        /// <summary>The method to call after <see cref="Farmer.draw(SpriteBatch)"/>.</summary>
        public static void After_Draw(Character __instance, SpriteBatch b)
        {
            if (Game1.CurrentEvent?.FestivalName != Mod.FestivalName || Game1.CurrentEvent?.playerControlSequenceID != "surfingRace")
                return;

            Mod.DrawSurfingStatuses(__instance, b);
        }
    }
}
