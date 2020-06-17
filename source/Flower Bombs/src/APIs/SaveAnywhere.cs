using System;

namespace Omegasis.SaveAnywhere.Framework
{
    public interface ISaveAnywhereAPI
    {
        event EventHandler BeforeSave;
        event EventHandler AfterSave;
        event EventHandler AfterLoad;

        // void addBeforeSaveEvent(string ID, Action BeforeSave);
        // void removeBeforeSaveEvent(string ID, Action BeforeSave);
        // void addAfterSaveEvent(string ID, Action AfterSave);
        // void removeAfterSaveEvent(string ID, Action AfterSave);
        // void addAfterLoadEvent(string ID, Action AfterLoad);
        // void removeAfterLoadEvent(string ID, Action AfterLoad);
    }
}
