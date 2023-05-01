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
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack.Actions;
using SolidFoundations.Framework.Models.Data;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Models.ContentPack
{
    public class Sequence
    {
        public int Frame { get; set; }
        public int? _cachedDuration { get; set; }
        public object Duration { private get; set; }
        public string Condition { get; set; }
        public string[] ModDataFlags { get; set; }
        public List<ModifyModDataAction> ModifyFlags { get; set; }
        public PlaySoundAction PlaySound { get; set; }

        public int GetDuration(bool recalculate = false, Random random = null)
        {
            if (recalculate is false && _cachedDuration is not null)
            {
                return (int)_cachedDuration;
            }

            return RefreshDuration(random);
        }

        public int RefreshDuration(Random random = null)
        {
            if (random is null)
            {
                random = Game1.random;
            }
            _cachedDuration = null;

            if (Duration is JObject jObject)
            {
                if (jObject["RandomRange"] != null)
                {
                    var randomRange = JsonConvert.DeserializeObject<RandomRange>(jObject["RandomRange"].ToString());
                    _cachedDuration = randomRange.Get(random);
                }
                else if (jObject["RandomValue"] != null)
                {
                    var randomValue = JsonConvert.DeserializeObject<RandomValue>(jObject["RandomValue"].ToString());
                    _cachedDuration = randomValue.Get<int>(random);
                }
            }
            else if (Duration is (Int32 or Int64))
            {
                _cachedDuration = Convert.ToInt32(Duration);
            }

            if (_cachedDuration is null)
            {
                _cachedDuration = 0;
            }

            return (int)_cachedDuration;
        }
    }
}
