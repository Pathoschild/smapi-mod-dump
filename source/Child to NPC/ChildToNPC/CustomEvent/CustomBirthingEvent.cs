using System;
using StardewValley;
using StardewValley.Events;
using StardewValley.Characters;
using Netcode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChildToNPC.CustomEvent
{
    /* This is almost exactly the same as from Family Planning.
     * The only change was removing the spouse.
     */
    class CustomBirthingEvent : FarmEvent, INetObject<NetFields>
    {
        private int timer;
        private string message;
        private string babyName;
        private bool isMale;
        private bool getBabyName;
        private bool naming;
        public NetFields NetFields { get; } = new NetFields();

        public bool setUp()
        {
            Random random = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
            Game1.player.CanMove = false;

            isMale = true;
            message = "Your child has been born.";

            return false;
        }

        public bool tickUpdate(GameTime time)
        {
            Game1.player.CanMove = false;
            timer += time.ElapsedGameTime.Milliseconds;
            Game1.fadeToBlackAlpha = 1f;
            if (timer > 1500 && !getBabyName)
            {
                if (message != null && (!Game1.dialogueUp && Game1.activeClickableMenu == null))
                {
                    Game1.drawObjectDialogue(message);
                    Game1.afterDialogues = new Game1.afterFadeFunction(afterMessage);
                }
            }
            else if (getBabyName)
            {
                if (!naming)
                {
                    //I replaced the old NamingMenu with my CustomNamingMenu (to allow for gender control)
                    //This title dialogue isn't so easily edited to allow for all languages, so CustomNamingMenu fixes it.
                    Game1.activeClickableMenu = new CustomNamingMenu(new CustomNamingMenu.doneNamingBehavior(returnBabyName), Game1.content.LoadString("Strings\\Events:BabyNamingTitle_Male"), Game1.content.LoadString("Strings\\Events:BabyNamingTitle_Female"), "");
                    naming = true;
                }
                if (babyName != null && babyName != "" && babyName.Length > 0)
                {
                    double num = Game1.player.hasDarkSkin() ? 0.5 : 0.0;
                    bool isDarkSkinned = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed).NextDouble() < num;

                    foreach (Character allCharacter in Utility.getAllCharacters())
                    {
                        if (allCharacter.Name.Equals(babyName))
                        {
                            babyName += " ";
                            break;
                        }
                    }
                    /*
                     * Generates the new child.
                     */
                    Child child = new Child(babyName, isMale, isDarkSkinned, Game1.player);
                    child.Age = 0;
                    child.Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f);
                    Utility.getHomeOfFarmer(Game1.player).characters.Add(child);
                    Game1.playSound("smallSelect");

                    if (Game1.keyboardDispatcher != null)
                        Game1.keyboardDispatcher.Subscriber = null;
                    Game1.player.Position = Utility.PointToVector2(Utility.getHomeOfFarmer(Game1.player).getBedSpot()) * 64f;
                    Game1.globalFadeToClear(null, 0.02f);
                    return true;
                }
            }
            return false;
        }

        public void returnBabyName(string name, string gender)
        {
            if (gender.Equals("Male"))
                isMale = true;
            else if (gender.Equals("Female"))
                isMale = false;
            babyName = name;
            Game1.exitActiveMenu();
        }

        public void afterMessage()
        {
            this.getBabyName = true;
        }

        public void draw(SpriteBatch b)
        {
        }

        public void makeChangesToLocation()
        {
        }

        public void drawAboveEverything(SpriteBatch b)
        {
        }
    }
}
