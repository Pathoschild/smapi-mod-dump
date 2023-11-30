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
using StardewDruid.Map;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Event.Character
{
    public class Effigy : TriggerHandle{

        public Effigy(GameLocation location, Map.Quest quest)
            : base(location, quest)
        {
            voicePosition = quest.triggerVector * 64 - new Vector2(48,48);
        }

        public override bool SetMarker()
        {

            if (Mod.instance.characters.ContainsKey("Effigy"))
            {
                return false;

            }
            
            return base.SetMarker();

        }

        public override bool CheckMarker(Rite rite)
        {

            if (Vector2.Distance(rite.castVector, targetVector) <= 3f)
            {

                RemoveAnimations();

                Mod.instance.triggerList.Remove(questData.name);

                Mod.instance.locationPoll["trigger"] = null;

                CastVoice("Well done", duration: 2000);

                ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 500);
                
                Game1.player.currentLocation.playSound("yoba");

                // ----- rock fall

                if (!Mod.instance.rockCasts.ContainsKey(rite.castLocation.Name))
                {

                    Mod.instance.rockCasts[rite.castLocation.Name] = 10;

                };

                for (int l = 0; l < 10; l++)
                {

                    int i = l % 5;

                    List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(rite.castLocation, rite.caster.getTileLocation(), i+2); // 2, 3, 4, 5, 6

                    if (rite.randomIndex.Next(2) == 0) { castSelection.Reverse(); } // clockwise iteration can slightly favour one side

                    int castSelect = castSelection.Count; // 12, 16, 24, 28, 32

                    if (castSelect != 0)
                    {

                        List<int> segmentList = new() // 2, 2, 3, 4, 4
                        {
                            6, 8, 8, 7, 8,
                        };

                        int castSegment = segmentList[i];

                        List<int> cycleList = new()
                        {
                            2, 2, 3, 4, 4,
                        };

                        int castCycle = cycleList[i];

                        int castIndex;

                        Vector2 newVector;

                        for (int k = 0; k < castCycle; k++)
                        {
                            int castLower = castSegment * k;

                            if (castLower + 1 >= castSelect)
                            {

                                continue;

                            }

                            int delay = (rite.randomIndex.Next(3,20) * 100);

                            int castHigher = Math.Min(castLower + castSegment, castSelection.Count);

                            castIndex = rite.randomIndex.Next(castLower, castHigher);

                            newVector = castSelection[castIndex];

                            Cast.Earth.Rockfall rockFall = new(newVector, rite);

                            rockFall.castDelay = delay;

                            rockFall.castSound = rite.randomIndex.Next(2) == 0 ? "boulderBreak" : "boulderCrack";

                            rockFall.CastEffect();

                            for(int j = 0; j < 3; j++)
                            {

                                List<Vector2> newSelection = ModUtility.GetTilesWithinRadius(rite.castLocation, newVector, j); // 2, 3, 4, 5, 6

                                foreach(Vector2 newSelect in newSelection)
                                {

                                    ModUtility.AnimateCastRadius(rite.castLocation, newSelect, new(0.8f, 1f, 0.8f, 1f),delay + (i*200));

                                }

                            }


                        }

                    }

                }

                DelayedAction.functionAfterDelay(AnimateEffigy, 1000);

                DelayedAction.functionAfterDelay(SpawnEffigy, 2000);

                return true;

            }

            return false;

        }

        public override void EventInterval()
        {
            
            base.EventInterval();

            int actionCycle = activeCounter % 17;

            switch (actionCycle)
            {
                case 2:

                    CastVoice("Farmer", duration: 3000);

                    break;

                case 5:

                    CastVoice("You come at last", duration: 3000);
                    
                    break;

                case 8:

                    CastVoice("I'm in the ceiling", duration: 3000);
                    
                    break;

                case 11:

                    CastVoice("Stand here and perform the rite", duration: 3000);

                    Mod.instance.CastMessage($"{Mod.instance.CastControl()} with a tool in hand to perform a rite of the Earth", -1);

                    break;

                case 14:

                    CastVoice("As the first farmer did long ago", duration: 3000);

                    break;

            }

        }

        public void AnimateEffigy()
        {

            Vector2 EffigyPosition = CharacterData.CharacterPosition() - new Vector2(0, 704);

            TemporaryAnimatedSprite EffigyAnimation = new(0, 1000f, 1, 1, EffigyPosition, false, false)
            {

                sourceRect = new(0, 0, 16, 32),

                texture = CharacterData.CharacterTexture("Effigy"),

                scale = 4f,

                motion = new Vector2(0, 0.64f),

                timeBasedMotion = true,

            };

            targetLocation.temporarySprites.Add(EffigyAnimation);

        }

        public void SpawnEffigy()
        {

            CharacterData.CharacterLoad("Effigy", "FarmCave");

            Mod.instance.CharacterRegister("Effigy", "FarmCave");

            Mod.instance.characters["Effigy"].checkAction(Game1.player, targetLocation);

        }
    
    }

}
