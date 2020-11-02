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
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassProduction
{
    public class MailManager
    {
        /// <summary>
        /// Adds all letters for getting recipes to MailFrameworkMod.
        /// </summary>
        /// <param name="machines"></param>
        public static void SetupMail()
        {
            string idPrefix = "JacquePott.MP.";
            string textFormat = "@:^^Congratulations on the progress of your farm. In the interest of helping you continue to bring prosperity to the valley, " +
                "we would like to give you these blueprints to help with production.^^- The Ferngill Industrial Co-Operative";
            Dictionary<string, string> craftingData = ModEntry.Instance.Helper.Content.Load<Dictionary<string, string>>("Data\\CraftingRecipes", ContentSource.GameContent);

            foreach (MPMSettings setting in ModEntry.MPMSettings.Values)
            {
                if (!craftingData.ContainsKey(setting.UpgradeObject))
                {
                    ModEntry.Instance.Monitor.Log(string.Format("Could not find recipe for {0} when adding mail - skipping.", setting.UpgradeObject), LogLevel.Warn);
                    continue;
                }
                
                string id = idPrefix + setting.UpgradeObject.Replace(" ", "").Replace("(", "").Replace(")", "");

                Letter letter = new Letter(id, textFormat, setting.UpgradeObject, setting.CheckIfRecipeCanBeLearned);
                letter.GroupId = "MassProduction.Blueprints";

                MailDao.SaveLetter(letter);
            }
        }
    }
}
