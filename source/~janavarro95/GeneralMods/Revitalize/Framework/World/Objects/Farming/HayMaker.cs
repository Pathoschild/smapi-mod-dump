/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Machines;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World.Objects.Farming
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.Farming.HayMaker")]
    public class HayMaker : Machine
    {
        public NetEnum<Enums.SDVObject> feedType = new NetEnum<Enums.SDVObject>(Enums.SDVObject.NULL);

        public const string AmaranthAnimation = "Amaranth";
        public const string CornAnimation = "Corn";
        public const string HayAnimation = "Hay";
        public const string FiberAnimation = "Fiber";
        public const string WheatAnimation = "Wheat";

        public NetBool isUsedForBuyingHayAtAnyTime = new NetBool();

        public HayMaker()
        {

        }

        public HayMaker(BasicItemInformation info, bool isUsedForBuyingHayAtAnyTime = false) : base(info)
        {
            this.isUsedForBuyingHayAtAnyTime.Value = isUsedForBuyingHayAtAnyTime;
            if (this.isUsedForBuyingHayAtAnyTime.Value == true && RevitalizeModCore.Configs.shopsConfigManager.hayMakerShopConfig.IsHayMakerShopUpAgainstAWall)
                this.basicItemInformation.boundingBoxTileDimensions.Value = new Vector2(1, 1);
            if (this.isUsedForBuyingHayAtAnyTime.Value == true)
                this.AnimationManager.playAnimation(HayAnimation);

        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddField(this.feedType);
            this.NetFields.AddField(this.isUsedForBuyingHayAtAnyTime);
        }

        /// <summary>
        /// What happens when a tool is used on this machine.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (this.isUsedForBuyingHayAtAnyTime.Value == true)
                return false;

            return base.performToolAction(t, location);
        }


        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            if (this.exceptionForPlacementIsValidForMarniesRanch(l, tile) == true) return true;
            return base.canBePlacedHere(l, tile);
        }

        /// <summary>
        /// The collision box for this machine.
        /// </summary>
        /// <param name="tileLocation"></param>
        /// <returns></returns>
        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            Rectangle rect = base.getBoundingBox(tileLocation);

            if (this.isUsedForBuyingHayAtAnyTime && RevitalizeModCore.Configs.shopsConfigManager.hayMakerShopConfig.IsHayMakerShopUpAgainstAWall == true)
                rect.Y += Game1.tileSize;
            return rect;
        }

        /// <summary>
        /// Checks to see if this is special handling fopr the machine outside of marnies ranch.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
        protected virtual bool exceptionForPlacementIsValidForMarniesRanch(GameLocation location, Vector2 tile)
        {
            if (this.isUsedForBuyingHayAtAnyTime.Value == true && tile.Equals(RevitalizeModCore.Configs.shopsConfigManager.hayMakerShopConfig.HayMakerTileLocation) && location.Name.Equals("Forest"))
                return true;
            return false;
        }

        public override bool canBeRemoved(Farmer who)
        {
            if (this.isUsedForBuyingHayAtAnyTime.Value == true)
                return false;

            return base.canBeRemoved(who);
        }

        public override bool rightClicked(Farmer who)
        {

            if (this.isUsedForBuyingHayAtAnyTime.Value == true)
            {
                if (Game1.activeClickableMenu == null)
                {

                    ShopMenu shopMenu = new ShopMenu(new Dictionary<ISalable, int[]>()
                    {
                        {RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.Hay,-1), new int[]{RevitalizeModCore.Configs.shopsConfigManager.hayMakerShopConfig.HayMakerShopHaySellPrice,-1 } }
                    });

                    //Load the shop tetx file and select a random dialogue text from it.
                    Dictionary<string, string> shopDialogue = JsonUtilities.LoadStringDictionaryFile(Path.Combine(StringsPaths.ShopDialogue, "HayMakerShopDialogue.json"));
                    int random = Game1.random.Next(0, shopDialogue.Count);
                    shopMenu.potraitPersonDialogue = shopDialogue.ElementAt(random).Value;

                    Game1.activeClickableMenu = shopMenu;
                }

                return true;
            }

            if (this.heldObject.Value != null)
                if (who.IsLocalPlayer)
                    this.cleanOutHayMaker(true);

            return base.rightClicked(who);
        }

        /// <summary>
        /// Cleans out the hay maker to produce more hay.
        /// </summary>
        /// <param name="addToPlayersInventory"></param>
        protected virtual void cleanOutHayMaker(bool addToPlayersInventory)
        {
            if (addToPlayersInventory)
            {
                SoundUtilities.PlaySound(Enums.StardewSound.coin);
                bool added = Game1.player.addItemToInventoryBool(this.heldObject.Value);
                if (added == false) return;
            }
            this.heldObject.Value = null;
            this.AnimationManager.playDefaultAnimation();
            this.feedType.Value = Enums.SDVObject.NULL;

        }

        /// <summary>
        /// Called when a new day is started. Attempt to refill the silos from the hay maker.
        /// </summary>
        /// <param name="location"></param>
        public override void doActualDayUpdateLogic(GameLocation location)
        {
            if (this.heldObject.Value != null)
            {
                this.attemptToFillFarmSilos();
                this.cleanOutHayMaker(false);
            }
        }

        /// <summary>
        /// Performed when dropping in an object into this feeder.
        /// </summary>
        /// <param name="dropInItem"></param>
        /// <param name="probe"></param>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            if (probe == true) return false; //Just checking for action.
            if (who.ActiveObject == null) return false;
            if (dropInItem == null) return false;
            if (this.MinutesUntilReady > 0) return false;
            if (this.isUsedForBuyingHayAtAnyTime.Value == true) return false;
            if (this.heldObject.Value != null)
                this.cleanOutHayMaker(true);

            bool itemAcceptedForHayProduction = this.insertItemForHayProduction(dropInItem, who);
            if (itemAcceptedForHayProduction) return itemAcceptedForHayProduction;


            return base.performObjectDropInAction(dropInItem, probe, who);
        }

        /// <summary>
        /// Handles all of the item conditions for accepting items to process hay.
        /// </summary>
        /// <param name="dropInItem">The item used for hay production</param>
        /// <param name="who">The <see cref="Farmer"/> who drops in the item.</param>
        /// <returns></returns>
        protected virtual bool insertItemForHayProduction(Item dropInItem, Farmer who)
        {
            if (dropInItem.parentSheetIndex == (int)Enums.SDVObject.Corn && who.ActiveObject.Stack >= RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.NumberOfCornRequired)
            {
                this.AnimationManager.playAnimation(CornAnimation);
                this.feedType.Value = Enums.SDVObject.Corn;
                who.ActiveObject.Stack -= RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.NumberOfCornRequired;
                this.MinutesUntilReady = RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.MinutesToProcess;
                who.currentLocation.PlaySound(Enums.StardewSound.Ship);
                if (who.ActiveObject.Stack == 0)
                    who.removeItemFromInventory(who.ActiveObject);
                return true;
            }
            if (dropInItem.parentSheetIndex == (int)Enums.SDVObject.Fiber && who.ActiveObject.Stack >= RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.NumberOfFiberRequired)
            {
                this.AnimationManager.playAnimation(FiberAnimation);
                this.feedType.Value = Enums.SDVObject.Fiber;
                who.ActiveObject.Stack -= RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.NumberOfFiberRequired;
                this.MinutesUntilReady = RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.MinutesToProcess;
                who.currentLocation.PlaySound(Enums.StardewSound.Ship);
                if (who.ActiveObject.Stack == 0)
                    who.removeItemFromInventory(who.ActiveObject);
                return true;
            }
            if (dropInItem.parentSheetIndex == (int)Enums.SDVObject.Wheat && who.ActiveObject.Stack >= RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.NumberOfWheatRequired)
            {
                this.AnimationManager.playAnimation(WheatAnimation);
                this.feedType.Value = Enums.SDVObject.Hay;
                who.ActiveObject.Stack -= RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.NumberOfWheatRequired;
                this.MinutesUntilReady = RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.MinutesToProcess;
                who.currentLocation.PlaySound(Enums.StardewSound.Ship);
                if (who.ActiveObject.Stack == 0)
                    who.removeItemFromInventory(who.ActiveObject);
                return true;
            }
            if (dropInItem.parentSheetIndex == (int)Enums.SDVObject.Amaranth && who.ActiveObject.Stack >= RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.NumberOfAmaranthRequired)
            {
                this.AnimationManager.playAnimation(AmaranthAnimation);
                this.feedType.Value = Enums.SDVObject.Amaranth;
                who.ActiveObject.Stack -= RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.NumberOfAmaranthRequired;
                this.MinutesUntilReady = RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.MinutesToProcess;
                who.currentLocation.PlaySound(Enums.StardewSound.Ship);
                if (who.ActiveObject.Stack == 0)
                    who.removeItemFromInventory(who.ActiveObject);
                return true;
            }
            return false;
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            if (this.heldObject.Value != null) return false;
            base.minutesElapsed(minutes, environment);

            if (this.MinutesUntilReady == 0 && this.feedType.Value != Enums.SDVObject.NULL)
            {
                if (this.feedType.Value == Enums.SDVObject.Corn)
                    this.heldObject.Value = RevitalizeModCore.ModContentManager.objectManager.getObject(Enums.SDVObject.Hay, RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.CornToHayOutput);
                if (this.feedType.Value == Enums.SDVObject.Fiber)
                    this.heldObject.Value = RevitalizeModCore.ModContentManager.objectManager.getObject(Enums.SDVObject.Hay, RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.FiberToHayOutput);
                if (this.feedType.Value == Enums.SDVObject.Wheat)
                    this.heldObject.Value = RevitalizeModCore.ModContentManager.objectManager.getObject(Enums.SDVObject.Hay, RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.WheatToHayOutput);
                if (this.feedType.Value == Enums.SDVObject.Amaranth)
                    this.heldObject.Value = RevitalizeModCore.ModContentManager.objectManager.getObject(Enums.SDVObject.Hay, RevitalizeModCore.Configs.objectConfigManager.hayMakerConfig.AmaranthToHayOutput);
                this.AnimationManager.playAnimation(HayAnimation);
                bool noHayRemainsInFeedMaker = this.attemptToFillFarmSilos();
                if (noHayRemainsInFeedMaker == false)
                    //swip and coin are valid sounds too.
                    SoundUtilities.PlaySound(Enums.StardewSound.dwop);

            }
            return true;
        }

        /// <summary>
        /// Attempts to automatically remove hay in the hay maker and put it into farm's Silo.
        /// </summary>
        protected virtual bool attemptToFillFarmSilos()
        {
            if (this.heldObject.Value == null) return false;
            int remainder = Game1.getFarm().tryToAddHay(this.heldObject.Value.Stack);
            if (remainder == 0)
            {
                this.cleanOutHayMaker(false);
                return true;
            }
            else
            {
                this.heldObject.Value.Stack = remainder;
                return false;
            }
        }

        protected override void drawStatusBubble(SpriteBatch b, int x, int y, float Alpha)
        {
            if (this.machineStatusBubbleBox == null || this.machineStatusBubbleBox.Value == null) this.createStatusBubble();
            if (this.MinutesUntilReady == 0 && this.heldObject.Value != null)
            {
                y--;
                float num = (float)(4.0 * Math.Round(Math.Sin(DateTime.UtcNow.TimeOfDay.TotalMilliseconds / 250.0), 2));
                this.machineStatusBubbleBox.Value.playAnimation(MachineStatusBubble_BlankBubbleAnimationKey);
                this.machineStatusBubbleBox.Value.draw(b, this.machineStatusBubbleBox.Value.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize + num)), new Rectangle?(this.machineStatusBubbleBox.Value.getCurrentAnimationFrameRectangle()), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (y + 2) * Game1.tileSize / 10000f) + .00001f);

                Rectangle itemSourceRectangle = GameLocation.getSourceRectForObject(this.heldObject.Value.ParentSheetIndex);
                this.machineStatusBubbleBox.Value.draw(b, Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize) + 8, y * Game1.tileSize + num + 16)), new Rectangle?(itemSourceRectangle), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (y + 2) * Game1.tileSize / 10000f) + .00002f);

            }
        }

        /// <summary>
        /// A simple method for calculating the scale size for showing a machine working.
        /// </summary>
        /// <returns></returns>
        public virtual float getScaleSizeForWorkingMachine()
        {
            float zoomSpeed = 0.01f;
            if (this.Scale.X < Game1.pixelZoom)
                this.Scale = new Vector2(Game1.pixelZoom, Game1.pixelZoom);

            if (this.feedType.Value != Enums.SDVObject.NULL && this.MinutesUntilReady > 0)
            {
                if (this.lerpScaleIncreasing.Value == true)
                {
                    this.Scale = new Vector2(this.scale.X + zoomSpeed, this.scale.Y + zoomSpeed);
                    if (this.Scale.X >= 5.0)
                        this.lerpScaleIncreasing.Value = false;
                }
                else
                {
                    this.Scale = new Vector2(this.scale.X - zoomSpeed, this.scale.Y - zoomSpeed);
                    if (this.Scale.X <= Game1.pixelZoom)
                        this.lerpScaleIncreasing.Value = true;
                }
                return this.Scale.X * Game1.options.zoomLevel;

            }
            else
            {
                float zoom = Game1.pixelZoom * Game1.options.zoomLevel;
                return zoom;
            }
        }

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {

            x = (int)this.TileLocation.X;

            y = (int)this.TileLocation.Y;


            if (this.feedType.Value != Enums.SDVObject.NULL && this.MinutesUntilReady > 0)
            {
                Vector2 origin = new Vector2(this.AnimationManager.getCurrentAnimationFrameRectangle().Width / 2, this.AnimationManager.getCurrentAnimationFrameRectangle().Height);

                this.basicItemInformation.animationManager.draw(spriteBatch, this.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((x + this.basicItemInformation.drawOffset.X) * Game1.tileSize) + this.basicItemInformation.shakeTimerOffset() + Game1.tileSize * origin.X / this.AnimationManager.getCurrentAnimationFrameRectangle().Width, (y + this.basicItemInformation.drawOffset.Y) * Game1.tileSize + this.basicItemInformation.shakeTimerOffset() + Game1.tileSize * (origin.Y / this.AnimationManager.getCurrentAnimationFrameRectangle().Height + 1))), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, origin, this.getScaleSizeForWorkingMachine(), this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (this.TileLocation.Y - this.basicItemInformation.drawOffset.Y) * Game1.tileSize / 10000f) + .00001f);
            }
            else
                this.basicItemInformation.animationManager.draw(spriteBatch, this.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((x + this.basicItemInformation.drawOffset.X) * Game1.tileSize) + this.basicItemInformation.shakeTimerOffset(), (y + this.basicItemInformation.drawOffset.Y) * Game1.tileSize + this.basicItemInformation.shakeTimerOffset())), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (this.TileLocation.Y - this.basicItemInformation.drawOffset.Y) * Game1.tileSize / 10000f) + .00001f);

            if (this.MinutesUntilReady == 0 && this.heldObject.Value != null)
                this.drawStatusBubble(spriteBatch, x + (int)this.basicItemInformation.drawOffset.X, y + (int)this.basicItemInformation.drawOffset.Y, alpha);

        }


        public override Item getOne()
        {
            return new HayMaker(this.basicItemInformation.Copy(), this.isUsedForBuyingHayAtAnyTime);
        }

        public virtual Item getOne(bool IsUsedForBuyingHayAtAnyTime)
        {
            return new HayMaker(this.basicItemInformation.Copy(), IsUsedForBuyingHayAtAnyTime);
        }
    }
}
