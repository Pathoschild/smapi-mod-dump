/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System.Runtime.CompilerServices;

namespace SonoCore
{
    /// <summary>Contains helpful assertions.</summary>
    /// <remarks>Assertion failures are error logged.</remarks>
    public static class Assert
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Verifies that two values are equal.</summary>
        /// <typeparam name="T">The type of the values to compare.</typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="callerFilePath">The file containing the code which called the assertion.</param>
        /// <param name="callerMemberName">The member which called the assertion.</param>
        /// <param name="callerLineNumber">The line number of the code which called the assertion.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AreEqual<T>(T expectedValue, T actualValue, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            if (expectedValue.Equals(actualValue))
                return;

            ModBase.Instance.Monitor.Log($"Assertion failed in {callerFilePath} in {callerMemberName} at Line {callerLineNumber}. ExpectedValue: {expectedValue}, ActualValue: {actualValue}", LogLevel.Error);
        }
    }
}
