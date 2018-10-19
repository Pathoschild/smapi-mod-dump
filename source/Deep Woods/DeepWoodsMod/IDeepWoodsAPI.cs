
using System.Reflection;

namespace DeepWoodsMod
{
    interface IDeepWoodsAPI
    {
        void RegisterCustomDeepWoodsLevelBuilder(TypeInfo type, string chance, int minLevel = 0, int levelModulo = 1);
        void RegisterCustomDeepWoodsClearingFiller(TypeInfo type, string chance, int minLevel = 0);
        void RegisterCustomDeepWoodsTerrainFeature(TypeInfo type, string chance, int minLevel = 0, int levelModulo = 1);
        void RegisterCustomDeepWoodsResourceClump(TypeInfo type, string chance, int minLevel = 0, int levelModulo = 1);
    }
}
