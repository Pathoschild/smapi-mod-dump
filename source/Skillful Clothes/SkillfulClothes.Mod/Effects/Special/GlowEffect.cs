/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework;
using SkillfulClothes.Types;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Special
{
    /// <summary>
    /// Adds a light source behind the player (as the glow rings a do)
    /// (Implementation is adapted from original GlowRing)
    /// </summary>
    class GlowEffect : SingleEffect<GlowEffectParameters>
    {
        const float drawXOffset = 26f;
        const float drawYOffset = -6f;

        public float Radius { get; }
        public Color Color { get; set; }

        protected int? lightSourceID;        

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.Glow, "Emits a constant light");

        public override void ReloadParameters()
        {
            base.ReloadParameters();

            // tint color so that the target color is correct
            Parameters.Color = new Color(255 - Parameters.Color.R, 255 - Parameters.Color.G, 255 - Parameters.Color.B, 155);
        }

        public GlowEffect(GlowEffectParameters parameters)
            : base(parameters)
        {
            // --
        }

        public GlowEffect(float radius, Color? color = null)
            : base(GlowEffectParameters.With(radius, color))
        {
            // --
        }

        private int GetUniqueId(GameLocation location)
        {
            int id = (int)Game1.player.uniqueMultiplayerID + Game1.year + Game1.dayOfMonth + Game1.timeOfDay + Game1.player.getTileX() + (int)Game1.stats.MonstersKilled + (int)Game1.stats.itemsCrafted;

            while (Game1.currentLocation.sharedLights.ContainsKey(id))
            {
                id++;
            }

            return id;
        }

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            RemoveLightSource(Game1.currentLocation);
            CreateLightSource(Game1.currentLocation);
                        
            EffectHelper.Events.LocationChanged -= Events_LocationChanged;
            EffectHelper.Events.LocationChanged += Events_LocationChanged;

            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking;
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
        }

        private void GameLoop_UpdateTicking(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            GameLocation environment = Game1.currentLocation;
            Farmer who = Game1.player;

            if (lightSourceID.HasValue)
            {                               
                Vector2 offset = Vector2.Zero;
                if (who.shouldShadowBeOffset)
                {
                    offset += (Vector2)who.drawOffset;
                }                
                environment.repositionLightSource(lightSourceID.Value, new Vector2(who.Position.X + drawXOffset, who.Position.Y + drawYOffset) + offset);                
            }
        }

        private void Events_LocationChanged(object sender, ValueChangeEventArgs<GameLocation> e)
        {
            if (e.OldValue != null) RemoveLightSource(e.OldValue);
            if (e.NewValue != null) CreateLightSource(e.NewValue);
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking;
            EffectHelper.Events.LocationChanged -= Events_LocationChanged;

            RemoveLightSource(Game1.currentLocation);
        }

        private void CreateLightSource(GameLocation location)
        {
            Farmer who = Game1.player;
            lightSourceID = GetUniqueId(Game1.currentLocation);
            Game1.currentLocation.sharedLights[lightSourceID.Value] = new LightSource(4, new Vector2(who.Position.X + drawXOffset, who.Position.Y + drawYOffset), Radius, Color, lightSourceID.Value, LightSource.LightContext.None, who.UniqueMultiplayerID);
        }

        private void RemoveLightSource(GameLocation location)
        {
            if (lightSourceID.HasValue)
            {
                Game1.currentLocation?.removeLightSource(lightSourceID.Value);
                lightSourceID = null;
            }
        }
    }

    public class GlowEffectParameters : IEffectParameters
    {
        public float Radius { get; set; } = 5f;
        public Color Color { get; set; } = new Color(0, 30, 150);

        public static GlowEffectParameters With(float radius, Color? color = null)
        {
            if (color == null) color = new Color(0, 30, 150);

            return new GlowEffectParameters() { Radius = radius, Color = color.Value };
        }
    }

}
