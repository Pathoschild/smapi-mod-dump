/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Dialogue;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace StardewDruid.Data
{
    public class IconData
    {

        public enum cursors
        {
            none,

            weald,
            mists,
            stars,
            fates,
            comet,
            shield,

            wealdCharge,
            mistsCharge,
            starsCharge,
            fatesCharge,
            chaosCharge,
            shadeCharge,

            scope,
            arrow,
            death,
            skull,
            rock,
            rockTwo,

            shadow,
            target,

        }

        public Texture2D cursorTexture;

        public int cursorColumns;


        public enum displays
        {
            none,

            weald,
            mists,
            stars,
            fates,
            ether,
            chaos,

            effigy,
            jester,
            shadowtin,
            blank1,
            blank2,
            blank3,

            active,
            reverse,
            forward,
            end,
            exit,
            complete,

            quest,
            effect,
            relic,
            herbalism,

        }

        public Texture2D displayTexture;

        public int displayColumns;


        public enum decorations
        {
            none,
            weald,
            mists,
            stars,
            fates,
            ether,

        }

        public Dictionary<Rite.rites, decorations> riteDecorations = new()
        {

            [Rite.rites.weald] = decorations.weald,

            [Rite.rites.mists] = decorations.mists,

            [Rite.rites.stars] = decorations.stars,

            [Rite.rites.fates] = decorations.fates,

            [Rite.rites.ether] = decorations.ether,

        };

        public Texture2D decorationTexture;

        public int decorationColumns;

        public enum impacts
        {
            none,
            impact,
            flashbang,
            glare,
            splash,
            sparkle,
            fish,
            cloud,
            death,
            splatter,
            nature,

        }

        public Texture2D impactsTexture;

        public enum skies
        {
            none,
            night,
            sunset,
            valley,
            mountain,
            hell,
            moon,

        }

        public Texture2D skyTexture;

        public Texture2D missileTexture;

        public enum missiles
        {
            none,
            blaze,
            sparks,
            trail,
            rocks,
        }

        public enum tilesheets
        {
            none,
            druid,
        }

        public Dictionary<tilesheets, Texture2D> sheetTextures = new();


        public enum relics
        {
            none,
            flask,
            flask1,
            flask2,
            flask3,
            flask4,
            flask5,
            bottle,
            bottle1,
            bottle2,
            bottle3,
            bottle4,
            bottle5,
            vial,
            vial1,
            vial2,
            vial3,
            vial4,
            vial5,
            effigy_crest,
            companion_blank_1, 
            companion_blank_2, 
            companion_blank_3, 
            companion_blank_4, 
            companion_blank_5, 
            minister_mitre,
            minister_blank_1, 
            minister_blank_2, 
            minister_blank_3, 
            minister_blank_4, 
            minister_blank_5,
            avalant_disc,
            avalant_chassis,
            avalant_gears,
            avalant_casing,
            avalant_needle,
            avalant_measure,

        }

        public Texture2D relicsTexture;

        public int relicColumns;

        public Texture2D boltTexture;

        public Texture2D warpTexture;

        public enum schemes
        {
            none,
            weald,
            mists,
            stars,
            fates,
            ether,
            fire,
            psychic,
            rock,
            rockTwo,
            death,
            apple,
            grannysmith,
            pumpkin,
            Emerald,
            Aquamarine,
            Ruby,
            Amethyst,
            Topaz,
            Solar,
            Void,
        }

        public Dictionary<IconData.schemes, Microsoft.Xna.Framework.Color> schemeColours = new()
        {
            [schemes.none] = Microsoft.Xna.Framework.Color.White,
            [schemes.weald] = Microsoft.Xna.Framework.Color.LightGreen,
            [schemes.mists] = new(75,138,187),
            [schemes.stars] = new(255,237,128),
            [schemes.fates] = new(237,119,123),
            [schemes.ether] = new(159, 209, 245),
            [schemes.fire] = new(255, 128, 96),
            [schemes.psychic] = new(204, 82, 132),
            [schemes.rock] = new(80, 114, 130),
            [schemes.rockTwo] = new(166, 124, 82),
            [schemes.death] = new(70, 60, 70),
            [schemes.apple] = new(255, 38, 38),
            [schemes.grannysmith] = new(67, 255, 83),
            [schemes.pumpkin] = new(255, 156, 33),
            [schemes.Emerald] = new(67, 255, 83),
            [schemes.Aquamarine] = new(74, 243, 255),
            [schemes.Ruby] = new(255, 38, 38),
            [schemes.Amethyst] = new(255, 67, 251),
            [schemes.Topaz] = new(255, 156, 33),
            [schemes.Solar] = new(255, 194, 128),
            [schemes.Void] = new(200, 100, 190),
        };

        public IconData()
        {

            cursorTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Cursors.png"));

            cursorColumns = 6;

            displayTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Displays.png"));

            displayColumns = 6;

            decorationTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Decorations.png"));

            decorationColumns = 5;

            impactsTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Impacts.png"));
            
            skyTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Skies.png"));

            missileTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Missiles.png"));

            sheetTextures[tilesheets.druid] = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Sheets", "Druid.png"));

            relicColumns = 6;

            relicsTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Relics.png"));

            boltTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Bolt.png"));

            warpTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Warp.png"));

        }

        public Microsoft.Xna.Framework.Rectangle CursorRect(cursors id)
        {

            if (id == cursors.none) { return new(); }

            int slot = Convert.ToInt32(id) - 1;

            return new(slot % cursorColumns * 32, slot / cursorColumns * 32, 32, 32);


        }

        public TemporaryAnimatedSprite CursorIndicator(GameLocation location, Vector2 origin, cursors cursorId, CursorAdditional additional)
        {

            Vector2 originOffset = origin - new Vector2(16 * additional.scale, 16 * additional.scale);

            if (additional.offset)
            {

                originOffset += new Vector2(32f, 32f);

            }

            Microsoft.Xna.Framework.Rectangle cursorRect = Mod.instance.iconData.CursorRect(cursorId);

            float layer = (origin.Y - 128) / 10000;

            TemporaryAnimatedSprite animation = new(0, additional.interval, 1, additional.loops, originOffset, false, false)
            {

                sourceRect = cursorRect,

                sourceRectStartingPos = new Vector2(cursorRect.X, cursorRect.Y),

                texture = cursorTexture,

                scale = additional.scale,

                layerDepth = layer,

                timeBasedMotion = true,

                Parent = location,

                alpha = additional.alpha,

                alphaFade = additional.fade,

                delayBeforeAnimationStart = additional.delay,

                color = additional.color,

            };

            if (additional.rotation > 0)
            {

                animation.rotationChange = (float)(Math.PI / additional.rotation);

            }

            location.temporarySprites.Add(animation);

            return animation;

        }

        public Microsoft.Xna.Framework.Rectangle DisplayRect(displays id)
        {

            if (id == displays.none) { return new(); }

            int slot = Convert.ToInt32(id) - 1;

            return new(slot % displayColumns * 16, slot / displayColumns * 16, 16, 16);

        }

        public Microsoft.Xna.Framework.Rectangle DecorativeRect(decorations id)
        {

            if (id == decorations.none) { return new(); }

            int slot = Convert.ToInt32(id) - 1;

            return new(slot % decorationColumns * 64, slot / decorationColumns * 64, 64, 64);

        }

        public TemporaryAnimatedSprite DecorativeIndicator(GameLocation location, Vector2 origin, decorations decorationId, float scale, DecorativeAdditional additional)
        {

            Vector2 originOffset = origin + new Vector2(32f, 32f) - (new Vector2(32f, 32f) * scale);

            Microsoft.Xna.Framework.Rectangle rect = DecorativeRect(decorationId);

            float interval = additional.interval;

            float rotation = 0;

            if (additional.rotation > 0)
            {

                rotation = (float)(Math.PI / additional.rotation);

            }

            float alpha = additional.alpha;

            int delay = additional.delay;

            int loops = additional.loops;

            float layer = (originOffset.Y - 32) / 10000;

            TemporaryAnimatedSprite animation = new(0, interval, 1, loops, originOffset, false, false)
            {

                sourceRect = rect,

                sourceRectStartingPos = new Vector2(rect.X, rect.Y),

                texture = decorationTexture,

                scale = scale,

                timeBasedMotion = true,

                layerDepth = layer,

                rotationChange = rotation,

                Parent = location,

                alpha = alpha,

                delayBeforeAnimationStart = delay,

            };

            location.temporarySprites.Add(animation);

            return animation;

        }

        public TemporaryAnimatedSprite ImpactIndicator(GameLocation location, Vector2 origin, impacts impact, float size, ImpactAdditional additional)
        {

            if (impact == impacts.none)
            {

                return null;
            }
            if (impact == impacts.nature)
            {
                additional.color = Microsoft.Xna.Framework.Color.Green;

                additional.light = 0f;
                
                ImpactIndicator(location, origin, IconData.impacts.sparkle, size - 1, additional);

                additional.color = Microsoft.Xna.Framework.Color.LightGreen;

                additional.light = 0f;

                ImpactIndicator(location, origin, IconData.impacts.sparkle, size - 2, additional);

                additional.color = Microsoft.Xna.Framework.Color.White;

                additional.light = 0.005f;

                return ImpactIndicator(location, origin, IconData.impacts.sparkle, size, additional);

            }

            if(impact == impacts.glare)
            {

                additional.light = 0.01f;

            }

            return CreateIndicator(location, origin, impact, size, additional);


        }

        public TemporaryAnimatedSprite CreateIndicator(GameLocation location, Vector2 origin, impacts impact, float scale, ImpactAdditional additional)
        {

            Vector2 originOffset = origin + new Vector2(32f, 32f) - (new Vector2(32f, 32f) * scale);

            float interval = additional.interval;

            int frame = additional.frame;

            Microsoft.Xna.Framework.Color color = additional.color;

            float rotation = 0;

            if (additional.rotation > 0)
            {

                rotation = (float)(Math.PI / additional.rotation);

            }

            float layer = additional.layer;

            if(additional.layer == -1)
            {

                layer = originOffset.Y / 10000;

            }

            int Y = (Convert.ToInt32(impact) - 1) * 64;

            TemporaryAnimatedSprite bomb = new(0, interval, 8 - frame, 1, originOffset, false, false)
            {
                sourceRect = new(64 * frame, Y, 64, 64),
                sourceRectStartingPos = new Vector2(64 * frame, Y),
                texture = impactsTexture,
                scale = scale,
                timeBasedMotion = true,
                layerDepth = layer,
                color = color,
                rotation = rotation,
                alpha = additional.alpha,
                flipped = additional.flip,
                delayBeforeAnimationStart = additional.delay,
            };

            location.temporarySprites.Add(bomb);

            if (additional.light > 0f)
            {

                TemporaryAnimatedSprite flash = new(23, 500f, 6, 1, origin, false, Game1.random.NextDouble() < 0.5)
                {
                    texture = Game1.mouseCursors,
                    light = true,
                    lightRadius = scale,
                    lightcolor = Microsoft.Xna.Framework.Color.Black,
                    alphaFade = additional.light,
                    Parent = location
                };

                location.temporarySprites.Add(flash);

            }

            return bomb;

        }

        public TemporaryAnimatedSprite SkyIndicator(GameLocation location, Vector2 origin, skies slot, float scale, SkyAdditional additional)
        {

            Vector2 originOffset = origin + new Vector2(32f, 32f) - (new Vector2(32f, 32f) * scale);

            if (slot == skies.none)
            {

                return null;

            }

            float interval = additional.interval;

            float alpha = additional.alpha;

            int delay = additional.delay;

            int loops = additional.loops;

            float layer = originOffset.Y / 10000;

            TemporaryAnimatedSprite sky= new(0, interval, 1, loops, originOffset, false, false)
            {

                sourceRect = new(((int)slot-1)*64, 0, 64, 64),

                sourceRectStartingPos = new Vector2(((int)slot - 1) * 64, 0),

                texture = skyTexture,

                scale = scale,

                layerDepth = layer,

                alpha = alpha,

                delayBeforeAnimationStart = delay,

            };

            location.temporarySprites.Add(sky);

            return sky;

        }


        public static Microsoft.Xna.Framework.Rectangle MissileRectangles(missiles missile)
        {

            Microsoft.Xna.Framework.Rectangle rect = new(0, ((int)missile -1)*96, 96, 96);

            return rect;

        }

        public Microsoft.Xna.Framework.Rectangle RelicRectangles(relics relic)
        {

            if (relic == relics.none) { return new(); }

            int slot = Convert.ToInt32(relic) - 1;

            return new(slot % relicColumns * 20, slot / relicColumns * 20, 20, 20);

        }


        public void AnimateBolt(GameLocation location, Vector2 origin)
        {

            Vector2 originOffset = new(origin.X - 64, origin.Y - 320);

            Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, origin + new Vector2(0, -320), IconData.impacts.cloud, 1.5f, new() { interval = 75, color = Mod.instance.iconData.schemeColours[schemes.mists], flip = true, layer = 999f, });

            Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, origin + new Vector2(0, -320), IconData.impacts.cloud, 2f, new() { interval = 75, color = Microsoft.Xna.Framework.Color.White, layer = 998f, });

            TemporaryAnimatedSprite boltAnimation = new(0, 75, 6, 1, originOffset, false, (Game1.random.NextDouble() < 0.5) ? true : false)
            {

                sourceRect = new(0, 0, 64, 128),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = boltTexture,

                layerDepth = 997f,

                alpha = 0.65f,

                scale = 3f,

            };

            location.temporarySprites.Add(boltAnimation);

            Vector2 lightOffset = new(origin.X, origin.Y - 192);

            TemporaryAnimatedSprite lightAnimation = new(23, 500f, 1, 1, lightOffset, flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false)
            {
                light = true,
                lightRadius = 6,
                lightcolor = Microsoft.Xna.Framework.Color.Black,
                alpha = 0.5f,
                alphaFade = 0.001f,
                Parent = location,

            };

            location.temporarySprites.Add(lightAnimation);

            location.playSound("flameSpellHit");

            return;

        }

        public void AnimateQuickWarp(GameLocation location, Vector2 origin, bool reverse = false)
        {

            Vector2 originOffset = origin - new Vector2(32, 32);

            Microsoft.Xna.Framework.Rectangle rect = reverse ? new(0, 32, 32, 32) : new(0, 0, 32, 32);

            TemporaryAnimatedSprite cursorAnimation = new(0, 75, 8, 1, originOffset, false, false)
            {

                sourceRect = rect,

                sourceRectStartingPos = new Vector2(0, rect.Y),

                texture = warpTexture,

                scale = 4f,

                layerDepth = 0.001f,

                alpha = 0.65f,

            };

            location.temporarySprites.Add(cursorAnimation);


        }

        public TemporaryAnimatedSprite AnimateTarget(GameLocation location, Vector2 origin, IconData.schemes scheme)
        {

            Vector2 originOffset = origin - new Vector2(32, 32);

            Vector2 animationMotion = new Vector2(0, -0.3f);

            Vector2 animationAcceleration = new Vector2(0f, 0.002f);

            Microsoft.Xna.Framework.Rectangle cursorRect = CursorRect(cursors.target);

            TemporaryAnimatedSprite targetAnimation = new(0, 5000f, 1, 1, originOffset, false, false)
            {

                sourceRect = cursorRect,

                sourceRectStartingPos = new Vector2(cursorRect.X, cursorRect.Y),

                texture = cursorTexture,

                layerDepth = 999f,

                scale = 2f,

                motion = animationMotion,

                acceleration = animationAcceleration,

                color = schemeColours[scheme],

            };

            location.temporarySprites.Add(targetAnimation);

            return targetAnimation;

        }


    }

    public class CursorAdditional
    {

        public float interval = 1200f;

        public float alpha = 0.65f;

        public float fade = 0f;

        public float rotation = 60;

        public int delay = 0;

        public float scale = 3f;

        public int loops = 1;

        public bool offset = true;

        public Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;

    }

    public class DecorativeAdditional
    {

        public float interval = 1000f;

        public float alpha = 0.65f;

        public float rotation = 120;

        public int delay = 0;

        public int loops = 1;

    }

    public class ImpactAdditional
    {

        public int frame = 0;

        public float interval = 100f;

        public float alpha = 0.5f;

        public float rotation = 0;

        public Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;

        public int delay = 0;

        public float light = 0.03f;

        public bool flip = false;

        public float layer = -1;

    }

    public class SkyAdditional
    {

        public float interval = 1000f;

        public float alpha = 0.75f;

        public int delay = 0;

        public int loops = 1;

    }
}
