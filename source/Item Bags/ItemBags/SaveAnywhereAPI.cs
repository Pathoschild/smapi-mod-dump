/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBags
{
    /// <summary>
    /// This is the Mod API provided by the creator of Save Anywhere.<para/>
    /// https://www.nexusmods.com/stardewvalley/mods/444?tab=description
    /// </summary>
    public interface ISaveAnywhereAPI
    {
        /*********
        ** Events
        *********/
        /// <summary>
        ///     Event that fires before game save
        /// </summary>
        event EventHandler BeforeSave;
        /// <summary>
        ///     Event that fires after game save
        /// </summary>
        event EventHandler AfterSave;
        /// <summary>
        ///     Event that fires after game load
        /// </summary>
        event EventHandler AfterLoad;

        /// <summary>
        /// Add in an event that can trigger before saving begins.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="BeforeSave"></param>
        void addBeforeSaveEvent(string ID, Action BeforeSave);
        /// <summary>
        /// Remove an event that can trigger before saving begins.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="BeforeSave"></param>
        void removeBeforeSaveEvent(string ID, Action BeforeSave);
        /// <summary>
        /// Add an event that tiggers after saving has finished.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="AfterSave"></param>
        void addAfterSaveEvent(string ID, Action AfterSave);
        /// <summary>
        ///Remove an event that triggers after saving has occured.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="AfterSave"></param>
        void removeAfterSaveEvent(string ID, Action AfterSave);
        /// <summary>
        /// Add in an event that triggers afer loading has occured.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="AfterLoad"></param>
        void addAfterLoadEvent(string ID, Action AfterLoad);
        /// <summary>
        /// Remove an event that occurs after loading has occured.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="AfterLoad"></param>
        void removeAfterLoadEvent(string ID, Action AfterLoad);
    }
}
