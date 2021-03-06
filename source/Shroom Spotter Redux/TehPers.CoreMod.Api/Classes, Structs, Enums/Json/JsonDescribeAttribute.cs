/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;

namespace TehPers.CoreMod.Api.Json {
    /// <summary>Indicates that the property should be commented based on its <see cref="System.ComponentModel.DescriptionAttribute"/>.</summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class JsonDescribeAttribute : Attribute { }
}
