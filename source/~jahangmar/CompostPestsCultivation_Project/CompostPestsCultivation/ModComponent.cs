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
using Microsoft.Xna.Framework;

namespace CompostPestsCultivation
{
    public class ModComponent
    {
        public static T LoadField<T>(string name)
        {
            SaveData data = ModEntry.GetHelper().Data.ReadSaveData<SaveData>(name);
            if (data == null)
                return default(T);
            else
                return ModEntry.GetHelper().Reflection.GetField<T>(ModEntry.GetHelper().Data.ReadSaveData<SaveData>(name), name).GetValue();
        }

        public static void SaveField<T>(T value)
        {
            SaveData data = new SaveData();
            ModEntry.GetHelper().Reflection.GetField<T>(data, value.GetType().Name).SetValue(value);
            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(value.GetType().Name, data);
        }

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
