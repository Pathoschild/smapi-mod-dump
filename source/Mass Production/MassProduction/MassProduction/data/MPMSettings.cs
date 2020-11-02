/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using MailFrameworkMod;
using ProducerFrameworkMod.ContentPack;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonAssetsIngredient = JsonAssets.Data.BigCraftableData.Recipe_.Ingredient;

namespace MassProduction
{
    /// <summary>
    /// Configurable settings for a MassProductionMachine.
    /// </summary>
    public class MPMSettings
    {
        public static readonly double STANDARD_BASE_MULTIPLIER = 10.0; //Ten times more input and output used over the normal machines

        public string Key { get; set; }
        public string UpgradeObject { get; set; }
        public double BaseMultiplier { get; set; } = STANDARD_BASE_MULTIPLIER;
        public int InputStaticChange { get; set; } = 0;
        public int OutputStaticChange { get; set; } = 0;
        public double InputMultiplier { get; set; } = 0.0;
        public double OutputMultiplier { get; set; } = 0.0;
        public double TimeMultiplier { get; set; } = 1.0;
        public bool AllowInputlessBases { get; set; } = false;
        public QualitySetting Quality { get; set; } = QualitySetting.NoStars;
        public Dictionary<string, object> UnlockConditions { get; set; }

        public int UpgradeObjectID
        {
            get
            {
                if (!string.IsNullOrEmpty(UpgradeObject))
                {
                    JsonAssets.Api jsonAssets = ModEntry.Instance.Helper.ModRegistry.GetApi("spacechase0.JsonAssets") as JsonAssets.Api;
                    int upgradeObjectId = jsonAssets.GetObjectId(UpgradeObject);
                    return upgradeObjectId;
                }
                return -1;
            }
        }

        /// <summary>
        /// Finds what new amount of input is required.
        /// </summary>
        /// <param name="baseInputStack"></param>
        /// <returns></returns>
        public int CalculateInputRequired(int baseInputStack)
        {
            if (baseInputStack == 0) { return 0; }

            double multiplier = BaseMultiplier + InputMultiplier;
            if (multiplier < 1.0) { multiplier = 1.0; }
            int inputRequired = (int)Math.Ceiling(baseInputStack * multiplier) + InputStaticChange;
            if (inputRequired < 1) { inputRequired = 1; }

            return inputRequired;
        }

        /// <summary>
        /// Finds what new amount of output is produced.
        /// </summary>
        /// <param name="baseOutputStack"></param>
        /// <returns></returns>
        public int CalculateOutputProduced(int baseOutputStack)
        {
            double multiplier = BaseMultiplier + OutputMultiplier;
            if (multiplier < 1.0) { multiplier = 1.0; }
            int outputRequired = (int)Math.Ceiling(baseOutputStack * multiplier) + OutputStaticChange;
            if (outputRequired < 1) { outputRequired = 1; }

            return outputRequired;
        }

        /// <summary>
        /// Calculates the new time required per operation.
        /// </summary>
        /// <param name="baseTime"></param>
        /// <returns></returns>
        public int CalculateTimeRequired(int baseTime)
        {
            //TOREVIEW: time needs to be in increments of ten minutes - does this accomplish that?
            int timeRequired = (int)Math.Round((baseTime / 10.0) * TimeMultiplier) * 10;

            return timeRequired;
        }

        /// <summary>
        /// Gets what quality output will be used.
        /// </summary>
        /// <returns></returns>
        public int GetOutputQuality()
        {
            return (Quality == QualitySetting.KeepInput) ? 0 : (int)Quality;
        }

        /// <summary>
        /// Checked by MailFrameworkMod to see if a recipe can be sent to the player.
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public bool CheckIfRecipeCanBeLearned(Letter letter)
        {
            try
            {
                if (Game1.player.knowsRecipe(letter.Recipe))
                {
                    return false;
                }
                else if (UnlockConditions.Count > 0)
                {
                    if (UnlockConditions.ContainsKey("TotalEarnings") &&
                        Game1.player.totalMoneyEarned < int.Parse(UnlockConditions["TotalEarnings"].ToString()))
                    {
                        return false;
                    }

                    if (UnlockConditions.ContainsKey("UnlockedUpgrade"))
                    {
                        string upgradeKey = UnlockConditions["UnlockedUpgrade"].ToString();
                        string upgradeObjectName = ModEntry.MPMSettings[upgradeKey].UpgradeObject;

                        if (!Game1.player.knowsRecipe(upgradeObjectName))
                        {
                            return false;
                        }
                    }

                    if (UnlockConditions.ContainsKey("IsEndgame") && bool.Parse(UnlockConditions["IsEndgame"].ToString()))
                    {
                        bool jojaComplete = false;

                        if (Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
                        {
                            GameLocation town = Game1.getLocationFromName("Town");
                            jojaComplete = ModEntry.Instance.Helper.Reflection.GetMethod(town, "checkJojaCompletePrerequisite").Invoke<bool>();
                        }

                        bool isEndgame = jojaComplete || Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.hasCompletedCommunityCenter();

                        if (!isEndgame)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Error in checking if recipe could be learned:\n{e}", StardewModdingAPI.LogLevel.Error);
                return false;
            }

            return true;
        }
    }
}
