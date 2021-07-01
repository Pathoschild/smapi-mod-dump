/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace ThrowableAxe
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        private ThrownAxe thrown;
        private bool clicking = false;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = Monitor;

            helper.Events.Input.ButtonPressed += onButtonPress;
            helper.Events.Input.ButtonReleased += onButtonRelease;
            helper.Events.GameLoop.UpdateTicking += onUpdateTicking;
            helper.Events.Player.Warped += onWarped;
        }

        private void onButtonPress(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == SButton.MouseRight && Game1.player.CurrentTool is Axe axe && thrown == null)
            {
                int[] dmg_ = new int[] { 8, 15, 30, 45, 60, 80 }; // 6 for support for prismatic tools
                float[] speed_ = new float[] { 10, 12, 14, 16, 18, 20 }; // 6 for support for prismatic tools
                int dmg = dmg_[axe.UpgradeLevel];
                float speed = speed_[axe.UpgradeLevel];
                
                thrown = new ThrownAxe(Game1.player, axe.UpgradeLevel, dmg, e.Cursor.AbsolutePixels, speed);
                Game1.currentLocation.projectiles.Add(thrown);

                Log.trace("Throwing axe");
                clicking = true;
            }
        }

        private void onButtonRelease(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button == SButton.MouseRight)
            {
                clicking = false;
            }
        }

        private void onUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (thrown != null)
            {
                /*
                if(clicking)
                {
                    thrown.target.Value = Helper.Input.GetCursorPosition().AbsolutePixels;
                }
                */
                if (!clicking || (thrown.GetPosition() - thrown.target).Length() < 1)
                {
                    var playerPos = Game1.player.getStandingPosition();
                    playerPos.X -= 16;
                    playerPos.Y -= 64;
                    thrown.target.Value = playerPos;
                    if ( (thrown.GetPosition() - playerPos).Length() < 16 )
                    {
                        thrown.dead = true;
                    }
                }

                if (thrown.dead)
                {
                    Log.trace("Axe destroyed");
                    thrown = null;
                }
            }
        }

        private void onWarped(object sender, WarpedEventArgs e)
        {
            if (thrown != null)
            {
                thrown.dead = true;
                thrown = null;
            }
        }
    }

}
