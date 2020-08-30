using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Loader.ContentPacks.Data
{
    class Dialogues : IContentPackData
    {
        public string CompanionName { get; set; }
        public string Source { get; set; }
        public string Locale { get; set; }

        public bool validate(out Dictionary<string, string> errors)
        {
            errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(this.CompanionName))
                errors.Add("CompanionName", "Missing required field");
            if (string.IsNullOrEmpty(this.Source))
                errors.Add("Source", "Missing required field");

            return errors.Count == 0;
        }
    }
}
