/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace HatsOnCats.Framework.Configuration
{
    [JsonConverter(typeof(OffsetReferenceConverter))]
    internal struct OffsetReference
    {
        public Frame Frame { get; }

        public OffsetReference(Frame frame)
        {
            this.Frame = frame;
        }

        public static OffsetReference Zero = new OffsetReference(Frame.Zero);

        public override string ToString()
        {
            return $"!{this.Frame.ToString()}";
        }
    }
}
