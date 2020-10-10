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

namespace SilentOak.QualityProducts.Utils
{
    /***
     * From https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/exceptions/creating-and-throwing-exceptions
     ***/
    [Serializable]
    internal class UnimplementedCaseException : Exception
    {
        public UnimplementedCaseException() { }
        public UnimplementedCaseException(string message) : base(message) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected UnimplementedCaseException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}