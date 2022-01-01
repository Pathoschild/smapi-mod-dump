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
using System.ComponentModel;
using System.Globalization;
using StardewModdingAPI;

namespace TehPers.Core.Api.Items
{
    /// <summary>
    /// A string key within a specific namespace.
    /// </summary>
    [TypeConverter(typeof(TypeConverter))]
    public readonly struct NamespacedKey : IEquatable<NamespacedKey>
    {
        /// <summary>
        /// The namespace for SDV.
        /// </summary>
        public const string StardewValleyNamespace = "StardewValley";

        /// <summary>
        /// The namespace this key is contained in.
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// The unique key within the namespace.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespacedKey"/> class.
        /// </summary>
        /// <param name="namespace">The namespace of the key.</param>
        /// <param name="key">The unique key within the namespace.</param>
        public NamespacedKey(string @namespace, string key)
        {
            this.Namespace = @namespace;
            this.Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespacedKey"/> class.
        /// </summary>
        /// <param name="manifest">The manifest that represents the namespace of the key.</param>
        /// <param name="key">The unique key within the namespace.</param>
        public NamespacedKey(IManifest manifest, string key)
            : this(manifest.UniqueID, key)
        {
        }

        /// <summary>
        /// Deconstructs this <see cref="NamespacedKey"/> into its components.
        /// </summary>
        /// <param name="namespace">The namespace of the key.</param>
        /// <param name="key">The unique key within the namespace.</param>
        public void Deconstruct(out string @namespace, out string key)
        {
            @namespace = this.Namespace;
            key = this.Key;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Namespace}:{this.Key}";
        }

        /// <inheritdoc/>
        public bool Equals(NamespacedKey other)
        {
            return this.Namespace == other.Namespace && this.Key == other.Key;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is NamespacedKey other && this.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Namespace, this.Key);
        }

        /// <summary>
        /// Compares two namespaced keys for equivalency.
        /// </summary>
        /// <param name="left">The first <see cref="NamespacedKey"/>.</param>
        /// <param name="right">The second <see cref="NamespacedKey"/>.</param>
        /// <returns>Whether the keys are equivalent.</returns>
        public static bool operator ==(NamespacedKey left, NamespacedKey right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two namespaced keys for inequivalency.
        /// </summary>
        /// <param name="left">The first <see cref="NamespacedKey"/>.</param>
        /// <param name="right">The second <see cref="NamespacedKey"/>.</param>
        /// <returns>Whether the keys are inequivalent.</returns>
        public static bool operator !=(NamespacedKey left, NamespacedKey right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Tries to parse a <see cref="NamespacedKey"/> from a raw string. Strings in the format
        /// <c>namespace:key</c> are parsed into their components. The key may contain colons, but
        /// the namespace cannot.
        /// </summary>
        /// <param name="raw">The raw string to parse.</param>
        /// <param name="key">The resulting <see cref="NamespacedKey"/>, if any.</param>
        /// <returns>Whether parsing was successful.</returns>
        public static bool TryParse(string raw, out NamespacedKey key)
        {
            var parts = raw.Split(':');
            if (parts.Length < 2)
            {
                key = default;
                return false;
            }

            key = new(parts[0], string.Join(':', parts[1..]));
            return true;
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a tool in the SDV namespace.
        /// </summary>
        /// <param name="toolType">The tool type.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvTool(string toolType)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Tool, toolType);
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a tool in the SDV namespace.
        /// </summary>
        /// <param name="toolType">The tool type.</param>
        /// <param name="quality">The quality level of the tool.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvTool(string toolType, int quality)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Tool, $"{toolType}/{quality}");
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a clothing item in the SDV namespace.
        /// </summary>
        /// <param name="parentSheetIndex">The index of the clothing item.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvClothing(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Clothing, parentSheetIndex.ToString());
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a wallpaper item in the SDV namespace.
        /// </summary>
        /// <param name="parentSheetIndex">The index of the wallpaper item.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvWallpaper(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Wallpaper, parentSheetIndex.ToString());
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a flooring item in the SDV namespace.
        /// </summary>
        /// <param name="parentSheetIndex">The index of the flooring item.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvFlooring(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Flooring, parentSheetIndex.ToString());
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for boots in the SDV namespace.
        /// </summary>
        /// <param name="parentSheetIndex">The index of the boots.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvBoots(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Boots, parentSheetIndex.ToString());
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a hat in the SDV namespace.
        /// </summary>
        /// <param name="parentSheetIndex">The index of the hat.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvHat(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Hat, parentSheetIndex.ToString());
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a weapon in the SDV namespace.
        /// </summary>
        /// <param name="parentSheetIndex">The index of the weapon.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvWeapon(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Weapon, parentSheetIndex.ToString());
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a furniture item in the SDV namespace.
        /// </summary>
        /// <param name="parentSheetIndex">The index of the furniture item.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvFurniture(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Furniture, parentSheetIndex.ToString());
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a craftable item in the SDV namespace.
        /// </summary>
        /// <param name="parentSheetIndex">The index of the craftable item.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvBigCraftable(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.BigCraftable, parentSheetIndex.ToString());
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a ring in the SDV namespace.
        /// </summary>
        /// <param name="parentSheetIndex">The index of the ring.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvRing(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Ring, parentSheetIndex.ToString());
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for an object in the SDV namespace.
        /// </summary>
        /// <param name="parentSheetIndex">The index of the object.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvObject(int parentSheetIndex)
        {
            return NamespacedKey.SdvCustom(ItemTypes.Object, parentSheetIndex.ToString());
        }

        /// <summary>
        /// Creates a <see cref="NamespacedKey"/> for a custom item in the SDV namespace.
        /// </summary>
        /// <param name="itemType">The item type of the custom item.</param>
        /// <param name="key">The unique key of the custom item within the namespace and item type.</param>
        /// <returns>The resulting <see cref="NamespacedKey"/>.</returns>
        public static NamespacedKey SdvCustom(string itemType, string key)
        {
            return new(NamespacedKey.StardewValleyNamespace, $"{itemType}/{key}");
        }

        internal class TypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value
            )
            {
                if (value is string raw && NamespacedKey.TryParse(raw, out var key))
                {
                    return key;
                }

                return base.ConvertFrom(context, culture, value)!;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(string)
                    || base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value,
                Type destinationType
            )
            {
                if (value is NamespacedKey key && destinationType == typeof(string))
                {
                    return key.ToString();
                }

                return base.ConvertTo(context, culture, value, destinationType)!;
            }
        }
    }
}