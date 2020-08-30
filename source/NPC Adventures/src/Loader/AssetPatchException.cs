using System;
using System.Runtime.Serialization;

namespace NpcAdventure.Loader
{
    [Serializable]
    internal class AssetPatchException : Exception
    {
        public AssetPatchException()
        {
        }

        public AssetPatchException(Type type) : base($"Unable to apply patch for type `{type}`")
        {
        }

        public AssetPatchException(string message) : base(message)
        {
        }

        public AssetPatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AssetPatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
