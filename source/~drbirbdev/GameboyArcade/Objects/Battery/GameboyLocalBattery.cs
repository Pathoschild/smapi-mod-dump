/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using CoreBoy.memory.cart.battery;
using StardewModdingAPI;
using StardewValley;

namespace GameboyArcade
{
    class GameboyLocalBattery : IBattery
    {
        private string MinigameId;

        public GameboyLocalBattery(string minigameId)
        {
            this.MinigameId = minigameId;
        }

        public void LoadRam(int[] ram)
        {
            SaveState loaded = ModEntry.Instance.Helper.Data.ReadJsonFile<SaveState>($"data/{MinigameId}/{Constants.SaveFolderName}/{Game1.player.UniqueMultiplayerID}.json");
            if (loaded is null)
            {
                return;
            }
            loaded.RAM.CopyTo(ram, 0);
        }

        public void LoadRamWithClock(int[] ram, long[] clockData)
        {
            SaveState loaded = ModEntry.Instance.Helper.Data.ReadJsonFile<SaveState>($"data/{MinigameId}/{Constants.SaveFolderName}/{Game1.player.UniqueMultiplayerID}.json");
            if (loaded is null)
            {
                return;
            }
            loaded.RAM.CopyTo(ram, 0);
            loaded.ClockData.CopyTo(clockData, 0);
        }

        public void SaveRam(int[] ram)
        {
            SaveRamWithClock(ram, null);
        }

        public void SaveRamWithClock(int[] ram, long[] clockData)
        {
            ModEntry.Instance.Helper.Data.WriteJsonFile<SaveState>($"data/{MinigameId}/{Constants.SaveFolderName}/{Game1.player.UniqueMultiplayerID}.json", new SaveState {
                RAM = ram,
                ClockData = clockData,
            });
        }
    }
}
