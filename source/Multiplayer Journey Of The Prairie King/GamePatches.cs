/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/scayze/multiprairie
**
*************************************************/

using MultiPlayerPrairie;
using MultiplayerPrairieKing.Utility;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerPrairieKing
{
    internal class GamePatches
    {
        public class GameLocationPatches
        {
            //Opens up Dialogue options when the player interacts with the arcade machine
            public static bool ShowPrairieKingMenu_Prefix()
            {
                ModMultiPlayerPrairieKing.instance.playerID.Value = Game1.player.UniqueMultiplayerID;
                //These Three options are taken from the game code, and would be available anyway
                string question = Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Cowboy_Menu");

                //Response[] prairieKingOptions = new Response[4];
                List<Response> prairieKingOptions = new();

                prairieKingOptions.Add(new Response("Continue", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Cowboy_Continue")));
                prairieKingOptions.Add(new Response("NewGame", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Cowboy_NewGame")));
                
                //Display additional Host/Join optidrawObjectDialogueon depending on if theres a lobby available
                if (ModMultiPlayerPrairieKing.instance.isHostAvailable)
                    prairieKingOptions.Add(new Response("JoinMultiplayer", "Join Co-op Journey"));


                prairieKingOptions.Add(new Response("HostMultiplayer", "Host Co-op Journey"));
                if (ModMultiPlayerPrairieKing.instance.GetSaveState() != null)
                {
                    prairieKingOptions.Add(new Response("HostMultiplayerContinue", "Host Co-op Journey (Continue)"));
                }

                prairieKingOptions.Add(new Response("Exit", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Exit")));


                //Create the dialogue
                Game1.currentLocation.createQuestionDialogue(question, prairieKingOptions.ToArray(), new GameLocation.afterQuestionBehavior(ArcadeDialogueSet));

                //Always Skip original code
                return false;
            }

            public static void CheckForActionOnPrairieKingArcadeSystem_Postfix(StardewValley.Object __instance, Farmer who)
            {
                ModMultiPlayerPrairieKing.instance.lastInteractedArcadeMachine = __instance.TileLocation.GetHashCode() + __instance.Location.GetHashCode();
            }

            // Callback for choosing from the dialogue options when interacting with the arcade machine
            static public void ArcadeDialogueSet(Farmer who, string dialogue_id)
            {
                switch (dialogue_id)
                {
                    case "NewGame":
                        Game1.player.jotpkProgress.Value = null;
                        Game1.currentMinigame = new StardewValley.Minigames.AbigailGame();
                        break;
                    case "Continue":
                        Game1.currentMinigame = new StardewValley.Minigames.AbigailGame();
                        break;
                    case "JoinMultiplayer":
                        ModMultiPlayerPrairieKing.instance.isHost.Value = false;

                        //NET Join Lobby
                        PK_JoinLobby mJoinLobby = new()
                        {
                            playerId = ModMultiPlayerPrairieKing.instance.playerID.Value
                        };
                        ModMultiPlayerPrairieKing.instance.SyncMessage(mJoinLobby, SYNC_SCOPE.GLOBAL);

                        //Start Game
                        Game1.player.jotpkProgress.Value = null;
                        Game1.currentMinigame = new GameMultiplayerPrairieKing(ModMultiPlayerPrairieKing.instance, ModMultiPlayerPrairieKing.instance.isHost.Value);
                        break;
                    case "HostMultiplayer":
                        ModMultiPlayerPrairieKing.instance.UpdateSaveState(null);
                        HostGame();
                        break;
                    case "HostMultiplayerContinue":
                        HostGame();
                        break;
                }
            }

            static public void HostGame()
            {
                //When host is available
                ModMultiPlayerPrairieKing.instance.isHost.Value = true;
                ModMultiPlayerPrairieKing.instance.isHostAvailable = true;

                ModMultiPlayerPrairieKing.instance.playerList.Value.Clear();
                ModMultiPlayerPrairieKing.instance.playerList.Value.Add(ModMultiPlayerPrairieKing.instance.playerID.Value);

                //NET Start Hosting
                PK_StartHosting mStartHostingContinue = new()
                {
                    arcadeMachine = ModMultiPlayerPrairieKing.instance.lastInteractedArcadeMachine
                };
                ModMultiPlayerPrairieKing.instance.SyncMessage(mStartHostingContinue, SYNC_SCOPE.GLOBAL);

                Game1.currentMinigame = new GameMultiplayerPrairieKing(ModMultiPlayerPrairieKing.instance, ModMultiPlayerPrairieKing.instance.isHost.Value);
            }
        }
    }
}
