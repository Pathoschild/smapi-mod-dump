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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System;

namespace StardewDruid.Event.Other
{
    public class Effigy : TriggerHandle
    {

        public Effigy(GameLocation location, Map.Quest quest)
            : base(location, quest)
        {

        }

        public override bool SetMarker()
        {

            if (Mod.instance.characters.ContainsKey("Effigy"))
            {
                return false;

            }

            triggerThreshold = 192;

            AddActor(questData.triggerVector * 64 - new Vector2(48, 48));

            return base.SetMarker();

        }

        public override void TriggerQuest()
        {

            CastVoice("Well done", duration: 2000);

            ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 500);

            Game1.player.currentLocation.playSound("yoba");

            // ----- rock fall
            ModUtility.AnimateDecoration(targetLocation, targetVector * 64, "weald");

            ModUtility.AnimateRockfalls(targetLocation, Game1.player.Tile);

            DelayedAction.functionAfterDelay(RockfallSounds, 575);

            DelayedAction.functionAfterDelay(RockfallSounds, 675);

            DelayedAction.functionAfterDelay(RockfallSounds, 775);

            DelayedAction.functionAfterDelay(AnimateEffigy, 1000);

            DelayedAction.functionAfterDelay(SpawnEffigy, 2000);

        }

        public void RockfallSounds()
        {
            targetLocation.playSound(new Random().Next(2) == 0 ? "boulderBreak" : "boulderCrack", targetVector*64, 800);
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

                    Mod.instance.CastMessage($"{Mod.instance.CastControl()} with a tool in hand to perform a rite of the Weald", -1);

                    break;

                case 14:

                    CastVoice("As the first farmer did long ago", duration: 3000);

                    break;

            }

        }

        public void AnimateEffigy()
        {

            Vector2 EffigyPosition = WarpData.WarpStart() - new Vector2(0, 704);

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

            if (!Context.IsMainPlayer)
            {

                if (!CharacterData.CharacterFind("Effigy"))
                { 
                    
                    Mod.instance.CastMessage("Effigy introduction requires multiplayer host");

                    return;

                }

            }
            else
            {
               
                CharacterData.CharacterLoad("Effigy", "FarmCave");

            }

            Mod.instance.CharacterRegister("Effigy", "FarmCave");

            Mod.instance.characters["Effigy"].checkAction(Game1.player, targetLocation);

        }

    }

}
