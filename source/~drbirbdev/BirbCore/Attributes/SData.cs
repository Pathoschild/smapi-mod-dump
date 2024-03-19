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
    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        if (!mod.GetType().TryGetMemberOfType(type, out MemberInfo modData))
        {
            Log.Error("Mod must define a data property");
            return;
        }

        modData.GetSetter()(mod, instance);
        base.Handle(type, instance, mod);
    }

    public class SaveData(string key) : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            mod.Helper.Events.GameLoop.SaveLoaded += (sender, e) =>
            {
                object? saveData = mod.Helper.Data.GetType().GetMethod("ReadSaveData")
                    ?.MakeGenericMethod(fieldType)
                    .Invoke(mod.Helper.Data, [key]);

                setter(instance, saveData);
            };

            mod.Helper.Events.GameLoop.SaveCreated += (sender, e) =>
            {
                object? saveData = AccessTools.CreateInstance(fieldType);

                setter(instance, saveData);
            };

            mod.Helper.Events.GameLoop.DayEnding += (sender, e) =>
            {
                object? saveData = getter(instance);

                mod.Helper.Data.WriteSaveData(key, saveData);
            };
        }
    }

    public class LocalData(string jsonFile) : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            object? localData = mod.Helper.Data.GetType().GetMethod("ReadJsonFile")
                ?.MakeGenericMethod(fieldType)
                .Invoke(mod.Helper.Data, [jsonFile]);

            localData ??= AccessTools.CreateInstance(fieldType);

            setter(instance, localData);

            mod.Helper.Events.GameLoop.DayEnding += (sender, e) =>
            {
                object? data = getter(instance);

                mod.Helper.Data.WriteJsonFile(jsonFile, data);
            };
        }
    }

    public class GlobalData(string key) : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            object? globalData = mod.Helper.Data.GetType().GetMethod("ReadGlobalData")
                ?.MakeGenericMethod(fieldType)
                .Invoke(mod.Helper.Data, [key]);

            globalData ??= AccessTools.CreateInstance(fieldType);

            setter(instance, globalData);

            mod.Helper.Events.GameLoop.DayEnding += (sender, e) =>
            {
                object? data = getter(instance);

                mod.Helper.Data.WriteGlobalData(key, data);
            };
        }
    }

}
