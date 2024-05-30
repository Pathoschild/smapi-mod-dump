/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Enums;
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
        public string PackName { get; set; }
        public string Author { get; set; }
        public string ItemName { get { return string.IsNullOrEmpty(_itemName) ? ItemId : _itemName; } set { _itemName = value; } }
        private string _itemName;
        public string ItemId { get; set; }
        public List<string> CollectiveNames { get; set; } = new List<string>();
        public List<string> CollectiveIds { get; set; } = new List<string>();
        public TextureType Type { get; set; }
        [Obsolete("No longer used due SMAPI 3.14.0 allowing for passive invalidation checks.")]
        public bool EnableContentPatcherCheck { get; set; }
        public bool IgnoreBuildingColorMask { get; set; } // Only usable by Type == "Building"
        public List<string> Keywords { get; set; } = new List<string>();
        public List<string> Seasons { get; set; } = new List<string>(); // For use by mod user to determine which seasons the texture is valid for
        internal string Season { get; set; } // Used by framework to split the Seasons property into individual AlternativeTextureModel models
        internal string TextureId { get; set; }
        internal string ModelName { get; set; }
        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }
        public int Variations { get; set; } = 1;
        public int? DefaultVariation { get; set; }
        internal int MaxVariationsPerTextures { get; set; } = -1;
        internal string TileSheetPath { get; set; }
        internal Dictionary<int, Texture2D> Textures { get; set; } = new Dictionary<int, Texture2D>();
        public List<VariationModel> ManualVariations { get; set; } = new List<VariationModel>();
        public List<AnimationModel> Animation { get; set; } = new List<AnimationModel>();

        public static int MAX_TEXTURE_HEIGHT { get { return 16384; } }
        public enum TextureType
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
            Decoration,
            ArtifactSpot
        }

        public AlternativeTextureModel ShallowCopy()
        {
            return (AlternativeTextureModel)this.MemberwiseClone();
        }

        public string GetTextureType()
        {
            return Type.ToString();
        }

        public string GetId()
        {
            return TextureId;
        }

        public bool IsUsingItemId()
        {
            return string.IsNullOrEmpty(ItemId) is false;
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

        public bool IsManualVariationsValid()
        {
            if (ManualVariations.Any(v => v.Id == 1) is true && ManualVariations.Any(v => v.Id == 0) is false)
            {
                return false;
            }

            return true;
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

        public int GetNextValidFrameFromIndex(int variation, int index, bool isMachineActive)
        {
            var animationData = GetAnimationData(variation);

            index += 1;
            if (index >= GetAnimationData(variation).Count())
            {
                index = 0;
                return index;
            }

            return IsFrameValid(variation, index, isMachineActive) ? index : GetNextValidFrameFromIndex(variation, index, isMachineActive);
        }

        public Texture2D GetTexture(int variation)
        {
            var texture = Textures.ContainsKey(variation) ? Textures[variation] : Textures[0];
            if (texture.IsDisposed)
            {
                AlternativeTextures.monitor.LogOnce($"Error drawing the texture {TextureId}: It was incorrectly disposed!", StardewModdingAPI.LogLevel.Warn);
                AlternativeTextures.monitor.LogOnce(this.ToString(), StardewModdingAPI.LogLevel.Trace);
                return AlternativeTextures.textureManager.ErrorTexture;
            }

            return texture;
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

        internal bool IsFrameValid(int variation, int currentFrame, bool isMachineActive)
        {
            var animationData = GetAnimationDataAtIndex(variation, currentFrame);
            if (animationData is null || (animationData.Type is FrameType.MachineActive && isMachineActive is false) || (animationData.Type is FrameType.MachineIdle && isMachineActive is true))
            {
                return false;
            }

            return true;
        }

        internal List<string> HandleNameChanges()
        {
            List<string> changedNames = new List<string>();
            if (CollectiveNames is not null)
            {
                for (int x = 0; x < CollectiveNames.Count; x++)
                {
                    var changedName = AlternativeTextureModel.GetNameChange(Type, CollectiveNames[x]);

                    if (CollectiveNames[x] != changedName)
                    {
                        changedNames.Add(changedName);
                        CollectiveNames[x] = changedName;
                    }
                }
            }

            return changedNames;
        }

        private static string GetNameChange(TextureType type, string name)
        {
            if (type is TextureType.Building)
            {
                if (name.Equals("Log Cabin", StringComparison.OrdinalIgnoreCase) || name.Equals("Plank Cabin", StringComparison.OrdinalIgnoreCase) || name.Equals("Stone Cabin", StringComparison.OrdinalIgnoreCase))
                {
                    return "Cabin";
                }
            }

            return name;
        }

        internal bool HandleTypeChanges()
        {
            if (CollectiveNames is null)
            {
                return false;
            }

            if (Type is TextureType.Craftable && CollectiveNames.Any(n => n.Equals("Artifact Spot", StringComparison.OrdinalIgnoreCase)))
            {
                Type = TextureType.ArtifactSpot;
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"\n[\n" +
                $"\tOwner: {Owner} | ItemName: {ItemName} | ItemId: {ItemId} | Type: {Type} | Season: {Season}\n" +
                $"\tTextureWidth x TextureHeight: [{TextureWidth}x{TextureHeight}] | Variations: {Variations}\n";
        }
    }
}
