/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

namespace TehPers.FishingOverhaul.Parsing
{
    /// <summary>
    /// A binary operataor in an expression.
    /// </summary>
    internal enum BinaryOperator
    {
        /// <summary>
        /// The add operator (<c>+</c>).
        /// </summary>
        Add,

        /// <summary>
        /// The subtraction operator (<c>-</c>).
        /// </summary>
        Subtract,

        /// <summary>
        /// The multiplication operator (<c>*</c>).
        /// </summary>
        Multiply,

        /// <summary>
        /// The division operator (<c>/</c>).
        /// </summary>
        Divide,

        /// <summary>
        /// The modulus operator (<c>%</c>).
        /// </summary>
        Modulo,

        /// <summary>
        /// The power operator (<c>^</c>).
        /// </summary>
        Power,
    }
}
