/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

namespace DeluxeJournal.Framework.Data
{
    internal class NotesData
    {
        /// <summary>Notes text per save file.</summary>
        public IDictionary<string, string> Text { get; set; } = new Dictionary<string, string>();
    }
}
