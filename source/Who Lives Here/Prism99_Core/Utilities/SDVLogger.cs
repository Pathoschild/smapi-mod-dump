/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using StardewModdingAPI.Events;
using System.Collections.Generic;
using StardewModdingAPI;
using System.IO;
using System;
using System.Linq;

namespace Prism99_Core.Utilities
{
    public class SDVLogger
    {
        private IMonitor Monitor;
#if DEBUG
        public bool Debug = true;
        public bool CustomLog = true;
#else
        public bool Debug = false;
        public bool CustomLog = false;
#endif
        public List<string> LogLines = new List<string> { };
        private readonly List<string> OnceLogs = new List<string> { };
        private string logDir = "";
        private string logName = "customlog.log";
        public Dictionary<Type, Action<string, object>> CustomDumps = new Dictionary<Type, Action<string, object>> { };

        public SDVLogger(IMonitor mon, string loggingDir, IModHelper helper, bool deepLogging = false, string slogName = null)
        {
            Monitor = mon;
            if (!string.IsNullOrEmpty(slogName))
            {
                logName = slogName;
            }

            logDir = loggingDir;
            string savePath = Path.Combine(logDir, logName);
            if (CustomLog)
            {
                if (File.Exists(savePath))
                {
                    try
                    {
                        File.Delete(savePath);
                    }
                    catch { }
                    AddToCustomLog($"{GetTimeStamp()} Logging started.");
                }
            }

            if (Monitor != null)
            {
                Monitor.Log($"Logger debug mode: {Debug}", LogLevel.Info);
            }
            if (deepLogging)
            {
                helper.Events.World.NpcListChanged += World_NpcListChanged;
                helper.Events.World.ChestInventoryChanged += World_ChestInventoryChanged;
                helper.Events.World.ObjectListChanged += World_ObjectListChanged;
                helper.Events.World.BuildingListChanged += World_BuildingListChanged;
                helper.Events.World.LocationListChanged += World_LocationListChanged;
                helper.Events.World.DebrisListChanged += World_DebrisListChanged;
                helper.Events.World.FurnitureListChanged += World_FurnitureListChanged;
                helper.Events.World.LargeTerrainFeatureListChanged += World_LargeTerrainFeatureListChanged;
                helper.Events.World.TerrainFeatureListChanged += World_TerrainFeatureListChanged;
            }
        }
        private void AddToCustomLog(string message)
        {
            if (CustomLog)
            {
                string savePath = Path.Combine(logDir, logName);
                if (File.Exists(savePath))
                {
                    FileInfo fileInfo = new FileInfo(savePath);
                    if (fileInfo.Length > 8000000)
                    {
                        try
                        {
                            File.Delete(savePath);
                        }
                        catch { }
                    }
                }
                try
                {
                    File.AppendAllLines(savePath, new[] { message });
                }
                catch { }
            }

        }
        private string GetTimeStamp()
        {
            var dt = DateTime.Now;
            return $"[{dt.ToString("yyyy-MM-dd")} {dt.ToString("HH:mm")}] ";
        }
        private string FormatMessage(string message, LogLevel lLevel, string label, bool detailed)
        {
            string time = GetTimeStamp();
            string messageWithLabel = (label == "" ? ($"   {message}") : (message == "" ? label : $"{label}: {message}"));
            string sid = "";
            if (Context.IsSplitScreen)
            {
                sid = $"[SID:{Context.ScreenId}] ";
            }
            if (detailed)
            {
                return $"{time}{sid}[{lLevel}] {messageWithLabel}";
            }
            else
            {
                return $"{sid}{messageWithLabel}";
            }
        }
        public void LogMessage(string message, LogLevel lLevel, string label, bool skipMonitor = false)
        {
            string detailline = FormatMessage(message, lLevel, label, true);
            string line = FormatMessage(message, lLevel, label, false);

            AddToCustomLog(detailline);
            if (Debug || lLevel == LogLevel.Error) LogLines.Add(detailline);
            if (!skipMonitor && Monitor != null)
            {
                if (Debug)
                {
                    if (lLevel == LogLevel.Trace) lLevel = LogLevel.Info;
                    Monitor.Log(line, lLevel);
                }
                else
                {
                    switch (lLevel)
                    {
                        //case LogLevel.Trace:
                        case LogLevel.Debug:
                            break;
                        default:
                            Monitor.Log(line, lLevel);
                            break;
                    }
                }
            }
        }

        public void LogMessageOnce(string message, LogLevel lLevel, string label)
        {
            if (!OnceLogs.Contains($"{lLevel}.{message}.{label}"))
            {
                OnceLogs.Add($"{lLevel}.{message}.{label}");
                if (Monitor != null)
                {
                    Monitor.LogOnce(message, lLevel);
                }
                LogMessage(message, lLevel, label, true);
            }
        }
        public void LogOnce(string message, LogLevel level)
        {
            LogMessageOnce(message, level, "");
        }
        public void LogMessage(string message, string label = null)
        {
            LogMessage(message, LogLevel.Trace, label);
        }

        public void Log(string message, LogLevel lLevel = LogLevel.Debug)
        {
            LogMessage(message, lLevel, "");
        }
        public void LogError(string sMethodId, Exception err)
        {
            LogError(sMethodId, "Threw an error.");
            //LogError("Details", err.ToString());
            DumpObject("err", err);
        }
        public void LogError(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Error, sMethodId);
        }
        public void LogDebug(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Debug, sMethodId);
        }
        public void LogTrace(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Trace, sMethodId);
        }
        public void LogWarning(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Warn, sMethodId);
        }
        public void LogWarningOnce(string sMethodId, string sErrText)
        {
            LogMessageOnce(sErrText, LogLevel.Warn, sMethodId);
        }
        public void LogInfo(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Info, sMethodId);
        }
        public void LogAlert(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Alert, sMethodId);
        }


        //
        //  object dumps
        //
        internal void DumpObject(string sName, object oObject)
        {
            if (oObject == null)
            {
                LogTrace(sName, " is null");
            }
            else
            {
                if (CustomDumps.ContainsKey(typeof(object)))
                {
                    CustomDumps[typeof(object)].Invoke(sName, oObject);
                }
                else
                {
                    DumpObject(sName, oObject.ToString());
                }
            }
        }
        internal void DumpObject(string sName, Exception ex)
        {
            LogTrace("Exeception " + sName, "");
            LogTrace("{", "");
            DumpObject("   Message", ex.Message);
            DumpObject("   StackTrace", ex.StackTrace);
            DumpObject("   GetType", ex.GetType().ToString());
            DumpObject("   Source", ex.Source);
            LogTrace("}", "");
        }
        internal void DumpObject(string sName, string sValue)
        {
            LogTrace(sName, sValue == null ? "null" : "'" + sValue.Replace("\n", " ^ ") + "'");
        }
        private void World_NpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            //
            //  Passive logging for debugging
            //
#if v16
            if (e.Added.Any())
            {
                Log($"World_NpcListChanged, [{e.Location.Name}] Added: {string.Join(',', e.Added.Select(p => p.Name + " (" + p.Tile.X.ToString() + ", " + p.Tile.Y.ToString() + ")").ToArray())}", LogLevel.Debug);
            }
            if (e.Removed.Any())
            {
                Log($"World_NpcListChanged, [{e.Location.Name}] Removed: {string.Join(',', e.Removed.Select(p => p.Name + " (" + p.Tile.X.ToString() + ", " + p.Tile.Y.ToString() + ")").ToArray())}", LogLevel.Debug);
            }
#else
            if (e.Added.Any())
            {
                Log($"World_NpcListChanged, [{e.Location.Name}] Added: {string.Join(',', e.Added.Select(p => p.Name + " (" + p.getTileX().ToString() + ", " + p.getTileY().ToString() + ")").ToArray())}", LogLevel.Debug);
            }
            if (e.Removed.Any())
            {
                Log($"World_NpcListChanged, [{e.Location.Name}] Removed: {string.Join(',', e.Removed.Select(p => p.Name + " (" + p.getTileX().ToString() + ", " + p.getTileY().ToString() + ")").ToArray())}", LogLevel.Debug);
            }
#endif
        }
        private void World_TerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs e)
        {
            //
            //  Passive logging for debugging
            //
            Log($"World_TerrainFeatureListChanged", LogLevel.Debug);
        }

        private void World_LargeTerrainFeatureListChanged(object? sender, LargeTerrainFeatureListChangedEventArgs e)
        {
            //
            //  Passive logging for debugging
            //
            Log($"World_LargeTerrainFeatureListChanged", LogLevel.Debug);
        }

        private void World_FurnitureListChanged(object? sender, FurnitureListChangedEventArgs e)
        {
            //
            //  Passive logging for debugging
            //
            Log($"World_FurnitureListChanged", LogLevel.Debug);
        }

        private void World_DebrisListChanged(object? sender, DebrisListChangedEventArgs e)
        {
            //
            //  Passive logging for debugging
            //
            //
            //  triggered for every tree chop or pickaxe strikes
            //
            Log($"World_DebrisListChanged", LogLevel.Debug);
        }

        private void World_LocationListChanged(object? sender, LocationListChangedEventArgs e)
        {
            //
            //  Passive logging for debugging
            //
            if (e.Added.Any())
            {
                Log($"World_LocationListChanged, Added: {string.Join(',', e.Added.Select(p => p.Name).ToArray())}", LogLevel.Debug);
            }
            if (e.Removed.Any())
            {
                Log($"World_LocationListChanged, Removed: {string.Join(',', e.Removed.Select(p => p.Name).ToArray())}", LogLevel.Debug);
            }
        }

        private void World_BuildingListChanged(object? sender, BuildingListChangedEventArgs e)
        {
            //
            //  Passive logging for debugging
            //
            if (e.Added.Any())
            {
                Log($"World_BuildingListChanged, [{e.Location.Name}] Added: {string.Join(',', e.Added.Select(p => p.buildingType.Value + " (" + p.tileY.Value.ToString() + ", " + p.tileX.Value.ToString() + ")").ToArray())},", LogLevel.Debug);
            }
            if (e.Removed.Any())
            {
                Log($"World_BuildingListChanged, [{e.Location.Name}] Removed: {string.Join(',', e.Removed.Select(p => p.buildingType.Value + " (" + p.tileY.Value.ToString() + ", " + p.tileX.Value.ToString() + ")").ToArray())}", LogLevel.Debug);
            }
        }

        private void World_ObjectListChanged(object? sender, ObjectListChangedEventArgs e)
        {
            //
            //  Passive logging for debugging
            //
            if (e.Added.Any())
            {
                Log($"World_ObjectListChanged, [{e.Location.Name}] Added: {string.Join(',', e.Added.Select(p => p.Value.Name + "x" + p.Value.Stack).ToArray())}", LogLevel.Debug);
            }
            if (e.Removed.Any())
            {
                Log($"World_ObjectListChanged, [{e.Location.Name}] Removed: {string.Join(',', e.Removed.Select(p => p.Value.Name + "x" + p.Value.Stack).ToArray())}", LogLevel.Debug);
            }
        }




        private void World_ChestInventoryChanged(object? sender, ChestInventoryChangedEventArgs e)
        {
            //
            //  Passive logging for debugging
            //
            if (e.Added.Any())
            {
                Log($"World_ChestInventoryChanged, [{e.Location.Name}] Added: {string.Join(',', e.Added.Select(p => p.Name + "x" + p.Stack).ToArray())}", LogLevel.Debug);
            }
            if (e.Removed.Any())
            {
                Log($"World_ChestInventoryChanged, [{e.Location.Name}] Removed: {string.Join(',', e.Removed.Select(p => p.Name + "x" + p.Stack).ToArray())}", LogLevel.Debug);
            }
        }
    }
}
