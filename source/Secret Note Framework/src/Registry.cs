/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/SecretNoteFramework
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;

namespace ichortower.SNF
{
    internal class GameStateQueries
    {
        public static void Register()
        {
            GameStateQuery.Register($"{SNF.ModId}_PLAYER_HAS_MOD_NOTE",
                    PLAYER_HAS_MOD_NOTE);
        }

        public static bool PLAYER_HAS_MOD_NOTE(
                string[] query, GameStateQueryContext context)
        {
            if (!ArgUtility.TryGet(query, 1, out var playerKey, out var error) ||
                    !ArgUtility.TryGet(query, 2, out var modNoteId, out error)) {
                return GameStateQuery.Helpers.ErrorResult(query, error);
            }
            return GameStateQuery.Helpers.WithPlayer(context.Player, playerKey,
                    (Farmer target) => ModData.HasNote(target, modNoteId));
        }
    }

    internal class CPTokens
    {
        public static void Register()
        {
            var cpapi = SecretNoteFramework.instance.Helper.ModRegistry
                    .GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (cpapi == null) {
                Log.Warn("Could not load Content Patcher API");
                return;
            }
            cpapi.RegisterToken(SecretNoteFramework.instance.ModManifest,
                    "HasModNote", () => {
                return new[] {ModData.NotesAsToken(Game1.player)};
            });
        }
    }

    internal class TriggerActions
    {
        public static void Register()
        {
            TriggerActionManager.RegisterAction($"{SNF.ModId}_MarkModNoteSeen",
                    MarkModNoteSeen);
        }

        public static bool MarkModNoteSeen(string[] args,
                TriggerActionContext context,
                out string error)
        {
            if (!ArgUtility.TryGet(args, 1, out var playerKey, out error) ||
                    !ArgUtility.TryGet(args, 2, out var modNoteId, out error)) {
                return false;
            }
            _ = ArgUtility.TryGetOptionalBool(args, 3, out bool isSeen, out error, true);
            if (string.Equals(playerKey, "All", StringComparison.OrdinalIgnoreCase)) {
                foreach (Farmer f in Game1.getAllFarmers()) {
                    if (isSeen) {
                        ModData.AddNote(f, modNoteId);
                    }
                    else {
                        ModData.RemoveNote(f, modNoteId);
                    }
                }
                error = null;
                return true;
            }
            Farmer who = null;
            if (string.Equals(playerKey, "Current", StringComparison.OrdinalIgnoreCase)) {
                who = Game1.player;
            }
            else if (string.Equals(playerKey, "Host", StringComparison.OrdinalIgnoreCase)) {
                who = Game1.MasterPlayer;
            }
            else if (long.TryParse(playerKey, out var parsedId)) {
                who = Game1.getFarmerMaybeOffline(parsedId);
            }
            if (who == null) {
                error = $"In action '{String.Join(" ", args)}': " +
                        $"specified farmer '{playerKey}' not found";
                return false;
            }
            if (isSeen) {
                ModData.AddNote(who, modNoteId);
            }
            else {
                ModData.RemoveNote(who, modNoteId);
            }
            return true;
        }
    }

    internal class ConsoleCommands
    {
        public static void Register()
        {
            SecretNoteFramework.instance.Helper.ConsoleCommands.Add(
                    "snf_reload",
                    "Reloads modded secret note data and/or rechecks note conditions." +
                        " 'snf_reload help' for details.",
                    SNF_Reload);
        }

        public static string[] HelpText = {
            "snf_reload refreshes modded secret note data. It is intended for use in debugging.",
            "",
            "snf_reload <target>",
            "   target should be one of 'data', 'check', 'full'.",
            "",
            $"   data: invalidate and reload the data asset ('{SecretModNotes.NotesAsset}').",
            "   check: reevaluate conditions on modded notes.",
            "   full: reload data and reevaluate conditions.",
        };

        public static void SNF_Reload(string command, string[] args)
        {
            if (args.Length == 0) {
                args = new string[1] {"help"};
            }
            switch (args[0].ToLower()) {
            case "data":
                SecretNoteFramework.instance.Helper.GameContent.InvalidateCache(
                        SecretModNotes.NotesAsset);
                break;
            case "check":
                SecretModNotes.RefreshAvailableNotes();
                Log.Info("Refreshed note conditions.");
                break;
            case "full":
                SecretNoteFramework.instance.Helper.GameContent.InvalidateCache(
                        SecretModNotes.NotesAsset);
                SecretModNotes.RefreshAvailableNotes();
                Log.Info("Refreshed note conditions.");
                break;
            case "help":
            default:
                string output = String.Join(Environment.NewLine, HelpText);
                Log.Info(output);
                break;
            }
        }
    }
}

#nullable enable

namespace ContentPatcher
{
    public interface IContentPatcherAPI
    {
        void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>?> getValue);
    }
}

#nullable disable
