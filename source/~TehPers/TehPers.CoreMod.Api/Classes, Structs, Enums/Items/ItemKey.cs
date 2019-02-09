using System;
using StardewModdingAPI;

namespace TehPers.CoreMod.Api.Items {
    public readonly struct ItemKey : IEquatable<ItemKey> {
        /// <summary>The mod which owns the item type identified by this key.</summary>
        public string OwnerId { get; }

        /// <summary>The local name of this key. It uniquely identifies an item type registered by its owner.</summary>
        public string LocalKey { get; }

        /// <summary>The global name of this key. It uniquely identifies an item type registered through Teh's Core Mod.</summary>
        public string GlobalKey => $"{this.OwnerId}:{this.LocalKey}";

        public ItemKey(IMod owner, string localKey) : this(owner.ModManifest.UniqueID, localKey) { }
        public ItemKey(IManifest manifest, string localKey) : this(manifest.UniqueID, localKey) { }
        private ItemKey(string ownerId, string localKey) {
            this.OwnerId = ownerId;
            this.LocalKey = localKey;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return obj is ItemKey other && this.Equals(other);
        }

        /// <inheritdoc />
        public bool Equals(ItemKey other) {
            return object.Equals(this.OwnerId, other.OwnerId) && string.Equals(this.LocalKey, other.LocalKey);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return unchecked(((this.OwnerId?.GetHashCode() ?? 0) * 397) ^ (this.LocalKey?.GetHashCode() ?? 0));
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.GlobalKey;
        }
    }
}