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