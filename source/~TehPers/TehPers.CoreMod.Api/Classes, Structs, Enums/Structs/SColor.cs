using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.Conflux.Matching;

namespace TehPers.CoreMod.Api.Structs {
    public readonly struct SColor : IEquatable<SColor>, IEquatable<Color> {
        public uint PackedValue { get; }
        public byte A => unchecked((byte) (this.PackedValue >> 24));
        public byte R => unchecked((byte) (this.PackedValue >> 16));
        public byte G => unchecked((byte) (this.PackedValue >> 8));
        public byte B => unchecked((byte) this.PackedValue);

        public SColor(float r, float g, float b) : this(r, g, b, 0) { }
        public SColor(float r, float g, float b, float a) : this((byte) (r * byte.MaxValue), (byte) (g * byte.MaxValue), (byte) (b * byte.MaxValue), (byte) (a * byte.MaxValue)) { }
        public SColor(byte r, byte g, byte b) : this(r, g, b, 1) { }
        public SColor(byte r, byte g, byte b, byte a) : this((uint) (a << 24) + (uint) (r << 16) + (uint) (g << 8) + b) { }
        public SColor(uint packedValue) {
            this.PackedValue = packedValue;
        }

        public bool Equals(SColor other) {
            return this.PackedValue == other.PackedValue;
        }

        public bool Equals(Color other) {
            return this.A == other.A && this.R == other.R && this.G == other.G && this.B == other.B;
        }

        public override bool Equals(object obj) {
            return obj.Match<object, bool>()
                .When<Color>(this.Equals)
                .When<SColor>(this.Equals)
                .Else(false);
        }

        public override int GetHashCode() {
            return (int) this.PackedValue;
        }

        public override string ToString() {
            return $"{{{{R:{this.R} G:{this.G} B:{this.B} A:{this.A}}}}}";
        }

        public static implicit operator Color(in SColor source) {
            return new Color(source.R, source.G, source.B, source.A);
        }

        public static implicit operator SColor(Color source) {
            return new SColor(source.R, source.G, source.B, source.A);
        }

        public static SColor operator +(in SColor first, in SColor second) {
            byte r = (byte) Math.Min(byte.MaxValue, first.R + second.R);
            byte g = (byte) Math.Min(byte.MaxValue, first.G + second.G);
            byte b = (byte) Math.Min(byte.MaxValue, first.B + second.B);
            byte a = (byte) Math.Min(byte.MaxValue, first.A + second.A);
            return new SColor(r, g, b, a);
        }

        public static SColor operator -(in SColor first, in SColor second) {
            byte r = (byte) Math.Max(byte.MinValue, first.R - second.R);
            byte g = (byte) Math.Max(byte.MinValue, first.G - second.G);
            byte b = (byte) Math.Max(byte.MinValue, first.B - second.B);
            byte a = (byte) Math.Max(byte.MinValue, first.A - second.A);
            return new SColor(r, g, b, a);
        }

        public static SColor operator *(in SColor first, in SColor second) {
            float r = (first.R / 255f) * (second.R / 255f);
            float g = (first.G / 255f) * (second.G / 255f);
            float b = (first.B / 255f) * (second.B / 255f);
            float a = (first.A / 255f) * (second.A / 255f);
            return new SColor(r, g, b, a);
        }

        public static SColor operator /(in SColor first, in SColor second) {
            float r = (float) first.R * byte.MaxValue / second.R;
            float g = (float) first.G * byte.MaxValue / second.G;
            float b = (float) first.B * byte.MaxValue / second.B;
            float a = (float) first.A * byte.MaxValue / second.A;
            return new SColor(r, g, b, a);
        }

        public static bool operator ==(in SColor first, in SColor second) {
            return first.Equals(second);
        }

        public static bool operator ==(in SColor first, Color second) {
            return first.Equals(second);
        }

        public static bool operator !=(in SColor first, in SColor second) {
            return !first.Equals(second);
        }

        public static bool operator !=(in SColor first, Color second) {
            return !first.Equals(second);
        }
    }
}
