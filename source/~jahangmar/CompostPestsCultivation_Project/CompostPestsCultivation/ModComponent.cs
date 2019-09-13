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
        public static SaveData AddToSaveData<T>(SaveData data, string name, T value)
        {
            ModEntry.GetHelper().Reflection.GetField<T>(data, name).SetValue(value);
            return data;
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

        public static System.Random random = new Random();

        public static bool CheckChance(double chance)
        {
            double randd = random.NextDouble() * 100;
            return (randd <= chance);
        }

        public static int GetRandomInt(int max)
        {
            return random.Next(max);
        }
    }
}
