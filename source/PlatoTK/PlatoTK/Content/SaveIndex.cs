/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using PlatoTK.APIs;

namespace PlatoTK.Content
{
    public interface ISaveIndexHandle
    {
        string Id { get; }
        string Value { get; }
        int Index { get; }

        void SetGenericData(string genericData);
    }

    public interface ISaveIndex : ISaveIndexHandle
    {
        void ValidateIndex();
        bool TryAddToContentPatcher();
    }

    public class IndexCheck : ISaveIndexHandle
    {
        public string Id { get; }

        public string Value { get; }

        public int Index { get; }

        public void SetGenericData(string genericData)
        {
            if (SaveIndex.GenericData.ContainsKey(Index))
                SaveIndex.GenericData[Index] = genericData;
            else
                SaveIndex.GenericData.Add(Index, genericData);
        }

        public IndexCheck(string id, string value, int index)
        {
            Id = id;
            Value = value;
            Index = index;
        }
    }

    internal class SaveIndex : ISaveIndex
    {
        internal static readonly List<int> IndexCache = new List<int>();
        internal static readonly Dictionary<int, string> GenericData = new Dictionary<int, string>();
        internal static readonly Dictionary<string, int> Indexes = new Dictionary<string, int>();

        private readonly int MinIndex; 
        private readonly Func<ISaveIndexHandle, bool> Validator;
        private readonly Action<ISaveIndexHandle> Injector;
        private readonly Func<IDictionary<int, string>> DataLoader;
        private readonly IPlatoHelper Helper;

        public string Id { get; }
        public string Value
        {
            get
            {
                var dict = LoadData();
                if (dict.ContainsKey(Index))
                    return dict[Index];

                return "";
            }

        }

        public int Index { get; private set; } = -1;
        
        public SaveIndex(string id, 
            Func<IDictionary<int,string>> loadData, 
            Func<ISaveIndexHandle, bool> validateValue, 
            Action<ISaveIndexHandle> injectValue,
            IPlatoHelper helper, 
            int minIndex = 13000)
        {
            Helper = helper;
            Id = id;
            Validator = validateValue;
            Injector = injectValue;
            DataLoader = loadData;
            MinIndex = minIndex;
            Index = GetNewIndex();
            Injector?.Invoke(this);
            Validator?.Invoke(this);
        }

        public SaveIndex(string id, 
            string dataSource, 
            Func<ISaveIndexHandle, bool> validateValue, 
            Action<ISaveIndexHandle> injectValue, 
            IPlatoHelper helper, 
            int minIndex = 13000)
            : this(id,() => Game1.content.Load<Dictionary<int, string>>(dataSource), validateValue,injectValue, helper, minIndex)
        {
         
        }

        public SaveIndex(string id,
            string genericData,
            IPlatoHelper helper,
            int minIndex = 13000)
            : this(id, () => GenericData, (s) =>  s.Value == genericData, (s) => s.SetGenericData(genericData), helper, minIndex)
        {

        }

        public void ValidateIndex()
        {
            var dict = LoadData();

            if (Validator?.Invoke(this) ?? true)
            {
                Index = Index;
                return;
            }

            Index = GetNewIndex();
            Injector?.Invoke(this);
        }

        private IDictionary<int, string> LoadData()
        {
            return DataLoader?.Invoke() ?? GenericData;
        }

        public void SetGenericData(string genericData)
        {
            if (GenericData.ContainsKey(Index))
                GenericData[Index] = genericData;
            else
                GenericData.Add(Index, genericData);
        }

        private int GetNewIndex()
        {
            var Data = LoadData();

            if (Indexes.ContainsKey(Id))
            {
                Index = Indexes[Id];
                if (Validator?.Invoke(new IndexCheck(Id, Data?[Index] ?? "", Index)) ?? false)
                    return Index;
            }

            if (Index > -1 && !Data.ContainsKey(Index))
            {
                if (Indexes.ContainsKey(Id))
                    Indexes[Id] = Index;
                else
                    Indexes.Add(Id, Index);

                return Index;
            }

            if (Data?.Any(d => Validator?.Invoke(new IndexCheck(Id, d.Value, d.Key)) ?? false) ?? false)
            {
                int i = Data.First(d => Validator?.Invoke(new IndexCheck(Id, d.Value, d.Key)) ?? false).Key;
                
                if (Indexes.ContainsKey(Id))
                    Indexes[Id] = i;
                else
                    Indexes.Add(Id,i);

                return i;
            }

            List<int> indexes = new List<int>()
            {
                MinIndex, GenericData.Keys.Count > 0 ? GenericData.Keys.Max() : 0, IndexCache.Count > 0 ? IndexCache.Max() : 0,LoadData()?.Keys?.Max() ?? 0
            };

            int index = indexes.Max() + 1;
            IndexCache.Add(index);

            if (Indexes.ContainsKey(Id))
                Indexes[Id] = index;
            else
                Indexes.Add(Id, index);

            Index = index;

            return index;
        }

        public bool TryAddToContentPatcher()
        {
            if (!Helper.ModHelper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher"))
                return false;

            var api = Helper.ModHelper.ModRegistry.GetApi<IContentPatcher>("Pathoschild.ContentPatcher");
            api.RegisterToken(Helper.ModHelper.ModRegistry.Get(Helper.ModHelper.ModRegistry.ModID).Manifest, Id, () =>
            {
                ValidateIndex();
                return new[] { Id };
            });

            return true;
        }
    }
}
