/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using Custom_Farm_Loader.Lib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;

namespace Custom_Farm_Loader.Lib
{
    public class HeldObject
    {
        public string ID = "0";
        HeldObjectType Type = HeldObjectType.Furniture;
        public int Rotations;
        public int Quality;

        public static HeldObject parseHeldObject(JObject obj)
        {
            HeldObject held = new HeldObject();

            string name = "";
            foreach (JProperty property in obj.Properties()) {
                name = property.Name;
                string value = property.Value.ToString();

                switch (name.ToLower()) {
                    case "id":
                        held.ID = value;
                        break;

                    case "rotations":
                        held.Rotations = int.Parse(value);
                        break;

                    case "type":
                        held.Type = UtilityMisc.parseEnum<HeldObjectType>(value);
                        break;

                    case "quality":
                        switch (value.ToLower()) {
                            case "silver":
                                held.Quality = 1; break;
                            case "gold":
                                held.Quality = 2; break;
                            case "iridium":
                                held.Quality = 4; break;
                        }
                        break;
                }
            }

            held.ID = held.Type switch {
                HeldObjectType.Item => ItemObject.MapNameToParentsheetindex(held.ID),
                HeldObjectType.Furniture or _ => Furniture.MapNameToParentsheetindex(held.ID),
            };

            return held;
        }

        public StardewValley.Object objectFactory(Vector2 pos)
        {
            if (Type == HeldObjectType.Item) {
                var obj = new StardewValley.Object(int.Parse(ID), 1, quality: Quality);
                obj.TileLocation = pos;
                return obj;

            } else if (Type == HeldObjectType.Furniture)
                return new StardewValley.Objects.Furniture(int.Parse(ID), pos, Rotations);

            return null;
        }
    }
}
