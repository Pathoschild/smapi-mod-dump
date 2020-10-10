/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

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