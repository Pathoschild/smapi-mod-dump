using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.DialogueCore
{
    public class ReponseLogic
    {
        public static void answerDialogueResponses()
        {
            if (Game1.player.currentLocation.lastQuestionKey != null)
            {
                ModCore.CoreMonitor.Log(Game1.player.currentLocation.lastQuestionKey);
                while (Game1.player.currentLocation.lastQuestionKey == "Sleep")
                {
                    //WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.ESCAPE);
                    Game1.player.currentLocation.lastQuestionKey = null;
                    ModCore.CoreMonitor.Log("GO TO SLEEP");
                    answerDialogueSleep();
                    //Game1.player.currentLocation.answerDialogue(new Response(Game1.player.currentLocation.lastQuestionKey, "Yes"));
                }
            }
        }

        public static void answerDialogueSleep()
        {
            GameLocation location = Game1.player.currentLocation;
            if ((double)location.LightLevel == 0.0 && Game1.timeOfDay < 2000)
            {
                location.LightLevel = 0.6f;
                
                Game1.playSound("openBox");
                Game1.NewDay(600f);
            }
            else if ((double)location.LightLevel > 0.0 && Game1.timeOfDay >= 2000)
            {
                location.LightLevel = 0.0f;
                Game1.playSound("openBox");
                Game1.NewDay(600f);
            }
            else
                Game1.NewDay(0.0f);
            Game1.player.mostRecentBed = Game1.player.position;
            Game1.player.doEmote(24);
            Game1.player.freezePause = 2000;

           
        }


    }
}
