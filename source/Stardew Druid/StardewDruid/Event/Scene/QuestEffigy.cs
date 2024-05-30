/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Cast.Mists;
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;


namespace StardewDruid.Event.Scene
{
    public class QuestEffigy : EventHandle
    {

        public StardewDruid.Monster.Blobfiend blobking;

        public Vector2 beachWarp;

        public Vector2 campFire;

        public Vector2 bridgeVector;

        public Vector2 blobVector;

        public Vector2 atollVector;

        public Vector2 mistVector;

        public QuestEffigy()
        {

            mainEvent = true;

            expireIn = 600;

        }

        public override void EventActivate()
        {

            if (!Mod.instance.characters.ContainsKey(CharacterData.characters.Effigy))
            {

                return;

            }

            base.EventActivate();

            beachWarp = new Vector2(10f, 15f);

            campFire = new Vector2(48, 20);

            bridgeVector = new Vector2(55, 13);

            blobVector = new Vector2(75, 8);

            atollVector = new Vector2(92, 7);

            mistVector = new Vector2(18, 15);

            narrators = new()
            {
                [0] = new("The Effigy", Microsoft.Xna.Framework.Color.Green),
                [1] = new("The Jellyking", Microsoft.Xna.Framework.Color.OrangeRed),
                [2] = new("First Farmer", Microsoft.Xna.Framework.Color.DarkGreen),
                [3] = new("Lady Beyond", Microsoft.Xna.Framework.Color.Blue),
            }; ;

            companions[0] = Mod.instance.characters[CharacterData.characters.Effigy] as StardewDruid.Character.Effigy;

            voices[0] = companions[0];

            ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 600);

            location.playSound("discoverMineral");

        }

        public override void EventAbort()
        {

            companions[0].SwitchToMode(Character.Character.mode.random, Game1.player);

            //(Mod.instance.locations[LocationData.druid_atoll_name] as Atoll).ambientDarkness = false;

            if(companions.Count > 1)
            {

                companions[2].currentLocation.characters.Remove(companions[2]);
                companions[3].currentLocation.characters.Remove(companions[3]);

                companions.Remove(2);
                companions.Remove(3);

            }

            base.EventAbort();

        }

        public override void EventInterval()
        {

            if (Game1.activeClickableMenu != null)
            {

                expireTime++;

                return;

            }

            activeCounter++;

            switch (activeCounter)
            {

                // ------------------------------------------
                // 1 Beginning
                // ------------------------------------------

                case 1:

                    companions[0].SwitchToMode(Character.Character.mode.scene, Game1.player);

                    companions[0].netDirection.Set(3);

                    companions[0].eventName = eventId;

                    if (companions[0].currentLocation.Name != Mod.instance.rite.castLocation.Name)
                    {
                        companions[0].currentLocation.characters.Remove(companions[0]);

                        companions[0].currentLocation = Mod.instance.rite.castLocation;
                        
                        companions[0].currentLocation.characters.Add(companions[0]);
                    
                    }

                    companions[0].Position = beachWarp*64;

                    Mod.instance.iconData.AnimateQuickWarp(Mod.instance.rite.castLocation, companions[0].Position - new Vector2(0.0f, 32f));

                    DialogueCue(0, "A great day for the beach");

                    //activeCounter = 300;

                    break;

                case 3:

                    companions[0].netStandbyActive.Set(true);

                    DialogueCue(0, "As my old friend would say");

                    break;

                case 4:

                    Mod.instance.CastMessage("Talk to the Effigy when '...' appears");

                    break;

                case 6:

                    DialogueLoad(0, 1);

                    DialogueCue(0, "..."); break;

                case 9: DialogueCue(0, "..."); break;
                case 12: DialogueCue(0, "..."); break;
                case 15: DialogueCue(0, "..."); break;
                case 18: activeCounter = 100; break;


                // ------------------------------------------
                // 2 Fish surprise
                // ------------------------------------------

                case 101: // clear

                    DialogueClear(0);

                    companions[0].netStandbyActive.Set(false);

                    break;

                case 102:

                    DialogueCue(0, "Time to reminisce");

                    companions[0].TargetEvent(110, companions[0].Position + new Vector2(-128, 0), true);

                    break;

                case 112:

                    DialogueCue(0, "With an old angler technique");

                    companions[0].netSpecialActive.Set(true);

                    companions[0].specialTimer = 60;

                    Mod.instance.iconData.DecorativeIndicator(location, companions[0].Position, IconData.decorations.weald, 3f, new() { interval = 1200, });

                    break;

                case 113:


                    location.playSound("discoverMineral", null, 800);

                    Mod.instance.iconData.ImpactIndicator(location, (beachWarp - new Vector2(6, 0)) * 64, IconData.impacts.nature, 4f, new());

                    break;

                case 115: DialogueCue(0, "?"); break;

                case 118:

                    DialogueCue(0, "DENIZENS OF THE SHALLOWS, HEED MY VOICE");

                    companions[0].netSpecialActive.Set(true);

                    companions[0].specialTimer = 60;

                    break;

                case 119:

                    location.playSound("discoverMineral");

                    List<Vector2> vectors25 = new()
                    {

                        new(6,0),
                        new(8,2),
                        new(7,4),
                        new(7,-4),
                        new(9,-2),
                    };

                    Mod.instance.iconData.DecorativeIndicator(location, companions[0].Position, IconData.decorations.weald, 3f, new() { interval = 1200, });

                    for (int i = 0; i < vectors25.Count; i++)
                    {

                        Mod.instance.iconData.ImpactIndicator(location, (beachWarp - vectors25[i]) * 64, IconData.impacts.nature, 4f, new());

                    }

                    break;

                case 121:

                    DialogueCue(0, "!");

                    break;

                case 122:

                    companions[0].TargetEvent(140, (campFire * 64) - new Vector2(0, 128), true);

                    companions[0].netDirection.Set(3);

                    location.playSound("pullItemFromWater");

                    Mod.instance.iconData.ImpactIndicator(location, companions[0].Position - new Vector2(256, 0), IconData.impacts.fish, 3f, new());

                    new ThrowHandle(
                        companions[0].Position - new Vector2(256, 0),
                        companions[0].Position,
                        new StardewValley.Object("147", 1))
                    { pocket = true }.register();

                    break;

                case 123:
                case 124:
                case 125:

                    if (activeCounter == 124)
                    {

                        DialogueCue(0, "Hasten away!");

                        companions[0].netDirection.Set(1);

                    }

                    location.playSound("pullItemFromWater");

                    List<Vector2> vectors28 = new()
                    {

                        new(6,0),
                        new(8,2),
                        new(7,4),
                        new(7,-4),
                        new(9,-2),
                    };

                    Vector2 position28 = origin;

                    for (int i = 0; i < vectors28.Count; i++)
                    {

                        Mod.instance.iconData.ImpactIndicator(location, (beachWarp - vectors28[i]) * 64, IconData.impacts.fish, 3f, new());

                        new ThrowHandle(
                            origin - (vectors28[i] * 64),
                            companions[0].Position - new Vector2(256, 128) + new Vector2(Mod.instance.randomIndex.Next(16) * 32, Mod.instance.randomIndex.Next(8) * 32),
                            new StardewValley.Object("147", 1)
                        )
                        { pocket = true }.register();

                        new ThrowHandle(
                            origin - (vectors28[i] * 64),
                            companions[0].Position - new Vector2(256, 128) + new Vector2(Mod.instance.randomIndex.Next(16) * 32, Mod.instance.randomIndex.Next(8) * 32),
                            new StardewValley.Object("147", 1)
                        )
                        { pocket = true }.register();

                    }

                    break;

                case 141:

                    DialogueLoad(0, 2);

                    DialogueCue(0, "...");

                    break;

                case 144: DialogueCue(0, "..."); break;
                case 147: DialogueCue(0, "..."); break;
                case 150: DialogueCue(0, "..."); break;
                case 153: DialogueCue(0, "..."); break;
                case 156: activeCounter = 200; break;


                // ------------------------------------------
                // 3 Fish stew
                // ------------------------------------------

                case 201:

                    DialogueClear(0);

                    DialogueCue(0, "Let us make a fish stew");

                    Vector2 cursor56 = campFire * 64;

                    companions[0].LookAtTarget(cursor56, true);

                    break;

                case 203:

                    companions[0].netSpecialActive.Set(true);

                    companions[0].specialTimer = 60;

                    Mod.instance.iconData.DecorativeIndicator(location, companions[0].Position, IconData.decorations.mists, 3f, new() { interval = 1200, });

                    break;

                case 204:

                    if (!location.objects.ContainsKey(campFire))
                    {
                        Torch camp = new("278", true)
                        {
                            Fragility = 1,
                            destroyOvernight = true
                        };

                        location.objects.Add(campFire, camp);

                    }

                    Vector2 cursor57 = campFire * 64;

                    Mod.instance.iconData.CursorIndicator(location, cursor57, IconData.cursors.mists, new());

                    Mod.instance.iconData.AnimateBolt(location, cursor57);

                    break;

                case 205:

                    DialogueCue(0, "I would often prepare this for friends");

                    Game1.playSound("fireball");

                    Mod.instance.iconData.ImpactIndicator(location, campFire * 64, IconData.impacts.impact, 3f, new());

                    break;

                case 206:

                    companions[0].netSpecialActive.Set(true);

                    companions[0].specialTimer = 60;

                    Mod.instance.iconData.DecorativeIndicator(location, companions[0].Position, IconData.decorations.mists, 3f, new() { interval = 1200, });

                    break;

                case 207:

                    Vector2 cursor60 = campFire * 64;

                    Mod.instance.iconData.CursorIndicator(location, cursor60, IconData.cursors.mists, new());

                    Mod.instance.iconData.AnimateBolt(location, campFire * 64);

                    break;

                case 208:

                    DialogueCue(0, "BY THE POWER BEYOND THE SHORE");

                    break;

                case 209:

                    Game1.playSound("fireball");

                    Mod.instance.iconData.ImpactIndicator(location, campFire * 64, IconData.impacts.impact, 3f, new());

                    break;

                case 210:

                    companions[0].netSpecialActive.Set(true);

                    companions[0].specialTimer = 60;

                    Mod.instance.iconData.DecorativeIndicator(location, companions[0].Position, IconData.decorations.mists, 3f, new() { interval = 1200, });

                    break;

                case 211:

                    Vector2 cursor64 = campFire * 64;

                    Mod.instance.iconData.CursorIndicator(location, cursor64, IconData.cursors.mists, new());

                    Mod.instance.iconData.AnimateBolt(location, cursor64);

                    break;

                case 213:

                    DialogueCue(0, "Perfection");

                    new ThrowHandle(campFire * 64, campFire * 64 - new Vector2(128, 0), new StardewValley.Object("728", 1)).register();

                    Game1.playSound("fireball");

                    Mod.instance.iconData.ImpactIndicator(location, campFire * 64, IconData.impacts.impact, 5f, new());

                    break;

                case 214:

                    new ThrowHandle(campFire * 64 - new Vector2(128, 0), campFire * 64 - new Vector2(256, 0), new StardewValley.Object("728", 1)).register();

                    Game1.playSound("fireball");

                    Mod.instance.iconData.ImpactIndicator(location, (campFire - new Vector2(2, 0)) * 64, IconData.impacts.impact, 3f, new());

                    break;

                case 215:

                    new ThrowHandle(campFire * 64 - new Vector2(256, 0), campFire * 64 - new Vector2(384, 0), new StardewValley.Object("728", 1)) { pocket = true }.register();

                    Game1.playSound("fireball");

                    Mod.instance.iconData.ImpactIndicator(location, (campFire - new Vector2(4, 0)) * 64, IconData.impacts.impact, 3f, new());

                    break;

                case 216:

                    DialogueLoad(0, 3);

                    DialogueCue(0, "...");

                    Game1.playSound("fireball");

                    Mod.instance.iconData.ImpactIndicator(location, (campFire - new Vector2(6, 0)) * 64, IconData.impacts.impact, 4f, new());

                    break;

                case 219: DialogueCue(0, "..."); break;
                case 222: DialogueCue(0, "..."); break;
                case 225: DialogueCue(0, "..."); break;
                case 228: activeCounter = 250; break;

                // ------------------------------------------
                // 3.5 Position before Jellyking
                // ------------------------------------------

                case 251:

                    DialogueClear(0);

                    companions[0].TargetEvent(255, bridgeVector*64, true);

                    companions[0].TargetEvent(260, (bridgeVector + new Vector2(12,0))*64, false);

                    companions[0].TargetEvent(300, (bridgeVector + new Vector2(14, -2))*64, false);

                    break;

                case 252:

                    DialogueCue(0, "This stream is a new feature");

                    break;


                // ------------------------------------------
                // 4 Jellyking
                // ------------------------------------------

                case 301:

                    DialogueClear(0);

                    break;

                case 302:

                    companions[0].netStandbyActive.Set(true);

                    DialogueCue(0, "We would often gaze at the sky"); 
                    
                    break;

                case 305: 
                    
                    DialogueCue(0, "That big cloud might have once been a dragon"); 
                    
                    break;

                case 308: 
                    
                    DialogueCue(0, "Things are simpler now"); 
 
                    blobking = new(blobVector, Mod.instance.CombatDifficulty());

                    blobking.netScheme.Set(2);

                    blobking.SetMode(3);

                    blobking.netPosturing.Set(true);

                    blobking.netDirection.Set(2);

                    blobking.netAlternative.Set(3);

                    location.characters.Add(blobking);

                    blobking.update(Game1.currentGameTime, location);

                    voices[1] = blobking;

                    break;

                case 311: DialogueCue(1, "Ha ha ha. The wooden puppet returns."); break;

                case 313:

                    companions[0].netStandbyActive.Set(false);

                    companions[0].LookAtTarget(blobking.Position);

                    break;

                case 314: DialogueCue(0, "Jellyking. The wretched fiend"); break;
                case 317: DialogueCue(1, "I heard your creaky, broken voice"); break;
                case 320: DialogueCue(1, "And came to laugh at you"); break;
                case 323: DialogueCue(0, "Have you no fear?"); break;
                case 326: DialogueCue(1, "Your friends are gone, your power spent"); break;
                case 329: DialogueCue(1, "You can no longer guard the change"); break;

                case 332:

                    DialogueCue(0, "Enough of you!");

                    companions[0].netSmashActive.Set(true);

                    companions[0].TargetEvent(340, blobking.Position - new Vector2(64, 0), true);

                    SetTrack("Cowboy_undead");

                    break;

                //340 blobking fight triggered

                case 341:

                    DialogueCue(1, "!");

                    blobking.netPosturing.Set(false);

                    blobking.DamageToFarmer = 1;

                    blobking.Health = 9999;

                    blobking.MaxHealth = 9999;

                    companions[0].SwitchToMode(Character.Character.mode.track, Game1.player);

                    break;

                case 345:

                    DialogueCue(1, "Creak creak creak goes the little wooden man");

                    break;

                case 348:

                    DialogueCue(0, "You are nothing but a slimey abberation");

                    break;

                case 351:

                    DialogueCue(0, "How have you survived so many long, barren winters");

                    break;

                case 354:

                    DialogueCue(1, "I have been resurrected by a new power, a hungry power");

                    break;

                case 357:

                    DialogueCue(0, "Face my judgement, Jelly-fiend");

                    break;

                case 359:

                    companions[0].SwitchToMode(Character.Character.mode.scene, Game1.player);

                    companions[0].netSpecialActive.Set(true);

                    companions[0].specialTimer = 90;

                    companions[0].LookAtTarget(blobking.Position, true);

                    Mod.instance.iconData.DecorativeIndicator(location, companions[0].Position, IconData.decorations.stars, 3f, new() { interval = 1200, });

                    SpellHandle meteor = new(location, blobking.Position, Game1.player.Position);

                    meteor.type = SpellHandle.spells.orbital;

                    meteor.projectile = 4;

                    meteor.scheme = IconData.schemes.stars;

                    Mod.instance.spellRegister.Add(meteor);

                    DialogueCue(1, "Ha ha ha. Too slow, scarecrow.");

                    blobking.ResetActives();

                    blobking.netAlternative.Set(1);

                    blobking.PerformFlight(new(blobking.Position.X + 1280, blobking.Position.Y + 192),0);

                    break;

                case 361:

                    DialogueLoad(0, 4);

                    DialogueCue(0, "...");

                    companions[0].netDirection.Set(2);

                    location.characters.Remove(blobking);

                    blobking = null;

                    voices.Remove(1);

                    Game1.stopMusicTrack(MusicContext.Default);

                    break;


                case 364: DialogueCue(0, "..."); break;
                case 367: DialogueCue(0, "..."); break;
                case 370: DialogueCue(0, "..."); break;
                case 373: activeCounter = 400; break;

                // ------------------------------------------
                // 4.5 Transfer to Atoll
                // ------------------------------------------

                case 401:

                    DialogueClear(0);

                    companions[0].TargetEvent(420, atollVector * 64, true);

                    break;

                case 402:

                    DialogueCue(0, "I need a space to reflect");

                    break;

                case 421:

                    Game1.warpFarmer(LocationData.druid_atoll_name, 5, 10, 1);

                    Game1.xLocationAfterWarp = 5;

                    Game1.yLocationAfterWarp = 10;

                    inabsentia = true;

                    location = Mod.instance.locations[LocationData.druid_atoll_name];

                    (Mod.instance.locations[LocationData.druid_atoll_name] as Atoll).ambientDarkness = true;

                    companions[0].currentLocation.characters.Remove(companions[0]);

                    companions[0].currentLocation = Mod.instance.locations[LocationData.druid_atoll_name];

                    companions[0].currentLocation.characters.Add(companions[0]);

                    companions[0].Position = new Vector2(6,11) * 64;

                    break;

                case 422:
                case 423:
                case 424:
                case 425:

                    if(Game1.player.currentLocation.Name == LocationData.druid_atoll_name)
                    {

                        inabsentia = false;

                        activeCounter = 450;

                    }

                    break;

                case 426:

                    inabsentia = false;

                    activeCounter = 450;

                    break;

                // ------------------------------------------
                // 5 Wisps
                // ------------------------------------------

                case 451:
                    
                    Game1.ambientLight = new(64, 0, 0);

                    companions[0].TargetEvent(455, mistVector * 64, true);

                    break;

                case 452:

                    DialogueCue(0, "This is the shore I remember");

                    Wisps wispNew = new();

                    wispNew.EventSetup(mistVector * 64, "wisps");

                    wispNew.eventLocked = true;

                    wispNew.EventActivate();

                    wispNew.AddWisps(1, 120);

                    wispNew.AddWisps(3, 120);

                    wispNew.AddWisps(5, 120);

                    wispNew.AddWisps(7, 120);

                    break;

                case 455: DialogueCue(0, "We are not alone on this lonely atoll"); break;

                case 458:

                    DialogueLoad(0, 5);

                    DialogueCue(0, "...");

                    break;

                case 461: DialogueCue(0, "..."); break;
                case 464: DialogueCue(0, "..."); break;
                case 467: DialogueCue(0, "..."); break;
                case 470: activeCounter = 500; break;

                // ------------------------------------------
                // 6 First Farmer / Lady Beyond
                // ------------------------------------------

                case 501:

                    DialogueClear(0);

                    break;

                case 502:

                    DialogueCue(0, "It seems the wisps would remind me of something");

                    break;

                case 504:

                    companions[0].netDirection.Set(0);

                    break;

                case 505:

                    DialogueCue(0, "A fragment of the past");

                    companions[0].netSpecialActive.Set(true);

                    companions[0].specialTimer = 60;

                    break;

                case 506:

                    Vector2 mistCorner = mistVector * 64 - new Vector2(72*4,72*5);

                    List<int> corners = new() { 0, 6,};

                    for (int i = 0; i < 7; i++)
                    {

                        for (int j = 0; j < 7; j++)
                        {

                            if (corners.Contains(i) && corners.Contains(j))
                            {
                                continue;
                            }

                            Vector2 glowVector = mistCorner + new Vector2(i * 72, j * 72);

                            TemporaryAnimatedSprite glowSprite = new TemporaryAnimatedSprite(0, 5000f, 1, 13, glowVector, false, false)
                            {
                                sourceRect = new Microsoft.Xna.Framework.Rectangle(88, 1779, 30, 30),
                                sourceRectStartingPos = new Vector2(88, 1779),
                                texture = Game1.mouseCursors,
                                motion = new Vector2(-0.0004f + Mod.instance.randomIndex.Next(5) * 0.0002f, -0.0004f + Mod.instance.randomIndex.Next(5) * 0.0002f),
                                scale = 4f,
                                layerDepth = 991f,
                                timeBasedMotion = true,
                                alpha = 0.5f,
                                color = new Microsoft.Xna.Framework.Color(0.75f, 0.75f, 1f, 1f),
                            };

                            location.temporarySprites.Add(glowSprite);

                        }

                    }

                    companions[2] = new Cuchulan(CharacterData.characters.Cuchulan);

                    voices[2] = companions[2];

                    companions[2].SwitchToMode(Character.Character.mode.scene, Game1.player);

                    companions[2].netDirection.Set(1);

                    if (companions[2].currentLocation.Name != Mod.instance.rite.castLocation.Name)
                    {
                        companions[2].currentLocation.characters.Remove(companions[2]);

                        companions[2].currentLocation = location;

                        companions[2].currentLocation.characters.Add(companions[2]);

                    }

                    companions[2].Position = (mistVector + new Vector2(-2, -3)) * 64;

                    /*AddActor(2, farmerVector);

                    voices[2] = actors[2];

                    TemporaryAnimatedSprite farmerSprite = new TemporaryAnimatedSprite(0, 5000f, 1, 12, farmerVector - new Vector2(32, 0), false, false)
                    {
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32),
                        sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "FirstFarmer.png")),
                        scale = 4f,
                        layerDepth = 992f,
                        alpha = 0.5f,
                    };
                    location.temporarySprites.Add(farmerSprite);*/

                    companions[3] = new Morrigan(CharacterData.characters.Morrigan);

                    voices[3] = companions[3];

                    companions[3].SwitchToMode(Character.Character.mode.scene,Game1.player);

                    companions[3].netDirection.Set(1);

                    companions[3].flip = true;

                    if (companions[3].currentLocation.Name != Mod.instance.rite.castLocation.Name)
                    {
                        companions[3].currentLocation.characters.Remove(companions[3]);

                        companions[3].currentLocation = location;

                        companions[3].currentLocation.characters.Add(companions[3]);

                    }

                    companions[3].Position = (mistVector + new Vector2(2, -2)) * 64;


                    /*AddActor(3, ladyVector);

                    voices[3] = actors[3];

                    TemporaryAnimatedSprite ladySprite = new TemporaryAnimatedSprite(0, 5000f, 1, 11, ladyVector - new Vector2(32, 0), false, false)
                    {
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32),
                        sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "LadyBeyond.png")),
                        scale = 4f,
                        layerDepth = 992f,
                        alpha = 0.5f,
                        flipped = true,

                    };
                    location.temporarySprites.Add(ladySprite);*/

                    break;


                case 508: 
                    DialogueCue(3, "You seem upset"); companions[3].netSpecialActive.Set(true); companions[3].specialTimer = 90;
                    break;
                case 511: 
                    DialogueCue(2, "It's just a bit... sudden."); companions[2].netSpecialActive.Set(true); companions[2].specialTimer = 90;
                    break;
                case 514: 
                    DialogueCue(3, "The health of my kin deteriorates."); companions[3].netSpecialActive.Set(true); companions[3].specialTimer = 90;
                    break;
                case 517: 
                    DialogueCue(3, "I would have realised sooner...");
                    break;
                case 520: 
                    DialogueCue(3, "...had I not been... distracted"); 
                    break;
                case 523: 
                    DialogueCue(2, "So you'll go across the sea then"); companions[2].netSpecialActive.Set(true); companions[2].specialTimer = 90;
                    break;
                case 526: 
                    DialogueCue(3, "I must care for them in their slumber"); companions[3].netSpecialActive.Set(true); companions[3].specialTimer = 90; 
                    break;
                case 529: 
                    DialogueCue(2, "What about the valley, our circle?"); companions[2].netSpecialActive.Set(true); companions[2].specialTimer = 90;
                    break;
                case 532: 
                    DialogueCue(3, "You have talents, space... and our friend"); companions[3].netSpecialActive.Set(true); companions[3].specialTimer = 90;
                    break;
                case 535: 
                    DialogueCue(2, "He needs you more than he does me"); companions[2].netSpecialActive.Set(true); companions[2].specialTimer = 90; 
                    break;
                case 538: 
                    DialogueCue(3, "You must continue to mentor him"); companions[3].netSpecialActive.Set(true); companions[3].specialTimer = 90;
                    break;
                case 541: 
                    DialogueCue(3, "Keep him safe from the Fates");
                    break;
                case 544: 
                    DialogueCue(3, "Show him the beauty of the valley"); 
                    break;
                case 547: 
                    DialogueCue(2, "It's not enough for me"); companions[2].netSpecialActive.Set(true); companions[2].specialTimer = 90;
                    break;
                case 550: 
                    DialogueCue(3, "??");
                    break;
                case 553: 
                    DialogueCue(2, "Please. Stay."); companions[2].netSpecialActive.Set(true); companions[2].specialTimer = 90;
                    break;
                case 556: 
                    DialogueCue(3, "I have given you all the time I can"); companions[3].netSpecialActive.Set(true); companions[3].specialTimer = 90;
                    break;
                case 559: 
                    DialogueCue(3, "Goodbye, farmer");
                    companions[3].netDirection.Set(2);
                    break;
                case 561: 
                    DialogueCue(2, "Lady...");
                    companions[2].netDirection.Set(2);
                    companions[3].currentLocation.characters.Remove(companions[3]);
                    companions.Remove(3);
                    break;
                case 564: 
                    DialogueCue(0, "He was never the same after this");
                    companions[2].currentLocation.characters.Remove(companions[2]);
                    companions.Remove(2);

                    break;
                case 567:
                    DialogueCue(0, "Will I ever understand why?");
                    break;
                case 570:

                    DialogueCue(0, "...");

                    DialogueLoad(0, 6);

                    break;

                case 573: DialogueCue(0, "..."); break;
                case 576: DialogueCue(0, "..."); break;
                case 579: DialogueCue(0, "..."); break;
                case 582: activeCounter = 600; break;

                // ------------------------------------------
                // 7 Ending
                // ------------------------------------------

                case 601:

                    DialogueClear(0);

                    DialogueCue(0, "This is for you.");

                    ThrowHandle throwRelic = new(Game1.player, companions[0].Position, IconData.relics.effigy_crest);

                    throwRelic.register();

                    Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.effigy_crest.ToString());

                    break;

                case 604:

                    DialogueCue(0, "At least this I know after the many years...");

                    break;

                case 607:

                    DialogueCue(0, "The valley is beautiful");

                    break;

                case 609:

                    companions[0].netDirection.Set(1);

                    Mod.instance.questHandle.CompleteQuest(eventId);

                    EventQuery();

                    companions[0].SwitchToMode(Character.Character.mode.random, Game1.player);

                    expireEarly = true;

                    //(Mod.instance.locations[LocationData.druid_atoll_name] as Atoll).ambientDarkness = false;

                    break;


            }

        }

        public override void EventScene(int index)
        {

            switch (index)
            {

                case 100: // first dialogue

                    activeCounter = Math.Max(100, activeCounter);

                    break;

                case 110: // cast position

                    activeCounter = 110;

                    break;

                case 140: // camp position

                    activeCounter = 140;

                    break;

                case 200: // second dialogue

                    activeCounter = Math.Max(200, activeCounter);

                    break;

                case 250:

                    activeCounter = Math.Max(250, activeCounter);

                    break;

                case 300: // third dialogue

                    activeCounter = Math.Max(300, activeCounter);

                    break;

                case 340: // Jellyking position

                    activeCounter = 340;

                    break;

                case 400: // fourth dialogue

                    activeCounter = Math.Max(400, activeCounter);

                    break;

                case 420: // mist position

                    activeCounter = 420;

                    break;

                case 500: // fifth dialogue

                    activeCounter = Math.Max(500, activeCounter);

                    break;

                case 600: // six dialogue

                    activeCounter = Math.Max(600, activeCounter);

                    break;

            }

        }


        public override void DialogueSetups(StardewDruid.Character.Character npc, int dialogueId)
        {

            string intro;

            switch (dialogueId)
            {
                default: // beachOne

                    intro = "Here again. At the hem of the valley.";

                    break;

                case 2:

                    intro = "I've only ever possessed a small amount of talent in invoking the energies of the Weald.";

                    break;

                case 3:

                    intro = "That peculiar cooking technique was taught to me by the Lady herself. " +
                        "It was a favourite of the first farmer's. He was her champion.";

                    break;

                case 4:

                    intro = "That menace. I swear that when I find the means, I will turn him to juice and rind. I always react poorly to his words.";

                    break;

                case 5:

                    intro = "The rolling energies of the mists gather here."; 

                    break;

                case 6:

                    intro = "I witnessed this, many ages ago, when I was still new to this world. " +
                        "I was unable to assist my friend with his troubles, and I was unable to advance the cause of the circle on my own.";

                    break;


            }

            List<Response> responseList = new();

            switch (dialogueId)
            {

                default: //beachOne

                    responseList.Add(new Response("1a", "Is the beach how you remember it?"));

                    break;

                case 2:

                    responseList.Add(new Response("2a", "I thought what you just did was great."));
                    responseList.Add(new Response("2a", "I'm surprised you consider yourself untalented in the Weald."));

                    break;

                case 3:

                    responseList.Add(new Response("3a", "You once told me the first farmer knew her."));
                    responseList.Add(new Response("3a", "So it's true then, you were created with the Lady's power?"));

                    break;

                case 4:

                    responseList.Add(new Response("4a", "Sociopathic slime monsters tend to talk smack."));
                    responseList.Add(new Response("4a", "That blob was the most disgusting thing I've ever seen... today."));

                    break;

                case 5:

                    responseList.Add(new Response("5a", "Are those... wisps?"));

                    break;

                case 6:

                    responseList.Add(new Response("6a", "You kept the circle active in the valley all this time."));
                    responseList.Add(new Response("6a", "Your friend was heartbroken. Such is life. His burdens are no longer yours to bear."));

                    break;

            }

            GameLocation.afterQuestionBehavior questionBehavior = new(DialogueResponses);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);

            return;

        }

        public override void DialogueResponses(Farmer visitor, string dialogueId)
        {

            StardewValley.NPC npc = companions[0];

            switch (dialogueId)
            {
                default: //beachOne

                    activeCounter = Math.Max(100, activeCounter);

                    DialogueDraw(npc, "The sands and waves glimmer as they once did, but I think I remember them differently. " +
                            "Perhaps they will appear more familiar when I've spent some time here.");

                    break;

                case "2a":

                    activeCounter = Math.Max(200, activeCounter);

                    DialogueDraw(npc, "I feel a resistance when I entreat the aid of the Two Kings. A slowness to respond. " +
                        "Yes. The farm and grove of my former masters want for a better caretaker.");

                    break;

                case "3a":

                    activeCounter = Math.Max(250, activeCounter);

                    DialogueDraw(npc, "The first farmer and the Lady worked together in the aftermath of the war to revitalise the valley. " +
                            "From the moment I awoke in this form, the circle of druids has been my mission, and my home. We built a little utopia in the valley, the first farm. " +
                            "Then we rested and talked about the legends of our time. " +
                            "But the dragons disappeared, and the elderborn departed, and the humans were left with all their creations. I have lost count of the days since that time.");

                    break;

                case "4a":

                    activeCounter = Math.Max(400, activeCounter);

                    DialogueDraw(npc, "Though it pains me to admit, the Jellyking spoke the truth about my present position. I am not an adequate guardian of the change. " +
                            "Such duties have fallen to others, such as yourself, and the wizard and his ally. " +
                            "Even the slime can see it. I have failed the duty given to me by the first farmer.");

                    break;

                case "5a":

                    activeCounter = Math.Max(500, activeCounter);

                    DialogueDraw(npc, "The will of the sleeping monarchs, their dreams and promises, become the wisps. " +
                            "In a gentle moment, they revealed themselves to me, and continued to keep me company in the long period that I awaited for my friend's successor. " +
                            "Now they reveal themselves to you too. It is a special privilege.");

                    break;

                case "6a":

                    activeCounter = Math.Max(600, activeCounter);

                    DialogueDraw(npc, "The Lady fulfilled her duty to the Weald and to Yoba. " +
                            "After a time, I began to hear her voice in the storms, and mist would blanket the valley at odd times. " +
                            "It became a mark of her presence, a sign of her enduring affection for the sacred places. " +
                            "The circle weakened in her absence. The First Farmer's attention shifted to other matters, and he began to neglect his duty. " +
                            "He become obsessed with fanciful ideas, and a desire for something beyond his grasp. Then he left.");

                    break;

            }

        }
    }

}