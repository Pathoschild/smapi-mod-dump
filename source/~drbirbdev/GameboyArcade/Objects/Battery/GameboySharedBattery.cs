/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Threading;
using CoreBoy.memory.cart.battery;
using StardewModdingAPI;
using StardewValley;

namespace GameboyArcade
{
    class GameboySharedBattery : IBattery
    {
        private string MinigameId;
        private bool RemotePlayer = false;

        private bool AwaitingMessage = false;
        private SaveState Save;

        public GameboySharedBattery(string minigameId)
        {
            this.MinigameId = minigameId;

            if (Context.IsMainPlayer)
            {

            }
            else if (!Context.IsOnHostComputer)
            {
                this.RemotePlayer = true;
            }
        }

        private void Multiplayer_ModMessageReceived_LoadReceive(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModEntry.Instance.ModManifest.UniqueID && e.Type == "LoadReceive")
            {
                this.Save = e.ReadAs<SaveState>();
                this.AwaitingMessage = false;
                ModEntry.Instance.Helper.Events.Multiplayer.ModMessageReceived -= this.Multiplayer_ModMessageReceived_LoadReceive;
            }
        }

        public void LoadRam(int[] ram)
        {
            LoadRamWithClock(ram, null);
        }

        public void LoadRamWithClock(int[] ram, long[] clockData)
        {
            if (this.RemotePlayer)
            {
                this.AwaitingMessage = true;
                ModEntry.Instance.Helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived_LoadReceive;
                ModEntry.Instance.Helper.Multiplayer.SendMessage<string>(this.MinigameId, "LoadRequest", new string[] { ModEntry.Instance.ModManifest.UniqueID }, new long[] { Game1.MasterPlayer.UniqueMultiplayerID });
                while(this.AwaitingMessage)
                {
                    Thread.Sleep(1);
                }
                Save.RAM.CopyTo(ram, 0);
                if (clockData is not null)
                {
                    Save.ClockData.CopyTo(clockData, 0);
                }
            }
            else
            {
                SaveState loaded = ModEntry.Instance.Helper.Data.ReadJsonFile<SaveState>($"data/{MinigameId}/{Constants.SaveFolderName}/file.json");
                if (loaded is null)
                {
                    return;
                }
                loaded.RAM.CopyTo(ram, 0);
                if (clockData is not null)
                {
                    loaded.ClockData.CopyTo(clockData, 0);
                }
            }
        }

        public void SaveRam(int[] ram)
        {
            SaveRamWithClock(ram, null);
        }

        public void SaveRamWithClock(int[] ram, long[] clockData)
        {
            SaveState save = new SaveState { RAM = ram, ClockData = clockData };
            if (this.RemotePlayer)
            {
                ModEntry.Instance.Helper.Multiplayer.SendMessage<SaveState>(save, $"SaveRequest {MinigameId}", new string[] { ModEntry.Instance.ModManifest.UniqueID }, new long[] { Game1.MasterPlayer.UniqueMultiplayerID });
            }
            else
            {
                ModEntry.Instance.Helper.Data.WriteJsonFile<SaveState>($"data/{MinigameId}/{Constants.SaveFolderName}/file.json", save);
            }
        }
    }
}
