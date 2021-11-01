/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace SuperHopper.Patches
{
    /// <summary>Applies Harmony patches to <see cref="SObject"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class ObjectPatcher : BasePatcher
    {
        /*********
        ** Fields
        *********/
        /// <summary>The method to call after a machine updates on time change.</summary>
        private static Action<SObject, GameLocation> OnMachineMinutesElapsed;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="onMachineMinutesElapsed">The method to call after a machine updates on time change.</param>
        public ObjectPatcher(Action<SObject, GameLocation> onMachineMinutesElapsed)
        {
            ObjectPatcher.OnMachineMinutesElapsed = onMachineMinutesElapsed;
        }

        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<SObject>(nameof(SObject.minutesElapsed)),
                postfix: this.GetHarmonyMethod(nameof(After_MinutesElapsed))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call after <see cref="SObject.minutesElapsed"/>.</summary>
        private static void After_MinutesElapsed(SObject __instance, GameLocation environment)
        {
            ObjectPatcher.OnMachineMinutesElapsed(__instance, environment);
        }
    }
}
