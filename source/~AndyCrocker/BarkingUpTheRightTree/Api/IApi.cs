/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using BarkingUpTheRightTree.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BarkingUpTheRightTree
{
    /// <summary>Provides basic tree apis.</summary>
    public interface IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Adds a custom tree.</summary>
        /// <param name="name">The name of the tree, NOTE: this is an internal unique name, as such it's recomended this be: [modUniqueId].[TreeName]. This is also the same name that will be used in map tile data.</param>
        /// <param name="texture">The texture for the tree.</param>
        /// <param name="tappedProduct">The item the tree drops when using a tapper on it.</param>
        /// <param name="wood">The item the tree drops when it gets cut down.</param>
        /// <param name="dropsSap">Whether the tree drops sap when it gets cut down.</param>
        /// <param name="seed">The item to plant to grow the tree.</param>
        /// <param name="requiredToolLevel">The required tool level to cut down the tree.</param>
        /// <param name="shakingProducts">The items the tree can drop whenever it's shaken.</param>
        /// <param name="includeIfModIsPresent">The tree will only get loaded if atleast one of the listed mods are present.</param>
        /// <param name="excludeIfModIsPresent">The tree will only get loaded if none of the listed mods are present.</param>
        /// <param name="barkProduct">The item the tree drops when using the <see cref="BarkRemover"/> tool on it.</param>
        /// <param name="modName">The name of the mod adding the trees (this is only used for logging the name of tree conflicts).</param>
        /// <param name="unfertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's unfertilised.</param>
        /// <param name="fertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's fertilised.</param>
        /// <returns><see langword="true"/> if the tree was successfully added; otherwise, <see langword="false"/>.<br/>Note: if the tree wasn't added because it didn't pass the check for <paramref name="includeIfModIsPresent"/> or <paramref name="excludeIfModIsPresent"/> then <see langword="false"/> will be returned.</returns>
        public bool AddTree(string name, Texture2D texture, (float DaysBetweenProduce, string Product, int Amount) tappedProduct, string wood, bool dropsSap, string seed, int requiredToolLevel, List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)> shakingProducts, List<string> includeIfModIsPresent, List<string> excludeIfModIsPresent, (int DaysBetweenProduce, string Product, int Amount) barkProduct, string modName, float unfertilisedGrowthChance = .2f, float fertilisedGrowthChance = 1);

        /// <summary>Gets all the raw trees.</summary>
        /// <returns>The raw trees.</returns>
        public IEnumerable<(int Id, Texture2D Texture, (float DaysBetweenProduce, string Product, int Amount) TappedProduct, string Wood, bool DropsSap, string Seed, int RequiredToolLevel, List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)> ShakingProducts, List<string> IncludeIfModIsPresent, List<string> ExcludeIfModIsPresent, (int DaysBetweenProduce, string Product, int Amount) BarkProduct, float UnfertilisedGrowthChance, float FertilisedGrowthChance)> GetAllRawTrees();

        /// <summary>Gets all the trees.</summary>
        /// <returns>All the loaded trees.</returns>
        public IEnumerable<(int Id, Texture2D Texture, (float DaysBetweenProduce, int Product, int Amount) TappedProduct, int Wood, bool DropsSap, int Seed, int RequiredToolLevel, List<(int DaysBetweenProduce, int Product, int Amount, string[] Seasons)> ShakingProducts, List<string> IncludeIfModIsPresent, List<string> ExcludeIfModIsPresent, (int DaysBetweenProduce, int Product, int Amount) BarkProduct, float UnfertilisedGrowthChance, float FertilisedGrowthChance)> GetAllTrees();

        /// <summary>Gets the tree id by tree name.</summary>
        /// <param name="name">The name of the tree.</param>
        /// <returns>The id of the tree, if it exists; otherwise, -1.</returns>
        public int GetIdByName(string name);

        /// <summary>Gets data of a raw tree by its id.</summary>
        /// <param name="id">The id of the tree to get the data of.</param>
        /// <param name="name">The name of the tree.</param>
        /// <param name="texture">The texture for the tree.</param>
        /// <param name="tappedProduct">The item the tree drops when using a tapper on it.</param>
        /// <param name="wood">The item the tree drops when it gets cut down.</param>
        /// <param name="dropsSap">Whether the tree drops sap when it gets cut down.</param>
        /// <param name="seed">The item to plant to grow the tree.</param>
        /// <param name="requiredToolLevel">The required tool level to cut down the tree.</param>
        /// <param name="shakingProducts">The items the tree can drop whenever it's shaken.</param>
        /// <param name="includeIfModIsPresent">The tree will only get loaded if atleast one of the listed mods are present.</param>
        /// <param name="excludeIfModIsPresent">The tree will only get loaded if none of the listed mods are present.</param>
        /// <param name="barkProduct">The item the tree drops when using the <see cref="BarkRemover"/> tool on it.</param>
        /// <param name="unfertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's unfertilised.</param>
        /// <param name="fertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's fertilised.</param>
        /// <returns><see langword="true"/> if data was successfully retrieved; otherwise, <see langword="false"/>.</returns>
        /// <remarks>A 'raw' tree means that any tokens haven't been resolved (so ids are strings and not necessarily integers).<br/>A save doesn't need to be loaded to get a successful result.</remarks>
        public bool GetRawTreeById(int id, out string name, out Texture2D texture, out (float DaysBetweenProduce, string Product, int Amount) tappedProduct, out string wood, out bool dropsSap, out string seed, out int requiredToolLevel, out List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)> shakingProducts, out List<string> includeIfModIsPresent, out List<string> excludeIfModIsPresent, out (int DaysBetweenProduce, string Product, int Amount) barkProduct, out float unfertilisedGrowthChance, out float fertilisedGrowthChance);

        /// <summary>Gets data of a raw tree by its name.</summary>
        /// <param name="name">The name of the tree to get the data of.</param>
        /// <param name="id">The id of the tree.</param>
        /// <param name="texture">The texture for the tree.</param>
        /// <param name="tappedProduct">The item the tree drops when using a tapper on it.</param>
        /// <param name="wood">The item the tree drops when it gets cut down.</param>
        /// <param name="dropsSap">Whether the tree drops sap when it gets cut down.</param>
        /// <param name="seed">The item to plant to grow the tree.</param>
        /// <param name="requiredToolLevel">The required tool level to cut down the tree.</param>
        /// <param name="shakingProducts">The items the tree can drop whenever it's shaken.</param>
        /// <param name="includeIfModIsPresent">The tree will only get loaded if atleast one of the listed mods are present.</param>
        /// <param name="excludeIfModIsPresent">The tree will only get loaded if none of the listed mods are present.</param>
        /// <param name="barkProduct">The item the tree drops when using the <see cref="BarkRemover"/> tool on it.</param>
        /// <param name="unfertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's unfertilised.</param>
        /// <param name="fertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's fertilised.</param>
        /// <returns><see langword="true"/> if data was successfully retrieved; otherwise, <see langword="false"/>.</returns>
        /// <remarks>A 'raw' tree means that any tokens haven't been resolved (so ids are strings and not necessarily integers).<br/>A save doesn't need to be loaded to get a successful result.</remarks>
        public bool GetRawTreeByName(string name, out int id, out Texture2D texture, out (float DaysBetweenProduce, string Product, int Amount) tappedProduct, out string wood, out bool dropsSap, out string seed, out int requiredToolLevel, out List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)> shakingProducts, out List<string> includeIfModIsPresent, out List<string> excludeIfModIsPresent, out (int DaysBetweenProduce, string Product, int Amount) barkProduct, out float unfertilisedGrowthChance, out float fertilisedGrowthChance);

        /// <summary>Gets data of a tree by its id.</summary>
        /// <param name="id">The id of the tree to get the data of.</param>
        /// <param name="name">The name of the tree.</param>
        /// <param name="texture">The texture for the tree.</param>
        /// <param name="tappedProduct">The item the tree drops when using a tapper on it.</param>
        /// <param name="wood">The item the tree drops when it gets cut down.</param>
        /// <param name="dropsSap">Whether the tree drops sap when it gets cut down.</param>
        /// <param name="seed">The item to plant to grow the tree.</param>
        /// <param name="requiredToolLevel">The required tool level to cut down the tree.</param>
        /// <param name="shakingProducts">The items the tree can drop whenever it's shaken.</param>
        /// <param name="includeIfModIsPresent">The tree will only get loaded if atleast one of the listed mods are present.</param>
        /// <param name="excludeIfModIsPresent">The tree will only get loaded if none of the listed mods are present.</param>
        /// <param name="barkProduct">The item the tree drops when using the <see cref="BarkRemover"/> tool on it.</param>
        /// <param name="unfertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's unfertilised.</param>
        /// <param name="fertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's fertilised.</param>
        /// <returns><see langword="true"/> if data was successfully retrieved; otherwise, <see langword="false"/>.</returns>
        /// <remarks>A save must be loaded to successfully get a result.</remarks>
        public bool GetTreeById(int id, out string name, out Texture2D texture, out (float DaysBetweenProduce, int Product, int Amount) tappedProduct, out int wood, out bool dropsSap, out int seed, out int requiredToolLevel, out List<(int DaysBetweenProduce, int Product, int Amount, string[] Seasons)> shakingProducts, out List<string> includeIfModIsPresent, out List<string> excludeIfModIsPresent, out (int DaysBetweenProduce, int Product, int Amount) barkProduct, out float unfertilisedGrowthChance, out float fertilisedGrowthChance);

        /// <summary>Gets data of a tree by its name.</summary>
        /// <param name="name">The name of the tree to get the data of.</param>
        /// <param name="id">The id of the tree.</param>
        /// <param name="texture">The texture for the tree.</param>
        /// <param name="tappedProduct">The item the tree drops when using a tapper on it.</param>
        /// <param name="wood">The item the tree drops when it gets cut down.</param>
        /// <param name="dropsSap">Whether the tree drops sap when it gets cut down.</param>
        /// <param name="seed">The item to plant to grow the tree.</param>
        /// <param name="requiredToolLevel">The required tool level to cut down the tree.</param>
        /// <param name="shakingProducts">The items the tree can drop whenever it's shaken.</param>
        /// <param name="includeIfModIsPresent">The tree will only get loaded if atleast one of the listed mods are present.</param>
        /// <param name="excludeIfModIsPresent">The tree will only get loaded if none of the listed mods are present.</param>
        /// <param name="barkProduct">The item the tree drops when using the <see cref="BarkRemover"/> tool on it.</param>
        /// <param name="unfertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's unfertilised.</param>
        /// <param name="fertilisedGrowthChance">The chance the tree has to grow a stage (at the start of each day) when it's fertilised.</param>
        /// <returns><see langword="true"/> if data was successfully retrieved; otherwise, <see langword="false"/>.</returns>
        /// <remarks>A save must be loaded to successfully get a result.</remarks>
        public bool GetTreeByName(string name, out int id, out Texture2D texture, out (float DaysBetweenProduce, int Product, int Amount) tappedProduct, out int wood, out bool dropsSap, out int seed, out int requiredToolLevel, out List<(int DaysBetweenProduce, int Product, int Amount, string[] Seasons)> shakingProducts, out List<string> includeIfModIsPresent, out List<string> excludeIfModIsPresent, out (int DaysBetweenProduce, int Product, int Amount) barkProduct, out float unfertilisedGrowthChance, out float fertilisedGrowthChance);

        /// <summary>Gets the bark state of a tree.</summary>
        /// <param name="locationName">The name of the location that contains the tree.</param>
        /// <param name="tileLocation">The location of the tree to check.</param>
        /// <returns><see langword="true"/> if the tree at the given location has bark; otherwise, <see langword="false"/>.</returns>
        public bool GetBarkState(string locationName, Vector2 tileLocation);

        /// <summary>Sets the bark state of a tree.</summary>
        /// <param name="locationName">The name of the location that contains the tree.</param>
        /// <param name="tileLocation">The location of the tree to update.</param>
        /// <param name="hasBark">Whether the tree should be set as having bark.</param>
        /// <returns><see langword="true"/> if the tree at the given location was successfully updated; otherwise, <see langword="false"/>.</returns>
        public bool SetBarkState(string locationName, Vector2 tileLocation, bool hasBark);
    }
}
