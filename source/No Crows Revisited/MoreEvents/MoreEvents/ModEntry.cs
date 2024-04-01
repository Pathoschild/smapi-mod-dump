/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/siweipancc/StardewMods
**
*************************************************/

using System.Reflection;
using MoreEvents.Events.Commands;
using MoreEvents.Events.Precondition;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;


namespace MoreEvents;

using static Logger;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        InitMonitor(Monitor);
        RegisterCommands();
        RegisterPreconditions();
    }

    private static void RegisterPreconditions()
    {
        MethodInfo[] methods = typeof(FarmCavePreconditions).GetMethods(BindingFlags.Static | BindingFlags.Public);
        foreach (MethodInfo method in methods)
        {
            try
            {
                EventPreconditionDelegate eventCommandDelegate =
                    (EventPreconditionDelegate)Delegate.CreateDelegate(typeof(EventPreconditionDelegate), method);
                var name = $"Siweipancc.{method.Name}";
                Event.RegisterPrecondition(name, eventCommandDelegate);
            }
            catch (Exception e)
            {
                Warn(e.Message);
                throw new Exception($"cannot add Precondition: {method.Name}", e);
            }
        }
    }

    private static void RegisterCommands()
    {
        MethodInfo[] methods = typeof(FarmCaveCommands).GetMethods(BindingFlags.Static | BindingFlags.Public);
        foreach (MethodInfo method in methods)
        {
            try
            {
                EventCommandDelegate eventCommandDelegate =
                    (EventCommandDelegate)Delegate.CreateDelegate(typeof(EventCommandDelegate), method);
                var name = $"Siweipancc.{method.Name}";
                Event.RegisterCommand(name, eventCommandDelegate);
            }
            catch (Exception e)
            {
                Warn(e.Message);
                throw new Exception($"cannot add Command: {method.Name}", e);
            }
        }
    }
}