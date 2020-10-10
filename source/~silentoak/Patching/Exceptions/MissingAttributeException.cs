/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

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