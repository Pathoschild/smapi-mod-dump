/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Mono.CSharp;
using SpaceShared;
using StardewModdingAPI;

namespace ConsoleCode
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = Monitor;

            helper.ConsoleCommands.Add("cs", "Execute C# code.", onCommandReceived);
        }

        private void onCommandReceived( string cmd, string[] args )
        {
            string line = string.Join(" ", args).Replace('`', '"');
            if ( args[0] == "--script" )
            {
                line = File.ReadAllText(Path.Combine(Helper.DirectoryPath, args[1]));
            }
            Log.trace($"Input: {line}");
            try
            {
                var func = makeFunc(line);
                object result = null;
                func.Invoke(ref result);
                if (result == null)
                    Log.info("Output: <null>");
                else if (result is string)
                    Log.info($"Output: \"{result}\"");
                else
                    Log.info($"Output: {result}");
            }
            catch (Exception e)
            {
                Log.error("Exception: " + e);
            }
        }
        
        private CompiledMethod makeFunc(string userCode)
        {
            var settings = new CompilerSettings
            {
                Unsafe = true
            };

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    settings.AssemblyReferences.Add(asm.CodeBase);
                }
                catch
                {
                    //Log.trace("Couldn't add assembly " + asm + ": " + e);
                }
            }

            var eval = new Evaluator(new CompilerContext(settings, new ConsoleReportPrinter()));
            var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using xTile;";
            eval.Compile(code);
            return eval.Compile("IModHelper Helper = ConsoleCode.Mod.instance.Helper;\n" + userCode);
        }
    }
}
