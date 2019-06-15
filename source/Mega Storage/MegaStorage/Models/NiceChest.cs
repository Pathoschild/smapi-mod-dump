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
    public abstract class NiceChest : Chest
    {
        public abstract string ItemName { get; }
        public abstract string Description { get; }
        public abstract int Capacity { get; }
        public abstract int ItemId { get; }
        public abstract ChestType ChestType { get; }
        public abstract string SpritePath { get; }
        public abstract string RecipeString { get; }
        public abstract string BigCraftableInfo { get; }
        protected abstract LargeItemGrabMenu CreateItemGrabMenu();

        private readonly IReflectedField<int> _currentLidFrameReflected;
        private int CurrentLidFrame
        {
            get => _currentLidFrameReflected.GetValue();
            set => _currentLidFrameReflected.SetValue(value);
        }

        private LargeItemGrabMenu _largeItemGrabMenu;

        protected NiceChest() : base(true)
        {
            ParentSheetIndex = ItemId;
            _currentLidFrameReflected = MegaStorageMod.Reflection.GetField<int>(this, "currentLidFrame");
            startingLidFrame.Value = ItemId + 1;
            name = ItemName;
        }

        public override string getDescription() => Description;

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

        public override void grabItemFromChest(Item item, Farmer who)
        {
            if (!who.couldInventoryAcceptThisItem(item))
                return;
            items.Remove(item);
            clearNulls();
            _largeItemGrabMenu.Refresh();
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
                    _largeItemGrabMenu = CreateItemGrabMenu();
                    Game1.activeClickableMenu = _largeItemGrabMenu;
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
            _largeItemGrabMenu.Refresh();
            _largeItemGrabMenu.heldItem = addedItem;
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
            location.objects.Add(objectKey, this);
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

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            var drawColor = playerChoiceColor.Value == Color.Black ? Color.White : playerChoiceColor.Value;
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, drawColor, drawShadow);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            var drawColor = playerChoiceColor.Value == Color.Black ? Color.White : playerChoiceColor.Value;
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))), Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, ParentSheetIndex, 16, 32), drawColor * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 4) / 10000f);
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 + 20)), new Rectangle(0, 725, 16, 11), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 6) / 10000f);
            //spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))), Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, 176 + (CurrentLidFrame - ParentSheetIndex), 16, 32), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 6) / 10000f);
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))), Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, CurrentLidFrame, 16, 32), drawColor * alpha * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (y * 64 + 5) / 10000f);
        }

    }
}
