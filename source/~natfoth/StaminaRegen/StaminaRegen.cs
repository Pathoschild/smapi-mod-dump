/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/natfoth/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace StaminaRegen
{
  public class StaminaRegenMod : Mod
  {

    public override void Entry(params object[] objects)
    {
      ReadConfig();
      StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
    }

    private float RegenTime = 3.0f;
    private int StaminaRegenAmount = 1;
    private int HealthRegenAmount = 1;

    private double prevTime = 0;

    void GameEvents_UpdateTick(object sender, EventArgs e)
    {
      if (Game1.player == null || !Game1.hasLoadedGame)
        return;

      if (Game1.currentGameTime.TotalGameTime.TotalSeconds < prevTime + RegenTime)
        return;

      prevTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;

      if (Game1.player.stamina < Game1.player.maxStamina)
      {
        Game1.player.stamina += StaminaRegenAmount;
      }
      else
      {
        Game1.player.stamina = Game1.player.maxStamina;
      }
      if (Game1.player.health < Game1.player.maxHealth)
      {
        Game1.player.health += HealthRegenAmount;
      }
      else
      {
        Game1.player.health = Game1.player.maxHealth;
      }
    }

    private void ReadConfig()
    {
      var configLocation = Path.Combine(PathOnDisk, "StaminaRegenConfig.ini");
      if (File.Exists(configLocation))
      {
        var fileData = File.ReadAllLines(configLocation);
        if (fileData.Length > 1)
        {
          //Load in RegenTickRate
          var regenTickRateString = fileData[0];
          regenTickRateString = regenTickRateString.Replace("RegenTickRate:", "").Trim();
          int newRate;
          if (int.TryParse(regenTickRateString, out newRate))
          {
            RegenTime = Math.Max(1, newRate);
          }

          //Load in StaminaRegenAmount
          var staminaRegenAmountString = fileData[1];
          staminaRegenAmountString = staminaRegenAmountString.Replace("StaminaRegenAmount:", "").Trim();
          int newStaminaAmount;
          if (int.TryParse(staminaRegenAmountString, out newStaminaAmount))
          {
            StaminaRegenAmount = newStaminaAmount;
          }

          //Load in HealthRegenAmount
          var healthRegenAmountString = fileData[2];
          healthRegenAmountString = healthRegenAmountString.Replace("HealthRegenAmount:", "").Trim();
          int newHealthAmount;
          if (int.TryParse(healthRegenAmountString, out newHealthAmount))
          {
            HealthRegenAmount = newHealthAmount;
          }

        }
      }
      else
      {
        var dataToWrite = @"RegenTickRate: 3
StaminaRegenAmount: 1
HealthRegenAmount: 1";

        File.WriteAllText(configLocation, dataToWrite);
      }
    }

  }
}
