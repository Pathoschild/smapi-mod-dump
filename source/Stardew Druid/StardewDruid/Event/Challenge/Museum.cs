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
using StardewDruid.Map;
using StardewDruid.Monster;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewDruid.Event.Challenge
{
    public class Museum : ChallengeHandle
    {
        public bool modifiedLocation;
        public BossDino bossMonster;
        public Vector2 returnPosition;
        public NPC Gunther;

        public Museum(Vector2 target, Rite rite, Quest quest)
          : base(target, rite, quest)
        {
            targetVector = target;
            voicePosition = (targetVector*64f)+ new Vector2(0.0f, -32f);
            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 60.0;
            returnPosition = rite.caster.Position;
            Gunther = targetLocation.getCharacterFromName(nameof(Gunther));
        }

        public override void EventTrigger()
        {
            ModUtility.AnimateRadiusDecoration(targetLocation, targetVector, "Mists", 1f, 1f);
            ModUtility.AnimateBolt(targetLocation, targetVector);
            Mod.instance.RegisterEvent(this, "active");
            AddActor(voicePosition, true);
        }

        public override bool EventActive()
        {
            if (targetPlayer.currentLocation == targetLocation && !eventAbort)
            {
                double totalSeconds = Game1.currentGameTime.TotalGameTime.TotalSeconds;
                if (expireTime < totalSeconds || expireEarly)
                    return EventExpire();
                int num = (int)Math.Round(expireTime - totalSeconds);
                if (activeCounter != 0 && num % 10 == 0 && num != 0)
                    Game1.addHUDMessage(new HUDMessage(string.Format("{0} more minutes left!", num), "2"));
                return true;
            }
            EventAbort();
            return false;
        }

        public override void RemoveMonsters()
        {
            if (bossMonster != null)
            {
                riteData.castLocation.characters.Remove(bossMonster);
                bossMonster = null;
            }
            base.RemoveMonsters();
        }

        public override void EventRemove()
        {
            ResetLocation();
            base.EventRemove();
        }

        public override bool EventExpire()
        {
            if (eventLinger == -1)
            {
                RemoveMonsters();
                ResetLocation();
                eventLinger = 4;
                return true;
            }
            if (eventLinger == 3)
            {
                if (expireEarly)
                {
                    GuntherVoice("A glorious battle");
                    Vector2 vector2 = Gunther.getTileLocation()+ new Vector2(0.0f, 1f);
                    if (!questData.name.Contains("Two"))
                        Game1.createObjectDebris(74, (int)vector2.X, (int)vector2.Y, -1, 0, 1f, null);
                    Mod.instance.CompleteQuest(questData.name);
                }
                else
                {
                    GuntherVoice("ughh... what a mess");
                    Mod.instance.CastMessage("Try again tomorrow");
                }
            }
            return base.EventExpire();
        }

        public override void EventInterval()
        {
            ++activeCounter;
            if (eventLinger != -1)
                return;
            if (activeCounter < 6)
            {
                switch (activeCounter)
                {
                    case 1:
                        CastVoice("croak");
                        GuntherVoice("Farmer? What's this?");
                        using (List<NPC>.Enumerator enumerator = targetLocation.characters.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                NPC current = enumerator.Current;
                                if (current.isVillager() && current.Name != "Gunther")
                                    current.doEmote(8, true);
                            }
                            break;
                        }
                    case 3:
                        GuntherVoice("Oh... oh no no no");
                        break;
                    case 4:
                        CastVoice("CROAK");
                        break;
                    case 5:
                        GuntherVoice("Protect the library!");
                        break;
                }
                Vector2 vector2 = targetVector + new Vector2(0.0f, 1f) - new Vector2(randomIndex.Next(7), randomIndex.Next(3));
                ModUtility.AnimateRadiusDecoration(targetLocation, vector2, "Mists", 1f, 1f);
                ModUtility.AnimateBolt(targetLocation, vector2);
            }
            else if (activeCounter == 6)
            {
                ModifyLocation();
                bossMonster = MonsterData.CreateMonster(14, targetVector+new Vector2(2f, 0.0f), riteData.combatModifier) as BossDino;
                if (questData.name.Contains("Two"))
                    bossMonster.HardMode();
                riteData.castLocation.characters.Add(bossMonster);
                bossMonster.update(Game1.currentGameTime, riteData.castLocation);
                SetTrack("heavy");
            }
            else if (bossMonster.defeated || bossMonster.Health <= 0 || bossMonster == null || !riteData.castLocation.characters.Contains(bossMonster))
            {
                expireEarly = true;
            }
            else
            {
                switch (activeCounter)
                {
                    case 10:
                        GuntherVoice("What have I got to throw here...");
                        GuntherThrowRandomShit();
                        break;
                    case 15:
                        GuntherVoice("Pre-cretacious creep!");
                        break;
                    case 20:
                        GuntherVoice("It's defacing my inlaid hardwood panelling!");
                        GuntherThrowRandomShit();
                        break;
                    case 25:
                        GuntherVoice("I need a weapon... but I loaned them all to Zuzu Mid");
                        break;
                    case 30:
                        GuntherVoice("Marlon has a lot to answer for");
                        GuntherThrowRandomShit();
                        break;
                    case 35:
                        GuntherVoice("Tell the guildmaster I wont accept any more cursed artifacts!");
                        break;
                    case 40:
                        GuntherVoice("How are you doing these amazing feats of magic?");
                        GuntherThrowRandomShit();
                        break;
                    case 41:
                        bossMonster.showTextAboveHead("Stop throwing things at me old man!", -1, 2, 3000, 0);
                        bossMonster.dialogueTimer = 300;
                        break;
                    case 45:
                        GuntherVoice("Can't you perform a rite of banishment or something?");
                        break;
                    case 50:
                        GuntherVoice("Goodbye, priceless artifact. Sniff.");
                        GuntherThrowRandomShit();
                        break;
                    case 55:
                        GuntherVoice("Leave the corpse. I might be able to sell it's parts.");
                        break;
                    case 59:
                        GuntherVoice("This is going to cost the historic trust society");
                        GuntherThrowRandomShit();
                        expireEarly = true;
                        break;
                }
            }
        }

        public void GuntherVoice(string speech)
        {
            Gunther.showTextAboveHead(speech, 3000, 2, 3000, 0);
        }

        public void GuntherThrowRandomShit()
        {
            List<int> intList = new List<int>()
            {
                96,
                97,
                98,
                99,
                100,
                101,
                103,
                104,
                105,
                106,
                107,
                108,
                109,
                110,
                111,
                112,
                113,
                114,
                115,
                116,
                117,
                118,
                119,
                120,
                121,
                122,
                123,
                124,
                125,
                126,
                sbyte.MaxValue,
                579,
                580,
                581,
                582,
                583,
                584,
                585,
                586,
                587,
                588,
                589
            };
            new Throw(targetPlayer, bossMonster.Position, new StardewValley.Object(intList[riteData.randomIndex.Next(intList.Count)], 1, false, -1, 0), Gunther.Position).AnimateObject();
        }

        public void ResetLocation()
        {
            if (!modifiedLocation)
                return;
            targetLocation.loadMap(targetLocation.mapPath.Value, true);
            foreach (NPC character in targetLocation.characters)
            {
                if (character.isVillager() && character.Name != "Gunther" && character.IsInvisible)
                    character.IsInvisible = false;
            }
            if (Game1.eventUp || Game1.fadeToBlack || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen || !(Game1.player.currentLocation is LibraryMuseum))
                return;
            Game1.fadeScreenToBlack();
            targetPlayer.Position = returnPosition;
            if (soundTrack)
            {
                Game1.stopMusicTrack(0);
                soundTrack = false;
            }
            modifiedLocation = false;
        }

        public void ModifyLocation()
        {
            modifiedLocation = true;
            targetLocation.temporarySprites.Clear();
            foreach (NPC character in targetLocation.characters)
            {
                if (character.isVillager() && character.Name != "Gunther")
                    character.IsInvisible = true;
            }
            Layer layer1 = targetLocation.map.GetLayer("Back");
            Layer layer2 = targetLocation.map.GetLayer("Buildings");
            Layer layer3 = targetLocation.map.GetLayer("Front");
            Layer layer4 = targetLocation.map.GetLayer("AlwaysFront");
            TileSheet tileSheet = targetLocation.map.TileSheets[1];
            Vector2 vector2_1 = new(targetVector.X - 8, targetVector.Y - 5);//Vector2.op_Subtraction(targetVector, new Vector2(8f, 5f));
            for (int index1 = 0; index1 < 13; ++index1)
            {
                for (int index2 = 0; index2 < 13; ++index2)
                {
                    Vector2 vector2_2 = new(vector2_1.X + index2, vector2_1.Y + index1);//Vector2.op_Addition(vector2_1, new Vector2(index2, index1));
                    if (layer2.Tiles[(int)vector2_2.X, (int)vector2_2.Y] != null)
                        layer2.Tiles[(int)vector2_2.X, (int)vector2_2.Y] = null;
                    if (layer3.Tiles[(int)vector2_2.X, (int)vector2_2.Y] != null)
                        layer3.Tiles[(int)vector2_2.X, (int)vector2_2.Y] = null;
                    if (layer4.Tiles[(int)vector2_2.X, (int)vector2_2.Y] != null)
                        layer4.Tiles[(int)vector2_2.X, (int)vector2_2.Y] = null;
                    layer1.Tiles[(int)vector2_2.X, (int)vector2_2.Y] = randomIndex.Next(4) == 0 ? (randomIndex.Next(5) != 0 ? (randomIndex.Next(5) != 0 ? new StaticTile(layer1, tileSheet, 0, 607) : (Tile)new StaticTile(layer1, tileSheet, 0, 606)) : new StaticTile(layer1, tileSheet, 0, 639)) : new StaticTile(layer1, tileSheet, 0, 638);
                }
            }
        }
    }
}
