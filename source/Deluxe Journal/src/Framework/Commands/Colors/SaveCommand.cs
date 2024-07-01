/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;

namespace DeluxeJournal.Framework.Commands.Colors
{
    internal class SaveCommand : ColorCommand
    {
        private const string DefaultSaveFileName = "saved.json";

        public SaveCommand() : base("dj_colors_save",
            BuildDocString("Save color data.",
                "dj_colors_save [filename]",
                $"filename: Optional file name. Defaults to '{DefaultSaveFileName}'."))
        {
        }

        protected override void Handle(IMonitor monitor, string command, string[] args)
        {
            if (DeluxeJournalMod.Instance is not DeluxeJournalMod mod)
            {
                return;
            }

            string saveFileName;
            string path;

            if (args.Length > 0)
            {
                saveFileName = args[0];

                if (!saveFileName.EndsWith(".json"))
                {
                    saveFileName += ".json";
                }
            }
            else
            {
                saveFileName = DefaultSaveFileName;
            }

            path = $"{DeluxeJournalMod.ColorDataPath}/{saveFileName}";
            mod.SaveColorSchemas(path);
            monitor.Log($"Saved color data to '{path}'.", LogLevel.Info);
        }
    }
}
