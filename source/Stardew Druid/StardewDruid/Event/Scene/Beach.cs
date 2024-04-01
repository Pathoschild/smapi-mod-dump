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
using StardewDruid.Character;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;

namespace StardewDruid.Event.Scene
{
    public class Beach : EventHandle
    {

        public StardewDruid.Character.Effigy companion;

        public StardewDruid.Monster.Template.BlobSlime blobking;

        public Vector2 campFire;

        public Vector2 blobVector;

        public Vector2 mistVector;

        public int dialogueCounter;

        public Beach(Vector2 target,  Quest quest)
          : base(target)
        {
            questData = quest;
        }

        public override void EventTrigger()
        {

            if (!Mod.instance.characters.ContainsKey("Effigy"))
            {

                return;

            }

            campFire = FireData.FireVectors(targetLocation);

            blobVector = campFire + new Vector2(16, 0);

            mistVector = campFire + new Vector2(40, 3);

            cues = DialogueScene();

            narrators = DialogueNarrator();

            companion = Mod.instance.characters["Effigy"] as StardewDruid.Character.Effigy;

            voices = new() { [0] = companion, };

            Mod.instance.RegisterEvent(this, "heartEffigy");

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 600.0;

        }

        public override void EventRemove()
        {

            companion.SwitchPreviousMode();

            base.EventRemove();

        }

        public override void EventInterval()
        {

            if(Game1.activeClickableMenu != null)
            {
                
                expireTime++;

                return;

            }

            activeCounter++;

            DialogueSpecial(activeCounter);

            DialogueCue(activeCounter);

            switch (activeCounter)
            {
                case 1:

                    companion.SwitchSceneMode();

                    companion.moveDirection = 3;

                    companion.netDirection.Set(3);

                    companion.eventName = "heartEffigy";

                    if (companion.currentLocation.Name != Mod.instance.rite.castLocation.Name)
                    {
                        companion.TargetIdle(30);
                        companion.currentLocation.characters.Remove(companion);
                        companion.currentLocation = Mod.instance.rite.castLocation;
                        companion.currentLocation.characters.Add(companion);
                    }

                    companion.Position = new(questData.triggerVector.X * 64f, questData.triggerVector.Y * 64f);

                    ModUtility.AnimateQuickWarp(Mod.instance.rite.castLocation, companion.Position - new Vector2(0.0f, 32f));

                    sceneCounter = 20;

                    break;

                case 3:

                    Mod.instance.CastMessage("Talk to the Effigy when '...' appears");

                    break;

                case 20:

                    companion.ResetActives();

                    companion.eventVectors.Clear();

                    companion.eventVectors.Add(21, companion.Position + new Vector2(-128, 0));

                    break;

                case 22:

                    companion.netCastActive.Set(true);

                    companion.netSpecialActive.Set(true);

                    companion.specialTimer = 60;

                    ModUtility.AnimateDecoration(targetLocation, companion.Position,"weald");

                    Vector2 cursor22 = (questData.triggerVector - new Vector2(6, 0)) * 64;

                    targetLocation.playSound("discoverMineral", cursor22, 800);

                    ModUtility.AnimateCursor(targetLocation, cursor22, "weald");

                    break;

                case 25:

                    companion.netCastActive.Set(true);

                    companion.netSpecialActive.Set(true);

                    companion.specialTimer = 60;

                    targetLocation.playSound("discoverMineral", companion.Position, 1600);
                    targetLocation.playSound("discoverMineral", companion.Position, 1200);

                    List<Vector2> vectors25 = new()
                    {

                        new(6,0),
                        new(8,2),
                        new(7,4),
                        new(7,-4),
                        new(9,-2),
                    };

                    ModUtility.AnimateDecoration(targetLocation, companion.Position, "weald");


                    for (int i = 0; i < vectors25.Count; i++)
                    {
                        
                        Vector2 cursor25 = (questData.triggerVector - vectors25[i]) * 64;

                        ModUtility.AnimateCursor(targetLocation, cursor25, "weald");

                    }

                    break;

                case 27:

                    companion.ResetActives();

                    companion.eventVectors.Add(39, (campFire * 64) - new Vector2(0, 128));

                    targetLocation.playSound("pullItemFromWater", companion.Position, 1200);

                    new Throw(targetPlayer, companion.Position, new StardewValley.Object("147", 1, quality: 4), companion.Position - new Vector2(256, 0)) { itemDebris = true }.AnimateObject();

                    break;

                case 28:
                case 29:
                case 30:

                    targetLocation.playSound("pullItemFromWater", companion.Position, 900);
                    targetLocation.playSound("pullItemFromWater", companion.Position, 1200);
                    targetLocation.playSound("pullItemFromWater", companion.Position, 1500);

                    List<Vector2> vectors28 = new()
                    {

                        new(6,0),
                        new(8,2),
                        new(7,4),
                        new(7,-4),
                        new(9,-2),
                    };

                    Vector2 position28 = questData.triggerVector * 64;

                    for (int i = 0; i < vectors28.Count; i++)
                    {

                        new Throw(targetPlayer, companion.Position, new StardewValley.Object("147", 1,quality:4), position28 - (vectors28[i] *64) - new Vector2(randomIndex.Next(384),randomIndex.Next(128))) { itemDebris = true }.AnimateObject(randomIndex.Next(9)*100);

                        new Throw(targetPlayer, companion.Position, new StardewValley.Object("147", 1, quality: 4), position28 - (vectors28[i] * 64) - new Vector2(randomIndex.Next(384), randomIndex.Next(128))) { itemDebris = true }.AnimateObject(randomIndex.Next(9) * 100);

                    }

                    break;

                case 40:

                    companion.ResetActives();

                    break;

                case 56:

                    companion.ResetActives();

                    companion.netCastActive.Set(true);

                    companion.netSpecialActive.Set(true);

                    companion.specialTimer = 60;

                    if (!targetLocation.objects.ContainsKey(campFire))
                    {
                        Torch camp = new("278",true)
                        {
                            Fragility = 1,
                            destroyOvernight = true
                        };

                        targetLocation.objects.Add(campFire, camp);

                    }

                    ModUtility.AnimateDecoration(targetLocation, companion.Position, "mists");

                    Vector2 cursor56 = campFire * 64;

                    ModUtility.AnimateCursor(targetLocation, cursor56, "mists");

                    ModUtility.AnimateBolt(targetLocation, campFire * 64 + new Vector2(32));

                    break;

                case 58:

                    Game1.playSound("fireball");

                    ModUtility.AnimateImpact(targetLocation, campFire * 64 + new Vector2(32), 1);

                    break;

                case 60:

                    companion.ResetActives();

                    companion.netCastActive.Set(true);

                    companion.netSpecialActive.Set(true);

                    companion.specialTimer = 60;

                    ModUtility.AnimateDecoration(targetLocation, companion.Position, "mists");

                    Vector2 cursor60 = campFire * 64;

                    ModUtility.AnimateCursor(targetLocation, cursor60, "mists");

                    ModUtility.AnimateBolt(targetLocation, campFire * 64 + new Vector2(32));

                    break;

                case 62:

                    Game1.playSound("fireball");

                    ModUtility.AnimateImpact(targetLocation, campFire * 64 + new Vector2(32), 2);

                    break;

                case 64:

                    companion.ResetActives();

                    companion.netCastActive.Set(true);

                    companion.netSpecialActive.Set(true);

                    companion.specialTimer = 60;

                    ModUtility.AnimateDecoration(targetLocation, companion.Position, "mists");

                    Vector2 cursor64 = campFire * 64;

                    ModUtility.AnimateCursor(targetLocation, cursor64, "mists");

                    ModUtility.AnimateBolt(targetLocation, campFire * 64 + new Vector2(32));

                    break;

                case 66:

                    Game1.playSound("fireball");

                    ModUtility.AnimateImpact(targetLocation, campFire * 64 + new Vector2(32), 3);

                    new Throw(targetPlayer, campFire * 64 - new Vector2(128, 0), new StardewValley.Object("728", 1), campFire * 64) { throwHeight = 3, dontInventorise = true, throwScale = 3f }.AnimateObject();

                    break;

                case 67:

                    new Throw(targetPlayer, campFire * 64 - new Vector2(256, 0), new StardewValley.Object("728", 1), campFire * 64 - new Vector2(128, 0)) { dontInventorise = true, throwScale = 3f }.AnimateObject();
                    
                    Game1.playSound("fireball");

                    ModUtility.AnimateImpact(targetLocation, (campFire - new Vector2(2, 0)) * 64 + new Vector2(32), 1);

                    break;

                case 68:

                    new Throw(targetPlayer, campFire * 64 - new Vector2(384, 0), new StardewValley.Object("728", 1), campFire * 64 - new Vector2(256, 0)) { throwScale = 3f }.AnimateObject();

                    Game1.playSound("fireball");

                    ModUtility.AnimateImpact(targetLocation, (campFire - new Vector2(4, 0)) * 64 + new Vector2(32), 1);

                    break;

                case 69:

                    Game1.playSound("fireball");

                    ModUtility.AnimateImpact(targetLocation, (campFire - new Vector2(6, 0)) * 64 + new Vector2(32), 2);

                    break;

                case 82:

                    companion.netLieActive.Set(true);

                    break;

                case 88:

                    blobking = new(blobVector*64,Mod.instance.CombatModifier());

                    blobking.posturing.Set(true);

                    targetLocation.characters.Add(blobking);

                    blobking.update(Game1.currentGameTime, targetLocation);

                    voices[1] = blobking;

                    break;

                case 92:

                    companion.ResetActives();

                    companion.netDirection.Set(1);

                    break;

                case 100:

                    companion.ResetActives();

                    companion.eventVectors.Add(85, (campFire + new Vector2(6,-1)) * 64);

                    break;

                case 111:

                    companion.moveTimer = companion.dashInterval;

                    companion.netDashActive.Set(true);

                    companion.dashSweep = true;

                    companion.NextTarget(blobking.Position - new Vector2(196,0), -1);

                    //---------------------- meteor animation

                    SpellHandle meteor = new(Mod.instance.rite.castLocation, blobking.Position, Game1.player.Position);

                    meteor.type = SpellHandle.barrages.meteor;

                    Mod.instance.spellRegister.Add(meteor);

                    ModUtility.AnimateDecoration(targetLocation, companion.Position, "stars");

                    break;

                case 112:

                    targetLocation.playSound("crit");

                    Microsoft.Xna.Framework.Rectangle blobBox = blobking.GetBoundingBox();

                    targetLocation.debris.Add(new Debris(9999, new Vector2(blobBox.Center.X + 16, blobBox.Center.Y), Microsoft.Xna.Framework.Color.Yellow, 2f, blobking));

                    blobking.Health = 0;

                    blobking.deathAnimation();

                    voices.Remove(1);

                    break;

                case 114:

                    companion.ResetActives();

                    companion.netDirection.Set(2);

                    targetLocation.characters.Remove(blobking);

                    blobking = null;

                    break;

                case 126:

                    List<Vector2> vectors182 = new()
                    {

                        new(5,0),
                        new(-6,-6),
                        new(6,4),
                        new(-5,5),
                        new(-7),
                        new(0,6)

                    };

                    for (int i = 0; i < vectors182.Count; i++)
                    {

                        Mod.instance.rite.CastWisps(mistVector + vectors182[i]);

                    }

                    companion.ResetActives();

                    companion.eventVectors.Add(131, mistVector*64);

                    break;

                case 143:

                    if(dialogueCounter < 5)
                    {

                        activeCounter = 223;

                    }

                    break;

                case 146:

                    companion.ResetActives();

                    companion.netDirection.Set(0);

                    break;

                case 148:

                    companion.netCastActive.Set(true);

                    companion.netSpecialActive.Set(true);

                    companion.specialTimer = 60;

                    Vector2 mistCorner = (mistVector - new Vector2(2, 6))*64;

                    for (int i = 0; i < 6; i++)
                    {

                        for (int j = 0; j < 6; j++)
                        {

                            if ((i == 0 || i == 3) && (j == 0 || j == 3))
                            {
                                continue;
                            }

                            Vector2 glowVector = mistCorner + new Vector2(i * 48, j * 48);

                            TemporaryAnimatedSprite glowSprite = new TemporaryAnimatedSprite(0, 5000f, 1, 12, glowVector, false, false)
                            {
                                sourceRect = new Microsoft.Xna.Framework.Rectangle(88, 1779, 30, 30),
                                sourceRectStartingPos = new Vector2(88, 1779),
                                texture = Game1.mouseCursors,
                                motion = new Vector2(-0.0004f + randomIndex.Next(5) * 0.0002f, -0.0004f + randomIndex.Next(5) * 0.0002f),
                                scale = 4f,
                                layerDepth = 991f,
                                timeBasedMotion = true,
                                alpha = 0.5f,
                                color = new Microsoft.Xna.Framework.Color(0.75f, 0.75f, 1f, 1f),
                            };

                            targetLocation.temporarySprites.Add(glowSprite);

                        }

                    }

                    TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(0, 5000f, 1, 12, mistCorner - new Vector2(64,64), false, false)
                    {
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(64, 64, 64, 64),
                        sourceRectStartingPos = new Vector2(128, 64),
                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Decorations.png")),
                        scale = 7.5f,
                        layerDepth = 990f,
                        timeBasedMotion = true,
                        rotationChange = -0.03f,
                        alpha = 0.2f,
                        color = new Microsoft.Xna.Framework.Color(0.75f, 0.75f, 1f, 1f)
                    };
                    targetLocation.temporarySprites.Add(temporaryAnimatedSprite);

                    Vector2 farmerVector = (mistVector + new Vector2(-2, -4)) * 64;

                    Actor firstFarmer = new(farmerVector, targetLocation.Name, "FirstFarmer");

                    actors.Add(firstFarmer);

                    targetLocation.characters.Add(firstFarmer);

                    firstFarmer.update(Game1.currentGameTime, targetLocation);

                    voices[2] = firstFarmer;

                    TemporaryAnimatedSprite farmerSprite = new TemporaryAnimatedSprite(0, 5000f, 1, 12, farmerVector - new Vector2(32,0), false, false)
                    {
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32),
                        sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "FirstFarmer.png")),
                        scale = 4f,
                        layerDepth = 992f,
                        alpha = 0.5f,
                    };
                    targetLocation.temporarySprites.Add(farmerSprite);

                    Vector2 ladyVector = (mistVector + new Vector2(2, -3)) * 64;

                    Actor ladyBeyond = new(ladyVector, targetLocation.Name, "LadyBeyond");

                    actors.Add(ladyBeyond);

                    targetLocation.characters.Add(ladyBeyond);

                    ladyBeyond.update(Game1.currentGameTime, targetLocation);

                    voices[3] = ladyBeyond;

                    TemporaryAnimatedSprite ladySprite = new TemporaryAnimatedSprite(0, 5000f, 1, 11, ladyVector - new Vector2(32, 0), false, false)
                    {
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32),
                        sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "LadyBeyond.png")),
                        scale =4f,
                        layerDepth = 992f,
                        alpha = 0.5f,
                        flipped = true,

                    };
                    targetLocation.temporarySprites.Add(ladySprite);

                    break;

                case 208:

                    companion.ResetActives();

                    companion.netDirection.Set(1);

                    break;

                case 224:

                    companion.ResetActives();

                    companion.netDirection.Set(1);

                    Mod.instance.CompleteQuest(questData.name);

                    EventQuery();

                    expireEarly = true;

                    break;

            }

        }

        public override void EventScene(int index)
        {

            switch (index)
            {

                case 21: // cast position

                    activeCounter = 21;

                    break;

                case 39: // camp position

                    activeCounter = 39;

                    break;

                case 131: // mist position

                    activeCounter = 131;

                    break;

                case 991: // first dialogue

                    activeCounter = Math.Max(18, activeCounter);

                    dialogueCounter++;

                    break;

                case 992: // second dialogue

                    activeCounter = Math.Max(54, activeCounter);

                    dialogueCounter++;

                    break;

                case 993: // third dialogue

                    activeCounter = Math.Max(80, activeCounter);

                    dialogueCounter++;

                    break;

                case 994: // fourth dialogue

                    activeCounter = Math.Max(125, activeCounter);

                    dialogueCounter++;

                    break;

                case 995: // fifth dialogue

                    activeCounter = Math.Max(143, activeCounter);

                    dialogueCounter++;

                    break;

                case 996: // six dialogue

                    activeCounter = Math.Max(223, activeCounter);

                    break;

            }

        }

        public static Dictionary<int, Dictionary<int, string>> DialogueScene()
        {

            Dictionary<int, Dictionary<int, string>> sceneDialogue = new()
            {

                [1] = new() { [0] = "A great day for the beach", },
                [3] = new() { [0] = "As my old friend would say", },

                [5] = new() { [0] = "...", },
                [8] = new() { [0] = "...", },
                [11] = new() { [0] = "...", },
                [14] = new() { [0] = "...", },
                [17] = new() { [0] = "...", },

                [19] = new() { [0] = "Time to reminisce", },
                [23] = new() { [0] = "?", },
                [25] = new() { [0] = "More is required", },
                [27] = new() { [0] = "!", },
                [29] = new() { [0] = "Hasten away!", },

                [40] = new() { [0] = "...", },
                [43] = new() { [0] = "...", },
                [46] = new() { [0] = "...", },
                [49] = new() { [0] = "...", },
                [52] = new() { [0] = "...", },

                [55] = new() { [0] = "Let us make a fish stew", },
                [58] = new() { [0] = "The first farmer would often ask for this", },
                [61] = new() { [0] = "BY THE POWER BEYOND THE SHORE", },
                [66] = new() { [0] = "Perfection", },

                [69] = new() { [0] = "...", },
                [72] = new() { [0] = "...", },
                [75] = new() { [0] = "...", },
                [78] = new() { [0] = "...", },

                [81] = new() { [0] = "We would often gaze at the sky", },
                [84] = new() { [0] = "That big cloud might have once been a dragon", },
                [87] = new() { [0] = "Things are simpler now",},
                [90] = new() { [1] = "Ha ha ha. The wooden puppet returns.", },
                [93] = new() { [0] = "Blobbins. The wretched fiend", },
                [96] = new() { [1] = "I heard your creaky, broken voice", },
                [99] = new() { [1] = "And came to laugh at you", },
                [102] = new() { [0] = "Have you no fear?", },
                [105] = new() { [1] = "Your friends are gone, your power spent", },
                [108] = new() { [1] = "You can no longer guard the change", },
                [111] = new() { [0] = "Enough of you!", },

                [114] = new() { [0] = "...", },
                [117] = new() { [0] = "...", },
                [120] = new() { [0] = "...", },
                [123] = new() { [0] = "...", },

                [126] = new() { [0] = "The spirits of the Weald gather", },
                [129] = new() { [0] = "This is the shore I remember", },

                [132] = new() { [0] = "...", },
                [135] = new() { [0] = "...", },
                [138] = new() { [0] = "...", },
                [141] = new() { [0] = "...", },

                [144] = new() { [0] = "A memory springs to mind", },
                [147] = new() { [0] = "A fragment of the past", },
                [150] = new() { [3] = "Why are you upset?", },
                [153] = new() { [2] = "It's just a bit... sudden.", },
                [156] = new() { [3] = "The health of my kin deteriorates.", },
                [159] = new() { [3] = "I would have realised sooner...", },
                [162] = new() { [3] = "...had I not been... distracted", },
                [165] = new() { [2] = "So you'll go across the sea then", },
                [168] = new() { [3] = "I must care for them in their slumber", },
                [171] = new() { [2] = "What about the valley, our circle?", },
                [174] = new() { [3] = "You have talents, space... and our friend", },
                [177] = new() { [2] = "He needs you more than he does me", },
                [180] = new() { [3] = "You must continue to mentor him", },
                [183] = new() { [3] = "Keep him safe from the Fates", },
                [186] = new() { [3] = "Show him the beauty of the valley", },
                [189] = new() { [2] = "It's not enough for me", },
                [192] = new() { [3] = "??", },
                [194] = new() { [2] = "Please. Stay.", },
                [197] = new() { [3] = "I have given you all the time I can", },
                [200] = new() { [3] = "Goodbye, farmer", },
                [203] = new() { [2] = "Lady...", },
                [206] = new() { [0] = "He was never the same after this", },
                [209] = new() { [0] = "Will I ever understand why?", },

                [212] = new() { [0] = "...", },
                [215] = new() { [0] = "...", },
                [218] = new() { [0] = "...", },
                [221] = new() { [0] = "...", },

                [224] = new() { [0] = "The valley is beautiful", },

            };

            return sceneDialogue;

        }

        public void DialogueSpecial(int index)
        {

            switch (index)
            {

                case 5:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy", "beachOne");

                    break;

                case 18: // clear

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy");

                    break;

                case 40:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy", "beachTwo");

                    break;

                case 54:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy");

                    break;

                case 69:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy", "beachThree");

                    break;

                case 80:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy");

                    break;

                case 114:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy", "beachFour");

                    break;

                case 125:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy");

                    break;

                case 132:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy", "beachFive");

                    break;

                case 142:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy");

                    break;

                case 212:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy", "beachSix");

                    break;

                case 222:

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy");

                    break;
            }

        }

        public static Dictionary<int, Dialogue.Narrator> DialogueNarrator()
        {

            Dictionary<int, Dialogue.Narrator> sceneNarrator = new()
            {
                [0] = new("Forgotten Effigy", Microsoft.Xna.Framework.Color.Green),
                [1] = new("Slimey Blobbins", Microsoft.Xna.Framework.Color.White),
                [2] = new("First Farmer", Microsoft.Xna.Framework.Color.Yellow),
                [3] = new("Lady Beyond", Microsoft.Xna.Framework.Color.Blue),
            };

            return sceneNarrator;

        }

        public static string DialogueIntros(string dialogue)
        {

            string intro;

            switch (dialogue)
            {

                case "beachTwo":

                    intro = "I seemed to have lost some sensitivity to the power of the Two Kings. " +
                        "I've only ever possessed a small amount of talent in that regard.";

                    break;

                case "beachThree":

                    intro = "That peculiar cooking technique was taught to me by the Lady herself. " +
                        "It was a favourite of the first farmer's. He was her champion.";

                    break;

                case "beachFour":

                    intro = "Blobbins was correct.";

                    break;

                case "beachFive":

                    intro = "Thank you for displaying an interest in my former life.";

                    break;

                case "beachSix":

                    intro = "I witnessed this, many ages ago, when I was still new to this world. " +
                        "I was unable to assist my friend with his troubles, and I was unable to advance the cause of the circle on my own. I am deficient.";

                    break;

                default: // beachOne

                    intro = "Here again. At the hem of the valley.";

                    break;

            }

            return intro;


        }

        public static List<Response> DialogueSetups(string dialogue)
        {

            List<Response> responseList = new();

            switch (dialogue)
            {

                case "beachTwo":

                    responseList.Add(new Response("beachTwo", "I thought what you just did was great."));
                    responseList.Add(new Response("beachTwo", "I'm surprised you consider yourself untalented in the Weald."));

                    break;

                case "beachThree":

                    responseList.Add(new Response("beachThree", "You once told me the first farmer knew her."));
                    responseList.Add(new Response("beachThree", "So it's true then, you were created with the Lady's power?"));

                    break;

                case "beachFour":

                    responseList.Add(new Response("beachFour", "Sociopathic slime monsters say all sorts of smack."));
                    responseList.Add(new Response("beachFour", "That blob was the most disgusting thing I've ever seen... today."));

                    break;

                case "beachFive":

                    responseList.Add(new Response("beachFive", "Are those... wisps?"));

                    break;

                case "beachSix":

                    responseList.Add(new Response("beachSix", "You kept the circle active in the valley all this time. You have not been deficient."));
                    responseList.Add(new Response("beachSix", "Your friend was heartbroken. Such is life. It wasn't your fault."));

                    break;

                default: //beachOne

                    responseList.Add(new Response("beachOne", "Is the beach how you remember it?"));

                    break;

            }

            return responseList;

        }

        public static void DialogueResponses(StardewDruid.Character.Character npc, string dialogue)
        {

            switch (dialogue)
            {

                case "beachTwo":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(992);

                    }

                    npc.CurrentDialogue.Push(new(npc, "0", "I feel a resistance when I entreat the aid of the Two Kings. A slowness to respond. " +
                        "It has never been easy to perform my duty as a steward of the Weald. " +
                        "The farm and grove of my former masters want for a better caretaker. "));

                    Game1.drawDialogue(npc);

                    break;

                case "beachThree":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(993);

                    }

                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0", 
                            "The first farmer and the Lady worked together in the aftermath of the war to revitalise the valley. " +
                            "From the moment I awoke in this form, the circle of druids has been my mission, and my home. We built a little utopia in the valley, the first farm. " +
                            "Then we rested and talked about the legends of our time. " +
                            "But the dragons disappeared, and the elderborn departed, and the humans were left with all their creations. I have lost count of the days since that time."
                        )
                    );

                    Game1.drawDialogue(npc);

                    break;

                case "beachFour":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(994);

                    }

                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0",
                            "I am not an adequate guardian of the change. " +
                            "Such duties have fallen to others, such as yourself, and the wizard and his ally. " +
                            "Even the slime can see it. I have failed my old friend."
                        )
                    );

                    Game1.drawDialogue(npc);
                    break;

                case "beachFive":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(995);

                    }

                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0",
                            "The will of the sleeping monarchs, their dreams and promises, become the wisps. " +
                            "In a gentle moment, they revealed themselves to me, and continued to keep me company in the long period that I awaited for my friend's successor. " +
                            "Now they reveal themselves to you too. It is a special privilege."
                        )
                    );

                    Game1.drawDialogue(npc);

                    break;

                case "beachSix":

                    if (Context.IsMainPlayer)
                    {

                        npc.UpdateEvent(996);

                    }

                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0",
                            "The Lady fulfilled her duty to the Weald and to Yoba. " +
                            "After a time, I began to hear her voice in the storms, and mist would blanket the valley at odd times. " +
                            "It became a mark of her presence, a sign of her enduring affection for the sacred places. " +
                            "The circle weakened in her absence. The First Farmer's attention shifted to other matters, and he began to neglect his duty. " +
                            "He become obsessed with fanciful ideas, and a desire for something beyond his grasp. Then he left."
                        )
                    );

                    Game1.drawDialogue(npc);

                    break;


                default: //beachOne

                    if (Context.IsMainPlayer)
                    {
                        
                        npc.UpdateEvent(991);

                    }
                    npc.CurrentDialogue.Push(
                        new(
                            npc, "0",
                            "The sands and waves glimmer as they once did, but I think I remember them differently. " +
                            "Perhaps they will appear more familiar when I've spent some time here."
                        )
                    );

                    Game1.drawDialogue(npc);

                    break;

            }
        }
    }

}