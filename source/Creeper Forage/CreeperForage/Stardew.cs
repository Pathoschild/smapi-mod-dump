using StardewValley;

namespace CreeperForage
{
    public static class Stardew
    {
        public static Farmer GetPlayer()
        {
            return Game1.getFarmer(0);
        }

        public static int GetFriendshipPoints(string NPC)
        {
            Farmer f2 = GetPlayer();
            if (f2.friendshipData.ContainsKey(NPC)) return f2.friendshipData[NPC].Points;
            else return 0;
        }

        public static void SetFriendshipPoints(string NPC, int points)
        {
            Farmer f2 = GetPlayer();
            if (!f2.friendshipData.ContainsKey(NPC)) f2.friendshipData[NPC] = new Friendship(points);
            else f2.friendshipData[NPC].Points = points;
        }
    }
}
