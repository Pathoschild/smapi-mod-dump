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
