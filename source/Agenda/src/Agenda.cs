/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/StardewValley-Agenda
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.TokenizableStrings;
using static StardewValley.Menus.Billboard;
using System.Linq;
using StardewValley.GameData.Characters;
using System.Reflection;

namespace MyAgenda
{
    public class Agenda : IClickableMenu
    {
        public static Agenda Instance;
        public static AgendaPage agendaPage;
        public static Texture2D agendaTexture, buttonTexture, triggerTexture;
        public static ClickableTextureComponent prev, next, hover;
        public static string[,] pageTitle, pageBirthday, pageFestival, pageNote, titleSubsitute;
        public static string[] triggerTitle, triggerNote;
        public static int season;
        public static int[][] triggerValue;
        public static Rectangle[] bounds, triggerBounds, triggerValueBounds;
        public static Rectangle hoverBounds;
        public static IMonitor monitor;
        public static IModHelper helper;
        public static string hoverText = "";

        public Agenda(IModHelper helper) : base()
        {
            Game1.mouseCursorTransparency = 1f;
            if (Game1.gameMode == 3 && Game1.player != null && !Game1.eventUp)
            {
                Game1.player.Halt();
            }
            if (Game1.player != null && !Game1.player.UsingTool && !Game1.eventUp)
            {
                Game1.player.forceCanMove();
            }

            Agenda.helper = helper;
            agendaTexture = helper.ModContent.Load<Texture2D>("assets\\Agenda");
            buttonTexture = helper.ModContent.Load<Texture2D>("assets\\buttons");
            triggerTexture = helper.ModContent.Load<Texture2D>("assets\\trigger");

            pageTitle = helper.Data.ReadSaveData<string[,]>("title");
            titleSubsitute = new string[4, 28];
            pageBirthday = new string[4, 28];
            pageFestival = new string[4, 28];
            pageNote = helper.Data.ReadSaveData<string[,]>("notes");
            triggerTitle = helper.Data.ReadSaveData<string[]>("trigger_title");
            triggerNote = helper.Data.ReadSaveData<string[]>("trigger_notes");
            triggerValue = helper.Data.ReadSaveData<int[][]>("triggers");

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    pageBirthday[i, j] = "";
                    pageFestival[i, j] = "";
                    titleSubsitute[i, j] = "";
                }
            }

            if (pageTitle == null)
            {
                pageTitle = new string[4, 28];
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        pageTitle[i, j] = "";
                    }
                }
            }
            if (pageNote == null)
            {
                pageNote = new string[4, 28];
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        pageNote[i, j] = "";
                    }
                }
            }
            if (triggerNote == null) 
            {
                triggerNote = new string[14];
                for (int j = 0; j < 14; j++)
                {
                    triggerNote[j] = "";
                }
            }
            if (triggerTitle == null)
            {
                triggerTitle = new string[14];
                for (int j = 0; j < 14; j++)
                {
                    triggerTitle[j] = "";
                }
            }
            if(triggerValue == null)
            {
                triggerValue = new int[14][];
                for (int i = 0; i < 14; i++)
                {
                    triggerValue[i] = new int[3];
                    for (int j = 0; j < 3; j++)
                    {
                        triggerValue[i][j] = 0;
                    }
                }
            }
            bounds = new Rectangle[28];
            triggerBounds = new Rectangle[14];
            triggerValueBounds = new Rectangle[14];

            season = Utility.getSeasonNumber(Game1.currentSeason);
            width = 316 * 4;
            height = 230 * 4;
            Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            xPositionOnScreen = (int)topLeftPositionForCenteringOnScreen.X;
            yPositionOnScreen = (int)topLeftPositionForCenteringOnScreen.Y;
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
            prev = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 875, yPositionOnScreen + 50, 96, 48), buttonTexture, new Rectangle(0, 24, 48, 24), 2f);
            next = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 971, yPositionOnScreen + 50, 96, 48), buttonTexture, new Rectangle(0, 0, 48, 24), 2f);
            hoverBounds = new Rectangle(-200, -100, 48 * 4, 24 * 4);
            hover = new ClickableTextureComponent(hoverBounds, buttonTexture, new Rectangle(0, 48, 48, 24), 2f);
            for (int i = 0; i < 28; i++)
            {
                bounds[i] = new Rectangle(xPositionOnScreen + (i) % 7 * 40 * 4 + 75, yPositionOnScreen + 220 + (i) / 7 * 40 * 4, 38 * 4, 38 * 4);
            }
            for (int i = 0; i < 14; i++)
            {
                triggerBounds[i] = new Rectangle(xPositionOnScreen + (i) % 7 * 40 * 4 + 75, yPositionOnScreen + 220 + (i) / 7 * 80 * 4, 38 * 4, 48 * 4);
            }
            for (int i = 0; i < 14; i++)
            {
                triggerValueBounds[i] = new Rectangle(xPositionOnScreen + (i) % 7 * 40 * 4 + 75, yPositionOnScreen + 220 + 50 * 4 + (i) / 7 * 80 * 4, 38 * 4, 28 * 4);
            }
            
            foreach (Season season in Enum.GetValues<Season>())
            {
                Dictionary<int, List<NPC>> birthdays = GetBirthdays(season);
                foreach (KeyValuePair<int, List<NPC>> p in birthdays)
                {
                    pageBirthday[Utility.getSeasonNumber(Utility.getSeasonKey(season)), p.Key - 1] = p.Value[0].displayName;
                    string str = "";
                    foreach (NPC npc in p.Value)
                    {
                        str += npc.displayName + ", ";
                    }
                    str = str.Substring(0, str.Length - 2);
                    titleSubsitute[Utility.getSeasonNumber(Utility.getSeasonKey(season)), p.Key - 1] += helper.Translation.Get("birthday_title", new { character = str });
                }
                for (int day = 1; day <= 28; day++)
                {
                    List<BillboardEvent> events = GetEventsForDay(day, season);
                    foreach (BillboardEvent e in events)
                    {
                        pageFestival[Utility.getSeasonNumber(Utility.getSeasonKey(season)), day - 1] = e.DisplayName;
                        titleSubsitute[Utility.getSeasonNumber(Utility.getSeasonKey(season)), day - 1] += e.DisplayName; 
                    }
                }  
            }

            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                snapToDefaultClickableComponent();
            }
        }

        public virtual Dictionary<int, List<NPC>> GetBirthdays(Season season)
        {
            HashSet<string> addedBirthdays = new HashSet<string>();
            Dictionary<int, List<NPC>> birthdays = new Dictionary<int, List<NPC>>();
            Utility.ForEachVillager(delegate (NPC npc)
            {
                if (npc.Birthday_Season != Utility.getSeasonKey(season))
                {
                    return true;
                }

                CalendarBehavior? calendarBehavior = npc.GetData()?.Calendar;
                if (calendarBehavior == CalendarBehavior.HiddenAlways || (calendarBehavior == CalendarBehavior.HiddenUntilMet && !Game1.player.friendshipData.ContainsKey(npc.Name)))
                {
                    return true;
                }

                if (addedBirthdays.Contains(npc.Name))
                {
                    return true;
                }

                if (!birthdays.TryGetValue(npc.Birthday_Day, out var value))
                {
                    value = (birthdays[npc.Birthday_Day] = new List<NPC>());
                }

                value.Add(npc);
                addedBirthdays.Add(npc.Name);
                return true;
            });
            return birthdays;
        }

        public virtual List<BillboardEvent> GetEventsForDay(int day, Season season)
        {
            List<BillboardEvent> list = new List<BillboardEvent>();
            if (Utility.isFestivalDay(day, season))
            {
                string text = Utility.getSeasonKey(season) + day;
                string displayName = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + text)["name"];
                list.Add(new BillboardEvent(BillboardEventType.Festival, new string[1] { text }, displayName));
            }

            if (Utility.TryGetPassiveFestivalDataForDay(day, season, null, out var id, out var data, ignoreConditionsCheck: true) && (data?.ShowOnCalendar ?? false))
            {
                string displayName2 = TokenParser.ParseText(data.DisplayName);
                if (!GameStateQuery.CheckConditions(data.Condition))
                {
                    list.Add(new BillboardEvent(BillboardEventType.PassiveFestival, new string[1] { id }, "???")
                    {
                        locked = true
                    });
                }
                else
                {
                    list.Add(new BillboardEvent(BillboardEventType.PassiveFestival, new string[1] { id }, displayName2));
                }
            }

            if (season == Season.Summer && (day == 20 || day == 21))
            {
                string displayName3 = Game1.content.LoadString("Strings\\1_6_Strings:TroutDerby");
                list.Add(new BillboardEvent(BillboardEventType.FishingDerby, Array.Empty<string>(), displayName3));
            }
            else if (season == Season.Winter && (day == 12 || day == 13))
            {
                string displayName4 = Game1.content.LoadString("Strings\\1_6_Strings:SquidFest");
                list.Add(new BillboardEvent(BillboardEventType.FishingDerby, Array.Empty<string>(), displayName4));
            }

            if (getDaysOfBooksellerThisSeason(Utility.getSeasonNumber(Utility.getSeasonKey(season))).Contains(day))
            {
                string displayName5 = Game1.content.LoadString("Strings\\1_6_Strings:Bookseller");
                list.Add(new BillboardEvent(BillboardEventType.Bookseller, Array.Empty<string>(), displayName5));
            }

            HashSet<Farmer> hashSet = new HashSet<Farmer>();
            FarmerCollection onlineFarmers = Game1.getOnlineFarmers();
            foreach (Farmer item2 in onlineFarmers)
            {
                if (hashSet.Contains(item2) || !item2.isEngaged() || item2.hasCurrentOrPendingRoommate())
                {
                    continue;
                }

                string text2 = null;
                WorldDate worldDate = null;
                NPC characterFromName = Game1.getCharacterFromName(item2.spouse);
                if (characterFromName != null)
                {
                    worldDate = item2.friendshipData[item2.spouse].WeddingDate;
                    text2 = characterFromName.displayName;
                }
                else
                {
                    long? spouse = item2.team.GetSpouse(item2.UniqueMultiplayerID);
                    if (spouse.HasValue)
                    {
                        Farmer farmerMaybeOffline = Game1.getFarmerMaybeOffline(spouse.Value);
                        if (farmerMaybeOffline != null && onlineFarmers.Contains(farmerMaybeOffline))
                        {
                            worldDate = item2.team.GetFriendship(item2.UniqueMultiplayerID, spouse.Value).WeddingDate;
                            hashSet.Add(farmerMaybeOffline);
                            text2 = farmerMaybeOffline.Name;
                        }
                    }
                }

                if (!(worldDate == null))
                {
                    if (worldDate.TotalDays < Game1.Date.TotalDays)
                    {
                        worldDate = new WorldDate(Game1.Date);
                        worldDate.TotalDays++;
                    }

                    if (worldDate?.TotalDays >= Game1.Date.TotalDays && day == worldDate.DayOfMonth)
                    {
                        list.Add(new BillboardEvent(BillboardEventType.Wedding, new string[2] { item2.Name, text2 }, Game1.content.LoadString("Strings\\UI:Calendar_Wedding", item2.Name, text2)));
                        hashSet.Add(item2);
                    }
                }
            }

            return list;
        }

        // copied and moldified from Utility.getDayOfBooksellerThisSeason::List<int>
        public static List<int> getDaysOfBooksellerThisSeason(int season)
        {
            Random random = Utility.CreateRandom(Game1.year * 11, Game1.uniqueIDForThisGame, season);
            int[] array = null;
            List<int> list = new List<int>();
            switch (season)
            {
                case 0:
                    array = new int[5] { 11, 12, 21, 22, 25 };
                    break;
                case 1:
                    array = new int[5] { 9, 12, 18, 25, 27 };
                    break;
                case 2:
                    array = new int[8] { 4, 7, 8, 9, 12, 19, 22, 25 };
                    break;
                case 3:
                    array = new int[6] { 5, 11, 12, 19, 22, 24 };
                    break;
            }

            int num = random.Next(array.Length);
            list.Add(array[num]);
            list.Add(array[(num + array.Length / 2) % array.Length]);
            return list;
        }

        public static int[] getDate(String date)
        {
            for (int i = 0; i < date.Length; i++)
            {
                if (date[i] < 'a')
                {
                    return new int[] { Utility.getSeasonNumber(date.Substring(0, i)), int.Parse(date.Substring(i)) - 1 };
                }
            }
            return null;
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Game1.playSound("bigDeSelect");
            exitThisMenu();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if(next.containsPoint(x, y))
            {
                season ++;
                season %= 5;
                return;
            }

            if (prev.containsPoint(x, y))
            {
                season+=4;
                season %= 5;
                return;
            }

            if(season == 4)
            {
                for(int i = 0; i < 14; i++)
                {
                    if (triggerBounds[i].Contains(x, y))
                    {
                        exitThisMenu();
                        Game1.keyboardDispatcher.Subscriber = Trigger.tbox;
                        Trigger.note = triggerNote[i];
                        Trigger.title = triggerTitle[i];
                        Trigger.note_back = "";
                        Trigger.title_back = "";
                        Trigger.indexOnPage = i;
                        Trigger.selectedTrigger = triggerValue[i];
                        Trigger.selected = 0;
                        Game1.activeClickableMenu = Trigger.Instance;
                        return;
                    }
                }
                return;
            }

            for (int i = 0; i < 28; i++)
            {
                if (bounds[i].Contains(x, y))
                {
                    exitThisMenu();
                    Game1.keyboardDispatcher.Subscriber = AgendaPage.tbox;
                    agendaPage.note = pageNote[season, i];
                    agendaPage.title = pageTitle[season, i];
                    agendaPage.title_back = "";
                    agendaPage.note_back = "";
                    agendaPage.subsituteTitle = titleSubsitute[season, i];
                    agendaPage.birthday = pageBirthday[season, i];
                    agendaPage.festival = pageFestival[season, i];
                    agendaPage.season = season;
                    agendaPage.day = i + 1;
                    Game1.activeClickableMenu = agendaPage;
                    return;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            hoverText = "";

            if(prev.containsPoint(x, y))
            {
                hover.bounds = prev.bounds;
            }
            else if(next.containsPoint(x, y))
            {
                hover.bounds = next.bounds;
            }
            else
            {
                hover.bounds = hoverBounds;
            }

            if (bounds == null)
            {
                return;
            }

            if(season == 4)
            {
                for(int i = 0; i < 14; i++)
                {
                    if (triggerBounds[i].Contains(x, y))
                    {
                        hoverText = triggerNote[i].Substring(0, Math.Min(triggerNote[i].Length, 20));
                        return;
                    }
                }
                return;
            }

            for (int i = 0; i < 28; i++)
            {
                if (bounds[i].Contains(x, y))
                {
                    hoverText = pageNote[season, i].Substring(0, Math.Min(pageNote[season, i].Length, 20));
                    return;
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            if(season != 4)
            {
                b.Draw(agendaTexture, new Vector2(xPositionOnScreen, yPositionOnScreen - 226 * 4), new Rectangle(0, 0, 316, 456), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                b.DrawString(Game1.dialogueFont, Utility.getSeasonNameFromNumber(season), new Vector2(xPositionOnScreen + 160, yPositionOnScreen + 80), Game1.textColor);
                for (int i = 0; i < 28; i++)
                {
                    Util.drawStr(b, (pageTitle[season, i] == "" ? titleSubsitute[season, i] : pageTitle[season, i]), bounds[i], Game1.dialogueFont);

                    if (season != Utility.getSeasonNumber(Game1.currentSeason)) { continue; }

                    if (Game1.dayOfMonth > i + 1)
                    {
                        b.Draw(Game1.staminaRect, bounds[i], Color.Gray * 0.25f);
                    }
                    else if (Game1.dayOfMonth == i + 1)
                    {
                        int num = (int)(4f * Game1.dialogueButtonScale / 8f);
                        drawTextureBox(b, Game1.mouseCursors, new Rectangle(379, 357, 3, 3), bounds[i].X - num, bounds[i].Y - num, bounds[i].Width + num * 2, bounds[i].Height + num * 2, Color.Blue, 4f, drawShadow: false);
                    }
                }
            }
            else
            {
                b.DrawString(Game1.dialogueFont, helper.Translation.Get("trigger"), new Vector2(xPositionOnScreen + 160, yPositionOnScreen + 80), Game1.textColor);
                b.Draw(triggerTexture, new Vector2(xPositionOnScreen, yPositionOnScreen - 226 * 4), new Rectangle(0, 0, 316, 456), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                for (int i = 0; i < 14; i++)
                {
                    Util.drawStr(b, (triggerTitle[i] == "" ? helper.Translation.Get("subsitute") : triggerTitle[i]), triggerBounds[i], Game1.dialogueFont);
                    drawTrigger(b, i);
                }
            }
            

            base.draw(b);
            prev.draw(b);
            next.draw(b);
            hover.draw(b);
            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
            if (hoverText.Length > 0)
            {
                drawHoverText(b, hoverText, Game1.dialogueFont);
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            width = 316 * 4;
            height = 230 * 4;
            Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            xPositionOnScreen = (int)topLeftPositionForCenteringOnScreen.X;
            yPositionOnScreen = (int)topLeftPositionForCenteringOnScreen.Y;
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
            prev = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 875, yPositionOnScreen + 50, 96, 48), buttonTexture, new Rectangle(0, 24, 48, 24), 2f);
            next = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 971, yPositionOnScreen + 50, 96, 48), buttonTexture, new Rectangle(0, 0, 48, 24), 2f);
            hoverBounds = new Rectangle(-200, -100, 48 * 4, 24 * 4);
            hover = new ClickableTextureComponent(hoverBounds, buttonTexture, new Rectangle(0, 48, 48, 24), 2f);
            for (int i = 0; i < 28; i++)
            {
                bounds[i] = new Rectangle(xPositionOnScreen + (i) % 7 * 40 * 4 + 75, yPositionOnScreen + 224 + (i) / 7 * 40 * 4, 40 * 4, 40 * 4);
            }
            for (int i = 0; i < 14; i++)
            {
                triggerBounds[i] = new Rectangle(xPositionOnScreen + (i) % 7 * 40 * 4 + 75, yPositionOnScreen + 220 + (i) / 7 * 80 * 4, 38 * 4, 48 * 4);
            }
            for (int i = 0; i < 14; i++)
            {
                triggerValueBounds[i] = new Rectangle(xPositionOnScreen + (i) % 7 * 40 * 4 + 75, yPositionOnScreen + 220 + 50 * 4 + (i) / 7 * 80 * 4, 38 * 4, 28 * 4);
            }
        }

        public static void save(int season, int day)
        {
            pageTitle[season, day] = agendaPage.title + agendaPage.title_back;
            pageNote[season, day] = agendaPage.note + agendaPage.note_back;
            agendaPage.selected = 0;
            AgendaPage.tbox.Text = "0";
        }

        public static void write(IModHelper helper)
        {
            helper.Data.WriteSaveData("title", pageTitle);
            helper.Data.WriteSaveData("notes", pageNote);
            helper.Data.WriteSaveData("trigger_title", triggerTitle);
            helper.Data.WriteSaveData("trigger_notes", triggerNote);
            helper.Data.WriteSaveData("triggers", triggerValue);
        }

        public static void drawTrigger(SpriteBatch b, int index)
        {
            Rectangle triggerBox = triggerValueBounds[index];
             
            Vector2 lastPosition = Util.drawStr(b, Trigger.choices[Trigger.renderOrder[0]][triggerValue[index][Trigger.renderOrder[0]]], triggerBox, Game1.smallFont);
            triggerBox.Y = (int)lastPosition.Y;
            triggerBox.Height = (int)(triggerValueBounds[index].Height + triggerValueBounds[index].Y - lastPosition.Y);
            lastPosition = Util.drawStr(b, Trigger.choices[Trigger.renderOrder[1]][triggerValue[index][Trigger.renderOrder[1]]], triggerBox, Game1.smallFont, (int)lastPosition.X - triggerBox.X);
            triggerBox.Y = (int)lastPosition.Y;
            triggerBox.Height = (int)(triggerValueBounds[index].Height + triggerValueBounds[index].Y - lastPosition.Y);
            Util.drawStr(b, Trigger.choices[Trigger.renderOrder[2]][triggerValue[index][Trigger.renderOrder[2]]], triggerBox, Game1.smallFont, (int)lastPosition.X - triggerBox.X);
        }

        public static bool hasSomethingToDo(int season, int day)
        {
            return pageTitle[season, day] != "" || titleSubsitute[season, day] != "" || pageNote[season, day] != "";
        }
    }
}
