using MegaStorage.Framework.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.IO;
using System.Linq;

namespace MegaStorage.Framework.Models
{
    public abstract class CustomChest : Chest
    {
        private readonly ChestType _chestType;

        // Custom Chest Features
        public abstract int Capacity { get; }
        public bool EnableCategories { get; }
        public bool EnableRemoteStorage { get; protected set; }

        // State
        private readonly IReflectedField<int> _currentLidFrameReflected;
        private int CurrentLidFrame
        {
            get => _currentLidFrameReflected.GetValue();
            set => _currentLidFrameReflected.SetValue(value);
        }

        protected internal CustomItemGrabMenu CreateItemGrabMenu() => new CustomItemGrabMenu(this);
        protected CustomChest(ChestType chestType, Vector2 tileLocation) : base(true, tileLocation)
        {
            _chestType = chestType;

            var config = chestType switch
            {
                ChestType.LargeChest => ModConfig.Instance.LargeChest,
                ChestType.MagicChest => ModConfig.Instance.MagicChest,
                ChestType.SuperMagicChest => ModConfig.Instance.SuperMagicChest,
                _ => throw new InvalidOperationException("Invalid Chest Type")
            };

            EnableCategories = config.EnableCategories;
            ParentSheetIndex = CustomChestFactory.CustomChestIds[_chestType];
            startingLidFrame.Value = ParentSheetIndex + 1;
            _currentLidFrameReflected = MegaStorageMod.Instance.Helper.Reflection.GetField<int>(this, "currentLidFrame");
        }

        public override Item addItem(Item itemToAdd)
        {
            if (itemToAdd is null)
                return null;

            itemToAdd.resetState();
            clearNulls();

            // Find Stackable slot
            foreach (var item in items.Where(item => !(item is null) && item.canStackWith(itemToAdd)))
            {
                itemToAdd.Stack = item.addToStack(itemToAdd);
                if (itemToAdd.Stack <= 0)
                    return null;
            }

            if (items.Count >= Capacity)
                return itemToAdd;

            items.Add(itemToAdd);

            return null;
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            var currentLidFrameValue = CurrentLidFrame;
            fixLidFrame();
            mutex.Update(environment);
            if (shakeTimer > 0 && time != null)
            {
                shakeTimer -= time.ElapsedGameTime.Milliseconds;
                if (shakeTimer <= 0)
                {
                    health = 10;
                }
            }
            if (frameCounter.Value > -1 && currentLidFrameValue < getLastLidFrame() + 1)
            {
                --frameCounter.Value;
                if (frameCounter.Value > 0 || !mutex.IsLockHeld())
                    return;
                if (currentLidFrameValue == getLastLidFrame())
                {
                    Game1.activeClickableMenu = CreateItemGrabMenu();
                    frameCounter.Value = -1;
                }
                else
                {
                    frameCounter.Value = 5;
                    ++currentLidFrameValue;
                }
            }
            else
            {
                if (frameCounter.Value != -1 || currentLidFrameValue <= startingLidFrame.Value || Game1.activeClickableMenu != null || !mutex.IsLockHeld())
                    return;
                mutex.ReleaseLock();
                currentLidFrameValue = getLastLidFrame();
                frameCounter.Value = 2;
                environment?.localSound("doorCreakReverse");
            }
            CurrentLidFrame = currentLidFrameValue;
        }

        public override void grabItemFromChest(Item item, Farmer who)
        {
            if (who is null || !who.couldInventoryAcceptThisItem(item))
                return;

            items.Remove(item);
            clearNulls();
            //Game1.activeClickableMenu = _itemGrabMenu ??= CreateItemGrabMenu();
        }

        public override void grabItemFromInventory(Item item, Farmer who)
        {
            if (item is null || who is null)
                return;

            if (item.Stack == 0)
                item.Stack = 1;

            var addedItem = addItem(item);
            if (addedItem is null)
                who.removeItemFromInventory(item);
            else
                addedItem = who.addItemToInventory(addedItem);

            clearNulls();

            if (MegaStorageMod.ActiveItemGrabMenu is null)
                Game1.activeClickableMenu = CreateItemGrabMenu();
            MegaStorageMod.ActiveItemGrabMenu.heldItem = addedItem;

            var id = !(Game1.activeClickableMenu.currentlySnappedComponent is null)
                ? Game1.activeClickableMenu.currentlySnappedComponent.myID : -1;
            if (id == -1)
                return;

            Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(id);
            Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            if (location is null)
                return false;

            var objectKey = new Vector2(x / 64f, y / 64f);
            health = 10;
            owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;
            if (location.objects.ContainsKey(objectKey) || location is MineShaft)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                return false;
            }
            shakeTimer = 50;
            var newCustomChest = CustomChestFactory.Create(_chestType, objectKey);
            location.objects.Add(objectKey, newCustomChest);
            location.playSound("axe");
            return true;
        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t?.getLastFarmerToUse() != null && t.getLastFarmerToUse() != Game1.player)
                return false;

            if (t == null || t is MeleeWeapon || !t.isHeavyHitter())
                return false;

            var player = t.getLastFarmerToUse();
            if (player == null)
                return false;

            var c = player.GetToolLocation() / 64f;
            c.X = (int)c.X;
            c.Y = (int)c.Y;
            mutex.RequestLock(() =>
            {
                clearNulls();
                if (items.Count == 0)
                {
                    performRemoveAction(tileLocation.Value, location);
                    if (location.Objects.Remove(c)
                        && type.Value.Equals("Crafting", StringComparison.InvariantCultureIgnoreCase)
                        && Fragility != 2)
                    {
                        location.debris.Add(CreateDebris(player));
                    }
                }
                else if (t.isHeavyHitter() && !(t is MeleeWeapon))
                {
                    location.playSound("hammer");
                    shakeTimer = 100;
                }
                mutex.ReleaseLock();
            });
            return false;
        }

        private Debris CreateDebris(Farmer player) =>
            new Debris(-ParentSheetIndex,
                player.GetToolLocation(),
                new Vector2(player.GetBoundingBox()
                        .Center.X,
                    player.GetBoundingBox()
                        .Center.Y))
            {
                item = this
            };

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            var layerDepth = Math.Max(0.0f, ((y + 1f) * Game1.tileSize - 24f) / 10000f) + x * 1E-05f;
            var globalPosition = new Vector2(x * Game1.tileSize, (y - 1) * Game1.tileSize);
            if (playerChoiceColor.Value.Equals(Color.Black))
            {
                // Draw Chest
                spriteBatch?.Draw(
                    Game1.bigCraftableSpriteSheet,
                    Game1.GlobalToLocal(Game1.viewport, globalPosition + ShakeOffset(-1, 2)),
                    Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, ParentSheetIndex, 16, 32),
                    tint.Value * alpha,
                    0.0f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    layerDepth);
                
                // Draw Lid
                spriteBatch?.Draw(
                    Game1.bigCraftableSpriteSheet,
                    Game1.GlobalToLocal(Game1.viewport, globalPosition + ShakeOffset(-1, 2)),
                    Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, CurrentLidFrame, 16, 32),
                    tint.Value * alpha * alpha,
                    0.0f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    layerDepth + 1E-05f);
            }
            else
            {
                var spriteBraceBottom = Game1.getSourceRectForStandardTileSheet(
                    Game1.bigCraftableSpriteSheet,
                    ParentSheetIndex + 12,
                    16,
                    32);
                spriteBraceBottom.Y += 21;
                spriteBraceBottom.Height = 11;
                
                // Draw Colorized Chest
                spriteBatch?.Draw(
                    Game1.bigCraftableSpriteSheet,
                    Game1.GlobalToLocal(Game1.viewport, globalPosition + ShakeOffset(-1, 2)),
                    Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, ParentSheetIndex + 6, 16, 32),
                    playerChoiceColor.Value * alpha,
                    0.0f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    layerDepth);
                
                // Draw Bottom-Half Braces
                spriteBatch?.Draw(
                    Game1.bigCraftableSpriteSheet,
                    Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(0,Game1.tileSize + 20)),
                    spriteBraceBottom,
                    Color.White * alpha,
                    0.0f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    layerDepth + 2E-05f);
                
                // Draw Top-Half Braces
                spriteBatch?.Draw(
                    Game1.bigCraftableSpriteSheet,
                    Game1.GlobalToLocal(Game1.viewport, globalPosition + ShakeOffset(-1, 2)),
                    Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, CurrentLidFrame + 12, 16, 32),
                    Color.White * alpha,
                    0.0f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    layerDepth + 2E-05f);
                
                // Draw Colorized Lid
                spriteBatch?.Draw(
                    Game1.bigCraftableSpriteSheet,
                    Game1.GlobalToLocal(Game1.viewport, globalPosition + ShakeOffset(-1, 2)),
                    Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, CurrentLidFrame + 6, 16, 32),
                    playerChoiceColor.Value * alpha * alpha,
                    0.0f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    layerDepth + 1E-05f);
            }
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (playerChoiceColor.Value.Equals(Color.Black))
            {
                // Draw Chest
                spriteBatch?.Draw(
                    Game1.bigCraftableSpriteSheet,
                    location + new Vector2(32f, 32f),
                    Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, ParentSheetIndex, 16, 32),
                    color * transparency,
                    0.0f,
                    new Vector2(8f, 16f), 
                    4f * (scaleSize < 0.2f ? scaleSize : scaleSize / 2f),
                    SpriteEffects.None,
                    layerDepth);
            }
            else
            {
                // Draw Colorized Chest
                spriteBatch?.Draw(
                    Game1.bigCraftableSpriteSheet,
                    location + new Vector2(32f, 32f),
                    Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, ParentSheetIndex + 6, 16, 32),
                    playerChoiceColor.Value * transparency,
                    0.0f,
                    new Vector2(8f, 16f),
                    4f * (scaleSize < 0.2f ? scaleSize : scaleSize / 2f),
                    SpriteEffects.None,
                    layerDepth);

                // Draw Braces
                spriteBatch?.Draw(
                    Game1.bigCraftableSpriteSheet,
                    location + new Vector2(32f, 32f),
                    Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, ParentSheetIndex + 12, 16, 32),
                    color * transparency,
                    0.0f,
                    new Vector2(8f, 16f),
                    4f * (scaleSize < 0.2f ? scaleSize : scaleSize / 2f),
                    SpriteEffects.None,
                    layerDepth);
            }
            if (drawStackNumber == StackDrawType.Draw && maximumStackSize() > 1 && (scaleSize > 0.3 && Stack != int.MaxValue) && Stack > 1)
            {
                Utility.drawTinyDigits(Stack, spriteBatch, location + new Vector2(64 - Utility.getWidthOfTinyDigitString(Stack, 3f * scaleSize) + 3f * scaleSize, (float)(64.0 - 18.0 * scaleSize + 2.0)), 3f * scaleSize, 1f, color);
            }
        }

        private Vector2 ShakeOffset(int minValue, int maxValue) =>
            shakeTimer > 0
                ? new Vector2(Game1.random.Next(minValue, maxValue), 0)
                : Vector2.Zero;
    }
}
