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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using static StardewDruid.Data.IconData;
using static System.Formats.Asn1.AsnWriter;

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

            wealdCharge,
            mistsCharge,
            starsCharge,
            fatesCharge,

            scope,
            death,
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
            revenant,
            jester,
            buffin,
            shadowtin,
            blank1,

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
            replay,
            flag,

            speech,
            blaze,
            morph,
            skull,
            daze,
            knock,

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

            spiral,
            clouds,
            splatter,
            puff,
            cinder,
            slash,

            death,
            deathwhirl,
            immolation,

            nature,
            bomb,
            combustion,
            boltswirl,
            deathbomb,

        }

        public Texture2D impactsTexture;

        public Texture2D impactsTextureTwo;

        public Texture2D impactsTextureThree;

        public enum skies
        {
            none,
            night,
            sunset,
            valley,
            mountain,
            temple,
            moon,

        }

        public Texture2D skyTexture;

        public Texture2D missileTexture;

        public enum missiles
        {
            none,
            fireball,
            cannonball,
            meteor,
            death,
            slimeball,
            rockfall,
            whisk,
        }

        public enum missileIndexes
        {
            blazeCore1,
            blazeCore2,
            blazeCore3,
            blazeCore4,
            blazeInner1,
            blazeInner2,
            blazeInner3,
            blazeInner4,
            blazeOuter1,
            blazeOuter2,
            blazeOuter3,
            blazeOuter4,
            blazeOutline1,
            blazeOutline2,
            blazeOutline3,
            blazeOutline4,
            trail1,
            trail2,
            trail3,
            trail4,
            trailOutline1,
            trailOutline2,
            trailOutline3,
            trailOutline4,
            sparkCore1,
            sparkCore2,
            sparkCore3,
            sparkCore4,
            sparkInner1,
            sparkInner2,
            sparkInner3,
            sparkInner4,
            sparkOutline1,
            sparkOutline2,
            sparkOutline3,
            sparkOutline4,
            scatter1,
            scatter2,
            scatter3,
            scatter4,
            meteor1,
            meteor2,
            meteor3,
            rock1,
            rock2, 
            rock3, 
            death,
            cannonball,
            star1,
            star2,
            star3,
            star4,

        }

        public enum tilesheets
        {
            none,
            druid,
            atoll,
            chapel,
            court,
            tomb,

        }

        public Dictionary<tilesheets, Texture2D> sheetTextures = new();

        public const string druid_assetName = "Sheets/Druid";

        public const string druid_tilesheet = "Sheets/Druid.png";

        public const string atoll_assetName = "Sheets/Atoll";

        public const string atoll_tilesheet = "Sheets/Atoll.png";

        public const string chapel_assetName = "Sheets/Chapel";

        public const string chapel_tilesheet = "Sheets/Chapel.png";

        public const string court_assetName = "Sheets/Court";

        public const string court_tilesheet = "Sheets/Court.png";

        public const string tomb_assetName = "Sheets/Tomb";

        public const string tomb_tilesheet = "Sheets/Tomb.png";

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
            jester_dice,
            shadowtin_tome,
            companion_3, 
            companion_4, 
            companion_5,
            wayfinder_lantern,
            wayfinder_water,
            wayfinder_eye,
            wayfinder_ceremonial,
            warp_4,
            warp_5,
            herbalism_mortar,
            herbalism_pan,
            herbalism_still,
            herbalism_crucible,
            herbalism_1,
            herbalism_2,
            runestones_spring,
            runestones_farm,
            runestones_moon,
            runestones_cat, 
            saurus_skull, 
            runestones_6,
            avalant_disc,
            avalant_chassis,
            avalant_gears,
            avalant_casing,
            avalant_needle,
            avalant_measure,
            book_wyrven,
            texts_1,
            texts_2, texts_3, texts_4,
            texts_5,
            courtesan_pin,

        }

        public Texture2D relicsTexture;

        public int relicColumns;

        public Texture2D boltTexture;

        public Texture2D laserTexture;

        public Texture2D warpTexture;

        public Texture2D gravityTexture;

        public Texture2D emberTexture;

        public Texture2D crateTexture;

        public Texture2D warpstrikeTexture;

        public Texture2D echoTexture;

        public Texture2D wispTexture;

        public Texture2D shieldTexture;

        public enum schemes
        {
            none,

            ember,
            ether,
            bolt,
            fates,
            weald,

            rock,
            rockTwo,
            rockThree,
            death,
            Emerald,
            Aquamarine,
            Ruby,
            Amethyst,
            Topaz,
            Solar,
            Void,

            apple,
            grannysmith,
            pumpkin,
            plum,
            blueberry,
            melon,

            RedDragon,
            GreenDragon,


        }

        public Dictionary<IconData.schemes, Microsoft.Xna.Framework.Color> schemeColours = new()
        {
            [schemes.none] = Microsoft.Xna.Framework.Color.White,

            [schemes.ember] = new(255, 173, 84),
            [schemes.ether] = new(84, 163, 218),
            [schemes.bolt] = new(75, 138, 187),
            [schemes.fates] = new(119, 75, 131),

            [schemes.rock] = new(120, 154, 160),
            [schemes.rockTwo] = new(196, 164, 122),
            [schemes.rockThree] = new(144, 126, 144),
            [schemes.death] = new(70, 60, 70),
            [schemes.Emerald] = new(67, 255, 83),
            [schemes.Aquamarine] = new(74, 243, 255),
            [schemes.Ruby] = new(255, 38, 38),
            [schemes.Amethyst] = new(255, 67, 251),
            [schemes.Topaz] = new(255, 156, 33),
            [schemes.Solar] = new(255, 194, 128),
            [schemes.Void] = new(200, 100, 190),

            [schemes.apple] = new(237, 32, 36),
            [schemes.grannysmith] = new(141, 198, 63),
            [schemes.pumpkin] = new(238, 102, 37),
            [schemes.plum] = new(196, 35, 86),
            [schemes.blueberry] = new(84, 104, 177),
            [schemes.melon] = new(107, 192, 111),

        };

        public Dictionary<IconData.schemes, List<Microsoft.Xna.Framework.Color>> gradientColours = new()
        {
            [schemes.none] = new() { Microsoft.Xna.Framework.Color.White, Microsoft.Xna.Framework.Color.Gray, Microsoft.Xna.Framework.Color.DarkGray, },
            [schemes.ember] = new() { new(255, 230, 166), new(255, 173, 84), new(231, 102, 84), },
            [schemes.ether] = new() { new(111, 203, 220), new(84,163,218), new(13, 114, 185), },
            [schemes.bolt] = new() { new(75, 138, 187), new(70, 81, 144), new(83,96,150), },
            [schemes.fates] = new() { new(119, 75, 131), new(59, 55, 100), new(37,34,74), new(236, 118, 124), },
            [schemes.weald] = new() { Microsoft.Xna.Framework.Color.White, Microsoft.Xna.Framework.Color.LightGreen, Microsoft.Xna.Framework.Color.Green },
            [schemes.RedDragon] = new() { new(190, 30, 45), new(191, 142, 93), new(39, 170, 225) },
            [schemes.GreenDragon] = new() { new(121,172,66), new(207,165,73), new(248, 149, 32) },
        };

        public Dictionary<Rite.rites, IconData.schemes> RiteScheme = new()
        {
            [Rite.rites.weald] = schemes.grannysmith,
            [Rite.rites.mists] = schemes.bolt,
            [Rite.rites.stars] = schemes.apple,
            [Rite.rites.fates] = schemes.plum,
            [Rite.rites.ether] = schemes.ether,

        };

        public IconData()
        {

            cursorTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Cursors.png"));

            cursorColumns = 4;

            displayTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Displays.png"));

            displayColumns = 6;

            decorationTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Decorations.png"));

            decorationColumns = 5;

            impactsTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Impacts.png"));

            impactsTextureTwo = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "ImpactsTwo.png"));

            impactsTextureThree = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "ImpactsThree.png"));

            skyTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Skies.png"));

            missileTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Missiles.png"));

            sheetTextures[tilesheets.druid] = Mod.instance.Helper.ModContent.Load<Texture2D>(druid_tilesheet);

            sheetTextures[tilesheets.atoll] = Mod.instance.Helper.ModContent.Load<Texture2D>(atoll_tilesheet);

            sheetTextures[tilesheets.chapel] = Mod.instance.Helper.ModContent.Load<Texture2D>(chapel_tilesheet);

            sheetTextures[tilesheets.court] = Mod.instance.Helper.ModContent.Load<Texture2D>(court_tilesheet);

            sheetTextures[tilesheets.tomb] = Mod.instance.Helper.ModContent.Load<Texture2D>(tomb_tilesheet);

            relicColumns = 6;

            relicsTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Relics.png"));

            boltTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Bolt.png"));

            laserTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Laser.png"));

            warpTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Warp.png"));

            gravityTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Gravity.png"));

            emberTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Embers.png"));

            crateTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "DragonCrate.png"));

            warpstrikeTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Warpstrike.png"));

            echoTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Echo.png"));

            wispTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Wisp.png"));

            shieldTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Shield.png"));

        }

        public Microsoft.Xna.Framework.Color SchemeColour(schemes scheme)
        {

            if (schemeColours.ContainsKey(scheme))
            {
                
                return schemeColours[scheme];
            
            }

            return Microsoft.Xna.Framework.Color.White;

        }

        public Microsoft.Xna.Framework.Rectangle CursorRect(cursors id)
        {

            if (id == cursors.none) { return new(); }

            int slot = Convert.ToInt32(id) - 1;

            return new(slot % cursorColumns * 48, slot / cursorColumns * 48, 48, 48);


        }

        public TemporaryAnimatedSprite CursorIndicator(GameLocation location, Vector2 origin, cursors cursorId, CursorAdditional additional)
        {

            Vector2 originOffset = origin + new Vector2(32f, 32f) - new Vector2(24 * additional.scale, 24 * additional.scale);

            Microsoft.Xna.Framework.Rectangle cursorRect = Mod.instance.iconData.CursorRect(cursorId);

            float layer = (origin.Y - 128) / 10000;

            Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;

            switch (cursorId)
            {
                case cursors.scope:
                case cursors.death:
                case cursors.target:

                    color = SchemeColour(additional.scheme);

                    break;

            }

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

                color = color,

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

        public Microsoft.Xna.Framework.Rectangle QuestDisplay(Journal.Quest.questTypes questType)
        {
            switch (questType)
            {
                case Journal.Quest.questTypes.challenge:

                    return DisplayRect(displays.quest);

                case Journal.Quest.questTypes.lesson:

                    return DisplayRect(displays.effect);

                case Journal.Quest.questTypes.miscellaneous:

                    return DisplayRect(displays.active);

                default:

                    return DisplayRect(displays.speech);

            }

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
            switch (impact)
            {

                case impacts.none:

                    return null;

                case impacts.impact:

                    additional.alpha = 0.85f;

                    return null;

                case impacts.glare:

                    additional.light = 0.01f;

                    additional.alpha = 0.85f;

                    break;

                case impacts.splatter:

                    size += 0.5f;

                    additional.color = SchemeColour(additional.scheme);

                    additional.alpha = 0.7f;

                    break;

                case impacts.puff:

                    size = Math.Max(Math.Min(5, size), 3);

                    break;

                case impacts.death:

                    additional.color = SchemeColour(additional.scheme);

                    additional.alpha = 0.65f;

                    break;

                case impacts.deathwhirl:

                    additional.color = SchemeColour(additional.scheme);

                    additional.alpha = 0.7f;

                    break;

                case impacts.clouds:

                    additional.girth = 2;

                    break;

                case impacts.bomb:

                    additional.alpha = 0.85f;

                    additional.light = 0f;

                    additional.layerOffset = 0.001f;

                    CreateImpact(location, origin, IconData.impacts.cinder, size + 2, additional);

                    additional.layerOffset = -0.001f;

                    CreateImpact(location, origin, IconData.impacts.puff, size + 1, additional);

                    additional.light = 0.005f;

                    additional.layerOffset = 0f;

                    return CreateImpact(location, origin, IconData.impacts.impact, size, additional);

                case impacts.combustion:

                    additional.light = 0f;

                    additional.layerOffset = 0.001f;

                    CreateImpact(location, origin, IconData.impacts.cinder, size + 1f, additional);

                    additional.light = 0.005f;

                    additional.layerOffset = 0f;

                    return CreateImpact(location, origin, IconData.impacts.puff, size + 0.5f, additional);

                case impacts.nature:

                    if(additional.scheme == schemes.none)
                    {

                        additional.scheme = schemes.weald;

                    }

                    additional.color = gradientColours[additional.scheme][2];

                    additional.light = 0f;

                    CreateImpact(location, origin, IconData.impacts.sparkle, size - 1, additional);

                    additional.color = gradientColours[additional.scheme][1];

                    additional.light = 0f;

                    CreateImpact(location, origin, IconData.impacts.sparkle, size - 2, additional);

                    additional.color = gradientColours[additional.scheme][0];

                    additional.light = 0.005f;

                    return CreateImpact(location, origin, IconData.impacts.sparkle, size, additional);

                case impacts.boltswirl:

                    additional.color = Microsoft.Xna.Framework.Color.White;

                    additional.light = 0f;

                    additional.alpha = 0.7f;

                    additional.interval = 100;

                    additional.layer = 997f;

                    CreateImpact(location, origin, IconData.impacts.spiral, 4f, additional);

                    additional.color = gradientColours[schemes.bolt][0];

                    additional.alpha = 0.4f;

                    additional.layer = 995f;

                    additional.light = 0.005f;

                    return CreateImpact(location, origin, IconData.impacts.spiral, 6f, additional);

                case impacts.deathbomb:

                    additional.alpha = 0.85f;

                    additional.light = 0f;

                    additional.layerOffset = 0.001f;

                    additional.color = gradientColours[schemes.fates][0];

                    CreateImpact(location, origin, IconData.impacts.cinder, size + 2, additional);

                    additional.layerOffset = -0.001f;

                    additional.color = gradientColours[schemes.fates][1];

                    CreateImpact(location, origin, IconData.impacts.puff, size + 1, additional);

                    additional.light = 0.005f;

                    additional.layerOffset = 0f;

                    additional.color = gradientColours[schemes.fates][2];

                    return CreateImpact(location, origin, IconData.impacts.death, size, additional);

            }

            return CreateImpact(location, origin, impact, size, additional);


        }

        public int ImpactIndex(impacts impact)
        {

            int Y = (Convert.ToInt32(impact) - 1);

            if (Y >= 6 && Y < 12)
            {
                Y -= 6;

            }
            else if (Y >= 12)
            {
                Y -= 12;

            }

            return Y;

        }

        public Texture2D ImpactSheet(impacts impact)
        {

            Texture2D sheet = impactsTexture;

            int Y = (Convert.ToInt32(impact) - 1);

            if (Y >= 6 && Y < 12)
            {

                sheet = impactsTextureTwo;
            }
            else if (Y >= 12)
            {

                sheet = impactsTextureThree;
            }

            return sheet;

        }

        public TemporaryAnimatedSprite CreateImpact(GameLocation location, Vector2 origin, impacts impact, float scale, ImpactAdditional additional)
        {

            Vector2 originOffset = origin + new Vector2(32f, 32f) - (new Vector2(32f * additional.girth, 32f) * scale );

            float interval = additional.interval;

            Microsoft.Xna.Framework.Color color = additional.color;

            float rotation = 0;

            if (additional.rotation > 0)
            {

                rotation = (float)(Math.PI / additional.rotation);

            }

            float layer = additional.layer;

            if (additional.layer == -1)
            {

                layer = originOffset.Y / 10000;

            }

            layer += additional.layerOffset;

            Texture2D sheet = ImpactSheet(impact);

            int Y = ImpactIndex(impact);

            Microsoft.Xna.Framework.Rectangle source = new((64) * additional.frame, Y * 64, 64 * additional.girth, 64);

            TemporaryAnimatedSprite bomb = new(0, interval,additional.frames, additional.loops, originOffset, false, false)
            {
                sourceRect = source,
                sourceRectStartingPos = new Vector2(source.X, source.Y),
                texture = sheet,
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

        public List<TemporaryAnimatedSprite> MissileConstruct(GameLocation location,missiles missile,Vector2 origin, int scale, int increments, float depth, schemes scheme)
        {

            List<TemporaryAnimatedSprite> missileAnimations = new();

            Microsoft.Xna.Framework.Rectangle rect = new(0, ((int)missile - 1) * 96, 96, 96);

            Vector2 setat = origin - (new Vector2(48, 48) * scale) + new Vector2(32, 32);

            int loops = (int)Math.Ceiling(increments * 0.5);

            int frames = 4;

            int interval = (increments * 250) / (loops * frames);

            Microsoft.Xna.Framework.Color coreColour = Microsoft.Xna.Framework.Color.White;

            Microsoft.Xna.Framework.Color schemeColour = SchemeColour(scheme);

            Microsoft.Xna.Framework.Color schemeLight = new(Math.Min((int)schemeColour.R + 32, 255), Math.Min((int)schemeColour.G + 32, 255), Math.Min((int)schemeColour.B + 32, 255));

            int trydarkR = (int)schemeColour.R - 48;

            int trydarkG = (int)schemeColour.G - 48;

            int trydarkB= (int)schemeColour.B - 48;

            Microsoft.Xna.Framework.Color schemeDark = new(trydarkR < 0 ? 0 : trydarkR, trydarkG < 0 ? 0 : trydarkG, trydarkB < 0 ? 0 : trydarkB);

            schemes rockScheme = schemes.rock;

            rockScheme = (schemes)((int)rockScheme + Mod.instance.randomIndex.Next(3));

            Microsoft.Xna.Framework.Color rockColour = SchemeColour(rockScheme);

            switch (missile)
            {

                case missiles.fireball:

                    missileAnimations.Add(MissileAnimation(location,missileIndexes.blazeCore1,setat,scale,interval,frames,loops,depth,coreColour,0.75f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeInner1, setat, scale, interval, frames, loops, depth, gradientColours[schemes.ember][0], 0.75f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeOuter1, setat, scale, interval, frames, loops, depth, gradientColours[schemes.ember][1], 0.75f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeOutline1, setat, scale, interval, frames, loops, depth, gradientColours[schemes.ember][2], 0.75f));

                    break;

                case missiles.meteor:

                    //lineColour = new(76, 72, 125);

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeCore1, setat, scale, interval, frames, loops, depth, coreColour, 1f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeInner1, setat, scale, interval, frames, loops, depth, gradientColours[schemes.ember][0], 1f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeOuter1, setat, scale, interval, frames, loops, depth, gradientColours[schemes.ember][1], 1f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeOutline1, setat, scale, interval, frames, loops, depth, gradientColours[schemes.ember][2], 1f));

                    missileIndexes meteorite = missileIndexes.meteor1;

                    meteorite = (missileIndexes)((int)meteorite + Mod.instance.randomIndex.Next(6));

                    Vector2 coreSet = origin - (new Vector2(48, 48) * (scale*0.75f)) + new Vector2(32, 32);

                    TemporaryAnimatedSprite meteor = MissileAnimation(location, meteorite, coreSet, scale * 0.75f, interval * frames * loops, 1, 1, depth + 0.0001f, rockColour, 1f);
                   
                    meteor.rotation = (float)Math.PI * 0.5f * Mod.instance.randomIndex.Next(4);

                    meteor.rotationChange = (float)Math.PI / 60;

                    missileAnimations.Add(meteor);

                    break;

                case missiles.rockfall:

                    missileIndexes rockfalling = missileIndexes.rock1;

                    rockfalling = (missileIndexes)((int)rockfalling + Mod.instance.randomIndex.Next(3));

                    TemporaryAnimatedSprite rockScatter = MissileAnimation(location, missileIndexes.scatter1, setat, scale, interval, frames, loops, depth, rockColour, 1f);

                    switch (Mod.instance.randomIndex.Next(2))
                    {

                        case 1: rockScatter.verticalFlipped = true; break;

                    }

                    missileAnimations.Add(rockScatter);

                    TemporaryAnimatedSprite rockCore = MissileAnimation(location, rockfalling, setat, scale, interval * frames * loops, 1, 1, depth + 0.0001f, rockColour, 1f);

                    rockCore.rotation = (float)Math.PI * 0.5f * Mod.instance.randomIndex.Next(4);

                    missileAnimations.Add(rockCore);

                    break;

                case missiles.slimeball:

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeCore1, setat, scale, interval, frames, loops, depth, coreColour, 0.75f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeInner1, setat, scale, interval, frames, loops, depth, schemeLight, 0.75f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.trail1, setat, scale, interval, frames, loops, depth, schemeColour, 0.75f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.trailOutline1, setat, scale, interval, frames, loops, depth, schemeDark, 0.75f));

                    break;

                case missiles.cannonball:

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeCore1, setat, scale, interval, frames, loops, depth, coreColour, 0.75f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeInner1, setat, scale, interval, frames, loops, depth, gradientColours[schemes.ember][0], 0.75f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeOuter1, setat, scale, interval, frames, loops, depth, gradientColours[schemes.ember][1], 0.75f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeOutline1, setat, scale, interval, frames, loops, depth, gradientColours[schemes.ember][2], 0.75f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.cannonball, setat, (int)(scale * 0.75f), interval * frames * loops, 1, 1, depth + 0.0001f, SchemeColour(schemes.death), 0.75f));

                    break;

                case missiles.death:

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeCore1, setat, scale, interval, frames, loops, depth, schemeDark, 0.9f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeInner1, setat, scale, interval, frames, loops, depth, schemeColour, 0.9f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeOuter1, setat, scale, interval, frames, loops, depth, schemeLight, 0.9f));

                    missileAnimations.Add(MissileAnimation(location, missileIndexes.blazeOutline1, setat, scale, interval, frames, loops, depth, coreColour, 0.9f));

                    TemporaryAnimatedSprite deathAnimation = MissileAnimation(location, missileIndexes.death, setat, (int)(scale * 0.9f), interval * frames * loops, 1, 1, depth + 0.0001f, coreColour, 0.9f);

                    deathAnimation.rotationChange = 0.0001f;

                    missileAnimations.Add(deathAnimation);

                    break;

                case missiles.whisk:

                    TemporaryAnimatedSprite whisk1 = MissileAnimation(location, missileIndexes.star3, setat, scale, interval * frames * loops, 1, 1, depth, gradientColours[schemes.fates][3], 0.8f);

                    whisk1.rotationChange = (float)(Math.PI / 60);

                    missileAnimations.Add(whisk1);

                    TemporaryAnimatedSprite whisk2 = MissileAnimation(location, missileIndexes.star2, setat, scale, interval * frames * loops, 1, 1, depth, gradientColours[schemes.fates][0], 0.8f);

                    whisk2.rotationChange = (float)(Math.PI / 60);

                    missileAnimations.Add(whisk2);

                    TemporaryAnimatedSprite whisk3 = MissileAnimation(location, missileIndexes.star1, setat, scale, interval * frames * loops, 1, 1, depth, gradientColours[schemes.fates][1], 0.8f);

                    whisk3.rotationChange = (float)(Math.PI / 60);

                    missileAnimations.Add(whisk3);

                    break;

            }

            return missileAnimations;

        }

        public TemporaryAnimatedSprite MissileAnimation(GameLocation location, missileIndexes missile, Vector2 origin, float scale, int interval, int frames, int loops,  float depth, Microsoft.Xna.Framework.Color color, float alpha)
        {

            Microsoft.Xna.Framework.Rectangle rect = new((int)missile % 4 * 96, (int)((int)missile / 4) * 96, 96, 96);

            TemporaryAnimatedSprite missileAnimation = new(0, interval, frames, loops, origin, false, false)
            {

                sourceRect = rect,

                sourceRectStartingPos = new Vector2(rect.X, rect.Y),

                texture = missileTexture,

                scale = scale,

                layerDepth = depth,

                alpha = alpha,

                color = color,

            };

            location.temporarySprites.Add(missileAnimation);

            return missileAnimation;

        }

        public List<TemporaryAnimatedSprite> EmberConstruct(GameLocation location, schemes scheme, Vector2 origin, float scale, int grade, int Time = 3, float layer = -1f)
        {

            List<TemporaryAnimatedSprite> emberAnimations = new();

            if(layer <= 0)
            {

                layer = origin.Y / 10000;

            }

            emberAnimations.Add(emberAnimation(location, origin, scale, grade, 0, gradientColours[scheme][2], layer, 0.65f, Time));

            emberAnimations.Add(emberAnimation(location, origin, scale, grade, 1, gradientColours[scheme][1], layer, 0.85f, Time));

            emberAnimations.Add(emberAnimation(location, origin, scale, grade, 2, gradientColours[scheme][0], layer, 0.85f, Time));

            emberAnimations.Add(emberAnimation(location, origin, scale, grade, 3, Microsoft.Xna.Framework.Color.White, layer, 0.85f, Time));

            return emberAnimations;

        }

        public TemporaryAnimatedSprite emberAnimation(GameLocation location, Vector2 origin, float scale, int grade, int part, Microsoft.Xna.Framework.Color color, float layer, float alpha, int Time)
        {

            TemporaryAnimatedSprite burnAnimation = new(0, 125, 4, Time * 2, origin + new Vector2(32) - (new Vector2(16) * scale), false, false)
            {

                sourceRect = new(0, (grade * 32) + (part * 96), 32, 32),

                sourceRectStartingPos = new(0, (grade * 32) + (part * 96)),

                texture = emberTexture,

                scale = scale,

                layerDepth = layer,

                alpha = alpha,

                color = color,

            };

            location.TemporarySprites.Add(burnAnimation);

            return burnAnimation;

        }

        public Microsoft.Xna.Framework.Rectangle RelicRectangles(relics relic)
        {

            if (relic == relics.none) { return new(); }

            int slot = Convert.ToInt32(relic) - 1;

            return new(slot % relicColumns * 20, slot / relicColumns * 20, 20, 20);

        }

        public List<TemporaryAnimatedSprite> BoltAnimation(GameLocation location, Vector2 origin, IconData.schemes scheme = schemes.bolt, int size = 2)
        {

            List<TemporaryAnimatedSprite> animations = new();

            float boltScale = 1.5f + 0.5f * size;

            int viewY = Game1.viewport.Y;

            int offset = 0;

            Vector2 originOffset = new((int)origin.X + 32 - (int)(32 * boltScale), (int)origin.Y + 48 - (int)(384 * boltScale));

            if ((int)originOffset.Y < viewY && (int)origin.Y - viewY >= 192)
            {

                offset = (int)((viewY - (int)originOffset.Y) / boltScale);

                originOffset.Y = viewY;

            }

            int randSet1 = 48 * Mod.instance.randomIndex.Next(4);

            bool flippit = (Game1.random.NextDouble() < 0.5) ? true : false;

            TemporaryAnimatedSprite bolt1 = new(0, 500, 1, 999, originOffset + new Vector2(-32 + Mod.instance.randomIndex.Next(5) * 16, 0), false, flippit)
            {

                sourceRect = new(randSet1, offset, 48, 385 - offset),

                sourceRectStartingPos = new Vector2(randSet1, offset),

                texture = boltTexture,

                layerDepth = 802f,

                scale = boltScale,

            };

            location.temporarySprites.Add(bolt1);

            animations.Add(bolt1);

            int randSet2 = 48 * Mod.instance.randomIndex.Next(4);

            TemporaryAnimatedSprite bolt2 = new(0, 500, 1, 999, originOffset + new Vector2(-32 + Mod.instance.randomIndex.Next(5) * 16, 0), false, (Game1.random.NextDouble() < 0.5) ? true : false)
            {

                sourceRect = new(192 + randSet2, offset, 48, 385 - offset),

                sourceRectStartingPos = new Vector2(192 + randSet2, offset),

                texture = boltTexture,

                layerDepth = 801f,

                scale = boltScale,

                color = gradientColours[scheme][0],

            };

            location.temporarySprites.Add(bolt2);

            animations.Add(bolt2);

            int randSet3 = 48 * Mod.instance.randomIndex.Next(4);

            TemporaryAnimatedSprite bolt3 = new(0, 500, 1, 999, originOffset, false, (Game1.random.NextDouble() < 0.5) ? true : false)
            {

                sourceRect = new(384 + randSet3, offset, 48, 385 - offset),

                sourceRectStartingPos = new Vector2(384 + randSet3, offset),

                texture = boltTexture,

                layerDepth = 800f,

                scale = boltScale,

                color = gradientColours[scheme][1],

            };

            location.temporarySprites.Add(bolt3);

            animations.Add(bolt3);

            // ---------------------- clouds

            if (size <= 1)
            {

                return animations;

            }

            TemporaryAnimatedSprite bolt4 = CreateImpact(
                location,
                originOffset + new Vector2(64, -64),
                impacts.clouds, 
                boltScale + 3.5f,
                new() {
                    color = gradientColours[scheme][2],
                    girth = 2,
                    layer = 803f,
                    interval = 200,
                    frames = 4,
                    alpha = 0.7f,
                });

            animations.Add(bolt4);

            TemporaryAnimatedSprite bolt5 = CreateImpact(
                location,
                originOffset + new Vector2(80, -32),
                impacts.clouds, 
                boltScale + 2.5f,
                new()
                {
                    color = gradientColours[scheme][0],
                    girth = 2,
                    layer = 804f,
                    interval = 200,
                    frames = 4,
                    alpha = 0.8f,
                });

            animations.Add(bolt5);

            TemporaryAnimatedSprite bolt6 = CreateImpact(
                location,
                originOffset + new Vector2(96, 0),
                impacts.clouds, 
                boltScale+0.5f,
                new()
                {
                    girth = 2,
                    layer = 805f,
                    interval = 200,
                    frames = 4,
                    alpha = 0.9f,
                });

            animations.Add(bolt6);

            return animations;

            /*Vector2 originOffset = new(origin.X - 64, origin.Y - 320);

            //Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, origin + new Vector2(0, -320), IconData.impacts.cloud, 1.5f, new() { interval = 75, color = Mod.instance.iconData.schemeColours[schemes.mists], flip = true, layer = 999f, });

            Mod.instance.iconData.ImpactIndicator(Game1.player.currentLocation, origin + new Vector2(0, -320), IconData.impacts.cloud, 4f, new() { interval = 125, frame = 3, color = Microsoft.Xna.Framework.Color.White, layer = 998f, alpha = 1f,});

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

            return;*/

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

        public TemporaryAnimatedSprite AnimateTarget(GameLocation location, Vector2 origin, IconData.schemes scheme, int counter)
        {

            Vector2 originOffset = origin - new Vector2(32, 8 * (counter < 3 ? counter : 6 - counter));

            Vector2 animationMotion = new Vector2(0, counter < 3 ? -0.008f : 0.008f);

            Microsoft.Xna.Framework.Rectangle cursorRect = CursorRect(cursors.target);

            TemporaryAnimatedSprite targetAnimation = new(0, 1000f, 1, 1, originOffset, false, false)
            {

                sourceRect = cursorRect,

                sourceRectStartingPos = new Vector2(cursorRect.X, cursorRect.Y),

                texture = cursorTexture,

                layerDepth = 999f,

                scale = 2f,

                motion = animationMotion,

                timeBasedMotion = true,

                color = SchemeColour(scheme),

            };

            location.temporarySprites.Add(targetAnimation);

            return targetAnimation;

        }
        
        public TemporaryAnimatedSprite StatusIndicator(GameLocation location, Vector2 origin, displays Display)
        {

            Microsoft.Xna.Framework.Rectangle statusRect = DisplayRect(Display);

            TemporaryAnimatedSprite statusAnimation = new(0, 1000, 1, 1, origin+ new Vector2(8,0), false, false)
            {

                sourceRect = statusRect,

                sourceRectStartingPos = new Vector2(statusRect.X, statusRect.Y),

                texture = displayTexture,

                layerDepth = 800f,

                scale = 3f,

                alpha = 0.5f,

            };

            location.temporarySprites.Add(statusAnimation);

            return statusAnimation;

        }

        public TemporaryAnimatedSprite AnimateWarpStrike(GameLocation location, Vector2 origin, int direction, int alternative = 0)
        {

            Vector2 offset = origin - new Vector2(96, 96);

            bool flip = false;

            Microsoft.Xna.Framework.Rectangle source = new(0, 0, 64, 64);

            switch (direction)
            {

                case 0:

                    offset.Y += 96;

                    source.Y += 256;

                    flip = true;

                    break;

                case 1:

                    offset.X -= 64;

                    offset.Y += 64;

                    source.Y += 128;

                    break;

                case 2:

                    offset.X -= 96;

                    source.Y += 64;

                    break;

                case 3:

                    offset.X -= 64;

                    offset.Y -= 96;

                    break;

                case 4:

                    offset.Y -= 96;

                    source.Y += 256;

                    break;

                case 5:

                    offset.X += 64;

                    offset.Y -= 96;

                    flip = true;

                    break;

                case 6:

                    offset.X += 96;

                    source.Y += 192;

                    flip = true;

                    break;

                case 7:

                    offset.X += 64;

                    offset.Y += 64;

                    flip = true;

                    source.Y += 128;

                    break;

            }

            TemporaryAnimatedSprite strike = new(0, 75, 4, 1, offset, false, flip)
            {
                sourceRect = source,
                sourceRectStartingPos = new Vector2(source.X, source.Y),
                texture = warpstrikeTexture,
                scale = 4f,
                layerDepth = 0.8f,
                alpha = 0.75f,
            };

            location.temporarySprites.Add(strike);

            return strike;

        }

        public TemporaryAnimatedSprite AnimateWarpSlash(GameLocation location, Vector2 origin)
        {

            Vector2 impact = origin - new Vector2(32, 32);

            Microsoft.Xna.Framework.Rectangle swipe = new(0, 320, 64, 64);

            int frames = 3;

            switch (Mod.instance.randomIndex.Next(3))
            {

                case 2:

                    swipe.X = 6 * 64;

                    frames = 2;

                    break;

                case 1:

                    swipe.X = 3 * 64;

                    break;

            }

            TemporaryAnimatedSprite slash = new(0, 125, frames, 1, impact, false, false)
            {
                sourceRect = swipe,
                sourceRectStartingPos = new Vector2(swipe.X, swipe.Y),
                texture = impactsTextureTwo,
                scale = 2f,
                layerDepth = 0.9f,
                alpha = 0.75f,
                timeBasedMotion = true,
                delayBeforeAnimationStart = 200,
            };

            location.temporarySprites.Add(slash);

            swipe.X += (frames - 1) * 64;

            TemporaryAnimatedSprite slashTwo = new(0, 1000, 1, 1, impact, false, false)
            {
                sourceRect = swipe,
                sourceRectStartingPos = new Vector2(swipe.X, swipe.Y),
                texture = impactsTextureTwo,
                scale = 2f,
                layerDepth = 0.9f,
                alpha = 0.75f,
                alphaFade = 0.000375f,
                motion = new(0.016f, 0.016f),
                timeBasedMotion = true,
                delayBeforeAnimationStart = 200 + (frames * 125),
            };

            location.temporarySprites.Add(slashTwo);

            return slash;

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

        public IconData.schemes scheme = IconData.schemes.none;

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

        public int frames = 8;

        public float interval = 100f;

        public int girth = 1;

        public int loops = 1;

        public float alpha = 0.5f;

        public float rotation = 0;

        public Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White;

        public int delay = 0;

        public float light = 0.03f;

        public bool flip = false;

        public float layer = -1;

        public float layerOffset = 0f;

        public IconData.schemes scheme = IconData.schemes.none;

    }

    public class SkyAdditional
    {

        public float interval = 1000f;

        public float alpha = 0.75f;

        public int delay = 0;

        public int loops = 1;

    }
}
