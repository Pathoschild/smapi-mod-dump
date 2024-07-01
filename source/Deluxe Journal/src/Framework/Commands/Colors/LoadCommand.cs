/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;

namespace DeluxeJournal.Framework.Commands.Colors
{
    internal class LoadCommand : ColorCommand
    {
        public LoadCommand() : base("dj_colors_load",
            BuildDocString("Load color data.",
                "dj_colors_load [filename]",
                $"filename: Optional file name. Loads from '{DeluxeJournalMod.ColorDataPath}'."))
        {
        }

        protected override void Handle(IMonitor monitor, string command, string[] args)
        {
            if (DeluxeJournalMod.Instance is not DeluxeJournalMod mod || DeluxeJournalMod.Config is not Config config)
            {
                return;
            }

            string? path = null;

            try
            {
                if (args.Length > 0)
                {
                    path = $"{DeluxeJournalMod.ColorDataPath}/{args[0]}";
                }
                else if (!string.IsNullOrEmpty(config.TargetColorSchemaFile))
                {
                    path = $"{DeluxeJournalMod.ColorDataPath}/{config.TargetColorSchemaFile}";
                }

                if (path?.EndsWith(".json") == false)
                {
                    path += ".json";
                }

                path = mod.LoadColorSchemas(path, false);
                monitor.Log($"Loaded color data from '{path}'.", LogLevel.Info);
            }
            catch (ContentLoadException ex)
            {
                monitor.Log(path == null ? ex.Message : $"Could not load color data from '{path}'.", LogLevel.Error);
                return;
            }
        }
    }
}
