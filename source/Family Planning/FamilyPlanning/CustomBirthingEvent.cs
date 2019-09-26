using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Events;
using Netcode;
using Microsoft.Xna.Framework;
using StardewValley.Characters;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;

namespace FamilyPlanning
{
    class CustomBirthingEvent : FarmEvent, INetObject<NetFields>
    {
        //private int behavior;
        private int timer;
        //private string soundName;
        private string message;
        private string babyName;
        //private bool playedSound;
        //private bool showedMessage;
        private bool isMale;
        private bool getBabyName;
        private bool naming;
        //private Vector2 targetLocation;
        //private TextBox babyNameBox;
        //private ClickableTextureComponent okButton;
        public NetFields NetFields { get; } = new NetFields();
        
        /*
         * The vast majority of this code is the same as the original BirthingEvent.
         * Instead of completely removing the unused variables in BirthingEvent, I've commented them out.
         */

        /*
         * CustomNamingMenu allows the player to choose the gender of the child,
         * so it's initialized to male for default purposes.
         * 
         * I'm removing the gender references in the message text because the message would always imply male.
         */
        public bool setUp()
        {
            Random random = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
            NPC characterFromName = Game1.getCharacterFromName(Game1.player.spouse, false);
            Game1.player.CanMove = false;
            
            isMale = true;

            string genderTerm = Lexicon.getGenderedChildTerm(isMale);
            message = !characterFromName.isGaySpouse() ? (characterFromName.Gender != 0 ? Game1.content.LoadString("Strings\\Events:BirthMessage_SpouseMother", (object)Lexicon.getGenderedChildTerm(this.isMale), (object)characterFromName.displayName) : Game1.content.LoadString("Strings\\Events:BirthMessage_PlayerMother", (object)Lexicon.getGenderedChildTerm(this.isMale))) : Game1.content.LoadString("Strings\\Events:BirthMessage_Adoption", (object)Lexicon.getGenderedChildTerm(this.isMale));
            //starting from i = 1 is a guess, but I'm pretty confident.
            for (int i = 1; i < message.Length - genderTerm.Length; i++)
            {
                if(message.Substring(i, genderTerm.Length).Equals(genderTerm))
                {
                    message = message.Substring(0, i - 1) + message.Substring(i + genderTerm.Length, message.Length - i - genderTerm.Length);
                    i = message.Length;
                }
            }

            return false;
        }

        public bool tickUpdate(GameTime time)
        {
            Game1.player.CanMove = false;
            timer += time.ElapsedGameTime.Milliseconds;
            Game1.fadeToBlackAlpha = 1f;
            if (timer > 1500 && /*!playedSound &&*/ !getBabyName)
            {
                /*
                if (soundName != null && !soundName.Equals(""))
                {
                    Game1.playSound(soundName);
                    playedSound = true;
                }
                */
                if (/*!playedSound &&*/ message != null && (!Game1.dialogueUp && Game1.activeClickableMenu == null))
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
                    double num = (Game1.player.spouse.Equals("Maru") ? 0.5 : 0.0) + (Game1.player.hasDarkSkin() ? 0.5 : 0.0);
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
                    Game1.player.getSpouse().daysAfterLastBirth = 5;
                    Game1.player.GetSpouseFriendship().NextBirthingDate = null;

                    /*
                     * Gives new lines to spouse based on number of children.
                     * As of now, it doesn't give any new lines after 2 children unless you're gay.
                     */
                    NPC spouse = Game1.player.getSpouse();
                    string s;
                    if (Game1.player.getSpouse().isGaySpouse())
                    {
                        if (Game1.player.getSpouse().Gender != 0)
                            s = ((IEnumerable<string>)Game1.content.LoadString("Data\\ExtraDialogue:NewChild_Adoption", (object)this.babyName).Split('/')).Last<string>();
                        else
                            s = ((IEnumerable<string>)Game1.content.LoadString("Data\\ExtraDialogue:NewChild_Adoption", (object)this.babyName).Split('/')).First<string>();
                        spouse.setNewDialogue(s, false, false);

                        if(Game1.player.getChildrenCount() == 2)
                            Game1.getSteamAchievement("Achievement_FullHouse");
                    }
                    else if (Game1.player.getChildrenCount() == 1)
                    {
                        if (Game1.player.getSpouse().Gender != 0)
                            s = ((IEnumerable<string>)Game1.content.LoadString("Data\\ExtraDialogue:NewChild_FirstChild", babyName).Split('/')).Last();
                        else
                            s = ((IEnumerable<string>)Game1.content.LoadString("Data\\ExtraDialogue:NewChild_FirstChild", babyName).Split('/')).First();
                        spouse.setNewDialogue(s, false, false);
                    }
                    else if(Game1.player.getChildrenCount() == 2)
                    {
                        if (Game1.random.NextDouble() >= 0.5)
                        {
                            if (Game1.player.getSpouse().Gender != 0)
                                s = ((IEnumerable<string>)Game1.content.LoadString("Data\\ExtraDialogue:NewChild_SecondChild2").Split('/')).Last<string>();
                            else
                                s = ((IEnumerable<string>)Game1.content.LoadString("Data\\ExtraDialogue:NewChild_SecondChild2").Split('/')).First<string>();
                        }
                        else if (Game1.player.getSpouse().Gender != 0)
                            s = ((IEnumerable<string>)Game1.content.LoadString("Data\\ExtraDialogue:NewChild_SecondChild1").Split('/')).Last<string>();
                        else
                            s = ((IEnumerable<string>)Game1.content.LoadString("Data\\ExtraDialogue:NewChild_SecondChild1").Split('/')).First<string>();
                        spouse.setNewDialogue(s, false, false);
                        Game1.getSteamAchievement("Achievement_FullHouse");
                    }
                    
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
