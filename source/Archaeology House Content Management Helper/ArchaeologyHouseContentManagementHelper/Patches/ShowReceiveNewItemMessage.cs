//using Netcode;
//using StardewValley;
//using StardewValley.BellsAndWhistles;
//using StardewValley.Locations;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace StardewMods.ArchaeologyHouseContentManagementHelper.Patches
//{
//    public class ShowReceiveNewItemMessage : Patch
//    {
//        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(Game1.player.GetType(), "showReceiveNewItemMessage");

//        public static bool Prefix()
//        {
//            return false;
//        }

//        public static void Postfix(Farmer who)
//        {
//            string dialogue = who.mostRecentlyGrabbedItem.checkForSpecialItemHoldUpMeessage();
//            if (dialogue != null)
//            {
//                Game1.drawObjectDialogue(dialogue);
//                if (who.mostRecentlyGrabbedItem.ParentSheetIndex == 102 && Game1.stats.NotesFound == Game1.stats.NotesFound)
//                {
//                    string nDialog = dialogue + "#$b Congratulations! You have found all lost books!";
//                    Game1.drawObjectDialogue(nDialog);
//                }
//                else
//                {
//                    Game1.drawObjectDialogue(dialogue);
//                }
                
//            }
//            else if (who.mostRecentlyGrabbedItem.ParentSheetIndex == 472 && who.mostRecentlyGrabbedItem.Stack == 15)
//            {
//                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1918"));
//            }
//            else
//            {
//                string dialog = who.mostRecentlyGrabbedItem.Stack > 1
//                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1922", (object)who.mostRecentlyGrabbedItem.Stack, (object)who.mostRecentlyGrabbedItem.DisplayName)
//                    : Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.1919", (object)who.mostRecentlyGrabbedItem.DisplayName, (object)Lexicon.getProperArticleForWord(who.mostRecentlyGrabbedItem.DisplayName));

//                Game1.drawObjectDialogue(dialog);

//                uint i = Game1.stats.NotesFound;
//            }

//            who.completelyStopAnimatingOrDoingAction();
//        }
//    }
//}
