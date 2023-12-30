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
using StardewDruid.Character;
using StardewDruid.Map;
using StardewValley;
using System.Collections.Generic;

namespace StardewDruid.Event.Scene
{
    public class Scythe : EventHandle
    {
        private Quest questData;

        public Scythe(Vector2 target, Rite rite, Quest quest)
          : base(target, rite)
        {
            questData = quest;
        }

        public override void EventTrigger()
        {
            Mod.instance.CompleteQuest("swordFates");
            //Jester character = Mod.instance.characters["Jester"] as Jester;
            Mod.instance.RegisterEvent(this, "scene");
            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 20.0;
        }

        public override void EventRemove()
        {
            Jester character = Mod.instance.characters["Jester"] as Jester;
            ModUtility.AnimateQuickWarp(character.currentLocation, character.Position - new Vector2(0.0f, 32f), "Solar");
            character.SwitchDefaultMode();
            character.WarpToDefault();
            base.EventRemove();
            //Mod.instance.ReassignQuest(questData.name);
        }

        public override bool EventActive()
        {
            return targetPlayer.currentLocation == targetLocation && expireTime >= Game1.currentGameTime.TotalGameTime.TotalSeconds;
        }

        public override void EventInterval()
        {
            
            activeCounter++;
            
            Jester character = Mod.instance.characters["Jester"] as Jester;

            if (activeCounter == 1)
            {
                
                if (character.currentLocation.Name != riteData.castLocation.Name)
                {
                    character.Halt();
                    character.currentLocation.characters.Remove(character);
                    character.currentLocation = riteData.castLocation;
                    character.currentLocation.characters.Add(character);
                }

                character.Position = new((questData.triggerVector.X * 64f)- 128f, (questData.triggerVector.Y * 64f) + 128f);

                ModUtility.AnimateQuickWarp(riteData.castLocation, character.Position - new Vector2(0.0f, 32f), "Solar");

                character.SwitchEventMode();

                character.priorities = new() { "event", "idle", };

                character.eventVectors.Clear();

                character.eventVectors.Add(character.Position + new Vector2(0.0f, -128f));

                character.timers["busy"] = 1000;

                Mod.instance.dialogue["Jester"].specialDialogue.Add("Thanatoshi", new List<string>()
                {
                  "I hope he found peace...",
                  "Grim. Dark. I Love this place.",
                  "Do you know this figure?",
                  "I have a strange foreboding about "
                });

            }

            if (activeCounter == 2)
            {
                
                character.showTextAboveHead("so Thanatoshi was here.", -1, 2, 3000, 0);
            
            }
            
        }
    }
}