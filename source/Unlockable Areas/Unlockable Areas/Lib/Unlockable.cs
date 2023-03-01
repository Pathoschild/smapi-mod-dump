/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using System.Reflection;
using Unlockable_Areas.NetLib;

namespace Unlockable_Areas.Lib
{
    public class Unlockable : INetObject<NetFields>
    {
        public NetFields NetFields { get; } = new NetFields();

        private NetString _id = new NetString();
        private NetString _location = new NetString();
        private NetString _locationUnique = new NetString();
        private NetString _shopDescription = new NetString();
        private NetString _shopPosition = new NetString();
        private NetString _shopTexture = new NetString();
        public NetString _shopAnimation = new NetString();
        public NetString _shopEvent = new NetString();
        public NetStringIntDictionary _price = new NetStringIntDictionary();
        private NetString _updateMap = new NetString();
        private NetString _updateType = new NetString();
        private NetString _updatePosition = new NetString();

        public string ID { get => _id.Value; set => _id.Value = value; }
        public string Location { get => _location.Value; set => _location.Value = value; }
        public string LocationUnique { get => _locationUnique.Value; set => _locationUnique.Value = value; }
        public string ShopDescription { get => _shopDescription.Value; set => _shopDescription.Value = value; }
        public string ShopPosition { get => _shopPosition.Value; set => _shopPosition.Value = value; }
        public string ShopTexture { get => _shopTexture.Value; set => _shopTexture.Value = value; }
        public string ShopAnimation { get => _shopAnimation.Value; set => _shopAnimation.Value = value; }
        public string ShopEvent { get => _shopEvent.Value; set => _shopEvent.Value = value; }
        public Dictionary<string, int> Price
        {
            get => this._price.Pairs.AsEnumerable().ToDictionary(x => x.Key, x => x.Value);
            set => this._price.CopyFrom(value.AsEnumerable());
        }
        public string UpdateMap { get => _updateMap.Value; set => _updateMap.Value = value; }
        public string UpdateType { get => _updateType.Value; set => _updateType.Value = value; }
        public string UpdatePosition { get => _updatePosition.Value; set => _updatePosition.Value = value; }
        public Vector2 vUpdatePosition { get => new Vector2(float.Parse(UpdatePosition.Split(",").First()), float.Parse(UpdatePosition.Split(",").Last())); } //UpdatePosition as Vector2
        public Vector2 vShopPosition { get => new Vector2(float.Parse(ShopPosition.Split(",").First()), float.Parse(ShopPosition.Split(",").Last())); } //ShopPosition as Vector2

        public Unlockable(UnlockableModel model)
        {
            this.ID = model.ID;
            this.Location = model.Location;
            this.LocationUnique = model.LocationUnique;
            this.ShopDescription = model.ShopDescription;
            this.ShopPosition = model.ShopPosition;
            this.ShopTexture = model.ShopTexture;
            this.ShopAnimation = model.ShopAnimation;
            this.ShopEvent = model.ShopEvent;
            this.Price = model.Price;
            this.UpdateMap = model.UpdateMap;
            this.UpdateType = model.UpdateType;
            this.UpdatePosition = model.UpdatePosition;
            addNetFields();
        }

        public Unlockable() => addNetFields();

        private void addNetFields() => this.NetFields.AddFields(
            _id,
            _location,
            _locationUnique,
            _shopDescription,
            _shopTexture,
            _shopAnimation,
            _shopEvent,
            _shopPosition,
            _price,
            _updateMap,
            _updateType,
            _updatePosition
            );

        public static Dictionary<string, Unlockable> convertModelDicToEntity(Dictionary<string, UnlockableModel> modelDic)
        {
            var entityDic = new Dictionary<string, Unlockable>();
            foreach (var entry in modelDic) {
                entry.Value.ID = entry.Key;
                entityDic.Add(entry.Key, new Unlockable(entry.Value));
            }

            return entityDic;
        }

        public GameLocation getGameLocation()
        {
            return Game1.getLocationFromName(LocationUnique, Location != LocationUnique);
        }
    }
}
