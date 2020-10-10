/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mikesnorth/StardewNewsFeed
**
*************************************************/


using StardewNPC = StardewValley.NPC;
namespace StardewNewsFeed.Wrappers {
    public class NPC {

        private readonly StardewNPC _npc;
        private readonly GameDate _birthday;

        public NPC(StardewNPC npc) {
            _npc = npc;
            _birthday = new GameDate(npc?.Birthday_Season ?? "spring", npc?.Birthday_Day ?? 0);
        }

        public string GetName() {
            return _npc.getName();
        }

        public bool IsMyBirthday(GameDate date) {
            return date.Equals(_birthday);
        }

        public string GetGiftSuggestion() {
            // TODO Implement
            return "Money";
        }
    }
}
