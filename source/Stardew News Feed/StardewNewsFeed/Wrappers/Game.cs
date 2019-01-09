using System;
using System.Collections.Generic;
using StardewModdingAPI;
using System.Linq;
using StardewNewsFeed.Enums;
using StardewValley;

namespace StardewNewsFeed.Wrappers {

    public class Game : IGame {

        public void DisplayMessage(IHudMessage message) {
            Game1.addHUDMessage(new HUDMessage(message.GetMessageText(), (int)message.GetMessageType()));
        }

        public IFarm GetFarm() {
            return new Farm(Game1.getFarm());
        }

        public FarmCaveChoice GetFarmCaveChoice() {
            switch(Game1.player.caveChoice.Value) {
                case 1:
                    return FarmCaveChoice.FruitBats;
                case 2:
                    return FarmCaveChoice.Mushrooms;
                default:
                    throw new Exception("I broke");
            }
        }

        public IList<ILocation> GetLocations(ITranslationHelper translationHelper) {
            return Game1.locations.Select(l => new Location(l, translationHelper) as ILocation).ToList();
        }
    }
}
