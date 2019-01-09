
using StardewNPC = StardewValley.NPC;
namespace StardewNewsFeed.Wrappers {
    public class NPC {

        private readonly StardewNPC _npc;
        private readonly GameDate _birthday;

        public NPC(StardewNPC npc) {
            _npc = npc;
            _birthday = new GameDate(npc.Birthday_Season, npc.Birthday_Day);
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
