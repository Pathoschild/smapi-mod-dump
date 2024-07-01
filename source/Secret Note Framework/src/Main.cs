/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/SecretNoteFramework
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ichortower.SNF
{
    internal sealed class SecretNoteFramework : Mod
    {
        public static SecretNoteFramework instance;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            SNF.ModId = instance.ModManifest.UniqueID;

            helper.Events.Content.AssetRequested += SecretModNotes.OnAssetRequested;
            helper.Events.Content.AssetReady += SecretModNotes.OnAssetReady;
            helper.Events.Content.AssetsInvalidated += SecretModNotes.OnAssetsInvalidated;
            helper.Events.GameLoop.GameLaunched += SecretNoteFramework.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += SecretNoteFramework.OnDayStarted;
            helper.Events.GameLoop.ReturnedToTitle += SecretNoteFramework.OnReturnedToTitle;

            Patches.Apply();
            GameStateQueries.Register();
            TriggerActions.Register();
            ConsoleCommands.Register();
        }

        public static void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            CPTokens.Register();
        }

        public static void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            SecretModNotes.Data = SecretModNotes.Load(Game1.content);
            SecretModNotes.RefreshAvailableNotes();
        }

        public static void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            ModData.ClearCache();
        }
    }

    internal class SNF
    {
        public static string ModId;
    }

    internal class Log
    {
        public static void Trace(string text) {
            SecretNoteFramework.instance.Monitor.Log(text, LogLevel.Trace);
        }
        public static void Debug(string text) {
            SecretNoteFramework.instance.Monitor.Log(text, LogLevel.Debug);
        }
        public static void Info(string text) {
            SecretNoteFramework.instance.Monitor.Log(text, LogLevel.Info);
        }
        public static void Warn(string text) {
            SecretNoteFramework.instance.Monitor.Log(text, LogLevel.Warn);
        }
        public static void Error(string text) {
            SecretNoteFramework.instance.Monitor.Log(text, LogLevel.Error);
        }
        public static void Alert(string text) {
            SecretNoteFramework.instance.Monitor.Log(text, LogLevel.Alert);
        }
        public static void Verbose(string text) {
            SecretNoteFramework.instance.Monitor.VerboseLog(text);
        }
    }

    internal class TR
    {
        public static string Get(string key) {
            return SecretNoteFramework.instance.Helper.Translation.Get(key);
        }
    }
}
