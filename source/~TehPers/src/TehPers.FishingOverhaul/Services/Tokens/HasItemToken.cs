/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewValley;

namespace TehPers.FishingOverhaul.Services.Tokens
{
    [SuppressMessage(
        "Style",
        "IDE0060:Remove unused parameter",
        Justification = "Used by Content Patcher."
    )]
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Used by Content Patcher."
    )]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by Content Patcher.")]
    internal class HasItemToken
    {
        public bool AllowsInput()
        {
            return true;
        }

        public bool CanHaveMultipleValues(string? input = null)
        {
            return true;
        }

        public bool IsReady()
        {
            return Game1.player is not null;
        }

        public IEnumerable<string> GetValues(string? input)
        {
            // Ensure input is not null
            if (input is null)
            {
                return Enumerable.Empty<string>();
            }

            // Split input
            var args = input.Split(',', StringSplitOptions.TrimEntries);
            if (args.Length != 2)
            {
                return Enumerable.Empty<string>();
            }

            // Get item ID
            if (!int.TryParse(args[0], out var index))
            {
                return Enumerable.Empty<string>();
            }

            // Get quantity
            if (!int.TryParse(args[1], out var quantity))
            {
                return Enumerable.Empty<string>();
            }

            // Get player's archaeology
            if (Game1.player is not { } player)
            {
                return Enumerable.Empty<string>();
            }

            return new[] { player.hasItemInInventory(index, quantity) ? "true" : "false" };
        }
    }
}