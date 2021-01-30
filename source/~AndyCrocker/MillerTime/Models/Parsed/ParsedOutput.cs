/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace MillerTime.Models.Parsed
{
    /// <summary>Represents the output of a recipe.</summary>
    /// <remarks>This is a version of <see cref="MillerTime.Models.Converted.Output"/> that has <see cref="MillerTime.Models.Converted.Output.Id"/> as <see langword="string"/>.<br/>The reason this is done is so content packs can have tokens in place of the ids to call mod APIs to get the id (so JsonAsset items can be used for example).</remarks>
    public class ParsedOutput
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the output.</summary>
        public string Id { get; set; } = "-1";

        /// <summary>The number of objects to output.</summary>
        public int Amount { get; set; } = 1;


        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public override string ToString() => $"<Id: {Id}, Amount: {Amount}>";
    }
}
