/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiplayerModChecker.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MultiplayerModChecker
{
    public class ModEntry : Mod
    {
        private List<MultiplayerReportData> _rawReports = new List<MultiplayerReportData>();
        private readonly List<string> _reports = new List<string>();
        private Config _config;
        public override void Entry(IModHelper helper)
        {
            this.Helper.Events.Multiplayer.PeerContextReceived += this.PeerConnected;
            this.Helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived;
            this._config = this.Helper.ReadConfig<Config>();
        }

        private void PeerConnected(object sender, PeerContextReceivedEventArgs e)
        {
            if (!Context.IsMainPlayer) return;

            var report = new MultiplayerReportData();
            report.TimeConnected = DateTime.Now;

            report.SmapiGameGameVersions.FarmhandHasSmapi = e.Peer.HasSmapi;

            report.FarmhandId = e.Peer.PlayerID;
            report.FarmhandName = Game1.getAllFarmers()
                .FirstOrDefault(f => f.UniqueMultiplayerID == e.Peer.PlayerID)
                ?.Name;

            if (string.IsNullOrEmpty(report.FarmhandName))
            {
                report.FarmhandName = this.Helper.Translation.Get("UnnamedFarmhand");
            }

            if (e.Peer.HasSmapi)
            {
                report.SmapiGameGameVersions.HostSmapiVersion = Constants.ApiVersion;
                report.SmapiGameGameVersions.FarmhandSmapiVersion = e.Peer.ApiVersion;

                var HostMods = this.Helper.ModRegistry.GetAll().Select(m => m.Manifest.UniqueID);
                var farmHandMods = e.Peer.Mods.Select(m => m.ID);
                var allMods = HostMods.Union(farmHandMods).Distinct();

                foreach (string mod in allMods)
                {
                    if (this._config.IgnoredMods.Contains(mod)) continue;

                    var modVersionData = new ModVersions();

                    var hostMod = this.Helper.ModRegistry.Get(mod);
                    if (hostMod != null)
                    {
                        modVersionData.DoesHostHave = true;
                        modVersionData.HostModVersion = hostMod.Manifest.Version;
                    }


                    var farmhandMod = e.Peer.GetMod(mod);
                    if (farmhandMod != null)
                    {
                        modVersionData.DoesFarmhandHave = true;
                        modVersionData.FarmhandModVersion = farmhandMod.Version;
                    }

                    if (hostMod != null) modVersionData.ModName = hostMod.Manifest.Name;
                    else if (farmhandMod != null) modVersionData.ModName = farmhandMod.Name;

                    modVersionData.ModUniqueId = mod;
                    report.Mods.Add(modVersionData);
                }
            }

            this._rawReports.Add(report);
            this.GenerateReport(report);
        }

        private void Multiplayer_ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != this.ModManifest.UniqueID && e.Type != "MultiplayerReport") return;
            var reportData = e.ReadAs<Dictionary<string, LogLevel>>();

            this.PublishReport(null, reportData);
        }

        private void GenerateReport(MultiplayerReportData reportData)
        {
            Dictionary<string, LogLevel> report = new Dictionary<string, LogLevel>();

            if (!reportData.SmapiGameGameVersions.FarmhandHasSmapi)
            {
                report.Add(this.Helper.Translation.Get("FarmhandMissingSMAPI", new { FarmhandID = reportData.FarmhandId, FarmHandName = reportData.FarmhandName, ConnectionTime = reportData.TimeConnected }), LogLevel.Alert);
            }
            else
            {
                if (!reportData.SmapiGameGameVersions.HostSmapiVersion.Equals(reportData.SmapiGameGameVersions.FarmhandSmapiVersion))
                    report.Add(this.Helper.Translation.Get("SMAPIVersionMismatch",
                            new { HostVersion = reportData.SmapiGameGameVersions.HostSmapiVersion, FarmhandVersion = reportData.SmapiGameGameVersions.FarmhandSmapiVersion }),
                        LogLevel.Warn);

                foreach (var modData in reportData.Mods)
                {
                    if (!modData.DoesHostHave)
                    {
                        reportData.MissingOnHost.Add($"{modData.ModName} ({modData.ModUniqueId})");
                    }
                    else if (!modData.DoesFarmhandHave)
                    {
                        reportData.MissingOnFarmhand.Add($"{modData.ModName} ({modData.ModUniqueId})");
                    }
                    else if (!modData.HostModVersion.Equals(modData.FarmhandModVersion))
                    {
                        reportData.VersionMismatch.Add(this.Helper.Translation.Get("ModMismatch.Version.Each", new { ModName = modData.ModName, ModID = modData.ModUniqueId, HostVersion = modData.HostModVersion, FarmhandVersion = modData.FarmhandModVersion }));
                    }
                }
            }

            if (reportData.MissingOnHost.Count > 0)
                report.Add(this.Helper.Translation.Get("ModMismatch.Host", new { ModList = string.Join(",", reportData.MissingOnHost) }), LogLevel.Warn);

            if (reportData.MissingOnFarmhand.Count > 0)
                report.Add(this.Helper.Translation.Get("ModMismatch.Farmhand", new { ModList = string.Join(",", reportData.MissingOnFarmhand) }), LogLevel.Warn);

            if (reportData.VersionMismatch.Count > 0)
                report.Add(this.Helper.Translation.Get("ModMismatch.Version", new { ModList = string.Join(",", reportData.VersionMismatch) }), LogLevel.Warn);

            this.Helper.Multiplayer.SendMessage(report, "MultiplayerReport", new[] { this.ModManifest.UniqueID }, new[] { reportData.FarmhandId });
            this.Helper.Data.WriteJsonFile("LatestMultiplayerModReport-Host.json", this._rawReports);
            this.PublishReport(reportData, report);
        }

        private void PublishReport(MultiplayerReportData reportData, Dictionary<string, LogLevel> report)
        {
            string preface = "";
            if (report.Count > 0)
            {
                if (reportData == null)
                {
                    preface = this.Helper.Translation.Get("FarmhandReport");
                    this.Monitor.Log(preface, this._config.HideReportInTrace ? LogLevel.Trace : LogLevel.Warn);
                }
                else
                {
                    preface = this.Helper.Translation.Get("HostReport", new { FarmhandID = reportData.FarmhandId, FarmhandName = reportData.FarmhandName, ConnectionTime = reportData.TimeConnected });
                    this.Monitor.Log(preface, this._config.HideReportInTrace ? LogLevel.Trace : LogLevel.Warn);
                }

                foreach (var log in report)
                {
                    this.Monitor.Log(log.Key, this._config.HideReportInTrace ? LogLevel.Trace : log.Value);
                }

                this._reports.Add($"{preface}\n--------------------------------\n{string.Join("\n", report.Keys)}");
                File.WriteAllText(Path.Combine(this.Helper.DirectoryPath, reportData == null ? "LatestMultiplayerModReports-Farmhand.txt" : "LatestMultiplayerModReports-Host.txt"), string.Join("\n\n", this._reports));
            }
            else
            {
                if (reportData == null)
                {
                    this.Monitor.Log(this.Helper.Translation.Get("SuccessfulConnectionFarmhandside"),
                        LogLevel.Info);
                }
                else
                {
                    this.Monitor.Log(this.Helper.Translation.Get("SuccessfulConnectionHostSide", new { FarmhandID = reportData.FarmhandId, FarmhandName = reportData.FarmhandName }),
                        LogLevel.Info);
                }
            }
        }
    }
}
