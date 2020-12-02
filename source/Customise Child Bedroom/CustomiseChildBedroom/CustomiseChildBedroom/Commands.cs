/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/CustomiseChildBedroom
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomiseChildBedroom
{
    class Commands
    {

        IModHelper Helper;
        public Commands(IModHelper helper)
        {
            Helper = helper;

            MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach(MethodInfo method in methods)
            {
                string MethodName;
                string MethodDescription;

                try
                {
                    MethodName = method.GetCustomAttribute<CommandName>().Name;
                }
                catch
                {
                    continue;
                }
                MethodDescription = Translation.GetString($"command.{MethodName}.description", new { CommandName = MethodName });

                Helper.ConsoleCommands.Add(MethodName, MethodDescription, (Action<string, string[]>)method.CreateDelegate(typeof(Action<string, string[]>), this));

            }
        }
        
        [CommandName("togglecrib")]
        internal void ToggleCrib(string command, string[] args)
        {
            string MethodName = MethodBase.GetCurrentMethod().GetCustomAttribute<CommandName>().Name;

            //handling spaces in names
            //options: 
            //replace spaces with other character - not ideal
            //l2handle them

            if (args.Length == 0 || args.Length > 1) //no player name given
            {
                CallHelp(MethodName);
            }
            else
            {
                string FarmerName = args[0];
                Farmer Who = ModEntry.Config.GetCurrentFarm().GetFarmer(FarmerName);
                if (Who is null)
                {
                    ModEntry.LogError(Translation.GetString("error.farmernamenotfound", new { FarmerName }));
                    return;
                }

                Who.ToggleCrib();

                ModEntry.Log(Translation.GetString("command.togglecrib.toggledo" + (Who.ShowCrib ? "n" : "ff"), new { FarmerName }));
                ModEntry.Config.Save();
            }
        }

        [CommandName("togglebed")]
        internal void ToggleBed(string command, string[] args)
        {
            string MethodName = MethodBase.GetCurrentMethod().GetCustomAttribute<CommandName>().Name;

            if (args.Length == 0 || args.Length > 2)
            {
                CallHelp(MethodName);
            }
            else
            {
                string FarmerName = args[0];
                Farmer Who = ModEntry.Config.GetCurrentFarm().GetFarmer(FarmerName);
                if (Who is null)
                {
                    ModEntry.LogError(Translation.GetString("error.farmernamenotfound", new { FarmerName }));
                    return;
                }

                bool DoLeft = false;
                bool DoRight = false;

                if (args.Length == 1)
                {
                    DoLeft = true;
                    DoRight = true;
                }
                else
                {
                    if (args[1].Equals("left", StringComparison.OrdinalIgnoreCase))
                    {
                        DoLeft = true;
                    }
                    else if (args[1].Equals("right", StringComparison.OrdinalIgnoreCase))
                    {
                        DoRight = true;
                    }
                    else
                    {
                        CallHelp(MethodName);
                        return;
                    }
                }

                if (DoLeft)
                {
                    Who.ToggleLeftBed();
                    ModEntry.Log(Translation.GetString("command.togglebed.left.toggledo" + (Who.ShowLeftBed ? "n" : "ff"), new { FarmerName }));
                    
                }
                if (DoRight)
                {
                    Who.ToggleRightBed();
                    ModEntry.Log(Translation.GetString("command.togglebed.right.toggledo" + (Who.ShowRightBed ? "n" : "ff"), new { FarmerName }));
                }

                if(DoLeft || DoRight)
                {
                    ModEntry.Config.Save();
                }
            }

        }

        private void CallHelp(params string[] CommandName) => Helper.ConsoleCommands.Trigger("help", CommandName);


    }
}
