/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.Common
{
    /// <summary>The common reasons why diagnostic warnings are suppressed.</summary>
    internal static class SuppressReasons
    {
        /// <summary>A message when suppressed unused-code warnings because Json.NET calls it automatically.</summary>
        public const string UsedViaOnDeserialized = "This method is called by Json.NET automatically based on the [OnDeserialized] attribute.";

        /// <summary>A message when suppressing redundant-null-check warnings because this is the method that validates them.</summary>
        public const string MethodValidatesNullability = "This is the method that prevents null values in the rest of the code.";
    }
}
