/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;
using SpaceShared;
using StardewModdingAPI;
using Microsoft.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;
using System.Runtime.Loader;

namespace ConsoleCode
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod Instance;

        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            helper.ConsoleCommands.Add("cs", "Execute C# code.", this.OnCommandReceived);
        }

        private void OnCommandReceived(string cmd, string[] args)
        {
            string line = string.Join(" ", args).Replace('`', '"');
            string[] scriptArgs = null;
            if (args[0] == "--script")
            {
                line = File.ReadAllText(Path.Combine(this.Helper.DirectoryPath, args[1]));
                scriptArgs = args.Skip( 2 ).ToArray();
            }
            Log.Trace($"Input: {line}");
            try
            {
                var func = this.MakeFunc(line);
                if ( func != null )
                {
                    object result = func?.Invoke( null, new object[] { Helper, scriptArgs } );
                    switch ( result )
                    {
                        case null:
                            Log.Info( "Output: <null>" );
                            break;

                        case string:
                            Log.Info( $"Output: \"{result}\"" );
                            break;

                        default:
                            Log.Info( $"Output: {result}" );
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception: " + e);
            }
        }

        private static int iter = 0;

        private MethodInfo MakeFunc(string userCode)
        {
            List<MetadataReference> refs = new();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    refs.Add( MetadataReference.CreateFromFile( asm.Location ) );
                }
                catch ( Exception e )
                {
                    //Log.Trace("Couldn't add assembly " + asm + ": " + e);
                }
            }

            string code = $@"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using StardewModdingAPI;
                using Microsoft.Xna.Framework;
                using Microsoft.Xna.Framework.Graphics;
                using StardewValley;
                using xTile;

                namespace ConsoleCode
                {{
                    public class UserCode{iter}
                    {{
                        public static object Main(IModHelper Helper, string[] args)
                        {{
                            {userCode}
                            return null;
                        }}
                    }}
                }}
            ";

            CSharpCompilation compilation = CSharpCompilation.Create( Path.GetRandomFileName(), new[] { CSharpSyntaxTree.ParseText( code ) }, refs, new CSharpCompilationOptions( OutputKind.DynamicallyLinkedLibrary ) );

            using MemoryStream ms = new();
            var result = compilation.Emit( ms );

            if ( !result.Success )
            {
                var fails = result.Diagnostics.Where( d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error );

                StringBuilder sb = new();
                sb.AppendLine( "Errors: " );
                foreach ( var fail in fails )
                {
                    sb.AppendLine( $"{fail.Id}: {fail.GetMessage()}" );
                }
                Log.Error( sb.ToString() );

                return null;
            }
            else
            {
                int i = iter++;

                ms.Seek( 0, SeekOrigin.Begin );
                Assembly asm = AssemblyLoadContext.Default.LoadFromStream( ms );
                Type type = asm.GetType( $"ConsoleCode.UserCode{i}" );
                MethodInfo meth = type.GetMethod( "Main" );
                return meth;
            }
        }
    }
}
