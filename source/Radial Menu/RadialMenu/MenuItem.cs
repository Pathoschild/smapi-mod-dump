/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadialMenu.Config;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using System.Numerics;
using System.Text;

namespace RadialMenu
{
    internal record MenuItem(
        string Title,
        string Description,
        int? StackSize,
        int? Quality,
        Texture2D? Texture,
        Rectangle? SourceRectangle,
        Rectangle? TintRectangle,
        Color? TintColor,
        Func<DelayedActions?, ItemAction, ItemActivationResult> Activate);

    internal class MenuItemBuilder(
        TextureHelper textureHelper,
        Action<CustomMenuItemConfiguration> customItemActivator)
    {
        private readonly Dictionary<(SpriteSourceFormat, string), TextureSegment?> spriteCache = [];

        public MenuItem CustomItem(CustomMenuItemConfiguration item)
        {
            var cacheKey = (item.SpriteSourceFormat, item.SpriteSourcePath);
            if (!spriteCache.TryGetValue(cacheKey, out var sprite))
            {
                sprite = textureHelper.GetSprite(item.SpriteSourceFormat, item.SpriteSourcePath);
                spriteCache.TryAdd(cacheKey, sprite);
            }
            return new(
                item.Name,
                item.Description,
                /* StackSize= */ null,
                /* Quality= */ null,
                sprite?.Texture,
                sprite?.SourceRect,
                /* TintRectangle= */ null,
                /* TintColor= */ null,
                (delayedActions, _) =>
                {
                    if (delayedActions == DelayedActions.All)
                    {
                        return ItemActivationResult.Delayed;
                    }
                    customItemActivator.Invoke(item);
                    return ItemActivationResult.Custom;
                });
        }

        public MenuItem GameItem(Item item, int itemIndex)
        {
            var data = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
            var texture = data.GetTexture();
            var sourceRect = data.GetSourceRect();
            var textureRedirect = GetTextureRedirect(item);
            if (textureRedirect is not null)
            {
                texture = textureRedirect.GetTexture();
                sourceRect = textureRedirect.GetSourceRect();
            }
            var (tintRect, tintColor) = GetTinting(item, textureRedirect ?? data);
            return new(
                item.DisplayName,
                UnparseText(item.getDescription()),
                item.maximumStackSize() > 1 ? item.Stack : null,
                item.Quality,
                texture,
                sourceRect,
                tintRect,
                tintColor,
                (delayedActions, preferredAction) =>
                    FuzzyActivation.ConsumeOrSelect(itemIndex, delayedActions, preferredAction));
        }

        private static ParsedItemData? GetTextureRedirect(Item item)
        {
            return item is StardewValley.Object obj && item.ItemId == "SmokedFish"
                ? ItemRegistry.GetData(obj.preservedParentSheetIndex.Value)
                : null;
        }

        private static (Rectangle? tintRect, Color? tintColor) GetTinting(
            Item item, ParsedItemData data)
        {
            if (item is not ColoredObject coloredObject)
            {
                return default;
            }
            if (item.ItemId == "SmokedFish")
            {
                // Smoked fish implementation is unique (and private) in ColoredObject.
                // We don't care about the animation here, but should draw it darkened; the quirky
                // way this is implemented is to draw a tinted version of the original item sprite
                // (not an overlay) sprite over top of the original sprite.
                return (data.GetSourceRect(), new Color(80, 30, 10) * 0.6f);
            }
            return !coloredObject.ColorSameIndexAsParentSheetIndex
                ? (data.GetSourceRect(1), coloredObject.color.Value)
                : (null, coloredObject.color.Value);
        }

        // When we call Item.getDescription(), most implementations go through `Game1.parseText`
        // which splits the string itself onto multiple lines. This tries to remove that, so that we
        // can do our own wrapping using our own width.
        //
        // N.B. The reason we don't just use `ParsedItemData.Description` is that, at least in the
        // current version, it's often only a "base description" and includes format placeholders,
        // or is missing suffixes.
        private static string UnparseText(string text)
        {
            var sb = new StringBuilder();
            var isWhitespace = false;
            var newlineCount = 0;
            foreach (var c in text)
            {
                if (c == ' ' || c == '\r' || c == '\n')
                {
                    if (!isWhitespace)
                    {
                        sb.Append(' ');
                    }
                    isWhitespace = true;
                    if (c == '\n')
                    {
                        newlineCount++;
                    }
                }
                else
                {
                    // If the original text has a "paragraph", the formatted text will often look
                    // strange if that is also collapsed into a space. So preserve _multiple_
                    // newlines somewhat as a single "paragraph break".
                    if (newlineCount > 1)
                    {
                        // From implementation above, newlines are counted as whitespace so we know
                        // that the last character will always be a space when hitting here.
                        sb.Length--;
                        sb.Append("\r\n\r\n");
                    }
                    sb.Append(c);
                    isWhitespace = false;
                    newlineCount = 0;
                }
            }
            if (isWhitespace)
            {
                sb.Length--;
            }
            return sb.ToString();
        }
    }
}
