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
using System.Collections.Generic;
using System.Linq;
using FarmVisitors.Datamodels;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Characters;

namespace FarmVisitors.Visit;

public class Data
{
    internal static readonly string[] personalityTraits = new[] { "Outgoing", "Shy", "Polite", "Neutral", "Rude" };

    internal static readonly List<string> bachelors = new() { "Abigail", "Alex", "Elliott", "Emily", "Haley", "Harvey", "Leah", "Maru", "Penny", "Sam", "Sebastian", "Shane" };

    internal static readonly List<string> villagers = new() { "Caroline", "Clint", "Demetrius", "Dwarf", "Evelyn", "George", "Gus", "Jas", "Jodi", "Kent", "Krobus", "Leo", "Lewis", "Linus", "Marnie", "Pam", "Pierre", "Robin", "Sandy", "Vincent", "Willy", "Wizard" };

    internal static readonly List<string> everyone = new() { "Abigail", "Alex", "Caroline", "Clint", "Demetrius", "Dwarf", "Elliott", "Emily", "Evelyn", "George", "Gus", "Haley", "Harvey", "Jas", "Jodi", "Kent", "Krobus", "Leah", "Leo", "Lewis", "Linus", "Marlon", "Marnie", "Maru", "Pam", "Penny", "Pierre", "Robin", "Sam", "Sandy", "Sebastian", "Shane", "Vincent", "Willy", "Wizard" };

    public static Point RandomSpotInSquare(NPC who, int maxdistance, int tries = 30)
    {
        var result = Point.Zero;
        var halved = (int)(maxdistance / 2);

        //rectangle that would be "around" NPC
        var rect = new Rectangle(
            who.TilePoint.X - halved, 
            who.TilePoint.Y - halved, 
            maxdistance, 
            maxdistance
            );

        ModEntry.Log($"Data: \nX={rect.X}, Y={rect.Y}\nWidth={rect.Width}, Height={rect.Height}");

        for (var i=0; i<tries; i++)
        {
            var x = Game1.random.Next(rect.X + maxdistance);
            var y = Game1.random.Next(rect.Y + maxdistance);
            ModEntry.Log($"Loop data: X={x}, Y={y}");

            var isFree = who.currentLocation.CanSpawnCharacterHere(new(x, y)) && who.currentLocation.CanItemBePlacedHere(new(x, y));

            if (!isFree)
                continue;

            var absoluteX = Math.Abs(who.TilePoint.X - x);
            var absoluteY = Math.Abs(who.TilePoint.Y - y);
            var inThreshold = absoluteX <= maxdistance && absoluteY <= maxdistance;


            if (!inThreshold)
                continue;

            result = new Point(x, y);
            break;
        }
        ModEntry.Log($"Chose {result}");

        return result;
    }
    public static Vector2 RandomTile(GameLocation where, NPC who, int maxdistance = 10)
    {
        var tile = Vector2.Zero;
        
        for (var i = 0; i < 15; i++)
        {
            tile = where.getRandomTile();
            //check free. if not, retry
            var free = where.CanSpawnCharacterHere(tile);
            if (!free)
                continue;

            //absolute distance from each other
            var x = Math.Abs(tile.X - who.Position.X);
            var y = Math.Abs(tile.Y - who.Position.Y);
            //if neither passes the given number
            var inRange = x <= maxdistance && y <= maxdistance;

            if (free && inRange)
                break;
        }
        if (ModEntry.Config.Debug)
            ModEntry.Log("New tile: " + tile);

        return tile;
    }
    
    internal static string TurnToString(List<string> list)
    {
        var result = "";

        foreach (var s in list)
        {
            result = s.Equals(list[0]) ? $"{s}" : $"{result}, {s}";
        }
        return result;
    }

    internal static string TurnToString(Stack<Point> stack)
    {
        var result = "";

        foreach (var s in stack)
        {
            result += s.ToString();
            result += ", ";
        }
        return result;
    }

    internal static bool IsScheduleValid(KeyValuePair<string, ScheduleData> pair)
    {
        var patch = pair.Value;
        try
        {
            if (patch.From is 600 || patch.From is 0)
            {
                ModEntry.Log(ModEntry.TL.Get("CantBe600"), LogLevel.Error);
                return false;
            }
            if (patch.To is 2600)
            {
                ModEntry.Log(ModEntry.TL.Get("CantBe2600"), LogLevel.Error);
                return false;
            }

            var inSave = ModEntry.NameAndLevel.ContainsKey(pair.Key);
            if (!inSave)
            {
                ModEntry.Log(ModEntry.TL.Get("NotInSave"), LogLevel.Debug);
                return false;
            }
            if (patch.From >= patch.To && patch.To is not 0)
            {
                ModEntry.Log(ModEntry.TL.Get("FromHigherThanTo"), LogLevel.Error);
                return false;
            }
            return true;
        }
        catch(Exception ex)
        {
            ModEntry.Log($"Error when checking schedule: {ex}", LogLevel.Error);
            return false;
        }
    }

    public static bool InLawOf_vanilla(string who)
    {
        var of = who switch
        {
            "Caroline" => "Abigail",
            "Pierre" => "Abigail",

            "Demetrius" => "Maru&Seb",
            "Robin" => "Maru&Seb",

            "Emily" => "Haley", //emily is inlaw of haley
            "Haley" => "Emily", //and haley of emily.

            "Evelyn" => "Alex",
            "George" => "Alex",

            "Pam" => "Penny",

            "Jodi" => "Sam",
            "Kent" => "Sam",

            "Marnie" => "Shane",

            _ => "none",
        };

        foreach(var spouse in ModEntry.MarriedNPCs)
        {
            if (spouse.Equals("Maru") || spouse.Equals("Sebastian"))
            {
                if (of.Equals("Maru&Seb"))
                {
                    return true;
                }

                return false;
            }

            if (spouse.Equals(of))
            {
                return true;
            }

            return false;
        }

        //if none applied
        return false;
    }

    public static string GetInLawDialogue(string who)
    {
        var choice = Game1.random.Next(0,11);
        string result;

        if (choice >= 5)
        {
            result = who switch
            {
                //for abigail
                "Caroline" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Abigail"]),
                "Pierre" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Abigail"]),

                //for alex
                "Evelyn" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Alex"]),
                "George" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Alex"]),

                //for haley and emily
                "Emily" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Haley"]),
                "Haley" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Emily"]),

                //for maru
                "Demetrius" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Maru"]),

                //for penny
                "Pam" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Penny"]),

                //for sam
                "Jodi" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Sam"]),
                "Kent" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Sam"]),

                //for sebastian
                "Robin" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Sebastian"]),

                //for shane
                "Marnie" => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Shane"]),

                _ => null,
            };
        }
        else
        {
            string notParsed = GetDialogueRaw();
            var spousename = GetSpouseName(who);

            result = string.Format(notParsed, spousename);
        }

        return result;
    }

    private static string GetSpouseName (string who)
    {
        var relatedTo = who switch
        {
            "Caroline" => "Abigail",
            "Pierre" => "Abigail",

            "Demetrius" => "Maru&Seb",
            "Robin" => "Maru&Seb",

            "Emily" => "Haley", //emily is inlaw of haley
            "Haley" => "Emily", //and haley of emily.

            "Evelyn" => "Alex",
            "George" => "Alex",

            "Pam" => "Penny",

            "Jodi" => "Sam",
            "Kent" => "Sam",

            "Marnie" => "Shane",

            _ => "none",
        };

        foreach(var spouse in ModEntry.MarriedNPCs)
        {
            if (spouse.Equals("Maru") && relatedTo.Equals("Maru&Seb"))
            {
                return "Maru";
            }

            if(spouse.Equals("Sebastian") && relatedTo.Equals("Maru&Seb"))
            {
                return "Sebastian";
            }

            if(spouse.Equals(relatedTo))
            {
                return spouse;
            }
        }

        throw new ArgumentException("Character must be related to a vanilla spouse.", nameof(who));
    }

    /// <summary>
    /// For asking about farmer's children.
    /// </summary>
    /// <param name="player">The farmer whose childrens to check.</param>
    /// <returns>A formatted string with a question about the kids.</returns>
    internal static string AskAboutKids(Farmer player)
    {
        var ran = Game1.random.Next(1, 6);

        var kids = player.getChildren();
        string result;
        
        if(kids.Count is 1)
        {
            var raw = ModEntry.TL.Get($"ask.singlechild.{ran}");
            result = string.Format(raw, kids[0].Name);
        }
        else
        {
            var raw = ModEntry.TL.Get($"ask.multiplechild.{ran}");
            result = string.Format(raw, kids[0].Name, kids[1].Name);
        }

        return result;
    }

    /// <summary>
    /// Get all relatives for NPC
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="who"></param>
    /// <returns>A list of characters they're family or friends with.</returns>
    internal static List<string> InlawOf_Mod(Dictionary<string, CharacterData> reference, string who)
    {
        if(!reference.ContainsKey(who))
        {
            ModEntry.Log("NPC not in dictionary!");
            return null;
        }

        List<string> result = new();

        var closeOnes = reference[who].FriendsAndFamily;

        if (closeOnes?.Count <= 0)
            return null;

        //add every entry in friendsandfamily
        foreach (var name in closeOnes.Keys)
        {
            result.Add(name);
        }

        return result;
    }

    /// <summary>
    /// Checks whether current visitor is friends with (or family of) any spouse. If multiple apply (ie in case of friends), chosen randomly.
    /// </summary>
    /// <param name="who"></param>
    /// <returns>The name of the spouse they're related to.</returns>
    public static string GetRelativeName(string who)
    {
        var relatives = new List<string>();

        if (!ModEntry.InLaws.Any())
        {
            return null;
        }

        if (!ModEntry.InLaws.ContainsKey(who))
        {
            return null;
        }
        if(ModEntry.InLaws[who] is null)
        {
            return null;
        }

        foreach(var spousename in ModEntry.MarriedNPCs)
        {
            if (ModEntry.InLaws[who].Contains(spousename))
            {
                relatives.Add(spousename);
            }
        }

        if(relatives.Count <= 0)
            return null;

        var random = Game1.random.ChooseFrom(relatives);
        return random;
    }

    /// <summary>
    /// Get generic dialogue for inlaws.
    /// </summary>
    /// <returns>A string to be formatted.</returns>
    public static string GetDialogueRaw() => Game1.random.ChooseFrom(ModEntry.InlawDialogue["Generic"]);

    /// <summary>
    /// Checks if inlaw is vanilla character.
    /// </summary>
    /// <param name="who"></param>
    /// <returns></returns>
    public static bool IsVanillaInLaw(string who)
    {
        var result = who switch
        {
            "Caroline" => true,
            "Pierre" => true,
            "Demetrius" => true,
            "Robin" => true,
            "Emily" => true,
            "Haley" => true,
            "Evelyn" => true,
            "George" => true,
            "Pam" => true,
            "Jodi" => true,
            "Kent" => true,
            "Marnie" => true,

            _ => false,
        };

        return result;
    }

    internal static Dictionary<string,List<string>> LoadInlawTemplate()
    {
        //we get all bachelors' data
        var temp = new Dictionary<string, List<string>>();
        foreach (var name in bachelors)
        {
            var list = new List<string>(){
                    ModEntry.TL.Get($"InLaw.{name}.1"),
                    ModEntry.TL.Get($"InLaw.{name}.2"),
                    ModEntry.TL.Get($"InLaw.{name}.3")
                };

            temp.Add(name, list);
        }

        //add all generic dialogue
        var list2 = new List<string>();
        for (var i = 0; i < 15; i++)
            list2.Add(ModEntry.TL.Get($"NPCRetiring.Generic.{i}"));

        temp.Add("Generic", list2);

        return temp;
    }

    internal static Dictionary<string, List<string>> LoadRetiringTemplate()
    {
        //we get all npcs' data
        var all = everyone;

        if (ModEntry.Config.Debug)
        {
            var asString = TurnToString(all);
            ModEntry.Log("Data in all: " + asString, LogLevel.Debug);
        }

        var temp2 = new Dictionary<string, List<string>>();
        foreach (var name in all)
        {
            var list = new List<string>(){
                    ModEntry.TL.Get($"NPCRetiring.{name}.1"),
                    ModEntry.TL.Get($"NPCRetiring.{name}.2"),
                    ModEntry.TL.Get($"NPCRetiring.{name}.3")
                };

            temp2.Add(name, list);
        }

        //we get generic dialogue
        foreach (var name in personalityTraits)
        {
            var list = new List<string>() {
                    ModEntry.TL.Get($"NPCRetiring.{name}1"),
                    ModEntry.TL.Get($"NPCRetiring.{name}2"),
                    ModEntry.TL.Get($"NPCRetiring.{name}3")
                };

            temp2.Add(name, list);
        }

        return temp2;
    }

    internal static string AnimalBuildingsTitle()
    {
        var barn = Game1.content.LoadString("Strings/Buildings:Barn_Name");
        var coop = Game1.content.LoadString("Strings/Buildings:Coop_Name");
        var separator = " / ";
        if (barn.Length + coop.Length > 25)
            separator = " /\n";

        var animalbuilding = $"{barn}{separator}{coop}";
        return animalbuilding;
    }
}