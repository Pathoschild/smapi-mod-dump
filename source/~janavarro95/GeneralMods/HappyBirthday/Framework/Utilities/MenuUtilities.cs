/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.HappyBirthday.Framework.Utilities
{
    public static class MenuUtilities
    {
        /// <summary>Checks if the current billboard is the daily quest screen or not.</summary>
        public static bool IsDailyQuestBoard;

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public static void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            switch (e.NewMenu)
            {
                case null:
                    OnActiveMenuChangedToNull();

                    return;

                case Billboard billboard:
                    {
                        OnMenuChangedToBillboard(billboard);
                        break;
                    }
                case DialogueBox dBox:
                    {
                        OnMenuChangedToDialogueBox();
                        break;
                    }
            }

        }

        /// <summary>
        /// Occurs when 
        /// </summary>
        public static void OnActiveMenuChangedToNull()
        {
            IsDailyQuestBoard = false;
            //Validate the gift and give it to the player.
            if (NPCUtilities.LastSpeaker != null)
            {
                if (HappyBirthdayModCore.Instance.giftManager.BirthdayGiftToReceive != null && HappyBirthdayModCore.Instance.birthdayManager.hasGivenBirthdayGift(NPCUtilities.LastSpeaker.Name) == false)
                {
                    while (HappyBirthdayModCore.Instance.giftManager.BirthdayGiftToReceive.Name == "Error Item" || HappyBirthdayModCore.Instance.giftManager.BirthdayGiftToReceive.Name == "Rock" || HappyBirthdayModCore.Instance.giftManager.BirthdayGiftToReceive.Name == "???")
                        HappyBirthdayModCore.Instance.giftManager.setNextBirthdayGift(NPCUtilities.LastSpeaker.Name);
                    Game1.player.addItemByMenuIfNecessaryElseHoldUp(HappyBirthdayModCore.Instance.giftManager.BirthdayGiftToReceive);
                    HappyBirthdayModCore.Instance.giftManager.BirthdayGiftToReceive = null;
                    HappyBirthdayModCore.Instance.birthdayManager.villagerQueue[NPCUtilities.LastSpeaker.Name].hasGivenBirthdayGift = true;
                    NPCUtilities.LastSpeaker = null;
                }
            }
        }

        public static void OnMenuChangedToDialogueBox()
        {
            if (Game1.eventUp) return;
            //Hijack the dialogue box and ensure that birthday dialogue gets spoken.
            if (Game1.currentSpeaker != null)
            {
                NPCUtilities.LastSpeaker = Game1.currentSpeaker;
                if (Game1.activeClickableMenu != null && HappyBirthdayModCore.Instance.birthdayManager.isBirthday())
                {
                    if (NPCUtilities.ShouldWishPlayerHappyBirthday(Game1.currentSpeaker.Name) == false) return;
                    if (Game1.activeClickableMenu is DialogueBox)
                    {
                        Game1.currentSpeaker.resetCurrentDialogue();
                        Game1.currentSpeaker.resetSeasonalDialogue();
                        HappyBirthdayModCore.Instance.Helper.Reflection.GetMethod(Game1.currentSpeaker, "loadCurrentDialogue", true).Invoke();
                        Game1.npcDialogues[Game1.currentSpeaker.Name] = Game1.currentSpeaker.CurrentDialogue;
                        if (HappyBirthdayModCore.Instance.birthdayManager.isBirthday() && HappyBirthdayModCore.Instance.birthdayManager.hasGivenBirthdayGift(Game1.currentSpeaker.Name) == false)
                        {
                            try
                            {
                                HappyBirthdayModCore.Instance.giftManager.setNextBirthdayGift(Game1.currentSpeaker.Name);
                                HappyBirthdayModCore.Instance.Monitor.Log("Setting next birthday gift.");
                            }
                            catch (Exception ex)
                            {
                                HappyBirthdayModCore.Instance.Monitor.Log(ex.ToString(), LogLevel.Error);
                            }
                        }

                        Game1.activeClickableMenu = new DialogueBox(new Dialogue(HappyBirthdayModCore.Instance.birthdayMessages.getBirthdayMessage(Game1.currentSpeaker.Name), Game1.currentSpeaker));
                        HappyBirthdayModCore.Instance.birthdayManager.villagerQueue[Game1.currentSpeaker.Name].hasGivenBirthdayWish = true;

                        // Set birthday gift for the player to recieve from the npc they are currently talking with.

                    }

                }
            }
        }

        public static void OnMenuChangedToBillboard(Billboard billboard)
        {
            IsDailyQuestBoard = HappyBirthdayModCore.Instance.Helper.Reflection.GetField<bool>((Game1.activeClickableMenu as Billboard), "dailyQuestBoard", true).GetValue();
            if (IsDailyQuestBoard)
                return;

            Texture2D text = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            Color[] col = new Color[1];
            col[0] = new Color(0, 0, 0, 1);
            text.SetData<Color>(col);
            //players birthday position rect=new ....

            if (!string.IsNullOrEmpty(HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData.BirthdaySeason))
            {
                if (HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData.BirthdaySeason.ToLower() == Game1.currentSeason.ToLower())
                {
                    int index = HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData.BirthdayDay;

                    string bdayDisplay = Game1.content.LoadString("Strings\\UI:Billboard_Birthday");
                    Rectangle birthdayRect = new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + 152 + (index - 1) % 7 * 32 * 4, Game1.activeClickableMenu.yPositionOnScreen + 200 + (index - 1) / 7 * 32 * 4, 124, 124);
                    billboard.calendarDays.Add(new ClickableTextureComponent("", birthdayRect, "", string.Format(bdayDisplay, Game1.player.Name), text, new Rectangle(0, 0, 124, 124), 1f, false));
                    //billboard.calendarDays.Add(new ClickableTextureComponent("", birthdayRect, "", $"{Game1.player.Name}'s Birthday", text, new Rectangle(0, 0, 124, 124), 1f, false));
                }
            }

            foreach (var pair in HappyBirthdayModCore.Instance.birthdayManager.othersBirthdays)
            {
                if (pair.Value.BirthdaySeason != Game1.currentSeason.ToLower()) continue;
                int index = pair.Value.BirthdayDay;

                string bdayDisplay = Game1.content.LoadString("Strings\\UI:Billboard_Birthday");
                Rectangle otherBirthdayRect = new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + 152 + (index - 1) % 7 * 32 * 4, Game1.activeClickableMenu.yPositionOnScreen + 200 + (index - 1) / 7 * 32 * 4, 124, 124);
                billboard.calendarDays.Add(new ClickableTextureComponent("", otherBirthdayRect, "", string.Format(bdayDisplay, Game1.getFarmer(pair.Key).Name), text, new Rectangle(0, 0, 124, 124), 1f, false));
            }
        }
    }
}
