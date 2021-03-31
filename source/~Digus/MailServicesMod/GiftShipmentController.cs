/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Menus;

namespace MailServicesMod
{
    internal class GiftShipmentController
    {
        internal const string GiftResponseKeyNext = "Next";
        internal const string GiftResponseKeyPrevious = "Previous";
        internal const string GiftResponseKeyNone = "None";
        internal const string GiftDialogKey = "MailServiceMod_GiftShipment";

        internal static bool CreateResponsePage(int page)
        {
            List<Response> options = new List<Response>();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (Game1.player.tryGetFriendshipLevelForNPC(npc.Name) != null
                    && Game1.player.friendshipData[npc.Name].GiftsToday < 1
                    && (Game1.player.friendshipData[npc.Name].GiftsThisWeek < 2 || npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth))
                    && !Game1.player.friendshipData[npc.Name].IsDivorced()
                    && (Game1.player.friendshipData[npc.Name].Points < 2500 || DataLoader.ModConfig.EnableGiftToNpcWithMaxFriendship)
                    && !(Game1.player.spouse != null && Game1.player.spouse.Equals(Game1.player.Name))
                    && !(npc is Child))
                {
                    options.Add(new Response(npc.Name, npc.displayName + (npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth) ? " (" + Game1.content.LoadString("Strings\\UI:Profile_Birthday") +  ")" : "")));
                }
            }
            if (options.Count > 0)
            {
                options.Sort((r1, r2) => r1.responseText.CompareTo(r2.responseText));
                List<Response> optionsPage = options.Skip(page * DataLoader.ModConfig.GiftChoicePageSize).Take(DataLoader.ModConfig.GiftChoicePageSize).ToList();
                if (page > 0)
                {
                    optionsPage.Add(new Response(GiftResponseKeyPrevious + (page - 1), DataLoader.I18N.Get("Shipment.Gift.Previous")));
                }
                if (page < (options.Count - 1) / DataLoader.ModConfig.GiftChoicePageSize)
                {
                    optionsPage.Add(new Response(GiftResponseKeyNext + (page + 1), DataLoader.I18N.Get("Shipment.Gift.Next")));
                }
                optionsPage.Add(new Response("None", DataLoader.I18N.Get("Shipment.Gift.None")));
                Game1.player.currentLocation.createQuestionDialogue(DataLoader.I18N.Get("Shipment.Gift.Question", new { Gift = Game1.player.ActiveObject.DisplayName }), optionsPage.ToArray(), GiftDialogKey);
                return false;
            }
            return true;
        }

        internal static void GiftToNpc(string npcName)
        {
            NPC npc = Game1.getCharacterFromName(npcName);
            string giftName = Game1.player.ActiveObject.DisplayName;
            npc.receiveGift(Game1.player.ActiveObject, Game1.player, true, 1, DataLoader.ModConfig.ShowDialogOnItemDelivery);
            ShopMenu.chargePlayer(Game1.player, 0, DataLoader.ModConfig.GiftServiceFee);
            Game1.player.reduceActiveItemByOne();
            if (!DataLoader.ModConfig.ShowDialogOnItemDelivery)
            {
                Game1.drawObjectDialogue(DataLoader.I18N.Get("Shipment.Gift.GiftSent", new { Gift = giftName, Npc = npc.displayName }));
            }
            if (DataLoader.ModConfig.EnableJealousyFromMailedGifts)
            {
                if (npc.datable.Value 
                    && Game1.player.spouse != null 
                    && !Game1.player.spouse.Contains(npc.Name) 
                    && !Game1.player.spouse.Contains("Krobus") 
                    && Utility.isMale(Game1.player.spouse) == Utility.isMale(npc.Name) 
                    && Game1.random.NextDouble() < 0.3 - (double)((float)Game1.player.LuckLevel / 100f) - Game1.player.DailyLuck 
                    && !npc.isBirthday(Game1.currentSeason, Game1.dayOfMonth) 
                    && Game1.player.friendshipData[npc.Name].IsDating())
                {
                    NPC spouse = Game1.getCharacterFromName(Game1.player.spouse);
                    Game1.player.changeFriendship(-30, spouse);
                    spouse.CurrentDialogue.Clear();
                    spouse.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3985", npc.displayName), spouse));
                }
            }
        }
    }
}
