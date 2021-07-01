/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Spacechase.Shared.Harmony;
using SpaceCore.Events;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;

namespace SpaceCore.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Event"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class EventPatcher : BasePatcher
    {
        /*********
        ** Accessors
        *********/
        internal static Dictionary<string, MethodInfo> CustomCommands = new();


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Event>(nameof(Event.tryEventCommand)),
                prefix: this.GetHarmonyMethod(nameof(Before_TryEventCommand))
            );

            harmony.Patch(
                original: this.RequireMethod<Event>(nameof(Event.checkAction)),
                prefix: this.GetHarmonyMethod(nameof(Before_CheckAction))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="Event.tryEventCommand"/>.</summary>
        private static bool Before_TryEventCommand(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            if (split.Length == 0)
                return false;
            string name = split[0];

            object[] eventCommandArgs = SpaceCore.Reflection.GetField<object[]>(typeof(Event), "_eventCommandArgs").GetValue();
            var commandLookup = SpaceCore.Reflection.GetField<Dictionary<string, MethodInfo>>(typeof(Event), "_commandLookup").GetValue();

            if (EventPatcher.CustomCommands.TryGetValue(name, out MethodInfo customCommand))
                customCommand.Invoke(null, new object[] { __instance, location, time, split });
            else if (commandLookup.TryGetValue(name, out MethodInfo command))
            {
                eventCommandArgs[0] = location;
                eventCommandArgs[1] = time;
                eventCommandArgs[2] = split;
                command.Invoke(__instance, eventCommandArgs);
            }
            else
                Log.Warn($"ERROR: Invalid command: {name}");

            return false;
        }

        /// <summary>The method to call before <see cref="Event.checkAction"/>.</summary>
        private static bool Before_CheckAction(Event __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            string actionStr = Game1.currentLocation.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
            if (actionStr != null)
                return !SpaceEvents.InvokeActionActivated(who, actionStr, tileLocation);
            return true;
        }
    }
}
