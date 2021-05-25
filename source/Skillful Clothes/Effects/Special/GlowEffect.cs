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
    class GlowEffect : SingleEffect
    {
        const float drawXOffset = 21f;

        public float Radius { get; }
        public Color Color { get; }

        protected int? lightSourceID;        

        protected override EffectDescriptionLine GenerateEffectDescription() => new EffectDescriptionLine(EffectIcon.None, "Emits a constant light");

        public GlowEffect(float radius = 5f, Color? color = null)
        {
            Radius = radius;
            Color = color ?? new Color(0, 30, 150);
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

            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking1;
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking1;
        }

        private void GameLoop_UpdateTicking1(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
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
                environment.repositionLightSource(lightSourceID.Value, new Vector2(who.Position.X + drawXOffset, who.Position.Y) + offset);
                if (!environment.isOutdoors && !(environment is MineShaft) && !(environment is VolcanoDungeon))
                {
                    LightSource i = environment.getLightSource(lightSourceID.Value);
                    if (i != null)
                    {
                        i.radius.Value = Math.Min(Radius, 3f);
                    }
                }
            }
        }

        private void Events_LocationChanged(object sender, ValueChangeEventArgs<GameLocation> e)
        {
            if (e.OldValue != null) RemoveLightSource(e.OldValue);
            if (e.NewValue != null) CreateLightSource(e.NewValue);
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            EffectHelper.ModHelper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking1;
            EffectHelper.Events.LocationChanged -= Events_LocationChanged;

            RemoveLightSource(Game1.currentLocation);
        }

        private void CreateLightSource(GameLocation location)
        {
            Farmer who = Game1.player;
            lightSourceID = GetUniqueId(Game1.currentLocation);
            Game1.currentLocation.sharedLights[lightSourceID.Value] = new LightSource(1, new Vector2(who.Position.X + drawXOffset, who.Position.Y + 64f), Radius, Color, lightSourceID.Value, LightSource.LightContext.None, who.UniqueMultiplayerID);
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
}
