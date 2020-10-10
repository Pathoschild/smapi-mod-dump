/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace HatsOnCats.Framework.Configuration
{
    [JsonConverter(typeof(OffsetConverter))]
    internal struct Offset
    {
        public Vector2 Value { get; }
        public OffsetReference Reference { get; }
        public bool IsReference { get; }

        public Offset(Vector2 value) : this(value, OffsetReference.Zero, false)
        {
        }

        public Offset(OffsetReference reference) : this(Vector2.Zero, reference, true)
        {
        }

        private Offset(Vector2 value, OffsetReference reference, bool isReference)
        {
            this.Value = value;
            this.Reference = reference;
            this.IsReference = isReference;
        }
    }
}
