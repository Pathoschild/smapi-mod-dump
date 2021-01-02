/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Underscore76/SDVPracticeMod
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

class Alerts
{
    public static void Success(string message)
    {
        HUDMessage hudMessage = new HUDMessage(message, HUDMessage.achievement_type);
        hudMessage.timeLeft = 1500f;
        Game1.addHUDMessage(hudMessage);
    }

    public static void Failure(string message)
    {
        HUDMessage hudMessage = new HUDMessage(message, HUDMessage.error_type);
        hudMessage.timeLeft = 1500f;
        Game1.addHUDMessage(hudMessage);
    }

    public static void Info(string message)
    {
        HUDMessage hudMessage = new HUDMessage(message, HUDMessage.newQuest_type);
        hudMessage.timeLeft = 1500f;
        Game1.addHUDMessage(hudMessage);
    }
}