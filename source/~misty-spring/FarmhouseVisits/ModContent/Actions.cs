/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Linq;
using FarmVisitors.Datamodels;
using FarmVisitors.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Pathfinding;
using lv = StardewModdingAPI.LogLevel;

namespace FarmVisitors.Visit;

internal static class Actions
{
    #region normal visit
    //regular visit: the one used by non-scheduled NPCs
    internal static void AddToFarmHouse(NPC visitor, FarmHouse farmHouse, bool hadConfirmation)
    {
        try
        {
            if (!Values.IsVisitor(visitor.Name))
            {
                ModEntry.Log($"{visitor.displayName} is not a visitor!");
                return;
            }

            if (hadConfirmation == false)
            {
                DrawDialogue(visitor, Values.GetDialogueType(visitor, DialogueType.Introduce));
            }

            var position = farmHouse.getEntryLocation();
            position.Y--;
            visitor.faceDirection(0);
            Game1.warpCharacter(visitor, farmHouse, position.ToVector2());

            var textBubble = string.Format(Values.GetDialogueType(visitor, DialogueType.WalkIn), Game1.player.Name);
            visitor.showTextAboveHead(textBubble);

            //set before greeting because "Push" leaves dialogues at the top
            if (Game1.player.isMarriedOrRoommates())
            {
                if (ModEntry.Config.InLawComments is not "None")
                    InLawActions(visitor);
            }

            var text = Values.GetDialogueType(visitor, DialogueType.Greet);
            var randomInt = Game1.random.Next(101);
            if (ModEntry.Config.GiftChance >= randomInt)
            {
                var withGift = $"{text}#$b#{Values.GetGiftDialogue(visitor)}";

                if (ModEntry.Config.Debug)
                    ModEntry.Log($"withGift: {withGift}");

                text = string.Format(withGift, Values.GetSeasonalGifts());
            }

            SetDialogue(visitor, text);

            PushDialogue(visitor, Values.GetDialogueType(visitor, DialogueType.Thanking));

            if (Game1.currentLocation.Equals(farmHouse))
            {
                Game1.currentLocation.playSound("doorClose");
            }

            position.Y--;
            visitor.controller = new PathFindController(visitor, farmHouse, position, 0);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error while adding to farmhouse: {ex}", lv.Error);
        }

    }

    internal static void Retire(NPC c)
    {
        var currentLocation = Game1.player.currentLocation;
        var inFarm = Values.NPCinScreen();

        if (currentLocation.Equals(c.currentLocation))
        {
            try
            {
                if (c.controller != null)
                {
                    c.Halt();
                    c.controller = null;
                }
                Game1.fadeScreenToBlack();
            }
            catch (Exception ex)
            {
                ModEntry.Log($"An error ocurred when pathing to entry: {ex}", lv.Error);
            }
            finally
            {
                DrawDialogue(c, Values.GetDialogueType(c, DialogueType.Retiring));
                ReturnToNormal(c);
                if (!inFarm)
                {
                    Game1.currentLocation.playSound("doorClose");
                }
            }
        }
        else
        {
            try
            {
                Leave(c);
            }
            catch (Exception ex)
            {
                ModEntry.Log($"An error ocurred when pathing to entry: {ex}", lv.Error);
            }
        }
    }

    private static void InLawActions(NPC visitor)
    {
        var addedAlready = false;
        var name = visitor.Name;

        if (!ModEntry.Config.ReplacerCompat && Data.IsVanillaInLaw(name))
        {
            if (Data.InLawOf_vanilla(name))
            {
                SetDialogue(visitor, Data.GetInLawDialogue(name));
                addedAlready = true;
            }
        }

        if (ModEntry.Config.InLawComments is "VanillaAndMod" || ModEntry.Config.ReplacerCompat)
        {
            var spouse = Data.GetRelativeName(name);
            if (spouse is not null && !addedAlready)
            {
                var formatted = string.Format(Data.GetDialogueRaw(), spouse);
                SetDialogue(visitor, formatted);
                addedAlready = true;
            }
        }

        if (Game1.player.getChildrenCount() > 0 && addedAlready)
        {
            SetDialogue(visitor, Data.AskAboutKids(Game1.player));
        }
    }
    #endregion

    #region custom
    // customized visits: ones set by user via ContentPatcher
    internal static void AddCustom(NPC c, FarmHouse farmHouse, ScheduleData data, bool hadConfirmation)
    {
        try
        {
            if (!Values.IsVisitor(c.Name))
            {
                ModEntry.Log($"{c.displayName} is not a visitor!");
                return;
            }

            if (hadConfirmation == false)
            {
                //if custom entry question, use that. if not, normal one
                string text;
                if (!string.IsNullOrWhiteSpace(data.EntryQuestion))
                    text = data.EntryQuestion;
                else
                    text = Values.GetDialogueType(c, DialogueType.Introduce);

                DrawDialogue(c, text);
            }

            //warp
            var position = farmHouse.getEntryLocation();
            position.Y--;
            c.faceDirection(0);
            Game1.warpCharacter(c, farmHouse, position.ToVector2());

            //if custom entry text, use that. if not, get normal text
            string textBubble;
            if (!string.IsNullOrWhiteSpace(data.EntryBubble))
                textBubble = string.Format(data.EntryBubble, Game1.player.Name);
            else
                textBubble = string.Format(Values.GetDialogueType(c, DialogueType.WalkIn), Game1.player.Name);

            c.showTextAboveHead(textBubble);

            //if has entry dialogue, set
            if (!string.IsNullOrWhiteSpace(data.EntryDialogue))
            {
                SetDialogue(c, data.EntryDialogue);
            }
            else
            {
                //otherwise, generic dialogue with % of gift
                var enterDialogue = Values.GetDialogueType(c, DialogueType.Greet);
                var randomInt = Game1.random.Next(101);
                if (ModEntry.Config.GiftChance >= randomInt)
                {
                    enterDialogue += "#$b#" + Values.GetGiftDialogue(c);
                    enterDialogue = string.Format(enterDialogue, Values.GetSeasonalGifts());
                }
                SetDialogue(c, enterDialogue);
            }

            if (Game1.currentLocation.Equals(farmHouse))
            {
                Game1.currentLocation.playSound("doorClose");
            }

            position.Y--;
            c.controller = new PathFindController(c, farmHouse, position, 0);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error while adding to farmhouse: {ex}", lv.Error);
        }

    }

    internal static void RetireCustom(NPC c, string text)
    {
        if (Game1.player.currentLocation.Equals(c.currentLocation))
        {
            try
            {
                if (c.controller is not null)
                {
                    c.Halt();
                    c.controller = null;
                }
                Game1.fadeScreenToBlack();
            }
            catch (Exception ex)
            {
                ModEntry.Log($"An error ocurred when pathing to entry: {ex}", lv.Error);
            }
            finally
            {
                DrawDialogue(c, text);

                ReturnToNormal(c);
                Game1.currentLocation.playSound("doorClose");
            }
        }
        else
        {
            try
            {
                Leave(c);
            }
            catch (Exception ex)
            {
                ModEntry.Log($"An error ocurred when pathing to entry: {ex}", lv.Error);
            }
        }
    }

    internal static void AddWhileOutside(NPC visitor)
    {
        try
        {
            var farmHouse = Utility.getHomeOfFarmer(Game1.player);

            if (!Values.IsVisitor(visitor.Name))
            {
                ModEntry.Log($"{visitor.displayName} is not a visitor!");
                return;
            }

            var position = farmHouse.getEntryLocation();
            position.Y--;
            visitor.faceDirection(0);
            Game1.warpCharacter(visitor, farmHouse, position.ToVector2());

            visitor.showTextAboveHead(string.Format(Values.GetDialogueType(visitor, DialogueType.WalkIn), Game1.player.Name));

            position.Y--;
            visitor.controller = new PathFindController(visitor, farmHouse, position, 0);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error while adding to farmhouse: {ex}", lv.Error);
        }
    }
    #endregion

    #region used by both

    /// <summary>
    /// Create dialogue and push to front of stack.
    /// </summary>
    /// <param name="who"></param>
    /// <param name="text"></param>
    internal static void PushDialogue(NPC who, string text)
    {
        var dialogue = new Dialogue(who, null, text);
        who.CurrentDialogue.Push(dialogue);
    }

    /// <summary>
    /// Set dialogue 
    /// </summary>
    /// <param name="who"></param>
    /// <param name="text"></param>
    internal static void SetDialogue(NPC who, string text, bool add = true)
    {
        var dialogue = new Dialogue(who, null, text);
        who.setNewDialogue(dialogue, add);
    }

    /// <summary>
    /// Draws dialogue to screen.
    /// </summary>
    /// <param name="who"></param>
    /// <param name="text"></param>
    // Because this method doesn't exist anymore in Game1, we do its equiv.
    public static void DrawDialogue(NPC who, string text)
    {
        var db = new Dialogue(who, null, text);
        who.CurrentDialogue.Push(db);
        Game1.drawDialogue(who);
    }

    private static void ReturnToNormal(NPC c)
    {
        var where = c.currentLocation;
        where.characters.Remove(c);

        ModEntry.SetNoVisitor();
    }

    private static void Leave(NPC c)
    {

        if (c.controller is not null)
        {
            c.Halt();
            c.controller = null;
        }
        Game1.drawObjectDialogue(string.Format(Values.GetNpcGone(Game1.currentLocation.Name.StartsWith("Cellar")), c.displayName));

        ReturnToNormal(c);
    }

    public static void GoToSleep(NPC who, VisitData context)
    {
        var bed = Values.GetBedSpot();
        if (bed == Point.Zero)
        {
            ModEntry.Log("Found no bed. Visit {who.Name} won't stay over.", lv.Warn);
            Retire(who);
            return;
        }

        context.IsGoingToSleep = true;

        var home = Utility.getHomeOfFarmer(Game1.player);
        if (!who.currentLocation.Equals(home))
        {
            Game1.fadeScreenToBlack();
            Game1.warpCharacter(who, home, home.getEntryLocation().ToVector2());

            if (Game1.player.currentLocation.Equals(home))
                Game1.player.currentLocation.playSound("doorClose");
            else
            {
                var rawtext = ModEntry.TL.Get("NPCGoneToSleep");
                var formatted = string.Format(rawtext, who.displayName);
                Game1.drawDialogueBox(formatted);
            }
        }

        who.doEmote(24, false);

        who.controller = new PathFindController(
            who,
            home,
            bed,
            1,
            DoSleep
        );
    }

    private static void DoSleep(Character c, GameLocation location) => (c as NPC).playSleepingAnimation();

    internal static void WalkAroundFarm(NPC c)
    {
        var where = Game1.getFarm();
        var newspot = Data.RandomTile(where, c, 15);

        if (newspot != Vector2.Zero)
        {
            c.temporaryController = null;
            c.controller = new PathFindController(
                c,
                where,
                newspot.ToPoint(),
                Game1.random.Next(0, 4)
                );

            if (ModEntry.Config.Debug)
            {
                ModEntry.Log($"is the controller empty?: {c.controller == null}", lv.Debug);
            }
        }

        if (Game1.random.Next(0, 11) <= 5)
        {
            var anyCrops = ModEntry.Crops.Any();

            if (Game1.currentSeason == "winter")
            {
                SetDialogue(c, Values.GetDialogueType(c, DialogueType.Winter));
            }
            else if ((Game1.random.Next(0, 2) <= 0 || !anyCrops) && ModEntry.Animals.Any())
            {
                var animal = Game1.random.ChooseFrom(ModEntry.Animals);
                var rawtext = Values.GetDialogueType(c, DialogueType.Animal);
                var formatted = string.Format(rawtext, animal);
                SetDialogue(c, formatted);
            }
            else if (anyCrops)
            {
                var crop = Game1.random.ChooseFrom(ModEntry.Crops);
                var rawtext = Values.GetDialogueType(c, DialogueType.Crop);
                var formatted = string.Format(rawtext, crop);
                SetDialogue(c, formatted);
            }
            else
            {
                SetDialogue(c, Values.GetDialogueType(c, DialogueType.NoneYet));
            }
        }
    }

    #endregion    
}