/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Framework.Caches;

using StardewModdingAPI.Events;
using StardewValley.Characters;
using StardewValley.Locations;

namespace PamTries;

internal static class DialogueManager
{
    public static void GrandKidsDialogue(object? sender, DayStartedEventArgs e)
    {
        if (NPCCache.GetByVillagerName("Pam") is not NPC pam)
        {
            return;
        }
    }

    public static string? CurrentMovie()
    {
        if (!Context.IsWorldReady)
        {
            return null;
        }
        return MovieTheater.GetMovieForDate(Game1.Date)?.ID;
    }

    internal static string ChildCount()
        => $"{Game1.player.getChildrenCount()}";

    internal static string? ListChildren()
    {
        if (!Context.IsWorldReady)
        {
            return null;
        }

        List<Child> children = Game1.player.getChildren();
        string and = PTUtilities.GetLexicon("and");

        if (children is null || children.Count == 0)
        {
            return string.Empty;
        }
        else if (children.Count == 1)
        {
            return children[0].displayName;
        }
        else if (children.Count == 2)
        {
            return $"{children[0].displayName} {and} {children[1].displayName}";
        }
        else
        {
            List<string> kidnames = children.Select((Child child) => child.displayName).ToList();
            return $"{string.Join(", ", kidnames, 0, kidnames.Count - 1)}, {and} {kidnames[kidnames.Count]}";
        }// deal with the possibility other countries have different grammer later.
    }

    internal static NPC? GetChildbyGender(string gender)
    {
        if (!Context.IsWorldReady)
        {
            return null;
        }
        int gender_int;
        switch (gender)
        {
            case "girl":
                gender_int = 1;
                break;
            case "boy":
                gender_int = 0;
                break;
            default:
                return null;
        }
        foreach (NPC child in Game1.player.getChildren())
        {
            if (child.Gender == gender_int)
            {
                return child;
            }
        }
        return null;
    }
}