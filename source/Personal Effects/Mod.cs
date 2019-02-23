using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;

namespace CreeperForage
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
#if DEBUG
        public static bool Debug = true;
#else
        public static bool Debug = false;
#endif
        public override void Entry(IModHelper helper)
        {
            instance = this;
            Monitor.Log("CreeperForage reporting in. " + (Debug ? "Debug" : "Release") + " build active.");

            Config.Load();
            if (Config.ready)
            {
                Item.Setup();
                Spot.Setup(helper);
            }

            if(Debug) helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.OemTilde)
            {

                Monitor.Log("test");
                Stardew.SetFriendshipPoints("Alex", 2500);
                Stardew.SetFriendshipPoints("Elliott", 2500);
                Stardew.SetFriendshipPoints("Harvey", 2500);
                Stardew.SetFriendshipPoints("Sam", 2500);
                Stardew.SetFriendshipPoints("Sebastian", 2500);
                Stardew.SetFriendshipPoints("Shane", 2500);
                Stardew.SetFriendshipPoints("Abigail", 2500);
                Stardew.SetFriendshipPoints("Emily", 2500);
                Stardew.SetFriendshipPoints("Haley", 2500);
                Stardew.SetFriendshipPoints("Leah", 2500);
                Stardew.SetFriendshipPoints("Maru", 2500);
                Stardew.SetFriendshipPoints("Penny", 2500);
                Stardew.SetFriendshipPoints("Caroline", 2500);
                Stardew.SetFriendshipPoints("Clint", 2500);
                Stardew.SetFriendshipPoints("Demetrius", 2500);
                Stardew.SetFriendshipPoints("Gus", 2500);
                Stardew.SetFriendshipPoints("Jodi", 2500);
                Stardew.SetFriendshipPoints("Kent", 2500);
                Stardew.SetFriendshipPoints("Lewis", 2500);
                Stardew.SetFriendshipPoints("Marnie", 2500);
                Stardew.SetFriendshipPoints("Pam", 2500);
                Stardew.SetFriendshipPoints("Pierre", 2500);
                Stardew.SetFriendshipPoints("Robin", 2500);
                Stardew.SetFriendshipPoints("Sandy", 2500);
                Stardew.SetFriendshipPoints("Willy", 2500);
                Stardew.SetFriendshipPoints("Wizard", 2500);
                Stardew.SetFriendshipPoints("Vincent", 2500);
                Stardew.SetFriendshipPoints("Jas", 2500);
                Stardew.SetFriendshipPoints("Linus", 2500);
                Stardew.SetFriendshipPoints("Evelyn", 2500);
                Stardew.SetFriendshipPoints("George", 2500);

                var f = Stardew.GetPlayer();
                this.Monitor.Log($"Loc: {Game1.currentLocation.Name} @ {(int)(f.getTileX())} x {(int)(f.getTileY())}", LogLevel.Alert);
            }
        }
    }
}