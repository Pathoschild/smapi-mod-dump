/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/inkle/ink
**
*************************************************/

using System.Collections.Generic;

namespace Ink.Parsed
{
    public interface IWeavePoint
    {
        int indentationDepth { get; }
        Runtime.Container runtimeContainer { get; }
        List<Parsed.Object> content { get; }
        string name { get; }
        Identifier identifier { get; }

    }
}

