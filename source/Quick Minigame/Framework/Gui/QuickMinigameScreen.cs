/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/QuickMinigame
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using StardewValley;
using StardewValley.Minigames;

namespace QuickMinigame.Framework.Gui
{
    public class QuickMinigameScreen : ScreenGui
    {
        public QuickMinigameScreen()
        {
            var gameLocation = Game1.game1.instanceGameLocation;
            var buttonTitle = GetTranslation("quickMinigame.button");
            AddElement(new Button($"{buttonTitle} {GetTranslation("quickMinigame.button.darts")}",
                $"{buttonTitle} {GetTranslation("quickMinigame.button.darts")}")
            {
                OnLeftClicked = () => { Game1.currentMinigame = new Darts(); }
            });
            
            AddElement(new Button(
                $"{buttonTitle} {GetTranslation("quickMinigame.button.slots")}",
                $"{buttonTitle} {GetTranslation("quickMinigame.button.slots")}")
            {
                OnLeftClicked = () => { Game1.currentMinigame = new Slots(); }
            });
            AddElement(new Button(
                $"{buttonTitle} {GetTranslation("quickMinigame.button.prairieKing")}",
                $"{buttonTitle} {GetTranslation("quickMinigame.button.prairieKing")}")
            {
                OnLeftClicked = () =>
                {
                    gameLocation.showPrairieKingMenu();
                }
            });
            AddElement(new Button(
                $"{buttonTitle} {GetTranslation("quickMinigame.button.calicoJack")}",
                $"{buttonTitle} {GetTranslation("quickMinigame.button.calicoJack")}")
            {
                OnLeftClicked = () => { Game1.currentMinigame = new CalicoJack(); }
            });
            AddElement(new Button(
                $"{buttonTitle} {GetTranslation("quickMinigame.button.craneGame")}",
                $"{buttonTitle} {GetTranslation("quickMinigame.button.craneGame")}")
            {
                OnLeftClicked = () =>
                {
                    gameLocation.createQuestionDialogue(
                        Game1.content.LoadString("Strings\\StringsFromMaps:MovieTheater_CranePlay", 500),
                        gameLocation.createYesNoResponses(), TryToStartCraneGame);
                }
            });
            AddElement(new Button(
                $"{buttonTitle} {GetTranslation("quickMinigame.button.minecart")}",
                $"{buttonTitle} {GetTranslation("quickMinigame.button.minecart")}")
            {
                OnLeftClicked = () =>
                {
                    var answerChoices = new[]
                    {
                        new Response("Progress",
                            Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_ProgressMode")),
                        new Response("Endless",
                            Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_EndlessMode")),
                        new Response("Exit", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Exit"))
                    };
                    gameLocation.createQuestionDialogue(
                        Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Menu"), answerChoices,
                        "MinecartGame");
                }
            });
        }

        private void TryToStartCraneGame(Farmer who, string whichAnswer)
        {
            if (whichAnswer.ToLower() != "yes")
                return;
            if (Game1.player.Money >= 500)
            {
                Game1.player.Money -= 500;
                Game1.changeMusicTrack("none", music_context: Game1.MusicContext.MiniGame);
                Game1.globalFadeToBlack(() => Game1.currentMinigame = (IMinigame) new CraneGame(), 0.008f);
            }
            else
                Game1.drawObjectDialogue(
                    Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325"));
        }

        private string GetTranslation(string key)
        {
            return ModEntry.GetInstance().Helper.Translation.Get(key);
        }
    }
}