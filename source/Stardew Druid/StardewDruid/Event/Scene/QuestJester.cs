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
using StardewDruid.Cast;
using StardewDruid.Cast.Effect;
using StardewDruid.Cast.Mists;
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using xTile.Dimensions;
using xTile.Tiles;
using static StardewDruid.Data.IconData;
using static StardewValley.Menus.CharacterCustomization;

namespace StardewDruid.Event.Scene
{
    public class QuestJester : EventHandle
    {

        public StardewDruid.Monster.Dinosaur dinosaur;

        public int dialogueCounter;

        public bool raceChallenge;

        public Vector2 companionVector;

        public Vector2 buffinVector;

        public Vector2 relicPosition;

        public QuestJester()
        {
            
            mainEvent = true;

            activeLimit = 600;

        }


        public override void EventActivate()
        {

            if (!Mod.instance.characters.ContainsKey(CharacterHandle.characters.Jester))
            {

                return;

            }

            base.EventActivate();

            narrators = new()
            {
                [0] = "Jester",
                [1] = "Buffin",
                [2] = "Marlon",
                [3] = "Gunther",
                [4] = "Summoned Saurus",
            };

            companions[0] = Mod.instance.characters[CharacterHandle.characters.Jester] as StardewDruid.Character.Jester;

            voices[0] = companions[0];

            companions[0].SwitchToMode(Character.Character.mode.scene, Game1.player);

            companions[0].eventName = eventId;

            CharacterMover.Warp(location, companions[0], origin + new Vector2(128, 128));

            companions[0].netDirection.Set(1);

            ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 600);

            location.playSound("discoverMineral");

        }

        public override bool AttemptReset()
        {

            companions[0].SwitchToMode(Character.Character.mode.random, Game1.player);

            if (companions.ContainsKey(1))
            {
                Mod.instance.iconData.AnimateQuickWarp(location, companions[1].Position, true);

                companions[1].currentLocation.characters.Remove(companions[1]);

                companions.Remove(1);

            }

            if (companions.ContainsKey(2))
            {
                Mod.instance.iconData.AnimateQuickWarp(location, companions[2].Position, true);

                companions[2].currentLocation.characters.Remove(companions[2]);

                companions.Remove(2);

            }

            Mod.instance.CastMessage("Event aborted, try again tomorrow", 3, true);

            return false;

        }


        public override void RemoveMonsters()
        {

            if (dinosaur != null)
            {

                dinosaur.currentLocation.characters.Remove(dinosaur);

                dinosaur.currentLocation = null;

                dinosaur = null;

            }

            base.RemoveMonsters();

        }

        public override void EventInterval()
        {

            activeCounter++;


            if (activeCounter > 900)
            {

                DialogueCue(activeCounter);

                if (activeCounter > 925 && activeCounter <= 964)
                {

                    if (!ModUtility.MonsterVitals(dinosaur, location))
                    {

                        activeCounter = 965;

                    }
                    else
                    {

                        relicPosition = dinosaur.Position;

                    }

                }

            }

            switch (activeCounter)
            {
                // Intro: Pelicans
                case 1:


                    DialogueCue(0,"So much human stuff happens here");

                    break;

                case 3:

                    Mod.instance.CastDisplay("Talk to the Jester when the green speech icon appears");

                    break;

                case 4:

                    DialogueCue( 0, "I smell something familiar");

                    companionVector = origin + new Vector2(320, 64);

                    companions[0].TargetEvent(0, companionVector, true);

                    break;

                case 7:

                    companions[0].netStandbyActive.Set(true);

                    DialogueLoad(0, 1);

                    break;

                case 18:
                    
                    activeCounter = 99; break;

                // Part One: Marlon
                case 100:

                    DialogueClear(0);

                    Vector2 marlonVector = companionVector + new Vector2(128,0);

                    companions[2] = new Marlon(CharacterHandle.characters.Marlon);

                    companions[2].SwitchToMode(Character.Character.mode.scene, Game1.player);

                    CharacterMover.Warp(location, companions[2], marlonVector);

                    companions[2].eventName = eventId;

                    companions[2].TargetEvent(301, companions[2].Position + new Vector2(0,192));

                    voices[2] = companions[2];

                    location.playSound("doorOpen");

                    DialogueCue(2, "Heh. Folks just like the sound of Pelican Town, is all");

                    break;

                case 103:

                    DialogueCue(0, "Hello old man");

                    companions[0].Position = companions[2].Position - new Vector2(48,0);

                    companions[0].netDirection.Set(1);

                    companions[0].netStandbyActive.Set(false);

                    companions[0].netWorkActive.Set(true);

                    companions[0].netSpecialActive.Set(true);

                    companions[0].specialTimer = 120;

                    companions[2].Halt();

                    companions[2].netDirection.Set(3);

                    break;

                case 105:

                    companions[0].Position = companions[2].Position - new Vector2(96, 0);

                    companions[0].netStandbyActive.Set(true);

                    break;

                case 106:

                    DialogueCue(2, "So, cat'o'fates, how was the mountain?");

                    break;

                case 109:

                    DialogueCue(0, "The fallen one eludes me yet");

                    break;

                case 112:

                    companions[2].netDirection.Set(2);

                    DialogueCue(2, "Ah. I'm sorry I was of no use to your search");

                    break;

                case 115:

                    DialogueCue(0, "What? Without your help I would have remained in that ditch");

                    break;

                case 118:

                    DialogueCue(2, "Only keeping the adventurer's oaths. Hiccup.");
                    break;

                case 121:

                    companions[0].netDirection.Set(1);

                    companions[0].netStandbyActive.Set(false);

                    DialogueCue(0, "Are you well? Looks like cheese poisoning");
                    break;

                case 124:

                    companions[2].netDirection.Set(3);

                    DialogueCue(2, "I was found unconscious on the path");
                    break;

                case 125:

                    DialogueCue(0, "!");
                    break;

                case 127:

                    companions[2].netDirection.Set(2);

                    DialogueCue(2, "Doc had to perform emergency surgery");
                    break;

                case 130:

                    companions[0].netStandbyActive.Set(true);

                    DialogueCue(0, "That's happened to farmer a few times");
                    break;

                case 133:

                    DialogueCue(2, "Maru fed me two chunks of iridium cheddar to aid recovery...");
                    break;

                case 134:

                    DialogueCue(0, "x");
                    break;

                case 136:

                    DialogueCue(0, "Gross, but effective");

                    break;

                case 139:

                    companions[2].netDirection.Set(3);

                    DialogueCue(2, "So, as I'm finding it hard to move, I have an errand for you.");

                    break;

                case 142:

                    DialogueCue(2, "I took this off a shadow raider.");

                    ThrowHandle throwRelic = new(companions[2].Position, companions[0].Position, IconData.relics.saurus_skull);

                    throwRelic.impact = IconData.impacts.puff;

                    throwRelic.register();

                    break;

                case 145:

                    DialogueCue(0, "Well that is gross.");

                    Microsoft.Xna.Framework.Rectangle relicRect = Mod.instance.iconData.RelicRectangles(IconData.relics.saurus_skull);

                    TemporaryAnimatedSprite animation = new(0, 5000, 1, 1, companions[0].Position + new Vector2(32,32), false, false)
                    {
                        sourceRect = relicRect,
                        sourceRectStartingPos = new(relicRect.X, relicRect.Y),
                        texture = Mod.instance.iconData.relicsTexture,
                        layerDepth = (companions[0].Position.Y + 65) / 10000,
                        scale = 4f,
                    };

                    location.TemporarySprites.Add(animation);

                    break;

                case 148:

                    DialogueCue(2, "Lots of shadowfolk topside these days, all graverobbers and thieves.");

                    break;

                case 151:

                    DialogueCue(2, "Gunther's waiting for it at the museum.");

                    break;

                case 154:

                    DialogueCue(2, "Goodbye cat'o'fates, goodbye farmer");

                    companions[2].TargetEvent(302, origin + new Vector2 (448, 64));

                    break;

                case 156:

                    DialogueCue(0, "Farewell friend");

                    companions[2].currentLocation.characters.Remove(companions[2]);

                    companions.Remove(2);

                    break;

                case 158:

                    DialogueLoad(0,2);

                    break;

                case 165:

                    activeCounter = 199; break;


                // ----------------------------------------
                // Part Two: Buffin
                
                case 200:

                    DialogueClear(0);

                    companionVector += new Vector2(640, 320);
                    
                    companions[0].netStandbyActive.Set(false);

                    companions[0].TargetEvent(0, companionVector, true);

                    DialogueCue(0, "Time to get this lumbering thing to the blue man");

                    break;

                case 203:

                    DialogueCue(0, "Guess tricks will have to wait");

                    break;

                case 205:

                    Vector2 BuffinPosition = companionVector + new Vector2(128,0);

                    companions[1] = new Buffin(CharacterHandle.characters.Buffin);

                    companions[1].SwitchToMode(Character.Character.mode.scene, Game1.player);

                    CharacterMover.Warp(location, companions[1], BuffinPosition);

                    companions[1].eventName = eventId;

                    companions[1].moveDirection = 3;

                    companions[1].netDirection.Set(3);

                    voices[1] = companions[1];

                    break;

                case 206:

                    DialogueCue(1, "!");

                    break;

                case 207:

                    DialogueCue(0, "!");

                    buffinVector = companionVector + new Vector2(960, 192);

                    companions[1].TargetEvent(0, buffinVector, true);

                    break;

                case 209:

                    DialogueCue(0, "Buffin!");

                    companionVector = companionVector + new Vector2(832, 192);

                    companions[0].TargetEvent(0, companionVector, true);

                    break;

                case 211:

                    companions[1].IsInvisible = true;

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[1].Position - new Vector2(0.0f, 32f));

                    break;

                case 212:

                    DialogueCue(0, "?");

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[1].Position + new Vector2(0.0f, -196f));

                    break;

                case 213:

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[1].Position + new Vector2(-320f, 128f));

                    break;

                case 214:

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[1].Position + new Vector2(-384f, -196f));

                    break;

                case 215:


                    activeCounter = 249;

                    break;

                case 250:

                    companions[0].LookAtTarget(Game1.player.Position,true);

                    DialogueLoad(0,3);

                    break;

                case 261:

                    activeCounter = 299; break;

                // --------------------------------------
                // Part Three: Contest

                case 300:

                    DialogueClear(0);

                    companions[1].IsInvisible = false;

                    companions[1].Position = companions[0].Position + new Vector2(196, 64);

                    companions[1].Halt();

                    companions[1].netDirection.Set(3);

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[1].Position - new Vector2(0.0f, 32f));

                    DialogueCue(1, "I've come to challenge you, Jester");

                    break;

                case 302:

                    companions[0].netDirection.Set(1);

                    break;

                case 303:

                    DialogueCue(0, "Of course you have");

                    break;

                case 304:

                    companions[1].doEmote(20, true);

                    break;

                case 306:

                    DialogueCue(0, "What's the contest then, Buffin?");

                    break;

                case 309:

                    DialogueCue(1, "I'll let you decide");

                    break;

                case 312:

                    DialogueCue(0, "And the stakes?");

                    break;

                case 315:

                    DialogueCue(1, "A boon, by the laws of the fates");

                    break;

                case 318:

                    DialogueCue(0, "Hmmm. A race then. Terrestrial");

                    break;

                case 321:

                    DialogueCue(1, "So no warp tricks? Suits me");

                    break;

                case 324:

                    DialogueCue(0, "Past the manor, over the bridge, to the museum");

                    break;

                case 327:

                    DialogueCue(1, "I'll make you choke on my tail fluff");

                    break;

                case 330:

                    DialogueCue(0, "Try to keep up Farmer. GO!");

                    break;

                case 331:

                    Mod.instance.CastDisplay("Wait for Jester by the museum");

                    // Jester route

                    Vector2 jesterRace = companions[0].Position + new Vector2(-240, 640);
                    
                    companions[0].TargetEvent(0, jesterRace, true);

                    jesterRace += new Vector2(64, 1152);

                    companions[0].TargetEvent(1, jesterRace, false);

                    jesterRace += new Vector2(2788, 0);

                    companions[0].TargetEvent(201, jesterRace, false);

                    //jesterRace += new Vector2(128, -2560);

                    //companions[0].TargetEvent(201, jesterRace, false);

                    companionVector = jesterRace;

                    companions[0].pathActive = Character.Character.pathing.running;

                    // Buffin route

                    Vector2 buffinRace = companions[1].Position + new Vector2(-384, 640);

                    companions[1].TargetEvent(0, buffinRace, true);

                    buffinRace += new Vector2(64, 1152);

                    companions[1].TargetEvent(1, buffinRace, false);

                    buffinRace += new Vector2(2788, 0);

                    companions[1].TargetEvent(2, buffinRace, false);

                    //buffinRace += new Vector2(256, -2624);

                    //companions[1].TargetEvent(3, buffinRace, false);

                    buffinVector = buffinRace;

                    companions[1].pathActive = Character.Character.pathing.running;

                    break;

                case 370:

                    companions[0].netStandbyActive.Set(true);

                    companions[0].netDirection.Set(1);

                    Mod.instance.CastDisplay("Jester has reached the town museum");

                    DialogueLoad(0, 4);

                    break;

                case 371:

                    companions[1].netStandbyActive.Set(true);

                    companions[1].netDirection.Set(3);

                    break;

                case 385:

                    activeCounter = 799;
                    
                    break;

                // --------------------------------------
                // Museum fight (inserted)

                case 800:

                    DialogueClear(0);

                    companions[0].netStandbyActive.Set(false);

                    companions[0].TargetEvent(203, companions[0].Position + new Vector2(128,-192), true);

                    companions[1].netStandbyActive.Set(false);

                    companions[1].TargetEvent(203, companions[0].Position + new Vector2(128, -128), true);

                    DialogueCue(0, "This place smells like bread and jam");

                    break;

                case 810:

                    Game1.warpFarmer(LocationData.druid_archaeum_name, 27, 18, 1);

                    Game1.xLocationAfterWarp = 27;

                    Game1.yLocationAfterWarp = 18;

                    inabsentia = true;

                    location = Mod.instance.locations[LocationData.druid_archaeum_name];

                    CharacterMover.Warp(Mod.instance.locations[LocationData.druid_archaeum_name], companions[0], new Vector2(24, 15) * 64, false);

                    companions[0].netStandbyActive.Set(true);

                    CharacterMover.Warp(Mod.instance.locations[LocationData.druid_archaeum_name], companions[1], new Vector2(31, 15) * 64, false);

                    companions[1].netStandbyActive.Set(true);

                    companions[1].netDirection.Set(3);

                    Vector2 GuntherPosition = new Vector2(27, 14)* 64;

                    companions[3] = new Gunther(CharacterHandle.characters.Gunther);

                    companions[3].SwitchToMode(Character.Character.mode.scene, Game1.player);

                    CharacterMover.Warp(location, companions[3], GuntherPosition);

                    companions[3].eventName = eventId;

                    companions[3].netDirection.Set(2);

                    voices[3] = companions[3];

                    break;

                case 811:
                case 812:
                case 813:
                case 814:

                    if (Game1.player.currentLocation.Name == LocationData.druid_archaeum_name)
                    {

                        inabsentia = false;

                        activeCounter = 899;

                    }

                    break;

                case 815:

                    inabsentia = false;

                    activeCounter = 899;

                    break;

                // --------------------------------------
                // Museum fight

                case 900:

                    cues = DialogueData.DialogueScene(eventId);

                    Microsoft.Xna.Framework.Rectangle relicRectTwo = Mod.instance.iconData.RelicRectangles(IconData.relics.saurus_skull);

                    TemporaryAnimatedSprite animationTwo = new(0, 20000, 1, 1, new Vector2(27,15)*64 + new Vector2(32), false, false)
                    {
                        sourceRect = relicRectTwo,
                        sourceRectStartingPos = new(relicRectTwo.X, relicRectTwo.Y),
                        texture = Mod.instance.iconData.relicsTexture,
                        layerDepth = 900f,
                        scale = 4f,
                    };

                    location.TemporarySprites.Add(animationTwo);

                    break;

                case 916:

                    companions[0].ResetActives(true);

                    companions[0].netDirection.Set(1);

                    companions[1].ResetActives(true);

                    companions[1].netDirection.Set(3);

                    Mod.instance.iconData.DecorativeIndicator(location, new Vector2(27,15) * 64 + new Vector2(32), IconData.decorations.fates, 3f, new() { interval = 6000 });

                    break;

                case 918:

                    Mod.instance.spellRegister.Add(new(new Vector2(27,15) * 64 + new Vector2(32), 128, IconData.impacts.puff, new()) { type = SpellHandle.spells.bolt, scheme = IconData.schemes.fates, projectile = 1 });

                    break;

                case 920:

                    Mod.instance.spellRegister.Add(new(new Vector2(27,15) * 64 + new Vector2(32), 128, IconData.impacts.puff, new()) { type = SpellHandle.spells.bolt, scheme = IconData.schemes.fates, projectile = 1 });

                    break;

                case 921:

                    dinosaur = new(new Vector2(27,16), Mod.instance.CombatDifficulty());

                    dinosaur.netScheme.Set(2);

                    dinosaur.baseJuice = 1;

                    dinosaur.basePulp = 40;

                    dinosaur.SetMode(3);

                    dinosaur.netPosturing.Set(true);

                    location.characters.Add(dinosaur);

                    dinosaur.update(Game1.currentGameTime, location);

                    voices[4] = dinosaur;

                    break;

                case 923:

                    companions[3].TargetEvent(0, companions[3].Position - new Vector2(384, 256));

                    companions[3].pathActive = Character.Character.pathing.running;

                    dinosaur.netPosturing.Set(false);

                    break;

                case 925:

                    companions[3].LookAtTarget(dinosaur.Position);

                    companions[0].SwitchToMode(Character.Character.mode.track, Game1.player);

                    Mod.instance.characters[CharacterHandle.characters.Buffin] = companions[1];
                    
                    companions[1].SwitchToMode(Character.Character.mode.track, Game1.player);

                    break;

                case 928:
                case 933:
                case 938:
                case 943:
                case 948:
                case 953:
                case 958:
                case 962:

                    companions[3].SpecialAttack(dinosaur);

                    break;

                case 964:

                    dinosaur.Halt();

                    dinosaur.idleTimer = 2000;

                    Mod.instance.iconData.DecorativeIndicator(location, relicPosition, IconData.decorations.fates, 3f, new() { interval = 2000 });

                    break;

                case 965:

                    location.characters.Remove(dinosaur);

                    dinosaur = null;

                    ThrowHandle newThrowRelic = new( relicPosition, companions[1].Position, IconData.relics.saurus_skull);

                    newThrowRelic.impact = IconData.impacts.puff;

                    newThrowRelic.register();

                    Mod.instance.spellRegister.Add(new(relicPosition, 320, IconData.impacts.deathbomb,new()));

                    voices.Remove(4);

                    companions[0].SwitchToMode(Character.Character.mode.scene, Game1.player);

                    companions[0].netStandbyActive.Set(true);

                    Mod.instance.characters.Remove(CharacterHandle.characters.Buffin);

                    companions[1].SwitchToMode(Character.Character.mode.scene, Game1.player);

                    companions[1].netStandbyActive.Set(true);

                    break;

                case 970:

                    Vector2 outsideTile = ModUtility.PositionToTile(companionVector);

                    Game1.warpFarmer("Town", (int)outsideTile.X+1, (int)outsideTile.Y + 2, 1);

                    Game1.xLocationAfterWarp = (int)outsideTile.X + 1;

                    Game1.yLocationAfterWarp = (int)outsideTile.Y + 2;

                    inabsentia = true;

                    location = Game1.getLocationFromName("Town");

                    CharacterMover.Warp(location, companions[0], companionVector, false);//new Vector2(100, 56) * 64, false);

                    buffinVector = companionVector + new Vector2(192, 0);

                    CharacterMover.Warp(location, companions[1], buffinVector, false);//new Vector2(102, 56) * 64, false);

                    companions[0].netDirection.Set(1);

                    companions[1].netDirection.Set(3);

                    break;

                case 971:
                case 972:
                case 973:
                case 974:

                    if (Game1.player.currentLocation.Name == LocationData.druid_archaeum_name)
                    {

                        inabsentia = false;

                        activeCounter = 399;

                    }

                    break;

                case 975:

                    inabsentia = false;

                    activeCounter = 399;

                    break;

                // --------------------------------------
                // Cat fight

                case 400:

                    DialogueClear(0);

                    companions[0].netStandbyActive.Set(false);

                    companions[1].netStandbyActive.Set(false);

                    companions[1].netDirection.Set(3);

                    DialogueCue(1, "Time to head back, Jester");

                    break;

                case 403:

                    companions[0].netDirection.Set(1);

                    DialogueCue(0, "What?");

                    break;

                case 406:

                    DialogueCue(1, "It's time to return to court");

                    break;

                case 409:

                    DialogueCue(0, "...why would I go back. I have my mission");

                    break;

                case 412:

                    DialogueCue(1, "It's where you belong. It's where we both belong");

                    break;

                case 415:

                    DialogueCue(0, "I think I belong here.");

                    break;

                case 418:

                    DialogueCue(1, "Where you keep making a fool of yourself?");

                    break;

                case 421:

                    DialogueCue(1, "Admit it, Fortumei stuffed up again");

                    companions[0].doEmote(16);

                    break;

                case 424:

                    DialogueCue(0, "Do not besmirch the High Priestess!");

                    break;

                case 427:

                    DialogueCue(1, "Why not, they all say it");

                    break;

                case 430:

                    DialogueCue(0, "Who cares what they say. Only She knows Yoba's will");

                    break;

                case 433:

                    DialogueCue(1, "I think it's time to make good on that boon you owe me");

                    companions[1].Position = companions[1].Position + new Vector2(196, 0);

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[1].Position - new Vector2(0.0f, 32f));

                    break;

                case 436:

                    DialogueCue(0, "I don't think so");

                    companions[0].Position = companions[0].Position + new Vector2(-196, 0);

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[0].Position - new Vector2(0.0f, 32f));

                    break;

                case 438:

                    companions[0].ResetActives(true);

                    companions[0].TargetEvent(0, companions[1].Position + new Vector2(64, -32),true);

                    companions[0].SetDash(companions[1].Position + new Vector2(64, -32),false);
                    
                    companions[1].ResetActives(true);

                    companions[1].TargetEvent(0, companions[0].Position - new Vector2(64, 32),true);

                    companions[1].SetDash(companions[0].Position - new Vector2(64, 32),false);

                    Mod.instance.iconData.ImpactIndicator(location, companions[0].Position + new Vector2(352,32), Data.IconData.impacts.flashbang, 3, new());

                    break;

                case 440:

                    companions[0].LookAtTarget(companions[1].Position);

                    companions[1].LookAtTarget(companions[0].Position);

                    break;

                case 441:

                    DialogueCue(1, "You're nothing next to Chaos");

                    companions[0].ResetActives(true);

                    companions[0].TargetEvent(0, companions[1].Position - new Vector2(64, 32), true);

                    companions[0].SetDash(companions[1].Position - new Vector2(64, 32), false);

                    companions[1].ResetActives(true);

                    companions[1].TargetEvent(0, companions[0].Position + new Vector2(64, -32), true);

                    companions[1].SetDash(companions[0].Position + new Vector2(64, -32), false);

                    Mod.instance.iconData.ImpactIndicator(location, companions[0].Position + new Vector2(-288,32), Data.IconData.impacts.flashbang, 3, new());

                    break;

                case 444:

                    DialogueCue(0, "BEHOLD! My new trick...");

                    companions[0].ResetActives(true);

                    companions[0].Position = companionVector + new Vector2(-32, -256);

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[0].Position - new Vector2(0.0f, 32f));

                    companions[1].ResetActives(true);

                    companions[1].Position = buffinVector + new Vector2(576, -256);

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[1].Position - new Vector2(0.0f, 32f));

                    companions[0].LookAtTarget(companions[1].Position);

                    companions[1].LookAtTarget(companions[0].Position);

                    break;

                case 446:

                    // Jester beam

                    companions[0].ResetActives(true);

                    companions[0].netSpecialActive.Set(true);

                    companions[0].specialTimer = 120;

                    SpellHandle beam = new(companions[0].currentLocation, companions[1].GetBoundingBox().Center.ToVector2(), companions[0].GetBoundingBox().Center.ToVector2());

                    beam.type = SpellHandle.spells.beam;

                    beam.scheme = schemes.ether;

                    Mod.instance.spellRegister.Add(beam);

                    // Buffin beam

                    companions[1].ResetActives(true);

                    companions[1].netSpecialActive.Set(true);

                    companions[1].specialTimer = 120;

                    SpellHandle beamTwo = new(companions[1].currentLocation, companions[0].GetBoundingBox().Center.ToVector2(), companions[1].GetBoundingBox().Center.ToVector2());

                    beamTwo.type = SpellHandle.spells.beam;

                    beamTwo.scheme = schemes.fates;

                    Mod.instance.spellRegister.Add(beamTwo);

                    Mod.instance.iconData.ImpactIndicator(location, companions[0].Position + new Vector2(416, 32), Data.IconData.impacts.flashbang, 3, new());

                    break;

                case 447:

                    Vector2 cornerVector = companionVector + new Vector2(0, -448);

                    for (int i = 1; i < 10; i++)
                    {

                        for (int j = 1; j < 6; j++)
                        {

                            Vector2 burnVector = cornerVector + new Vector2((i * 64), (j * 64)) - new Vector2(16 * Mod.instance.randomIndex.Next(0, 4), 16 * Mod.instance.randomIndex.Next(0, 4));

                            Mod.instance.iconData.EmberConstruct(location, IconData.schemes.ember, burnVector, Mod.instance.randomIndex.Next(2,5), Mod.instance.randomIndex.Next(3), 60, 999f);

                            if(i % 3 == 0 && j % 3 == 0)
                            {

                                TemporaryAnimatedSprite lightCircle = new(23, 200f, 6, 1, burnVector, false, Game1.random.NextDouble() < 0.5)
                                {
                                    texture = Game1.mouseCursors,
                                    light = true,
                                    lightRadius = 3f,
                                    lightcolor = Color.Black,
                                    alphaFade = 0.03f,
                                    Parent = location,
                                };

                                location.temporarySprites.Add(lightCircle);

                                animations.Add(lightCircle);
                            }

                        }

                    }

                    break;

                case 448:

                    companions[0].netDirection.Set(0);

                    companions[0].netAlternative.Set(1);

                    companions[0].Position = companionVector + new Vector2(128,64);

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[0].Position - new Vector2(0.0f, 32f));

                    companions[1].moveDirection = 0;

                    companions[1].netDirection.Set(0);

                    companions[1].netAlternative.Set(3);

                    companions[1].Position = buffinVector + new Vector2(128, 64);

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[1].Position - new Vector2(0.0f, 32f));

                    break;

                case 449:

                    DialogueCue(1, "Pretty");

                    break;

                case 450:

                    DialogueLoad(0,5);

                    break;

                case 460:

                    activeCounter = 499;

                    break;

                // -----------------------------------
                // aftermath

                case 500:

                    DialogueClear(0);

                    companions[1].netDirection.Set(3);

                    companions[0].netDirection.Set(1);

                    break;

                case 501:

                    DialogueCue(1, "Jester...");

                    break;

                case 504:

                    DialogueCue(0, "Hey Buffin, wanna sit on a bridge for a while?");

                    break;

                case 507:

                    DialogueCue(1, "I'm sorry for what happened at court");

                    companionVector = companionVector += new Vector2(-1280, 128);

                    companions[0].TargetEvent(202, companionVector, true);

                    buffinVector = companionVector + new Vector2(108, 0);

                    companions[1].TargetEvent(0, buffinVector, true);

                    break;

                case 510:

                    DialogueCue(0, "Yea. I was sad for a long time");

                    break;

                case 520:

                    DialogueCue(1, "I panicked when you volunteered yourself");

                    companions[0].netStandbyActive.Set(true);

                    companions[0].netDirection.Set(1);

                    break;

                case 523:

                    companions[1].netStandbyActive.Set(true);

                    companions[1].netDirection.Set(3);

                    DialogueCue(0, "To be honest. I was joking when I offered to go");

                    break;

                case 526:

                    DialogueCue(1, "What? You weren't serious?");

                    break;

                case 529:

                    DialogueCue(0, "I didn't think it would go this far");

                    break;

                case 532:

                    DialogueCue(1, "BUT WHAT ABOUT THE REAPER");

                    break;

                case 535:

                    DialogueCue(0, "Oh. Yea. I thought he would be easier to find");

                    break;

                case 538:

                    DialogueCue(0, "Imagine if I brought him home. The celebrations...");

                    break;

                case 541:

                    DialogueCue(1, "Have you found anything?");

                    break;

                case 544:

                    DialogueCue(0, "We found a hundred of his victims");

                    break;

                case 546:

                    DialogueCue(1, "!");

                    break;

                case 548:

                    DialogueCue(0, "They chased us the entire length of a dungeon");

                    break;

                case 550:

                    DialogueCue(1, "Oh my Chaos");

                    break;

                case 553:

                    DialogueCue(0, "Yea. At the end of it was statue made in Thanatoshi's honour");

                    break;

                case 556:

                    DialogueCue(0, "Was a bit weird");

                    break;

                case 559:

                    DialogueCue(1, "Jester, I can promise to sing your praises at court");

                    break;

                case 562:

                    companions[1].netStandbyActive.Set(false);

                    companions[1].TargetEvent(0, companions[0].Position + new Vector2(32, 0));

                    DialogueCue(1, "But I'll be watching you");

                    break;

                case 564:

                    companions[0].netStandbyActive.Set(false);

                    DialogueCue(0, "It is good to see you Buffin");

                    break;

                case 567:

                    companions[0].netStandbyActive.Set(true);

                    Mod.instance.iconData.AnimateQuickWarp(location, companions[1].Position - new Vector2(0.0f, 32f), true);

                    companions[1].currentLocation.characters.Remove(companions[1]);

                    companions[1] = null;

                    DialogueLoad(0, 6);

                    break;

                case 575:

                    activeCounter = 699;

                    break;

                case 700:

                    DialogueCue(0, "Good night Farmer. Thank you for coming");

                    companions[0].SwitchToMode(Character.Character.mode.random,Game1.player);

                    companions[0].TargetIdle(10000);

                    eventComplete = true;

                    break;

            }

        }

        public override void EventCompleted()
        {
            
            Mod.instance.questHandle.CompleteQuest(eventId);

        }

        public override void EventScene(int index)
        {

            switch (index)
            {

                case 201:

                    activeCounter = Math.Max(369, activeCounter);

                    break;

                case 202:

                    activeCounter = Math.Max(519, activeCounter);

                    break;

                case 203:

                    activeCounter = Math.Max(809, activeCounter);

                    break;

                case 301:

                    companions[2].Halt();

                    companions[2].netDirection.Set(3);

                    break;

                case 302:

                    companions[2].IsInvisible = true;

                    location.playSound("doorClose");

                    break;
            
            }

        }

        public override void DialogueSetups(StardewDruid.Character.Character npc, int dialogueId)
        {

            string intro;

            switch (dialogueId)
            {
                default:
                case 1:

                    intro = "Jester of Fate: ^So this is the town of pelicans. I guess the humans killed all the birds when they took it over.";

                    break;

                case 2:

                    intro = "Jester of Fate: ^He's one of the good ones, farmer. Makes me sad.";

                    break;

                case 3:

                    intro = "Jester of Fate: ^Any clue where that cosmic fox went?";

                    break;

                case 4:

                    intro = "Jester of Fate: ^I have no idea who actually won.";

                    break;

                case 5:

                    intro = "Jester of Fate: ^I think we went too far.";

                    break;

                case 6:

                    intro = "Jester of Fate: ^When Fortumei asked for a volunteer to take up Thanatoshi's cause, the only answer was silence. " +
                        "So I jested. I proclaimed, with big bravado, that I would get the job done.";

                    break;


            }

            List<Response> responseList = new();

            switch (dialogueId)
            {
                default:
                case 1: //townOne

                    responseList.Add(new Response("townOne", "Not quite... it's probable that the town founders were the Pelican family."));
                    responseList.Add(new Response("townOne", "Indeed. It was quite an effort for us humans to overthrow our big billed overlords."));

                    break;

                case 2:

                    responseList.Add(new Response("townTwo", "Why would you be sad about Marlon?"));
                    responseList.Add(new Response("townTwo", "It is a shame that there aren't more capes and eyepatches in the valley."));

                    break;

                case 3:

                    responseList.Add(new Response("townThree", "Probably eating cookies and sipping whiskey with the Muellers."));
                    responseList.Add(new Response("townThree", "Within the insatiable maw of the town dog from which there is no escape."));
                    responseList.Add(new Response("townThree", "Just check the trash. Everyone else does... right?"));

                    break;

                case 4:

                    responseList.Add(new Response("townFour", "I saw both of you arrive after me."));
                    responseList.Add(new Response("townFour", "Buffin is quicker on the ground."));
                    responseList.Add(new Response("townFour", "You Jester, you're a powerhouse of motion when you want to be."));

                    break;

                case 5:

                    responseList.Add(new Response("townFive", "Don't worry. The Junimos have a knack for fixing this kind of thing."));
                    responseList.Add(new Response("townFive", "I don't see Gunther's precious wooden panelling being consumed by the flame. Is that even a real fire?"));
                    //responseList.Add(new Response("townFive", "What kind of power does Buffin wield?"));

                    break;

                case 6:

                    responseList.Add(new Response("townSix", "You've done pretty well considering you never intended to get this far. Do you have any regrets?"));
                    responseList.Add(new Response("townSix", "That must have been quite the scene, with all those important Fates looking at you. Maybe some saw a fool, but some saw a hero. I see a hero."));

                    break;

            }

            GameLocation.afterQuestionBehavior questionBehavior = new(DialogueResponses);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);

            return;

        }

        public override void DialogueResponses(Farmer visitor, string dialogue)
        {

            StardewValley.NPC npc = companions[0];

            string response;

            switch (dialogue)
            {

                case "townOne":

                    activeCounter = Math.Max(99, activeCounter);

                    response = "I wonder if the pelicans were still around, would they be able to tell us where the fallen star is.";

                    DialogueDraw(npc, response);

                    break;

                case "townTwo":

                    activeCounter = Math.Max(199, activeCounter);

                    response = "When I first came to the valley, I started at the last known position of Thanatoshi before he vanished in his pursuit of the fallen one. " +
                            "Even after a week I couldn't find a clue to his whereabouts. So I cried a little. I even yelled a bit. Well sort of cat-screamed. " +
                            "I guess kind of loudly, because the one-eyed cheese-eating adventure man told me to shut up. " +
                            "Then he offered me a spot to sleep by a warm fire, and I learned from him about the star crater, and that long tunnel of death we explored. " +
                            "Yet, while he talked, I could also hear the faint whispers of the priesthood. " +
                            "The oracles predict that the one eyed warrior will never cease his crusade against the shadows of the valley. " +
                            "One day he will lie in an unmarked grave, buried respectfully by the young shadow brute who bests him. " +
                            "Pretty grim. But hey, the priesthood doesn't always get things right. ";

                    DialogueDraw(npc, response);

                    break;

                case "townThree":

                    activeCounter = Math.Max(299, activeCounter);

                    response = "Sounds like something the Buffoonette of Chaos would do. " +
                            "Most fates get confused or angry by her attempts at fun. I'm one of her only friends.";

                    DialogueDraw(npc, response);

                    break;

                case "townFour":

                    //activeCounter = Math.Max(399, activeCounter);
                    activeCounter = Math.Max(799, activeCounter);

                    response = "Heh, it didn't seem like Buffin put in much effort. She's usually very foxy. " +
                            "I guess there's something else going on with her. There always is.";

                    DialogueDraw(npc, response);

                    break;

                case "townFive":

                    activeCounter = Math.Max(499, activeCounter);

                    response = "Buffin serves the Stream of Chaos, who occupies one of the four seats of the Fates. " +
                            "The stream has an influence on some of the more, uh, fun aspects of the mysteries of the Fates, so things like this tend to happen when we get together. " +
                            "(Jester sighs) This isn't what the high priestess expects of me.";

                    DialogueDraw(npc, response);

                    break;

                case "townSix":

                    activeCounter = Math.Max(699, activeCounter);

                    response = "The faithful certainly took it as a joke. One heckler said the earth cats would chase me away. " +
                            "An oracle foretold that my sparkly star cape would be torn to shreds. " +
                            "Buffin showed the court an image of me in tears, stuck in a ditch. All of which ended up happening for real but besides that, Fortumei took me at my word. " +
                            "I'm more then a joke.";


                    DialogueDraw(npc, response);

                    break;

            }

        }

    }

}
