/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Additions;
using ItemExtensions.Models.Contained;
using ItemExtensions.Models.Internal;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;

namespace ItemExtensions.Events;

public static class ActionButton
{
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);
    public static void Pressed(object sender, ButtonPressedEventArgs e)
    {
        if(!Context.IsWorldReady)
            return;
        
        if(!ModEntry.ActionButtons.Contains(e.Button))
            return;
        
        //#if DEBUG
        //Log("Button is valid!",LogLevel.Debug);
        //#endif

        if (Game1.player.ActiveObject == null)
            return;

        if (!ModEntry.Data.TryGetValue(Game1.player.ActiveObject.ItemId, out var data))
            return;

        if (data.OnUse == null)
            return;
        
        CheckBehavior(data.OnUse);
    }

    public static void CheckBehavior(OnBehavior behavior)
    {
        if (!string.IsNullOrWhiteSpace(behavior.Conditions) && !GameStateQuery.CheckConditions(behavior.Conditions))
        {
            Log("Conditions for item don't match.");
            return;
        }

        var hasConfirm = !string.IsNullOrWhiteSpace(behavior.Confirm);
        var hasReject = !string.IsNullOrWhiteSpace(behavior.Reject);
        
        if (!string.IsNullOrWhiteSpace(behavior.Message) && (hasConfirm || hasReject))
        {
            var defaultResponse = Game1.currentLocation.createYesNoResponses();
            
            var responses = new[]
            {
                hasConfirm ? new Response("Yes", TokenParser.ParseText(behavior.Confirm)) : defaultResponse[0],
                hasReject ? new Response("No", TokenParser.ParseText(behavior.Reject)) : defaultResponse[1]
            };

            void AfterDialogueBehavior(Farmer who, string whichanswer)
            {
                if(whichanswer == "Yes")
                    RunBehavior(behavior, false);
            }

            Game1.currentLocation.createQuestionDialogue(TokenParser.ParseText(behavior.Message), responses, AfterDialogueBehavior);
        }
        else
        {
            RunBehavior(behavior);
        }
    }

    private static void RunBehavior(OnBehavior behavior, bool directAction = true)
    {
        if (behavior.ReduceBy > 0)
            Game1.player.ActiveObject.ConsumeStack(behavior.ReduceBy);

        if (!string.IsNullOrWhiteSpace(behavior.ChangeMoney))
        {
            Game1.player.Money = IWorldChangeData.ChangeValues(behavior.ChangeMoney, Game1.player.Money, Game1.player.Money);
        }
        
        IWorldChangeData.Solve(behavior);
        
        #region menus
        if(directAction && !string.IsNullOrWhiteSpace(behavior.Message))
        {
            Game1.addHUDMessage(new HUDMessage(TokenParser.ParseText(behavior.Message)));
        }

        if (behavior.ShowNote != null)
        {
            var note = behavior.ShowNote;
            if (!string.IsNullOrWhiteSpace(note.MailId))
            {
                if (DataLoader.Mail(Game1.content).TryGetValue(note.MailId, out var rawMail))
                {
                    var mail = TokenParser.ParseText(rawMail);
                    Game1.activeClickableMenu = new LetterViewerMenu(mail);
                }
            }
            else
            {
                ShowNote(note);
            }
        }
        #endregion
    }

    /// <summary>
    /// Shows a note.
    /// </summary>
    /// <param name="note">Note data.</param>
    /// <see cref="StardewValley.Menus.LetterViewerMenu"/>
    private static void ShowNote(NoteData note)
    {
        var menu = new LetterWithImage(note);
        Game1.activeClickableMenu = menu;
    }

    private static IClickableMenu GetMenuType(string which)
    {
        var split = which.Split(' ');
        var menu = split[0].ToLower();
        
        IClickableMenu result = menu switch
        {
            "forge" => new ForgeMenu(),
            "geode" => new GeodeMenu(),
            "billboard" => new Billboard(),
            "farmhand" => new FarmhandMenu(),
            "animal" or "animals" => new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock(Game1.currentLocation), Game1.currentLocation),
            "end" => new ShippingMenu(Game1.player.displayedShippedItems),
            "tailor" or "sew" or "sewing" => new TailoringMenu(),
            _ => null
        };

        return result;
    }
}