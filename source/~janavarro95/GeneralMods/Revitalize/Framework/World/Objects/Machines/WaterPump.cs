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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omegasis.Revitalize;
using Omegasis.Revitalize.Framework.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Machines;
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Revitalize.Framework.Objects.Machines
{
    public class WaterPump : Machine
    {

        public WaterPump() { }

        public WaterPump(BasicItemInformation info, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();

        }

        public WaterPump(BasicItemInformation info, Vector2 TileLocation, List<ResourceInformation> ProducedResources = null, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0, string CraftingBook = "") : base(info, TileLocation)
        {
            this.producedResources = ProducedResources ?? new List<ResourceInformation>();
            this.energyRequiredPer10Minutes = EnergyRequiredPer10Minutes;
            this.timeToProduce = TimeToProduce;
            this.MinutesUntilReady = TimeToProduce;
            this.craftingRecipeBook = CraftingBook;
            this.createStatusBubble();
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            base.updateWhenCurrentLocation(time, environment);

            this.AnimationManager.prepareForNextUpdateTick();
        }


        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            int remaining = minutes;
            while (remaining > 0)
            {
                if (LocationUtilities.IsThereWaterAtThisTile(environment, (int)this.TileLocation.X, (int)this.TileLocation.Y))
                {
                    this.GetFluidManager().produceFluid(ModCore.ObjectManager.resources.getFluid("Water"), 100);
                }
                remaining -= 10;
            }
            return false;
        }

        public override bool canBePlacedInWater()
        {
            return true;
        }

        public override bool rightClicked(Farmer who)
        {
            this.createMachineMenu();
            return true;
        }

        public override Item getOne()
        {
            WaterPump component = new WaterPump(this.basicItemInfo.Copy(), this.producedResources, this.energyRequiredPer10Minutes, this.timeToProduce, this.craftingRecipeBook);
            return component;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (this.AnimationManager == null)
            {
                if (this.AnimationManager.getExtendedTexture() == null)
                    ModCore.ModMonitor.Log("Tex Extended is null???");

                spriteBatch.Draw(this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.currentAnimation.sourceRectangle), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
            }

            else
            {
                float addedDepth = 0;
                this.AnimationManager.draw(spriteBatch, this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.currentAnimation.sourceRectangle), this.basicItemInfo.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);
                this.drawStatusBubble(spriteBatch, x, y, alpha);

                try
                {
                    if (this.AnimationManager.canTickAnimation())
                    {
                        this.AnimationManager.tickAnimation();
                    }
                }
                catch (Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString());
                }
            }
        }
    }
}
