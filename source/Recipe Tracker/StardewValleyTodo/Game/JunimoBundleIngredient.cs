/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System;

namespace StardewValleyTodo.Game {
    /// <summary>
    /// Bundle ingredient.
    /// </summary>
    public class JunimoBundleIngredient {
        /// <summary>
        /// Object identifier.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Index within bundle.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Localized name.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Required count.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Required minimal quality.
        /// </summary>
        public int Quality { get; }

        /// <summary>
        /// Is donated.
        /// </summary>
        public bool IsDonated { get; set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="id">Object identifier</param>
        /// <param name="index">Index within bundle</param>
        /// <param name="displayName">Localized name</param>
        /// <param name="count">Required count</param>
        /// <param name="quality">Required quality</param>
        /// <param name="isDonated">Is donated</param>
        public JunimoBundleIngredient(string id, int index, string displayName, int count, int quality, bool isDonated) {
            Id = id;
            Index = index;
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Count = count;
            Quality = quality;
            IsDonated = isDonated;
        }
    }
}
