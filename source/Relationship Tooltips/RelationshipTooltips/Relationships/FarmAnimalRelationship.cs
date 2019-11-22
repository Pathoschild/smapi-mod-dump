using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace RelationshipTooltips.Relationships
{
    public class FarmAnimalRelationship : IRelationship
    {
        public const int MAX_FRIENDSHIP_LEVEL = 5;
        public const int FRIENDSHIP_POINTS_PER_LEVEL = 200;

        protected ModConfig Config { get; private set; }
        public FarmAnimalRelationship(ModConfig config)
        {
            Config = config;
        }
        public virtual int Priority => 50000;
        public virtual Func<Character, Item, bool> ConditionsMet => (c, i) => { return c is FarmAnimal && OwnerIsPlayer((c as FarmAnimal).ownerID.Value); };

        public bool BreakAfter => false;

        protected bool OwnerIsPlayer(long ownerID)
        {
            return ownerID == Game1.player.UniqueMultiplayerID || 
                (Game1.IsMultiplayer && Game1.IsClient && Game1.serverHost.Value.UniqueMultiplayerID == ownerID);
        }

        public virtual string GetHeaderText<T>(string currentHeader, T character, Item item = null) where T : Character
        {
            return character.displayName;
        }

        public virtual string GetDisplayText<T>(string currentDisplay, T character, Item item = null) where T : Character
        {
            FarmAnimal animal = character as FarmAnimal;
            string display = Config.animalHappiness + ": " + animal.happiness;
            display += Environment.NewLine;
            display += Config.animalFriendship + ": " + animal.friendshipTowardFarmer / FRIENDSHIP_POINTS_PER_LEVEL + "/" + MAX_FRIENDSHIP_LEVEL;
            display += PettedResult(animal);
            return display;
        }
        private string PettedResult(FarmAnimal f)
        {
            if(f.wasPet)
                return Environment.NewLine + f.getMoodMessage();
            return Environment.NewLine + Config.animalNotPetted;
        }
    }
}
