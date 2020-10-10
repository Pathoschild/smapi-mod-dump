/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;

namespace TehPers.CoreMod.Api.Items.Crafting {
    public readonly struct CraftingKey : IEquatable<CraftingKey> {
        /// <summary>The mod which owns the item type identified by this key.</summary>
        public string OwnerId { get; }

        /// <summary>The local name of this key. It uniquely identifies an item type registered by its owner.</summary>
        public string LocalKey { get; }

        /// <summary>The global name of this key. It uniquely identifies an item type registered through Teh's Core Mod.</summary>
        public string GlobalKey => $"{this.OwnerId}:{this.LocalKey}";

        public CraftingKey(string ownerId, string localKey) {
            this.OwnerId = ownerId;
            this.LocalKey = localKey;
        }

        /// <inheritdoc />
        public bool Equals(CraftingKey other) {
            return string.Equals(this.OwnerId, other.OwnerId) && string.Equals(this.LocalKey, other.LocalKey);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return obj is CraftingKey other && this.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return unchecked(((this.OwnerId != null ? this.OwnerId.GetHashCode() : 0) * 397) ^ (this.LocalKey != null ? this.LocalKey.GetHashCode() : 0));
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.GlobalKey;
        }

        public static implicit operator string(in CraftingKey key) {
            return key.ToString();
        }
    }
}