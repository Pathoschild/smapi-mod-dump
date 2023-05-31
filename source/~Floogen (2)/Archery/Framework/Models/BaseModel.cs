/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models.Crafting;
using Archery.Framework.Models.Display;
using Archery.Framework.Models.Enums;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Archery.Framework.Models
{
    public class BaseModel
    {
        public string DisplayName { get { return string.IsNullOrEmpty(_displayName) ? Name : _displayName; } set { _displayName = value; } }
        private string _displayName;
        public string Name { get; set; }
        public string Description { get; set; }

        public List<ItemSpriteModel> ConditionalIcons { get; set; } = new List<ItemSpriteModel>();

        // Used additively by both weapons and ammo
        public float CriticalChance { get; set; }
        public float CriticalDamageMultiplier { get; set; } = 1f;

        public ItemSpriteModel Icon { get; set; }
        public DirectionalSpriteModel DirectionalSprites { get; set; }

        public RecipeModel Recipe { get; set; }
        public ShopModel Shop { get; set; }
        public List<string> Filter { get; set; }

        internal string Id { get; set; }
        internal IContentPack ContentPack { get; set; }
        internal Texture2D Texture { get; set; }
        internal string TexturePath { get; set; }
        internal ITranslationHelper Translations { get; set; }


        internal bool IsFilterDefined()
        {
            if (Filter is null || Filter.Count == 0)
            {
                return false;
            }

            return true;
        }

        internal bool IsWithinFilter(string id)
        {
            if (IsFilterDefined() is false)
            {
                return false;
            }

            foreach (var filterId in Filter)
            {
                try
                {
                    var regex = new Regex(filterId);
                    if (regex.IsMatch(id))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Archery.monitor.LogOnce($"The item {Id} contains an invalid regex in Filter ({filterId}). See the log for details.", LogLevel.Warn);
                    Archery.monitor.LogOnce($"[{Id}] Failed to parse filtered ID {filterId}:\n {ex}", LogLevel.Trace);
                }
            }

            return false;
        }

        internal ItemSpriteModel GetIcon(Farmer who, Tool tool = null)
        {
            foreach (var sprite in ConditionalIcons.Where(s => s is not null))
            {
                if (sprite.AreConditionsValid(who, tool))
                {
                    return sprite;
                }
            }
            Archery.conditionManager.Reset(ConditionalIcons);

            return Icon;
        }

        internal Direction GetSpriteDirectionFromGivenDirection(Farmer who)
        {
            if (who is null || DirectionalSprites is null)
            {
                return Direction.Any;
            }

            return DirectionalSprites.GetActualDirection((Direction)who.FacingDirection);
        }

        internal WorldSpriteModel GetSpriteFromDirection(Farmer who, Tool tool)
        {
            if (who is null || DirectionalSprites is null)
            {
                return null;
            }

            var spritesInGivenDirection = DirectionalSprites.GetSpritesFromDirection((Direction)who.FacingDirection);
            if (spritesInGivenDirection.Count == 0)
            {
                spritesInGivenDirection = DirectionalSprites.GetSpritesFromDirection(Direction.Any);
            }

            return GetValidOrDefaultSprite(who, tool, spritesInGivenDirection);
        }

        internal WorldSpriteModel GetValidOrDefaultSprite(Farmer who, Tool tool, List<WorldSpriteModel> sprites)
        {
            foreach (var sprite in sprites.Where(s => s is not null))
            {
                if (sprite.AreConditionsValid(who, tool))
                {
                    return sprite;
                }
            }
            Archery.conditionManager.Reset(sprites);

            return sprites.FirstOrDefault();
        }

        internal virtual void SetId(IContentPack contentPack)
        {
            if (Recipe is not null)
            {
                Recipe.ParentId = Id;
            }
        }

        internal string GetTranslation(string text)
        {
            if (Translations is not null && Translations.GetTranslations().Any(t => t.Key == text))
            {
                return Translations.Get(text);
            }

            return text;
        }
    }
}
