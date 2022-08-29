/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

namespace SmartBuilding
{
    public enum ProducerType
    {
        /// <summary>
        ///     This means the game automatically removes recipe items from the player's inventory (i.e., furnaces).
        /// </summary>
        AutomaticRemoval,

        /// <summary>
        ///     This means we need to handle the removal of items ourselves.
        /// </summary>
        ManualRemoval,

        /// <summary>
        ///     This is purely for Prismatic Fire integration.
        /// </summary>
        TechnicallyNotAProducerButIsATorch,

        /// <summary>
        ///     This is not a producer.
        /// </summary>
        NotAProducer
    }
}
