/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using FluentAssertions;
using NUnit.Framework;
using Pathoschild.Stardew.Common.Utilities;

namespace Pathoschild.Stardew.Tests.Common.CommonTests
{
    /// <summary>Unit tests for <see cref="Cached{TValue}"/>.</summary>
    [TestFixture]
    class CachedTests
    {
        /*********
        ** Unit tests
        *********/
        /// <summary>Ensure that <see cref="Cached{T}.Value"/> doesn't refresh the value if the cache key didn't change.</summary>
        [TestCase]
        public void SameCacheKey_DoesNotRefresh()
        {
            // arrange
            int timesFetched = 0;
            var cached = new Cached<int>(
                getCacheKey: () => "static",
                fetchNew: () => ++timesFetched
            );

            // act
            _ = cached.Value;
            _ = cached.Value;
            int lastValue = cached.Value;

            // assert
            timesFetched.Should().Be(1, "the value should only be fetched once");
            lastValue.Should().Be(timesFetched, "the value should match what we set it to");
        }

        /// <summary>Ensure that <see cref="Cached{T}.Value"/> refreshes the value each time the cache key changes.</summary>
        [TestCase]
        public void DifferentCacheKeys_Refreshes()
        {
            // arrange
            int cacheKey = 0;
            int timesFetched = 0;
            var cached = new Cached<int>(
                getCacheKey: () => $"{++cacheKey}",
                fetchNew: () => ++timesFetched
            );

            // act
            int valueA = cached.Value;
            int valueB = cached.Value;
            int valueC = cached.Value;

            // assert
            timesFetched.Should().Be(3, $"the value should have been fetched three times (once per {cached.Value} call)");
            valueA.Should().Be(1);
            valueB.Should().Be(2);
            valueC.Should().Be(3);
        }
    }
}
