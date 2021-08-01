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
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;

namespace SurfingFestival.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Event"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class EventPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(Harmony harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Event>(nameof(Event.setUpPlayerControlSequence)),
                postfix: this.GetHarmonyMethod(nameof(After_SetUpPlayerControlSequence))
            );

            harmony.Patch(
                original: this.RequireMethod<Event>(nameof(Event.setUpFestivalMainEvent)),
                postfix: this.GetHarmonyMethod(nameof(After_SetUpFestivalMainEvent))
            );

            harmony.Patch(
                original: this.RequireMethod<Event>(nameof(Event.draw)),
                postfix: this.GetHarmonyMethod(nameof(After_Draw))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call after <see cref="Event.setUpPlayerControlSequence"/>.</summary>
        private static void After_SetUpPlayerControlSequence(Event __instance, string id)
        {
            if (id == "surfing")
            {
                Mod.Instance.Helper.Reflection.GetField<NPC>(__instance, "festivalHost").SetValue(__instance.getActorByName("Lewis"));
                Mod.Instance.Helper.Reflection.GetField<string>(__instance, "hostMessage").SetValue("$q -1 null#Ready for the race?#$r -1 0 yes#Yes, let's start.#$r -1 0 no#Not yet.");
            }
        }

        /// <summary>The method to call after <see cref="Event.setUpFestivalMainEvent"/>.</summary>
        private static void After_SetUpFestivalMainEvent(Event __instance)
        {
            if (!__instance.isSpecificFestival("summer5"))
                return;

            // ...
        }

        /// <summary>The method to call after <see cref="Event.draw"/>.</summary>
        private static void After_Draw(Event __instance, SpriteBatch b)
        {
            if (!__instance.isSpecificFestival("summer5"))
                return;

            Mod.Instance.DrawObstacles(b);
        }
    }
}
