using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api.Conflux.Matching;

namespace TehPers.CoreMod.Api.Structs {
    public readonly struct AssetLocation : IEquatable<AssetLocation> {
        public string Path { get; }
        public ContentSource Source { get; }

        public AssetLocation(string path, ContentSource source) {
            this.Source = source;
            this.Path = AssetLocation.Normalize(path);
        }

        public T Load<T>(IContentHelper contentHelper) {
            switch (this.Source) {
                case ContentSource.GameContent:
                    return Game1.content.Load<T>(this.Path);
                case ContentSource.ModFolder:
                    return contentHelper.Load<T>(this.Path);
                default:
                    throw new InvalidOperationException($"Could not load from content source: {this.Source}");
            }
        }

        public override bool Equals(object obj) {
            return obj is AssetLocation other && this.Equals(other);
        }

        public bool Equals(AssetLocation other) {
            return this.Source == other.Source && string.Equals(this.Path, other.Path, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() {
            return unchecked(((this.Path?.GetHashCode() ?? 0) * 397) ^ (int) this.Source);
        }

        public override string ToString() {
            return $"{this.Path} from {this.Source}";
        }

        #region Static
        public static IEnumerable<string> GetParts(string path) {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            return path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(ImmutableStack<string>.Empty, (parts, part) => {
                    return part.Match<string, ImmutableStack<string>>()
                        .When(".", parts) // Do nothing
                        .When(p => p == ".." && !parts.Any(), parts.Push) // Move outside the root/current directory
                        .When(p => p == ".." && parts.Peek() == "..", parts.Push) // Move even further outside the root/current directory
                        .When(p => p == "..", _ => parts.Pop()) // Move up a directory
                        .Else(parts.Push);
                }
                // Move into a directory
                ).Reverse().AsEnumerable();
        }

        public static string Normalize(string path) {
            return string.Join("\\", AssetLocation.GetParts(path));
        }
        #endregion
    }
}