/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AeroCore.Utils
{
    [ModInit]
    public static class Misc
    {
        private static readonly PerScreen<List<Response>> PagedResponses = new(() => new());
        private static readonly PerScreen<int> PageIndex = new();
        private static readonly PerScreen<Action<Farmer, string>> PagedResponseConfirmed = new();
        private static readonly PerScreen<string> PagedQuestion = new();

        public static event Action NextGameTick;

        internal static void Init()
        {
            ModEntry.helper.Events.GameLoop.UpdateTicked += (s, e) =>
            {
                NextGameTick?.Invoke();
                NextGameTick = null;
            };
        }

        public static Point LocalToGlobal(int x, int y) => new(x + Game1.viewport.X, y + Game1.viewport.Y);
        public static Point LocalToGlobal(Point pos) => LocalToGlobal(pos.X, pos.Y);
        public static Vector2 LocalToGlobal(float x, float y) => new(x + Game1.viewport.X, y + Game1.viewport.Y);
        public static Vector2 LocalToGlobal(Vector2 pos) => LocalToGlobal(pos.X, pos.Y);
        public static IEnumerable<Point> PointsIn(this Rectangle rect)
        {
            for (int x = 0; x < rect.Width; x++)
                for (int y = 0; y < rect.Height; y++)
                    yield return new Point(x + rect.X, y + rect.Y);
        }
        public static IList<Point> AllPointsIn(this Rectangle rect)
        {
            var points = new Point[rect.Width * rect.Height];
            for (int x = 0; x < rect.Width; x++)
                for (int y = 0; y < rect.Height; y++)
                    points[x + y * rect.Width] = new(x + rect.X, y + rect.Y);
            return points;
        }
        public static bool IsFestivalAtLocation(string Location)
            => Location is not null && Game1.weatherIcon == 1 && Location.Equals(Game1.whereIsTodaysFest, StringComparison.OrdinalIgnoreCase);
        public static bool IsFestivalReady()
        {
            if (Game1.weatherIcon != 1)
                return true;

            string c = ModEntry.helper.GameContent.Load<Dictionary<string, string>>($"Data/Festivals/{Game1.currentSeason}{Game1.dayOfMonth}")["conditions"];

            return !int.TryParse(c.GetChunk('/', 1).GetChunk(' ',0), out int time) || time <= Game1.timeOfDay;
        }
        public static ReadOnlySpan<T> Concat<T>(this ReadOnlySpan<T> s1, ReadOnlySpan<T> s2)
        {
            var array = new T[s1.Length + s2.Length];
            s1.CopyTo(array);
            s2.CopyTo(array.AsSpan(s1.Length));
            return new(array);
        }
        public static void ShowPagedResponses(this GameLocation where, string question, List<KeyValuePair<string, string>> responses, Action<Farmer, string> on_response, bool auto_select_single = false)
        {
            if (responses.Count == 0)
                return;

            if (responses.Count == 1 && auto_select_single)
            {
                on_response(Game1.player, responses[0].Value);
                return;
            }

            List<Response> resp = new();
            foreach(var pair in responses)
                resp.Add(new(pair.Value, pair.Key)); 
            // flip-flopped to match 1.6. don't ask me I don't know.

            PagedResponses.Value = new(resp);
            PageIndex.Value = 0;
            PagedResponseConfirmed.Value = on_response;
            PagedQuestion.Value = question;

            ShowResponsePage(where);
        }
        private static void ShowResponsePage(GameLocation where)
        {
            List<Response> visible = new();
            if (PageIndex.Value > 0)
                visible.Add(new("_prevPage", '@' + ModEntry.i18n.Get("misc.generic.previous")));

            for(int i = PageIndex.Value * 5; i < PagedResponses.Value.Count; i++)
                visible.Add(PagedResponses.Value[i]);

            if (PagedResponses.Value.Count > (PageIndex.Value + 1) * 5)
                visible.Add(new("_nextPage", ModEntry.i18n.Get("misc.generic.next") + '>'));

            visible.Add(new("_cancel", ModEntry.i18n.Get("misc.generic.cancel")));

            where.createQuestionDialogue(PagedQuestion.Value, visible.ToArray(), HandlePagedResponse);
        }
        private static void HandlePagedResponse(Farmer who, string key)
        {
            if(key == "_nextPage" || key == "_prevPage")
            {
                if(key == "_nextPage")
                    PageIndex.Value++;
                else
                    PageIndex.Value--;
                ShowResponsePage(who.currentLocation);
            }
            else
            {
                PagedResponses.Value.Clear();
                PageIndex.Value = 0;
                PagedQuestion.Value = null;
                if(key != "_cancel")
                    PagedResponseConfirmed.Value(who, key);
                PagedResponseConfirmed.Value = null;
            }
        }
        public static string GetStringID(this Item item)
        {
            string ret = ModEntry.DGA?.GetDGAItemId(item);
            if (ret is not null)
                return ret;
            string index = item.ParentSheetIndex.ToString();
            string prefix =
                item is Wallpaper wp ? wp.isFloor.Value ? "(FL)" : "(WP)" :
                item is Clothing cl ? cl.clothesType.Value == 0 ? "(S)" : "(P)" :
                item is Boots ? "(B)" :
                item is Furniture ? "(F)" :
                item is MeleeWeapon ? "(W)" :
                item is Tool ? "(T)" :
                item is StardewValley.Object o ? o.bigCraftable.Value ? "(BC)" : "(O)" :
                "";
            return prefix + index;
        }
        public static bool TryLoadAsset<T>(IMonitor mon, IModHelper helper, string path, out T asset, LogLevel level = LogLevel.Warn)
        {
            try
            {
                asset = helper.GameContent.Load<T>(path);
            } catch(ContentLoadException e)
            {
                mon.Log(ModEntry.i18n.Get("misc.assetLoadFailed", new { Path = path, Msg = e.Message }), level);
                asset = default;
                return false;
            }
            return true;
        }
        public static IEnumerable<Building> GetAllBuildings()
        {
            if (!Game1.hasLoadedGame || Game1.getFarm() is null)
                return Array.Empty<Building>();

            return Game1.getFarm().buildings;
        }
        public static bool RemoveNamedItemsFromInventory(this Farmer who, string what, int count)
        {
            if (what is null)
                return false;
            List<Item> matched = new();
            int has = 0;
            foreach (var item in who.Items)
            {
                if (item is not null && item.Name == what)
                {
                    matched.Add(item);
                    has += item.Stack;
                }
            }
            if (has < count)
                return false;
            for (int i = 0; i < matched.Count && count > 0; i++)
            {
                var item = matched[i];
                var s = item.Stack;
                if (count >= item.Stack)
                    who.removeItemFromInventory(item);
                else
                    item.Stack -= count;
                count -= s;
            }
            return true;
        }
        public static bool RemoveItemsFromInventory(this Farmer who, Item what, int count = -1)
        {
            if (what is null)
                return false;
            if (count == -1)
                count = what.Stack;
            what = Patches.ItemWrapper.UnwrapItem(what);
            List<Item> matched = new();
            int has = 0;
            foreach (var item in who.Items) 
            {
                if (item is not null && what.canStackWith(item) && item.canStackWith(what)) 
                {
                    matched.Add(item); 
                    has += item.Stack;
                }
            }
            if (has < count)
                return false;
            for(int i = 0; i < matched.Count && count > 0; i++)
            {
                var item = matched[i];
                var s = item.Stack;
                if (count >= item.Stack)
                    who.removeItemFromInventory(item);
                else
                    item.Stack -= count;
                count -= s;
            }
            return true;
        }
        public static bool HasItemNamed(this Farmer who, string what, int count)
        {
            if (what is null)
                return false;
            what = what.Trim();
            int has = 0;
            foreach (var item in who.Items)
            {
                if (item is null)
                    continue;
                if (item.Name == what)
                    has += item.Stack;
                if (has >= count)
                    return true;
            }
            return false;
        }
        public static bool HasItem(this Farmer who, Item what, int count)
        {
            if (what is null)
                return false;
            what = Patches.ItemWrapper.UnwrapItem(what);
            int has = 0;
            foreach (var item in who.Items)
            {
                if (item is null)
                    continue;
                if (what.canStackWith(item) && item.canStackWith(what))
                    has += item.Stack;
                if (has >= count)
                    return true;
            }
            return false;
        }
        public static int Lerp(int from, int to, float amount)
            => from + (int)(amount * (to - from) + .5f);
        public static byte Lerp(byte from, byte to, float amount)
            => (byte)(from + (int)(amount * (to - from) + .5f));
        public static Color Interpolate(this Color from, Color to, float amount)
        {
            amount = Math.Clamp(amount, 0f, 1f);
            return new Color(
                Lerp(from.R, to.R, amount),
                Lerp(from.G, to.G, amount),
                Lerp(from.B, to.B, amount)
            ) * (Lerp(from.A, to.A, amount) / 255f);
        }
        public static bool TryGetCue(this ISoundBank soundBank, string name, out ICue cue)
        {
            try
            {
                cue = soundBank.GetCue(name);
            } catch
            {
                cue = null;
                return false;
            }
            return true;
        }
        public static bool TryGetCueDefinition(this ISoundBank soundBank, string name, out CueDefinition cue)
        {
            try
            {
                cue = soundBank.GetCueDefinition(name);
            } catch
            {
                cue = null;
                return false;
            }
            return true;
        }
        public static Rectangle Clamp(this Rectangle source, Rectangle bounds)
        {
            Point origin = new(
                Math.Clamp(source.X, bounds.Left, bounds.Right),
                Math.Clamp(source.Y, bounds.Top, bounds.Bottom));
            Point size = new(
                Math.Clamp(source.Width, bounds.Left - origin.X, bounds.Right - origin.X),
                Math.Clamp(source.Height, bounds.Top - origin.Y, bounds.Bottom - origin.Y));
            return new(origin, size);
        }
        public static Rectangle? Intersection(this Rectangle rect1, Rectangle rect2)
            => Math.Min(rect1.Right, rect2.Right) < Math.Max(rect1.Left, rect2.Left) ||
                Math.Min(rect1.Bottom, rect2.Bottom) < Math.Max(rect1.Top, rect2.Top)
                ? null : new(
                    Math.Max(rect1.Left, rect2.Left), Math.Min(rect1.Right, rect2.Right),
                    Math.Max(rect1.Top, rect2.Top), Math.Min(rect1.Bottom, rect2.Bottom)
                );
        public static int Volume(this Rectangle rect)
            => Math.Abs(rect.Width * rect.Height);
    }
}
