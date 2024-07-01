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
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Cast.Weald;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Event;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using static StardewDruid.Cast.SpellHandle;

namespace StardewDruid.Character
{
    public class Jester : StardewDruid.Character.Character
    {

        public Dictionary<int, List<Rectangle>> runningFrames;

        public Jester()
        {
        }

        public Jester(CharacterHandle.characters characterType)
          : base(characterType)
        {
            
        }

        public override void LoadOut()
        {

            characterType = CharacterHandle.characters.Jester;

            characterTexture = CharacterHandle.CharacterTexture(characterType);

            modeActive = mode.random;

            collidePriority = new Random().Next(20);

            overhead = 112;

            gait = 1.6f;

            moveInterval = 12;

            haltFrames = FrameSeries(32, 32, 0, 0, 1);

            alertFrames = haltFrames;

            walkFrames = FrameSeries(32, 32, 0, 0, 7);

            runningFrames = FrameSeries(32, 32, 0, 128, 6, FrameSeries(32, 32, 0, 0, 1));

            idleFrames = new()
            {
                [0] = new(){
                    new(0, 256, 32, 32),
                    new(32, 256, 32, 32),
                    new(64, 256, 32, 32),
                    new(96, 256, 32, 32),
                    //new(128, 256, 32, 32),
                    //new(160, 256, 32, 32),
                },

            };

            specialFrames = new()
            {

                [0] = new() { new(192, 192, 32, 32), },

                [1] = new() { new(192, 160, 32, 32), },

                [2] = new() { new(192, 128, 32, 32), },

                [3] = new() { new(192, 224, 32, 32), },

            };

            cooldownInterval = 300;

            specialInterval = 30;

            specialCeiling = 1;

            specialFloor = 1;

            sweepFrames = FrameSeries(32, 32, 0, 128, 3);

            sweepInterval = 9;

            workFrames = new()
            {

                [0] = new() {new(0, 288, 32, 32),
                    new(32, 288, 32, 32),
                    new(64, 288, 32, 32),
                    new(32, 288, 32, 32),
                },

            };

            dashPeak = 128;

            dashInterval = 9;

            dashFrames = new(sweepFrames);

            dashFrames[4] = new() { new(64, 192, 32, 32), };
            dashFrames[5] = new() { new(64, 160, 32, 32), };
            dashFrames[6] = new() { new(64, 128, 32, 32), };
            dashFrames[7] = new() { new(64, 192, 32, 32), };

            dashFrames[8] = new() { new(96, 192, 32, 32), new(128, 192, 32, 32), new(160, 192, 32, 32), };
            dashFrames[9] = new() { new(96, 160, 32, 32), new(128, 160, 32, 32), new(160, 160, 32, 32), };
            dashFrames[10] = new() { new(96, 128, 32, 32), new(128, 128, 32, 32), new(160, 128, 32, 32), };
            dashFrames[11] = new() { new(96, 192, 32, 32), new(128, 192, 32, 32), new(160, 192, 32, 32), };

            smashFrames = new(dashFrames);

            loadedOut = true;

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {

            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if (characterTexture == null)
            {

                return;

            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            DrawEmote(b);

            if (netStandbyActive.Value)
            {

                DrawStandby(b, localPosition, drawLayer);

                DrawShadow(b, localPosition, drawLayer);

                return;

            }
            else if (netHaltActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(28, 56f),
                    haltFrames[netDirection.Value][0],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3.75f,
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else if (netSweepActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(28, 56f),
                    sweepFrames[netDirection.Value][sweepFrame],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3.75f,
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else if (netDashActive.Value)
            {

                int dashSeries = netDirection.Value + (netDashProgress.Value * 4);

                int dashSetto = Math.Min(dashFrame, (dashFrames[dashSeries].Count - 1));

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(28, 56f + dashHeight),
                    dashFrames[dashSeries][dashSetto],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3.75f,
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else if (netSmashActive.Value)
            {
                
                int smashSeries = netDirection.Value + (netDashProgress.Value * 4);

                int smashSetto = Math.Min(dashFrame, (smashFrames[smashSeries].Count - 1));

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(28, 56f + dashHeight),
                    smashFrames[smashSeries][smashSetto],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    3.75f,
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else if (netWorkActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(24, 56f),
                    workFrames[0][specialFrame],
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    3.75f,
                    SpriteEffects.None,
                    drawLayer
                );

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(28, 56f),
                    specialFrames[netDirection.Value][0], 
                    Color.White, 
                    0.0f,
                    Vector2.Zero,
                    3.75f,
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, 
                    drawLayer
                );

            }
            else
            {

                if (TightPosition() && idleTimer > 0 && currentLocation.IsOutdoors && !netSceneActive.Value)
                {

                    DrawStandby(b, localPosition, drawLayer);

                    DrawShadow(b, localPosition, drawLayer);

                    return;

                }

                if (pathActive == pathing.running)
                {
                    b.Draw(
                        characterTexture,
                        localPosition - new Vector2(28, 56f),
                        runningFrames[netDirection.Value][moveFrame],
                        Color.White,
                        0f,
                        Vector2.Zero,
                        3.75f,
                        (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        drawLayer
                    );
                }
                else
                {
                    b.Draw(
                        characterTexture,
                        localPosition - new Vector2(28, 56f),
                        walkFrames[netDirection.Value][moveFrame],
                        Color.White,
                        0f,
                        Vector2.Zero,
                        3.75f,
                        (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        drawLayer
                    );

                }


            }

            DrawShadow(b, localPosition, drawLayer);

        }

        public override void DrawStandby(SpriteBatch b, Vector2 localPosition, float drawLayer)
        {

            int chooseFrame = IdleFrame();

            b.Draw(
                characterTexture,
                localPosition - new Vector2(28, 56f),
                idleFrames[0][chooseFrame],
                Color.White,
                0f,
                Vector2.Zero,
                3.75f,
                netDirection.Value == 3 || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer
            );

            return;

        }

        public override Rectangle GetBoundingBox()
        {

            if (netDirection.Value % 2 == 0)
            {

                return new Rectangle((int)Position.X + 8, (int)Position.Y + 8, 48, 48);

            }

            return new Rectangle((int)Position.X - 16, (int)Position.Y + 8, 96, 48);

        }

        public override bool SpecialAttack(StardewValley.Monsters.Monster monster)
        {

            if (!Mod.instance.questHandle.IsComplete(Journal.QuestHandle.questJester))
            {

                return false;

            }

            ResetActives();

            netSpecialActive.Set(true);

            specialTimer = 60;

            cooldownTimer = cooldownInterval;

            LookAtTarget(monster.Position, true);

            SpellHandle beam = new(Game1.player,new() { monster, },Mod.instance.CombatDamage() / 2);

            beam.origin = Position;

            beam.type = SpellHandle.spells.beam;

            beam.scheme = IconData.schemes.Void;

            Mod.instance.spellRegister.Add(beam);

            return true;

        }

        public override void CompanionBuff(Farmer player)
        {

            BuffEffects buffEffect = new();

            buffEffect.LuckLevel.Set(1);

            Buff luckBuff = new("184656", source: "Jester of Fate", displaySource: "Jester of Fate", duration: Buff.ENDLESS, displayName: "Jester's Luck", description: "Luck increased by companion", effects: buffEffect);

            player.buffs.Apply(luckBuff);

        }

        public override void RemoveCompanionBuff(Farmer player)
        {
            
            if (player.buffs.IsApplied("184656"))
            {

                player.buffs.Remove("184656");

            }

        }

        public override void ConnectSweep()
        {
                
            SpellHandle swipeEffect = new(Game1.player, Position, 192, Mod.instance.CombatDamage() / 2);

            swipeEffect.instant = true;

            swipeEffect.sound = sounds.swordswipe;

            swipeEffect.display = IconData.impacts.deathwhirl;

            if (Mod.instance.questHandle.IsGiven(Journal.QuestHandle.fatesTwo))
            {
                switch (Mod.instance.randomIndex.Next(4))
                {
                    case 0:

                        swipeEffect.added.Add(SpellHandle.effects.daze);
                        break;

                    case 1:

                        swipeEffect.added.Add(SpellHandle.effects.mug);
                        break;

                    case 2:

                        swipeEffect.added.Add(SpellHandle.effects.morph);
                        break;

                    default:
                    case 3:

                        swipeEffect.added.Add(SpellHandle.effects.doom);
                        break;
                }
            }
            else
            {
                swipeEffect.added.Add(SpellHandle.effects.push);

            }

            Mod.instance.spellRegister.Add(swipeEffect);

        }

        public override bool TargetWork()
        {

            if (currentLocation.characters.Count > 0 && !Mod.instance.eventRegister.ContainsKey("active"))
            {

                foreach (NPC rubVictim in currentLocation.characters)
                {

                    if (rubVictim is StardewValley.Monsters.Monster)
                    {
                        continue;
                    }

                    if (rubVictim is StardewDruid.Character.Character)
                    {
                        continue;
                    }

                    if (rubVictim is StardewDruid.Character.Dragon)
                    {
                        continue;
                    }

                    if (rubVictim is Pet)
                    {
                        continue;
                    }

                    if (!Game1.NPCGiftTastes.ContainsKey(rubVictim.Name))
                    {
                        continue;
                    }

                    if (Mod.instance.Witnessed(ReactionData.reactions.jester, rubVictim))
                    {
                        continue;
                    }

                    if (Vector2.Distance(rubVictim.Position, Position) < 480)
                    {

                        bool alreadyRubbed = Game1.player.hasPlayerTalkedToNPC(rubVictim.Name);

                        if (alreadyRubbed)
                        {

                            continue;

                        }

                        if (ModUtility.GroundCheck(currentLocation, rubVictim.Tile - new Vector2(1, 0), true) != "ground")
                        {

                            continue;

                        }

                        if (Game1.player.friendshipData.ContainsKey(rubVictim.Name))
                        {

                            Game1.player.friendshipData[rubVictim.Name].TalkedToToday = true;

                            Game1.player.changeFriendship(25, rubVictim);

                        }

                        rubVictim.Halt();

                        rubVictim.faceDirection(2);

                        ReactionData.ReactTo(rubVictim, ReactionData.reactions.jester);

                        Position = rubVictim.Position - new Vector2(64, 0);

                        SettleOccupied();

                        Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position);

                        workVector = rubVictim.Position;

                        netWorkActive.Set(true);

                        netSpecialActive.Set(true);

                        specialTimer = 120;

                        return true;

                    }

                }

            }

            if (currentLocation is Farm farmLocation)
            {

                foreach (KeyValuePair<long, FarmAnimal> pair in farmLocation.animals.Pairs)
                {

                    if (pair.Value.wasPet.Value)
                    {

                        continue;

                    }

                    if (Vector2.Distance(pair.Value.Position, Position) >= 480)
                    {

                        continue;

                    }

                    if (ModUtility.GroundCheck(currentLocation, pair.Value.Tile - new Vector2(1, 0), true) != "ground")
                    {

                        continue;

                    }

                    pair.Value.Halt();

                    pair.Value.faceDirection(2);

                    ModUtility.PetAnimal(Game1.player, pair.Value);

                    if(pair.Value.Sprite.SpriteWidth > 16)
                    {

                        Position = pair.Value.Position + new Vector2(-32, 32);

                    }
                    else
                    {
                        
                        Position = pair.Value.Position - new Vector2(64, 0);

                    }


                    SettleOccupied();

                    Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position);

                    workVector = pair.Value.Position;

                    netWorkActive.Set(true);

                    netSpecialActive.Set(true);

                    specialTimer = 120;

                    return true;

                }

            }

            if (currentLocation is AnimalHouse animalLocation)
            {

                foreach (KeyValuePair<long, FarmAnimal> pair in animalLocation.animals.Pairs)
                {
                    if (pair.Value.wasPet.Value)
                    {

                        continue;

                    }

                    if (Vector2.Distance(pair.Value.Position, Position) >= 480)
                    {

                        continue;

                    }

                    if (ModUtility.GroundCheck(currentLocation, pair.Value.Tile - new Vector2(1, 0), true) != "ground")
                    {

                        continue;

                    }

                    pair.Value.Halt();

                    pair.Value.faceDirection(2);

                    ModUtility.PetAnimal(Game1.player, pair.Value);

                    if (pair.Value.Sprite.SpriteWidth > 16)
                    {

                        Position = pair.Value.Position + new Vector2(-32, 32);

                    }
                    else
                    {

                        Position = pair.Value.Position - new Vector2(64, 0);

                    }

                    SettleOccupied();

                    Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position);

                    netWorkActive.Set(true);

                    netSpecialActive.Set(true);

                    specialTimer = 120;

                    return true;

                }

            }
            return false;

        }

    }

}
