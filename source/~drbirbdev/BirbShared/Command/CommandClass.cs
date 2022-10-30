/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace BirbShared.Command
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandClass : Attribute
    {
        public string Name = "";
        public string Prefix = "birb_";
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandMethod : Attribute
    {
        public string Docs = "";

        public CommandMethod(string docs)
        {
            this.Docs = docs;
        }
    }
}
