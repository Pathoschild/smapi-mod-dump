/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RealSweetPanda/SaveAnywhereRedux
**
*************************************************/

using System;

namespace SaveAnywhere.Framework
{
    public class SaveAnywhereApi
    {
        /// <summary>
        ///     Fires before save data starts writing
        /// </summary>
        public event EventHandler BeforeSave
        {
            add => SaveAnywhere.Instance.SaveManager.BeforeSave += value;
            remove => SaveAnywhere.Instance.SaveManager.BeforeSave -= value;
        }

        /// <summary>
        ///     Fires after saving is complete
        /// </summary>
        public event EventHandler AfterSave
        {
            add => SaveAnywhere.Instance.SaveManager.AfterSave += value;
            remove => SaveAnywhere.Instance.SaveManager.AfterSave -= value;
        }

        /// <summary>
        ///     Fires if the game has loaded with a mid-day save
        /// </summary>
        public event EventHandler AfterLoad
        {
            add => SaveAnywhere.Instance.SaveManager.AfterLoad += value;
            remove => SaveAnywhere.Instance.SaveManager.AfterLoad -= value;
        }

        /// <summary>
        ///     Add a function to be called before save data starts writing
        /// </summary>
        /// <param name="id">Any unique identifier</param>
        /// <param name="beforeSave">The function to be called</param>
        public void addBeforeSaveEvent(string id, Action beforeSave)
        {
            SaveAnywhere.Instance.SaveManager.BeforeCustomSavingBegins.Add(id, beforeSave);
        }

        /// <summary>
        ///     Remove a function from the list of functions to be called before save data starts writing
        /// </summary>
        /// <param name="id">Any unique identifier</param>
        /// <param name="beforeSave">Not necessary</param>
        public void removeBeforeSaveEvent(string id, Action beforeSave)
        {
            SaveAnywhere.Instance.SaveManager.BeforeCustomSavingBegins.Remove(id);
        }

        /// <summary>
        ///     Add a function to be called after saving has completed
        /// </summary>
        /// <param name="id">Any unique identifier</param>
        /// <param name="afterSave">The function to be called</param>
        public void addAfterSaveEvent(string id, Action afterSave)
        {
            SaveAnywhere.Instance.SaveManager.AfterCustomSavingCompleted.Add(id, afterSave);
        }

        /// <summary>
        ///     Remove a function from the list of functions to be called after saving has completed
        /// </summary>
        /// <param name="id">Any unique identifier</param>
        /// <param name="afterSave">Not necessary</param>
        public void removeAfterSaveEvent(string id, Action afterSave)
        {
            SaveAnywhere.Instance.SaveManager.AfterCustomSavingCompleted.Remove(id);
        }

        /// <summary>
        ///     Add a function to be called if the player loads a save with mid-day save data
        /// </summary>
        /// <param name="id">Any unique identifier</param>
        /// <param name="afterLoad">The function to be called</param>
        public void addAfterLoadEvent(string id, Action afterLoad)
        {
            SaveAnywhere.Instance.SaveManager.AfterSaveLoaded.Add(id, afterLoad);
        }

        /// <summary>
        ///     Remove a function from the list of functions to be called if the player loads a save with mid-day save data
        /// </summary>
        /// <param name="id">Any unique identifier</param>
        /// <param name="afterLoad">Not necessary</param>
        public void removeAfterLoadEvent(string id, Action afterLoad)
        {
            SaveAnywhere.Instance.SaveManager.AfterSaveLoaded.Remove(id);
        }
    }
}