/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomTokens
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Characters;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTokens
{
    class ChildTokens
        : BaseAdvancedToken
    {
        /// <summary>
        /// A list of the current children for each player, as of the last context update
        /// </summary>
        private readonly Dictionary<string, List<Child>> children;

        private readonly List<string> acceptedarguments = new List<string> { "birthdayday", "birthdayseason", "daysold", "darkskinned", "hat" };

        public ChildTokens()
        {
            // Create a new dictionary with a key for each playertype
            children = new Dictionary<string, List<Child>>(StringComparer.OrdinalIgnoreCase)
            {
                [main] = new List<Child>(),
                [local] = new List<Child>()
            };       
        }

        //Token is ready, when world is ready
        //The token allows and requires input
        //The token does not allow multiple values
       
        public override bool TryValidateInput(string input, out string error)
        {

            error = "";
            // Get input arguments
            string[] tokenarg = input.ToLower().Trim().Split('|');

            // Are the correct number of input arguments present
            if (tokenarg.Count() == 3)
            {
                // Yes, validate each argument

                // Could not find player input argument, add to error
                if (tokenarg[0].Contains("player=") == false)
                {
                    error += "player argument not found. ";
                }
              
                // Player input argument has invalid value, add to error
                else
                {
                    string playerType = tokenarg[0].Substring(tokenarg[0].IndexOf('=') + 1).Trim().Replace("player", "");

                    if (playerType.Equals("host") == false && playerType.Equals("local") == false)
                    {
                        error += "player argument must be one of the following values: 'host', 'local'. ";
                    }
                }

                // Could not find childindex input argument, add to error
                if (tokenarg[1].Contains("childindex=") == false)
                {
                    error += "childindex argument not found. ";
                }

                // Childindex input argument in not numerical as expected, add to error
                else
                {
                    string childindex = tokenarg[1].Substring(tokenarg[1].IndexOf('=') + 1).Trim().Replace("childindex", "");
                    if (childindex.Any(ch => char.IsDigit(ch) == false && ch != ' ') || childindex == "")
                    {
                        error += "childindex argument must be numeric. ";
                    }
                }

                bool foundacceptedargument = false;
                // Remove the '=' character at the end of the argument that appears for some reason
                var formattedtoken = tokenarg[2].Replace("=", "");

                // Iterate through each accepted argument at index 2
                foreach (var argument in acceptedarguments)
                { 
                    // Match found, set foundacceptedargument to true and break from loop
                    if (formattedtoken.Equals(argument) == true)
                    {
                        foundacceptedargument = true;
                        break;
                    }                    
                }

                // No accepted arguments found at index 2, add to error
                if (foundacceptedargument == false)
                {
                    error += "unrecognised argument value at index 2. Must be one of 'birthdayday' 'birthdayseason' 'daysold' 'darkskinned' 'hat'.";
                }
            }

            // Too many or too little input arguments found, add to error
            else
            {
                error = "Incorrect number of arguments.";
            }

            // Values are valid if error is an empty string
            return error.Equals("");
        }

        public override IEnumerable<string> GetValues(string input)
        {
            var output = new List<string>();

            string[] args = input.Split('|');

            // get specified player type
            string playertype = args[0].Substring(args[0].IndexOf('=') + 1).Trim().ToLower().Replace("player", "").Replace(" ", "");
            // get specified child index
            int childindex = Convert.ToInt32(args[1].Substring(args[1].IndexOf('=') + 1).Trim().ToLower().Replace("childindex", "").Replace(" ", ""));
            // get the value needed, remove any equal signs that show up, no idea why
            string tokenvalue = args[2].Trim().ToLower().Replace("=", "");          

            // player is the host
            if (playertype == "host")
            {
                // Get the required value
                bool found = TryGetValue(main, childindex, tokenvalue, out string hostdata);

                if (found == true)
                {
                    // Value was found, add it to the output list
                    output.Add(hostdata);
                }
                else
                {
                    // Value not found, add "null"
                    output.Add("null");
                }
            }

            // player is a connected farmhand
            else if (playertype == "local")
            {
                // Get the required value
                bool found = TryGetValue(local, childindex, tokenvalue, out string hostdata);

                if (found == true)
                {
                    // Value was found, add it to the output list
                    output.Add(hostdata);
                }
                else
                {
                    // Value not found, add "null"
                    output.Add("null");
                }
            }

            return output;
        }

        private bool TryGetValue(string playertype, int index, string token, out string founddata)
        {
            bool found = false;
            founddata = "";

            // player is also the local player if they are the main player, correct this
            if (Game1.IsMasterGame == true && playertype.Equals(local) == true)
            {
                playertype = main;
            }

            foreach (var child in this.children[playertype])
            {
                // Make sure the index of the child matches the given index to search for
                if (child.GetChildIndex() == index)
                {
                    // Found child of correct index, get data
                    found = true;

                    // Calculate birthday date based on child age
                    var birthday = SDate.Now().AddDays(-child.daysOld.Value) ?? SDate.Now();

                    // Change string value based on input argument at index 2
                    switch (token)
                    {
                        case "birthdayday":
                            founddata = birthday.Day.ToString();
                            break;
                        case "birthdayseason":
                            founddata = birthday.Season.ToString();
                            break;
                        case "daysold":
                            founddata = child.daysOld.ToString();
                            break;
                        case "darkskinned":
                            // Value is initially True or False, make all lowercase for ease of use
                            founddata = child.darkSkinned.Value.ToString().ToLower();
                            break;
                        case "hat":
                            // If no hat is found, change founddata to "null", else the name of the hat
                            founddata = child.hat.Value == null ? "null" : child.hat.Value.Name.ToString();
                            break;
                    }

                }
            }
                     
            return found;
        }

        protected override bool DidDataChange()
        {
            bool hasChanged = false;
            string playertype;

            // Update child data for local player if a connected farmhand
            if (Game1.IsMasterGame == false)
            {
                playertype = local;

                if (Game1.player.getChildren().Equals(this.children[playertype]) == false)
                {
                    hasChanged = true;
                    this.children[playertype] = Game1.player.getChildren();
                }

            }

            // Update child data for main player, occurs for all players
            playertype = main;

            if (Game1.MasterPlayer.getChildren().Equals(this.children[playertype]) == false)
            {
                hasChanged = true;
                this.children[playertype] = Game1.MasterPlayer.getChildren();
            }

            return hasChanged;
        }
    }
}
