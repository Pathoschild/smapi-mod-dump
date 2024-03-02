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
        OnTick,
    }

    public SEdit() : base(0)
    {

    }

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        instance = Activator.CreateInstance(type);
        base.Handle(type, instance, mod, args);

        mod.Helper.Events.Content.AssetRequested += (object? sender, AssetRequestedEventArgs e) =>
        {
            if (!e.Name.IsEquivalentTo($"Mods/{mod.ModManifest.UniqueID}/Strings"))
            {
                return;
            }

            e.Edit(apply =>
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (Translation translation in mod.Helper.Translation.GetTranslations())
                {
                    dict[translation.Key] = translation.ToString();
                }
                apply.ReplaceWith(dict);
            }, AssetEditPriority.Early);
        };

        mod.Helper.Events.Content.LocaleChanged += (object? sender, LocaleChangedEventArgs e) =>
        {
            mod.Helper.GameContent.InvalidateCache($"Mods/{mod.ModManifest.UniqueID}/Strings");
        };

        return;
    }

    public abstract class BaseEdit : FieldHandler
    {
        public string Target;
        public string? Condition;
        public Frequency Frequency;
        public AssetEditPriority Priority;
        protected IMod? Mod;
        private bool IsApplied = false;

        protected BaseEdit(string target, string? condition = null, Frequency frequency = Frequency.Never, AssetEditPriority priority = AssetEditPriority.Default)
        {
            this.Target = target;
            this.Condition = condition;
            this.Frequency = frequency;
            this.Priority = priority;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (GameStateQuery.IsImmutablyFalse(this.Condition))
            {
                Log.Error($"Condition {this.Condition} will never be true, so edit {name} will never be applied.");
                return;
            }

            this.Mod = mod;

            switch (this.Frequency)
            {
                case Frequency.OnDayStart: this.Mod.Helper.Events.GameLoop.DayStarted += this.InvalidateIfNeeded; break;
                case Frequency.OnLocationChange: this.Mod.Helper.Events.Player.Warped += this.InvalidateIfNeeded; break;
                case Frequency.OnTimeChange: this.Mod.Helper.Events.GameLoop.TimeChanged += this.InvalidateIfNeeded; break;
                case Frequency.OnTick: this.Mod.Helper.Events.GameLoop.UpdateTicked += this.InvalidateIfNeeded; break;
                default: break;
            }

            BaseEdit edit = this;

            this.Mod.Helper.Events.Content.AssetRequested += (object? sender, AssetRequestedEventArgs e) =>
            {
                if (!e.Name.IsEquivalentTo(edit.Target))
                {
                    return;
                }
                if (!GameStateQuery.CheckConditions(edit.Condition))
                {
                    return;
                }

                e.Edit(asset =>
                {
                    edit.DoEdit(asset, getter(instance), name, fieldType, instance);
                }, edit.Priority/*, edit.Mod.ModManifest.UniqueID // Can't use this because of SMAPI limitation */);
            };
        }

        public abstract void DoEdit(IAssetData asset, object? edit, string name, Type fieldType, object? instance);

        public void InvalidateIfNeeded(object? sender, object e)
        {
            if (this.Mod is not null && this.IsApplied != GameStateQuery.CheckConditions(this.Condition))
            {
                this.IsApplied = !this.IsApplied;
                this.Mod.Helper.GameContent.InvalidateCache(this.Target);
            }
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
    public class Data : BaseEdit
    {
        public string[]? Field;

        public Data(string target, string[]? field = null, string? condition = null, Frequency frequency = Frequency.Never, AssetEditPriority priority = AssetEditPriority.Default) : base(target, condition, frequency, priority)
        {
            this.Field = field;
        }

        public override void DoEdit(IAssetData asset, object? edit, string name, Type fieldType, object? instance)
        {
            if (this.Mod is null)
            {
                return;
            }
            List<object> toEdit = new()
            {
                asset.Data
            };

            if (this.Field != null && this.Field.Length >= 1)
            {
                for (int i = 0; i < this.Field.Length; i++)
                {
                    List<object> nextToEdit = new();
                    string field = this.Field[i];

                    foreach (object toEditValue in toEdit)
                    {
                        if (toEditValue is IList toEditValueList)
                        {
                            nextToEdit.AddRange(GetListEdits(field, toEditValueList));
                        }
                        else if (toEditValue is IDictionary toEditValueDictionary)
                        {
                            nextToEdit.AddRange(GetDictionaryEdits(field, toEditValueDictionary));
                        }
                        else
                        {
                            nextToEdit.AddRange(GetMemberEdits(field, toEditValue));
                        }
                    }

                    toEdit = nextToEdit;
                }
            }

            foreach (object toEditValue in toEdit)
            {
                if (toEditValue is IList toEditValueList)
                {
                    this.ApplyListEdit(toEditValueList, edit);
                }
                else if (toEditValue is IDictionary toEditValueDictionary)
                {
                    this.ApplyDictionaryEdit(toEditValueDictionary, edit, name);
                }
                else
                {
                    ApplyMemberEdit(toEditValue, edit);
                }
            }
        }

        private static List<object> GetListEdits(string field, IList toEdit)
        {
            List<object> nextToEdit = new List<object>();
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
            else if (field.StartsWith("#"))
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
                MemberInfo? id = toEdit[0]?.GetType().GetMemberOfName("Id") ?? toEdit[0]?.GetType().GetMemberOfName("ID");
                if (id is null)
                {
                    Log.Error($"SEdit.Data could not find key field for list");
                    return nextToEdit;
                }

                foreach (object item in toEdit)
                {
                    if ((string)id.GetValue(item) == field)
                    {
                        nextToEdit.Add(item);
                        return nextToEdit;
                    }
                }
            }
            return nextToEdit;
        }

        private static List<object> GetDictionaryEdits(string field, IDictionary toEdit)
        {
            if (field == "*")
            {
                List<object> edits = new List<object>();
                foreach (object toEditItem in toEdit.Values)
                {
                    edits.Add(toEditItem);
                }
                return edits;
            }

            if (!toEdit.Contains(field))
            {
                Log.Error($"SEdit.Data could not find dictionary key with value {field}");
                return new List<object>();
            }
            object? item = toEdit[field];
            if (item is null)
            {
                Log.Error($"SEdit.Data dictionary contained null value for {field}");
                return new List<object>();
            }
            return new List<object> { item };
        }

        private static List<object> GetMemberEdits(string field, object toEdit)
        {
            object? nextToEdit = toEdit.GetType().GetMemberOfName(field)?.GetValue(toEdit);
            if (nextToEdit is null)
            {
                Log.Error($"SEdit.Data could not find field or property of name {field}");
                return new List<object>();
            }
            return new List<object> { nextToEdit };
        }

        private void ApplyListEdit(IList toEdit, object? edit)
        {
            MemberInfo? id = toEdit[0]?.GetType().GetMemberOfName("Id") ?? toEdit[0]?.GetType().GetMemberOfName("ID");

            if (edit is not IList editList)
            {
                if (id is null)
                {
                    toEdit.Add(edit);
                    return;
                }
                for (int i = 0; i < toEdit.Count; i++)
                {
                    if (id.GetValue(toEdit[i]) == id.GetValue(edit))
                    {
                        toEdit[i] = edit;
                        return;
                    }
                }
                toEdit.Add(edit);
                return;
            }

            foreach (object editListItem in editList)
            {
                if (id is null)
                {
                    toEdit.Add(editListItem);
                    return;
                }

                for (int i = 0; i < toEdit.Count; i++)
                {
                    if (id.GetValue(toEdit[i]) == id.GetValue(editListItem))
                    {
                        toEdit[i] = editListItem;
                        return;
                    }
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
    public class Image : BaseEdit
    {
        public PatchMode PatchMode;

        public Image(string target, PatchMode patchMode, string? condition = null, Frequency frequency = Frequency.Never, AssetEditPriority priority = AssetEditPriority.Default) : base(target, condition, frequency, priority)
        {
            this.PatchMode = patchMode;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (fieldType != typeof(string))
            {
                Log.Error($"SEdit.Image only works with string fields or properties, but was {fieldType}");
                return;
            }

            base.Handle(name, fieldType, getter, setter, instance, mod, args);
        }

        public override void DoEdit(IAssetData asset, object? edit, string name, Type fieldType, object? instance)
        {
            if (edit is null || this.Mod is null)
            {
                return;
            }
            string filePath = (string)edit;
            IAssetDataForImage image = asset.AsImage();

            IRawTextureData source = this.Mod.Helper.ModContent.Load<IRawTextureData>(filePath);
            if (source is null)
            {
                return;
            }
            Rectangle? sourceRect = null;
            Rectangle? targetRect = null;

            Func<object?, object?>? rectGetter = fieldType.GetMemberOfName(name + "SourceArea")?.GetGetter();
            if (rectGetter is not null)
            {
                sourceRect = (Rectangle?)rectGetter(instance);
            }
            rectGetter = instance?.GetType().GetMemberOfName(name + "TargetArea")?.GetGetter();
            if (rectGetter is not null)
            {
                targetRect = (Rectangle?)rectGetter(instance);
            }

            image.PatchImage(source, sourceRect, targetRect, this.PatchMode);
        }
    }

    public class Map : BaseEdit
    {
        public PatchMapMode PatchMode;

        public Map(string target, PatchMapMode patchMode = PatchMapMode.Overlay, string? condition = null, Frequency frequency = Frequency.Never, AssetEditPriority priority = AssetEditPriority.Default) : base(target, condition, frequency, priority)
        {
            this.PatchMode = patchMode;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (fieldType != typeof(string))
            {
                Log.Error($"SEdit.Map only works with string fields or properties, but was {fieldType}");
                return;
            }

            base.Handle(name, fieldType, getter, setter, instance, mod, args);
        }

        public override void DoEdit(IAssetData asset, object? edit, string name, Type fieldType, object? instance)
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
            Func<object?, object?>? rectGetter = fieldType.GetMemberOfName(name + "SourceArea")?.GetGetter();
            if (rectGetter is not null)
            {
                sourceRect = (Rectangle?)rectGetter(instance);
            }
            rectGetter = instance?.GetType().GetMemberOfName(name + "TargetArea")?.GetGetter();
            if (rectGetter is not null)
            {
                targetRect = (Rectangle?)rectGetter(instance);
            }

            map.PatchMap(source, sourceRect, targetRect, this.PatchMode);
        }
    }
}
