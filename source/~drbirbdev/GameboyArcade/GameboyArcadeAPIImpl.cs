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
            Content content = ModEntry.SearchGames(idOrName);
            return content?.UniqueID;
        }

        public string GameLoaded()
        {
            if (Game1.currentMinigame is null || Game1.currentMinigame is not GameboyMinigame minigame)
            {
                return null;
            }
            return minigame.Content.Name;
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
            Content content = ModEntry.SearchGames(idOrName);
            return GameboyMinigame.LoadGame(content);
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
