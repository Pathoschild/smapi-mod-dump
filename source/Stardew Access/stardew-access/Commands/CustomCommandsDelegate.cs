/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

namespace stardew_access.Commands;

/// <summary>
/// Handles a custom mod command.
/// <br/> The name of the method (case insensitive) is used as the command's name
/// i.e., a method named "TestCommand" will have a corresponding command name of "testcommand".
/// <br/> But, if you want to have a different name, then you can append it with an underscore.
/// For example, "TestCommand_tc" will have the command name "tc".
/// </summary>
/// <param name="args">The arguments passed with the command.</param>
/// <param name="fromChatBox">Whether the delegate was invoked by the chat box or not.
/// If it was then the info and error messages are sent through the chat box instead of the terminal.</param>
public delegate void CustomCommandsDelegate(string[] args, bool fromChatBox = false);
