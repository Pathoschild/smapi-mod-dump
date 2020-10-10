/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hcoona/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using SaveAnywhereV3.DataContract;
using SaveAnywhereV3.Service;
using StardewValley;

namespace SaveAnywhereV3
{
    public class SaveLoadManager
    {
        private readonly IList<ISaveLoadService> m_services;
        private readonly Lazy<string> m_saveFilePath;

        internal string SaveFilePath { get { return m_saveFilePath.Value; } }
        internal string BackupSaveFilePath { get { return SaveFilePath + ".bak"; } }

        public SaveLoadManager(IList<ISaveLoadService> services)
        {
            this.m_services = services;
            this.m_saveFilePath = new Lazy<string>(() => Path.Combine(Global.Helper.DirectoryPath, "Data", $"{Game1.player.name}_{Game1.uniqueIDForThisGame}.json"));
        }

        public void Save()
        {
            var aggregatedModel = new AggregatedModel();
            foreach (var s in m_services)
            {
                s.SaveTo(aggregatedModel);
            }

            if (File.Exists(SaveFilePath))
            {
                File.Delete(BackupSaveFilePath);
                File.Move(SaveFilePath, BackupSaveFilePath);
            }
            Global.Helper.WriteJsonFile(SaveFilePath, aggregatedModel);
        }

        public void Load()
        {
            var aggregatedModel = Global.Helper.ReadJsonFile<AggregatedModel>(SaveFilePath);
            foreach (var s in m_services)
            {
                s.LoadFrom(aggregatedModel);
            }
        }
    }
}
