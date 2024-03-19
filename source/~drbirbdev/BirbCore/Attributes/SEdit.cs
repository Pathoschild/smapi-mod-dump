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
using BirbCore.Extensions;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Sickhead.Engine.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BirbCore.Attributes;

/// <summary>
/// A collection of Edits made to the content pipeline.  Similar functionality to Content Patcher, but in code, and with far fewer features.
/// </summary>
public class SEdit : ClassHandler
{

    public enum Frequency
    {
        Never,
        OnDayStart,
        OnLocationChange,
        OnTimeChange,
        OnTick
    }

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        base.Handle(type, instance, mod, args);

        mod.Helper.Events.Content.AssetRequested += (sender, e) =>
        {
            if (!e.Name.IsEquivalentTo($"Mods/{mod.ModManifest.UniqueID}/Strings"))
            {
                return;
            }

            e.Edit(apply =>
            {
                Dictionary<string, string> dict = new();
                foreach (Translation translation in mod.Helper.Translation.GetTranslations())
                {
                    dict[translation.Key] = translation.ToString();
                }
                apply.ReplaceWith(dict);
            }, AssetEditPriority.Early);
        };

        mod.Helper.Events.Content.LocaleChanged += (sender, e) =>
        {
            mod.Helper.GameContent.InvalidateCache($"Mods/{mod.ModManifest.UniqueID}/Strings");
        };
    }

    public abstract class BaseEdit(
        string target,
        string? condition = null,
        Frequency frequency = Frequency.Never,
        AssetEditPriority priority = AssetEditPriority.Default)
        : FieldHandler
    {
        private readonly string _target = target;
        private readonly string? _condition = condition;
        private readonly AssetEditPriority _priority = priority;
        protected IMod? Mod;
        private bool _isApplied;

        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (GameStateQuery.IsImmutablyFalse(this._condition))
            {
                Log.Error($"Condition {this._condition} will never be true, so edit {name} will never be applied.");
                return;
            }

            this.Mod = mod;

            switch (frequency)
            {
                case Frequency.OnDayStart: this.Mod.Helper.Events.GameLoop.DayStarted += this.InvalidateIfNeeded; break;
                case Frequency.OnLocationChange: this.Mod.Helper.Events.Player.Warped += this.InvalidateIfNeeded; break;
                case Frequency.OnTimeChange: this.Mod.Helper.Events.GameLoop.TimeChanged += this.InvalidateIfNeeded; break;
                case Frequency.OnTick: this.Mod.Helper.Events.GameLoop.UpdateTicked += this.InvalidateIfNeeded; break;
            }

            BaseEdit edit = this;

            this.Mod.Helper.Events.Content.AssetRequested += (sender, e) =>
            {
                if (!e.Name.IsEquivalentTo(edit._target))
                {
                    return;
                }
                if (!GameStateQuery.CheckConditions(edit._condition))
                {
                    return;
                }

                e.Edit(asset =>
                {
                    edit.DoEdit(asset, getter(instance), name, fieldType, instance);
                }, edit._priority/*, edit.Mod.ModManifest.UniqueID // Can't use this because of SMAPI limitation */);
            };
        }

        protected abstract void DoEdit(IAssetData asset, object? edit, string name, Type fieldType, object? instance);

        private void InvalidateIfNeeded(object? sender, object e)
        {
            if (this.Mod is null || this._isApplied == GameStateQuery.CheckConditions(this._condition))
            {
                return;
            }

            this._isApplied = !this._isApplied;
            this.Mod.Helper.GameContent.InvalidateCache(this._target);
        }
    }

    /// <summary>
    /// Change some data content.
    /// Target - the asset name to edit.
    /// Field - path to a field within the asset, similar to TargetField in Content Patcher.  Optional, default empty.
    ///     List items in the field path can follow several notations:
    ///     - numeric indices like '#0' for the first index
    ///     - '*' as a wildcard for all entries in the list
    ///     - an alphanumeric identifier to match an ID or Id field or property of the listed objects
    ///     Dictionary items in the field path can follow several notations:
    ///     - a key in the dictionary
    ///     - '*' as a wildcard for all entries in the dictionary
    /// Condition - a Game State Query for when to apply this change.  Optional, default always.
    /// Frequency - the frequency to recheck the condition to see if this asset should be invalidated.
    /// Priority - the priority with which to apply this change.  Optional, default 0 (normal priority).
    /// </summary>
    public class Data(
        string target,
        string[]? field = null,
        string? condition = null,
        Frequency frequency = Frequency.Never,
        AssetEditPriority priority = AssetEditPriority.Default)
        : BaseEdit(target, condition, frequency, priority)
    {
        protected override void DoEdit(IAssetData asset, object? edit, string name, Type fieldType, object? instance)
        {
            if (this.Mod is null)
            {
                return;
            }
            List<object> toEdit = [asset.Data];

            if (field is { Length: >= 1 })
            {
                foreach (string t in field)
                {
                    List<object> nextToEdit = [];

                    foreach (object toEditValue in toEdit)
                    {
                        switch (toEditValue)
                        {
                            case IList toEditValueList:
                                nextToEdit.AddRange(GetListEdits(t, toEditValueList));
                                break;
                            case IDictionary toEditValueDictionary:
                                nextToEdit.AddRange(GetDictionaryEdits(t, toEditValueDictionary));
                                break;
                            default:
                                nextToEdit.AddRange(GetMemberEdits(t, toEditValue));
                                break;
                        }
                    }

                    toEdit = nextToEdit;
                }
            }

            foreach (object toEditValue in toEdit)
            {
                switch (toEditValue)
                {
                    case IList toEditValueList:
                        ApplyListEdit(toEditValueList, edit);
                        break;
                    case IDictionary toEditValueDictionary:
                        this.ApplyDictionaryEdit(toEditValueDictionary, edit, name);
                        break;
                    default:
                        ApplyMemberEdit(toEditValue, edit);
                        break;
                }
            }
        }

        private static IEnumerable<object> GetListEdits(string field, IList toEdit)
        {
            List<object> nextToEdit = [];
            if (toEdit.Count <= 0)
            {
                return nextToEdit;
            }
            if (field == "*")
            {
                foreach (object item in toEdit)
                {
                    nextToEdit.Add(item);
                }
                return nextToEdit;
            }

            if (field.StartsWith("#"))
            {
                if (!int.TryParse(field[1..], out int index))
                {
                    Log.Error($"SEdit.Data could not parse field {field} because it expected a numeric index");
                    return nextToEdit;
                }
                if (index >= toEdit.Count)
                {
                    Log.Error($"SEdit.Data could not parse field {field} because the index was out of bounds");
                    return nextToEdit;
                }
                object? item = toEdit[index];
                if (item is null)
                {
                    Log.Error($"SEdit.Data could not parse field {field} because the value at the index was null");
                    return nextToEdit;
                }
                nextToEdit.Add(item);
            }
            else
            {
                Type? t = toEdit[0]?.GetType();
                if (t is null)
                {
                    Log.Error("SEdit.Data could not find type of index 0");
                    return nextToEdit;
                }
                if (!t.TryGetMemberOfName("Id", out MemberInfo id) && !t.TryGetMemberOfName("ID", out id))
                {
                    Log.Error("SEdit.Data could not find key field for list");
                    return nextToEdit;
                }

                foreach (object item in toEdit)
                {
                    if ((string)id.GetValue(item) != field)
                    {
                        continue;
                    }

                    nextToEdit.Add(item);
                    return nextToEdit;
                }
            }
            return nextToEdit;
        }

        private static IEnumerable<object> GetDictionaryEdits(string field, IDictionary toEdit)
        {
            if (field == "*")
            {
                List<object> edits = [];
                foreach (object toEditItem in toEdit.Values)
                {
                    edits.Add(toEditItem);
                }
                return edits;
            }

            if (!toEdit.Contains(field))
            {
                Log.Error($"SEdit.Data could not find dictionary key with value {field}");
                return [];
            }
            object? item = toEdit[field];
            if (item is not null)
            {
                return [item];
            }

            Log.Error($"SEdit.Data dictionary contained null value for {field}");
            return [];
        }

        private static IEnumerable<object> GetMemberEdits(string field, object toEdit)
        {
            if (!toEdit.GetType().TryGetMemberOfName(field, out MemberInfo memberInfo))
            {
                Log.Error($"SEdit.Data could not find field or property of name {field}");
                return [];
            }

            object? nextToEdit = memberInfo.GetValue(toEdit);
            if (nextToEdit is not null)
            {
                return [nextToEdit];
            }

            Log.Error($"SEdit.Data could not find field or property of name {field}");
            return [];
        }

        private static void ApplyListEdit(IList toEdit, object? edit)
        {
            Type? t = toEdit[0]?.GetType();
            if (t is null)
            {
                Log.Error("SEdit.Data could not find edits");
                return;
            }

            bool hasId = t.TryGetMemberOfName("Id", out MemberInfo id) || t.TryGetMemberOfName("ID", out id);

            if (edit is not IList editList)
            {
                if (!hasId)
                {
                    toEdit.Add(edit);
                    return;
                }
                for (int i = 0; i < toEdit.Count; i++)
                {
                    if (id.GetValue(toEdit[i]) != id.GetValue(edit))
                    {
                        continue;
                    }

                    toEdit[i] = edit;
                    return;
                }
                toEdit.Add(edit);
                return;
            }

            foreach (object editListItem in editList)
            {
                if (!hasId)
                {
                    toEdit.Add(editListItem);
                    return;
                }

                for (int i = 0; i < toEdit.Count; i++)
                {
                    if (id.GetValue(toEdit[i]) != id.GetValue(editListItem))
                    {
                        continue;
                    }

                    toEdit[i] = editListItem;
                    return;
                }
                toEdit.Add(editListItem);
            }
        }

        private void ApplyDictionaryEdit(IDictionary toEdit, object? edit, string name)
        {
            if (edit is not IDictionary editDictionary)
            {
                string key = $"{this.Mod?.ModManifest.UniqueID}_{name}";
                toEdit[key] = edit;
                return;
            }

            foreach (DictionaryEntry editDictionaryEntry in editDictionary)
            {
                toEdit[editDictionaryEntry.Key] = editDictionaryEntry.Value;
            }
        }

        private static void ApplyMemberEdit(object toEdit, object? edit)
        {
            edit.ShallowCloneTo(toEdit);
        }
    }

    /// <summary>
    /// Expects a relative path to a source image file as the string field value.
    /// </summary>
    public class Image(
        string target,
        PatchMode patchMode,
        string? condition = null,
        Frequency frequency = Frequency.Never,
        AssetEditPriority priority = AssetEditPriority.Default)
        : BaseEdit(target, condition, frequency, priority)
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (fieldType != typeof(string))
            {
                Log.Error($"SEdit.Image only works with string fields or properties, but was {fieldType}");
                return;
            }

            base.Handle(name, fieldType, getter, setter, instance, mod, args);
        }

        protected override void DoEdit(IAssetData asset, object? edit, string name, Type fieldType, object? instance)
        {
            if (edit is null || this.Mod is null)
            {
                return;
            }
            string filePath = (string)edit;
            IAssetDataForImage image = asset.AsImage();

            IRawTextureData source = this.Mod.Helper.ModContent.Load<IRawTextureData>(filePath);
            Rectangle? sourceRect = null;
            Rectangle? targetRect = null;

            if (fieldType.TryGetGetterOfName(name + "SourceArea", out Func<object?, object?> rectGetter))
            {
                sourceRect = (Rectangle?)rectGetter(instance);
            }
            if (fieldType.TryGetGetterOfName(name + "TargetArea", out rectGetter))
            {
                targetRect = (Rectangle?)rectGetter(instance);
            }

            image.PatchImage(source, sourceRect, targetRect, patchMode);
        }
    }

    public class Map(
        string target,
        PatchMapMode patchMode = PatchMapMode.Overlay,
        string? condition = null,
        Frequency frequency = Frequency.Never,
        AssetEditPriority priority = AssetEditPriority.Default)
        : BaseEdit(target, condition, frequency, priority)
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (fieldType != typeof(string))
            {
                Log.Error($"SEdit.Map only works with string fields or properties, but was {fieldType}");
                return;
            }

            base.Handle(name, fieldType, getter, setter, instance, mod, args);
        }

        protected override void DoEdit(IAssetData asset, object? edit, string name, Type fieldType, object? instance)
        {
            if (edit is null || this.Mod is null)
            {
                return;
            }
            string filePath = (string)edit;
            IAssetDataForMap map = asset.AsMap();

            xTile.Map source = this.Mod.Helper.ModContent.Load<xTile.Map>(filePath);

            Rectangle? sourceRect = null;
            Rectangle? targetRect = null;
            if (fieldType.TryGetGetterOfName(name + "SourceArea", out Func<object?, object?> rectGetter))
            {
                sourceRect = (Rectangle?)rectGetter(instance);
            }
            if (fieldType.TryGetGetterOfName(name + "TargetArea", out rectGetter))
            {
                targetRect = (Rectangle?)rectGetter(instance);
            }

            map.PatchMap(source, sourceRect, targetRect, patchMode);
        }
    }
}
