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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Items;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using StardewValley;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Graphics;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Omegasis.Revitalize.Framework.Utilities.Extensions;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.Misc
{
    //Game1.netWorldState.Value.MuseumPieces =>gets total musuem pices donated. Key is position, value is parent sheet index.
    //

    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Machines.Misc.CrystalRefiner")]
    public class CrystalRefiner : ItemRecipeDropInMachine
    {

        public CrystalRefiner()
        {

        }

        public CrystalRefiner(BasicItemInformation Info) : this(Info, Vector2.Zero)
        {

        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            if((t is Pickaxe || t is Axe) && this.heldObject.Value!=null)
            {
                this.dropHeldObject(location,this.TileLocation*Game1.tileSize,Game1.player.getTileLocation()*Game1.tileSize);
                SoundUtilities.PlaySoundAt(Enums.StardewSound.woodyStep, location, this.TileLocation);
                this.MinutesUntilReady = 0;
                this.updateAnimation();


                this.shakeTimer = 200;
                this.basicItemInformation.shakeTimer.Value = 200;
                SoundUtilities.PlaySound(Enums.StardewSound.hammer);

                return false;
            }

            return base.performToolAction(t, location);
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            int originalMinutesUntilReady = this.MinutesUntilReady;
            base.minutesElapsed(minutes, environment);

            if (this.heldObject.Value != null)
            {
                if(this.heldObject.Value.Quality<this.getMaxQualityLevelForItems() && this.MinutesUntilReady == 0)
                {
                    this.MinutesUntilReady = originalMinutesUntilReady -= minutes; //Underflow the value to have some carry over into processing the next quality tier.
                    if (this.heldObject.Value.Quality == 2 || this.heldObject.Value.Quality == 4)
                    {
                        this.heldObject.Value.Quality += 2;
                    }
                    else
                    {
                        this.heldObject.Value.Quality += 1;
                    }

                    if (this.heldObject.Value.Quality != this.getMaxQualityLevelForItems())
                    {
                        this.MinutesUntilReady += this.getTimeToProcessForQuality(this.heldObject.Value.Quality).toInGameMinutes();
                    }
                }
            }


            return false;
        }

        public override void updateAnimation()
        {
            base.updateAnimation();
            if (this.isIdle())
            {
                this.basicItemInformation.DrawColor = Color.White;
            }
            else if (this.isWorking())
            {
                try
                {
                    this.basicItemInformation.DrawColor = Texture2DExtensions.GetMiddlePixelFromParentSheetIndex(Game1.objectSpriteSheet, this.heldObject.Value.ParentSheetIndex, 16, 16);
                }
                catch(Exception) {
                    this.basicItemInformation.DrawColor = Color.White;
                    StardewValley.Menus.TailoringMenu.GetDyeColor(this.heldObject.Value);
                }
            }
            else
            {
                this.basicItemInformation.DrawColor = Color.White;
            }
        }

        public CrystalRefiner(BasicItemInformation Info, Vector2 TilePosition) : base(Info, TilePosition)
        {
        }

        public override CraftingResult processInput(IList<Item> inputItems, Farmer who, bool ShowRedMessage = true)
        {
            if (string.IsNullOrEmpty(this.getCraftingRecipeBookId()) || this.isWorking() || this.finishedProduction())
            {
                return new CraftingResult(false);
            }

            List<KeyValuePair<IList<Item>, ProcessingRecipe>> validRecipes = this.getListOfValidRecipes(inputItems, who, ShowRedMessage);

            if (validRecipes.Count > 0)
            {
                return this.onSuccessfulRecipeFound(validRecipes.First().Key, validRecipes.First().Value, who);
            }

            return new CraftingResult(false);
        }

        /// <summary>
        /// Generate the list of potential recipes based on the contents of the farmer's inventory.
        /// </summary>
        /// <param name="inputItems"></param>
        /// <returns></returns>
        public override List<ProcessingRecipe> getListOfPotentialRecipes(IList<Item> inputItems, Farmer who = null)
        {
            List<ProcessingRecipe> possibleRecipes = new List<ProcessingRecipe>();
            possibleRecipes.AddRange(base.getListOfPotentialRecipes(inputItems));
            foreach (Item item in inputItems)
            {
                if (item == null) continue;
                if ((item.Category == StardewValley.Object.mineralsCategory || item.Category == StardewValley.Object.GemCategory) && (item as StardewValley.Object).Quality < this.getMaxQualityLevelForItems())
                {
                    ItemReference input = new ItemReference(item.getOne());
                    ItemReference output = new ItemReference(item.getOne(), 1, input.Quality);
                    GameTimeStamp timeToProcess = this.getTimeToProcessForQuality(input.Quality);

                    possibleRecipes.Add(new ProcessingRecipe(input.RegisteredObjectId, timeToProcess, input, new LootTableEntry(output)));
                }
            }
            return possibleRecipes;
        }



        /// <summary>
        /// Gets the maximum possible quality level for an object.
        /// </summary>
        /// <returns></returns>
        public virtual int getMaxQualityLevelForItems()
        {
            return RevitalizeModCore.Configs.objectConfigManager.objectsConfig.maxQualityLevel;
        }

        /// <summary>
        /// Gets the number of days it takes to process an object's quality to the next level.
        /// </summary>
        /// <param name="QualityLevel"></param>
        /// <returns></returns>
        public virtual GameTimeStamp getTimeToProcessForQuality(int QualityLevel)
        {
            if (QualityLevel == StardewValley.Object.highQuality)
            {
                QualityLevel = 3; //Make it so that gold->iridium quality takes 4 days.
            }

            return new GameTimeStamp(0,0, Math.Max(1, QualityLevel+1),0,0);
        }

        public override void playDropInSound()
        {
            SoundUtilities.PlaySound(Enums.StardewSound.woodyStep);
        }


        public override Item getOne()
        {
            return new CrystalRefiner(this.basicItemInformation.Copy());
        }



        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            x = (int)this.TileLocation.X;

            y = (int)this.TileLocation.Y;

            if (base.heldObject.Value != null)
            {

                if (this.heldObject.Value.ParentSheetIndex == (int)Enums.SDVObject.PrismaticShard)
                {
                    this.basicItemInformation.DrawColor = Utility.GetPrismaticColor();
                }

                if ((int)base.heldObject.Value.Quality > 0)
                {
                    Vector2 scaleFactor = (((int)base.MinutesUntilReady > 0) ? new Vector2(Math.Abs(base.scale.X - 5f), Math.Abs(base.scale.Y - 5f)) : Vector2.Zero);
                    scaleFactor *= 4f;
                    Vector2 position = Game1.GlobalToLocal(Game1.viewport, (this.TileLocation * 64) - new Vector2(0, 32f));
                    Microsoft.Xna.Framework.Rectangle destination = new Microsoft.Xna.Framework.Rectangle((int)(position.X + 32f - 8f - scaleFactor.X / 2f) + ((base.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(position.Y + 64f + 8f - scaleFactor.Y / 2f) + ((base.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(16f + scaleFactor.X), (int)(16f + scaleFactor.Y / 2f));
                    spriteBatch.Draw(Game1.mouseCursors, destination, ((int)base.heldObject.Value.Quality < 4) ? new Microsoft.Xna.Framework.Rectangle(338 + ((int)base.heldObject.Value.Quality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8), Microsoft.Xna.Framework.Color.White * 0.95f, 0f, Vector2.Zero, SpriteEffects.None, Math.Max(0f, (this.TileLocation.Y + 1 + /*Added depth*/0f) * Game1.tileSize / 10000f) + .0002f);
                }
            }

            if (this.isWorking())
            {
                Vector2 origin = new Vector2(this.AnimationManager.getCurrentAnimationFrameRectangle().Width / 2, this.AnimationManager.getCurrentAnimationFrameRectangle().Height);

                this.basicItemInformation.animationManager.draw(spriteBatch, this.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((x + this.basicItemInformation.drawOffset.X) * Game1.tileSize) + this.basicItemInformation.shakeTimerOffset() + Game1.tileSize * origin.X / this.AnimationManager.getCurrentAnimationFrameRectangle().Width, (y + this.basicItemInformation.drawOffset.Y) * Game1.tileSize + this.basicItemInformation.shakeTimerOffset() + Game1.tileSize * (origin.Y / this.AnimationManager.getCurrentAnimationFrameRectangle().Height + 1))), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, origin, this.getScaleSizeForWorkingMachine(), this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (this.TileLocation.Y - this.basicItemInformation.drawOffset.Y) * Game1.tileSize / 10000f) + .00001f);
            }
            else
                this.basicItemInformation.animationManager.draw(spriteBatch, this.basicItemInformation.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((x + this.basicItemInformation.drawOffset.X) * Game1.tileSize) + this.basicItemInformation.shakeTimerOffset(), (y + this.basicItemInformation.drawOffset.Y) * Game1.tileSize + this.basicItemInformation.shakeTimerOffset())), new Rectangle?(this.AnimationManager.getCurrentAnimation().getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (this.TileLocation.Y - this.basicItemInformation.drawOffset.Y) * Game1.tileSize / 10000f) + .00001f);

            if (this.finishedProduction())
                this.drawStatusBubble(spriteBatch, x + (int)this.basicItemInformation.drawOffset.X, y + (int)this.basicItemInformation.drawOffset.Y, alpha);
        }
    }
}
