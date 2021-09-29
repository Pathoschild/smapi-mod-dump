/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Xml.Serialization;

namespace DynamicGameAssets.Game
{
    /// <summary>A custom item added through Dynamic Game Assets.</summary>
    public interface IDGAItem
    {
        /// <summary>The unique ID of the content pack which added this item.</summary>
        [XmlIgnore]
        string SourcePack { get; }

        /// <summary>The item ID within the content pack. For a globally unique ID, see <see cref="FullId"/>.</summary>
        [XmlIgnore]
        string Id { get; }

        /// <summary>The globally unique item ID.</summary>
        [XmlIgnore]
        string FullId { get; }
    }
}
