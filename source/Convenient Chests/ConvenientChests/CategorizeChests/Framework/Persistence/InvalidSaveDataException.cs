/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System;

namespace ConvenientChests.CategorizeChests.Framework.Persistence
{
    /// <summary>
    /// An exception to be raised when save data is malformed or fails to
    /// correspond to the state of the game world.
    /// </summary>
    class InvalidSaveDataException : Exception
    {
        public InvalidSaveDataException(string message) : base(message) {}
        public InvalidSaveDataException(string message, Exception inner) : base(message, inner) {}
    }
}