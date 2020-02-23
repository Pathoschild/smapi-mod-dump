using MegaStorage.Framework.Interface;
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
        public abstract int Capacity { get; }
        public abstract ChestType ChestType { get; }
        public CustomChestConfig Config { get; }
        protected abstract LargeItemGrabMenu CreateItemGrabMenu();

        private readonly Texture2D _sprite;
        private readonly Texture2D _spriteBW;
        private readonly Texture2D _spriteBraces;

        private LargeItemGrabMenu _itemGrabMenu;

        private readonly IReflectedField<int> _currentLidFrameReflected;
        private int CurrentLidFrame
        {
            get => _currentLidFrameReflected.GetValue();
            set => _currentLidFrameReflected.SetValue(value);
        }

        protected CustomChest(int parentSheetIndex, CustomChestConfig config, Vector2 tileLocation) : base(true, tileLocation)
        {
            var contentHelper = MegaStorageMod.Instance.Helper.Content;

            if (config is null)
            {
                MegaStorageMod.Instance.Monitor.Log("Cannot load CustomChest, missing config", LogLevel.Error);
                return;
            }

            Config = config;
            ParentSheetIndex = parentSheetIndex;
            startingLidFrame.Value = parentSheetIndex + 1;
            _currentLidFrameReflected = MegaStorageMod.Instance.Helper.Reflection.GetField<int>(this, "currentLidFrame");

            _sprite = contentHelper.Load<Texture2D>(Path.Combine("assets", Config.SpritePath));
            _spriteBW = contentHelper.Load<Texture2D>(Path.Combine("assets", Config.SpriteBWPath));
            _spriteBraces = contentHelper.Load<Texture2D>(Path.Combine("assets", Config.SpriteBracesPath));
        }

        public override Item addItem(Item itemToAdd)
        {
            if (itemToAdd is null)
            {
                return null;
            }

            itemToAdd.resetState();
            clearNulls();

            foreach (var item in items.Where(item => item != null && item.canStackWith(itemToAdd)))
            {
                itemToAdd.Stack = item.addToStack(itemToAdd);
                if (itemToAdd.Stack <= 0)
                {
                    return null;
                }
            }

            if (items.Count >= Capacity)
            {
                return itemToAdd;
            }

            items.Add(itemToAdd);

            return null;
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            if (time is null)
            {
                return;
            }

            var currentLidFrameValue = CurrentLidFrame;
            fixLidFrame();
            mutex.Update(environment);
            if (shakeTimer > 0)
            {
                shakeTimer -= time.ElapsedGameTime.Milliseconds;
                if (shakeTimer <= 0)
                {
                    health = 10;
                }
            }
            if (frameCounter.Value > -1 && currentLidFrameValue < ParentSheetIndex + 6)
            {
                --frameCounter.Value;
                if (frameCounter.Value > 0 || !mutex.IsLockHeld())
                {
                    return;
                }

                if (currentLidFrameValue == ParentSheetIndex + 5)
                {
                    _itemGrabMenu = CreateItemGrabMenu();
                    Game1.activeClickableMenu = _itemGrabMenu;
                    frameCounter.Value = -1;
                }
                else
                {
                    frameCounter.Value = 5;
                    ++currentLidFrameValue;
                    CurrentLidFrame = currentLidFrameValue;
                }
            }
            else
            {
                if (frameCounter.Value != -1 || currentLidFrameValue <= ParentSheetIndex + 1 || Game1.activeClickableMenu != null || !mutex.IsLockHeld())
                {
                    return;
                }

                mutex.ReleaseLock();
                currentLidFrameValue = ParentSheetIndex + 5;
                CurrentLidFrame = currentLidFrameValue;
                frameCounter.Value = 2;
                environment?.localSound("doorCreakReverse");
            }
        }

        public override void grabItemFromChest(Item item, Farmer who)
        {
            if (who is null || !who.couldInventoryAcceptThisItem(item)) return;

            items.Remove(item);
            clearNulls();
            if (_itemGrabMenu == null)
            {
                _itemGrabMenu = CreateItemGrabMenu();
            }

            Game1.activeClickableMenu = _itemGrabMenu;
        }

        public override void grabItemFromInventory(Item item, Farmer who)
        {
            if (item is null || who is null) return;

            if (item.Stack == 0)
            {
                item.Stack = 1;
            }

            var addedItem = addItem(item);
            if (addedItem == null)
            {
                who.removeItemFromInventory(item);
            }
            else
            {
                addedItem = who.addItemToInventory(addedItem);
            }

            clearNulls();
            var id = Game1.activeClickableMenu.currentlySnappedComponent != null ? Game1.activeClickableMenu.currentlySnappedComponent.myID : -1;
            if (_itemGrabMenu == null)
            {
                _itemGrabMenu = CreateItemGrabMenu();
            }

            _itemGrabMenu.heldItem = addedItem;
            Game1.activeClickableMenu = _itemGrabMenu;
            if (id == -1) return;

            Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(id);
            Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            if (location is null)
            {
                return false;
            }

            var objectKey = new Vector2(x / 64, y / 64);
            health = 10;
            owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;
            if (location.objects.ContainsKey(objectKey) || location is MineShaft)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                return false;
            }
            shakeTimer = 50;
            var newCustomChest = CustomChestFactory.Create(ChestType);
            location.objects.Add(objectKey, newCustomChest);
            location.playSound("axe");
            return true;
        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t?.getLastFarmerToUse() != null && t.getLastFarmerToUse() != Game1.player)
            {
                return false;
            }

            if (t == null || t is MeleeWeapon || !t.isHeavyHitter())
            {
                return false;
            }

            var player = t.getLastFarmerToUse();
            if (player == null)
            {
                return false;
            }

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

        private Debris CreateDebris(Farmer player)
        {
            var position = new Vector2(player.GetBoundingBox().Center.X, player.GetBoundingBox().Center.Y);
            return new Debris(-ParentSheetIndex, player.GetToolLocation(), position)
            {
                item = this
            };
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            if (spriteBatch is null)
            {
                return;
            }

            var lidFrameIndex = CurrentLidFrame - ParentSheetIndex - 1;
            if (playerChoiceColor.Value.Equals(Color.Black))
            {
                spriteBatch.Draw(_sprite,
                    Game1.GlobalToLocal(Game1.viewport,
                        new Vector2(x * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (y - 1) * 64)),
                    Game1.getSourceRectForStandardTileSheet(_sprite, 0, 16, 32), tint.Value * alpha, 0.0f, Vector2.Zero,
                    4f, SpriteEffects.None, (y * 64 + 4) / 10000f);
                spriteBatch.Draw(_sprite,
                    Game1.GlobalToLocal(Game1.viewport,
                        new Vector2(x * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (y - 1) * 64)),
                    Game1.getSourceRectForStandardTileSheet(_sprite, lidFrameIndex, 16, 32), tint.Value * alpha * alpha,
                    0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 5) / 10000f);
            }
            else
            {
                spriteBatch.Draw(_spriteBW,
                    Game1.GlobalToLocal(Game1.viewport,
                        new Vector2(x * 64, (y - 1) * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))),
                    Game1.getSourceRectForStandardTileSheet(_spriteBW, 0, 16, 32), playerChoiceColor.Value * alpha,
                    0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 4) / 10000f);
                spriteBatch.Draw(_spriteBW,
                    Game1.GlobalToLocal(Game1.viewport,
                        new Vector2(x * 64, (y - 1) * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))),
                    Game1.getSourceRectForStandardTileSheet(_spriteBW, lidFrameIndex, 16, 32),
                    playerChoiceColor.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
                    (y * 64 + 5) / 10000f);
                spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
                    Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 + 20)),
                    new Rectangle(0, 725, 16, 11), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None,
                    (y * 64 + 6) / 10000f);
                spriteBatch.Draw(_spriteBraces,
                    Game1.GlobalToLocal(Game1.viewport,
                        new Vector2(x * 64, (y - 1) * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))),
                    Game1.getSourceRectForStandardTileSheet(_spriteBraces, lidFrameIndex, 16, 32), Color.White * alpha,
                    0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 6) / 10000f);
            }
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (spriteBatch is null)
            {
                return;
            }

            if (playerChoiceColor.Value.Equals(Color.Black))
            {
                spriteBatch.Draw(_sprite, location + new Vector2(32f, 32f), Game1.getSourceRectForStandardTileSheet(_sprite, 0, 16, 32), color * transparency, 0.0f, new Vector2(8f, 16f), (float)(4.0 * (scaleSize < 0.2 ? scaleSize : scaleSize / 2.0)), SpriteEffects.None, layerDepth);
            }
            else
            {
                spriteBatch.Draw(_spriteBW, location + new Vector2(32f, 32f), Game1.getSourceRectForStandardTileSheet(_spriteBW, 0, 16, 32), playerChoiceColor.Value * transparency, 0.0f, new Vector2(8f, 16f), (float)(4.0 * (scaleSize < 0.2 ? scaleSize : scaleSize / 2.0)), SpriteEffects.None, layerDepth);
                spriteBatch.Draw(_spriteBraces, location + new Vector2(32f, 32f), Game1.getSourceRectForStandardTileSheet(_spriteBraces, 0, 16, 32), color * transparency, 0.0f, new Vector2(8f, 16f), (float)(4.0 * (scaleSize < 0.2 ? scaleSize : scaleSize / 2.0)), SpriteEffects.None, layerDepth);
            }
            if (drawStackNumber == StackDrawType.Draw && maximumStackSize() > 1 && (scaleSize > 0.3 && Stack != int.MaxValue) && Stack > 1)
            {
                Utility.drawTinyDigits(Stack, spriteBatch, location + new Vector2(64 - Utility.getWidthOfTinyDigitString(Stack, 3f * scaleSize) + 3f * scaleSize, (float)(64.0 - 18.0 * scaleSize + 2.0)), 3f * scaleSize, 1f, color);
            }
        }

        public LargeItemGrabMenu GetItemGrabMenu()
        {
            MegaStorageMod.ModMonitor.Log("GetItemGrabMenu");
            return _itemGrabMenu ?? (_itemGrabMenu = CreateItemGrabMenu());
        }
    }
}
