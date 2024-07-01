/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/JojaFinancial
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NermNermNerm.Stardew.LocalizeFromSource;
using StardewModdingAPI;
using StardewModdingAPI.Framework.Logging;

namespace JojaFinancial.Tests
{
    internal class StubMonitor
        : IMonitor
    {
        List<string> Errors { get; } = new List<string>();
        List<string> Warnings { get; } = new List<string>();
        List<string> Info { get; } = new List<string>();

        public bool IsVerbose => false;

        public void Log(string message, LogLevel level = LogLevel.Trace)
        {
            switch (level)
            {
                case LogLevel.Error:
                    this.Errors.Add(message);
                    break;
                case LogLevel.Warn:
                    this.Warnings.Add(message);
                    break;
                case LogLevel.Info:
                    this.Info.Add(message);
                    break;
                default:
                    break;
            }
        }

        public void LogOnce(string message, LogLevel level = LogLevel.Trace)
        {
            this.Log(message, level);
        }

        public void VerboseLog(string message)
        {
        }

        public void VerboseLog([InterpolatedStringHandlerArgument("")] ref VerboseLogStringHandler message)
        {
        }

        private static object modEntryLock = new object();

        /// <summary>
        ///   Runs mod.ModEntry, but wraps that call to deal with localization and inserts property values and much
        ///   other reflection shenanigans.
        /// </summary>
        public static void PrepMod(Mod mod, StubMonitor stubMonitor, StubModHelper stubHelper)
        {
            var prop = typeof(Mod).GetProperty(nameof(mod.Monitor), BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            prop!.SetValue(mod, stubMonitor);
            prop = typeof(Mod).GetProperty(nameof(mod.Helper), BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            prop!.SetValue(mod, stubHelper);

            // mod.Entry is expected to set up Localization, which, alas, mucks with some static
            // properties.  Lacking a better plan, we'll use reflection to hijack that process
            // and do it under a lock to enable multithreaded test-running, if we ever get that far.
            lock (modEntryLock) {
                var sdvLocalizeMethodsType = typeof(SdvLocalize);
                var translatorField = sdvLocalizeMethodsType.GetField("translator", BindingFlags.NonPublic | BindingFlags.Static)!;
                var oldTranslatorValue = (Translator?)translatorField.GetValue(null);
                translatorField.SetValue(null, null); // Make it uninitialized so it doesn't complain about being initialized twice.
                mod.Entry(stubHelper);
                if (oldTranslatorValue != null)
                {
                    // Undo the creation of the new translator entry so that we don't spend time attempting to re-reading the default.json
                    translatorField.SetValue(mod, oldTranslatorValue);
                }

                ((Translator)translatorField.GetValue(null)!).DoPseudoLoc = false;
            }
        }
    }
}
