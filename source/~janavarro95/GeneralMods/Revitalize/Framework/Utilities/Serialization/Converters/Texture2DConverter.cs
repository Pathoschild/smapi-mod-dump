/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Revitalize.Framework.Graphics;
using StardewValley;

namespace Revitalize.Framework.Utilities.Serialization.Converters
{
    public class Texture2DConverter : JsonConverter
    {


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var texture = (Texture2D)value;
            writer.WriteValue(texture.Name);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string textureName = reader.Value as string;
            //ModCore.log(textureName);
            if (string.IsNullOrEmpty(textureName)) return new Texture2D(Game1.graphics.GraphicsDevice, 2, 2);
            string[] names = textureName.Split('.');
            if (names.Length == 0) return null;

            if (!TextureManager.TextureManagers.ContainsKey(names[0])) return null;
            return textureName == null ? null : Revitalize.Framework.Graphics.TextureManager.TextureManagers[names[0]].getTexture(names[1]).texture;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Texture2D);
        }
    }
}
