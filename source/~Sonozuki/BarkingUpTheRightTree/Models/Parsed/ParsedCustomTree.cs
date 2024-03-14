/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using BarkingUpTheRightTree.Models.Converted;
using BarkingUpTheRightTree.Tools;
using System;
using System.Collections.Generic;

namespace BarkingUpTheRightTree.Models.Parsed
{
    /// <summary>Represents a custom tree.</summary>
    /// <remarks>This is a version of <see cref="CustomTree"/> that uses <see cref="ParsedTapperTimedProduct"/> and <see cref="ParsedTimedProduct"/>.<br/>The reason this is done is so content packs can have tokens in place of the ids to call mod APIs to get the id (so JsonAsset items can be used for example).</remarks>
    public class ParsedCustomTree
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the tree.</summary>
        public string Name { get; set; }

        /// <summary>The item the tree drops when using a tapper on it.</summary>
        public ParsedTapperTimedProduct TappedProduct { get; set; }

        /// <summary>The id of the item the tree drops when it gets cut down.</summary>
        public string WoodId { get; set; }

        /// <summary>Whether the tree drops sap when it gets cut down.</summary>
        public bool DropsSap { get; set; }

        /// <summary>The id of the item to plant to grow the tree.</summary>
        public string SeedId { get; set; }

        /// <summary>The required tool level to cut down the tree.</summary>
        public int RequiredToolLevel { get; set; }

        /// <summary>Whether the tree turns into a stump in winter, like the mushroom tree.</summary>
        public bool IsStumpInWinter { get; set; }

        /// <summary>The items the tree can drop whenever it's shaken.</summary>
        public List<ParsedSeasonalTimedProduct> ShakingProducts { get; set; }

        /// <summary>The tree will only get loaded if atleast one of the listed mods are present.</summary>
        public List<string> IncludeIfModIsPresent { get; set; }

        /// <summary>The tree will only get loaded if none of the listed mods are present.</summary>
        public List<string> ExcludeIfModIsPresent { get; set; }

        /// <summary>The item the tree drops when using the <see cref="BarkRemover"/> tool on it.</summary>
        public ParsedTimedProduct BarkProduct { get; set; }

        /// <summary>The chance the tree has to grow a stage (at the start of each day) when it's unfertilised.</summary>
        public float UnfertilisedGrowthChance { get; set; } = .2f;

        /// <summary>The chance the tree has to grow a stage (at the start of each day) when it's fertilised.</summary>
        public float FertilisedGrowthChance { get; set; } = 1;


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="name">The name of the tree.</param>
        /// <param name="tappedProduct">The item the tree drops when using a tapper on it.</param>
        /// <param name="woodId">The id of the item the tree drops when it gets cut down.</param>
        /// <param name="dropsSap">Whether the tree drops sap when it gets cut down.</param>
        /// <param name="seedId">The id of the item to plant to grow the tree.</param>
        /// <param name="requiredToolLevel">The required tool level to cut down the tree.</param>
        /// <param name="isStumpInWinter">Whether the tree turns into a stump in winter, like the mushroom tree.</param>
        /// <param name="shakingProducts">The items the tree can drop whenever it's shaken.</param>
        /// <param name="includeIfModIsPresent">The tree will only get loaded if atleast one of the listed mods are present.</param>
        /// <param name="excludeIfModIsPresent">The tree will only get loaded if none of the listed mods are present.</param>
        /// <param name="barkProduct">The item the tree drops when using the <see cref="BarkRemover"/> tool on it.</param>
        /// <param name="unfertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's unfertilised.</param>
        /// <param name="fertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's fertilised.</param>
        public ParsedCustomTree(string name, ParsedTapperTimedProduct tappedProduct, string woodId, bool dropsSap, string seedId, int requiredToolLevel, bool isStumpInWinter, List<ParsedSeasonalTimedProduct> shakingProducts, List<string> includeIfModIsPresent, List<string> excludeIfModIsPresent, ParsedTimedProduct barkProduct, float unfertilisedGrowthChance, float fertilisedGrowthChance)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TappedProduct = tappedProduct;
            WoodId = woodId ?? "-1";
            DropsSap = dropsSap;
            SeedId = seedId ?? "-1";
            RequiredToolLevel = requiredToolLevel;
            IsStumpInWinter = isStumpInWinter;
            ShakingProducts = shakingProducts ?? new List<ParsedSeasonalTimedProduct>();
            IncludeIfModIsPresent = includeIfModIsPresent ?? new List<string>();
            ExcludeIfModIsPresent = excludeIfModIsPresent ?? new List<string>();
            BarkProduct = barkProduct;
            UnfertilisedGrowthChance = unfertilisedGrowthChance;
            FertilisedGrowthChance = fertilisedGrowthChance;
        }
    }
}
