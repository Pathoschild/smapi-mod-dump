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
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Security.Cryptography.X509Certificates;

namespace StardewDruid.Event.Scene
{
    public class Scythe : EventHandle
    {

        public Scythe(Vector2 target,  Quest quest)
          : base(target)
        {

            targetVector = Mod.instance.rite.castVector;

            questData = quest;

            eventId = quest.name;

        }

        public override void EventTrigger()
        {

            Mod.instance.CompleteQuest("swordFates");

            if (!Context.IsMainPlayer)
            {

                return;

            }

            if (!Mod.instance.characters.ContainsKey("Jester"))
            {

                return;

            }

            cues = DialogueData.DialogueScene("swordFates");

            narrators = DialogueData.DialogueNarrator("swordFates");

            voices = new() { [0] = Mod.instance.characters["Jester"], };

            sceneCounter = 3;

            Mod.instance.RegisterEvent(this, "swordFates");

            expireTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + 20.0;

        }

        public override void EventRemove()
        {

            Jester character = Mod.instance.characters["Jester"] as Jester;

            ModUtility.AnimateQuickWarp(character.currentLocation, character.Position - new Vector2(0.0f, 32f), true);

            character.SwitchPreviousMode();

            base.EventRemove();

        }

        public override void EventInterval()
        {

            activeCounter++;

            if (activeCounter == 1)
            {

                Jester character = Mod.instance.characters["Jester"] as Jester;

                character.SwitchSceneMode();

                if (character.currentLocation.Name != Mod.instance.rite.castLocation.Name)
                {
                    character.currentLocation.characters.Remove(character);
                    character.currentLocation = Mod.instance.rite.castLocation;
                    character.currentLocation.characters.Add(character);
                    character.Halt();
                    character.TargetIdle(30);
                }

                character.Position = new((questData.triggerVector.X * 64f)- 128f, (questData.triggerVector.Y * 64f) + 128f);

                ModUtility.AnimateQuickWarp(Mod.instance.rite.castLocation, character.Position - new Vector2(0.0f, 32f));

                character.eventVectors.Clear();

                character.eventVectors = new()
                {

                    [3] = (character.Position + new Vector2(0.0f, -128f)),

                };

                character.eventIndex = 3;

                character.eventName = "swordFates";

                return;

            }

            if(activeCounter == 2)
            {

                DialogueCue(1);

                return;

            }

            if(activeCounter == 4)
            {
                
                sceneCounter = 5;

                Mod.instance.dialogue["Jester"].AddSpecial("Jester", "Thanatoshi");

                return;

            }

        }

    }

}