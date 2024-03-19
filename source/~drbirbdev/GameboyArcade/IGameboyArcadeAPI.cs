/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace GameboyArcade
{
    public interface IGameboyArcadeApi
    {
        /// <summary>
        /// Look for the given game by id, else look for a game of the same name.
        /// </summary>
        /// <param name="idOrName">ID or name of a game</param>
        /// <returns>ID of a matching game, or null</returns>
        string FindGame(string idOrName);

        /// <summary>
        /// Load the given game by id, else look for a game of the same name to load.
        /// </summary>
        /// <param name="idOrName">ID or name of a game</param>
        /// <returns></returns>
        bool LoadGame(string idOrName);

        /// <summary>
        /// Get the currently loaded game.
        /// </summary>
        /// <returns>ID of the currently loaded game, or null</returns>
        string GameLoaded();

        /// <summary>
        /// Get a byte from the game emulation.
        /// You should be able to extract any data from the currently running game, but you'll have to understand
        /// the Gameboy memory model, as well as how memory is used for a specific game.
        /// Keep in mind that the emulation runs in a different thread, so you won't be able to precisely time this.
        /// </summary>
        /// <param name="address">Memory address of the byte</param>
        /// <returns>A byte of data from the emulator, or null if no emulator is running</returns>
        byte? GetMemoryByte(ushort address);

        /// <summary>
        /// Set a byte in the game emulation.
        /// You should be able to set any data for the currently running game, but you'll have to understand
        /// the Gameboy memory model, as well as how memory is used for a specific game.
        /// Keep in mind that the emulation runs in a different thread, so you won't be able to precisely time this.
        /// </summary>
        /// <param name="address">Memory address of the byte</param>
        /// <param name="value">A byte of data</param>
        void SetMemoryByte(ushort address, byte value);
    }
}
