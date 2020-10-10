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

namespace QuestFramework.Framework
{
    [Serializable]
    internal class InvalidQuestException : Exception
    {
        public InvalidQuestException()
        {
        }

        public InvalidQuestException(string message) : base(message)
        {
        }

        public InvalidQuestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidQuestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}