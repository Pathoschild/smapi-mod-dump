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
  public class ModDataDictionaryWrapper : IModDataDictionaryWrapper
  {
    public ModDataDictionaryWrapper(ModDataDictionary modDataDictionary) => GetBaseType = modDataDictionary;
    public uint DirtyTick { get; set; }
    public bool NeedsTick { get; set; }
    public bool ChildNeedsTick { get; set; }
    public INetSerializable Parent { get; set; }
    public bool Dirty { get; }
    public INetRoot Root { get; set; }
    public INetSerializable NetFields { get; }
    public bool IsReadOnly { get; }

    public NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
      NetStringDictionary<string, NetString>>.KeysCollection Keys { get; }

    public NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
      NetStringDictionary<string, NetString>>.ValuesCollection Values { get; }

    public NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
      NetStringDictionary<string, NetString>>.PairsCollection Pairs { get; }

    public IDictionary<string, NetString> FieldDict { get; }

    public string this[string key]
    {
      get => null;
      set { }
    }

    public void SetFromSerialization(IModDataDictionaryWrapper source)
    {
    }

    public IModDataDictionaryWrapper GetForSerialization() => null;

    public bool Equals(NetStringDictionary<string, NetString> other) => false;

    public void MarkDirty()
    {
    }

    public void MarkClean()
    {
    }

    public bool Tick() => false;

    public void Read(BinaryReader reader, NetVersion version)
    {
    }

    public void Write(BinaryWriter writer)
    {
    }

    public void ReadFull(BinaryReader reader, NetVersion version)
    {
    }

    public void WriteFull(BinaryWriter writer)
    {
    }

    public void CopyFrom(IEnumerable<KeyValuePair<string, string>> dict)
    {
    }

    public void Set(IEnumerable<KeyValuePair<string, string>> dict)
    {
    }

    public void MoveFrom(NetStringDictionary<string, NetString> dict)
    {
    }

    public void SetEqualityComparer(IEqualityComparer<string> comparer)
    {
    }

    public void Add(string key, string value)
    {
    }

    public void Add(string key, NetString field)
    {
    }

    public void Add(SerializableDictionary<string, string> dict)
    {
    }

    public void Clear()
    {
    }

    public bool ContainsKey(string key) => false;

    public int Count() => 0;

    public bool Remove(string key) => false;

    public void Filter(Func<KeyValuePair<string, string>, bool> f)
    {
    }

    public bool TryGetValue(string key, out string value)
    {
      value = null;
      return false;
    }

    public IEnumerator<SerializableDictionary<string, string>> GetEnumerator()
    {
      yield break;
    }

    public event
      NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
        NetStringDictionary<string, NetString>>.ContentsChangeEvent OnValueAdded;

    public event
      NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
        NetStringDictionary<string, NetString>>.ContentsChangeEvent OnValueRemoved;

    public event
      NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
        NetStringDictionary<string, NetString>>.ContentsUpdateEvent OnValueTargetUpdated;

    public event
      NetDictionary<string, string, NetString, SerializableDictionary<string, string>,
        NetStringDictionary<string, NetString>>.ConflictResolveEvent OnConflictResolve;

    public ModDataDictionary GetBaseType { get; }
  }
}