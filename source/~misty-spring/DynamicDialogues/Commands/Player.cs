/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace DynamicDialogues.Commands;

internal static class Player
{
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);

    /// <summary>
    /// Broadcast mail to all players.
    /// </summary>
    /// <param name="event">Event</param>
    /// <param name="args">Parameters to use.</param>
    /// <param name="context">Event context.</param>
    public static void MultiplayerMail(Event @event, string[] args, EventContext context) =>
        Game1.Multiplayer.broadcastPartyWideMail(args[1]);

    /// <summary>
	/// Add skill experience to player(s).
	/// </summary>
	/// <param name="event">Event</param>
	/// <param name="args">Parameters to use.</param>
	/// <param name="context">Event context.</param>
	public static void AddExp(Event @event, string[] args, EventContext context)
    {
        /* if values exist, use. if not, use defaults
		 * legend: <required>, [optional]
		 * 
		 * AddExp <skill> [amt] [who]
		 *   0       1      2     3
		 *
         */

        if (args.Length <= 1)
        {
            context.LogErrorAndSkip("Must state which skill to add EXP to.");
            return;
        }

        ArgUtility.TryGetOptionalInt(args, 2, out var amount, out _, 50);
        ArgUtility.TryGetOptional(args, 3, out var whoRaw, out _, "current");

        var skill = args[1].ToLower() switch
        {
            "farming" => 0,
            "fishing" => 1,
            "foraging" => 2,
            "mining" => 3,
            "combat" => 4,
            "luck" => 5,
            _ => 0
        };

#if DEBUG
        Log($"Values:\namount = {amount}\nwho = {whoRaw}, \nskill = {skill}({args[1]})", LogLevel.Debug);
#endif
        
        if (whoRaw.ToLower() is "all" or "multiplayer" or "any")
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                farmer.gainExperience(skill, amount);
            }
        }
        else
        {
            var who = whoRaw.ToLower() switch
            {
                "local" or "current" => Game1.player,
                "host" => Game1.MasterPlayer,
                _ => Game1.player
            };

            who.gainExperience(skill, amount);
        }
        @event.CurrentCommand++;
    }

    internal static void Health(Event @event, string[] args, EventContext context)
    {
        //health <action> <amt>

        if (args.Length < 3)
        {
            context.LogErrorAndSkip("Must state an action and amount.");
            return;
        }

        var type = args[1].ToLower();
        var amt = int.Parse(args[2]);
        var addsOrReduces = type switch
        {
            "add" => true,
            "more" => true,
            "reduce" => true,
            "less" => true,
            "+" => true,
            "-" => true,
            _ => false
        };

        if(addsOrReduces)
        {
            Log("Adding/Substracting from player health.");

            //add/reduce hp
            if (type is "less" or "-" or "reduce")
            {
                var trueAmt = @event.farmer.health - amt;
                @event.farmer.health = trueAmt <= 0 ? 1 : trueAmt;
            }
            else
            {
                var trueAmt = @event.farmer.health + amt;
                @event.farmer.health = trueAmt >= @event.farmer.maxHealth ? @event.farmer.maxHealth : trueAmt;
            }
        }
        else if (type == "reset")
        {
            Log("Resetting player health.");
            @event.farmer.health = @event.farmer.maxHealth;
        }
        else
        {
            Log("Setting player health.");
            //set
            @event.farmer.health = amt;
        }

        @event.CurrentCommand++;
    }

    internal static void Stamina(Event @event, string[] args, EventContext context)
    {
        //stamina <action> <amt>

        if (args.Length < 3)
        {
            context.LogErrorAndSkip("Must state an action and amount.");
            return;
        }

        var type = args[1].ToLower();
        var amt = int.Parse(args[2]);
        var addsOrReduces = type switch
        {
            "add" => true,
            "more" => true,
            "reduce" => true,
            "less" => true,
            "+" => true,
            "-" => true,
            _ => false
        };

        if (addsOrReduces)
        {
            Log("Adding/Substracting from player stamina.");

            //add/reduce hp
            if (type is "less" or "-" or "reduce")
            {
                var trueAmt = @event.farmer.stamina - amt;
                @event.farmer.stamina = trueAmt <= 0 ? 1 : trueAmt;
            }
            else
            {
                var trueAmt = @event.farmer.stamina + amt;
                @event.farmer.stamina = trueAmt >= @event.farmer.MaxStamina ? @event.farmer.MaxStamina : trueAmt;
            }
        }
        else if (type == "reset")
        {
            Log("Resetting player stamina.");
            @event.farmer.stamina = @event.farmer.MaxStamina;
        }
        else
        {
            Log("Setting player stamina.");
            //set
            @event.farmer.stamina = amt;
        }

        @event.CurrentCommand++;
    }
}