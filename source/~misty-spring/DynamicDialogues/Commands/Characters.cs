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
using StardewValley;

namespace DynamicDialogues.Commands;

internal static class Characters
{

    /// <summary>
    /// Resets the given character's name.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="args"></param>
    /// <param name="context"></param>
    public static void ResetName(Event @event, string[] args, EventContext context)
    {
        if (args.Length <= 1)
        {
            context.LogErrorAndSkip("Must state which NPC to reset name for.");
            return;
        }

        var who = args[1];
        var actor = @event.getActorByName(who);
        if (actor == null)
        {
            context.LogErrorAndSkip("no NPC found with name '" + who + "'");
            return;
        }
        try
        {
            var orig = Game1.characterData[actor.Name].DisplayName;
            actor.displayName = orig;
        }
        catch (Exception)
        {
            context.LogErrorAndSkip("Couldn't find character in NPC data.");
            return;
        }
        @event.CurrentCommand++;
    }

    /// <summary>
    /// Sets NPC as dating, or breaks up.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="args"></param>
    /// <param name="context"></param>
    /// <see cref="NPC.tryToReceiveActiveObject(Farmer, bool)"/>
    internal static void SetDating(Event @event, string[] args, EventContext context)
    {
        //setDating <who> [breakup] [inform]
        if (args.Length <= 1)
        {
            context.LogErrorAndSkip("Must state which NPC to set status for.");
            return;
        }

        var who = Utility.fuzzyCharacterSearch(args[1]);
        ArgUtility.TryGetOptionalBool(args, 3, out var breakUp, out _);
        ArgUtility.TryGetOptionalBool(args, 4, out var inform, out _, true);

        if (who == null)
        {
            context.LogErrorAndSkip("NPC wasn't found.");
            return;
        }

        //if no data, skip
        if (!@event.farmer.friendshipData.TryGetValue(who.Name, out var data))
        {
            context.LogErrorAndSkip("NPC has no friendship data.");
            return;
        }

        //if not datable, skip
        if(!who.datable.Value)
        {
            context.LogErrorAndSkip("NPC isn't datable.");
            return;
        }

        //depending on status, either date, breakup, or nothing
        if (data.Status != FriendshipStatus.Friendly)
        {
            if (data.IsDating() && breakUp)
            {
                //break up
                if(inform)
                {
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Wilted_Bouquet_Effect", who.displayName));
                    Game1.Multiplayer.globalChatInfoMessage("BreakUp", Game1.player.Name, who.GetTokenizedDisplayName());
                }

                data.Status = FriendshipStatus.Friendly;

                if (@event.farmer.spouse == who.Name)
                {
                    @event.farmer.spouse = null;
                }

                data.WeddingDate = null;
                data.Points = Math.Min(data.Points, 1250);
            }
            else
            {
                context.LogErrorAndSkip("NPC must specifically be on friendly terms (no unknown, no dating).");
                return;
            }
        }
        else
        {
            //set dating
            data.Status = FriendshipStatus.Dating;
            if(inform)
                Game1.Multiplayer.globalChatInfoMessage("Dating", Game1.player.Name, who.GetTokenizedDisplayName());
            data.Points += 25;
        }

        @event.CurrentCommand++;
    }

    //never worked out but the code's still here
    /*
    private static void AddJunimo(Event @event, string[] args, EventContext context)
    {
        // format:
        // command <name> <x> <y> <facing> [color]
        //

        if (args.Length < 5)
        {
            //log error
            context.LogErrorAndSkip("Not enough parameters. Must include at minimum: Name, X, Y, facing");
            return;
        }

        var name = args[1];
        var x = int.Parse(args[2]);
        var y = int.Parse(args[3]);
        var facing = int.Parse(args[4]);

        //Get color (if value given). If not, set lime. If that fails, also set lime
        var color = (args.Length >= 6 ? Utility.StringToColor(args[4]) : Color.Lime) ?? Color.Lime;

        var juni = new Junimo()
        {
            Position = new(x, y),
            Name = name,
            displayName = name,
            EventActor = true,
            FacingDirection = facing
        };

        //
        //set net values
        //juni.temporaryJunimo.Set(true);
        //juni.friendly.Set(follow);

        //set color
        //var juni_netcolor = ModEntry.Help.Reflection.GetField<Netcode.NetColor>(juni, "color");
        //var juni_color = juni_netcolor.GetValue();
        //juni_color.Value = color;
        //juni_netcolor.SetValue(juni_color);
        //

        //note: glow is calculated as color * transparency, & junimo alpha is 1
        var npc = new NPC(juni.Sprite, juni.Position, juni.FacingDirection, name)
        {
            Portrait = juni.Portrait,
            eventActor = true,
            Breather = false,
            displayName = name,
            glowingColor = color,
            glowingTransparency = 1, 
            isGlowing = true,
            glowRate = 0,
            AllowDynamicAppearance = false,
            TemporaryDialogue = new Stack<Dialogue>()
        };
        npc.HideShadow = npc.Sprite.SpriteWidth >= 32;

        //add
        @event.actors.Add(npc);
        
        //next command
        @event.CurrentCommand++;
    }

    internal static void SetJunimo(Event @event, string[] args, EventContext context)
    {
        // format:
        // command nameofjuni action [stop]
         
        // possible actions:
        // holdingStar / star / holdstar
        // holdingBundle / bundle / holdbundle
        // stayPut / stay
        // sayingGoodbye / bye / goodbye / saybye / saygoodbye
        // jump
        //

        if (args.Length < 3)
        {
            //log error
            context.LogErrorAndSkip("Not enough parameters. Must include at minimum: Command, Name, Action");
            return;
        }

        var name = args[1];
        var character = @event.getCharacterByName(name);
        
        try
        {
            //set as junimo
            var juni = character as Junimo;
            var action = args[2].ToLower();
            var newval = args.Length >= 4 && bool.Parse(args[3]);
            //reflection stuff
            var rf = ModEntry.Help.Reflection;
            StardewModdingAPI.IReflectedField<Netcode.NetBool> net;
            Netcode.NetBool asbool;

            switch (action)
            {
                case "holdingStar":
                case "star": 
                case "holdstar":
                    net = rf.GetField<Netcode.NetBool>(juni, "holdingStar");
                    asbool = net.GetValue();
                    asbool.Set(newval);
                    net.SetValue(asbool);
                    break;
                case "holdingBundle":
                case "bundle":
                case "holdbundle":
                    net = rf.GetField<Netcode.NetBool>(juni, "holdingBundle");
                    asbool = net.GetValue();
                    asbool.Set(newval);
                    net.SetValue(asbool);
                    break;
                case "stayPut":
                case "stay":
                    net = rf.GetField<Netcode.NetBool>(juni, "stayPut");
                    asbool = net.GetValue();
                    asbool.Set(newval);
                    net.SetValue(asbool);
                    break;
                case "sayingGoodbye": 
                case "bye":
                case "goodbye":
                case "saybye":
                case "saygoodbye":
                    net = rf.GetField<Netcode.NetBool>(juni, "sayingGoodbye");
                    asbool = net.GetValue();
                    asbool.Set(newval);
                    net.SetValue(asbool);
                    break;
                case "jump":
                    juni.jump();
                    context.Location.playSound("junimoMeep1");
                    break;
                default: 
                    context.LogError("Couldn't recognize the action. (Allowed values: holdstar, holdbundle, stayput, saygoodbye, jump)");
                    break;
            }

            //update and go next command
            juni.update(Game1.currentGameTime,context.Location);
            @event.CurrentCommand++;
        }
        catch(Exception ex)
        {
            context.LogErrorAndSkip("Error: " + ex);
            return;
        }
    }*/
}
