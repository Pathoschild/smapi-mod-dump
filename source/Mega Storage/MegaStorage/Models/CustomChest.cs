using MegaStorage.Mapping;
using MegaStorage.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

namespace MegaStorage.Models
{
    public abstract class CustomChest : Chest
    {
        public abstract int Capacity { get; }
        public abstract ChestType ChestType { get; }
        public abstract LargeItemGrabMenu CreateItemGrabMenu();

        public CustomChestConfig Config { get; }
        public string BigCraftableInfo => $"{Config.Name}/0/-300/Crafting -9/{Config.Description}/true/true/0";
        public string RecipeString => $"{Config.Recipe}/Home/{Config.Id}/true/{Config.Name}";

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

        protected CustomChest(CustomChestConfig config) : base(true)
        {
            Config = config;
            ParentSheetIndex = config.Id;
            _currentLidFrameReflected = MegaStorageMod.Reflection.GetField<int>(this, "currentLidFrame");
            startingLidFrame.Value = config.Id + 1;
            name = config.Name;
            _sprite = MegaStorageMod.ModHelper.Content.Load<Texture2D>(config.SpritePath);
            _spriteBW = MegaStorageMod.ModHelper.Content.Load<Texture2D>(config.SpriteBWPath);
            _spriteBraces = MegaStorageMod.ModHelper.Content.Load<Texture2D>(config.SpriteBracesPath);
        }

        public override string getDescription() => Config.Description;

        public override Item addItem(Item itemToAdd)
        {
            itemToAdd.resetState();
            clearNulls();
            foreach (var item in items)
            {
                if (item == null || !item.canStackWith(itemToAdd))
                    continue;
                itemToAdd.Stack = item.addToStack(itemToAdd.Stack);
                if (itemToAdd.Stack <= 0)
                    return null;
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
            var currentLidFrameValue = CurrentLidFrame;
            fixLidFrame();
            mutex.Update(environment);
            if (shakeTimer > 0)
            {
                shakeTimer -= time.ElapsedGameTime.Milliseconds;
                if (shakeTimer <= 0)
                    health = 10;
            }
            if (frameCounter.Value > -1 && currentLidFrameValue < ParentSheetIndex + 6)
            {
                --frameCounter.Value;
                if (frameCounter.Value > 0 || !mutex.IsLockHeld())
                    return;
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
                    return;
                mutex.ReleaseLock();
                currentLidFrameValue = ParentSheetIndex + 5;
                CurrentLidFrame = currentLidFrameValue;
                frameCounter.Value = 2;
                environment.localSound("doorCreakReverse");
            }
        }

        public override void grabItemFromChest(Item item, Farmer who)
        {
            if (!who.couldInventoryAcceptThisItem(item))
                return;
            items.Remove(item);
            clearNulls();
            if (_itemGrabMenu == null)
                _itemGrabMenu = CreateItemGrabMenu();
            _itemGrabMenu.Refresh();
            Game1.activeClickableMenu = _itemGrabMenu;
        }

        public override void grabItemFromInventory(Item item, Farmer who)
        {
            if (item.Stack == 0)
                item.Stack = 1;
            var addedItem = addItem(item);
            if (addedItem == null)
                who.removeItemFromInventory(item);
            else
                addedItem = who.addItemToInventory(addedItem);
            clearNulls();
            var id = Game1.activeClickableMenu.currentlySnappedComponent != null ? Game1.activeClickableMenu.currentlySnappedComponent.myID : -1;
            if (_itemGrabMenu == null)
                _itemGrabMenu = CreateItemGrabMenu();
            _itemGrabMenu.Refresh();
            _itemGrabMenu.heldItem = addedItem;
            Game1.activeClickableMenu = _itemGrabMenu;
            if (id == -1)
                return;
            Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(id);
            Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
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
                    if (location.Objects.Remove(c) && type.Value.Equals("Crafting") && Fragility != 2)
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
            var lidFrameIndex = CurrentLidFrame - ParentSheetIndex - 1;
            if (playerChoiceColor.Value.Equals(Color.Black))
            {
                spriteBatch.Draw(_sprite, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (y - 1) * 64)), Game1.getSourceRectForStandardTileSheet(_sprite, 0, 16, 32), tint.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 4) / 10000f);
                spriteBatch.Draw(_sprite, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (y - 1) * 64)), Game1.getSourceRectForStandardTileSheet(_sprite, lidFrameIndex, 16, 32), tint.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 5) / 10000f);
            }
            else
            {
                spriteBatch.Draw(_spriteBW, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))), Game1.getSourceRectForStandardTileSheet(_spriteBW, 0, 16, 32), playerChoiceColor.Value * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 4) / 10000f);
                spriteBatch.Draw(_spriteBW, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))), Game1.getSourceRectForStandardTileSheet(_spriteBW, lidFrameIndex, 16, 32), playerChoiceColor.Value * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 5) / 10000f);
                spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 + 20)), new Rectangle(0, 725, 16, 11), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 6) / 10000f);
                spriteBatch.Draw(_spriteBraces, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))), Game1.getSourceRectForStandardTileSheet(_spriteBraces, lidFrameIndex, 16, 32), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 6) / 10000f);
            }
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            if (playerChoiceColor.Value.Equals(Color.Black))
            {
                spriteBatch.Draw(_sprite, location + new Vector2(32f, 32f), Game1.getSourceRectForStandardTileSheet(_sprite, 0, 16, 32), color * transparency, 0.0f, new Vector2(8f, 16f), (float)(4.0 * (scaleSize < 0.2 ? scaleSize : scaleSize / 2.0)), SpriteEffects.None, layerDepth);
            }
            else
            {
                spriteBatch.Draw(_spriteBW, location + new Vector2(32f, 32f), Game1.getSourceRectForStandardTileSheet(_spriteBW, 0, 16, 32), playerChoiceColor.Value * transparency, 0.0f, new Vector2(8f, 16f), (float)(4.0 * (scaleSize < 0.2 ? scaleSize : scaleSize / 2.0)), SpriteEffects.None, layerDepth);
                spriteBatch.Draw(_spriteBraces, location + new Vector2(32f, 32f), Game1.getSourceRectForStandardTileSheet(_spriteBraces, 0, 16, 32), color * transparency, 0.0f, new Vector2(8f, 16f), (float)(4.0 * (scaleSize < 0.2 ? scaleSize : scaleSize / 2.0)), SpriteEffects.None, layerDepth);
            }
            if (drawStackNumber && maximumStackSize() > 1 && (scaleSize > 0.3 && Stack != int.MaxValue) && Stack > 1)
            {
                Utility.drawTinyDigits(Stack, spriteBatch, location + new Vector2(64 - Utility.getWidthOfTinyDigitString(Stack, 3f * scaleSize) + 3f * scaleSize, (float)(64.0 - 18.0 * scaleSize + 2.0)), 3f * scaleSize, 1f, color);
            }
        }

    }
}
