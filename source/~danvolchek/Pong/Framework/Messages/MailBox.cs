/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace Pong.Framework.Messages
{
    internal static class MailBox
    {
        public static void Send(MessageEnvelope envelope)
        {
            ModEntry.Instance.Helper.Multiplayer.SendMessage(envelope.Data, envelope.MessageType, new[] { ModEntry.Instance.PongId }, new[] { envelope.RecipientId });
        }
    }
}
