/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

namespace DeluxeJournal.Patching
{
    /// <summary>Base class for singleton IPatches.</summary>
    /// <typeparam name="T">Inheriting type to create an instance of.</typeparam>
    internal abstract class PatchBase<T> : IPatch where T : IPatch
    {
        private static T? _instance;

        /// <summary>Patch instance. This should be set by the derived class.</summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException(typeof(T).FullName + " instance accessed before assignment!");
                }

                return _instance;
            }

            protected set
            {
                if (_instance != null)
                {
                    throw new InvalidOperationException(typeof(T).FullName + " has already been instantiated!");
                }

                _instance = value;
            }
        }

        public string Name => GetType().Name;

        protected IMonitor Monitor { get; }

        public PatchBase(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public abstract void Apply(Harmony harmony);

        /// <remarks>This is a <see cref="Harmony.Patch"/> wrapper that includes logging.</remarks>
        /// <param name="harmony">The Harmony instance.</param>
        /// <inheritdoc cref="Harmony.Patch"/>
        protected MethodInfo Patch(Harmony harmony, MethodBase original, HarmonyMethod? prefix = null, HarmonyMethod? postfix = null, HarmonyMethod? transpiler = null, HarmonyMethod? finalizer = null)
        {
            string format = "Applying Harmony patch '{0}': {1} on SDV method '{2}'.";
            
            if (prefix != null)
            {
                Monitor.Log(string.Format(format, Name, nameof(prefix), original.Name));
            }

            if (postfix != null)
            {
                Monitor.Log(string.Format(format, Name, nameof(postfix), original.Name));
            }

            if (transpiler != null)
            {
                Monitor.Log(string.Format(format, Name, nameof(transpiler), original.Name));
            }

            if (finalizer != null)
            {
                Monitor.Log(string.Format(format, Name, nameof(finalizer), original.Name));
            }

            return harmony.Patch(original, prefix, postfix, transpiler, finalizer);
        }

        /// <summary>Log an error that occurred inside a patch method.</summary>
        /// <param name="ex">Error exception.</param>
        /// <param name="methodName">The name of the patch method that produced the error.</param>
        protected void LogError(Exception ex, string methodName)
        {
            Monitor.Log($"Error occurred in Harmony patch {Name}.{methodName}: {ex}", LogLevel.Error);
        }
    }
}
