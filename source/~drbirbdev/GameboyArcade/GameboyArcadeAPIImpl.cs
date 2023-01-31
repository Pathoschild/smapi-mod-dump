/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using StardewValley;

namespace GameboyArcade
{
    public class GameboyArcadeAPIImpl : IGameboyArcadeAPI
    {
        public string FindGame(string idOrName)
        {
            if (ModEntry.LoadedContentPacks.ContainsKey(idOrName))
            {
                return idOrName;
            }
            foreach (string id in ModEntry.LoadedContentPacks.Keys)
            {
                if (idOrName == ModEntry.LoadedContentPacks[id].Name)
                {
                    return id;
                }
            }
            return null;
        }

        public string GameLoaded()
        {
            if (Game1.currentMinigame is null || Game1.currentMinigame is not GameboyMinigame minigame)
            {
                return null;
            }
            return minigame.Content.UniqueID;
        }

        public byte? GetMemoryByte(ushort address)
        {
            if (Game1.currentMinigame is null || Game1.currentMinigame is not GameboyMinigame minigame)
            {
                return null;
            }
            return (byte)minigame.Emulator.Gameboy.Mmu.GetByte(address);
        }

        public bool LoadGame(string idOrName)
        {
            string id = this.FindGame(idOrName);
            if (id is null)
            {
                return false;
            }
            return GameboyMinigame.LoadGame(ModEntry.LoadedContentPacks[id]);
        }

        public void SetMemoryByte(ushort address, byte value)
        {
            if (Game1.currentMinigame is null || Game1.currentMinigame is not GameboyMinigame minigame)
            {
                return;
            }
            minigame.Emulator.Gameboy.Mmu.SetByte(address, value);
        }
    }
}
