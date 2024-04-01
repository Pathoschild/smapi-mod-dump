/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles.Lib.AdvancedPricing
{
    public class AdvancedPricingItem : Item
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public const string ASSET = "UnlockableBundles/AdvancedPricing";
        public const string APTYPEDEFINITION = "(UB.AP)";
        public const string FLAVOREDTYPEDEFINITION = "(UB.Flavored)";

        public Item ItemCopy;
        private Item SubItemCopy;
        private string TextureAsset = "";
        private AnimatedTexture Texture;
        private AnimatedTexture SubTexture;
        private string Description;
        private int ItemSpriteSize = 16;
        private float ItemSpriteScale { get => 64f / ItemSpriteSize; }
        private int SubItemSpriteSize = 16;
        private float SubItemSpriteScale { get => 64f / SubItemSpriteSize; }
        public List<string> ContextTags;
        public List<string> ItemTypes;

        public bool UsesFlavoredSyntax;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
        }
        public static Item parseItem(string id, int initialStack = 0, int quality = 0)
        {
            var cleanedId = id.TrimStart().StartsWith(APTYPEDEFINITION, StringComparison.OrdinalIgnoreCase)
                ? id.TrimStart()[APTYPEDEFINITION.Length..].Trim()
                : id.Trim();

            var asset = Helper.GameContent.Load<Dictionary<string, AdvancedPricing>>(ASSET);

            if (asset.TryGetValue(cleanedId, out var el))
                return new AdvancedPricingItem(cleanedId, el, initialStack, quality);

            else {
                return new AdvancedPricingItem() { ItemId = cleanedId, Name = "Error Item" };

            }
        }

        public AdvancedPricingItem() { }
        public AdvancedPricingItem(string id, AdvancedPricing advancedPricing, int initialStack = 0, int quality = 0)
        {
            ItemId = id;
            Stack = initialStack;
            Quality = quality;
            Name = advancedPricing.Name;
            ContextTags = advancedPricing.ContextTags ?? new();
            ItemTypes = advancedPricing.ItemTypes ?? new();

            if (advancedPricing.ItemSpriteSize > 0)
                ItemSpriteSize = advancedPricing.ItemSpriteSize;

            if (advancedPricing.SubItemSpriteSize > 0)
                SubItemSpriteSize = advancedPricing.SubItemSpriteSize;

            if (advancedPricing.ItemCopy is not null && advancedPricing.ItemCopy.Trim() != "")
                ItemCopy = Unlockable.parseItem(advancedPricing.ItemCopy, initialStack, quality);

            if (advancedPricing.ItemSprite is not null && advancedPricing.ItemSprite.Trim() != "") {
                var texture = Helper.GameContent.Load<Texture2D>(advancedPricing.ItemSprite);
                Texture = new AnimatedTexture(texture, advancedPricing.ItemSpriteAnimation, ItemSpriteSize, ItemSpriteSize);
                TextureAsset = advancedPricing.ItemSprite;
            }

            if (advancedPricing.SubItemCopy is not null && advancedPricing.SubItemCopy != "")
                SubItemCopy = Unlockable.parseItem(advancedPricing.SubItemCopy, 1, 0);

            if (advancedPricing.SubItemSprite is not null & advancedPricing.SubItemSprite != "") {
                var subTexture = Helper.GameContent.Load<Texture2D>(advancedPricing.SubItemSprite);
                SubTexture = new AnimatedTexture(subTexture, advancedPricing.SubItemSpriteAnimation, SubItemSpriteSize, SubItemSpriteSize);
            }

            Description = advancedPricing.Description;
        }

        public static Item parseFlavoredItem(string id, int initialStack, int quality)
        {
            id = id.TrimStart().StartsWith(FLAVOREDTYPEDEFINITION, StringComparison.OrdinalIgnoreCase)
                ? id.TrimStart()[FLAVOREDTYPEDEFINITION.Length..].Trim()
                : id.Trim();

            StardewValley.Object ingredient = null;
            StardewValley.Object obj = null;

            var preserveType = (StardewValley.Object.PreserveType)Enum.Parse(typeof(StardewValley.Object.PreserveType), id.Split(".")[0].Trim());
            ingredient = ItemRegistry.Create<StardewValley.Object>(string.Join(".", id.Split(".").Skip(1)));
            obj = ItemRegistry.GetObjectTypeDefinition().CreateFlavoredItem(preserveType, ingredient);

            var res = new AdvancedPricingItem() {
                ItemId = id,
                Name = obj.Name,
                Stack = initialStack,
                Quality = quality,
                ContextTags = new() {
                        ItemContextTagManager.SanitizeContextTag("id_" + obj.QualifiedItemId),
                        "preserve_sheet_index_" + ItemContextTagManager.SanitizeContextTag(obj.preservedParentSheetIndex.Value)
                    },
                ItemTypes = new() { obj.TypeDefinitionId },
                ItemCopy = obj,
                SubItemCopy = ingredient,
                UsesFlavoredSyntax = true
            };

            return res;
        }

        public override string TypeDefinitionId => UsesFlavoredSyntax ? FLAVOREDTYPEDEFINITION : APTYPEDEFINITION;

        public override void drawInMenu(SpriteBatch b, Vector2 screenPosition, float scale, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (drawShadow)
                b.Draw(Game1.shadowTexture, screenPosition + new Vector2(32f, 48f), Game1.shadowTexture.Bounds, color * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);

            if (Texture is not null) {
                var offsetRectangle = Texture.getOffsetRectangle();
                b.Draw(Texture.Texture, screenPosition + new Vector2(32f, 32f), offsetRectangle, color * transparency, 0f, new Vector2(offsetRectangle.Width / 2, offsetRectangle.Height / 2), ItemSpriteScale * scale, SpriteEffects.None, layerDepth);
                DrawMenuIcons(b, screenPosition, scale, transparency, layerDepth + 0.0001f, drawStackNumber, color);
                Texture.update(Game1.currentGameTime);

            } else if (ItemCopy is not null) {
                ItemCopy.drawInMenu(b, screenPosition, scale, transparency, layerDepth, drawStackNumber, color, drawShadow);

            }
            DrawMenuIcons(b, screenPosition, scale, transparency, layerDepth + 0.0001f, drawStackNumber, color);


            if (SubTexture is not null) {
                var offsetRectangle = SubTexture.getOffsetRectangle();
                b.Draw(SubTexture.Texture, screenPosition + new Vector2(16f, 16f), offsetRectangle, color * transparency, 0f, new Vector2(offsetRectangle.Width / 2, offsetRectangle.Height / 2), SubItemSpriteScale * scale * 0.5f, SpriteEffects.None, layerDepth + 0.0002f);

                SubTexture.update(Game1.currentGameTime);
            } else if (SubItemCopy is not null)
                SubItemCopy.drawInMenu(b, screenPosition + new Vector2(-16, -16), scale * 0.5f, transparency, layerDepth + 0.0002f, drawStackNumber, color, drawShadow);
        }
        public override string DisplayName => Name ?? ItemCopy?.DisplayName ?? "";
        public override string getDescription() => Description ?? ItemCopy?.getDescription() ?? "";

        public override bool isPlaceable()
            => false;

        public override int maximumStackSize()
            => int.MaxValue;
        protected override Item GetOneNew()
            => throw new NotImplementedException();

        public KeyValuePair<string, Rectangle> getAnimationTexture()
        {
            if (Texture is not null) {
                return new(TextureAsset, new(0, 0, ItemSpriteSize, ItemSpriteSize));

            } else if (ItemCopy is not null) {
                var itemData = ItemRegistry.GetDataOrErrorItem(ItemCopy.QualifiedItemId);
                return new(itemData.GetTextureName(), itemData.GetSourceRect());

            } else {
                var itemData = ItemRegistry.GetDataOrErrorItem(ItemId);
                return new(itemData.GetTextureName(), itemData.GetSourceRect());

            }
        }
    }
}
