/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ForageFantasy
{
    using StardewModdingAPI;
    using StardewValley;
    using System;

    public class ForageFantasy : Mod, IAssetEditor
    {
        public ForageFantasyConfig Config { get; set; }

        public static int DetermineForageQuality(Farmer farmer, bool allowBotanist = true)
        {
            if (allowBotanist && farmer.professions.Contains(Farmer.botanist))
            {
                return 4;
            }
            else
            {
                if (Game1.random.NextDouble() < farmer.ForagingLevel / 30f)
                {
                    return 2;
                }
                else if (Game1.random.NextDouble() < farmer.ForagingLevel / 15f)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ForageFantasyConfig>();

            ForageFantasyConfig.VerifyConfigValues(Config, this);

            Helper.Events.GameLoop.GameLaunched += delegate { ForageFantasyConfig.SetUpModConfigMenu(Config, this); };

            Helper.Events.GameLoop.GameLaunched += delegate { DeluxeGrabberCompatibility.Setup(this); };

            Helper.Events.GameLoop.DayStarted += delegate { TapperAndMushroomQualityLogic.IncreaseTreeAges(this); };

            Helper.Events.GameLoop.SaveLoaded += delegate { FernAndBurgerLogic.ChangeBundle(this); };

            Patcher.PatchAll(this);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return FernAndBurgerLogic.CanEdit<T>(asset, Config);
        }

        public void Edit<T>(IAssetData asset)
        {
            FernAndBurgerLogic.Edit<T>(asset, Config);
        }

        /// <summary>
        /// Small helper method to log to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        /// <summary>
        /// Small helper method to log an error to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        /// <param name="e">an optional error message to log additionally</param>
        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }
    }
}