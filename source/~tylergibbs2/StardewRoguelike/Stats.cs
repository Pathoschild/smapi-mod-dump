/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace StardewRoguelike
{
    /// <summary>
    /// The API response schema that we receive
    /// from the uploading server.
    /// </summary>
    public class StatsAPIResponse
    {
        public int status { get; set; }

        public string? result { get; set; } = null;

        public string? error { get; set; } = null;
    }

    /// <summary>
    /// Stores the statistics for a run and also has methods
    /// for printing and uploading the statistics online.
    /// </summary>
    public class Stats
    {
        public bool HardMode { get; set; } = false;

        public bool Multiplayer { get; set; } = false;

        public int PlayerCount { get; set; } = 0;

        public DateTime? StartTime { get; set; } = null;

        public DateTime? EndTime { get; set; } = null;

        public DateTime? DinoKillEndTime { get; set; } = null;

        public int MonstersKilled { get; set; } = 0;

        public int BossesDefeated { get; set; } = 0;

        public int FloorsDescended { get; set; } = 0;

        public int GoldSpent { get; set; } = 0;

        public int GoldEarned { get; set; } = 0;

        public int TotalHealing { get; set; } = 0;

        public int DamageDealt { get; set; } = 0;

        public int DamageTaken { get; set; } = 0;

        public string? Patch { get; set; } = null;

        public string? Seed { get; set; } = null;

        private int oldMoney = -1;

        private int oldHealth = -1;

        /// <summary>
        /// Resets all the statistics.
        /// </summary>
        public void Reset()
        {
            Multiplayer = false;
            PlayerCount = 0;
            StartTime = null;
            EndTime = null;
            DinoKillEndTime = null;
            MonstersKilled = 0;
            BossesDefeated = 0;
            FloorsDescended = 0;
            GoldSpent = 0;
            GoldEarned = 0;
            TotalHealing = 0;
            DamageDealt = 0;
            DamageTaken = 0;
            Patch = null;
            Seed = null;
        }

        /// <summary>
        /// Gets all the statistics as a list of user-friendly strings.
        /// </summary>
        /// <returns>Key:Value formatted strings with parsed statistics data.</returns>
        public List<string> GetLines()
        {
            List<string> lines = new();

            if (EndTime is not null && StartTime is not null)
            {
                TimeSpan span = (TimeSpan)(EndTime - StartTime);
                lines.Add($"{I18n.Stats_Duration()}:{span:hh\\h\\ mm\\m\\ ss\\s}");
            }
            else if (EndTime is null && StartTime is not null)
            {
                TimeSpan span = (TimeSpan)(DateTime.UtcNow - StartTime);
                lines.Add($"{I18n.Stats_Duration()}:{span:hh\\h\\ mm\\m\\ ss\\s}");
            }
            else
                lines.Add($"{I18n.Stats_Duration()}:{I18n.Stats_NotStarted()}");

            if (DinoKillEndTime is not null && StartTime is not null)
            {
                TimeSpan span = (TimeSpan)(DinoKillEndTime - StartTime);
                lines.Add($"{I18n.Stats_CompletionTime()}:{span:hh\\h\\ mm\\m\\ ss\\s}");
            }
            else if (DinoKillEndTime is null && StartTime is not null && EndTime is null)
            {
                TimeSpan span = (TimeSpan)(DateTime.UtcNow - StartTime);
                lines.Add($"{I18n.Stats_CompletionTime()}:{span:hh\\h\\ mm\\m\\ ss\\s}");
            }
            else
                lines.Add($"{I18n.Stats_CompletionTime()}:{I18n.Stats_NotStarted()}");

            lines.Add($"{I18n.Stats_FloorsDescended()}:{FloorsDescended}");
            lines.Add($"{I18n.Stats_MonstersKilled()}:{MonstersKilled}");
            lines.Add($"{I18n.Stats_BossesDefeated()}:{BossesDefeated}");
            lines.Add($"{I18n.Stats_DamageDealt()}:{DamageDealt}");
            lines.Add($"{I18n.Stats_DamageTaken()}:{DamageTaken}");
            lines.Add($"{I18n.Stats_TotalHealing()}:{TotalHealing}");
            lines.Add($"{I18n.Stats_GoldEarned()}:{GoldEarned}");
            lines.Add($"{I18n.Stats_GoldSpent()}:{GoldSpent}");

            return lines;
        }

        /// <summary>
        /// Uploads the statistics data online.
        /// </summary>
        /// <returns>True if success, False otherwise.</returns>
        public bool Upload()
        {
            string statsJson = JsonSerializer.Serialize(this);

            ModEntry.ModMonitor.Log($"Upload request content: {statsJson}", LogLevel.Debug);

            HttpRequestMessage request = new(
                HttpMethod.Post,
                $"{ModEntry.WebsiteBaseUrl}/api/runs"
            )
            {
                Content = new StringContent(
                    statsJson,
                    encoding: Encoding.UTF8,
                    mediaType: "application/json"
                )
            };

            HttpResponseMessage response;
            try
            {
                response = ModEntry.WebClient.Send(request);
            }
            catch (HttpRequestException e)
            {
                ModEntry.ModMonitor.Log($"Server error when sending request: {e.Message}", LogLevel.Error);
                return false;
            }
            using var reader = new StreamReader(response.Content.ReadAsStream());

            string responseText = reader.ReadToEnd();
            ModEntry.ModMonitor.Log($"Upload response text: {responseText}", LogLevel.Trace);
            StatsAPIResponse? runIdResponse = null;
            try
            {
                runIdResponse = JsonSerializer.Deserialize<StatsAPIResponse>(responseText);
            }
            catch (JsonException e)
            {
                ModEntry.ModMonitor.Log($"JSON Error when deserializing upload response: {e.Message}", LogLevel.Error);
                return false;
            }

            if (runIdResponse is null)
            {
                ModEntry.ModMonitor.Log("JSON Error when deserializing upload response: deserialized response is null", LogLevel.Error);
                return false;
            }
            else if (runIdResponse.error is not null)
            {
                ModEntry.ModMonitor.Log($"Server error when receiving response: {runIdResponse.error}", LogLevel.Error);
                return false;
            }
            else if (runIdResponse.result is not null)
            {
                string runId = runIdResponse.result;
                ModEntry.ModMonitor.Log($"Run successfully uploaded, ID is '{runId}'", LogLevel.Debug);
                var ps = new ProcessStartInfo($"{ModEntry.WebsiteBaseUrl}/claim?id={runId}")
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
                return true;
            }
            else
            {
                ModEntry.ModMonitor.Log("An unknown error occured when uploading the run.", LogLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// Event handler for checking if certain stats have changed.
        /// This handles checking for gold and HP differences.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UpdateTickedEventArgs"/> instance containing the event data.</param>
        public void WatchChanges(object? sender, UpdateTickedEventArgs e)
        {
            if (Roguelike.CurrentLevel == 0)
                return;

            if (oldMoney == -1)
                oldMoney = Game1.player.Money;

            if (Game1.player.Money != oldMoney && EndTime is null)
            {
                int difference = Game1.player.Money - oldMoney;
                if (difference > 0)
                    GoldEarned += difference;
                else if (difference < 0)
                    GoldSpent += Math.Abs(difference);
                oldMoney = Game1.player.Money;
            }

            if (Game1.player.health != oldHealth && EndTime is null)
            {
                int difference = Game1.player.health - oldHealth;
                if (difference > 0)
                    TotalHealing += difference;
                else if (difference < 0)
                    DamageTaken += Math.Abs(difference);
                oldHealth = Game1.player.health;
            }
        }
    }
}
