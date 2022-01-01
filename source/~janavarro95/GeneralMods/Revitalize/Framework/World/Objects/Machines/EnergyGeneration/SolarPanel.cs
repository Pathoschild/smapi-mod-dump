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
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.EnergyGeneration
{
    public class SolarPanel:Machine
    {
        public SolarPanel() { }

        public SolarPanel(BasicItemInformation info, int EnergyRequiredPer10Minutes = 0, int TimeToProduce = 0) : base(info,null,EnergyRequiredPer10Minutes,TimeToProduce,"")
        {
        }

        public override Item getOne()
        {
            SolarPanel component = new SolarPanel(this.basicItemInfo.Copy(), this.energyRequiredPer10Minutes, this.timeToProduce);
            return component;
        }


        public override void produceEnergy()
        {
            if (this.GetEnergyManager().canReceieveEnergy)
            {
                int energy= this.energyRequiredPer10Minutes;
                if (WeatherUtilities.IsWetWeather())
                {
                    energy = (int)(energy * ModCore.Configs.machinesConfig.solarPanelNonSunnyDayEnergyMultiplier);
                }

                if (Game1.timeOfDay >= Game1.getModeratelyDarkTime())
                {
                    energy = (int)(energy * ModCore.Configs.machinesConfig.solarPanelNightEnergyGenerationMultiplier);
                }
                GameLocation loc = this.getCurrentLocation();
                if (loc != null)
                {
                    if (loc.IsOutdoors == false) return;
                }
                this.GetEnergyManager().produceEnergy(energy);
            }

        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (this.AnimationManager == null)
            {
                spriteBatch.Draw(this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.currentAnimation.sourceRectangle), this.getItemInformation().DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)(y * Game1.tileSize) / 10000f));
            }

            else
            {
                float addedDepth = 0;
                this.AnimationManager.draw(spriteBatch, this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.AnimationManager.currentAnimation.sourceRectangle), this.getItemInformation().DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, (float)((y + addedDepth) * Game1.tileSize) / 10000f) + .00001f);
                try
                {
                    this.AnimationManager.tickAnimation();
                }
                catch (Exception err)
                {
                    ModCore.ModMonitor.Log(err.ToString());
                }
            }

        }

    }
}
