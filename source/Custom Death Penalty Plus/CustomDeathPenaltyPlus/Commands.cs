/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomDeathPenaltyPlus
**
*************************************************/

using StardewModdingAPI;

namespace CustomDeathPenaltyPlus
{
    public class Commands
    {
        internal static ModConfig config;
        internal static void SetConfig(ModConfig config)
        {
            Commands.config = config;
        }
        public void DeathPenalty(string[] args, IMonitor monitor, IModHelper helper)
        {
            var dp = config.DeathPenalty;

            switch (args[0])
            {
                case "items":
                case "restoreitems":
                    {
                        dp.RestoreItems = bool.Parse(args[1]);
                        monitor.Log($"RestoreItems set to {args[1]}", LogLevel.Info);
                        break;   
                    }
                case "cap":
                case "moneylosscap":
                    {
                        if (int.Parse(args[1]) < 0)
                        {
                            monitor.Log("Value specified is not in the valid range for MoneyLossCap", LogLevel.Error);
                        }
                        else
                        {
                            dp.MoneyLossCap = int.Parse(args[1]);
                            monitor.Log($"DeathPenalty - MoneyLossCap set to {args[1]}", LogLevel.Info);
                        }
                        
                        break;
                    }
                case "money":
                case "moneytorestorepercentage":
                    {
                        if (double.Parse(args[1]) < 0 || double.Parse(args[1]) > 1)
                        {
                            monitor.Log("Value specified is not in the valid range for MoneytoRestorePercentage", LogLevel.Error);
                        }
                        else
                        {
                            dp.MoneytoRestorePercentage = double.Parse(args[1]);
                            monitor.Log($"DeathPenalty - MoneytoRestorePercentage set to {args[1]}", LogLevel.Info);
                        }
                        break;
                    }
                case "health":
                case "healthtorestorepercentage":
                    {
                        if (double.Parse(args[1]) < 0 || double.Parse(args[1]) > 1)
                        {
                            monitor.Log("Value specified is not in the valid range for HealthtoRestorePercentage", LogLevel.Error);
                        }
                        else
                        {
                            dp.HealthtoRestorePercentage = double.Parse(args[1]);
                            monitor.Log($"HealthtoRestorePercentage set to {args[1]}", LogLevel.Info);
                        }
                        break;
                    }
                case "energy":
                case "energytorestorepercentage":
                    {
                        if (double.Parse(args[1]) < 0 || double.Parse(args[1]) > 1)
                        {
                            monitor.Log("Value specified is not in the valid range for EnergytoRestorePercentage", LogLevel.Error);
                        }
                        else
                        {
                            dp.EnergytoRestorePercentage = double.Parse(args[1]);
                            monitor.Log($"DeathPenalty - EnergytoRestorePercentage set to {args[1]}", LogLevel.Info);
                        }
                        break;
                    }
                case "friendship":
                case "friendshippenalty":
                    {
                        if (int.Parse(args[1]) < 0)
                        {
                            monitor.Log("Value specified is not in the valid range for FriendshipPenalty", LogLevel.Error);
                        }
                        else
                        {
                            dp.FriendshipPenalty = int.Parse(args[1]);
                            monitor.Log($"FriendshipPenalty set to {args[1]}", LogLevel.Info);
                        }
                        break;
                    }
                case "nextday":
                case "wakeupnextdayinclinic":
                    {
                        
                        dp.WakeupNextDayinClinic = bool.Parse(args[1]);
                        monitor.Log($"WakeupNextDayinClinic set to {args[1]}", LogLevel.Info);
                        break;
                    }
                default:
                    {
                        monitor.Log("Invalid config option specified" +
                            "\n\nAvailable options:" +
                            "\n\n- restoreitems OR items" +
                            "\n- moneylosscap OR cap" +
                            "\n- moneytorestorepercentage OR money" +
                            "\n- healthtorestorepercentage OR health" +
                            "\n- energytorestorepercentage OR energy" +
                            "\n- friendshippenalty OR friendship" +
                            "\n- wakeupnextdayinclinic OR nextday", LogLevel.Error);
                        break;
                    }
            }
            helper.WriteConfig(config);
        }
        public void PassOutPenalty(string[] args, IMonitor monitor, IModHelper helper)
        {
            var pp = config.PassOutPenalty;

            switch (args[0])
            {
                case "cap":
                case "moneylosscap":
                    {
                        if (int.Parse(args[1]) < 0)
                        {
                            monitor.Log("Value specified is not in the valid range for MoneyLossCap", LogLevel.Error);
                        }
                        else
                        {
                            pp.MoneyLossCap = int.Parse(args[1]);
                            monitor.Log($"PassOutPenalty - MoneyLossCap set to {args[1]}", LogLevel.Info);
                        }
                        break;
                    }
                case "money":
                case "moneytorestorepercentage":
                    {
                        if (double.Parse(args[1]) < 0 || double.Parse(args[1]) > 1)
                        {
                            monitor.Log("Value specified is not in the valid range for MoneytoRestorePercentage", LogLevel.Error);
                        }
                        else
                        {
                            pp.MoneytoRestorePercentage = double.Parse(args[1]);
                            monitor.Log($"PassOutPenalty - MoneytoRestorePercentage set to {args[1]}", LogLevel.Info);
                        }
                        break;
                    }
                case "energy":
                case "energytorestorepercentage":
                    {
                        if (double.Parse(args[1]) < 0 || double.Parse(args[1]) > 1)
                        {
                            monitor.Log("Value specified is not in the valid range for EnergytoRestorePercentage", LogLevel.Error);
                        }
                        else
                        {
                            pp.EnergytoRestorePercentage = double.Parse(args[1]);
                            monitor.Log($"PassOutPenalty - EnergytoRestorePercentage set to {args[1]}", LogLevel.Info);
                        }
                        break;
                    }
                default:
                    {
                        monitor.Log("Invalid config option specified" +
                            "\n\nAvailable options:" +
                            "\n\n- moneylosscap OR cap" +
                            "\n- moneytorestorepercentage OR money" +
                            "\n- energytorestorepercentage OR energy", LogLevel.Error);
                        break;
                    }
            }
            helper.WriteConfig(config);
        }

        public void ConfigInfo(string[] args, IMonitor monitor)
        {
            monitor.Log($"Current config settings:" +
                $"\n\nDeathPenalty" +
                $"\n\nRestoreItems: {config.DeathPenalty.RestoreItems.ToString().ToLower()}" +
                $"\nMoneyLossCap: {config.DeathPenalty.MoneyLossCap}" +
                $"\nMoneytoRestorePercentage: {config.DeathPenalty.MoneytoRestorePercentage}" +
                $"\nEnergytoRestorePercentage: {config.DeathPenalty.EnergytoRestorePercentage}" +
                $"\nHealthtoRestorePercentage: {config.DeathPenalty.HealthtoRestorePercentage}" +
                $"\nWakeupNextDayinClinic: {config.DeathPenalty.WakeupNextDayinClinic.ToString().ToLower()}" +
                $"\nFriendshipPenalty: {config.DeathPenalty.FriendshipPenalty}" +
                $"\n\nPassOutPenalty" +
                $"\n\nMoneyLossCap: {config.PassOutPenalty.MoneyLossCap}" +
                $"\nMoneytoRestorePercentage: {config.PassOutPenalty.MoneytoRestorePercentage}" +
                $"\nEnergytoRestorePercentage: {config.PassOutPenalty.EnergytoRestorePercentage}",
                LogLevel.Info);
        }
    }    
}
