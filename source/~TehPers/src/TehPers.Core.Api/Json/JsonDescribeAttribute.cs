/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.ComponentModel;

namespace TehPers.Core.Api.Json
{
    /// <summary>Indicates that the properties of this type that are annotated with <see cref="DescriptionAttribute"/> should be commented when serialized by an <see cref="IJsonProvider"/>.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class JsonDescribeAttribute : Attribute
    {
    }
}
