/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using System.Reflection;
using StardewModdingAPI;

namespace stardew_access.Commands;

public class CommandManager
{
    private static Dictionary<string, CustomCommandsDelegate> _commands = new();
    internal static Dictionary<string, CustomCommandsDelegate> Commands { get => _commands; }

    private static readonly Dictionary<string, string> Descriptions = new()
    {
        // Read Tile
        {"readtile", "Toggle read tile feature."},
        {"flooring", "Toggle flooring in read tile."},
        {"watered", "Toggle speaking watered or unwatered for crops."},
        // Tile Marking
        {"mark", "Marks the player's position for use in building construction in Carpenter Menu."},
        {"marklist", "List all marked positions."},
        {"buildlist", "List all buildings for selection for upgrading/demolishing/painting"},
        {"buildsel", "Select the building index which you want to upgrade/demolish/paint"},
        // Other Commands
        {"refsr", "Refresh screen reader"},
        {"refmc", "Refresh mod config"},
        {"hnspercent", "Toggle between speaking in percentage or full health and stamina."},
        {"snapmouse", "Toggle snap mouse feature."},
        {"warning", "Toggle warnings feature."},
        {"tts", "Toggles the screen reader/tts"},
        {"rlt", "Repeat the last spoken texts."},
        // Radar
        {"radar", "Toggle radar feature."},
        {"rdebug", "Toggle debugging in radar feature."},
        {"rstereo", "Toggle stereo sound in radar feature."},
        {"rfocus", "Toggle focus mode in radar feature."},
        {"rdelay", "Set the delay of radar feature in milliseconds."},
        {"rrange", "Set the range of radar feature."},
        {"readd", "Add an object key to the exclusions list of radar feature."},
        {"reremove", "Remove an object key from the exclusions list of radar feature."},
        {"relist", "List all the exclusions in the radar feature."},
        {"reclear", "Clear the focus exclusions in the radar featrure."},
        {"recount", "Number of exclusions in the radar feature."},
        {"rfadd", "Add an object key to the focus list of radar feature."},
        {"rfremove", "Remove an object key from the focus list of radar feature."},
        {"rflist", "List all the focused objects in the radar feature."},
        {"rfclear", "Clear the focus list in the radar featrure."},
        {"rfcount", "Number of list in the radar feature."},
    };

    /// <summary>
    /// Registers the custom commands to the ConsoleCommands to be used in the terminal.
    /// Also adds them to the Commands property which is used in the ChatBox patch to allow commands to be executed via chat messages.
    /// </summary>
    public static void RegisterAll(IModHelper modHelper)
    {
        Type[] CommandsGroups =
        {
            typeof(ReadTileCommands),
            typeof(TileMarkingCommands),
            typeof(OtherCommands),
            typeof(RadarCommands),
        };

        foreach (var commandsGroup in CommandsGroups)
        {
            MethodInfo[] methods = commandsGroup.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var methodInfo in methods)
            {
                string commandName = methodInfo.Name.ToLower();
                try
                {
                    // Use the alternate command name if provided.
                    if (commandName.Contains('_')) commandName = commandName.Split('_')[1];

                    // Store the delegate into the dictionary to be used in the chat box.
                    _commands.Add(commandName, (CustomCommandsDelegate)methodInfo.CreateDelegate(typeof(CustomCommandsDelegate)));

                    // Add the command to ConsoleCommands to be able to use it in terminals as usual.
                    modHelper.ConsoleCommands.Add(commandName, Descriptions[commandName], (_, args) => methodInfo.Invoke(null, [args, false]));

                    Log.Verbose($"CommandManager: Registered command with name: {commandName}");
                }
                catch (System.Exception)
                {
                    Log.Error($"CommandManager: Error occurred while registering command: {commandName}");
                }
            }
        }
    }
}
