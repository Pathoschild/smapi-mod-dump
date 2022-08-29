/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace PlatoTK.Utils
{
    public interface IBasicUtils
    {
        ISalable GetSalable(string type, int index = -1, string name = "none");

        Item GetObject(bool bigCraftable, int index = -1, string name = "none");

        Item GetItem(string type, int index = -1, string name = "none");

        int GetIndexByName(IDictionary<int, string> dictionary, string name);
        Type GetTypeSDV(string type);

        Map GetMap(string location);

        void SetProperty(Map map, string property, object value);
        void SetProperty(Map map, string layerid, string property, object value);
        void SetProperty(Map map, string layerid, int x, int y, string property, object value);

        object GetProperty(Map map, string property);
        object GetProperty(Map map, string layerid, string property);
        object GetProperty(Map map, string layerid, int x, int y, string property);

        void UpdateWarps(string locationName);

        void UpdateWarps(GameLocation location);

        void ReloadMap(GameLocation location);
    }

    internal class BasicUtils : InnerHelper, IBasicUtils
    {
        public BasicUtils(IPlatoHelper helper)
            : base(helper)
        {

        }

        public void ReloadMap(GameLocation location)
        {
            GameLocation l = (location ?? Game1.currentLocation);
            Plato.ModHelper.GameContent.InvalidateCache(l.mapPath.Value);
            l?.reloadMap();
            l?.resetForPlayerEntry();
        }

        public void UpdateWarps(string locationName)
        {
            UpdateWarps(Game1.getLocationFromName(locationName));
        }

        public void UpdateWarps(GameLocation location)
        {
            location.warps.Clear();
            if (location.Map.Properties.TryGetValue("Warp", out PropertyValue p) && !string.IsNullOrEmpty(p))
                location.updateWarps();
        }

        public Map GetMap(string location)
        {
            return Game1.getLocationFromName(location)?.Map;
        }

        public void SetProperty(Map map, string property, object value)
        {
            map.Properties[property] = (PropertyValue)value;
        }

        public void SetProperty(Map map, string layerid, string property, object value)
        {
            if (map.GetLayer(layerid) is Layer layer)
                layer.Properties[property] = (PropertyValue)value;
        }

        public void SetProperty(Map map, string layerid, int x, int y, string property, object value)
        {
            if (map.GetLayer(layerid) is Layer layer
                && layer.Tiles[x,y] is Tile tile)
                tile.Properties[property] = (PropertyValue)value;
        }

        public object GetProperty(Map map, string property)
        {
            return map?.Properties[property];
        }

        public object GetProperty(Map map, string layerid, string property)
        {
            return map?.GetLayer(layerid)?.Properties[property];
        }

        public object GetProperty(Map map, string layerid, int x, int y, string property)
        {
            return map?.GetLayer(layerid)?.Tiles[x,y]?.Properties[property];
        }

        public int GetIndexByName(IDictionary<int, string> dictionary, string name)
        {
            if (name.StartsWith("startswith:"))
                return (dictionary.Where(d => d.Value.Split('/')[0].StartsWith(name.Split(':')[1])).FirstOrDefault()).Key;
            else if (name.StartsWith("endswith:"))
                return (dictionary.Where(d => d.Value.Split('/')[0].EndsWith(name.Split(':')[1])).FirstOrDefault()).Key;
            else if (name.StartsWith("contains:"))
                return (dictionary.Where(d => d.Value.Split('/')[0].Contains(name.Split(':')[1])).FirstOrDefault()).Key;
            else
                return (dictionary.Where(d => d.Value.Split('/')[0] == name).FirstOrDefault()).Key;
        }

        public Type GetTypeSDV(string type)
        {
            string prefix = "StardewValley.";
            Type defaulSDV = Type.GetType(prefix + type + ", Stardew Valley");

            if (defaulSDV != null)
                return defaulSDV;
            else
                return Type.GetType(prefix + type + ", StardewValley");
        }

        public Item GetObject(bool bigCraftable, int index = -1, string name = "none")
        {
            if (GetSalable(bigCraftable ? "BigObject" : "Object", index, name) is StardewValley.Object obj)
                return obj;

            return null;
        }

        public Item GetItem(string type, int index = -1, string name = "none")
        {
            if (GetSalable(type, index, name) is Item item)
                return item;

            return null;
        }

        public ISalable GetSalable(string type, int index = -1, string name = "none")
        {
            Item item = null;

            if (type == "Object")
            {
                if (index != -1)
                    item = new StardewValley.Object(index, 1);
                else if (name != "none")
                    item = new StardewValley.Object(GetIndexByName(Game1.objectInformation, name), 1);
            }
            else if (type == "BigObject")
            {
                if (index != -1)
                    item = new StardewValley.Object(Vector2.Zero, index);
                else if (name != "none")
                    item = new StardewValley.Object(Vector2.Zero, GetIndexByName(Game1.bigCraftablesInformation, name));
            }
            else if (type == "Ring")
            {
                if (index != -1)
                    item = new Ring(index);
                else if (name != "none")
                    item = new Ring(GetIndexByName(Game1.objectInformation, name));
            }
            else if (type == "Hat")
            {
                if (index != -1)
                    item = new Hat(index);
                else if (name != "none")
                    item = new Hat(GetIndexByName(Plato.ModHelper.GameContent.Load<Dictionary<int, string>>(@"Data/hats"), name));
            }
            else if (type == "Boots")
            {
                if (index != -1)
                    item = new Boots(index);
                else if (name != "none")
                    item = new Boots(GetIndexByName(Plato.ModHelper.GameContent.Load<Dictionary<int, string>>(@"Data/Boots"), name));
            }
            else if (type == "Clothing")
            {
                if (index != -1)
                    item = new Clothing(index);
                else if (name != "none")
                    item = new Clothing(GetIndexByName(Plato.ModHelper.GameContent.Load<Dictionary<int, string>>(@"Data/ClothingInformation"), name));
            }
            else if (type == "TV")
            {
                if (index != -1)
                    item = new StardewValley.Objects.TV(index, Vector2.Zero);
                else if (name != "none")
                    item = new TV(GetIndexByName(Plato.ModHelper.GameContent.Load<Dictionary<int, string>>(@"Data/Furniture"), name), Vector2.Zero);
            }
            else if (type == "IndoorPot")
                item = new StardewValley.Objects.IndoorPot(Vector2.Zero);
            else if (type == "CrabPot")
                item = new StardewValley.Objects.CrabPot(Vector2.Zero);
            else if (type == "Chest")
                item = new StardewValley.Objects.Chest(true);
            else if (type == "Cask")
                item = new StardewValley.Objects.Cask(Vector2.Zero);
            else if (type == "Cask")
                item = new StardewValley.Objects.Cask(Vector2.Zero);
            else if (type == "Furniture")
            {
                if (index != -1)
                    item = new StardewValley.Objects.Furniture(index, Vector2.Zero);
                else if (name != "none")
                    item = new Furniture(GetIndexByName(Plato.ModHelper.GameContent.Load<Dictionary<int, string>>(@"Data/Furniture"), name), Vector2.Zero);
            }
            else if (type == "Sign")
                item = new StardewValley.Objects.Sign(Vector2.Zero, index);
            else if (type == "Wallpaper")
                item = new StardewValley.Objects.Wallpaper(Math.Abs(index), false);
            else if (type == "Floors")
                item = new StardewValley.Objects.Wallpaper(Math.Abs(index), true);
            else if (type == "MeleeWeapon")
            {
                if (index != -1)
                    item = new MeleeWeapon(index);
                else if (name != "none")
                    item = new MeleeWeapon(GetIndexByName(Plato.ModHelper.GameContent.Load<Dictionary<int, string>>(@"Data/weapons"), name));

            }
            else if (type == "SDVType")
            {
                if (index == -1)
                    item = Activator.CreateInstance(GetTypeSDV(name)) is Item i ? i : null;
                else
                    item = Activator.CreateInstance(GetTypeSDV(name), index) is Item i ? i : null;

            }
            else if (type == "ByType")
            {
                try
                {
                    if (index == -1)
                        item = Activator.CreateInstance(Type.GetType(name)) is Item i ? i : null;
                    else
                        item = Activator.CreateInstance(Type.GetType(name), index) is Item i ? i : null;
                }
                catch (Exception ex)
                {
                }
            }

            return item;
        }

       
    }
}
