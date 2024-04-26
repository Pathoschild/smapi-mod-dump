/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using System;
using System.Collections.Generic;

public sealed class FuelConfig {
  public IList<string> extraFuel = new List<string>();
}

public sealed class ModConfig {
  public IDictionary<String, FuelConfig> flavoredItems = new Dictionary<String, FuelConfig>() {
    {"(O)812", new FuelConfig() }, // Roe
    {"(O)447", new FuelConfig() }, // Aged Roe
    {"(O)340", new FuelConfig() }, // Honey
    {"(O)344", new FuelConfig() }, // Jelly
    {"(O)350", new FuelConfig() }, // Juice
    {"(O)342", new FuelConfig() }, // Pickle
    {"(O)348", new FuelConfig() }, // Wine
    {"(O)SpecificBait", new FuelConfig() },
    {"(O)DriedFruit", new FuelConfig() },
    {"(O)DriedMushrooms", new FuelConfig() },
    {"(O)SmokedFish", new FuelConfig() },

    // Atelier wildflour's milk mushrooms
    {"(O)wildflour.ateliercrops_milkshroom", new FuelConfig {extraFuel = new List<string>() {"-6"}} },
  };
}
