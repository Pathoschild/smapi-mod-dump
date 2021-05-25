/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/berkayylmao/StardewValleyMods
**
*************************************************/

#region License

// clang-format off
// 
//    ChestEx (StardewValleyMods)
//    Copyright (c) 2021 Berkay Yigit <berkaytgy@gmail.com>
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.
// 
// clang-format on

#endregion

using System;

using ChestEx.LanguageExtensions;

using Microsoft.Xna.Framework;

using StardewValley.Objects;

namespace ChestEx {
  public static class CustomChestConfigHelpers {
    public static CustomChestConfig GetCustomConfig(this Chest chest) {
      var config = new CustomChestConfig {
        mName = chest.GetCustomConfigName(), mDescription = chest.GetCustomConfigDescription(), mHingesColour = chest.GetCustomConfigHingesColour()
      };

      chest.SetCustomConfig(config);

      return config;
    }

    public static void SetCustomConfig(this Chest chest, CustomChestConfig config) {
      chest.SetCustomConfigName(config.mName);
      chest.SetCustomConfigDescription(config.mDescription);
      chest.SetCustomConfigHingesColour(config.mHingesColour);
    }

    public static String GetCustomConfigName(this Chest chest) {
      String return_text = CustomChestConfig.CONST_DEFAULT_NAME;

      if (chest.modData.TryGetValue($"{CustomChestConfig.CONST_MODDATA_PREFIX}/{CustomChestConfig.CONST_NAME_KEY}", out String name))
        if (!String.IsNullOrWhiteSpace(name))
          return_text = name;

      if (chest.modData.TryGetValue(CustomChestConfig.CONST_CHESTSANYWHERE_NAME_COMPATIBILITY_KEY, out String chestsanywhere_name))
        if (!String.IsNullOrWhiteSpace(chestsanywhere_name))
          return_text = chestsanywhere_name;

      return return_text;
    }

    public static String GetCustomConfigDescription(this Chest chest) {
      String return_text = String.Empty;

      if (chest.modData.TryGetValue($"{CustomChestConfig.CONST_MODDATA_PREFIX}/{CustomChestConfig.CONST_DESC_KEY}", out String desc))
        if (!String.IsNullOrWhiteSpace(desc))
          return_text = desc;

      return return_text;
    }

    public static Color GetCustomConfigHingesColour(this Chest chest) {
      Color return_colour = Color.Black;

      if (chest.modData.TryGetValue($"{CustomChestConfig.CONST_MODDATA_PREFIX}/{CustomChestConfig.CONST_HINGES_KEY}", out String hinges_text))
        if (!String.IsNullOrWhiteSpace(hinges_text) && hinges_text.Length == 6)
          return_colour = hinges_text.AsXNAColor();

      return return_colour;
    }

    public static void SetCustomConfigName(this Chest chest, String name) {
      String final_name = name.Trim().Trim('|');
      chest.modData[$"{CustomChestConfig.CONST_MODDATA_PREFIX}/{CustomChestConfig.CONST_NAME_KEY}"] = final_name;
      chest.modData[CustomChestConfig.CONST_CHESTSANYWHERE_NAME_COMPATIBILITY_KEY]                  = final_name;
    }

    public static void SetCustomConfigDescription(this Chest chest, String description) {
      chest.modData[$"{CustomChestConfig.CONST_MODDATA_PREFIX}/{CustomChestConfig.CONST_DESC_KEY}"] = description.Trim();
    }

    public static void SetCustomConfigHingesColour(this Chest chest, Color hingesColour) {
      chest.modData[$"{CustomChestConfig.CONST_MODDATA_PREFIX}/{CustomChestConfig.CONST_HINGES_KEY}"] = hingesColour.AsHexCode();
    }
  }
}
