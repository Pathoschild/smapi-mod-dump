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
using StardewDruid.Cast.Mists;
using StardewDruid.Character;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.IO;
using xTile.Dimensions;

namespace StardewDruid.Event.Scene
{
    public class Town : EventHandle
    {

        public StardewDruid.Character.Jester companion;

        public StardewDruid.Character.Buffy buffin;

        public int dialogueCounter;

        public bool raceChallenge;

        public Vector2 companionVector;

        public Vector2 buffinVector;

        public Town(Vector2 target, Quest quest)
          : base(target)
        {
            questData = quest;
        }

        public override void EventTrigger()
        {
            
            if (!Mod.instance.characters.ContainsKey("Jester"))
            {

                return;

            }

            cues = new();

            narrators = new()
            {
                [0] = new("Jester", Microsoft.Xna.Framework.Color.PaleGoldenrod),
                [1] = new("Buffin", Microsoft.Xna.Framework.Color.Purple),
                [2] = new("Marlon", Microsoft.Xna.Framework.Color.White),
                [3] = new("Fortumei, Priestess of Fate", Microsoft.Xna.Framework.Color.White),
            };

            companion = Mod.instance.characters["Jester"] as StardewDruid.Character.Jester;

            Mod.instance.RegisterEvent(this, "heartJester");

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 600.0;

        }

        public override void EventRemove()
        {

            companion.SwitchPreviousMode();

            if(buffin != null)
            {

                buffin.currentLocation.characters.Remove(buffin);

            }

            base.EventRemove();

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
                // Intro: Pelicans
                case 1:

                    companion.SwitchSceneMode();

                    companion.moveDirection = 2;

                    companion.netDirection.Set(1);

                    companion.eventName = "heartJester";

                    if (companion.currentLocation.Name != Mod.instance.rite.castLocation.Name)
                    {
                        companion.TargetIdle(30);
                        companion.currentLocation.characters.Remove(companion);
                        companion.currentLocation = Mod.instance.rite.castLocation;
                        companion.currentLocation.characters.Add(companion);
                    }

                    companion.Position = new(questData.triggerVector.X * 64f, questData.triggerVector.Y * 64f);

                    voices = new() { [0] = companion, };

                    ModUtility.AnimateQuickWarp(Mod.instance.rite.castLocation, companion.Position - new Vector2(0.0f, 32f));

                    DialogueCue(0,"So much human stuff happens here");

                    break;

                case 3:

                    Mod.instance.CastMessage("Talk to Jester when '...' appears");

                    break;

                case 4:

                    DialogueCue(0,"I'm going to rely on you, my trusty guide-human");

                    companion.ResetActives();

                    companionVector = targetVector + new Vector2(5, 1);

                    companion.eventVectors.Add(0, companionVector * 64);

                    break;

                case 7:

                    companion.TargetIdle();

                    companion.netStandbyActive.Set(true);

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester", "townOne");

                    DialogueCue(0,"...");

                    break;

                case 10:
                case 13:
                case 16:

                    DialogueCue(0);

                    break;

                case 18:
                    
                    activeCounter = 99; break;

                // Part One: Marlon
                case 100:

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester");

                    Vector2 marlonVector = (targetVector + new Vector2(7,-1)) *64;

                    Actor marlon = new(marlonVector, targetLocation.Name, "Marlon");

                    actors.Add(marlon);

                    targetLocation.characters.Add(marlon);

                    marlon.update(Game1.currentGameTime, targetLocation);

                    voices[2] = marlon;

                    TemporaryAnimatedSprite marlonSprite = new TemporaryAnimatedSprite(0, 2000f, 4, 10, marlonVector, false, false)
                    {
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 32, 16, 32),
                        sourceRectStartingPos = new Vector2(0.0f, 32f),
                        texture = Game1.content.Load<Texture2D>("Characters\\Marlon"),
                        scale = 4f,
                        layerDepth = 992f,
                        Parent = targetLocation,

                    };
                    targetLocation.temporarySprites.Add(marlonSprite);

                    animations.Add(marlonSprite);

                    targetLocation.playSound("doorOpen");

                    DialogueCue(2, "Heh. Folks just like the sound of Pelican Town, is all");

                    break;

                case 103:

                    DialogueCue(0, "Hello old man");

                    companion.ResetActives();

                    companion.Position = (targetVector + new Vector2(6, 1)) * 64 - new Vector2(0,32);

                    companion.rubVector = companion.Position + new Vector2(64,0);

                    companion.netRubActive.Set(true);

                    companion.netSpecialActive.Set(true);

                    companion.specialTimer = 120;

                    break;

                case 106:

                    DialogueCue(2, "So, cat'o'fates, how was the mountain?");

                    break;

                case 109:

                    companion.Position = companion.Position + new Vector2(0, 32);

                    companion.ResetActives();

                    companion.TargetIdle(6000);

                    DialogueCue(0, "The fallen one eludes me yet");

                    break;

                case 112:

                    DialogueCue(2, "Ah. I'm sorry I was of no use to your search");
                    break;

                case 115:

                    DialogueCue(0, "What? Without your shelter I would have perished");
                    break;

                case 118:

                    DialogueCue(2, "Just keeping the adventurer's oaths. Hiccup.");
                    break;

                case 121:

                    DialogueCue(0, "Are you well? Looks like cheese poisoning");
                    break;

                case 124:

                    DialogueCue(2, "I was found unconscious on the path");
                    break;

                case 125:

                    DialogueCue(0, "!");
                    break;

                case 127:

                    DialogueCue(2, "Doctor had to perform emergency surgery");
                    break;

                case 130:

                    DialogueCue(0, "That's happened a few times to farmer");
                    break;

                case 133:

                    DialogueCue(2, "I've just gulfed down two blocks of iridium cheddar...");
                    break;

                case 134:

                    DialogueCue(0, "x");
                    break;

                case 136:

                    DialogueCue(2, "...to fully recover my health and stamina");
                    break;

                case 139:

                    DialogueCue(0, "Gross, but effective");
                    break;

                case 142:

                    DialogueCue(2, "I might stay indoors until my gut settles");
                    break;

                case 145:

                    DialogueCue(2, "Goodbye cat, goodbye farmer");

                    break;

                case 148:

                    DialogueCue(0, "Farewell friend");

                    targetLocation.playSound("doorClose");

                    RemoveAnimations();

                    RemoveActors();

                    break;

                case 151:

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester", "townTwo");

                    DialogueCue(0, "...");

                    break;

                case 154:
                case 157:
                case 160:

                    DialogueCue(0);

                    break;

                case 162:

                    activeCounter = 199; break;

                // Part Two: Buffin
                case 200:

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester");

                    companion.ResetActives();

                    companionVector = companionVector + new Vector2(10, 5);

                    companion.eventVectors.Add(0, companionVector * 64);

                    DialogueCue(0, "Hmmm... I smell... chaos");

                    break;

                case 203:

                    DialogueCue(0, "Over this way");

                    break;

                case 205:

                    Vector2 BuffinPosition = (companionVector + new Vector2(2,0)) *64;

                    buffin = new(BuffinPosition,"Farm");

                    buffin.SwitchSceneMode();

                    buffin.moveDirection = 3;

                    buffin.netDirection.Set(3);

                    buffin.eventName = "heartJester";

                    buffin.currentLocation.characters.Remove(buffin);

                    buffin.currentLocation = targetLocation;

                    buffin.currentLocation.characters.Add(buffin);

                    buffin.Position = BuffinPosition;

                    voices[1] = buffin;

                    ModUtility.AnimateQuickWarp(targetLocation, buffin.Position - new Vector2(0.0f, 32f));

                    break;

                case 206:

                    DialogueCue(1, "!");

                    break;

                case 207:

                    DialogueCue(0, "!");

                    buffin.ResetActives();

                    buffinVector = companionVector + new Vector2(15, 3);

                    buffin.eventVectors.Add(0, buffinVector * 64);

                    break;

                case 209:

                    DialogueCue(0, "Buffin!");

                    companion.ResetActives();

                    companionVector = companionVector + new Vector2(13, 3);

                    companion.eventVectors.Add(0, companionVector * 64);

                    break;

                case 211:

                    buffin.IsInvisible = true;

                    ModUtility.AnimateQuickWarp(targetLocation, buffin.Position - new Vector2(0.0f, 32f));

                    break;

                case 212:

                    DialogueCue(0, "?");

                    ModUtility.AnimateQuickWarp(targetLocation, buffin.Position + new Vector2(0.0f, -196f));

                    break;

                case 213:

                    ModUtility.AnimateQuickWarp(targetLocation, buffin.Position + new Vector2(-320f, 128f));

                    break;

                case 214:

                    ModUtility.AnimateQuickWarp(targetLocation, buffin.Position + new Vector2(-384f, -196f));

                    break;

                case 215:


                    activeCounter = 249;

                    break;

                case 250:

                    companion.ResetActives();

                    companion.moveDirection = 3;

                    companion.netDirection.Set(3);

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester", "townThree");

                    DialogueCue(0, "...");

                    buffin.IsInvisible = false;

                    buffin.Position = companion.Position + new Vector2(196, 64);

                    buffin.Halt();

                    buffin.moveDirection = 3;

                    buffin.netDirection.Set(3);

                    ModUtility.AnimateQuickWarp(targetLocation, buffin.Position - new Vector2(0.0f, 32f));

                    break;

                case 253:
                case 256:
                case 259:

                    DialogueCue(0);

                    break;

                case 261:

                    activeCounter = 299; break;

                case 300:

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester");

                    companion.moveDirection = 1;

                    companion.netDirection.Set(1);

                    DialogueCue(1, "I've come to test you, Jester");

                    break;

                case 303:

                    DialogueCue(0, "Of course you have");

                    break;

                case 306:

                    DialogueCue(0, "What's the game then, Buffin?");

                    break;

                case 309:

                    DialogueCue(1, "Just a simple contest. I'll let you decide what");

                    break;

                case 312:

                    DialogueCue(0, "And the stakes?");

                    break;

                case 315:

                    DialogueCue(1, "Fool around and find out");

                    break;

                case 318:

                    DialogueCue(0, "Hmmm. A race then. Terrestrial style");

                    break;

                case 321:

                    DialogueCue(1, "So no warp tricks? Suits me");

                    break;

                case 324:

                    DialogueCue(0, "Past the manor, over the bridge, up to the old mart");

                    break;

                case 327:

                    DialogueCue(1, "I'll make you choke on my tail fluff");

                    break;

                case 330:

                    DialogueCue(0, "Alright farmer, try to keep up. GO");

                    break;

                case 331:

                    Mod.instance.CastMessage("Wait for Jester by the old Joja mart");

                    companion.ResetActives();

                    Vector2 jesterRace = companion.Position + new Vector2(-240, 640);
                    
                    companion.eventVectors.Add(0, jesterRace);
                    
                    jesterRace += new Vector2(64, 1152);
                    
                    companion.eventVectors.Add(1, jesterRace);
                    
                    jesterRace += new Vector2(1920, 0);
                    
                    companion.eventVectors.Add(2, jesterRace);
                    
                    jesterRace += new Vector2(128, -2560);
                    
                    companion.eventVectors.Add(201, jesterRace);

                    companionVector = jesterRace;

                    buffin.ResetActives();

                    Vector2 buffinRace = buffin.Position + new Vector2(-384, 640);
                    
                    buffin.eventVectors.Add(0, buffinRace);
                    
                    buffinRace += new Vector2(64, 1152);
                    
                    buffin.eventVectors.Add(1, buffinRace);
                    
                    buffinRace += new Vector2(1920, 0);
                    
                    buffin.eventVectors.Add(2, buffinRace);
                    
                    buffinRace += new Vector2(256, -2624);
                    
                    buffin.eventVectors.Add(3, buffinRace);

                    buffinVector = buffinRace;

                    break;

                case 370:

                    Mod.instance.CastMessage("Jester has reached the old Joja mart");

                    companion.TargetIdle();

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester", "townFour");

                    DialogueCue(0, "...");

                    break;

                case 371:

                    buffin.TargetIdle();

                    buffin.netDirection.Set(2);

                    buffin.netAlternative.Set(3);

                    break;

                case 373:
                case 376:
                case 379:
                case 382:
                case 385:

                    DialogueCue(0);

                    break;

                case 386:

                    activeCounter = 399;
                    
                    break;

                case 400:

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester");

                    companion.ResetActives();

                    companion.moveDirection = 1;

                    companion.netDirection.Set(1);

                    buffin.ResetActives();

                    buffin.moveDirection = 3;

                    buffin.netDirection.Set(3);

                    DialogueCue(1, "You've failed my trial, Jester of Fate");

                    break;

                case 403:

                    DialogueCue(0, "What? Can you make sense for once");

                    break;

                case 406:

                    DialogueCue(1, "By the laws of the fates, you owe me a boon");

                    break;

                case 409:

                    DialogueCue(0, "I'm your friend, Buffin");

                    break;

                case 412:

                    DialogueCue(0, "You can just ask me if you need help");

                    break;

                case 415:

                    DialogueCue(1, "Ok, well, I bid you return to court");

                    break;

                case 418:

                    DialogueCue(0, "...why would I go back. I have my mission");

                    break;

                case 421:

                    DialogueCue(1, "It's where you belong. It's where we both belong");

                    break;

                case 424:

                    DialogueCue(0, "Forget about it Buffin. This is my home now");

                    break;

                case 427:

                    DialogueCue(1, "I have the power to force you");

                    buffin.Position = buffin.Position + new Vector2(196, 0);

                    ModUtility.AnimateQuickWarp(targetLocation, buffin.Position - new Vector2(0.0f, 32f));

                    break;

                case 430:

                    DialogueCue(0, "Try it");

                    companion.Position = companion.Position + new Vector2(-196, 0);

                    ModUtility.AnimateQuickWarp(targetLocation, companion.Position - new Vector2(0.0f, 32f));

                    break;

                case 432:

                    companion.ResetActives();

                    companion.moveTimer = companion.dashInterval;

                    companion.netDashActive.Set(true);

                    companion.NextTarget(buffin.Position - new Vector2(0,32), -1);

                    buffin.ResetActives();

                    buffin.moveTimer = buffin.dashInterval;

                    buffin.netDashActive.Set(true);

                    buffin.NextTarget(companion.Position - new Vector2(0, 32), -1);

                    ModUtility.AnimateImpact(targetLocation, companion.Position + new Vector2(352,32), 3, 0, "Flashbang");

                    break;

                case 434:

                    DialogueCue(0, "My new friends have made me stronger");

                    break;

                case 435:

                    companion.ResetActives();

                    companion.moveTimer = companion.dashInterval;

                    companion.netDashActive.Set(true);

                    companion.NextTarget(buffin.Position - new Vector2(0, 32), -1);

                    buffin.ResetActives();

                    buffin.moveTimer = buffin.dashInterval;

                    buffin.netDashActive.Set(true);

                    buffin.NextTarget(companion.Position - new Vector2(0, 32), -1);

                    ModUtility.AnimateImpact(targetLocation, companion.Position + new Vector2(-288,32), 3, 0, "Flashbang");

                    break;

                case 438:

                    DialogueCue(1, "YOU'RE NOTHING AGAINST CHAOS");

                    companion.ResetActives();

                    companion.moveDirection = 1;

                    companion.netDirection.Set(1);

                    companion.Position = companionVector + new Vector2(128, -256);

                    ModUtility.AnimateQuickWarp(targetLocation, companion.Position - new Vector2(0.0f, 32f));

                    buffin.ResetActives();

                    buffin.moveDirection = 3;

                    buffin.netDirection.Set(3);

                    buffin.Position = buffinVector + new Vector2(1068, -256);

                    ModUtility.AnimateQuickWarp(targetLocation, buffin.Position - new Vector2(0.0f, 32f));

                    break;

                case 440:

                    companion.ResetActives();

                    companion.netSpecialActive.Set(true);

                    companion.specialTimer = 60;

                    SpellHandle beam = new(companion.currentLocation, buffin.GetBoundingBox().Center.ToVector2(), companion.GetBoundingBox().Center.ToVector2(), 2, 1, -1, Mod.instance.DamageLevel());

                    beam.type = SpellHandle.barrages.beam;

                    Mod.instance.spellRegister.Add(beam);

                    buffin.ResetActives();

                    buffin.netSpecialActive.Set(true);

                    buffin.specialTimer = 60;

                    beam = new(buffin.currentLocation, companion.GetBoundingBox().Center.ToVector2(), buffin.GetBoundingBox().Center.ToVector2(), 2, 1, -1, Mod.instance.DamageLevel());

                    beam.type = SpellHandle.barrages.chaos;

                    Mod.instance.spellRegister.Add(beam);

                    ModUtility.AnimateImpact(targetLocation, companion.Position + new Vector2(416, 32), 3, 0, "Flashbang");

                    break;

                case 441:

                    Texture2D flameTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "RedEmbers.png"));

                    Vector2 cornerVector = companion.Position + new Vector2(128, -384);

                    for (int i = 0; i < 12; i++)
                    {

                        for (int j = 0; j < 8; j++)
                        {

                            Vector2 burnVector = cornerVector + new Vector2((i * 64), (j * 64)) - new Vector2(16, 16);

                            int k = randomIndex.Next(3);

                            TemporaryAnimatedSprite burnSprite = new(0, 150, 4, 100, burnVector, false, false)
                            {

                                sourceRect = new(0, k * 32, 32, 32),

                                sourceRectStartingPos = new(0, k * 32),

                                texture = flameTexture,

                                scale = 3f,

                                extraInfoForEndBehavior = 99999,

                                layerDepth = 999f,

                                alpha = 0.75f,

                                drawAboveAlwaysFront = true,

                            };

                            targetLocation.temporarySprites.Add(burnSprite);

                            animations.Add(burnSprite);

                            if(i % 3 == 0 && j % 3 == 0)
                            {
                                TemporaryAnimatedSprite light = new(23, 500f, 6, 20, burnVector, false, Game1.random.NextDouble() < 0.5)
                                {
                                    texture = Game1.mouseCursors,
                                    light = true,
                                    lightRadius = 3,
                                    lightcolor = Color.Black,
                                    alpha = 0.75f,
                                    Parent = targetLocation
                                };

                                targetLocation.temporarySprites.Add(light);

                                animations.Add(light);

                            }

                        }

                    }

                    break;

                case 442:

                    companion.ResetActives();

                    companion.Halt();

                    companion.netStandbyActive.Set(true);

                    companion.moveDirection = 0;

                    companion.netDirection.Set(0);

                    companion.netAlternative.Set(1);

                    companion.Position = companionVector + new Vector2(480,64);

                    ModUtility.AnimateQuickWarp(targetLocation, companion.Position - new Vector2(0.0f, 32f));

                    buffin.ResetActives();

                    buffin.Halt();

                    buffin.netStandbyActive.Set(true);

                    buffin.moveDirection = 0;

                    buffin.netDirection.Set(0);

                    buffin.netAlternative.Set(3);

                    buffin.Position = buffinVector + new Vector2(480, 64);

                    ModUtility.AnimateQuickWarp(targetLocation, buffin.Position - new Vector2(0.0f, 32f));

                    break;

                case 443:

                    DialogueCue(1, "Pretty");

                    activeCounter = 469;

                    break;

                case 470:

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester", "townFive");

                    DialogueCue(0, "...");

                    break;

                case 473:
                case 476:
                case 479:

                    DialogueCue(0);

                    break;

                case 481:

                    activeCounter = 499;

                    break;

                case 500:

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester");

                    Mod.instance.CastMessage("Chaos charge-up unlocked. See journal for details.");

                    buffin.ResetActives();

                    buffin.Halt();

                    companion.netStandbyActive.Set(true);

                    buffin.moveDirection = 3;

                    buffin.netDirection.Set(3);

                    DialogueCue(1, "This wouldn't happen if you would just use your head for once");

                    break;

                case 503:

                    companion.ResetActives();

                    companion.Halt();

                    companion.netStandbyActive.Set(true);

                    companion.moveDirection = 3;

                    companion.netDirection.Set(3);

                    if (dialogueCounter >= 5)
                    {

                        DialogueCue(0, "Lets sit on a bridge for a while. Always helps me think");

                        activeCounter = 597;

                        break;

                    }

                    DialogueCue(0, "I just want a fun time out with my friends");

                    break;

                case 506:

                    DialogueCue(1, "I'm sorry I ruined everything again. Goodbye Jester");

                    break;

                case 509:

                    buffin.currentLocation.characters.Remove(buffin);

                    buffin = null;

                    ModUtility.AnimateQuickWarp(targetLocation, buffin.Position - new Vector2(0.0f, 32f));

                    break;

                case 510:

                    companion.TargetIdle();

                    DialogueCue(0, "Well that was a chaotic way to relieve some stress");

                    break;

                case 512:

                    activeCounter = 699;

                    break;

                case 600:

                    DialogueCue(1, "I'm sorry for what happened when you left");

                    companion.ResetActives();

                    companion.eventVectors.Clear();

                    companionVector = companionVector + new Vector2(-896, 64);

                    companion.eventVectors.Add(202, companionVector);

                    buffin.ResetActives();

                    buffin.eventVectors.Clear();

                    buffinVector = companionVector + new Vector2(196, 0);

                    buffin.eventVectors.Add(0, buffinVector);

                    break;

                case 603:

                    DialogueCue(0, "Yea. I was sad for a long time");

                    break;

                case 630:

                    DialogueCue(1, "I panicked when you volunteered yourself to Fortumei");

                    companion.ResetActives();

                    companion.TargetIdle(6000);

                    break;

                case 633:

                    buffin.ResetActives();

                    buffin.eventVectors.Clear();

                    buffin.TargetIdle(6000);

                    buffin.netDirection.Set(2);

                    buffin.netAlternative.Set(3);

                    DialogueCue(0, "To be honest. I was joking when I volunteered");

                    break;

                case 636:

                    DialogueCue(1, "What? You weren't serious? I've been worried sick");

                    break;

                case 639:

                    DialogueCue(0, "I didn't think it would go this far");

                    break;

                case 642:

                    DialogueCue(1, "WE LOST THE REAPER TO THIS FOLLY JESTER");

                    break;

                case 645:

                    DialogueCue(0, "I thought he would be easier to find");

                    break;

                case 648:

                    DialogueCue(0, "Imagine if I brought him home. The celebrations! Fortumei...");

                    break;

                case 651:

                    DialogueCue(1, "Did you even find him?");

                    break;

                case 654:

                    DialogueCue(0, "We found him as an ether-crazed revenant. Farmer put him to rest");

                    break;

                case 657:

                    DialogueCue(1, "Oh my Chaos");

                    break;

                case 660:

                    DialogueCue(0, "Yea. Someone erected a statue in his honour though. Was a bit weird");

                    break;

                case 663:

                    DialogueCue(1, "Have you informed Fortumei?");

                    break;

                case 666:

                    DialogueCue(0, "Not yet... I'll tell her when I'm ready");

                    break;

                case 669:

                    DialogueCue(0, "I know you're worried about me, but I'm strong, with strong friends");

                    break;

                case 672:

                    DialogueCue(0, "There's farmer, an enchanted golem, and even a shady archaeologist");

                    break;

                case 675:

                    DialogueCue(1, "Ok Jester. I'll sing your praises at court. But...");

                    break;

                case 678:

                    DialogueCue(1, "I'll miss you");

                    break;

                case 681:

                    DialogueCue(0, "It is good to see you Buffin.");

                    break;

                case 684:

                    Mod.instance.dialogue["Jester"].AddSpecial("Jester", "townSix");

                    DialogueCue(0, "...");

                    break;

                case 687:
                case 690:
                case 693:

                    DialogueCue(0);

                    break;

                case 696:

                    activeCounter = 699;

                    break;

                case 700:

                    DialogueCue(0, "Good night Farmer. Thank you for coming");

                    break;

                case 703:

                    Mod.instance.CompleteQuest(questData.name);

                    expireEarly = true;

                    break;

            }

        }


        public override void EventScene(int index)
        {

            switch (index)
            {

                case 101: // first dialogue

                    activeCounter = Math.Max(99, activeCounter);

                    dialogueCounter++;

                    break;

                case 102: // second dialogue

                    activeCounter = Math.Max(199, activeCounter);

                    dialogueCounter++;

                    break;

                case 103: // third dialogue

                    activeCounter = Math.Max(299, activeCounter);

                    dialogueCounter++;

                    break;

                case 104: // fourth dialogue

                    activeCounter = Math.Max(399, activeCounter);

                    dialogueCounter++;

                    break;

                case 105: // fifth dialogue

                    activeCounter = Math.Max(499, activeCounter);

                    dialogueCounter++;

                    break;

                case 106: // sixth dialogue

                    activeCounter = Math.Max(699, activeCounter);

                    break;

                case 201:

                    activeCounter = Math.Max(369, activeCounter);

                    break;

                case 202:

                    activeCounter = Math.Max(629, activeCounter);

                    break;
            }

        }

        public static string DialogueIntros(string dialogue)
        {

            string intro = "";

            switch (dialogue)
            {

                case "townOne":

                    intro = "Jester of Fate: ^So this is the town of pelicans. I guess the humans killed all the birds when they took it over.";

                    break;

                case "townTwo":

                    intro = "Jester of Fate: ^He's one of the good ones, farmer. Makes me sad.";

                    break;

                case "townThree":

                    intro = "Jester of Fate: ^Any clue where the overly large fox is hiding?";

                    break;

                case "townFour":

                    intro = "Jester of Fate: ^I have no idea who actually won.";
                    
                    break;

                case "townFive":

                    intro = "Jester of Fate: ^I think we went too far.";

                    break;

                case "townSix":

                    intro = "Jester of Fate: ^Fortumei gathered all the high servants to the temple of Fate. " +
                        "When she decreed that a champion was needed to seek Thanatoshi, the only answer was silence. " +
                        "So I jested. I proclaimed, theatrically, that I would accomplish the task.";

                    break;

            }

            return intro;

        }


        public static List<Response> DialogueSetups(string dialogue)
        {

            List<Response> responseList = new();

            switch (dialogue)
            {
                case "townOne": //townOne

                    responseList.Add(new Response("townOne", "Not quite... it's probable that the town founders were the Pelican family."));
                    responseList.Add(new Response("townOne", "Indeed. It was quite an effort for us humans to overthrow our big billed overlords."));

                    break;

                case "townTwo":

                    responseList.Add(new Response("townTwo", "It is a shame that there aren't more capes and eyepatches in the valley."));
                    responseList.Add(new Response("townTwo", "Why would you be sad about Marlon?"));

                    break;

                case "townThree":

                    responseList.Add(new Response("townThree", "Behind you."));
                    responseList.Add(new Response("townThree", "Probably eating cookies and sipping whiskey with the Muellers."));
                    responseList.Add(new Response("townThree", "Within the insatiable maw of the town dog from which there is no escape."));
                    responseList.Add(new Response("townThree", "Just check the trash. Everyone else does... right?"));

                    break;

                case "townFour":

                    responseList.Add(new Response("townFour", "I saw both of you arrive after me."));
                    responseList.Add(new Response("townFour", "Buffin is quicker on the ground."));
                    responseList.Add(new Response("townFour", "You Jester, you're a powerhouse of motion when you want to be."));

                    break;

                case "townFive":

                    responseList.Add(new Response("townFive", "Don't worry. The Junimos have a knack for fixing this kind of thing."));
                    responseList.Add(new Response("townFive", "You have besmirched the venerable house of Joja, and the consequences will be severe."));
                    responseList.Add(new Response("townFive", "What kind of power does Buffin wield?"));

                    break;

                case "townSix":

                    responseList.Add(new Response("townSix", "You've done pretty well considering you never intended to get this far. Do you have any regrets?"));
                    responseList.Add(new Response("townSix", "That must have been quite the scene, with all those important Fates looking at you. Maybe some saw a fool, but some saw a hero. I see a hero."));

                    break;



            }

            return responseList;

        }

        public static void DialogueResponses(StardewDruid.Character.Character npc, string dialogue)
        {

            switch (dialogue)
            {

                case "townOne":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(101);

                    }
                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0",
                            "I wonder if the pelicans were still around, would they be able to tell us about the undervalley. Do you think they would choose to side with the Lord of the Deep against the rightful monarchs, or protect the sacred spaces like your circle of druids?"
                        )
                    );

                    Game1.drawDialogue(npc);

                    break;

                case "townTwo":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(102);

                    }
                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0",
                            "When I first came to the valley, I started at the last known position of Thanatoshi before he vanished in his pursuit of the fallen one. " +
                            "Even after a week I couldn't find anymore clues. So I cried a little. I even yelled a bit. Well sort of cat-screamed. " +
                            "I guess kind of loudly, because the one-eyed cheese-eating adventure man told me to shut up. " +
                            "Then he offered me a place by warm fire, and because of him I learned the legend of the star crater, and that it was excavated. " +
                            "Still, while he talked, I could hear the faint whispers of the oracles of the fates. " +
                            "They predict that the one eyed warrior will never cease his crusade against the shadows of the valley. " +
                            "One day he will lie in an unmarked grave, buried respectfully by the young shadow brute who bests him. " +
                            "Of course that might not happen if you help Shadowtin liberate the shadowfolk from the Lord of the Deep. "
                        )
                    );

                    Game1.drawDialogue(npc);

                    break;

                case "townThree":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(103);

                    }
                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0",
                            "Sounds like something the Buffoonette of Chaos would do. " +
                            "Most fates get confused or angry by her attempts at fun. I'm one of her only friends."
                        )
                    );

                    Game1.drawDialogue(npc);

                    break;

                case "townFour":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(104);

                    }
                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0",
                            "Heh, it didn't seem like Buffin put in much effort. She's usually very foxy. " +
                            "I guess there's something else going on with her. There always is."
                        )
                    );

                    Game1.drawDialogue(npc);

                    break;

                case "townFive":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(105);

                    }
                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0",
                            "Buffin serves the Stream of Chaos, one of the primordial forces spawned from Yoba's work. " +
                            "You could probably produce some chaos yourself if you mix some unlikely sources, like Star power and Lady power. " +
                            "(Jester sighs) I should listen to what Buffin has to say."
                        )
                    );

                    Game1.drawDialogue(npc);

                    break;

                case "townSix":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(106);

                    }
                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0",
                            "The faithful certainly took it as a joke. One heckler said the earth cats would chase me away. " +
                        "They said my sparkly star cape would be torn to shreds. " +
                        "Buffin projected an image of me in tears, stuck in a ditch. All of which ended up happening for real but besides that, Fortumei took me at my word. " +
                        "She smiled, and not from one of my classic zingers, but it was like she believed in me. " +
                        "I think she knew you would find me, and that we would form a great team of friends. So I have no regrets about my humiliation at court. " +
                        "We're going to find the fallen one and we're going to bring them to justice."
                        )
                    );

                    Game1.drawDialogue(npc);

                    break;

            }

        }

    }

}
