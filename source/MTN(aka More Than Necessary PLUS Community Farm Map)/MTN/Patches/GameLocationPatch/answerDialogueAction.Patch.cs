using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.GameLocationPatch
{
    //[HarmonyPatch(typeof(GameLocation))]
    //[HarmonyPatch("answerDialogueAction")]
    class answerDialogueActionPatch
    {
        public static void Postfix(GameLocation __instance, ref bool __result, string questionAndAnswer)
        {
            switch (questionAndAnswer)
            {
                case "carpenter_HouseDesign":
                    //Game1.activeClickableMenu = new CarpenterMenuHouseDesign();
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin", false), "Oh, I'm sorry but the architect who lived in town has been away for quite sometimes. I believe he went to see his friend who lives far away. Strangely enough, his friend calls himself \"Pickles\", kind of a strange guy.");
                    break;
            }
        }
    }
}
