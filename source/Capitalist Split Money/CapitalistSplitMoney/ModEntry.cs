using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CapitalistSplitMoney
{
    public class ModEntry : Mod
    {
        public static IModHelper ModHelper { get; private set; }
        public static MoneyDataModel MoneyData { get; private set; }
		public static Config Config { get; private set; }
		public static Dictionary<Item, long> OldItems { get; set; } = new Dictionary<Item, long>();


        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;

            MoneyData = helper.Data.ReadJsonFile<MoneyDataModel>("moneyData.json");
            if (MoneyData == null)
            {
                Monitor.Log("Creating new moneyData.json");
                MoneyData = new MoneyDataModel();
                helper.Data.WriteJsonFile("moneyData.json", MoneyData);
            }

			Config = helper.Data.ReadJsonFile<Config>("config.json");
			if (Config == null)
			{
				Monitor.Log("Creating new config.json");
				Config = new Config();
				helper.Data.WriteJsonFile("config.json", Config);
			}

			StardewModdingAPI.Events.SaveEvents.AfterSave += delegate
            {
                if (Game1.IsMasterGame)
                {
                    Monitor.Log("Saving money data");
                    helper.Data.WriteJsonFile("moneyData.json", MoneyData);
                }
            };

			StardewModdingAPI.Events.SaveEvents.BeforeSave += delegate
			{
				if (Game1.IsMasterGame)
				{
					BinListener.Unregister();
				}
			};

			Patch.PatchAll("Ilyaki.CapitalistSplitMoney");
        }
    }
}
