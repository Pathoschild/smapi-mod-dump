/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using NUnit.Framework;
using SilentOak.Patching.Extensions;
using System;

namespace Patching.Tests
{
    [TestFixture]
    public class VersionMatchingTests
    {
        /// <summary>
        /// Ensures that <see cref="VersionExtensions.Match(Version, string)"/> succeeds when match expression is *
        /// </summary>
        [TestCase]
        public void Test_Match_Any()
        {
            Version version = new Version("1.2.3.4");
            Assert.True(version.Match("*"));
        }

        /// <summary>
        /// Ensures that <see cref="VersionExtensions.Match(Version, string)"/> succeeds on same major when minor in matching expression is *
        /// </summary>
        [TestCase]
        public void Test_Match_Major()
        {
            Version version = new Version("1.2.3.4");
            Assert.True(version.Match("1.*"));
            Assert.True(version.Match("1"));
        }

        /// <summary>
        /// Ensures that <see cref="VersionExtensions.Match(Version, string)"/> succeeds on same major and same minor when build in matching expression is *.
        /// </summary>
        [TestCase]
        public void Test_Match_Minor()
        {
            Version version = new Version("1.2.3.4");
            Assert.True(version.Match("1.2.*"));
            Assert.True(version.Match("1.2"));
        }

        /// <summary>
        /// Ensures that <see cref="VersionExtensions.Match(Version, string)"/> succeeds on same major, same minor and same build when revision in matching expression is *
        /// </summary>
        [TestCase]
        public void Test_Match_Build()
        {
            Version version = new Version("1.2.3.4");
            Assert.True(version.Match("1.2.3.*"));
            Assert.True(version.Match("1.2.3"));
        }

        /// <summary>
        /// Ensures that <see cref="VersionExtensions.Match(Version, string)"/> succeeds when matching expression is identical to version.
        /// </summary>
        [TestCase]
        public void Test_Match_SameVersion()
        {
            Version version = new Version("1.2.3.4");
            Assert.True(version.Match("1.2.3.4"));
        }

        /// <summary>
        /// Ensures that <see cref="VersionExtensions.Match(Version, string)"/> fails when major doesn't match.
        /// </summary>
        [TestCase]
        public void Test_Match_FailMajor()
        {
            Version version = new Version("1.2.3.4");
            Assert.False(version.Match("2.*"));
            Assert.False(version.Match("2"));
        }

        /// <summary>
        /// Ensures that <see cref="VersionExtensions.Match(Version, string)"/> fails when major matches but minor doesn't.
        /// </summary>
        [TestCase]
        public void Test_Match_FailMinor()
        {
            Version version = new Version("1.2.3.4");
            Assert.False(version.Match("1.3.*"));
            Assert.False(version.Match("1.3"));
        }

        /// <summary>
        /// Ensures that <see cref="VersionExtensions.Match(Version, string)"/> fails when major and minor match but build doesn't.
        /// </summary>
        [TestCase]
        public void Test_Match_FailBuild()
        {
            Version version = new Version("1.2.3.4");
            Assert.False(version.Match("1.2.4.*"));
            Assert.False(version.Match("1.2.4"));
        }

        /// <summary>
        /// Ensures that <see cref="VersionExtensions.Match(Version, string)"/> fails when no components match.
        /// </summary>
        [TestCase]
        public void Test_Match_FailRevision()
        {
            Version version = new Version("1.2.3.4");
            Assert.False(version.Match("1.2.3.5"));
        }

        /// <summary>
        /// Ensures that <see cref="VersionExtensions.Match(Version, string)"/> throws when matching expression is in invalid format.
        /// </summary>
        [TestCase]
        public void Test_Match_FailFormat()
        {
            Version version = new Version("1.2.3.4");
            Assert.Throws<FormatException>(() => version.Match("1.2."));
            Assert.Throws<FormatException>(() => version.Match("1a.2b"));
            Assert.Throws<FormatException>(() => version.Match("00-"));
            Assert.Throws<FormatException>(() => version.Match("anything"));
        }
    }
}
