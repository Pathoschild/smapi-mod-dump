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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Dialogue;
using StardewDruid.Map;
using StardewDruid.Monster.Boss;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event.World
{
    public class Crate : EventHandle
    {

        public Dictionary<int, TemporaryAnimatedSprite> crateAnimations;

        public bool treasureValid;

        public Vector2 treasureVector;

        public Vector2 treasurePosition;

        public string treasureTerrain;

        public string treasureGlyph;

        public bool treasureClaim;

        public int chaseTimer;

        public bool claimEvent;

        public bool chaseEvent;

        public List<StardewValley.Monsters.Monster> chaseMonsters;

        public int claimCounter;

        public Crate(Rite rite)
            : base(rite.castVector, rite)
        {

            crateAnimations = new();

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 10;

            chaseMonsters = new();

        }

        public override void EventTrigger()
        {

            int layerWidth = targetLocation.map.Layers[0].LayerWidth;

            int layerHeight = targetLocation.map.Layers[0].LayerHeight;

            for(int i = 0; i < 10; i++)
            {

                int X = riteData.randomIndex.Next(3, layerWidth - 3);

                int Y = riteData.randomIndex.Next(3, layerHeight - 3);

                treasureVector = new Vector2(X, Y);

                if(ModUtility.NeighbourCheck(targetLocation,treasureVector,0).Count > 0)
                {

                    continue;

                }

                treasureTerrain = ModUtility.GroundCheck(targetLocation, treasureVector);

                switch (treasureTerrain)
                {

                    case "ground":

                        treasureGlyph = "GlyphEarth";

                        treasureValid = true;

                        break;

                    case "water":

                        treasureGlyph = "GlyphWater";

                        treasureValid = true;

                        break;

                }

                if(treasureValid)
                {

                    break;
                
                }

            }

            if(!treasureValid)
            {

                return;

            }

            treasurePosition = treasureVector * 64f;

            TreasureGlyph();

            Mod.instance.RegisterEvent(this,"crate");

        }

        public void TreasureGlyph()
        {

            animations.Clear();

            TemporaryAnimatedSprite radiusAnimation = new(0, 4000, 1, 1, (treasureVector * 64) - new Vector2(16, 16), false, false)
            {

                sourceRect = new(0, 0, 64, 64),

                sourceRectStartingPos = new Vector2(0, 0),

                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", treasureGlyph + ".png")),

                scale = 1.5f, //* size,

                layerDepth = 900f,

                rotationChange = (float)Math.PI / 1000,

                alpha = 0.75f,

            };

            targetLocation.temporarySprites.Add(radiusAnimation);

            animations.Add(radiusAnimation);

        }

        public override void RemoveMonsters()
        {

            if(chaseMonsters.Count > 0)
            {

                targetLocation.characters.Remove(chaseMonsters[0]);

                chaseMonsters.Clear();

            }

            base.RemoveMonsters();

        }

        public override void EventRemove()
        {

            RemoveMonsters();

            RemoveAnimations();

        }

        public override void EventDecimal()
        {
            
            if (chaseEvent)
            {

                if(chaseTimer > 1)
                {

                    if (!ModUtility.MonsterVitals(chaseMonsters[0], targetLocation))
                    {

                        chaseMonsters.Clear();

                        claimEvent = true;

                        chaseEvent = false;

                        treasurePosition = Game1.player.Position - new Vector2(0,64);

                        ClaimEvent();

                        return;

                    }

                }

            }

        }

        public override void EventInterval()
        {

            activeCounter++;

            if (claimEvent)
            {

                ClaimEvent();

                return;

            }

            if (chaseEvent)
            {

                chaseTimer++;

                return;

            }

            if (treasureClaim)
            {


                Mod.instance.specialCasts[targetLocation.Name].Add("crate");

                RemoveAnimations();

                if (treasureTerrain != "water" && randomIndex.Next(2) == 0)
                {

                    expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 60;

                    chaseEvent = true;

                    if(randomIndex.Next(2) == 0)
                    {

                        //chaseMonsters.Add(Mod.instance.SpawnMonster(targetLocation, treasureVector, new() {19, }, "random"));

                        StardewValley.Monsters.Monster scavenger = MonsterData.CreateMonster(19, treasureVector);

                        chaseMonsters.Add(scavenger);

                        targetLocation.characters.Add(scavenger);

                        scavenger.update(Game1.currentGameTime, targetLocation);

                        (chaseMonsters[0] as Scavenger).ChaseMode();

                        chaseMonsters[0].showTextAboveHead("mine!");

                    }
                    else
                    {

                        //chaseMonsters.Add(Mod.instance.SpawnMonster(targetLocation, treasureVector, new() { 20, }, "random"));
                        StardewValley.Monsters.Monster scavenger = MonsterData.CreateMonster(20, treasureVector);

                        chaseMonsters.Add(scavenger);

                        targetLocation.characters.Add(scavenger);

                        scavenger.update(Game1.currentGameTime, targetLocation);

                        (chaseMonsters[0] as Rogue).ChaseMode();

                        chaseMonsters[0].showTextAboveHead("heh heh");

                    }

                    Mod.instance.CastMessage("A thief has snatched the treasure!");

                }
                else
                {

                    claimEvent = true;

                    ClaimEvent();

                }

                return;

            }

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 5;

            if (activeCounter % 4 == 0)
            {

                if (!targetLocation.temporarySprites.Contains(animations.First()))
                {

                    TreasureGlyph();


                }
                else
                {
                    animations.First().reset();

                }

            }

        }

        public void ClaimEvent()
        {

            claimCounter++;

            if(targetLocation is MineShaft mineShaft)
            {

                Layer layer = targetLocation.map.GetLayer("Buildings");

                layer.Tiles[(int)treasureVector.X, (int)treasureVector.Y] = new StaticTile(layer, targetLocation.map.TileSheets[0], 0, mineShaft.mineLevel > 120 ? 174 : 173);

                Game1.player.TemporaryPassableTiles.Add(new Microsoft.Xna.Framework.Rectangle((int)treasureVector.X * 64, (int)treasureVector.Y * 64, 64, 64));

                Mod.instance.CastMessage("A way down has appeared");

                expireEarly = true;

                return;

            }

            if (claimCounter == 1)
            {

                TemporaryAnimatedSprite crate = new(0, 1000, 1, 1, treasurePosition + new Vector2(16, 0), false, false)
                {

                    sourceRect = new(0, 0, 32, 64),

                    sourceRectStartingPos = new Vector2(0, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "DragonCrate.png")),

                    scale = 1f, //* size,

                    layerDepth = 900f,

                    scaleChange = 0.001f,

                    motion = new Vector2(-0.016f, -0.064f),

                    timeBasedMotion = true,

                };

                targetLocation.temporarySprites.Add(crate);

                crateAnimations[1] = crate;

            }

            if (claimCounter == 2)
            {

                RemoveAnimations();

                targetLocation.playSound("doorCreak");

                TemporaryAnimatedSprite crateOpen = new(0, 150, 2, 1, treasurePosition - new Vector2(0, 64), false, false)
                {

                    sourceRect = new(32, 0, 32, 64),

                    sourceRectStartingPos = new Vector2(32, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "DragonCrate.png")),

                    scale = 2f, //* size,

                    layerDepth = 900f,

                };

                targetLocation.temporarySprites.Add(crateOpen);

                crateAnimations[4] = crateOpen;

                TemporaryAnimatedSprite crateGlitter = new(0, 300, 2, 2, treasurePosition - new Vector2(0, 64), false, false)
                {

                    sourceRect = new(96, 0, 32, 64),

                    sourceRectStartingPos = new Vector2(96, 0),

                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "DragonCrate.png")),

                    scale = 2f, //* size,

                    layerDepth = 900f,

                    timeBasedMotion = true,

                    delayBeforeAnimationStart = 300,

                };

                targetLocation.temporarySprites.Add(crateGlitter);

                crateAnimations[5] = crateGlitter;

            }

            if (claimCounter == 3)
            {

                ReleaseTreasure();

                targetLocation.playSound("yoba");

                expireEarly = true;

            }

        }

        public void ReleaseTreasure()
        {

            if (!Mod.instance.TaskList().ContainsKey("masterTreasure"))
            {

                Mod.instance.UpdateTask("lessonTreasure", 1);

                int etherIndex = Mod.instance.TaskList()["lessonTreasure"] - 1;

                Dictionary<int, List<string>> etherPages = DialogueData.EtherPages();

                List<string> etherPage = etherPages[etherIndex];

                Narrator etherNarrator = DialogueData.EtherNarrator(etherIndex);

                etherNarrator.BufferHUD(etherPage);

                Mod.instance.CastMessage("Ether-drenched letter added to Journal");

                Throw book = new Throw(Game1.player, treasurePosition - new Vector2(16,48), 102);

                book.throwFade = 0.0005f;

                book.throwHeight = 2;

                book.dontInventorise = true;

                book.ThrowObject();

            }

            int treasureIndex = SpawnData.HighTreasure(treasureTerrain);

            Throw treasure = new(Game1.player, treasurePosition, treasureIndex);

            treasure.throwFade = 0.0005f;

            treasure.throwHeight = 2;

            treasure.ThrowObject();

            Mod.instance.CastMessage("Found a " + treasure.objectInstance.Name);

        }

    }

}
