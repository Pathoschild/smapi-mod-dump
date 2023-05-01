/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlternativeTextures.Framework.Models
{
    public class AlternativeTextureModel
    {
        public string Owner { get; set; }
        public string ItemName { get; set; }
        public List<string> CollectiveNames { get; set; } = new List<string>();
        internal int ItemId { get; set; } = -1;
        public string Type { get; set; }
        [Obsolete("No longer used due SMAPI 3.14.0 allowing for passive invalidation checks.")]
        public bool EnableContentPatcherCheck { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
        public List<string> Seasons { get; set; } = new List<string>(); // For use by mod user to determine which seasons the texture is valid for
        internal string Season { get; set; } // Used by framework to split the Seasons property into individual AlternativeTextureModel models
        internal string TextureId { get; set; }
        internal string ModelName { get; set; }
        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }
        public int Variations { get; set; } = 1;
        internal int MaxVariationsPerTextures { get; set; } = -1;
        internal string TileSheetPath { get; set; }
        internal Dictionary<int, Texture2D> Textures { get; set; } = new Dictionary<int, Texture2D>();
        public List<VariationModel> ManualVariations { get; set; } = new List<VariationModel>();
        public List<AnimationModel> Animation { get; set; } = new List<AnimationModel>();

        public static int MAX_TEXTURE_HEIGHT { get { return 16384; } }
        internal enum TextureType
        {
            Unknown,
            Craftable,
            Grass,
            Tree,
            FruitTree,
            Crop,
            GiantCrop,
            ResourceClump,
            Bush,
            Flooring,
            Furniture,
            Character,
            Building,
            Decoration
        }

        public AlternativeTextureModel ShallowCopy()
        {
            return (AlternativeTextureModel)this.MemberwiseClone();
        }

        public string GetTextureType()
        {
            if (!Enum.TryParse<TextureType>(Type.Trim(), true, out var textureType))
            {
                return TextureType.Unknown.ToString();
            }

            return textureType.ToString();
        }

        public string GetId()
        {
            return TextureId;
        }

        public string GetTokenId(int? variation = null)
        {
            string seasonSuffix = String.IsNullOrEmpty(Season) ? String.Empty : String.Concat("_", Season);
            string variationSuffix = variation is null ? String.Empty : String.Concat("_", variation);
            return String.Concat(Owner, ".", ItemName, seasonSuffix, variationSuffix);
        }

        public string GetNameWithSeason()
        {
            return ModelName;
        }

        public int GetVariations()
        {
            return ManualVariations.Where(v => v.Id >= 0).Count() > 0 ? ManualVariations.Where(v => v.Id >= 0).Count() : Variations;
        }

        public List<AnimationModel> GetAnimationData(int variation)
        {
            var manualVariation = ManualVariations.FirstOrDefault(v => v.Id == variation && v.HasAnimation());
            if (manualVariation != null)
            {
                return manualVariation.Animation;
            }

            return Animation;
        }

        public AnimationModel GetAnimationDataAtIndex(int variation, int index)
        {
            return GetAnimationData(variation).ElementAt(index);
        }

        public Texture2D GetTexture(int variation)
        {
            if (Textures.ContainsKey(variation))
            {
                return Textures[variation];
            }

            return Textures[0];
        }

        public int GetTextureOffset(int variation)
        {
            return 0;
        }

        public Color GetRandomTint(int variation)
        {
            if (!HasTint(variation))
            {
                return Color.White;
            }

            var tints = ManualVariations.First(v => v.Id == variation).Tints;
            var selectedTint = tints[Game1.random.Next(tints.Count())];
            return new Color(selectedTint[0], selectedTint[1], selectedTint[2], selectedTint[3]);
        }

        public bool IsDecoration()
        {
            return String.Equals(GetTextureType(), "Decoration", StringComparison.OrdinalIgnoreCase);
        }

        public bool HasKeyword(string variationString, string keyword)
        {
            if (!Int32.TryParse(variationString, out var variation))
            {
                return false;
            }

            return HasKeyword(variation, keyword);
        }

        public bool HasKeyword(int variation, string keyword)
        {
            if (ManualVariations.Any(v => v.Id == variation))
            {
                return ManualVariations.First(v => v.Id == variation).Keywords.Any(k => k.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return Keywords.Any(k => k.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public bool HasAnimation(int variation)
        {
            return Animation.Count() > 0 || ManualVariations.Any(v => v.Id == variation && v.HasAnimation());
        }

        public bool HasTint(int variation)
        {
            return ManualVariations.Any(v => v.Id == variation && v.HasTint());
        }

        public override string ToString()
        {
            return $"\n[\n" +
                $"\tOwner: {Owner} | ItemName: {ItemName} | ItemId: {ItemId} | Type: {Type} | Season: {Season}\n" +
                $"\tTextureWidth x TextureHeight: [{TextureWidth}x{TextureHeight}] | Variations: {Variations}\n";
        }
    }
}
