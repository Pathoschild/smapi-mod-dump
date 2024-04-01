/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace AutomaticHammerSlam
{
    public class ModEntry : Mod
    {
        private Dictionary<int, int> MonsterHPMap = new Dictionary<int, int>();
        private bool ClubOnCooldown = false;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            MonsterHPMap.Clear();
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            LogDamageTaken();
            AutomaticHammerSlamSpam();
            PlaySoundNotificationForHammerCooldown();
        }

        private void LogDamageTaken()
        {
            foreach (NPC npc in Game1.currentLocation.characters)
            {
                if (!(npc is StardewValley.Monsters.Monster))
                {
                    continue;
                }

                StardewValley.Monsters.Monster monster = npc as StardewValley.Monsters.Monster;
                int hash = monster.GetHashCode();

                int? oldHP = null;
                if (MonsterHPMap.ContainsKey(hash))
                {
                    oldHP = MonsterHPMap[hash];
                }

                int currentHP = monster.Health;
                if (currentHP != oldHP && oldHP != null)
                {
                    int castedOldHP = oldHP ?? default(int);
                    int HPDifference = castedOldHP - currentHP;

                    Log($"{monster.Name} took damage: {HPDifference}");

                }

                MonsterHPMap[hash] = currentHP;
            }
        }

        private void AutomaticHammerSlamSpam()
        {
            if (IsPlayerSlammingHammer())
            {
                Game1.pressUseToolButton();
            }
        }

        private bool IsPlayerSlammingHammer()
        {
            if (!Game1.player.UsingTool)
            {
                return false;
            }

            if (!(Game1.player.CurrentTool is StardewValley.Tools.MeleeWeapon))
            {
                return false;
            }

            StardewValley.Tools.MeleeWeapon weapon = Game1.player.CurrentTool as StardewValley.Tools.MeleeWeapon;
            if (weapon.type.Value != StardewValley.Tools.MeleeWeapon.club)
            {
                return false;
            }

            return weapon.isOnSpecial;
        }

        private void PlaySoundNotificationForHammerCooldown()
        {
            int clubCooldown = StardewValley.Tools.MeleeWeapon.clubCooldown;

            if (clubCooldown > 0)
            {
                ClubOnCooldown = true;
                return;
            }

            if (ClubOnCooldown)
            {
                ClubOnCooldown = false;
                Game1.playSound("cowboy_powerup");
            }
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
