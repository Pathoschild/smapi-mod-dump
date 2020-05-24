using Harmony;
using NpcAdventure.Internal;
using NpcAdventure.Internal.Patching;
using StardewModdingAPI;
using System;

namespace NpcAdventure.Patches
{
    internal abstract class Patch<T> : IPatch where T : IPatch
    {
        private static SetOnce<T> instance = new SetOnce<T>();

        /// <summary>
        /// Reference to an internal game patch instance
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected static T Instance { get => instance.Value; set => instance.Value = value; }
        protected IMonitor Monitor { get; set; }
        public abstract string Name { get; }
        public bool Applied { get; private set; }

        protected abstract void Apply(HarmonyInstance harmony);

        /// <summary>
        /// Setup and apply game patch
        /// </summary>
        /// <param name="harmony"></param>
        /// <param name="monitor"></param>
        /// <exception cref="Exception">Any exception raised while appling game patch</exception>
        public void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            if (Instance == null)
            {
                throw new InvalidOperationException($"Cannot apply patch '{this.Name}' without static reference to itself!");
            }

            if (this.Applied || Instance.Applied)
            {
                throw new InvalidOperationException($"Patch '{this.Name}' is already applied!");
            }

            this.Monitor = monitor;
            this.Apply(harmony);
            this.Applied = true; // Set applied flag
        }

        protected void LogFailure(Exception ex, string patchMethodName)
        {
            this.Monitor.Log($"Failed in game patch {this.Name}.{patchMethodName}:\n{ex}", LogLevel.Error);
        }
    }
}
