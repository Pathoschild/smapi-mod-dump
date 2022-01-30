/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Netcode;
using StardewValley;
using StardewValley.Network;

namespace SDV.Shared.Abstractions
{
  public interface IModDataDictionaryWrapper : IWrappedType<ModDataDictionary>
  {
    uint DirtyTick { get; set; }
    bool NeedsTick { get; set; }
    bool ChildNeedsTick { get; set; }
    INetSerializable Parent { get; set; }
    bool Dirty { get; }
    INetRoot Root { get; set; }
    INetSerializable NetFields { get; }
    bool IsReadOnly { get; }

    NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
      NetStringDictionary<string, NetString>>.KeysCollection Keys { get; }

    NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
      NetStringDictionary<string, NetString>>.ValuesCollection Values { get; }

    NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
      NetStringDictionary<string, NetString>>.PairsCollection Pairs { get; }

    IDictionary<string, NetString> FieldDict { get; }
    string this[string key] { get; set; }
    void SetFromSerialization(IModDataDictionaryWrapper source);
    IModDataDictionaryWrapper GetForSerialization();
    bool Equals(NetStringDictionary<string, NetString> other);
    void MarkDirty();
    void MarkClean();
    bool Tick();
    void Read(BinaryReader reader, NetVersion version);
    void Write(BinaryWriter writer);
    void ReadFull(BinaryReader reader, NetVersion version);
    void WriteFull(BinaryWriter writer);
    void CopyFrom(IEnumerable<KeyValuePair<string, string>> dict);
    void Set(IEnumerable<KeyValuePair<string, string>> dict);
    void MoveFrom(NetStringDictionary<string, NetString> dict);
    void SetEqualityComparer(IEqualityComparer<string> comparer);
    void Add(string key, string value);
    void Add(string key, NetString field);
    void Add(SerializableDictionary<string, string> dict);
    void Clear();
    bool ContainsKey(string key);
    int Count();
    bool Remove(string key);
    void Filter(Func<KeyValuePair<string, string>, bool> f);
    bool TryGetValue(string key, out string value);
    IEnumerator<SerializableDictionary<string, string>> GetEnumerator();

    event NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
      NetStringDictionary<string, NetString>>.ContentsChangeEvent OnValueAdded;

    event NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
      NetStringDictionary<string, NetString>>.ContentsChangeEvent OnValueRemoved;

    event NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
      NetStringDictionary<string, NetString>>.ContentsUpdateEvent OnValueTargetUpdated;

    event NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
      NetStringDictionary<string, NetString>>.ConflictResolveEvent OnConflictResolve;
  }
}