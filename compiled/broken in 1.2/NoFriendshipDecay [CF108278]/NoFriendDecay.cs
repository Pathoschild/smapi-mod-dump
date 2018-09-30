using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace NoFriendshipDecay
{
    public class NoFriendshipDecay : Mod
    {
        public override string Name
        {
            get
            {
                return "No Friendship Decay";
            }
        }

        public override string Authour
        {
            get
            {
                return "-";
            }
        }

        public override string Version
        {
            get
            {
                return "1.0";
            }
        }

        public override string Description
        {
            get
            {
                return "Prevents minor friendship decay on new day.";
            }
        }


        public override void Entry(params object[] objects)
        {
            Console.WriteLine("{0} loaded.", this.Name);
            TimeEvents.DayOfMonthChanged += Events_DayOfMonthChanged;
        }


        /*public void resetFriendshipsForNewDay()
		{
			string[] array = this.friendships.Keys.ToArray<string>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
                Game1.showGlobalMessage("Resetting friendships for : " + array2[i]);
				string text = array2[i];
				this.friendships[text][3] = 0;
				if (this.spouse != null && text.Equals(this.spouse) && !this.hasPlayerTalkedToNPC(text))
				{
					this.friendships[text][0] = Math.Max(this.friendships[text][0] - 20, 0);
				}
				if (this.hasPlayerTalkedToNPC(text))
				{
                    // reset talked to value
					this.friendships[text][2] = 0;
				}
				else if (!text.Equals("Shane") || this.friendships[text][0] < 2500)
				{
                    // if the NPC isn't shane.. OR the current level is < 2500, reduce it by 2, to a minimum of 0.
					this.friendships[text][0] = Math.Max(this.friendships[text][0] - 2, 0);
				}
                // Once it's been a week, we check if we've given them 2 gifts, if we have, we increase friendship level by 10.
				if (Game1.dayOfMonth % 7 == 0)
				{
                    // if we gave them 2 gifts this week..
					if (this.friendships[text][1] == 2)
					{
                        // increase our friendship by 10! or, keep it at 2749.
						this.friendships[text][0] = Math.Min(this.friendships[text][0] + 10, 2749);
					}
                    // reset gift counter.
					this.friendships[text][1] = 0;
				}
			}
		}*/

        public void Events_DayOfMonthChanged(object sender, EventArgs e)
        {
            string[] array = StardewValley.Game1.player.friendships.Keys.ToArray<string>();
			string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text = array2[i];
                if(StardewValley.Game1.player.spouse != null && text.Equals(StardewValley.Game1.player.spouse))
                {
                    StardewValley.Game1.player.friendships[text][0] += 20;
                }

                if (Game1.player.friendships[text][0] < 2500 && !Game1.player.hasTalkedToFriendToday(text))
                {
                    //Game1.showGlobalMessage("decay halt for : " + text);
                    Game1.player.friendships[text][0] += 2;
                }

            }
        }

    }
}
