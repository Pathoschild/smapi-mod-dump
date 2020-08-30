using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace DialogueEmotes
{
    class ContentPackLoader
    {
        public static Dictionary<string, Dictionary<string, int>> LoadEmotes(IEnumerable<IContentPack> packs)
        {
            Dictionary<string, Dictionary<string, int>> emoteBank = new Dictionary<string, Dictionary<string, int>>();

            foreach (var pack in packs)
            {
                var bankPatch = pack.ReadJsonFile<Dictionary<string, Dictionary<string, int>>>("emotes.json");

                if (bankPatch == null)
                {
                    DialogueEmotesMod._monitor.Log(
                        $"Content pack {pack.Manifest.Name} ({pack.Manifest.UniqueID}) is not valid emotes pack!", LogLevel.Error);
                    continue;
                }

                bankPatch.ToList().ForEach(p => emoteBank[p.Key] = p.Value);
                DialogueEmotesMod._monitor.Log(
                    $"Loaded emotes content pack {pack.Manifest.Name} {pack.Manifest.Version} " +
                    $"by {pack.Manifest.Author} ({pack.Manifest.UniqueID})", LogLevel.Info);
            }

            return emoteBank;
        }
    }
}
