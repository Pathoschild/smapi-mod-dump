using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicApi2.Contracts;
using Igorious.StardewValley.DynamicApi2.Data;
using Igorious.StardewValley.DynamicApi2.Extensions;
using Igorious.StardewValley.DynamicApi2.Services;
using Igorious.StardewValley.ShowcaseMod.Constants;
using Igorious.StardewValley.ShowcaseMod.Data;
using Igorious.StardewValley.ShowcaseMod.ModConfig;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace Igorious.StardewValley.ShowcaseMod.Core
{
    public partial class Showcase : Furniture, IInitializable
    {
        private ShowcaseConfig Config { get; }
        private GlowConfig GlowConfig { get; }
        private ItemFilter Filter { get; }
        private ItemGridProvider ItemProvider { get; set; }
        private Texture2D LocalFurnitureTexture { get; }
        private Texture2D GlowTexture { get; }
        private IReadOnlyCollection<RotationEffect> RotationEffects { get; }

        public Showcase(int id) : base(id, Vector2.Zero)
        {
            Config = ShowcaseMod.GetShowcaseConfig(ParentSheetIndex);
            GlowConfig = ShowcaseMod.Config.Glows;
            Filter = new ItemFilter(Config.Filter);
            LocalFurnitureTexture = ShowcaseMod.TextureModule.GetTexture(TextureNames.Furniture);
            GlowTexture = ShowcaseMod.TextureModule.GetTexture(TextureNames.Glow);
            RotationEffects = ShowcaseMod.Config.RotationEffects;
        }

        public override bool canBePlacedHere(GameLocation location, Vector2 tile)
        {
            var result = base.canBePlacedHere(location, tile);
            if (!result || getTilesWide() != 1) return result;

            var decoratableLocation = (DecoratableLocation)location;
            for (var ty = 0; ty < getTilesHigh(); ++ty)
            {
                var position = (tile + new Vector2(0.5f, ty + 0.5f)) * Game1.tileSize;
                foreach (var furniture in decoratableLocation.furniture)
                {
                    if (furniture.furniture_type == 11 && furniture.getBoundingBox(furniture.tileLocation).Contains((int)position.X, (int)position.Y) && furniture.heldObject == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool clicked(Farmer who)
        {
            var chest = heldObject;
            heldObject = null;
            var result = base.clicked(who);
            heldObject = chest;
            return result;
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (justCheckingForActivity) return true;

            ItemProvider.Recalculate();
            var rows = ItemProvider.Rows;
            var cellsCount = (ItemProvider.Count + rows - 1) / rows * rows;
            var allowColoring = (Config.Tint != null || Config.SecondTint != null) && !Config.AutoTint;
            Game1.activeClickableMenu = new ShowcaseContainer(this, ItemProvider.GetInternalList(), cellsCount, rows, Filter.IsPass, allowColoring);
            return true;
        }

        public override Item getOne()
        {
            var thisCopy = new Showcase(parentSheetIndex);
            Cloner.Instance.CopyData(base.getOne(), thisCopy);
            thisCopy.Initialize();
            thisCopy.ItemProvider.AddRange(ItemProvider);
            return thisCopy;
        }

        public override bool performObjectDropInAction(Object dropIn, bool probe, Farmer who)
        {
            if (!Filter.IsPass(dropIn)) return false;
            ItemProvider.Recalculate();
            if (!ItemProvider.Contains(null)) return false;
            if (probe) return true;

            Game1.playSound("woodyStep");
            who.reduceActiveItemByOne();
            ItemProvider.Add(dropIn.getOne());
            return true;
        }

        private Texture2D GetTexture(SpriteInfo spriteInfo)
        {
            return (spriteInfo.Kind == TextureKind.Local)? LocalFurnitureTexture : furnitureTexture;
        }

        private Rectangle GetDefaultSourceRect(SpriteInfo spriteInfo, int width, int heigth)
        {
            return TextureInfo.Furnitures.GetSourceRect(GetTexture(spriteInfo), spriteInfo.Index, width / spriteSheetTileSize, heigth / spriteSheetTileSize);
        }

        public void Initialize()
        {
            name = Config.Name;
            description = Config.Description ?? description;
            defaultSourceRect = sourceRect = GetDefaultSourceRect(Config.Sprite, defaultSourceRect.Width, defaultSourceRect.Height);
            for (var i = 0; i < currentRotation; ++i) rotate();

            var heldChest = heldObject as Chest;
            if (heldChest == null)
            {
                heldChest = new Chest(true);
                heldChest.items.AddRange(Enumerable.Repeat<Item>(null, Config.Layout.Rows * Config.Layout.Columns));
                heldChest.items[0] = heldObject;
                heldObject = heldChest;
            }
            ItemProvider = new ItemGridProvider(heldChest.items, Config.Layout.Rows, Config.Layout.Columns, currentRotation);
        }
    }
}