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
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Journal;
using StardewDruid.Monster;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using System;

namespace StardewDruid.Event.Challenge
{
    public class ChallengeWeald : EventHandle
    {

        public Queue<Vector2> trashQueue;

        public int trashCollected;

        public Batwing bossMonster;

        public List<TemporaryAnimatedSprite> trashAnimations = new();

        public Vector2 relicPosition;

        public ChallengeWeald()
        {

            mainEvent = true;

        }

        public override void TriggerInterval()
        {
            base.TriggerInterval();

            TemporaryAnimatedSprite newAnimation;

            List<Vector2> trashVectors = new()
            {
                new(origin.X+64,origin.Y-256),
                new(origin.X+128,origin.Y+256),
                new(origin.X+320,origin.Y-64),

            };

            if(trashAnimations.Count == 0)
            {

                foreach (Vector2 trashVector in trashVectors)
                {

                    Microsoft.Xna.Framework.Rectangle targetRectangle = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 168, 16, 16);

                    newAnimation = new(
                        "Maps\\springobjects",
                        targetRectangle,
                        trashVector - new Vector2(16, 0),
                        flipped: false,
                        0f,
                        Color.White * 0.5f
                    )
                    {
                        interval = 99999f,
                        totalNumberOfLoops = 99999,
                        scale = 3f,
                    };

                    location.temporarySprites.Add(newAnimation);

                    trashAnimations.Add(newAnimation);

                }

            }
            else
            {

                foreach(TemporaryAnimatedSprite trashAnimation in trashAnimations)
                {

                    trashAnimation.reset();

                }

            }

            foreach (Vector2 trashVector in trashVectors)
            {
                
                newAnimation = new(
                    "LooseSprites\\Cursors",
                    new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10),
                    trashVector,
                    flipped: false,
                    0.002f,
                    Color.Green
                )
                {
                    alpha = 0.75f,
                    motion = new Vector2(0f, -0.5f),
                    //acceleration = new Vector2(0.002f, 0f),
                    interval = 9999f,
                    layerDepth = 0.001f,
                    scale = 2f,
                    scaleChange = 0.02f,
                    rotationChange = Game1.random.Next(-5, 6) * MathF.PI / 256f,
                };

                location.temporarySprites.Add(newAnimation);

            }

        }

        public override void EventActivate()
        {

            base.EventActivate();

            monsterHandle = new(origin, location);

            monsterHandle.spawnSchedule = new();

            for (int i = 1; i <= 55; i += 4)
            {

                monsterHandle.spawnSchedule.Add(i, new() { new(MonsterHandle.bosses.batwing,4,Mod.instance.randomIndex.Next(2)) });

            }

            monsterHandle.spawnWithin = new(17, 10);

            monsterHandle.spawnRange = new(9, 9);

            monsterHandle.spawnWater = true;

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 65;

            eventProximity = 640;

            Mod.instance.CastMessage("Remain on the rite circle to increase trash collection", 2);

            SetTrack("tribal");

            TrashVectors();

            PortalAnimations();

            ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 600);

            location.playSound("discoverMineral");

        }

        public void PortalAnimations()
        {

            TemporaryAnimatedSprite radialAnimation = Mod.instance.iconData.DecorativeIndicator(location, origin, IconData.decorations.weald, 3f, new() { interval = 61000, rotation = 120, alpha = 0.5f, });

            animations.Add(radialAnimation);

            TemporaryAnimatedSprite skyAnimation = Mod.instance.iconData.SkyIndicator(location, origin, IconData.skies.mountain, 1f, new() { interval = 1000, alpha = 0.75f, });

            skyAnimation.scaleChange = 0.002f;

            skyAnimation.motion = new(-0.064f, -0.064f);

            skyAnimation.timeBasedMotion = true;

            animations.Add(skyAnimation);

            TemporaryAnimatedSprite skyAnimationTwo = Mod.instance.iconData.SkyIndicator(location, origin, IconData.skies.mountain, 3f, new() { interval = 60000, delay = 1000, alpha = 0.75f, });

            animations.Add(skyAnimationTwo);

        }

        public void TrashVectors()
        {

            Layer backLayer = location.Map.GetLayer("Back");

            trashQueue = new();

            trashCollected = 0;

            Vector2 from = ModUtility.PositionToTile(origin) + new Vector2(4,0);
            
            for (int i = 0; i < 3; i++)
            {

                List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(location, from, 2 + i);

                foreach (Vector2 castVector in castSelection)
                {

                    Tile backTile = backLayer.PickTile(new xTile.Dimensions.Location((int)castVector.X * 64, (int)castVector.Y * 64), Game1.viewport.Size);

                    if (backTile != null)
                    {

                        if (backTile.TileIndexProperties.TryGetValue("Water", out _))
                        {

                            if (Mod.instance.randomIndex.Next(2) == 0) { continue; };

                            trashQueue.Enqueue(castVector);

                        }

                    }

                }

            }

        }

        public override void RemoveMonsters()
        {

            if (bossMonster != null)
            {

                Mod.instance.rite.castLocation.characters.Remove(bossMonster);

                bossMonster = null;

            }

            base.RemoveMonsters();

        }

        public void RemoveTrashAnimations()
        {
            
            if (trashAnimations.Count > 0)
            {

                foreach (TemporaryAnimatedSprite animation in trashAnimations)
                {

                    location.temporarySprites.Remove(animation);

                }

                trashAnimations.Clear();

            }

        }

        public override void RemoveAnimations()
        {

            RemoveTrashAnimations();

            base.RemoveAnimations();
        }

        public override void EventInterval()
        {

            activeCounter++;

            monsterHandle.SpawnCheck();
            
            RemoveLadders();
            
            monsterHandle.SpawnInterval();

            if (activeCounter % 2 == 0 && Vector2.Distance(Game1.player.Position,origin) <= 256)
            {

                ThrowTrash();

            }

            if(activeCounter % 3 == 0)
            {


                Mod.instance.iconData.ImpactIndicator(location, origin, IconData.impacts.nature, 5f, new());


            }

            if (activeCounter == 30)
            {

                Mod.instance.CastMessage($"Cleanup is at the halfway point", 2);

            }

            if (activeCounter == 50)
            {

                Mod.instance.CastMessage($"Cleanup is almost complete", 2);

            }

            if (activeCounter == 20)
            {

                bossMonster = new(new Vector2(30, 11),Mod.instance.CombatDifficulty());

                bossMonster.SetMode(2);

                bossMonster.netPosturing.Set(true);

                bossMonster.netDirection.Set(2);

                bossMonster.netAlternative.Set(3);

                bossMonster.netScheme.Set(1);

                bossMonster.tempermentActive = Boss.temperment.cautious;

                Mod.instance.rite.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, Mod.instance.rite.castLocation);

                cues = DialogueData.DialogueScene(eventId);

                narrators = DialogueData.DialogueNarrators(eventId);

                voices[0] = bossMonster;

            }

            if (activeCounter <= 20)
            {
                return;
            }

            if (ModUtility.MonsterVitals(bossMonster,location))
            {

                DialogueCue(activeCounter);

                switch (activeCounter)
                {

                    case 39:

                        bossMonster.netPosturing.Set(false);

                        bossMonster.focusedOnFarmers = true; 
                        
                        break;

                    case 56:

                        bossMonster.Halt();

                        SpellHandle rockSpell = new(Game1.player, bossMonster.Position, 256, 999);

                        rockSpell.display = IconData.impacts.impact;

                        rockSpell.type = SpellHandle.spells.orbital;

                        rockSpell.projectile = 3;

                        rockSpell.scheme = IconData.schemes.rock;

                        rockSpell.sound = SpellHandle.sounds.explosion;

                        Mod.instance.spellRegister.Add(rockSpell);

                        break;

                    case 59:

                        bossMonster.takeDamage(999, 0, 0, false, 999, Game1.player);

                        break;

                    default: 
                        
                        break;

                }

                relicPosition = bossMonster.Position;

            }

            if(activeCounter == 60)
            {
                expireEarly = true;

                int friendship = trashCollected * 15;

                Mod.instance.CastMessage($"Collected {trashCollected} pieces of trash, gained " + friendship + " friendship with mountain residents", 2);

                VillagerData.CommunityFriendship("mountain", friendship);

                Mod.instance.questHandle.CompleteQuest(eventId);

                ThrowRelic();

            }

        }

        public void ThrowRelic()
        {

            ThrowHandle throwRelic = new(Game1.player, relicPosition, IconData.relics.minister_mitre);

            throwRelic.register();

            Mod.instance.relicsData.ReliquaryUpdate(IconData.relics.minister_mitre.ToString());

        }

        public void ThrowTrash()
        {

            if (trashQueue.Count == 0)
            {
                return;
            }

            Vector2 splash = trashQueue.Dequeue() * 64;

            Dictionary<int, int> artifactIndexes = new()
            {
                [0] = 105,
                [1] = 106,
                [2] = 110,
                [3] = 111,
                [4] = 112,
                [5] = 115,
                [6] = 117,
            };

            Dictionary<int, int> objectIndexes = new()
            {
                [0] = artifactIndexes[Mod.instance.randomIndex.Next(7)],
                [1] = 167,
                [2] = 168,
                [3] = 169,
                [4] = 170,
                [5] = 171,
                [6] = 172,
            };

            int objectIndex = objectIndexes[Mod.instance.randomIndex.Next(7)];

            ThrowHandle throwObject;

            if (trashCollected == 8)
            {

                throwObject = new(Game1.player, origin, new Ring("517"));


            }
            else if (trashCollected == 16)
            {

                throwObject = new(Game1.player, origin, new Ring("519"));

            }
            else
            {

                throwObject = new(splash, origin, objectIndex, 0) { pocket = true };

            }

            throwObject.register();

            location.playSound("pullItemFromWater");

            //Mod.instance.iconData.ImpactIndicator(location, splash, IconData.impacts.sparkle, 5f, new());

            Mod.instance.iconData.ImpactIndicator(location, splash, IconData.impacts.fish, 3f, new());

            trashCollected++;

        }


    }

}
