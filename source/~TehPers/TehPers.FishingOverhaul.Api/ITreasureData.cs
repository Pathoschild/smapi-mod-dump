using System;
using System.Collections.Generic;
using StardewValley;
using TehPers.Core.Api.Weighted;

namespace TehPers.FishingOverhaul.Api {
    public interface ITreasureData : IWeighted {
        bool AllowDuplicates { get; }
        bool MeleeWeapon { get; }
        int MinAmount { get; }
        int MaxAmount { get; }

        /// <summary>Whether the given farmer can obtain this treasure.</summary>
        /// <param name="farmer">The farmer.</param>
        /// <returns>True if this treasure is obtainable, false if not.</returns>
        bool IsValid(Farmer farmer);

        /// <summary>Gets all the IDs in this treasure data.</summary>
        /// <returns>All the IDs that can be chosen from in this treasure data.</returns>
        /// <remarks><see cref="Array"/> implements <see cref="IList{T}"/></remarks>
        IList<int> PossibleIds();
    }
}