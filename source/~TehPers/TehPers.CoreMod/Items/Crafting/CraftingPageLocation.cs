using System;

namespace TehPers.CoreMod.Items.Crafting {
    internal readonly struct CraftingPageLocation : IEquatable<CraftingPageLocation> {
        public int Page { get; }
        public int X { get; }
        public int Y { get; }

        public CraftingPageLocation(int page, int y, int x) {
            this.Page = page;
            this.X = x;
            this.Y = y;
        }

        public CraftingPageLocation NextLocation(int pageWidth, int pageHeight) {
            // Next slot on same row
            if (this.X + 1 < pageWidth) {
                return new CraftingPageLocation(this.Page, this.Y, this.X + 1);
            }

            // Next row on same page
            if (this.Y + 1 < pageHeight) {
                return new CraftingPageLocation(this.Page, this.Y + 1, 0);
            }

            // Next page
            return new CraftingPageLocation(this.Page + 1, 0, 0);
        }

        public CraftingPageLocation PreviousLocation(int pageWidth, int pageHeight) {
            // Previous slot on same row
            if (this.X > 0) {
                return new CraftingPageLocation(this.Page, this.Y, this.X - 1);
            }

            // Previous row on same page
            if (this.Y > 0) {
                return new CraftingPageLocation(this.Page, this.Y - 1, pageWidth - 1);
            }

            // Previous page
            return new CraftingPageLocation(this.Page - 1, pageHeight - 1, pageWidth - 1);
        }

        public CraftingPageLocation SlotBelow(int pageHeight) {
            // Next row on same page
            if (this.Y + 1 < pageHeight) {
                return new CraftingPageLocation(this.Page, this.Y + 1, this.X);
            }

            // Next page
            return new CraftingPageLocation(this.Page + 1, 0, this.X);
        }

        public void Deconstruct(out int page, out int y, out int x) {
            page = this.Page;
            y = this.Y;
            x = this.X;
        }

        public bool Equals(CraftingPageLocation other) {
            return this.Page == other.Page && this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj) {
            return obj is CraftingPageLocation other && this.Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = this.Page;
                hashCode = (hashCode * 397) ^ this.X;
                hashCode = (hashCode * 397) ^ this.Y;
                return hashCode;
            }
        }

        public override string ToString() {
            return $"{{{{Page: {this.Page}, Y: {this.Y}, X: {this.X}}}}}";
        }
    }
}