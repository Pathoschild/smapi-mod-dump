//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace CompostPestsCultivation
{
    public class ModComponent
    {
        /*
        public static T LoadField<T>(string name, T def = default(T))
        {
            ModEntry.GetMonitor().Log("Loading field: " + name, StardewModdingAPI.LogLevel.Trace);
            //SaveData data = ModEntry.GetHelper().Data.ReadSaveData<SaveData>(name);
            T data = ModEntry.GetHelper().Data.ReadSaveData<T>(name);
            if (data == null)
            {
                ModEntry.GetMonitor().Log("No value found, returning default value", StardewModdingAPI.LogLevel.Trace);
                return def;
            }
            else
            {
                FieldInfo fi = typeof(SaveData).GetField(name);
                if (fi == null)
                {
                    ModEntry.GetMonitor().Log("Couldn't access field " + name + " in " + nameof(SaveData), StardewModdingAPI.LogLevel.Error);
                    return def;
                }
                return (T)fi.GetValue(data);
            }

                //return ModEntry.GetHelper().Reflection.GetField<T>(data, name).GetValue();
        }
        */
           
        /*
        public static T LoadField<T>(T field)
        {
            return LoadField<T>(nameof(field), field);
        }
        */

        /*
        public static void SaveField<T>(string name, T value)
        {
            ModEntry.GetMonitor().Log("Saving field: " + name, StardewModdingAPI.LogLevel.Trace);
            SaveData data = new SaveData();
            ModEntry.GetHelper().Reflection.GetField<T>(data, name).SetValue(value);
            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(name, data);
        }
        */

        public static SaveData AddToSaveData<T>(SaveData data, string name, T value)
        {
            ModEntry.GetHelper().Reflection.GetField<T>(data, name).SetValue(value);
            return data;
        }

        /*
        public static void SaveSaveData(SaveData data)
        {
            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(nameof(SaveData), data);
        }

        public static SaveData LoadSaveData()
        {
            return ModEntry.GetHelper().Data.ReadSaveData<SaveData>(nameof(SaveData));
        }
*/

        public static List<Vector2> GetAdjacentTiles(Vector2 vec)
        {
            List<Vector2> adjacentTiles = new List<Vector2>();
            adjacentTiles.Add(new Vector2(vec.X - 1, vec.Y));
            adjacentTiles.Add(new Vector2(vec.X - 1, vec.Y - 1));
            adjacentTiles.Add(new Vector2(vec.X - 1, vec.Y + 1));
            adjacentTiles.Add(new Vector2(vec.X, vec.Y - 1));
            adjacentTiles.Add(new Vector2(vec.X, vec.Y + 1));
            adjacentTiles.Add(new Vector2(vec.X + 1, vec.Y));
            adjacentTiles.Add(new Vector2(vec.X + 1, vec.Y - 1));
            adjacentTiles.Add(new Vector2(vec.X + 1, vec.Y + 1));
            return adjacentTiles;
        }
    }
}
