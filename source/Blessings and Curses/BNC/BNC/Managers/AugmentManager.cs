/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using BNC.Managers.Augments;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BNC.Managers
{
    public class AugmentManager
    {

        public String CurrentAugment = null;
        public bool finishedLoop = false;

        public Dictionary<String, BaseAugment> _nameToAugments = new Dictionary<string, BaseAugment>();

        private ConcurrentQueue<BaseAugment> _augmentQueue = new ConcurrentQueue<BaseAugment>();

        public List<Monster> queue = new List<Monster>();
        private DateTime startTime = DateTime.Now;


        public void Init()
        {
            AddAugment(new Tired().setChatId("tired"));
            AddAugment(new Crabs().setChatId("crabs"));
            AddAugment(new Extra().setChatId("extra"));
            AddAugment(new Harder().setChatId("harder"));
            AddAugment(new Exp().setChatId("exp"));
            AddAugment(new Exp().setBoostAmount(2).setDisplayName("Monsters Extra Extra Exp!").setDescription("Monster have 2x more experiance.").setChatId("moreexp"));
            AddAugment(new Health().setChatId("health"));
            AddAugment(new Health().setBoostAmount(2).setChatId("morehealth").setDisplayName("Monster Health Increase More!").setDescription("Monster have 2x more health."));
            AddAugment(new Regen().setChatId("regen"));
            AddAugment(new Speed().setChatId("speed"));
            AddAugment(new Stamina().setChatId("stamina"));
            AddAugment(new Trolls().setChatId("troll"));
            AddAugment(new Phasing().setChatId("phasing"));

            BNC_Core.helper.Events.Player.Warped += mineLevelChanged;

            BNC_Core.helper.Events.Display.RenderedHud += GraphicsEvents_OnPostRenderHudEvent;
        }


        private void AddAugment(BaseAugment augment)
        {
            var type = augment.id;
            _nameToAugments.Add(type, augment);
            BNC_Core.Logger.Log($"Added Augment: {type}", LogLevel.Debug);
        }


        public void AddAugmentQueue(BaseAugment augment)
        {
            _augmentQueue.Enqueue(augment);
        }

        // Handle when Mining Level Changes
        public void mineLevelChanged(object sender, WarpedEventArgs e)
        {

           //BNC_Core.Logger.Log("Warped", LogLevel.Info);
            if (e.NewLocation is MineShaft mine)
            {
                if (shouldUpdateAugment(e))
                //if (true)
                {
                    if (TwitchIntergration.isConnected())
                    {
                        //BNC_Core.Logger.Log("connected", LogLevel.Info);
                        if (!TwitchIntergration.hasAugPollStarted)
                            TwitchIntergration.StartAugPoll(getRandomBuff(3, true));
                    }
                    else
                    {
                        BaseAugment aug = getRandomBuff(1)[0];
                        aug.isMineOnly = true;
                        AddAugmentQueue(aug);
                        Game1.addHUDMessage(new HUDMessage(aug.DisplayName + ": " + aug.desc, null));
                    }
                }
            }

            if (_augmentQueue.Count() > 0)
                UpdateLocation(e);
        }


        // Used to get a random augment
        public BaseAugment[] getRandomBuff(int count, bool isMineOnly = false)
        {
            List<BaseAugment> list = Enumerable.ToList(_nameToAugments.Values);
            List<BaseAugment> returnList = new List<BaseAugment>();

            while (returnList.Count < 3)
            {
                int num = Game1.random.Next(list.Count);
                BaseAugment item = list[num];
                item.isMineOnly = isMineOnly;
                if (item != null && !returnList.Contains(item))
                    returnList.Add(item);
            }
            return returnList.ToArray();
        }


        public bool shouldUpdateAugment(WarpedEventArgs e)
        {
            if (e.NewLocation is MineShaft mine)
            {
                if (lastAugment() && (mine.mineLevel % BNC_Core.config.Mine_Augment_Every_x_Levels == 0 || CurrentAugment == null || mine.mineLevel == 1))
                {
                    startTime = DateTime.Now;
                    return true;
                }
            }
            return false;
        }

        public bool lastAugment()
        {
            if (DateTime.Now > startTime.AddSeconds(120))
                return true;
            return false;
        }

        public void UpdateLocation(WarpedEventArgs e)
        {

            Actions.SpawnCat.tryMoveCats();

            foreach (var aug in _augmentQueue.ToArray())
            {
                aug.WarpLocation(e);
            }

            foreach (NPC npc in Game1.player.currentLocation.characters)
            {
                if (!(npc is Monster)) continue;
                Monster m = (Monster)npc;

                foreach (var aug in _augmentQueue.ToArray())
                    aug.UpdateMonster(e, m);
            }
        }

        public void UpdateTick()
        {
            GameLocation location = Game1.player.currentLocation;

            bool monsterTick = false;
            foreach (var aug in _augmentQueue.ToArray()) {
                if (aug.hasMonsterTick)
                    monsterTick = true;
                aug.PlayerTickUpdate();

                if (aug.isMineOnly && !(location is MineShaft mine))
                    aug.markForRemoval();
            }

            if (monsterTick)
                MonsterTick();

            if (!_augmentQueue.TryDequeue(out var check)) return;

            if (!check.shouldRemove())
            {
                _augmentQueue.Enqueue(check);
            }
            else if(check.shouldRemove())
            {
                //BNC_Core.Logger.Log($"Removing {check.DisplayName}", LogLevel.Debug);
                check.OnRemove();
            }
        }

        private void MonsterTick()
        {
            foreach (NPC npc in Game1.player.currentLocation.characters)
            {
                if (!(npc is Monster)) continue;
                Monster m = (Monster)npc;
                foreach (var aug in _augmentQueue.ToArray())
                {
                    if (aug.hasMonsterTick)
                        aug.MonsterTickUpdate(m);
                }
            }
        }


        private void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            if (_augmentQueue.Count() == 0) return;


            int num1 = 64;
            SpriteFont smallFont = Game1.smallFont;
            SpriteBatch spriteBatch = Game1.spriteBatch;
            String[] currentDisplayText = new String[_augmentQueue.Count()];
            int i = 0;
            foreach (var item in _augmentQueue.ToArray())
            {
                currentDisplayText[i++] = item.DisplayName;
            }
            Vector2 vector2 = smallFont.MeasureString("");
            foreach (String str in currentDisplayText)
            {
                Vector2 temp = smallFont.MeasureString(str);
                if (temp.X > vector2.X)
                    vector2 = temp;
            }
            Array.Sort(currentDisplayText, (a, b) => String.Compare(a, b));

            int num2 = num1 / 2;
            int width = (int)((double)vector2.X + (double)num2) + 65;
            int height = Math.Max(60, 60 + 35 * (currentDisplayText.Count() - 1));
            int x = 0;
            int y = 0;
                //                   y = (Game1.viewport.Height/2)  - (height/2);
            if (x + width > Game1.viewport.Width)
            {
                x = Game1.viewport.Width - width;
                y += num2;
            }
            if (y + height > Game1.viewport.Height)
            {
                x += num2;
                y = Game1.viewport.Height - height;
            }
            x += 20;
            int cnt = 0;
            foreach (String str in currentDisplayText)
            {
                Utility.drawTextWithShadow(spriteBatch, str, smallFont, new Vector2((float)(x + num1 / 4) + 5, (float)(y + num1 / 4) + (cnt * vector2.Y)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                cnt++;
            }
        }

        public void Clear()
        {
            foreach (var item in this._augmentQueue.ToArray())
            {
                item.markForRemoval();
            }
            
        }
    }
}

