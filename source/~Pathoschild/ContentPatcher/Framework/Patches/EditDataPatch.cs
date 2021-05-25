/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ContentPatcher.Framework.Conditions;
using ContentPatcher.Framework.ConfigModels;
using ContentPatcher.Framework.Constants;
using ContentPatcher.Framework.Tokens;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using xTile;

namespace ContentPatcher.Framework.Patches
{
    /// <summary>Metadata for data to edit into a data file.</summary>
    internal class EditDataPatch : Patch
    {
        /*********
        ** Fields
        *********/
        /// <summary>Encapsulates monitoring and logging.</summary>
        private readonly IMonitor Monitor;

        /// <summary>Simplifies dynamic access to game code.</summary>
        private readonly IReflectionHelper Reflection;

        /// <summary>The data records to edit.</summary>
        private EditDataPatchRecord[] Records;

        /// <summary>The data fields to edit.</summary>
        private EditDataPatchField[] Fields;

        /// <summary>The records to reorder, if the target is a list asset.</summary>
        private EditDataPatchMoveRecord[] MoveRecords;

        /// <summary>The text operations to apply to existing values.</summary>
        private readonly TextOperation[] TextOperations;

        /// <summary>Parse the data change fields for an <see cref="PatchType.EditData"/> patch.</summary>
        private readonly TryParseFieldsDelegate TryParseFields;

        /// <summary>Whether the patch already tried loading the <see cref="Patch.FromAsset"/> asset for the current context. This doesn't necessarily means it succeeded (e.g. the file may not have existed).</summary>
        private bool AttemptedDataLoad;


        /*********
        ** Accessors
        *********/
        /// <summary>Parse the data change fields for an <see cref="PatchType.EditData"/> patch.</summary>
        /// <param name="context">The tokens available for this content pack.</param>
        /// <param name="entry">The change to load.</param>
        /// <param name="entries">The parsed data entry changes.</param>
        /// <param name="fields">The parsed data field changes.</param>
        /// <param name="moveEntries">The parsed move entry records.</param>
        /// <param name="error">The error message indicating why parsing failed, if applicable.</param>
        /// <returns>Returns whether parsing succeeded.</returns>
        public delegate bool TryParseFieldsDelegate(IContext context, PatchConfig entry, out List<EditDataPatchRecord> entries, out List<EditDataPatchField> fields, out List<EditDataPatchMoveRecord> moveEntries, out string error);


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="indexPath">The path of indexes from the root <c>content.json</c> to this patch; see <see cref="IPatch.IndexPath"/>.</param>
        /// <param name="path">The path to the patch from the root content file.</param>
        /// <param name="assetName">The normalized asset name to intercept.</param>
        /// <param name="conditions">The conditions which determine whether this patch should be applied.</param>
        /// <param name="fromFile">The normalized asset key from which to load entries (if applicable), including tokens.</param>
        /// <param name="records">The data records to edit.</param>
        /// <param name="fields">The data fields to edit.</param>
        /// <param name="moveRecords">The records to reorder, if the target is a list asset.</param>
        /// <param name="textOperations">The text operations to apply to existing values.</param>
        /// <param name="updateRate">When the patch should be updated.</param>
        /// <param name="contentPack">The content pack which requested the patch.</param>
        /// <param name="parentPatch">The parent patch for which this patch was loaded, if any.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="reflection">Simplifies dynamic access to game code.</param>
        /// <param name="normalizeAssetName">Normalize an asset name.</param>
        /// <param name="tryParseFields">Parse the data change fields for an <see cref="PatchType.EditData"/> patch.</param>
        public EditDataPatch(int[] indexPath, LogPathBuilder path, IManagedTokenString assetName, IEnumerable<Condition> conditions, IManagedTokenString fromFile, IEnumerable<EditDataPatchRecord> records, IEnumerable<EditDataPatchField> fields, IEnumerable<EditDataPatchMoveRecord> moveRecords, IEnumerable<TextOperation> textOperations, UpdateRate updateRate, IContentPack contentPack, IPatch parentPatch, IMonitor monitor, IReflectionHelper reflection, Func<string, string> normalizeAssetName, TryParseFieldsDelegate tryParseFields)
            : base(
                indexPath: indexPath,
                path: path,
                type: PatchType.EditData,
                assetName: assetName,
                conditions: conditions,
                updateRate: updateRate,
                contentPack: contentPack,
                parentPatch: parentPatch,
                normalizeAssetName: normalizeAssetName,
                fromAsset: fromFile
            )
        {
            // set fields
            this.Records = records?.ToArray();
            this.Fields = fields?.ToArray();
            this.MoveRecords = moveRecords?.ToArray();
            this.TextOperations = textOperations?.ToArray() ?? new TextOperation[0];
            this.Monitor = monitor;
            this.Reflection = reflection;
            this.TryParseFields = tryParseFields;

            // track contextuals
            this.Contextuals
                .Add(this.Records)
                .Add(this.Fields)
                .Add(this.MoveRecords)
                .Add(this.TextOperations)
                .Add(this.Conditions);
        }

        /// <inheritdoc />
        public override bool UpdateContext(IContext context)
        {
            // skip: don't need to handle a data file
            if (this.RawFromAsset == null)
                return base.UpdateContext(context);

            // skip: file already loaded and target didn't change
            if (!this.ManagedRawTargetAsset.UpdateContext(context) && this.AttemptedDataLoad)
                return base.UpdateContext(context);

            // reload non-data changes
            this.Contextuals
                .Remove(this.Records)
                .Remove(this.Fields)
                .Remove(this.MoveRecords);
            base.UpdateContext(context);

            // reload data
            this.Records = new EditDataPatchRecord[0];
            this.Fields = new EditDataPatchField[0];
            this.MoveRecords = new EditDataPatchMoveRecord[0];
            if (this.IsReady)
            {
                if (this.TryLoadFile(this.RawFromAsset, context, out List<EditDataPatchRecord> records, out List<EditDataPatchField> fields, out List<EditDataPatchMoveRecord> moveEntries, out string error))
                {
                    this.Records = records.ToArray();
                    this.Fields = fields.ToArray();
                    this.MoveRecords = moveEntries.ToArray();
                }
                else
                    this.Monitor.Log($"Can't load \"{this.Path}\" fields from file '{this.RawFromAsset}': {error}.", LogLevel.Warn);

                this.AttemptedDataLoad = true;
            }

            // update context
            this.Contextuals
                .Add(this.Records)
                .Add(this.Fields)
                .Add(this.MoveRecords)
                .UpdateContext(context);
            this.IsReady = this.IsReady && this.Contextuals.IsReady;

            return true;
        }

        /// <inheritdoc />
        public override void Edit<T>(IAssetData asset)
        {
            // throw on invalid type
            if (typeof(Texture2D).IsAssignableFrom(typeof(T)) || typeof(Map).IsAssignableFrom(typeof(T)))
            {
                this.Monitor.Log($"Can't apply data patch \"{this.Path}\" to {this.TargetAsset}: this file isn't a data file (found {(typeof(Texture2D).IsAssignableFrom(typeof(T)) ? "image" : typeof(T).Name)}).", LogLevel.Warn);
                return;
            }

            // handle dictionary types
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                // get dictionary's key/value types
                Type[] genericArgs = typeof(T).GetGenericArguments();
                if (genericArgs.Length != 2)
                    throw new InvalidOperationException("Can't parse the asset's dictionary key/value types.");
                Type keyType = typeof(T).GetGenericArguments().FirstOrDefault();
                Type valueType = typeof(T).GetGenericArguments().LastOrDefault();
                if (keyType == null)
                    throw new InvalidOperationException("Can't parse the asset's dictionary key type.");
                if (valueType == null)
                    throw new InvalidOperationException("Can't parse the asset's dictionary value type.");

                // get underlying apply method
                MethodInfo method = this.GetType().GetMethod(nameof(this.ApplyDictionary), BindingFlags.Instance | BindingFlags.NonPublic);
                if (method == null)
                    throw new InvalidOperationException($"Can't fetch the internal {nameof(this.ApplyDictionary)} method.");

                // invoke method
                method
                    .MakeGenericMethod(keyType, valueType)
                    .Invoke(this, new object[] { asset });
            }

            // handle list types
            else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                // get list's value type
                Type keyType = typeof(T).GetGenericArguments().FirstOrDefault();
                if (keyType == null)
                    throw new InvalidOperationException("Can't parse the asset's list value type.");

                // get underlying apply method
                MethodInfo method = this.GetType().GetMethod(nameof(this.ApplyList), BindingFlags.Instance | BindingFlags.NonPublic);
                if (method == null)
                    throw new InvalidOperationException($"Can't fetch the internal {nameof(this.ApplyList)} method.");

                // invoke method
                method
                    .MakeGenericMethod(keyType)
                    .Invoke(this, new object[] { asset });
            }

            // unknown type
            else
                throw new NotSupportedException($"Unknown data asset type {typeof(T).FullName}, expected dictionary or list.");
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetChangeLabels()
        {
            if (this.Records?.Any(p => p.Value?.Value == null) == true)
                yield return "deleted entries";

            if (this.Fields?.Any() == true || this.Records?.Any(p => p.Value?.Value != null) == true)
                yield return "changed entries";

            if (this.MoveRecords?.Any() == true)
                yield return "reordered entries";

            if (this.TextOperations.Any())
                yield return "applied text operations";
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Parse the data change fields for an <see cref="PatchType.EditData"/> patch.</summary>
        /// <param name="fromFile">The normalized asset key from which to load entries (if applicable), including tokens.</param>
        /// <param name="context">The tokens available for this content pack.</param>
        /// <param name="entries">The parsed data entry changes.</param>
        /// <param name="fields">The parsed data field changes.</param>
        /// <param name="moveEntries">The parsed move entry records.</param>
        /// <param name="error">The error message indicating why parsing failed, if applicable.</param>
        /// <returns>Returns whether parsing succeeded.</returns>
        private bool TryLoadFile(ITokenString fromFile, IContext context, out List<EditDataPatchRecord> entries, out List<EditDataPatchField> fields, out List<EditDataPatchMoveRecord> moveEntries, out string error)
        {
            if (fromFile.IsMutable && !fromFile.IsReady)
            {
                error = $"the {nameof(fromFile)} contains tokens which aren't available yet"; // this shouldn't happen, since the patch should check before calling this method
                entries = null;
                fields = null;
                moveEntries = null;
                return false;
            }

            // validate path
            if (!this.ContentPack.HasFile(fromFile.Value))
            {
                error = "that file doesn't exist in the content pack";
                entries = null;
                fields = null;
                moveEntries = null;
                return false;
            }

            // load JSON file
            PatchConfig model;
            try
            {
                model = this.ContentPack.ReadJsonFile<PatchConfig>(fromFile.Value);
            }
            catch (JsonException ex)
            {
                error = $"could not parse that file: {ex}";
                entries = null;
                fields = null;
                moveEntries = null;
                return false;
            }

            // parse fields
            return this.TryParseFields(context, model, out entries, out fields, out moveEntries, out error);
        }

        /// <summary>Apply the patch to a dictionary asset.</summary>
        /// <typeparam name="TKey">The dictionary key type.</typeparam>
        /// <typeparam name="TValue">The dictionary value type.</typeparam>
        /// <param name="asset">The asset to edit.</param>
        private void ApplyDictionary<TKey, TValue>(IAssetData asset)
        {
            // get data
            IDictionary<TKey, TValue> data = asset.AsDictionary<TKey, TValue>().Data;

            // apply field/record edits
            this.ApplyCollection<TKey, TValue>(
                asset,
                hasEntry: key => data.ContainsKey(key),
                getEntry: key => data[key],
                setEntry: (key, value) => data[key] = value,
                removeEntry: key => data.Remove(key)
            );

            // apply moves
            if (this.MoveRecords.Any())
                this.Monitor.LogOnce($"Can't move records for \"{this.Path}\" > {nameof(PatchConfig.MoveEntries)}: target asset '{this.TargetAsset}' isn't an ordered list).", LogLevel.Warn);
        }

        /// <summary>Apply the patch to a list asset.</summary>
        /// <typeparam name="TValue">The list value type.</typeparam>
        /// <param name="asset">The asset to edit.</param>
        private void ApplyList<TValue>(IAssetData asset)
        {
            // get data
            IList<TValue> data = asset.GetData<List<TValue>>();
            TValue GetByKey(string key) => data.FirstOrDefault(p => this.GetKey(p) == key);

            // apply field/record edits
            this.ApplyCollection<string, TValue>(
                asset,
                hasEntry: key => GetByKey(key) != null,
                getEntry: key => GetByKey(key),
                setEntry: (key, value) =>
                {
                    TValue match = GetByKey(key);
                    if (match != null)
                    {
                        int index = data.IndexOf(match);
                        data.RemoveAt(index);
                        data.Insert(index, value);
                    }
                    else
                        data.Add(value);
                },
                removeEntry: key =>
                {
                    TValue match = GetByKey(key);
                    if (match != null)
                    {
                        int index = data.IndexOf(match);
                        data.RemoveAt(index);
                    }
                }
            );

            // apply moves
            foreach (EditDataPatchMoveRecord moveRecord in this.MoveRecords)
            {
                if (!moveRecord.IsReady)
                    continue;
                string errorLabel = $"record \"{this.Path}\" > {nameof(PatchConfig.MoveEntries)} > \"{moveRecord.ID}\"";

                // get entry
                TValue entry = GetByKey(moveRecord.ID.Value);
                if (entry == null)
                {
                    this.Monitor.LogOnce($"Can't move {errorLabel}: no entry with that ID exists.", LogLevel.Warn);
                    continue;
                }
                int fromIndex = data.IndexOf(entry);

                // move to position
                if (moveRecord.ToPosition == MoveEntryPosition.Top)
                {
                    data.RemoveAt(fromIndex);
                    data.Insert(0, entry);
                }
                else if (moveRecord.ToPosition == MoveEntryPosition.Bottom)
                {
                    data.RemoveAt(fromIndex);
                    data.Add(entry);
                }
                else if (moveRecord.AfterID.IsMeaningful() || moveRecord.BeforeID.IsMeaningful())
                {
                    // get config
                    bool isAfterID = moveRecord.AfterID.IsMeaningful();
                    string anchorID = isAfterID ? moveRecord.AfterID.Value : moveRecord.BeforeID.Value;
                    errorLabel += $" {(isAfterID ? nameof(PatchMoveEntryConfig.AfterID) : nameof(PatchMoveEntryConfig.BeforeID))} \"{anchorID}\"";

                    // get anchor entry
                    TValue anchorEntry = GetByKey(anchorID);
                    if (anchorEntry == null)
                    {
                        this.Monitor.LogOnce($"Can't move {errorLabel}: no entry with the relative ID exists.", LogLevel.Warn);
                        continue;
                    }
                    if (object.ReferenceEquals(entry, anchorEntry))
                    {
                        this.Monitor.LogOnce($"Can't move {errorLabel}: can't move entry relative to itself.", LogLevel.Warn);
                        continue;
                    }

                    // move record
                    data.RemoveAt(fromIndex);
                    int newIndex = data.IndexOf(anchorEntry);
                    data.Insert(isAfterID ? newIndex + 1 : newIndex, entry);
                }
            }
        }

        /// <summary>Apply the patch to a dictionary asset.</summary>
        /// <typeparam name="TKey">The dictionary key type.</typeparam>
        /// <typeparam name="TValue">The dictionary value type.</typeparam>
        /// <param name="asset">The asset being edited.</param>
        /// <param name="hasEntry">Get whether the collection has the given entry.</param>
        /// <param name="getEntry">Get an entry from the collection.</param>
        /// <param name="removeEntry">Remove an entry from the collection.</param>
        /// <param name="setEntry">Add or replace an entry in the collection.</param>
        private void ApplyCollection<TKey, TValue>(IAssetInfo asset, Func<TKey, bool> hasEntry, Func<TKey, TValue> getEntry, Action<TKey> removeEntry, Action<TKey, TValue> setEntry)
        {
            // apply records
            if (this.Records != null)
            {
                int i = 0;
                foreach (EditDataPatchRecord record in this.Records)
                {
                    string errorPrefix = $"Can't apply data patch \"{this.Path} > entry #{i}\" to {this.TargetAsset}";
                    i++;

                    // get key
                    TKey key = (TKey)Convert.ChangeType(record.Key.Value, typeof(TKey));

                    // apply string
                    if (typeof(TValue) == typeof(string))
                    {
                        if (record.Value?.Value == null)
                            removeEntry(key);
                        else if (record.Value.Value is JValue field)
                            setEntry(key, field.Value<TValue>());
                        else
                            this.Monitor.Log($"{errorPrefix}: this asset has string values (but {record.Value.Value.Type} values were provided).", LogLevel.Warn);
                    }

                    // apply object
                    else
                    {
                        if (record.Value?.Value == null)
                            removeEntry(key);
                        else if (record.Value.Value is JObject field)
                            setEntry(key, field.ToObject<TValue>());
                        else
                            this.Monitor.Log($"{errorPrefix}: this asset has {typeof(TValue)} values (but {record.Value.Value.Type} values were provided).", LogLevel.Warn);
                    }
                }
            }

            // apply fields
            if (this.Fields != null)
            {
                char fieldDelimiter = this.GetStringFieldDelimiter(asset);
                foreach (IGrouping<string, EditDataPatchField> recordGroup in this.Fields.GroupByIgnoreCase(p => p.EntryKey.Value))
                {
                    string errorPrefix = $"Can't apply data patch \"{this.Path}\" to {this.TargetAsset}";

                    // get key
                    TKey key = (TKey)Convert.ChangeType(recordGroup.Key, typeof(TKey));
                    if (!hasEntry(key))
                    {
                        this.Monitor.Log($"{errorPrefix}: there's no record matching key '{key}' under {nameof(PatchConfig.Fields)}.", LogLevel.Warn);
                        continue;
                    }

                    // apply string
                    if (typeof(TValue) == typeof(string))
                    {
                        string[] actualFields = ((string)(object)getEntry(key)).Split(fieldDelimiter);
                        foreach (EditDataPatchField field in recordGroup)
                        {
                            if (!int.TryParse(field.FieldKey.Value, out int index))
                            {
                                this.Monitor.Log($"{errorPrefix}: record '{key}' under {nameof(PatchConfig.Fields)} is a string, so it requires a field index between 0 and {actualFields.Length - 1} (received \"{field.FieldKey}\" instead)).", LogLevel.Warn);
                                continue;
                            }
                            if (index < 0 || index > actualFields.Length - 1)
                            {
                                this.Monitor.Log($"{errorPrefix}: record '{key}' under {nameof(PatchConfig.Fields)} has no field with index {index} (must be 0 to {actualFields.Length - 1}).", LogLevel.Warn);
                                continue;
                            }

                            actualFields[index] = field.Value.Value.Value<string>();
                        }

                        setEntry(key, (TValue)(object)string.Join(fieldDelimiter.ToString(), actualFields));
                    }

                    // apply object
                    else
                    {
                        JObject obj = new JObject();
                        foreach (EditDataPatchField field in recordGroup)
                            obj[field.FieldKey.Value] = field.Value.Value;

                        JsonSerializer serializer = new JsonSerializer();
                        using JsonReader reader = obj.CreateReader();
                        serializer.Populate(reader, getEntry(key));
                    }
                }
            }

            // apply text operations
            for (int i = 0; i < this.TextOperations.Length; i++)
            {
                if (!this.TryApplyTextOperation(asset, this.TextOperations[i], hasEntry, getEntry, setEntry, out string error))
                    this.Monitor.Log($"Can't data patch \"{this.Path} > text operation #{i}\" to {this.TargetAsset}: {error}", LogLevel.Warn);
            }
        }

        /// <summary>Try to apply a text operation.</summary>
        /// <typeparam name="TKey">The dictionary key type.</typeparam>
        /// <typeparam name="TValue">The dictionary value type.</typeparam>
        /// <param name="asset">The asset being edited.</param>
        /// <param name="operation">The text operation to apply.</param>
        /// <param name="hasEntry">Get whether the collection has the given entry.</param>
        /// <param name="getEntry">Get an entry from the collection.</param>
        /// <param name="setEntry">Add or replace an entry in the collection.</param>
        /// <param name="error">An error indicating why applying the operation failed, if applicable.</param>
        /// <returns>Returns whether applying the operation succeeded.</returns>
        private bool TryApplyTextOperation<TKey, TValue>(IAssetInfo asset, TextOperation operation, Func<TKey, bool> hasEntry, Func<TKey, TValue> getEntry, Action<TKey, TValue> setEntry, out string error)
        {
            var targetRoot = operation.GetTargetRoot();
            switch (targetRoot)
            {
                case TextOperationTargetRoot.Entries:
                    {
                        // validate
                        if (typeof(TValue) != typeof(string))
                            return this.Fail($"an '{TextOperationTargetRoot.Entries}' text operation can only be used for string entries. For data model entries, use '{TextOperationTargetRoot.Fields}' instead.", out error);
                        if (operation.Target.Length != 2)
                            return this.Fail($"an '{TextOperationTargetRoot.Entries}' path must have exactly one other segment: the entry key.", out error);

                        // parse path
                        if (!this.TryParseEntryKey(operation.Target[1].Value, out TKey key, out error))
                            return false;

                        // get value
                        string value = hasEntry(key)
                            ? (string)(object)getEntry(key)
                            : null;

                        // set value
                        setEntry(key, (TValue)(object)operation.Apply(value));
                    }
                    break;

                case TextOperationTargetRoot.Fields:
                    {
                        // validate
                        if (operation.Target.Length != 3)
                            return this.Fail($"a '{TextOperationTargetRoot.Fields}' path must have exactly two other segments: one for the entry key, and one for the field key or index.", out error);

                        // parse path
                        if (!this.TryParseEntryKey(operation.Target[1].Value, out TKey entryKey, out error))
                            return false;
                        string fieldKey = operation.Target[2].Value;

                        // get entry
                        TValue entry;
                        {
                            bool entryExists = hasEntry(entryKey);
                            entry = entryExists
                                ? getEntry(entryKey)
                                : default;

                            if (!entryExists || entry is null)
                                return this.Fail($"record '{entryKey}' has no value, so field '{fieldKey}' can't be modified using text operations.", out error);
                        }

                        // edit field value
                        if (typeof(TValue) == typeof(string))
                        {
                            // read fields
                            char fieldDelimiter = this.GetStringFieldDelimiter(asset);
                            string[] actualFields = ((string)(object)entry).Split(fieldDelimiter);

                            // validate key
                            if (!int.TryParse(fieldKey, out int fieldIndex))
                                return this.Fail($"record '{entryKey}' needs a field index between 0 and {actualFields.Length - 1} (received \"{fieldKey}\" instead)).", out error);
                            if (fieldIndex < 0 || fieldIndex > actualFields.Length - 1)
                                return this.Fail($"record '{entryKey}' has no field with index {fieldIndex} (must be 0 to {actualFields.Length - 1}).", out error);

                            // apply change
                            actualFields[fieldIndex] = operation.Apply(actualFields[fieldIndex]);
                            setEntry(entryKey, (TValue)(object)string.Join(fieldDelimiter.ToString(), actualFields));
                        }
                        else
                        {
                            // get field
                            JObject entryToken = (JObject)(object)entry;
                            JProperty field = entryToken.Property(fieldKey);

                            // apply change
                            if (field == null)
                                entryToken[fieldKey] = operation.Apply("");
                            else
                            {
                                if (field.Value.Type != JTokenType.String)
                                    return this.Fail($"field '{entryKey}' > '{fieldKey}' has type '{entryToken.Type}', but you can only apply text operations to a text field.", out error);

                                field.Value = operation.Apply(field.Value<string>());
                            }
                        }
                    }
                    break;

                default:
                    return this.Fail(
                        targetRoot == null
                            ? $"unknown path root '{operation.Target[0]}'."
                            : $"path root '{targetRoot}' isn't valid for an {nameof(PatchType.EditMap)} patch",
                        out error
                    );
            }

            error = null;
            return true;
        }

        /// <summary>Try to parse a raw string into an entry key.</summary>
        /// <typeparam name="TKey">The dictionary key type.</typeparam>
        /// <param name="rawKey">The string key to parse.</param>
        /// <param name="parsed">The parsed key.</param>
        /// <param name="error">The error indicating why the <paramref name="rawKey"/> couldn't be parsed, if applicable.</param>
        /// <returns>Returns whether parsing succeeded.</returns>
        bool TryParseEntryKey<TKey>(string rawKey, out TKey parsed, out string error)
        {
            parsed = default;
            error = null;

            // to string
            if (typeof(TKey) == typeof(string))
            {
                parsed = (TKey)(object)rawKey;
                return true;
            }

            // to int
            if (typeof(TKey) == typeof(int))
            {
                if (!int.TryParse(rawKey, out int intKey))
                    return this.Fail($"can't use value '{rawKey}' as an entry key because this asset uses numeric keys.", out error);

                parsed = (TKey)(object)intKey;
                return true;
            }

            // invalid
            parsed = default;
            return this.Fail($"unsupported asset key type '{typeof(TKey).FullName}'.", out error);
        }

        /// <summary>Get the key for a list asset entry.</summary>
        /// <typeparam name="TValue">The list value type.</typeparam>
        /// <param name="entity">The entity whose ID to fetch.</param>
        private string GetKey<TValue>(TValue entity)
        {
            return InternalConstants.GetListAssetKey(entity, this.Reflection);
        }

        /// <summary>Get the delimiter used in string entries for an asset.</summary>
        /// <param name="asset">The asset being edited.</param>
        private char GetStringFieldDelimiter(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/Achievements")
                ? '^'
                : '/';
        }
    }
}
