using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using CasinoHelper.Injectors;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Minigames;
using StardewValley.Menus;

namespace CasinoHelper.Injectors
{
    [HarmonyPatch(typeof(CalicoJack), "receiveLeftClick")]
    public class Injectors
    {
        // private IReflectedField<int> something;
        //public static IReflectedField<int> curBet;
        public static void receiveLeftClick(int x, int y, bool playSount = true)
        {
            if (Game1.currentMinigame != null && "CalicoJack".Equals(Game1.currentMinigame.GetType().Name))
            {
                //Game1.currentMinigame = (IMinigame)new CalicoJack(curBet, highStakes);
                Type minigameType = Game1.currentMinigame.GetType();
                //Grab current bet;
                List<int[]> playerCards = new List<int[]>();//helper.Reflection.GetField<List<int[]>>(Game1.currentMinigame, "playerCards").GetValue();
                int dealerTurnTimer = -1;
                //List<int[]> dealerCards = //helper.Reflection.GetField<List<int[]>>(Game1.currentMinigame, "dealerCards").GetValue();
                int curBet = CasinoHelper.Modhelper.Reflection.GetField<int>(Game1.currentMinigame, "currentBet").GetValue();
                Random r = CasinoHelper.Modhelper.Reflection.GetField<Random>(Game1.currentMinigame, "r").GetValue();
                int startTimer = CasinoHelper.Modhelper.Reflection.GetField<int>(Game1.currentMinigame, "startTimer").GetValue();
                int bustTimer = CasinoHelper.Modhelper.Reflection.GetField<int>(Game1.currentMinigame, "bustTimer").GetValue();
                ClickableComponent hit = CasinoHelper.Modhelper.Reflection.GetField<ClickableComponent>(Game1.currentMinigame, "hit").GetValue();
                ClickableComponent stand = CasinoHelper.Modhelper.Reflection.GetField<ClickableComponent>(Game1.currentMinigame, "stand").GetValue();
                ClickableComponent doubleOrNothing = CasinoHelper.Modhelper.Reflection.GetField<ClickableComponent>(Game1.currentMinigame, "doubleOrNothing").GetValue();
                ClickableComponent playAgain = CasinoHelper.Modhelper.Reflection.GetField<ClickableComponent>(Game1.currentMinigame, "playAgain").GetValue();
                ClickableComponent quit = CasinoHelper.Modhelper.Reflection.GetField<ClickableComponent>(Game1.currentMinigame, "quit").GetValue();
                ClickableComponent currentlySnappedComponent = CasinoHelper.Modhelper.Reflection.GetField<ClickableComponent>(Game1.currentMinigame, "currentlySnappedComponent").GetValue();
                bool showingResultsScreen = CasinoHelper.Modhelper.Reflection.GetField<bool>(Game1.currentMinigame, "showingResultsScreen").GetValue();
                bool playerWon = CasinoHelper.Modhelper.Reflection.GetField<bool>(Game1.currentMinigame, "playerWon").GetValue();
                bool highStakes = CasinoHelper.Modhelper.Reflection.GetField<bool>(Game1.currentMinigame, "highStakes").GetValue();
                string coinBuffer = CasinoHelper.Modhelper.Reflection.GetField<string>(Game1.currentMinigame, "coinBuffer").GetValue();

                //if (startTimer <= 0 && bustTimer <= 0)
                   // showingResultsScreen = !showingResultsScreen;

                if (!showingResultsScreen && bustTimer <= 0)
                {
                    if (hit.bounds.Contains(x, y))
                    {
                        playerCards.Add(new int[2] { r.Next(1,10), 400 });
                        Game1.playSound("shwip");

                        int num = 0;
                        foreach (int[] playerCard in playerCards)
                            num += playerCard[0];
                        if (num == 21)
                            bustTimer = 1000;
                        else if (bustTimer > 21)
                            bustTimer = 1000;
                        Game1.playSound("coin");
                        CasinoHelper.Modhelper.Reflection.GetField<int>(Game1.currentMinigame, "bustTimer").SetValue(bustTimer);
                    }
                    if (stand.bounds.Contains(x, y))
                        return;
                    dealerTurnTimer = 1000;
                    CasinoHelper.Modhelper.Reflection.GetField<int>(Game1.currentMinigame, "dealerTurnTimer").SetValue(dealerTurnTimer);
                }
                else
                {
                    if (!showingResultsScreen)
                        return;
                    if(playerWon && doubleOrNothing.containsPoint(x, y))
                    {
                        Game1.currentMinigame = (IMinigame)new CalicoJack(curBet * 2, highStakes);
                        Game1.playSound("bigSelect");
                    }
                    if(Game1.player.clubCoins >= curBet && playAgain.containsPoint(x, y))
                    {
                        Game1.currentMinigame = (IMinigame)new CalicoJack(-1, highStakes);
                        Game1.playSound("smallSelect");
                    }
                    if (!quit.containsPoint(x, y))
                        return;
                    Game1.currentMinigame = (IMinigame)null;
                    Game1.playSound("bigDeSelect");
                }
            }
        }

        internal static bool Prefix(int x, int y, bool playSound = true)
        {
            receiveLeftClick(x, y, playSound);
            return false;
        }
    }
}
