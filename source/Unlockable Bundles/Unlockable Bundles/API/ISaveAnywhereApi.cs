/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using System;

namespace SaveAnywhere.Framework
{
    public interface ISaveAnywhereApi
    {
        /// <summary>
        ///     Fires before save data starts writing
        /// </summary>
        public event EventHandler BeforeSave;

        /// <summary>
        ///     Fires after saving is complete
        /// </summary>
        public event EventHandler AfterSave;

        /// <summary>
        ///     Fires if the game has loaded with a mid-day save
        /// </summary>
        public event EventHandler AfterLoad;

        public void addAfterSaveEvent(string id, Action afterSave);
    }
}