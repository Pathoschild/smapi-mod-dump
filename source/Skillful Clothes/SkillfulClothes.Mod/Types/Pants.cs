/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
    public static class KnownPants
    {
        public static Pants None = -1,
        FarmerPants = 0,
        Shorts = 1,
        Dress = 2,
        Skirt = 3,
        PleatedSkirt = 4,
        DinosaurPants = 5,
        GrassSkirt = 6,
        LuauSkirt = 7,
        GeniePants = 8,
        TightPants = 9,
        BaggyPants = 10,
        SimpleDress = 11,
        RelaxedFitPants = 12,
        RelaxedFitShorts = 13,
        PolkaDotShorts = 14,
        TrimmedLuckyPurpleShorts = 15,
        PrismaticPants = 998,
        PrismaticGeniePants = 999;

        static Dictionary<string, Pants> lut_ids = new Dictionary<string, Pants>();
        static Dictionary<string, Pants> lut_names = new Dictionary<string, Pants>();

        static KnownPants()
        {
            foreach (var field in typeof(KnownPants)
                .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(x => x.FieldType == typeof(Pants)))
            {
                var pants = field.GetValue(null) as Pants;
                pants.ItemName = field.Name;

                lut_ids.Add(pants.ItemId, pants);
                lut_names.Add(pants.ItemName, pants);
            }
        }

        public static Pants GetById(string itemId)
        {
            if (itemId == null) return KnownPants.None;

            if (lut_ids.TryGetValue(itemId, out Pants knownPants) || lut_names.TryGetValue(itemId, out knownPants))
            {
                return knownPants;
            }
            else
                return new Pants(itemId) { ItemName = itemId };
        }
    }

    public class Pants : AlphanumericItemId
    {        
        public Pants(string itemId)
            : base(itemId, ClothingItemType.Pants)
        {
        }

        public static implicit operator Pants(int value)
        {
            return new Pants(value.ToString());
        }

        public static implicit operator Pants(string value)
        {
            return new Pants(value);
        }
    }
}
