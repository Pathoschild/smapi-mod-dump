using System;
using System.Runtime.Serialization;

namespace SilentOak.Patching.Exceptions
{
    [Serializable]
    internal class MissingAttributeException : Exception
    {
        public MissingAttributeException()
        {
        }

        public MissingAttributeException(string message) : base(message)
        {
        }

        public MissingAttributeException(string className, string attribute) : base($"Missing {attribute} in {className}.")
        {
        }

        public MissingAttributeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingAttributeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}