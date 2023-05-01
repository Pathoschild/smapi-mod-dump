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
        public event EventHandler BeforeSave
        {
            add => SaveAnywhere.Instance.SaveManager.BeforeSave += value;
            remove => SaveAnywhere.Instance.SaveManager.BeforeSave -= value;
        }

        public event EventHandler AfterSave
        {
            add => SaveAnywhere.Instance.SaveManager.AfterSave += value;
            remove => SaveAnywhere.Instance.SaveManager.AfterSave -= value;
        }

        public event EventHandler AfterLoad
        {
            add => SaveAnywhere.Instance.SaveManager.AfterLoad += value;
            remove => SaveAnywhere.Instance.SaveManager.AfterLoad -= value;
        }

        public void addBeforeSaveEvent(string id, Action beforeSave)
        {
            SaveAnywhere.Instance.SaveManager.BeforeCustomSavingBegins.Add(id, beforeSave);
        }

        public void removeBeforeSaveEvent(string id, Action beforeSave)
        {
            SaveAnywhere.Instance.SaveManager.BeforeCustomSavingBegins.Remove(id);
        }

        public void addAfterSaveEvent(string id, Action afterSave)
        {
            SaveAnywhere.Instance.SaveManager.AfterCustomSavingCompleted.Add(id, afterSave);
        }

        public void removeAfterSaveEvent(string id, Action afterSave)
        {
            SaveAnywhere.Instance.SaveManager.AfterCustomSavingCompleted.Remove(id);
        }

        public void addAfterLoadEvent(string id, Action afterLoad)
        {
            SaveAnywhere.Instance.SaveManager.AfterSaveLoaded.Add(id, afterLoad);
        }

        public void removeAfterLoadEvent(string id, Action afterLoad)
        {
            SaveAnywhere.Instance.SaveManager.AfterSaveLoaded.Remove(id);
        }
    }
}