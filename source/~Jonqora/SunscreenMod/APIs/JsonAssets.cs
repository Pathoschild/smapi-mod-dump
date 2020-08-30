using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonAssets
{
    public interface IApi
    {
        void LoadAssets(string path);

        int GetObjectId(string name);
        int GetCropId(string name);
        int GetFruitTreeId(string name);
        int GetBigCraftableId(string name);
        //  Added Jan 29 2019 (v1.3.0+)
        //int GetHatId(string name);
        //int GetWeaponId(string name);
        //  Added Nov 25 2019 (v1.4+)
        //int GetClothingId(string name);

        //  Added Mar 4 2019 (v1.3.2+)
        //IDictionary<string, int> GetAllObjectIds();
        //IDictionary<string, int> GetAllCropIds();
        //IDictionary<string, int> GetAllFruitTreeIds();
        //IDictionary<string, int> GetAllBigCraftableIds();
        //IDictionary<string, int> GetAllHatIds();
        //IDictionary<string, int> GetAllWeaponIds();
        //  Added Nov 25 2019 (v1.4+)
        //IDictionary<string, int> GetAllClothingIds();

        //  Added Dec 26 2019 (v1.5.4+)
        //List<string> GetAllObjectsFromContentPack(string cp);
        //List<string> GetAllCropsFromContentPack(string cp);
        //List<string> GetAllFruitTreesFromContentPack(string cp);
        //List<string> GetAllBigCraftablesFromContentPack(string cp);
        //List<string> GetAllHatsFromContentPack(string cp);
        //List<string> GetAllWeaponsFromContentPack(string cp);
        //List<string> GetAllClothingFromContentPack(string cp);

        //  Added Nov 28 2019 (v1.5.0+)
        //event EventHandler ItemsRegistered;
        event EventHandler IdsAssigned;
        event EventHandler AddedItemsToShop;
        //  Added Dec 25 2019 (v1.5.3+)
        //event EventHandler IdsFixed;

        //  Added Dec 28 2019 (v1.5.4+)
        //bool FixIdsInItem(Item item);
        //void FixIdsInItemList(List<Item> items);
        //void FixIdsInLocation(GameLocation location);

        //  Added Nov 25 2019 (v1.4+)
        //bool TryGetCustomSprite(object entity, out Texture2D texture, out Rectangle sourceRect);
        //bool TryGetCustomSpriteSheet(object entity, out Texture2D texture, out Rectangle sourceRect);
    }
}
