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
    internal class ChallengeRequestResponseMessage
    {
        public bool Accepted { get; }
        public string Reason { get; }

        public ChallengeRequestResponseMessage(bool accepted, string reason = null)
        {
            this.Accepted = accepted;
            this.Reason = reason;
        }
    }
}
