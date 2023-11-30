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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using BirbCore.Extensions;
using HarmonyLib;
using StardewModdingAPI;

namespace BirbCore.Attributes;

public class SContent : ClassHandler
{
    public string FileName;
    public bool IsList;
    public bool IsDictionary;

    public SContent(string fileName = "content.json", bool isList = false, bool isDictionary = false) : base(0)
    {
        this.FileName = fileName;
        this.IsList = isList;
        this.IsDictionary = isDictionary;
    }

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        // Get the type on ModEntry that stores all contents, and set it.
        // Should be a Dictionary<string, T> where T can be a list, dictionary, or Content object.
        Type modEntryValueType = type;
        if (this.IsList)
        {
            modEntryValueType = typeof(List<>).MakeGenericType(type);
        }
        else if (this.IsDictionary)
        {
            modEntryValueType = typeof(Dictionary<,>).MakeGenericType(typeof(string), type);
        }

        Type modEntryType = typeof(Dictionary<,>).MakeGenericType(typeof(string), modEntryValueType);

        MemberInfo modContent = mod.GetType().GetMemberOfType(modEntryType);
        if (modContent is null)
        {
            Log.Error("Mod must define a Content dictionary property");
            return;
        }

        IDictionary contentDictionary = (IDictionary)AccessTools.CreateInstance(modEntryType);
        if (contentDictionary is null)
        {
            Log.Error("contentDictionary was null.  Underlying type might be static? Cannot initialize.");
            return;
        }
        modContent.GetSetter()(mod, contentDictionary);


        MemberInfo? idMember = type.GetMemberWithCustomAttribute(typeof(ContentId));
        if (idMember is not null && idMember.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
        {
            // The provided ContentID is not serialized, so it's not provided to us.
            idMember = null;
        }

        // Iterate through all mods which provide content packs.
        // A content pack type will match the modEntryValueType, ie a list, dictionary, or content object
        foreach (IContentPack contentPack in mod.Helper.ContentPacks.GetOwned())
        {
            object? content = contentPack.GetType().GetMethod("ReadJsonFile")
                ?.MakeGenericMethod(modEntryValueType)
                .Invoke(contentPack, new object[] { this.FileName });

            if (content is null)
            {
                Log.Error($"{this.FileName} in content pack {contentPack.Manifest.UniqueID} was null");
                continue;
            }

            // Figure out a unique ID for each content within the content pack.
            // Will use key value for dictionary, array index for list, or empty string for content object.
            if (this.IsList)
            {
                IList contentList = (IList)content;
                for (int i = 0; i < contentList.Count; i++)
                {
                    string id = (string)(idMember?.GetGetter()(contentList[i]) ?? i);

                    base.Handle(type, contentList[i], mod, new object[] { contentPack, id });
                }
            }
            else if (this.IsDictionary)
            {
                foreach (DictionaryEntry entry in (IDictionary)content)
                {
                    string key = (string)entry.Key;
                    object? value = entry.Value;
                    string id = (string)(idMember?.GetGetter()(value) ?? key);
                    base.Handle(type, value, mod, new object[] { contentPack, id });
                }
            }
            else
            {
                string id = (string)(idMember?.GetGetter()(content) ?? "");

                base.Handle(type, content, mod, new object[] { contentPack, id });
            }

            string modId = contentPack.Manifest.UniqueID;

            contentDictionary.Add(modId, content);
        }
        return;
    }

    public class ModId : FieldHandler
    {
        public override void Handle(string name, Type fieldType, Func<object, object?> getter, Action<object, object> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (instance is null)
            {
                Log.Error("Content instance might be static? Failing to add all content packs");
                return;
            }
            if (args?[0] == null)
            {
                Log.Error("Something went wrong in BirbCore Content Pack parsing");
                return;
            }

            setter(instance, ((IContentPack)args[0]).Manifest.UniqueID);
        }
    }

    public class UniqueId : FieldHandler
    {
        public override void Handle(string name, Type fieldType, Func<object, object?> getter, Action<object, object> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (instance is null)
            {
                Log.Error("Content instance might be static? Failing to add all content packs");
                return;
            }
            if (args?[0] == null || args?[1] == null)
            {
                Log.Error("Something went wrong in BirbCore Content Pack parsing");
                return;
            }
            setter(instance, $"{((IContentPack)args[0]).Manifest.UniqueID}_{args[1]}");
        }
    }

    public class ContentId : FieldHandler
    {
        public override void Handle(string name, Type fieldType, Func<object, object?> getter, Action<object, object> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (instance is null)
            {
                Log.Error("Content instance might be static? Failing to add all content packs");
                return;
            }
            if (args?[1] == null)
            {
                Log.Error("Something went wrong in BirbCore Content Pack parsing");
                return;
            }
            setter(instance, args[1]);
        }
    }

    public class ContentPack : FieldHandler
    {
        public override void Handle(string name, Type fieldType, Func<object, object?> getter, Action<object, object> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (instance is null)
            {
                Log.Error("Content instance might be static? Failing to add all content packs");
                return;
            }
            if (args?[0] == null)
            {
                Log.Error("Something went wrong in BirbCore Content Pack parsing");
                return;
            }
            if (fieldType != typeof(IContentPack))
            {
                Log.Error("ContentPack attribute can only set value to field or property of type IContentPack");
                return;
            }
            setter(instance, args[0]);
        }
    }
}
