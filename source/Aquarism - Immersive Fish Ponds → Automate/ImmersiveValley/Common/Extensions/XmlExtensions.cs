/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Extensions;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Xml;

#endregion using directives

/// <summary>Extensions for reading data from <see cref="XmlElement"/> instance.</summary>
public static class XmlElementExtensions
{
    /// <summary>Read the specified node in the <see cref="XmlElement"/> instance and parse to a generic type.</summary>
    /// <param name="name">The name of the node.</param>
    public static T ReadAs<T>(this XmlElement xml, string name)
    {
        if (!xml.TryReadAs<T>(name, out var result))
            ThrowHelper.ThrowArgumentException($"Element [{name}] was not found in {xml.Name}.");

        return result;
    }

    /// <summary>Try to read the specified node in the <see cref="XmlElement"/> instance.</summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="result">The parsed value, if successful. Otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the node exists and can be parsed, otherwise <see langword="false"/>.</returns>
    public static bool TryReadAs<T>(this XmlElement xml, string name, [NotNullWhen(true)] out T? result)
    {
        result = default;
        if (xml.SelectSingleNode(name) is not XmlElement node) return false;

        var innerText = node.InnerText;
        return !string.IsNullOrEmpty(innerText) && innerText.TryParse(out result);
    }

    /// <summary>Read the specified node in the <see cref="XmlElement"/> instance as <see cref="string"/>.</summary>
    /// <param name="name">The name of the node.</param>
    public static string Read(this XmlElement xml, string name)
    {
        if (!xml.TryRead(name, out var result))
            ThrowHelper.ThrowArgumentException($"Element [{name}] was not found in {xml.Name}.");

        return result;
    }

    /// <summary>Try to read the specified node in the <see cref="XmlElement"/> instance as <see cref="string"/>.</summary>
    /// <param name="name">The name of the node.</param>
    /// <param name="innerText">The inner text of the node, if it exists. Otherwise <see cref="string.Empty"/>.</param>
    /// <returns><see langword="true"/> if the node exists and is non-empty, otherwise <see langword="false"/>.</returns>
    public static bool TryRead(this XmlElement xml, string name, out string innerText)
    {
        innerText = string.Empty;
        if (xml.SelectSingleNode(name) is not XmlElement node) return false;

        innerText = node.InnerText;
        return string.IsNullOrEmpty(innerText);
    }
}
