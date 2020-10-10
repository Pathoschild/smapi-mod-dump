/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitalizeUpdater
{
    class Program
    {
        static void Main()
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "C:\\Users\\owner\\Desktop\\games\\Stardew_stuff\\XNB\\UpdateAndRunStardew.bat";
            proc.StartInfo.WorkingDirectory = "C:\\Users\\owner\\Desktop\\games\\Stardew_stuff\\XNB";
            proc.Start();
        }
    }
}
