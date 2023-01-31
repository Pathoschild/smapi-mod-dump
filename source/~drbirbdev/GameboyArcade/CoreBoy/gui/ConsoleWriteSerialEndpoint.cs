/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using CoreBoy.serial;

namespace CoreBoy.gui
{
    public class ConsoleWriteSerialEndpoint : SerialEndpoint
    {
        public int transfer(int b)
        {
            Console.Write((char) b);
            return 0;
        }
    }
}