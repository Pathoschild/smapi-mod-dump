/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewModdingAPI;

namespace stardew_access.Commands;

public class CommandManager
{
    public static void AddAll(IModHelper modHelper)
    {
        List<ICustomCommand> allCommands = new List<ICustomCommand>()
        {
            new ReadTileCommands(),
            new TileMarkingCommands(),
            new OtherCommands(),
            new RadarCommands(),
        };

        foreach (ICustomCommand command in allCommands)
        {
            command.Add(modHelper);
        }
    }
}