/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using FluentAssertions;
using NUnit.Framework;
using Pathoschild.Stardew.Common.Utilities;

namespace Pathoschild.Stardew.Tests.Common.CommonTests
{
    /// <summary>Unit tests for <see cref="InvariantDictionary{TValue}"/>.</summary>
    [TestFixture]
    class InvariantDictionaryTests
    {
        /*********
        ** Unit tests
        *********/
        /// <summary>Ensure that Add throws an exception when adding a case-insensitively duplicate key.</summary>
        [TestCase]
        public void Add_FailsIfDuplicate()
        {
            // ReSharper disable once CollectionNeverQueried.Local
            InvariantDictionary<bool> dict = new() { ["a"] = true };

            FluentActions
                .Invoking(() => dict.Add("A", false))
                .Should().Throw<ArgumentException>();
        }

        /// <summary>Ensure that setting values through the indexer yields the expected case-insensitive count.</summary>
        [TestCase("a", "b", "c", ExpectedResult = 3)]
        [TestCase("a", "A", "á", ExpectedResult = 2)]
        [TestCase("a", "A", "a", ExpectedResult = 1)]
        public int Indexer_Count(params string[] keys)
        {
            InvariantDictionary<bool> dict = new();
            foreach (string key in keys)
                dict[key] = true;
            return dict.Count;
        }

        /// <summary>Ensure that Contains yields the expected case-insensitive result.</summary>
        [TestCase("d", "a", "b", "c", ExpectedResult = false)]
        [TestCase("a", "a", "b", "c", ExpectedResult = true)]
        [TestCase("A", "a", "b", "c", ExpectedResult = true)]
        [TestCase("a", "A", "b", "c", ExpectedResult = true)]
        [TestCase("á", "a", "b", "c", ExpectedResult = false)]
        public bool ContainsKey(string search, params string[] keys)
        {
            InvariantDictionary<bool> dict = new();
            foreach (string key in keys)
                dict.Add(key, true);

            return dict.ContainsKey(search);
        }
    }
}
