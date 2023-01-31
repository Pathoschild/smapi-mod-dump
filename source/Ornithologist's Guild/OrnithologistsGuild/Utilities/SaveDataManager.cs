/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace OrnithologistsGuild
{
    public class SaveDataManager
    {
        private const string KEY = "OrnithologistsGuild";

        public static Models.SaveData SaveData;

        public static void Load()
        {
            SaveData = ModEntry.Instance.Helper.Data.ReadSaveData<Models.SaveData>(KEY) ?? new Models.SaveData();
            ModEntry.Instance.Monitor.Log($"Loaded {SaveData.LifeList.Count} life list entries");
        }

        public static void Save()
        {
            ModEntry.Instance.Helper.Data.WriteSaveData(KEY, SaveData);
            ModEntry.Instance.Monitor.Log($"Saved {SaveData.LifeList.Count} life list entries");
        }
    }
}
