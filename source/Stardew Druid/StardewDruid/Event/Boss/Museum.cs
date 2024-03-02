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
using StardewDruid.Event.Challenge;
using StardewDruid.Map;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewDruid.Event.Boss
{
    public class Museum : BossHandle
    {
        public bool modifiedLocation;
        public Monster.Boss.Dino bossMonster;
        public Vector2 returnPosition;
        public NPC Gunther;

        public Museum(Vector2 target, Rite rite, Quest quest)
          : base(target, rite, quest)
        {

            targetVector = target;

            voicePosition = targetVector * 64f - new Vector2(0.0f, 32f);

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 60.0;

            returnPosition = rite.caster.Position;

        }

        public override void EventTrigger()
        {
            cues = DialogueData.DialogueScene(questData.name);

            ModUtility.AnimateRadiusDecoration(targetLocation, targetVector, "Mists", 1f, 1f);

            ModUtility.AnimateBolt(targetLocation, targetVector);

            Mod.instance.RegisterEvent(this, "active");

            AddActor(voicePosition, true);

            Gunther = targetLocation.getCharacterFromName(nameof(Gunther));

            AddActor(Gunther.Position - new Vector2(0.0f, 32f));

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

                eventLinger = 3;

                return true;

            }

            if (eventLinger == 2)
            {

                if (expireEarly)
                {

                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[1], }, 991);

                    Vector2 vector2 = Gunther.getTileLocation() + new Vector2(0.0f, 1f);

                    if (!questData.name.Contains("Two"))
                    {
                        
                        Game1.createObjectDebris(74, (int)vector2.X, (int)vector2.Y, -1, 0, 1f, null);
                    
                    }

                    EventComplete();

                }
                else
                {
                    
                    DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[1], }, 992);

                    Mod.instance.CastMessage("Try again tomorrow");

                }

            }

            return base.EventExpire();

        }

        public override void EventInterval()
        {
            ++activeCounter;

            if (eventLinger != -1)
            {
                return;
            }

            if (activeCounter < 6)
            {

                DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[1], [1] = actors[0], }, activeCounter);

                if (activeCounter == 1)
                {
                    using (List<NPC>.Enumerator enumerator = targetLocation.characters.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            NPC current = enumerator.Current;
                            if (current.isVillager() && current.Name != "Gunther")
                                current.doEmote(8, true);
                        }
                    }

                }
                
                Vector2 vector2 = targetVector + new Vector2(0.0f, 1f) - new Vector2(randomIndex.Next(7), randomIndex.Next(3));
                
                ModUtility.AnimateRadiusDecoration(targetLocation, vector2, "Mists", 1f, 1f);
                
                ModUtility.AnimateBolt(targetLocation, vector2);

                return;

            }
            
            if (activeCounter == 6)
            {

                modifiedLocation = true;

                targetLocation.temporarySprites.Clear();

                foreach (NPC character in targetLocation.characters)
                {
                    //if (character.isVillager() && character.Name != "Gunther")
                    if (character is StardewValley.Monsters.Monster || character is Character.Character || character is Character.Dragon || character.Name == "Gunther")
                    {
                        continue;

                    }

                    character.IsInvisible = true;

                }

                Location.LocationData.MuseumEdit();

                EventQuery("LocationEdit");
                
                bossMonster = MonsterData.CreateMonster(14, targetVector + new Vector2(2f, 0.0f)) as Monster.Boss.Dino;
                
                if (questData.name.Contains("Two"))
                {
                    bossMonster.HardMode();
                }

                bossMonster.currentLocation = targetLocation;

                riteData.castLocation.characters.Add(bossMonster);

                bossMonster.update(Game1.currentGameTime, riteData.castLocation);

                SetTrack("heavy");

                return;
            
            }

            DialogueCue(DialogueData.DialogueNarrator(questData.name), new() { [0] = actors[1], [1] =bossMonster,}, activeCounter);

            switch (activeCounter)
            {
                case 10:
                    GuntherThrowRandomShit();
                    break;
                case 11:
                    GuntherApplyDamage();
                    break;
                case 20:
                    GuntherThrowRandomShit();
                    break;
                case 21:
                    GuntherApplyDamage();
                    break;
                case 30:
                    GuntherThrowRandomShit();
                    break;
                case 31:
                    GuntherApplyDamage();
                    break;
                case 40:
                    GuntherThrowRandomShit();
                    break;
                case 41:
                    GuntherApplyDamage();
                    break;
                case 50:
                    GuntherThrowRandomShit();
                    break;
                case 51:
                    GuntherApplyDamage();
                    break;
                case 57:
                    GuntherThrowRandomShit();
                    //expireEarly = true;
                    break;
                case 58:
                    GuntherApplyDamage();
                    break;
            }

            if (activeCounter > 8 && !ModUtility.MonsterVitals(bossMonster, targetLocation))
            {

                expireEarly = true;

            }

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
                127,
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

        public void GuntherApplyDamage()
        {

            ModUtility.DamageMonsters(targetLocation, new() { bossMonster, }, targetPlayer, Mod.instance.DamageLevel());

        }

        public void ResetLocation()
        {
            if (!modifiedLocation)
            {
                return;

            }

            Location.LocationData.MuseumReset();

            EventQuery("LocationReset");

            foreach (NPC character in targetLocation.characters)
            {
                if (character.IsInvisible)
                {

                    character.IsInvisible = false;
                }

            }
            
            if (Game1.eventUp || Game1.fadeToBlack || Game1.currentMinigame != null || Game1.isWarping || Game1.killScreen || !(Game1.player.currentLocation is LibraryMuseum))
            {
                
                return;
            
            }

            Game1.fadeScreenToBlack();

            targetPlayer.Position = returnPosition;

            if (soundTrack)
            {
                Game1.stopMusicTrack(0);

                soundTrack = false;
            }

            modifiedLocation = false;

        }
    
    }

}
