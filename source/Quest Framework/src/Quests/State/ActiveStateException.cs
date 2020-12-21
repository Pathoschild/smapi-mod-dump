/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using System;
using System.Runtime.Serialization;

namespace QuestFramework.Quests.State
{
    [Serializable]
    public sealed class ActiveStateException : Exception
    {
        internal ActiveStateException()
        {
        }

        internal ActiveStateException(string message) : base(message)
        {
        }

        internal ActiveStateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}