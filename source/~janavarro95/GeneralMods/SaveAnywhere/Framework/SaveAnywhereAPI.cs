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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.SaveAnywhere.Framework
{
    /// <summary>
    ///     Interface for the Save Anywhere API
    ///     Other mods can use this interface to get the
    ///         API from the SMAPI helper
    /// </summary>
    public class SaveAnywhereAPI
    {

        /*********
** Events
*********/
        /// <summary>
        ///     Event that fires before game save
        /// </summary>
        public event EventHandler BeforeSave
        {
            add
            {
                SaveAnywhere.Instance.SaveManager.beforeSave += value;
            }
            remove
            {
                SaveAnywhere.Instance.SaveManager.beforeSave -= value;
            }
        }
        /// <summary>
        ///     Event that fires after game save
        /// </summary>
        public event EventHandler AfterSave
        {
            add
            {
                SaveAnywhere.Instance.SaveManager.afterSave += value;
            }
            remove
            {
                SaveAnywhere.Instance.SaveManager.afterSave -= value;
            }
        }
        /// <summary>
        ///     Event that fires after game load
        /// </summary>
        public event EventHandler AfterLoad
        {
            add
            {
                SaveAnywhere.Instance.SaveManager.afterLoad += value;
            }
            remove
            {
                SaveAnywhere.Instance.SaveManager.afterLoad -= value;
            }
        }


        public SaveAnywhereAPI()
        {

        }

        /// <summary>
        /// Add in an event that can trigger before saving begins.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="BeforeSave"></param>
        public void addBeforeSaveEvent(string ID, Action BeforeSave)
        {
            SaveAnywhere.Instance.SaveManager.beforeCustomSavingBegins.Add(ID, BeforeSave);
        }
        /// <summary>
        /// Remove an event that can trigger before saving begins.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="BeforeSave"></param>
        public void removeBeforeSaveEvent(string ID, Action BeforeSave)
        {
            SaveAnywhere.Instance.SaveManager.beforeCustomSavingBegins.Remove(ID);
        }
        /// <summary>
        /// Add an event that tiggers after saving has finished.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="AfterSave"></param>
        public void addAfterSaveEvent(string ID, Action AfterSave)
        {
            SaveAnywhere.Instance.SaveManager.afterCustomSavingCompleted.Add(ID, AfterSave);
        }
        /// <summary>
        ///Remove an event that triggers after saving has occured.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="AfterSave"></param>
        public void removeAfterSaveEvent(string ID, Action AfterSave)
        {
            SaveAnywhere.Instance.SaveManager.afterCustomSavingCompleted.Remove(ID);
        }
        /// <summary>
        /// Add in an event that triggers afer loading has occured.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="AfterLoad"></param>
        public void addAfterLoadEvent(string ID, Action AfterLoad)
        {
            SaveAnywhere.Instance.SaveManager.afterSaveLoaded.Add(ID, AfterLoad);
        }
        /// <summary>
        /// Remove an event that occurs after loading has occured.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="AfterLoad"></param>
        public void removeAfterLoadEvent(string ID, Action AfterLoad)
        {
            SaveAnywhere.Instance.SaveManager.afterSaveLoaded.Remove(ID);
        }

    }
}
