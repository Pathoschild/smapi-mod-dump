using System;
using System.Runtime.Serialization;

namespace NpcAdventure.Loader
{
    [Serializable]
    internal class ContentPackException : Exception
    {
        public ContentPackException()
        {
        }

        public ContentPackException(string message) : base(message)
        {
        }

        public ContentPackException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ContentPackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}