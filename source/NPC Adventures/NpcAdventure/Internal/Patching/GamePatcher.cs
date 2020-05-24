using Harmony;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NpcAdventure.Internal.Patching
{
    internal class GamePatcher
    {
        private readonly IMonitor monitor;
        private readonly bool paranoid;
        private readonly HarmonyInstance harmony;
        private readonly List<MethodBase> knownConflictedPatches;

        public GamePatcher(string uid, IMonitor monitor, bool paranoid = true)
        {
            this.monitor = monitor;
            this.paranoid = paranoid;
            this.knownConflictedPatches = new List<MethodBase>();
            this.harmony = HarmonyInstance.Create(uid);
        }

        /// <summary>
        /// Returns all patched methods by NPC Adventures mod.
        /// </summary>
        /// <returns>Enumerable patched methods</returns>
        public IEnumerable<PatchedMethod> GetPatchedMethods()
        {
            var methods = this.harmony
                .GetPatchedMethods()
                .ToArray();
            foreach (var method in methods)
            {
                Harmony.Patches info = this.harmony.GetPatchInfo(method);

                if (info != null && info.Owners.Contains(this.harmony.Id))
                {
                    yield return new PatchedMethod(method, info);
                }
            }
        }

        public void CheckPatches(bool hard = false)
        {
            if (hard)
            {
                this.knownConflictedPatches.Clear();
            }

            try
            {
                foreach (var patchedMethod in this.GetPatchedMethods())
                {
                    if (!this.knownConflictedPatches.Contains(patchedMethod.Method) && patchedMethod.PatchInfo.Owners.Count > 1)
                    {
                        IEnumerable<string> foreignOwners = this.GetForeignOwners(patchedMethod);

                        this.monitor.Log($"Detected another patches for game method '{patchedMethod.Method.FullDescription()}'. This method was patched too by: {string.Join(", ", foreignOwners)}",
                            this.paranoid ? LogLevel.Warn : LogLevel.Debug);
                        this.knownConflictedPatches.Add(patchedMethod.Method);
                    }
                }
            } catch (Exception ex)
            {
                this.monitor.Log("Unable to check game patches. See log for more details.", LogLevel.Error);
                this.monitor.Log(ex.ToString(), LogLevel.Trace);
            }
        }

        private IEnumerable<string> GetForeignOwners(PatchedMethod patchedMethod)
        {
            return patchedMethod.PatchInfo.Owners
                .Where(owner => owner != this.harmony.Id);
        }

        /// <summary>
        /// Apply game patches
        /// </summary>
        /// <param name="patches"></param>
        public void Apply(params IPatch[] patches)
        {
            foreach (IPatch patch in patches)
            {
                try
                {
                    patch.Apply(this.harmony, this.monitor);
                    this.monitor.Log($"Applied runtime patch '{patch.Name}' to the game.");
                } catch (Exception ex)
                {
                    this.monitor.Log($"Couldn't apply runtime patch '{patch.Name}' to the game. Some features may not works correctly. See log file for more details.", LogLevel.Error);
                    this.monitor.Log(ex.ToString(), LogLevel.Trace);
                }
            }
        }

        public class PatchedMethod
        {
            public PatchedMethod(MethodBase method, Harmony.Patches patchInfo)
            {
                this.Method = method;
                this.PatchInfo = patchInfo;
            }

            public MethodBase Method { get; }
            public Harmony.Patches PatchInfo { get; }
        }
    }
}
