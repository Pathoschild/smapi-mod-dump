/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SolidFoundations.Framework.Models.Data;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Models.ContentPack
{
    public class Light
    {
        public int TextureId { get; set; } = 4;
        public int[] Color { get; set; }
        public object Radius { get; set; }
        private float _radius;
        public object UpdateIntervalMilliseconds { get; set; } = 1000f;
        private float _updateIntervalMilliseconds;
        internal float ElapsedMilliseconds { get; set; }
        public Point Tile { get; set; }
        public Point TileOffsetInPixels { get; set; }

        internal float GetRadius(Random random = null)
        {
            if (random is null)
            {
                random = Game1.random;
            }

            if (Radius is JObject modelContext)
            {
                if (modelContext["RandomRange"] != null)
                {
                    var randomRange = JsonConvert.DeserializeObject<RandomRange>(modelContext["RandomRange"].ToString());
                    _radius = randomRange.Get(random);
                }
                else if (modelContext["RandomValue"] != null)
                {
                    var randomValue = JsonConvert.DeserializeObject<RandomValue>(modelContext["RandomValue"].ToString());
                    _radius = randomValue.Get<float>(random);
                }
            }
            else
            {
                _radius = float.Parse(Radius.ToString());
            }

            return _radius;
        }

        internal float GetUpdateInterval(Random random = null, bool recalculateValue = false)
        {
            if (random is null)
            {
                random = Game1.random;
            }

            if (recalculateValue)
            {
                if (UpdateIntervalMilliseconds is JObject modelContext)
                {
                    if (modelContext["RandomRange"] != null)
                    {
                        var randomRange = JsonConvert.DeserializeObject<RandomRange>(modelContext["RandomRange"].ToString());
                        _updateIntervalMilliseconds = randomRange.Get(random);
                    }
                    else if (modelContext["RandomValue"] != null)
                    {
                        var randomValue = JsonConvert.DeserializeObject<RandomValue>(modelContext["RandomValue"].ToString());
                        _updateIntervalMilliseconds = randomValue.Get<float>(random);
                    }
                }
                else
                {
                    _updateIntervalMilliseconds = float.Parse(UpdateIntervalMilliseconds.ToString());
                }
            }

            return _updateIntervalMilliseconds;
        }

        internal int GetTextureSource()
        {
            int source = TextureId;
            if (TextureId == 3)
            {
                source = 2;
            }
            else if (TextureId < 1 || TextureId > 8)
            {
                source = 1;
            }

            return source;
        }

        internal Color GetColor()
        {
            int alpha = 255;
            if (3 < Color.Length)
            {
                alpha = Color[3];
            }
            return new Color(byte.MaxValue - Color[0], byte.MaxValue - Color[1], byte.MaxValue - Color[2], alpha);
        }
    }
}