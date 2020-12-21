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

namespace QuestFramework.Framework.Helpers
{
    [Serializable]
    internal class ActiveStateFieldException : Exception
    {
        public ActiveStateFieldException()
        {
        }

        public ActiveStateFieldException(string message) : base(message)
        {
        }

        public ActiveStateFieldException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ActiveStateFieldException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}