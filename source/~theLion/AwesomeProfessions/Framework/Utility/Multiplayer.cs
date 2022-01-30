/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Utility;

#region using directives

using System.Threading.Tasks;

#endregion using directives

public static class Multiplayer
{
    public static TaskCompletionSource<string> ResponseReceived;

    /// <summary>Send an asynchronous request to a multiplayer peer and await a response.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="messageType">The message type.</param>
    /// <param name="playerId">The unique id of the recipient.</param>
    public static async Task<string> SendRequestAsync(string message, string messageType, long playerId)
    {
        ModEntry.ModHelper.Multiplayer.SendMessage(message, messageType, new[] {ModEntry.Manifest.UniqueID},
            new[] {playerId});

        ResponseReceived = new();
        return await ResponseReceived.Task;
    }
}