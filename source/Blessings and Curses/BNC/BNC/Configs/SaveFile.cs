/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using StardewModdingAPI;

namespace BNC
{
    public class SaveFile
    {

        public int nextBuffDate { get; set; } = -1;
        public int lastMineBuffLevel { get; set; } = 0;
        public int currentTwitchBits { get; set; } = 0;

        public void clearData()
        {
            nextBuffDate = -1;
            lastMineBuffLevel  = 0;
            currentTwitchBits = 0;
        }

        public void SaveModData(IModHelper helper)
        {
            BNC_Core.Logger.Log("Save File");
            helper.Data.WriteSaveData<SaveFile>(BNC_Core.saveFileName, BNC_Core.BNCSave);  
        }

        public void LoadModData(IModHelper helper)
        { 
            BNC_Core.Logger.Log("Load File");
            if (helper.Data.ReadSaveData<SaveFile>(BNC_Core.saveFileName) == null)
                SaveModData(helper);
            BNC_Core.BNCSave = helper.Data.ReadSaveData<SaveFile>(BNC_Core.saveFileName);
        }
    }
}
