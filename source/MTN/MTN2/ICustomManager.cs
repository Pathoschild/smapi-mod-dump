using Microsoft.Xna.Framework;
using MTN2.MapData;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;

namespace MTN2 {
    public interface ICustomManager {
        int CabinLimit { get; }
        List<CustomFarm> FarmList { get; }
        List<CustomGreenHouse> GreenHouseList { get; }
        bool NoDebris { get; set; }
        bool Canon { get; }
        int ScienceHouseIndex { get; }
        CustomFarm SelectedFarm { get; }
        CustomFarm LoadedFarm { get; }
        Interaction ShippingBinPoints { get; }
        Interaction RabbitShrine { get; }
        Interaction PetWaterBowl { get; }
        Point FarmHousePorch { get; }
        Point GreenHouseDoor { get; }
        Point FarmCaveOpening { get; }
        int FurnitureLayout { get; }
        int GreenHouseEntryX { get; }
        int GreenHouseEntryY { get; }

        void Populate(IModHelper helper, IMonitor monitor);
        void UpdateSelectedFarm(string farmName);
        void LoadCustomFarm();
        void LoadCustomFarm(int whichFarm);
        string GetAssetKey(out Map map, string type);
        string GetAssetKey(string fileName, FileType fileType);
        Map LoadMap(string fileName);
        Vector2 FarmHouseCoords(float OffsetX = 0, float OffsetY = 0);
        float FarmHouseLayerDepth();
        Vector2 GreenHouseCoords();
        float GreenHouseLayerDepth();
        Vector2 MailboxNotification(float xOffset, float yOffset, bool Option);
        float MailBoxNotifyLayerDepth(bool Option);
        Vector2 GrandpaShrineCoords();
        void SetScienceIndex(int index);
        void Reset();
    }
}
