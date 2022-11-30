/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.IO;
using NUnit.Framework;

namespace InformantTest.Implementation; 

public static class TestUtils {
    public static readonly string ModFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(
        TestContext.CurrentContext.TestDirectory)))! // Informant.Test/bin/Debug/net5.0
        .Replace("Informant.Test", "Informant");
}