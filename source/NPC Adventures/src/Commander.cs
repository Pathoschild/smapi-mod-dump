/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using System;
using System.Linq;
using Harmony;
using NpcAdventure.StateMachine;
using NpcAdventure.Story;
using NpcAdventure.Utils;
using StardewModdingAPI;
using StardewValley;

namespace NpcAdventure
{
    internal class Commander
    {
        private readonly NpcAdventureMod npcAdventureMod;
        private readonly IMonitor monitor;

        private Commander(NpcAdventureMod npcAdventureMod)
        {
            this.npcAdventureMod = npcAdventureMod;
            this.monitor = npcAdventureMod.Monitor;
            this.SetupCommands(npcAdventureMod.Helper.ConsoleCommands);
        }

        internal void SetupCommands(ICommandHelper consoleCommands)
        {
            if (!this.npcAdventureMod.Config.EnableDebug)
                return;

            consoleCommands.Add("npcadventure_eligible", "Make player eligible to recruit a companion (server or singleplayer only)", this.Eligible);
            consoleCommands.Add("npcadventure_recruit", "Recruit an NPC as companion (server or singleplayer only)", this.Recruit);
            consoleCommands.Add("npcadventure_patches", "List harmony patches applied by NPC Adventures\n\nUsage: npcadventure_patches [recheck]\n\n- recheck - Recheck conflictiong patches", this.GetPatches);
            consoleCommands.Add("npcadventure_debug", "Set a debug flag\n\nUsage: npcadventure_debug set|unset|list <flagName>", this.SetDebugFlag);
            this.monitor.Log("Registered debug commands", LogLevel.Info);
        }

        private void SetDebugFlag(string command, string[] args)
        {
            if (args.Length > 0)
            {
                switch(args[0])
                {
                    case "set":
                        if (args.Length > 1)
                            NpcAdventureMod.DebugFlags.Add(args[1]);
                        return;
                    case "unset":
                        if (args.Length > 1)
                            NpcAdventureMod.DebugFlags.Remove(args[1]);
                        return;
                    case "list":
                        this.monitor.Log($"Active debug flags:\n {string.Join("\n", NpcAdventureMod.DebugFlags)}", LogLevel.Info);
                        return;
                }
            }

            this.monitor.Log($"Invalid arguments.", LogLevel.Info);
        }

        private void Eligible(string command, string[] args)
        {
            if (Context.IsWorldReady && Context.IsMainPlayer && this.npcAdventureMod.GameMaster.Mode == GameMasterMode.MASTER)
            {
                this.npcAdventureMod.GameMaster.Data.GetPlayerState(Game1.player).isEligible = true;
                this.npcAdventureMod.GameMaster.SyncData();
                this.monitor.Log("Player is now eligible for recruit companion.", LogLevel.Info);
            } else
            {
                this.monitor.Log("Can't eligible player when game is not loaded, in non-adventure mode or not running on server!", LogLevel.Alert);
            }
        }

        private void GetPatches(string command, string[] args)
        {
            string describePatch(Patch p) => $"{p.patch.ReflectedType.FullName}.{p.patch.Name}";

            if (args.Length > 0 && args[0] == "recheck")
            {
                this.npcAdventureMod.Patcher.CheckPatches(true);
                return;
            }

            foreach (var patchedMethod in this.npcAdventureMod.Patcher.GetPatchedMethods())
            {
                this.monitor.Log($"{(patchedMethod.PatchInfo.Owners.Count > 1 ? "* " : "")}Patched method '{patchedMethod.Method.FullDescription()}'", LogLevel.Info);
                this.monitor.Log($"   patched by: {string.Join(", ", patchedMethod.PatchInfo.Owners)}", LogLevel.Info);
                this.monitor.Log($"   applied prefixes: {string.Join(", ", patchedMethod.PatchInfo.Prefixes.Select(describePatch))}", LogLevel.Info);
                this.monitor.Log($"   applied postfixes: {string.Join(", ", patchedMethod.PatchInfo.Postfixes.Select(describePatch))}", LogLevel.Info);
            }

            this.monitor.Log("* - This patched method was patched by any other mod", LogLevel.Info);
        }

        private void Recruit(string command, string[] args)
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
            {
                this.monitor.Log("Can't recruit a companion when game is not loaded or player is not main player.", LogLevel.Alert);
                return;
            }

            if (args.Length < 1)
            {
                this.monitor.Log("Missing NPC name", LogLevel.Info);
                return;
            }

            string npcName = args[0];
            Farmer farmer = this.npcAdventureMod.CompanionManager.Farmer;
            CompanionStateMachine recruited = this.npcAdventureMod
                .CompanionManager
                .PossibleCompanions
                .Values
                .FirstOrDefault((_csm) => _csm.CurrentStateFlag == CompanionStateMachine.StateFlag.RECRUITED);

            if (recruited != null)
            {
                this.monitor.Log($"You have recruited ${recruited.Name}, unrecruit them first!", LogLevel.Info);
                return;
            }

            if (!this.npcAdventureMod.CompanionManager.PossibleCompanions.TryGetValue(npcName, out CompanionStateMachine csm) || csm.Companion == null)
            {
                this.monitor.Log($"Cannot recruit '{npcName}' - NPC is not recruitable or doesn't exists.", LogLevel.Error);
                return;
            }

            Helper.WarpTo(csm.Companion, farmer.currentLocation, farmer.getTileLocationPoint());
            csm.Recruit();
        }

        public static Commander Register(NpcAdventureMod mod)
        {
            return new Commander(mod);
        }
    }
}
