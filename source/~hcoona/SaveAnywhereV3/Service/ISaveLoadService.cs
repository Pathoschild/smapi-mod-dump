using SaveAnywhereV3.DataContract;

namespace SaveAnywhereV3.Service
{
    public interface ISaveLoadService
    {
        bool Check();

        void Clear();

        void Commit();

        void Save();

        void Load();

        void SaveTo(AggregatedModel model);

        void LoadFrom(AggregatedModel model);
    }
}
