/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

#nullable enable
using System;
using System.Reflection;
using BirbCore.Extensions;
using HarmonyLib;
using StardewModdingAPI;

namespace BirbCore.Attributes;

public class SData : ClassHandler
{
    public SData() : base(0)
    {

    }

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        MemberInfo modData = mod.GetType().GetMemberOfType(type);
        if (modData == null)
        {
            Log.Error("Mod must define a data property");
            return;
        }

        instance = Activator.CreateInstance(type);
        modData.GetSetter()(mod, instance);
        base.Handle(type, instance, mod);
        return;
    }

    public class SaveData : FieldHandler
    {
        public string Key;

        public SaveData(string key)
        {
            this.Key = key;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            mod.Helper.Events.GameLoop.SaveLoaded += (object? sender, StardewModdingAPI.Events.SaveLoadedEventArgs e) =>
            {
                object? saveData = mod.Helper.Data.GetType().GetMethod("ReadSaveData")
                    ?.MakeGenericMethod(fieldType)
                    .Invoke(mod.Helper.Data, new object[] { this.Key });

                setter(instance, saveData);
            };

            mod.Helper.Events.GameLoop.SaveCreated += (object? sender, StardewModdingAPI.Events.SaveCreatedEventArgs e) =>
            {
                object? saveData = AccessTools.CreateInstance(fieldType);

                setter(instance, saveData);
            };

            mod.Helper.Events.GameLoop.DayEnding += (object? sender, StardewModdingAPI.Events.DayEndingEventArgs e) =>
            {
                object? saveData = getter(instance);

                mod.Helper.Data.WriteSaveData(this.Key, saveData);
            };
        }
    }

    public class LocalData : FieldHandler
    {
        public string JsonFile;

        public LocalData(string jsonFile)
        {
            this.JsonFile = jsonFile;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            object? localData = mod.Helper.Data.GetType().GetMethod("ReadJsonFile")
                ?.MakeGenericMethod(fieldType)
                .Invoke(mod.Helper.Data, new object[] { this.JsonFile });

            localData ??= AccessTools.CreateInstance(fieldType);

            setter(instance, localData);

            mod.Helper.Events.GameLoop.DayEnding += (object? sender, StardewModdingAPI.Events.DayEndingEventArgs e) =>
            {
                object? localData = getter(instance);

                mod.Helper.Data.WriteJsonFile(this.JsonFile, localData);
            };
        }
    }

    public class GlobalData : FieldHandler
    {
        public string Key;

        public GlobalData(string key)
        {
            this.Key = key;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            object? globalData = mod.Helper.Data.GetType().GetMethod("ReadGlobalData")
                ?.MakeGenericMethod(fieldType)
                .Invoke(mod.Helper.Data, new object[] { this.Key });

            globalData ??= AccessTools.CreateInstance(fieldType);

            setter(instance, globalData);

            mod.Helper.Events.GameLoop.DayEnding += (object? sender, StardewModdingAPI.Events.DayEndingEventArgs e) =>
            {
                object? globalData = getter(instance);

                mod.Helper.Data.WriteGlobalData(this.Key, globalData);
            };
        }
    }

}
