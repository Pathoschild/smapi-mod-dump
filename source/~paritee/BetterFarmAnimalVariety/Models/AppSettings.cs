using System.Collections.Generic;
using System.Linq;

namespace BetterFarmAnimalVariety.Models
{
    public class AppSettings
    {
        public List<AppSetting> Settings;

        public AppSettings(Dictionary<string, string> settings)
        {
            List<AppSetting> Settings = new List<AppSetting>();

            foreach(KeyValuePair<string, string> Entry in settings)
            {
                Settings.Add(new AppSetting(Entry));
            }

            Settings = Settings.OrderBy(kvp => kvp.Value).ToList();

            this.Settings = Settings;
        }

        public List<AppSetting> FindFarmAnimalAppSettings()
        {
            return this.Settings.FindAll(x => x.IsFarmAnimal());
        }
    }
}
