/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thespbgamer/LovedLabelsRedux
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LovedLabelsRedux
{
    internal class ModConfig
    {
        public KeybindList KeybindListToggleUIKey { get; set; } = KeybindList.Parse("LeftShift + OemPipe");

        public String AlreadyPettedMessage { get; set; } = "Already Petted";
        public String NeedsPettingMessage { get; set; } = "Needs Petting";

        public bool IsUIEnabled { get; set; } = true;

        public bool IsPettingEnabled { get; set; } = false;
    }
}