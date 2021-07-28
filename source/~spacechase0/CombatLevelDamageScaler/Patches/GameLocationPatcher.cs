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
using Microsoft.Xna.Framework;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;

namespace CombatLevelDamageScaler.Patches
{
    /// <summary>Applies Harmony patches to <see cref="GameLocation"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class GameLocationPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<GameLocation>(nameof(GameLocation.damageMonster), new[] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) }),
                prefix: this.GetHarmonyMethod(nameof(Before_DamageMonster))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="GameLocation.damageMonster(Rectangle,int,int,bool,Farmer)"/>.</summary>
        private static void Before_DamageMonster(ref int minDamage, ref int maxDamage, Farmer who)
        {
            float scale = 1.0f + who.CombatLevel * Mod.Config.DamageScalePerLevel;
            minDamage = (int)(minDamage * scale);
            maxDamage = (int)(maxDamage * scale);
        }
    }
}
