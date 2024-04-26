/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Pathfinding;

namespace NermNermNerm.Junimatic
{
    public class JunimoShuffler : NPC, ISimpleLog
    {
        private float alpha = 1f;
        private float alphaChange;
        private readonly NetColor color = new NetColor();
        private bool destroy;
        private readonly NetEvent1Field<int, NetInt> netAnimationEvent = new NetEvent1Field<int, NetInt>();
        private readonly NetRef<Inventory> carrying = new NetRef<Inventory>(new Inventory());
        private readonly WorkFinder? workFinder;

        public JunimoShuffler()
        {
            this.Breather = false;
            this.speed = 3;
            this.forceUpdateTimer = 9999;
            this.ignoreMovementAnimation = true;
            this.farmerPassesThrough = true;
            this.Scale = 0.75f;
            this.willDestroyObjectsUnderfoot = false;
            this.collidesWithOtherCharacters.Value = false;
            this.SimpleNonVillagerNPC = true;

            this.alpha = 0;
            this.alphaChange = 0.05f;
            this.LogTrace("Junimo cloned");
        }

        public JunimoShuffler(JunimoAssignment assignment, WorkFinder workFinder)
            : base(new AnimatedSprite(@"Characters\Junimo", 0, 16, 16), assignment.origin.ToVector2()*64, 2, "Junimo")
        {
            this.color.Value = assignment.projectType switch {
                JunimoType.MiningProcessing => Color.OrangeRed,
                JunimoType.Animals => Color.PapayaWhip,
                JunimoType.Forestry => Color.ForestGreen,
                JunimoType.CropProcessing => Color.Purple,
                _ => UnlockFishing.JunimoColor }; // Fishing
            this.currentLocation = assignment.hut.Location;
            this.Breather = false;
            this.speed = 3;
            this.forceUpdateTimer = 9999;
            this.ignoreMovementAnimation = true;
            this.farmerPassesThrough = true;
            this.Scale = 0.75f;
            this.willDestroyObjectsUnderfoot = false;
            this.collidesWithOtherCharacters.Value = false;
            this.SimpleNonVillagerNPC = true;

            this.Assignment = assignment;
            this.controller = new PathFindController(this, assignment.hut.Location, assignment.source.AccessPoint, 0, this.JunimoReachedSource);
            this.alpha = 0;
            this.alphaChange = 0.05f;
            this.workFinder = workFinder;
            this.LogTrace($"Junimo created {this.Assignment}");
        }

        public JunimoAssignment? Assignment { get; private set; }

        private Inventory Carrying => this.carrying.Value;

        private void JunimoReachedSource(Character c, GameLocation l)
        {
            if (this.Assignment is null)
            {
                return;
            }

            this.LogTrace($"Junimo reached its source {this.Assignment}");

            if (this.Carrying.Count != 0) throw new InvalidOperationException("inventory should be empty here");

            if (this.Assignment.source is GameStorage chest)
            {
                if (this.Assignment.itemsToRemoveFromChest is null) throw new InvalidOperationException("Should have some items to fetch");

                this.Assignment.itemsToRemoveFromChest.Reverse(); // <- tidy

                if (!chest.TryFulfillShoppingList(this.Assignment.itemsToRemoveFromChest, this.Carrying))
                {
                    this.LogTrace($"Assigned chest didn't have needed items");
                    this.JunimoQuitsInDisgust();
                    return;
                }

                l.playSound("pickUpItem"); // Maybe 'openChest' instead?
            }
            else if (this.Assignment.source is GameMachine machine && machine.HeldObject is not null)
            {
                this.Carrying.Add(machine.RemoveHeldObject());
                l.playSound("dwop"); // <- might get overriden by the furnace sound...  but if it's not a furnace...
            }
            else
            {
                this.JunimoQuitsInDisgust();
                return;
            }

            // Head to the target
            this.controller = new PathFindController(this, base.currentLocation, this.Assignment.target.AccessPoint, 0, this.JunimoReachedTarget);
        }

        private void JunimoQuitsInDisgust()
        {
            if (this.Assignment is null)
            {
                return;
            }

            this.LogTrace($"Junimo quits {this.Assignment}");
            foreach (Item item in this.Carrying)
            {
                this.TurnIntoDebris(item);
            }
            this.Carrying.Clear();
            this.doEmote(12);

            this.controller = new PathFindController(this, base.currentLocation, this.Assignment.origin, 0, this.JunimoReachedHut);
        }

        private void TurnIntoDebris(Item item)
        {
            base.currentLocation.debris.Add(new Debris(item, this.Tile*64));
        }

        private void JunimoReachedTarget(Character c, GameLocation l)
        {
            if (this.Assignment is null)
            {
                return;
            }

            this.LogTrace($"Junimo reached target {this.Assignment}");

            if (this.Assignment.target is GameStorage chest)
            {
                l.playSound("Ship");
                // Put what we're carrying into the chest or huck it overboard if we can't.
                if (!chest.TryStore(this.Carrying))
                {
                    this.LogWarning($"Target {chest} did not have room for {this.Carrying[0].Stack} {this.Carrying[0].Name}");
                    this.JunimoQuitsInDisgust();
                    return;
                }
            }
            else
            {
                bool isLoaded = ((GameMachine)this.Assignment.target).FillMachineFromInventory(this.Carrying);
                if (!isLoaded)
                {
                    this.LogTrace($"Junimo could not load {this.Assignment} - perhaps a player loaded it?");
                    this.JunimoQuitsInDisgust();
                    return;
                }

                l.playSound("dwop"); // <- might get overriden by the furnace sound...  but if it's not a furnace...
            }

            var newAssignment = (new WorkFinder()).FindProject(this.Assignment.hut, this.Assignment.projectType, this);
            if (newAssignment is not null)
            {
                this.Assignment = newAssignment;
                this.controller = new PathFindController(this, this.Assignment.hut.Location, this.Assignment.source.AccessPoint, 0, this.JunimoReachedSource);
            }
            else
            {
                this.controller = new PathFindController(this, base.currentLocation, this.Assignment.origin, 0, this.JunimoReachedHut);
            }
        }

        public void JunimoReachedHut(Character c, GameLocation l)
        {
            if (this.Assignment is null)
            {
                return;
            }

            this.LogTrace($"Junimo returned to its hut {this.Assignment}");
            this.controller = null;
            this.destroy = true;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields
                .AddField(this.color, "color")
                .AddField(this.netAnimationEvent, "netAnimationEvent")
                .AddField(this.carrying, "carrying");
            this.netAnimationEvent.onEvent += this.doAnimationEvent;
        }

        protected virtual void doAnimationEvent(int animId)
        {
            switch (animId)
            {
                case 0:
                    this.Sprite.CurrentAnimation = null;
                    break;

                // 2-5 are unused, as best as I can figure.
                case 2:
                    this.Sprite.currentFrame = 0;
                    break;
                case 3:
                    this.Sprite.currentFrame = 1;
                    break;
                case 4:
                    this.Sprite.currentFrame = 2;
                    break;
                case 5:
                    this.Sprite.currentFrame = 44;
                    break;

                // These are set randomly in Update
                case 6:
                    this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                    {
                        new FarmerSprite.AnimationFrame(12, 200),
                        new FarmerSprite.AnimationFrame(13, 200),
                        new FarmerSprite.AnimationFrame(14, 200),
                        new FarmerSprite.AnimationFrame(15, 200)
                    });
                    break;
                case 7:
                    this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                    {
                        new FarmerSprite.AnimationFrame(44, 200),
                        new FarmerSprite.AnimationFrame(45, 200),
                        new FarmerSprite.AnimationFrame(46, 200),
                        new FarmerSprite.AnimationFrame(47, 200)
                    });
                    break;
                case 8:
                    this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
                    {
                        new FarmerSprite.AnimationFrame(28, 100),
                        new FarmerSprite.AnimationFrame(29, 100),
                        new FarmerSprite.AnimationFrame(30, 100),
                        new FarmerSprite.AnimationFrame(31, 100)
                    });
                    break;
                case 1:
                    break;
            }
        }

        public void OnDayEnding(GameLocation location)
        {
            if (this.workFinder is null || this.Assignment is null) // if !mastergame
                return;

            if (this.Carrying.Count > 0)
            {
                this.JunimoReachedTarget(this, location);
            }
            location.characters.Remove(this);
        }

        public override void update(GameTime time, GameLocation location)
        {
            if (Game1.IsMasterGame && this.controller is null && this.workFinder is not null && this.Assignment is not null && !this.destroy)
            {
                this.workFinder.LogTrace("Junimo returned due to players leaving scene");
                if (this.Carrying.Count > 0)
                {
                    this.JunimoReachedTarget(this, location);
                }

                location.characters.Remove(this);
                return;
            }

            this.netAnimationEvent.Poll();
            base.update(time, location);

            this.forceUpdateTimer = 99999;

            if (this.destroy)
            {
                this.alphaChange = -0.05f;
            }

            this.alpha += this.alphaChange;
            if (this.alpha > 1f)
            {
                this.alpha = 1f;
            }
            else if (this.alpha < 0f)
            {
                this.alpha = 0f;
                if (this.destroy)
                {
                    location.characters.Remove(this);
                    return;
                }
            }

            if (Game1.IsMasterGame)
            {
                if (Game1.random.NextDouble() < 0.002)
                {
                    switch (Game1.random.Next(6))
                    {
                        case 0:
                            this.netAnimationEvent.Fire(6);
                            break;
                        case 1:
                            this.netAnimationEvent.Fire(7);
                            break;
                        case 2:
                            this.netAnimationEvent.Fire(0);
                            break;
                        case 3:
                            this.jumpWithoutSound();
                            this.yJumpVelocity /= 2f;
                            this.netAnimationEvent.Fire(0);
                            break;
                        case 5:
                            this.netAnimationEvent.Fire(8);
                            break;
                    }
                }
            }

            this.Sprite.CurrentAnimation = null;
            if (this.moveRight)
            {
                this.flip = false;
                if (this.Sprite.Animate(time, 16, 8, 50f))
                {
                    this.Sprite.currentFrame = 16;
                }
            }
            else if (this.moveLeft)
            {
                if (this.Sprite.Animate(time, 16, 8, 50f))
                {
                    this.Sprite.currentFrame = 16;
                }

                this.flip = true;
            }
            else if (this.moveUp)
            {
                if (this.Sprite.Animate(time, 32, 8, 50f))
                {
                    this.Sprite.currentFrame = 32;
                }
            }
            else if (this.moveDown)
            {
                this.Sprite.Animate(time, 0, 8, 50f);
            }
        }

        private static readonly int[] yBounceBasedOnFrame = new int[] { 12, 10, 8, 6, 4, 4, 8, 10 };
        private static readonly int[] xBounceBasedOnFrame = new int[] { 1, 3, 1, -1, -3, -1, 1, 0  };
        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (this.alpha > 0f)
            {
                float num = (float)base.StandingPixel.Y / 10000f;
                b.Draw(
                    this.Sprite.Texture,
                    this.getLocalPosition(Game1.viewport)
                        + new Vector2(
                            this.Sprite.SpriteWidth * 4 / 2,
                            (float)this.Sprite.SpriteHeight * 3f / 4f * 4f / (float)Math.Pow(this.Sprite.SpriteHeight / 16, 2.0) + (float)this.yJumpOffset - 8f)  // Apparently yJumpOffset is always 0.
                        + ((this.shakeTimer > 0) // Apparently shakeTimer is always 0.
                            ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2))
                            : Vector2.Zero),
                    this.Sprite.SourceRect,
                    this.color.Value * this.alpha, this.rotation,
                    new Vector2(this.Sprite.SpriteWidth * 4 / 2,
                    (float)(this.Sprite.SpriteHeight * 4) * 3f / 4f) / 4f,
                    Math.Max(0.2f, this.Scale) * 4f, this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, this.drawOnTop ? 0.991f : num));
                if (!this.swimming.Value)
                {
                    b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, base.Position + new Vector2((float)(this.Sprite.SpriteWidth * 4) / 2f, 44f)), Game1.shadowTexture.Bounds, this.color.Value * this.alpha, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), (4f + (float)this.yJumpOffset / 40f) * this.Scale, SpriteEffects.None, Math.Max(0f, num) - 1E-06f);
                }

                float xOffset = 0;
                foreach (var carried in this.Carrying)
                {
                    // This makes it vary between 0% and 5% bigger, independent of the animation frame because...  Well, I don't know if it's good or bad.
                    //  It also probably ought to affect bounce, if we were trying for some kind of specific effect, but we aren't, so it doesn't.
                    float scaleFactor = (float)((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.05f);

                    var bounce = new Vector2(xBounceBasedOnFrame[this.Sprite.CurrentFrame & 7], yBounceBasedOnFrame[this.Sprite.CurrentFrame & 7]);
                    var itemOffset = new Vector2(xOffset - 2.5f * this.Carrying.Count, 0);
                    xOffset += 5f;
                    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(carried.QualifiedItemId);
                    b.Draw(
                        dataOrErrorItem.GetTexture(),
                        Game1.GlobalToLocal(Game1.viewport, base.Position + new Vector2(8f, -64f * (float)this.Scale + 4f + (float)this.yJumpOffset) + bounce + itemOffset),
                        dataOrErrorItem.GetSourceRect(0, carried.ParentSheetIndex),
                        Color.White * this.alpha,
                        0f,
                        Vector2.Zero,
                        4f * (float)this.Scale*(1 + scaleFactor),
                        SpriteEffects.None,
                        base.Position.Y / 10000f + 0.0001f);
                }
            }
        }

        public void WriteToLog(string message, LogLevel level, bool isOnceOnly)
        {
            this.workFinder?.WriteToLog(message, level, isOnceOnly);
        }
    }
}
